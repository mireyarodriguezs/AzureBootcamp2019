using Microsoft.Azure.Documents.Client;
using System;

namespace ServerlessDataPipeline.Configuration
{
    public class CosmosDBConfiguration : BaseConfiguration<DocumentClient>
    {
        public override DocumentClient InitializeFromEnvironment()
        {
            var connectionString = GetEnvironmentVariable(Constants.CosmosHourlyDataReadWriteConnectionString);

            string[] connectionStringParts = connectionString.Split(';');
            Uri endpointUrl = new Uri(connectionStringParts[0].Split('=')[1]);
            int keyStartPosition = connectionStringParts[1].IndexOf('=') + 1;
            string authorizationKey = connectionStringParts[1].Substring(keyStartPosition, connectionStringParts[1].Length - keyStartPosition);

            return new DocumentClient(endpointUrl, authorizationKey);
        }
    }
}
