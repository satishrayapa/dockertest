using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TR.OGT.QueryExtract.Common;
using TR.OGT.QueryExtract.Domain;
using TR.OGT.QueryExtract.Infrastructure.Elastic;
using static TR.OGT.QueryExtract.Common.ResultExtensions;

namespace TR.OGT.QueryExtract.Infrastructure
{
	public class ExtractService : IExtractService
	{
		private readonly IHsDetailsElasticRepository _hsDetailsElasticRepository;
		private readonly IHsDetailsSqlRepository _productClassificationRepository;
		private readonly ILogger<ExtractService> _logger;

		public ExtractService(
			IHsDetailsElasticRepository hsDetailsElasticRepository,
			IHsDetailsSqlRepository productClassificationRepository,
			ILogger<ExtractService> logger)
		{
			_hsDetailsElasticRepository = hsDetailsElasticRepository;
			_productClassificationRepository = productClassificationRepository;
			_logger = logger;
		}

		public async Task<Result> HandleCreateEvent(IEnumerable<Guid> ids)
		{
			if (ids == null)
			{
				return new ArgumentNullException(nameof(ids));
			}

			var hsDetailsTask = TryCatch(_productClassificationRepository.GetHSDetails, ids.ToArray());

			var updateResult = await hsDetailsTask
				.Bind(d => _hsDetailsElasticRepository.GetActiveDocumentsByHsNumbers(d.Select(i => i.HSNumber)))
				.Tap(ExpireDocuments)
				.OnError(result => _logger.LogError(result.Error, "EndDate is not set for documents."));

			if (updateResult.IsFailed)
			{
				return updateResult.Error;
			}

			var insertResult = await hsDetailsTask
				.Map(HsDetailsElasticMapper.Map)
				.Tap(_hsDetailsElasticRepository.SaveDocuments)
				.OnError(result => _logger.LogError(result.Error, "Documents are not saved to elastic."));

			return insertResult.Error;
		}

		public async Task<Result> HandleUpdateEvent(IEnumerable<Guid> ids)
		{
			if (ids == null)
			{
				return new ArgumentNullException(nameof(ids));
			}

			var updateResult = await TryCatch(_hsDetailsElasticRepository.GetActiveDocumentsByGuids, ids.Select(i => i.ToString()))
				.Tap(ExpireDocuments)
				.OnError(result => _logger.LogError(result.Error, "EndDate is not set for documents."));

			if (updateResult.IsFailed)
			{
				return updateResult.Error;
			}

			var insertResult = await TryCatch(_productClassificationRepository.GetHSDetails, ids.ToArray())
				.Map(HsDetailsElasticMapper.Map)
				.Tap(_hsDetailsElasticRepository.SaveDocuments)
				.OnError(result => _logger.LogError(result.Error, "Documents are not saved to elastic."));

			return insertResult.Error;
		}

		public async Task<Result> HandleDeleteEvent(IEnumerable<Guid> ids)
		{
			if (ids == null)
			{
				return new ArgumentNullException(nameof(ids));
			}

			var notExpiredItems = await _hsDetailsElasticRepository.GetActiveDocumentsByGuids(ids.Select(i => i.ToString()));

			if (!notExpiredItems.Any())
			{
				_logger.LogInformation("HandleDeleteEvent: Nothing to delete");
				return default;
			}

			var result = await TryCatch(ExpireDocuments, notExpiredItems)
				.OnError(result => _logger.LogError(result.Error, "EndDate is not set for documents."));

			return result.Error;
		}

		private async Task ExpireDocuments(IEnumerable<HsDetailsElasticDto> documents)
		{
			foreach (var doc in documents)
			{
				doc.EndDate = DateTime.UtcNow;
			}

			await _hsDetailsElasticRepository.UpdateDocuments(documents);
		}
	}
}
