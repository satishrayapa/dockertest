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
    /// To export\import some items legal documents are required.
    /// These can be legal licences, internatiomal certificates, legal records, etc.
    /// 'tdrDocumentControlMap' is an intermediate table between documents and other entities.
    /// 
    /// <see cref="DocumentControlMap"/> is used to get related documents for Detail Controls.
    /// To find more about Detail Controls see <see cref="DetailControlDao"/>
    /// </summary>
    public class DocumentControlMap : BaseDao, IControlEntityDao
    {
        private const string DocumentControlEntityName = "tdrDocumentControlMap";

        private static readonly string _query = @"
                    select distinct
	                    pcd.ProdClassificationDetailGUID as 'Id'
                    from [ContentUpdates]..[tChangeTracking] ct with (nolock)
	                    join [Content_InProcess]..[tdrDocumentControlMap] dcm
		                    on ct.EntityID = dcm.DocumentControlGuid
	                    join [Content_InProcess]..[tPcDetailControl] dc
		                    on dcm.RelatedGUID = dc.ControlType
	                    join [Content_InProcess]..[tPcProductClassificationDetail] pcd
		                    on dc.ProdClassificationGuid = pcd.ProdClassificationGUID
                                and dc.Number = pcd.Number
                    where
	                    ct.TableName = 'tdrDocumentControlMap'
	                    and ct.ChangeVersion > @prevLastVersion 
	                    and ct.ChangeVersion <= @currentLastVersion
	                    and dcm.RelatedType = 'ControlDetail';
                    ";

        public DocumentControlMap(ILogger<DocumentControlMap> logger, IOptions<ContentDbConfig> options)
            : base(options.Value.ConnectionString, logger)
        {
        }

        public string EntityName => DocumentControlEntityName;

        public Task<Result<IEnumerable<DetailDto>>> GetParentEntityAsync(long lastVersion, long currentLastVersion)
        {
            return GetEntityContentAsync<DetailDto>(_query, lastVersion, currentLastVersion);
        }
    }
}
