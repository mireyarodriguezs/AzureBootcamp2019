using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DataPipeline.Functions.Models;
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
            var readings = JsonConvert.DeserializeObject<IEnumerable<HeatReading>>(requestBody);

            // Alert
            foreach (var reading in readings)
            {
                var outlier = new Outlier
                {
                    ClientId = new Guid(clientId),
                    Value = reading.Value,
                    TimeStamp = reading.TimeStamp
                };

                // Send To ServiceBus topic
                await collector.AddAsync(outlier).ConfigureAwait(false);
            }

            return new OkObjectResult(clientId);
        }
    }
}
