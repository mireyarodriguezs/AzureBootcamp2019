using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Hadoop.Avro;
using Microsoft.Hadoop.Avro.Container;
using Newtonsoft.Json;

namespace ServerlessDataPipeline.Services
{
    public class AvroFileService<T> : IAvroFileService<T> where T : new()
    {
        public IEnumerable<T> Decompress(Stream batchedReadings)
        {
            if (batchedReadings.Length == 508) //508 is the magic number that represents the size of an avro file with empty body.
            {
                return Enumerable.Empty<T>();
            }

            var decompressedList = new List<T>();
            batchedReadings.Seek(0, SeekOrigin.Begin);
            using (var reader = AvroContainer.CreateGenericReader(batchedReadings, false, new CodecFactory()))
            {
                while (reader.MoveNext())
                {
                    foreach (dynamic record in reader.Current.Objects)
                    {
                        try
                        {
                            var instance = Activator.CreateInstance(typeof(T), record);
                            decompressedList.Add(instance);
                        }
                        catch (JsonReaderException)
                        {
                        }
                    }
                }
            }

            return decompressedList;
        }

        public Stream Compress(IEnumerable<T> documents)
        {
            var buffer = new MemoryStream();

            var settings = new AvroSerializerSettings { GenerateSerializer = true };
            using (var w = AvroContainer.CreateWriter<T>(buffer, settings, Codec.Deflate))
            using (var writer = new SequentialWriter<T>(w, 24))
            {
                // Serialize the data to stream by using the sequential writer
                documents.ToList().ForEach(writer.Write);
            }

            buffer.Seek(0, SeekOrigin.Begin);

            return buffer;
        }
    }
}
