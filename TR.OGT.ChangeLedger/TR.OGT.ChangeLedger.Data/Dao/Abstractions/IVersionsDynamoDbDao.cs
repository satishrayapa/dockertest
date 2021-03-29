using System.Collections.Generic;
using System.Threading.Tasks;
using TR.OGT.ChangeLedger.Common;

namespace TR.OGT.ChangeLedger.Data.Dao.Abstractions
{
    public interface IVersionsDynamoDbDao
    {
        Task<Result<Dictionary<string, long>>> GetLastVersionsAsync();
        Task<Result> SaveLastVersionsAsync(Dictionary<string, long> versions);
    }
}
