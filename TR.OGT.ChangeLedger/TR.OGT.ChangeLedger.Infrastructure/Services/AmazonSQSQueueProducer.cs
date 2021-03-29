using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TR.OGT.ChangeLedger.Common;
using TR.OGT.ChangeLedger.Domain.Entities;
using TR.OGT.ChangeLedger.Infrastructure.Interfaces;

namespace TR.OGT.ChangeLedger.Infrastructure.Services
{
    public class AmazonSQSQueueProducer : IQueueProducer
    {
        private readonly ILogger<AmazonSQSQueueProducer> _logger;
        private readonly IAmazonSQS _sqsClient;

        public AmazonSQSQueueProducer(IAmazonSQS sqsClient, ILogger<AmazonSQSQueueProducer> logger)
        {
            _logger = logger;
            _sqsClient = sqsClient;
        }

        // todo: send a batch instead of single entity at once
        // todo: consider situations when single message exceeds size limits
        public async Task<Result<bool>> PushRecordsAsync(string queueName, IEnumerable<ChangeTrackingChange> changes)
        {
            if(string.IsNullOrEmpty(queueName))
            {
                return new ArgumentNullException(nameof(queueName));
            }
            if(changes == null)
            {
                return new ArgumentNullException(nameof(changes));
            }

            string queueUrl = await GetQueueUrlAsync(queueName);

            foreach (var record in changes)
            {
                string json = JsonConvert.SerializeObject(record);
                await SendMessage(queueUrl, json);
            }

            return true;
        }

        private async Task<string> GetQueueUrlAsync(string queueName)
        {
            GetQueueUrlResponse response = await _sqsClient.GetQueueUrlAsync(queueName);
            string url = response.QueueUrl;

            return url;
        }

        private async Task SendMessage(string queueUrl, string json)
        {
            var request = new SendMessageRequest(queueUrl, json);
            request.MessageDeduplicationId = Guid.NewGuid().ToString();
            request.MessageGroupId = Guid.NewGuid().ToString();

            SendMessageResponse responseSendMsg =
              await _sqsClient.SendMessageAsync(request);

            System.Net.HttpStatusCode statusCode = responseSendMsg.HttpStatusCode;

            _logger.LogInformation($"Messages sent. Status code: {statusCode}");
        }
    }
}
