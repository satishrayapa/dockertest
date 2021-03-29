using System.Collections.Generic;
using System.Threading.Tasks;
using TR.OGT.ChangeLedger.Common;
using TR.OGT.ChangeLedger.Data.Dto;

namespace TR.OGT.ChangeLedger.Data.Dao.Abstractions
{
    public interface IVersionsSqlDao
    {
        Task<Result<IEnumerable<LastVersionDto>>> GetLastVersionsAsync();
    }
}
