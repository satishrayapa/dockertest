using Amazon.SQS;
using Amazon.SQS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using TR.OGT.QueryExtract.Infrastructure;

namespace TR.OGT.QueryExtract.Queue
{
    public class SQSClient : IDisposable, ISQSClient
    {
        private readonly IAmazonSQS _amazonSQSClient;
        private readonly ReceiveMessageRequest _receiveMessageRequest;
        private readonly string _queueUrl;
        private readonly ILogger<SQSClient> _logger;

        private bool disposedValue;

        public SQSClient(IAmazonSQS sqsClient, SQSConfig sqsConfig, ILogger<SQSClient> logger)
        {
            if (sqsClient == null)
                throw new ArgumentNullException(nameof(sqsClient));

            if (sqsConfig == null)
                throw new ArgumentNullException(nameof(sqsConfig));

            _logger = logger;

            _logger.LogInformation("Initialization");

            _amazonSQSClient = sqsClient;

            _queueUrl = _amazonSQSClient.GetQueueUrlAsync(sqsConfig.QueueName).GetAwaiter().GetResult().QueueUrl;

            _receiveMessageRequest = new ReceiveMessageRequest
            {
                QueueUrl = _queueUrl,
                MaxNumberOfMessages = sqsConfig.BatchSize,
                WaitTimeSeconds = sqsConfig.WaitTime,
                VisibilityTimeout = sqsConfig.VisibilityTimeout
            };

            _logger.LogInformation("Initialized");
        }

        public async Task<ICollection<Message>> GetMessages()
        {
            var result = await _amazonSQSClient.ReceiveMessageAsync(_receiveMessageRequest);
            _logger.LogInformation($"Received: {result.Messages.Count} messages");
            return result.Messages;
        }

        public async Task ReleaseMessages(IEnumerable<Message> messages)
        {
            var releaseEntries = messages.Select(m => new ChangeMessageVisibilityBatchRequestEntry
            {
                Id = Guid.NewGuid().ToString(),
                ReceiptHandle = m.ReceiptHandle,
                VisibilityTimeout = default(int)
            }).ToList();

            await _amazonSQSClient.ChangeMessageVisibilityBatchAsync(_queueUrl, releaseEntries);
            _logger.LogInformation($"Messages released. Count: ${releaseEntries.Count}");
        }

        public async Task AckMessages(IEnumerable<Message> messages)
        {
            var tasks = messages.Select(m => AckMessage(m)).ToList();

            await Task.WhenAll();
        }

        public async Task AckMessage(Message message)
        {
            await _amazonSQSClient.DeleteMessageAsync(_queueUrl, message.ReceiptHandle);

            _logger.LogInformation($"Message {message.MessageId} acknowledged");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
                return;

            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            _amazonSQSClient.Dispose();
            disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
