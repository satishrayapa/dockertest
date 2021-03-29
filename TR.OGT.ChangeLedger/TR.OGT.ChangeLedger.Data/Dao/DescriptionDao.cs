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
    /// Description entity Dao. 
    /// Takes change tracking changes for tPcProductClassificationDescription table 
    /// and finds Detail Guids for those changes
    /// </summary>
    public class DescriptionDao : BaseDao, IChildEntityDao
    {
        private const string DescriptionEntityName = "tPcProductClassificationDescription";

        private static readonly string _query = @"
                    select distinct
	                    pcdesc.ProdClassificationDetailGUID as 'Id'
                    from [ContentUpdates]..[tChangeTracking] ct with (nolock)
	                    join [Content_InProcess]..[tPcProductClassificationDescription] pcdesc
		                    on ct.EntityID = pcdesc.ProdClassificationDescriptionGUID
                    where
	                    ct.TableName = 'tPcProductClassificationDescription'
	                    and ct.ChangeVersion > @prevLastVersion 
	                    and ct.ChangeVersion <= @currentLastVersion
                    ";

        public DescriptionDao(ILogger<DescriptionDao> logger, IOptions<ContentDbConfig> options) 
            : base(options.Value.ConnectionString, logger)
        {
        }

        public string EntityName => DescriptionEntityName;

        public Task<Result<IEnumerable<DetailDto>>> GetParentEntityAsync(long lastVersion, long currentLastVersion)
        {
            return GetEntityContentAsync<DetailDto>(_query, lastVersion, currentLastVersion);
        }
    }
}
