using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DataPipeline.Functions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServerlessDataPipeline.Configuration;
using ServerlessDataPipeline.Model;
using ServerlessDataPipeline.Services;

namespace ServerlessDataPipeline
{
    public static class HeatReadingsBlobTrigger
    {
        private static readonly HttpClient httpClient = new HttpClient();

        [FunctionName("HeatReadingsBlobTrigger")]
        public static async Task RunAsync(
            [BlobTrigger("eh-hourly-data/{name}", Connection = "BlobStorageConnectionString")] Stream myBlobStream,
            string name,
            ILogger log)
        {
            log.LogInformation($"C# HeatReadingsBlobTrigger executed at: {DateTime.Now}");

            var configuration = new CloudBlobConfiguration();
            var clouldBlobClient = configuration.InitializeFromEnvironment();

            var avroFileService = new AvroFileService<EventData<HeatReading>>();
            var timeSeriesBlobService = new TimeSeriesBlobService<HeatReading>(clouldBlobClient);

            await DoRun(avroFileService, timeSeriesBlobService, myBlobStream).ConfigureAwait(false);
        }

        private static async Task DoRun(AvroFileService<EventData<HeatReading>> avroFileService,
            TimeSeriesBlobService<HeatReading> timeSeriesBlobService, Stream myBlobStream)
        {
            var periodHourDocuments = avroFileService.Decompress(myBlobStream);

            var groupedDocuments = periodHourDocuments.GroupBy(x =>
                new
                {
                    x.Body.ClientId,
                    x.Body.TimeStamp.Year,
                    x.Body.TimeStamp.Month,
                    x.Body.TimeStamp.Day,
                    x.Body.TimeStamp.Hour
                }).Select(g => new
            {
                CustomerId = g.Key.ClientId,
                DailyFolderPath = $"{Constants.TimeSeriesCollectionName}/" +
                                  $"{g.Key.Year}/{g.Key.Month:D2}/{g.Key.Day:D2}",
                TimedFormatedBlobName = $"{g.Key.Year}_{g.Key.Month:D2}_{g.Key.Day:D2}_{g.Key.Hour:D2}.json",
                Readings = g.Select(x => x.Body),
                Outliers = g.Select(x => x.Body).Where(r => r.Value > 0.5)
            });

            foreach (var group in groupedDocuments)
            {
                await timeSeriesBlobService.AppendLines(
                    group.CustomerId,
                    $"{group.DailyFolderPath}/{group.TimedFormatedBlobName}",
                    group.Readings)
                        .ConfigureAwait(false);

                if (!group.Outliers.Any()) continue;

                // Notify Outlier
                var stringContent = new StringContent(JsonConvert.SerializeObject(group.Outliers), Encoding.UTF8, "application/json");
                await httpClient
                    .PostAsync("http://localhost:7071/api/api/v1/clients/{clientId}/notify_outlier",
                        stringContent).ConfigureAwait(false);
            }
        }
    }
}
