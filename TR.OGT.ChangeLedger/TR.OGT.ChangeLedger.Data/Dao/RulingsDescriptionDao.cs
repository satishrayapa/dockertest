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
    public class RulingsDescriptionDao : BaseDao, IRulingEntityDao
    {
        private const string RulingsEntityName = "tbrDescription";

        private static readonly string _query = @"
                    select distinct
	                    pcd1.ProdClassificationDetailGUID as 'Id'
                    from [ContentUpdates]..[tChangeTracking] ct with (nolock)
	                    join [BindingRulings]..[tbrDescription] rd
		                    on ct.EntityID = rd.BindingRulingDescriptionGuid
	                    join [BindingRulings]..[tbrRulings] r
		                    on rd.BindingRulingGuid = r.BindingRulingGuid
	                    join [Content_InProcess]..[tPcProductClassificationDetail] pcd1
		                    on r.ProdClassificationGuid = pcd1.ProdClassificationGUID
						join [BindingRulings]..[tbrPcMap] rmap
							on r.BindingRulingGuid = rmap.BindingRulingGuid
						join [Content_InProcess]..[tPcProductClassificationDetail] pcd2
							on rmap.Number = pcd2.Number
                    where
	                    ct.TableName = 'tbrDescription'
						and pcd1.ProdClassificationDetailGUID = pcd2.ProdClassificationDetailGUID
	                    and ct.ChangeVersion > @prevLastVersion 
	                    and ct.ChangeVersion <= @currentLastVersion;
                    ";

        public RulingsDescriptionDao(ILogger<RulingsDescriptionDao> logger, IOptions<ContentDbConfig> options)
            : base(options.Value.ConnectionString, logger)
        {
        }

        public string EntityName => RulingsEntityName;

        public Task<Result<IEnumerable<DetailDto>>> GetParentEntityAsync(long lastVersion, long currentLastVersion)
        {
            return GetEntityContentAsync<DetailDto>(_query, lastVersion, currentLastVersion);
        }
    }
}
