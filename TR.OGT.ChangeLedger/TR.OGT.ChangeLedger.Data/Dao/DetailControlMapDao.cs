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
    /// Export\Import Control might be applied for one countries, while not for other.
    /// 'tPcDetailControlMap' table is intermediate table that is used to keep conformity between
    /// Item Controls and the list of countries.
    /// 
    /// To find more about Item Controls see <see cref="DetailControlDao"/>
    /// </summary>
    public class DetailControlMapDao : BaseDao, IControlEntityDao
    {
        private const string DetailControlMapEntityName = "tPcDetailControlMap";

        private static readonly string _query = @"
                    select distinct
	                    pcd.ProdClassificationDetailGUID as 'Id'
                    from [ContentUpdates]..[tChangeTracking] ct with (nolock)
	                    join [Content_InProcess]..[tPcDetailControlMap] dcm
		                    on ct.EntityID = dcm.DetailControlMapGuid
	                    join [Content_InProcess]..[tPcDetailControl] dc
		                    on dcm.DetailControlGuid = dc.DetailControlGuid
	                    join [Content_InProcess]..[tPcProductClassificationDetail] pcd
		                    on dc.ProdClassificationGuid = pcd.ProdClassificationGUID
                                and dc.Number = pcd.Number
                    where
	                    ct.TableName = 'tPcDetailControlMap'
	                    and ct.ChangeVersion > @prevLastVersion 
	                    and ct.ChangeVersion <= @currentLastVersion;
                    ";

        public DetailControlMapDao(ILogger<DetailControlMapDao> logger, IOptions<ContentDbConfig> options)
            : base(options.Value.ConnectionString, logger)
        {
        }

        public string EntityName => DetailControlMapEntityName;

        public Task<Result<IEnumerable<DetailDto>>> GetParentEntityAsync(long lastVersion, long currentLastVersion)
        {
            return GetEntityContentAsync<DetailDto>(_query, lastVersion, currentLastVersion);
        }
    }
}
