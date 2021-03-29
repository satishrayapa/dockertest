using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;
using TR.OGT.ChangeLedger.Common;
using TR.OGT.ChangeLedger.Data.Interfaces;
using TR.OGT.ChangeLedger.Domain.Options;
using TR.OGT.ChangeLedger.Infrastructure.Interfaces;

namespace TR.OGT.ChangeLedger.Infrastructure.Services
{
    public class ChangeManager : IChangeManager
    {
        private readonly ILogger<ChangeManager> _logger;
        private readonly IContentUpdateRepository _contentRepository;
        private readonly IVersionRepository _versionRepository;
        private readonly IQueueProducer _queueProducer;
        
        private readonly string _queueName;

        public ChangeManager(
            IContentUpdateRepository contentRepository, 
            IVersionRepository versionRepository, 
            IQueueProducer queueProducer,
            IOptions<AWSConfig> options,
            ILogger<ChangeManager> logger)
        {
            _contentRepository = contentRepository;
            _versionRepository = versionRepository;
            _queueProducer = queueProducer;
            _logger = logger;

            _queueName = options.Value.SQSQueueName;
        }

        public async Task<bool> StartChangeLedger(CancellationToken token)
        {
            var executeResult = await ResultExtensions.TryCatch(() => Execute())
                .OnError(result => _logger.LogError(result.Error, "exception is thrown"));

            return executeResult.Value;
        }

        private async Task<Result<bool>> Execute()
        {
            var lastVersions = await _versionRepository.GetLatestVersionsAsync();
            var currentLastVersions = await _versionRepository.GetCurrentLastVersionsAsync();

            var dataResult = await _contentRepository.GetContentAsync(lastVersions.Value, currentLastVersions.Value);

            var queueResult = await _queueProducer.PushRecordsAsync(_queueName, dataResult.Value);
            if (queueResult.Value)
            {
                await _versionRepository.SaveLatestVersionsAsync(currentLastVersions.Value);
            }

            return queueResult.Value;
        }
    }
}
