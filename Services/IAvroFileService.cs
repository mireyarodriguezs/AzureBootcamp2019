using System.Collections.Generic;
using System.IO;

namespace ServerlessDataPipeline.Services
{
    public interface IAvroFileService<T>
    {
        Stream Compress(IEnumerable<T> documents);
        IEnumerable<T> Decompress(Stream batchedReadings);
    }
}
