using System.Collections.Generic;
using System.Threading.Tasks;
using TR.OGT.QueryExtract.Infrastructure.Elastic;

namespace TR.OGT.QueryExtract.Infrastructure
{
	public interface IHsDetailsElasticRepository
	{
		Task<ICollection<HsDetailsElasticDto>> GetActiveDocumentsByGuids(IEnumerable<string> prodClassificationDetailGuids);
		Task<ICollection<HsDetailsElasticDto>> GetActiveDocumentsByHsNumbers(IEnumerable<string> hsNumbers);
		Task SaveDocuments(IEnumerable<HsDetailsElasticDto> hsDetailsElasticDtos);
		Task UpdateDocuments(IEnumerable<HsDetailsElasticDto> hsDetailsElasticDtos);
	}
}