using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace Analytics.PeriodHourIngest.PeriodHourReadingsProcessorService.Services
{
    public class TimeSeriesBlobService<T> : ITimeSeriesBlobService<T>
    {
        private readonly CloudBlobClient _blobClient;

        public TimeSeriesBlobService(CloudBlobClient blobClient)
        {
            _blobClient = blobClient;
        }

        public async Task AppendLines(Guid customerId, string blobName,
            IEnumerable<T> lines)
        {
            var containerName = customerId.ToString();
            var content = CreateJsonLines(lines);

            var container = _blobClient.GetContainerReference(containerName);

            await container.CreateIfNotExistsAsync().ConfigureAwait(false);

            var appendBlob = container.GetAppendBlobReference(blobName);
            appendBlob.Properties.ContentType = "application/json";

            var exists = await appendBlob.ExistsAsync().ConfigureAwait(false);
            
            if (!exists)
            {
                await appendBlob.CreateOrReplaceAsync();
            }

            await appendBlob.AppendTextAsync(content);
        }

        private string CreateJsonLines(IEnumerable<T> lines)
        {
            var appender = new StringBuilder();
            foreach (var line in lines)
            {
                appender.AppendLine(JsonConvert.SerializeObject(line, Formatting.None));
            }

            return appender.ToString();
        }
    }
}
