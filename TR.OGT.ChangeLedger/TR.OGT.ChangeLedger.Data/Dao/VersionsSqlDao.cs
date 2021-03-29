using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using TR.OGT.ChangeLedger.Common;
using TR.OGT.ChangeLedger.Data.Dao.Abstractions;
using TR.OGT.ChangeLedger.Data.Dto;
using TR.OGT.ChangeLedger.Domain.Options;
using TR.OGT.Common.Sql;

namespace TR.OGT.ChangeLedger.Data.Dao
{
    public class VersionsSqlDao : BaseDao, IVersionsSqlDao
    {
        private static readonly string _query = @"
						select 
	                        TableName,
	                        max(ChangeVersion) as 'LastVersion'
                        from [ContentUpdates].[dbo].[tChangeTracking]
                        group by TableName";

        private readonly ILogger<VersionsSqlDao> _logger;

        public VersionsSqlDao(ILogger<VersionsSqlDao> logger, IOptions<ContentDbConfig> options)
            : base(options.Value.ConnectionString, logger)
        {
            _logger = logger;
        }

        public async Task<Result<IEnumerable<LastVersionDto>>> GetLastVersionsAsync()
        {
            Helper helper = CreateConnectionHelper(_query);

            List<LastVersionDto> versions = await helper.QueryAsTypeAsync<LastVersionDto>();

            if (versions == null)
            {
                throw new DataException(nameof(versions));
            }

            _logger.LogInformation($"last version fetched. count: {versions.Count}");
            return versions;
        }
    }
}
