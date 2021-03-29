using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using TR.OGT.ChangeLedger.Common;
using TR.OGT.ChangeLedger.Data.Dto;
using TR.OGT.ChangeLedger.Domain.Entities;

namespace TR.OGT.ChangeLedger.Data.Dao.Abstractions
{
    public abstract class ParentEntityDao : BaseDao
    {
        public ParentEntityDao(string connection, ILogger<ParentEntityDao> logger) : base(connection, logger)
        { }

        public abstract string EntityName { get; }

        public abstract Task<Result<IEnumerable<ChangeDto>>> GetContentAsync(long lastVersion, long currentLastVersion);
    }
}
