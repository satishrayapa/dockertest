using System.Collections.Generic;
using System.Threading.Tasks;
using TR.OGT.ChangeLedger.Common;
using TR.OGT.ChangeLedger.Data.Entities;

namespace TR.OGT.ChangeLedger.Data.Interfaces
{
    public interface IVersionRepository
    {
        Task<Result<Dictionary<string, long>>> GetLatestVersionsAsync();
        Task<Result<Dictionary<string, long>>> GetCurrentLastVersionsAsync();
        
        Task<Result> SaveLatestVersionsAsync(Dictionary<string, long> versions);
    }
}
