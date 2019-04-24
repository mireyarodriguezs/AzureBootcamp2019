using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Analytics.PeriodHourIngest.PeriodHourReadingsProcessorService.Services
{
    public interface ITimeSeriesBlobService<T>
    {
        Task AppendLines(Guid customerId, string blobName,
             IEnumerable<T> lines);
    }
}
