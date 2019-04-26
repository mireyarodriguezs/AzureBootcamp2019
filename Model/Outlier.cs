using System;

namespace ServerlessDataPipeline.Model
{
    public class Outlier
    {
        public Guid ClientId;

        public DateTime TimeStamp;

        public double Value;
    }
}
