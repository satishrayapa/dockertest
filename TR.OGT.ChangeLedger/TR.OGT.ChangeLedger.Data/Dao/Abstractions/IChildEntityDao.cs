using System.Collections.Generic;
using System.Threading.Tasks;

using TR.OGT.ChangeLedger.Common;
using TR.OGT.ChangeLedger.Data.Dto;

namespace TR.OGT.ChangeLedger.Data.Dao.Abstractions
{
    public interface IChildEntityDao
    {
        string EntityName { get; }

        Task<Result<IEnumerable<DetailDto>>> GetParentEntityAsync(long lastVersion, long currentLastVersion);
    }
}
