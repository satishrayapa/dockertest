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
    /// Description of Control Agency.
    /// See <see cref="AgencyDao"/> for details about Control Agencies.
    /// </summary>
    public class AgencyDescription : BaseDao, IControlAgencyEntityDao
    {
        private const string AgencyDescriptionEntityName = "tGcAgencyDescription";

        private static readonly string _queryTemplate = @"
                    select distinct
	                    pcd.ProdClassificationDetailGUID as 'Id'
                    from [ContentUpdates]..[tChangeTracking] ct with (nolock)
	                    join [Content_InProcess]..[tGcAgencyDescription] adesc
		                    on ct.EntityID = adesc.AgencyDescriptionGuid
	                    join [Content_InProcess]..[tGcAgency] ag
		                    on adesc.AgencyGuid = ag.AgencyGuid
                        join [Content_InProcess]..[tGcAgencyMap] amap
		                    on ag.AgencyGuid = amap.AgencyGuid
	                    join [Content_InProcess]..[tPcDetailControl] dc
		                    on amap.RelatedGuid = dc.ControlType
	                    join [Content_InProcess]..[tPcProductClassificationDetail] pcd
		                    on dc.ProdClassificationGuid = pcd.ProdClassificationGUID
                                and dc.Number = pcd.Number
                    where
	                    ct.TableName = 'tGcAgencyDescription'
                        and adesc.AgencyDescriptionType = 'NAME'
	                    and ct.ChangeVersion > @prevLastVersion 
	                    and ct.ChangeVersion <= @currentLastVersion
                        {0}
                    ";

        public AgencyDescription(ILogger<AgencyDescription> logger, IOptions<ContentDbConfig> options)
            : base(options.Value.ConnectionString, logger)
        {
        }

        public string EntityName => AgencyDescriptionEntityName;

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
