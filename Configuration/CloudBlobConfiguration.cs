using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
namespace ServerlessDataPipeline.Configuration
{
    public class CloudBlobConfiguration : BaseConfiguration<CloudBlobClient>
    {
        public override CloudBlobClient InitializeFromEnvironment()
        {
            var connectionString = GetEnvironmentVariable(Constants.BlobStorageConnectionString);

            var storageAccount = CloudStorageAccount.Parse(connectionString);
            return storageAccount.CreateCloudBlobClient();
        }
    }
}
