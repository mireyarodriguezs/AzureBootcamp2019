using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.EventHubs;

namespace ServerlessDataPipeline.Configuration
{
    public class EventHubConfiguration : BaseConfiguration<EventHubClient>
    {
        public override EventHubClient InitializeFromEnvironment()
        {
            var connectionString = GetEnvironmentVariable(Constants.EventHubConnectionString);
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(connectionString);
            var client = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());
            client.RetryPolicy = RetryPolicy.Default;

            return client;
        }
    }
}
