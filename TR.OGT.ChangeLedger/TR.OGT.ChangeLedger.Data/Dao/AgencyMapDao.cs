using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using TR.OGT.ChangeLedger.Common;
using TR.OGT.ChangeLedger.Data.Dao.Abstractions;
using TR.OGT.ChangeLedger.Data.Dto;
using TR.OGT.ChangeLedger.Domain.Entities;
using TR.OGT.ChangeLedger.Domain.Extensions;
using TR.OGT.ChangeLedger.Domain.Options;

namespace TR.OGT.ChangeLedger.Data.Dao
{
    /// <summary>
    /// tGcAgencyMap is an intermediate table for many-to-many relationship 
    /// between Export Controls documents <see cref="DocumentControlMap"/> and Control Agencies <see cref="AgencyDao"/>
    /// </summary>
    public class AgencyMapDao : BaseDao, IControlAgencyEntityDao
    {
        private const string AgencyEntityName = "tGcAgencyMap";

        private static readonly string _queryTemplate = @"
                    select distinct
	                    pcd.ProdClassificationDetailGUID as 'Id'
                    from [ContentUpdates]..[tChangeTracking] ct with (nolock)
	                    join [Content_InProcess]..[tGcAgencyMap] amap
		                    on ct.EntityID = amap.AgencyMapGuid
	                    join [Content_InProcess]..[tPcDetailControl] dc
		                    on amap.RelatedGuid = dc.ControlType
	                    join [Content_InProcess]..[tPcProductClassificationDetail] pcd
		                    on dc.ProdClassificationGuid = pcd.ProdClassificationGUID
                                and dc.Number = pcd.Number
                    where
	                    ct.TableName = 'tGcAgencyMap'
                        and amap.RelatedType = 'CONTROLDETAIL'
	                    and ct.ChangeVersion > @prevLastVersion 
	                    and ct.ChangeVersion <= @currentLastVersion
                        {0}
                    ";

        public AgencyMapDao(ILogger<AgencyMapDao> logger, IOptions<ContentDbConfig> options)
            : base(options.Value.ConnectionString, logger)
        {
        }

        public string EntityName => AgencyEntityName;

        Task<Result<IEnumerable<DetailDto>>> IChildEntityDao.GetParentEntityAsync(long lastVersion, long currentLastVersion)
        {
            string query = string.Format(_queryTemplate, string.Empty);

            return GetEntityContentAsync<DetailDto>(query, lastVersion, currentLastVersion);
        }

        public Task<Result<IEnumerable<DetailDto>>> GetParentEntityAsync(long lastVersion, long currentLastVersion, ChangeEventType eventType)
        {
            string wherePredicate = eventType.ToWherePredicate();
            string query = string.Format(_queryTemplate, $" and {wherePredicate}");

            return GetEntityContentAsync<DetailDto>(query, lastVersion, currentLastVersion);
        }
    }
}
