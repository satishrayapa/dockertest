using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TR.OGT.ChangeLedger.Common;
using TR.OGT.ChangeLedger.Data.Dao.Abstractions;
using TR.OGT.ChangeLedger.Data.Dto;
using TR.OGT.ChangeLedger.Domain.Options;

namespace TR.OGT.ChangeLedger.Data.Dao
{
    public class DetailDao : ParentEntityDao
    {
        private const string DetailEntityName = "tPcProductClassificationDetail";

        private static readonly string _query = @"
                        select distinct
	                        CAST(EntityId as UNIQUEIDENTIFIER) as 'Id',
	                        case Operation
		                        when 'I' then 1
		                        when 'U' then 2
		                        when 'D' then 4
	                        end as 'EventType'
                        from [ContentUpdates]..[tChangeTracking] with (nolock)
                        where
	                        TableName = 'tPcProductClassificationDetail'
	                        and ChangeVersion > @prevLastVersion 
	                        and ChangeVersion <= @currentLastVersion
                    ";

        private readonly ILogger<DetailDao> _logger;

        public DetailDao(ILogger<DetailDao> logger, IOptions<ContentDbConfig> options) 
            : base(options.Value.ConnectionString, logger)
        {
            _logger = logger;
        }

        public override string EntityName => DetailEntityName;

        public override Task<Result<IEnumerable<ChangeDto>>> GetContentAsync(long lastVersion, long currentLastVersion)
        {
            return GetEntityContentAsync<ChangeDto>(_query, lastVersion, currentLastVersion);
        }
    }
}
