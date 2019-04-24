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
        public static async System.Threading.Tasks.Task RunAsync([CosmosDBTrigger(
            databaseName: "hour-data",
            collectionName: "heat-registers",
            ConnectionStringSetting = "CosmosHourlyDataReadWriteConnectionString",
            LeaseCollectionName = "leases",
            CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> input, ILogger log)
        {
            var configuration = new EventHubConfiguration();

            var eventhubClient = configuration.InitializeFromEnvironment();

            await DoRunAsync(input, log, eventhubClient);
        }

        private static async System.Threading.Tasks.Task DoRunAsync(IReadOnlyList<Document> input, ILogger log, EventHubClient eventhubClient)
        {
            if (input != null && input.Count > 0)
            {
                var groupedEventData = input.Select(x =>
                {
                    dynamic myDynamicDocument = x;

                    var heatReadingDocument = new HeatReading
                    {
                        ClientId = new Guid(myDynamicDocument.ClientId),
                        TimeStamp = myDynamicDocument.TimeStamp,
                        RA = myDynamicDocument.RA,
                        RB = myDynamicDocument.RB,
                        RC = myDynamicDocument.RC
                    };

                    var serializedDocument = JsonConvert.SerializeObject(heatReadingDocument, Formatting.None);

                    return new
                    {
                        eventData = new EventData(Encoding.UTF8.GetBytes(serializedDocument)),
                        partitionKey = heatReadingDocument.PartitionKey
                    };
                })
                .GroupBy(x => x.partitionKey);

                foreach (var group in groupedEventData)
                {
                    // batching is internally done in 20ms windows
                    await eventhubClient.SendAsync(group.Select(e => e.eventData), group.Key);
                }
            }
        }
    }
}
