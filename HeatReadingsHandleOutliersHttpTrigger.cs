using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServerlessDataPipeline.Model;

namespace ServerlessDataPipeline
{
    public static class HeatReadingsHandleOutliersHttpTrigger
    {
        [FunctionName("HeatReadingsHandleOutliersHttpTrigger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(
                authLevel: AuthorizationLevel.Function,
                methods: new [] {"POST"},
                Route = "api/v1/clients/{clientId}/notify_outlier")] HttpRequest req
            , string clientId
            , [ServiceBus(
                Configuration.Constants.OutlierReceivedTopicName,
                EntityType = EntityType.Topic,
                Connection = Configuration.Constants.ServiceBusPeriodHourConnectionString)] IAsyncCollector<Outlier> collector
            , ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync().ConfigureAwait(false);
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            double value = data?.Value;
            OutlierType outlierType = data?.OutlierType;
            DateTime timeStamp = data?.Timestamp;

            // Business logic to filter goes here
            if (!ShouldAlert(outlierType, timeStamp))
                return new OkResult();

            // Alert
            var outlier = new Outlier
            {
                ClientId = new Guid(clientId),
                Value = value,
                OutlierType = outlierType
            };
            await collector.AddAsync(outlier).ConfigureAwait(false);

            return new OkObjectResult(clientId);
        }

        private static bool ShouldAlert(OutlierType outlierType, DateTime timeStamp)
        {
            return outlierType.IsUpperOutlier() && timeStamp.Hour == PeekHour ||
                    outlierType.IsLowerOutlier() && timeStamp.Hour == SleepHour;
        }

        public const int SleepHour = 3;

        public const int PeekHour = 12;
    }
}
