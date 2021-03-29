using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TR.OGT.QueryExtract.Common;
using TR.OGT.QueryExtract.Domain;

namespace TR.OGT.QueryExtract.Infrastructure
{
    public interface IHsDetailsSqlRepository
    {
        Task<Result<IReadOnlyCollection<HSDetails>>> GetHSDetails(Guid[] ids);
    }
}
