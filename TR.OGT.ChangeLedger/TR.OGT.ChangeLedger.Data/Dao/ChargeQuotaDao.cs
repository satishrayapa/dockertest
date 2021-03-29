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
    public class ChargeQuotaDao : BaseDao, IChildEntityDao
    {
        private const string QuotaEntityName = "tChChargeQuota";

        private static readonly string _query = @"
                    select distinct
	                    pcd1.ProdClassificationDetailGUID as 'Id'
                    from [ContentUpdates]..[tChangeTracking] ct with(nolock)
	                    join [Content_InProcess]..[tchChargeQuota] cq with(nolock)
		                    on ct.EntityID = cq.ChargeQuotaGuid
						join [Content_InProcess]..[tPcProductClassificationDetail] pcd1 with(nolock)
		                    on cq.ProdClassificationGUID = pcd1.ProdClassificationGUID
						join [Content_InProcess]..[tChChargeQuotaNumberMap] cqm with(nolock)
		                    on cq.ChargeQuotaGuid = cqm.ChargeQuotaGuid 
						join [Content_InProcess]..[tPcProductClassificationDetail] pcd2 with(nolock)
		                    on cqm.Number = pcd2.Number
                    where
	                    ct.TableName = 'tChChargeQuota'
						and pcd1.ProdClassificationDetailGUID = pcd2.ProdClassificationDetailGUID
	                    and ct.ChangeVersion > @prevLastVersion 
	                    and ct.ChangeVersion <= @currentLastVersion
                    ";


        public ChargeQuotaDao(ILogger<ChargeQuotaDao> logger, IOptions<ContentDbConfig> options)
            : base(options.Value.ConnectionString, logger)
        {
        }

        public string EntityName => QuotaEntityName;

        public Task<Result<IEnumerable<DetailDto>>> GetParentEntityAsync(long lastVersion, long currentLastVersion)
        {
            return GetEntityContentAsync<DetailDto>(_query, lastVersion, currentLastVersion);
        }
    }
}
