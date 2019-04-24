using ServerlessDataPipeline.Extensions;
using System;
using System.Runtime.Serialization;

namespace DataPipeline.Functions.Models
{
    public class HeatReading
    {
        [DataMember(Name = "id")]
        public string Id => new[] { PartitionKey }.GetMd5Id();

        [DataMember(Name = "partitionKey")]
        public string PartitionKey => $"{ClientId}_{TimeStamp.Date}";

        [DataMember(Name = "clientId")]
        public Guid ClientId { get; set; }

        [DataMember(Name = "timeStamp")]
        public DateTime TimeStamp { get; set; }

        [DataMember(Name = "ra")]
        public double RA { get; set; }

        [DataMember(Name = "rb")]
        public double RB { get; set; }

        [DataMember(Name = "rc")]
        public double RC { get; set; }
    }

}
