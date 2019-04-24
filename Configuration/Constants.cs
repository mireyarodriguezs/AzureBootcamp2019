namespace ServerlessDataPipeline.Configuration
{
    public class Constants
    {
        public const string CosmosHourlyDataReadWriteConnectionString = "CosmosHourlyDataReadWriteConnectionString";
        public const string CosmosDbDatabaseName = "hour-data";
        public const string CosmosDbCollectionName = "heat-registers";

        public const string EventHubConnectionString = "EventHubConnectionString";

        public const string BlobStorageConnectionString = "BlobStorageConnectionString";
        public const string TimeSeriesCollectionName = "hour-data";

        public const string OutlierReceivedTopicName = "OutlierReceivedTopicName";
        public const string ServiceBusPeriodHourConnectionString = "ServiceBusPeriodHourConnectionString";
    }
}
