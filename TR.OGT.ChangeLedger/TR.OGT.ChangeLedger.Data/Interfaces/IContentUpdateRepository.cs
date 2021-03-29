using System.Collections.Generic;
using System.Threading.Tasks;
using TR.OGT.ChangeLedger.Common;
using TR.OGT.ChangeLedger.Domain.Entities;

namespace TR.OGT.ChangeLedger.Data.Interfaces
{
    public interface IContentUpdateRepository
    {
        Task<Result<IEnumerable<ChangeTrackingChange>>> GetContentAsync(
            Dictionary<string, long> lastVersions, 
            Dictionary<string, long> currentLastVersions);
    }
}
