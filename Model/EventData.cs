using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace ServerlessDataPipeline.Model
{
    [DataContract(Name = "EventData", Namespace = "Microsoft.ServiceBus.Messaging")]
    public class EventData<T>
    {
        [DataMember(Name = "SequenceNumber")]
        public long SequenceNumber { get; set; }

        [DataMember(Name = "Offset")]
        public string Offset { get; set; }

        [DataMember(Name = "EnqueuedTimeUtc")]
        public DateTime EnqueuedTimeUtc { get; set; }


        [DataMember(Name = "Body")]
        public T Body { get; set; }

        public EventData() { }

        public EventData(dynamic record)
        {
            SequenceNumber = (long)record.SequenceNumber;
            Offset = (string)record.Offset;
            EnqueuedTimeUtc = ConvertToDateTime(record.EnqueuedTimeUtc);
            Body = ConvertToType(record.Body);
        }

        public T ConvertToType(dynamic body)
        {
            if (!(body is byte[]))
            {
                return Activator.CreateInstance(typeof(T), (dynamic)body);
            }

            var stringBody = Encoding.UTF8.GetString(body);
            return JsonConvert.DeserializeObject<T>(stringBody);
        }

        public DateTime ConvertToDateTime(dynamic date)
        {
            switch (date)
            {
                case long _:
                    return new DateTime(date);
                case string _:
                    return DateTime.Parse(date, CultureInfo.InvariantCulture);
            }

            return DateTime.MinValue;
        }
    }
}
