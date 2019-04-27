using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataPipeline.Functions.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServerlessDataPipeline.Configuration;

namespace ServerlessDataPipeline
{
    public static class HeatReadingsChangeFeedTrigger
    {
        [FunctionName("HeatReadingsChangeFeedTrigger")]
        public static void Run([CosmosDBTrigger(
            databaseName: "hour-data",
            collectionName: "heat-registers",
            ConnectionStringSetting = "CosmosHourlyDataReadWriteConnectionString",
            LeaseCollectionName = "leases",
            CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> input, ILogger log,
            [EventHub("hourly-data", Connection = "EventHubConnectionString")] ICollector<EventData> outputMessages)
        {
            log.LogInformation($"C# HeatReadingsChangeFeedTrigger executed at: {DateTime.Now}");
            DoRun(input, log, outputMessages);
        }

        private static void DoRun(IReadOnlyList<Document> input, ILogger log, ICollector<EventData> outputMessages)
        {
            if (input != null && input.Count > 0)
            {
                foreach (var x in input)
                {
                    dynamic myDynamicDocument = x;

                    var heatReading = new HeatReading
                    {
                        ClientId = new Guid(myDynamicDocument.ClientId),
                        TimeStamp = myDynamicDocument.TimeStamp,
                        Value = myDynamicDocument.Value
                    };

                    var serializedReading = JsonConvert.SerializeObject(heatReading, Formatting.None);
                    var eventData = new EventData(Encoding.UTF8.GetBytes(serializedReading));
                    outputMessages.Add(eventData);
                }
            }
        }
    }
}
