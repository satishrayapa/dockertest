using System.Collections.Generic;
using System.Threading.Tasks;
using TR.OGT.ChangeLedger.Common;
using TR.OGT.ChangeLedger.Domain.Entities;

namespace TR.OGT.ChangeLedger.Infrastructure.Interfaces
{
    public interface IQueueProducer
    {
        Task<Result<bool>> PushRecordsAsync(string queueName, IEnumerable<ChangeTrackingChange> changes);
    }
}
