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
    public class RelatedControlsDao : BaseDao, IChildEntityDao
    {
        private const string RelatedControlEntityName = "tPcRelatedControls";

        private static readonly string _query = @"
                    select distinct
	                    pcd.ProdClassificationDetailGUID as 'Id'
                    from [ContentUpdates]..[tChangeTracking] ct with(nolock)
						join [Content_InProcess]..[tpcRelatedControls] rc with(nolock)
							on ct.EntityID = rc.RelatedControlGuid
	                    join [Content_InProcess]..[tPcProductClassificationDetail] pcd with (nolock)
		                    on rc.ParentProductClassificationGuid = pcd.ProdClassificationGUID
                                and rc.ParentNumber = pcd.Number
                    where
	                    ct.TableName = 'tPcRelatedControls'
	                    and ct.ChangeVersion > @prevLastVersion 
	                    and ct.ChangeVersion <= @currentLastVersion;
                    ";

        public RelatedControlsDao(ILogger<RelatedControlsDao> logger, IOptions<ContentDbConfig> options)
            : base(options.Value.ConnectionString, logger)
        {
        }

        public string EntityName => RelatedControlEntityName;

        public Task<Result<IEnumerable<DetailDto>>> GetParentEntityAsync(long lastVersion, long currentLastVersion)
        {
            return GetEntityContentAsync<DetailDto>(_query, lastVersion, currentLastVersion);
        }
    }
}
