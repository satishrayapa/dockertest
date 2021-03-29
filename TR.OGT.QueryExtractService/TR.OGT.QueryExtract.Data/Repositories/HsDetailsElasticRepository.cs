using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TR.OGT.QueryExtract.Infrastructure;
using TR.OGT.QueryExtract.Infrastructure.Elastic;

namespace TR.OGT.QueryExtract.Data
{
	public class HsDetailsElasticRepository : IHsDetailsElasticRepository
	{
		private static DateTime DefaultElasticDate = new DateTime(1900, 1, 1);

		private readonly IElasticClient _elasticClient;

		public HsDetailsElasticRepository(IElasticClient elasticClient)
		{
			_elasticClient = elasticClient;
		}

		public async Task<ICollection<HsDetailsElasticDto>> GetActiveDocumentsByHsNumbers(IEnumerable<string> hsNumbers)
			=> await GetDocumentsByTerms(nameof(HsDetailsElasticDto.HSNumber), hsNumbers);

		public async Task<ICollection<HsDetailsElasticDto>> GetActiveDocumentsByGuids(IEnumerable<string> prodClassificationDetailGuids)
			=> await GetDocumentsByTerms(nameof(HsDetailsElasticDto.ProdClassificationDetailGUID), prodClassificationDetailGuids);

		public async Task<ICollection<HsDetailsElasticDto>> GetDocumentsByTerms(string fieldName, IEnumerable<string> terms)
		{
			if (terms == null)
				throw new ArgumentNullException(nameof(terms));

			if (!terms.Any())
				return Array.Empty<HsDetailsElasticDto>();

			var result = await _elasticClient.SearchAsync<HsDetailsElasticDto>(
				s => s.Query(
					q => q.Terms(t => t
							.Field(new Field(fieldName))
							.Terms(terms)
						) && q.Term(t => t
							.Field(f => f.EndDate)
							.Value(DefaultElasticDate)
						)
					).Take(terms.Count())
				);

			return result.Hits.Select(i => Map(i)).ToList();

			HsDetailsElasticDto Map(IHit<HsDetailsElasticDto> hit)
			{
				hit.Source.Id = hit.Id;
				return hit.Source;
			}
		}

		public async Task SaveDocuments(IEnumerable<HsDetailsElasticDto> hsDetailsElasticDtos)
		{
			if (hsDetailsElasticDtos == null)
			{
				throw new ArgumentNullException(nameof(hsDetailsElasticDtos));
			}

			if (!hsDetailsElasticDtos.Any())
			{
				return;
			}

			await _elasticClient.IndexManyAsync(hsDetailsElasticDtos);
		}

		public async Task UpdateDocuments(IEnumerable<HsDetailsElasticDto> hsDetailsElasticDtos)
		{
			if (hsDetailsElasticDtos == null)
			{
				throw new ArgumentNullException(nameof(hsDetailsElasticDtos));
			}

			if (!hsDetailsElasticDtos.Any())
			{
				return;
			}

			await _elasticClient.BulkAsync(b => b.UpdateMany(hsDetailsElasticDtos, (bu, d) => bu.Id(d.Id).Doc(d)));
		}
	}
}
