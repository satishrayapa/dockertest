using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using TR.OGT.ChangeLedger.Common;
using TR.OGT.ChangeLedger.Data.Dao.Abstractions;
using TR.OGT.ChangeLedger.Data.Dto;
using TR.OGT.ChangeLedger.Domain.Options;

namespace TR.OGT.ChangeLedger.Data.Dao
{
    /// <summary>
    /// Some items require special control before they can be exported from\imported to the country.
    /// For example, it can be military technologies that might be exported to one countries and not to others.
    /// To check if item require special export\import control <see cref="DetailControlDao"/> is used.
    /// </summary>
    public class DetailControlDao : BaseDao, IControlEntityDao
    {
        private const string DetailControlEntityName = "tPcDetailControl";

        private static readonly string _query = @"
                    select distinct
	                    pcd.ProdClassificationDetailGUID as 'Id'
                    from [ContentUpdates]..[tChangeTracking] ct with (nolock)
	                    join [Content_InProcess]..[tPcDetailControl] dc
		                    on ct.EntityID = dc.DetailControlGuid
	                    join [Content_InProcess]..[tPcProductClassificationDetail] pcd
		                    on dc.ProdClassificationGuid = pcd.ProdClassificationGUID
                                and dc.Number = pcd.Number
                    where
	                    ct.TableName = 'tPcDetailControl'
	                    and ct.ChangeVersion > @prevLastVersion 
	                    and ct.ChangeVersion <= @currentLastVersion;
                    ";

        public DetailControlDao(ILogger<DetailControlDao> logger, IOptions<ContentDbConfig> options)
            : base(options.Value.ConnectionString, logger)
        {
        }

        public string EntityName => DetailControlEntityName;

        public Task<Result<IEnumerable<DetailDto>>> GetParentEntityAsync(long lastVersion, long currentLastVersion)
        {
            return GetEntityContentAsync<DetailDto>(_query, lastVersion, currentLastVersion);
        }
    }
}
