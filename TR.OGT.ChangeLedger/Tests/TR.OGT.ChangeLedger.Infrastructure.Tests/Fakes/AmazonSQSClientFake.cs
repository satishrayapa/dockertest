using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TR.OGT.ChangeLedger.Infrastructure.Tests.Fakes
{
    class AmazonSQSClientFake : IAmazonSQS
    {
        private Dictionary<string, List<string>> _storage;
        private int _numberOfSendCalls;

        public AmazonSQSClientFake()
        {
            _storage = new Dictionary<string, List<string>>();
            _numberOfSendCalls = 0;
        }

        public int GetLocalStorageSize() => _storage?.Count ?? 0;

        public int GetNumberOfSendCalls() => _numberOfSendCalls;

        // todo: review structure of return result
        public List<T> DeserializeLocalStorage<T>(string queue)
        {
            List<T> result = _storage[queue]
                ?.Select(s => JsonConvert.DeserializeObject<T>(s))
                .ToList();

            return result;
        }


        public Task<SendMessageResponse> SendMessageAsync(
            string queueUrl, 
            string messageBody, 
            CancellationToken cancellationToken = default)
        {
            if(!_storage.ContainsKey(queueUrl))
            {
                _storage[queueUrl] = new List<string>();
            }

            _storage[queueUrl].Add(messageBody);
            _numberOfSendCalls += 1;

            var response = new SendMessageResponse()
            {
                HttpStatusCode = System.Net.HttpStatusCode.OK
            };

            return Task.FromResult(response);
        }

        public Task<SendMessageResponse> SendMessageAsync(
            SendMessageRequest request, 
            CancellationToken cancellationToken = default)
        {
            if (!_storage.ContainsKey(request.QueueUrl))
            {
                _storage[request.QueueUrl] = new List<string>();
            }

            _storage[request.QueueUrl].Add(request.MessageBody);
            _numberOfSendCalls += 1;

            var response = new SendMessageResponse()
            {
                HttpStatusCode = System.Net.HttpStatusCode.OK
            };

            return Task.FromResult(response);
        }

        public Task<SendMessageBatchResponse> SendMessageBatchAsync(
            string queueUrl, 
            List<SendMessageBatchRequestEntry> entries, 
            CancellationToken cancellationToken = default)
        {
            if (!_storage.ContainsKey(queueUrl))
            {
                _storage[queueUrl] = new List<string>();
            }

            var jsons = entries
                .Select(e => e.MessageBody)
                .ToList();

            _storage[queueUrl].AddRange(jsons);
            _numberOfSendCalls += 1;

            var response = new SendMessageBatchResponse()
            {
                HttpStatusCode = System.Net.HttpStatusCode.OK
            };

            return Task.FromResult(response);
        }

        public Task<SendMessageBatchResponse> SendMessageBatchAsync(
            SendMessageBatchRequest request, 
            CancellationToken cancellationToken = default)
        {
            if (!_storage.ContainsKey(request.QueueUrl))
            {
                _storage[request.QueueUrl] = new List<string>();
            }

            var jsons = request.Entries
                .Select(e => e.MessageBody)
                .ToList();

            _storage[request.QueueUrl].AddRange(jsons);
            _numberOfSendCalls += 1;

            var response = new SendMessageBatchResponse()
            {
                HttpStatusCode = System.Net.HttpStatusCode.OK
            };

            return Task.FromResult(response);
        }

        public Task<GetQueueUrlResponse> GetQueueUrlAsync(string queueName, CancellationToken cancellationToken = default)
        {
            var response = new GetQueueUrlResponse()
            {
                HttpStatusCode = System.Net.HttpStatusCode.OK,
                QueueUrl = queueName
            };

            return Task.FromResult(response);
        }

        public Task<GetQueueUrlResponse> GetQueueUrlAsync(GetQueueUrlRequest request, CancellationToken cancellationToken = default)
        {
            var response = new GetQueueUrlResponse()
            {
                HttpStatusCode = System.Net.HttpStatusCode.OK,
                QueueUrl = request.QueueName
            };

            return Task.FromResult(response);
        }


        public void Dispose()
        {
            // do nothing
        }


        #region not implemented contracts

        public ISQSPaginatorFactory Paginators => throw new NotImplementedException();

        public IClientConfig Config => throw new NotImplementedException();

        public object JsonDeserializer { get; private set; }

        public Task<AddPermissionResponse> AddPermissionAsync(string queueUrl, string label, List<string> awsAccountIds, List<string> actions, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<AddPermissionResponse> AddPermissionAsync(AddPermissionRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<string> AuthorizeS3ToSendMessageAsync(string queueUrl, string bucket)
        {
            throw new NotImplementedException();
        }

        public Task<ChangeMessageVisibilityResponse> ChangeMessageVisibilityAsync(string queueUrl, string receiptHandle, int visibilityTimeout, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ChangeMessageVisibilityResponse> ChangeMessageVisibilityAsync(ChangeMessageVisibilityRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ChangeMessageVisibilityBatchResponse> ChangeMessageVisibilityBatchAsync(string queueUrl, List<ChangeMessageVisibilityBatchRequestEntry> entries, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ChangeMessageVisibilityBatchResponse> ChangeMessageVisibilityBatchAsync(ChangeMessageVisibilityBatchRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<CreateQueueResponse> CreateQueueAsync(string queueName, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<CreateQueueResponse> CreateQueueAsync(CreateQueueRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<DeleteMessageResponse> DeleteMessageAsync(string queueUrl, string receiptHandle, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<DeleteMessageResponse> DeleteMessageAsync(DeleteMessageRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<DeleteMessageBatchResponse> DeleteMessageBatchAsync(string queueUrl, List<DeleteMessageBatchRequestEntry> entries, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<DeleteMessageBatchResponse> DeleteMessageBatchAsync(DeleteMessageBatchRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<DeleteQueueResponse> DeleteQueueAsync(string queueUrl, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<DeleteQueueResponse> DeleteQueueAsync(DeleteQueueRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, string>> GetAttributesAsync(string queueUrl)
        {
            throw new NotImplementedException();
        }

        public Task<GetQueueAttributesResponse> GetQueueAttributesAsync(string queueUrl, List<string> attributeNames, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<GetQueueAttributesResponse> GetQueueAttributesAsync(GetQueueAttributesRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ListDeadLetterSourceQueuesResponse> ListDeadLetterSourceQueuesAsync(ListDeadLetterSourceQueuesRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ListQueuesResponse> ListQueuesAsync(string queueNamePrefix, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ListQueuesResponse> ListQueuesAsync(ListQueuesRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ListQueueTagsResponse> ListQueueTagsAsync(ListQueueTagsRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<PurgeQueueResponse> PurgeQueueAsync(string queueUrl, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<PurgeQueueResponse> PurgeQueueAsync(PurgeQueueRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ReceiveMessageResponse> ReceiveMessageAsync(string queueUrl, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ReceiveMessageResponse> ReceiveMessageAsync(ReceiveMessageRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<RemovePermissionResponse> RemovePermissionAsync(string queueUrl, string label, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<RemovePermissionResponse> RemovePermissionAsync(RemovePermissionRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task SetAttributesAsync(string queueUrl, Dictionary<string, string> attributes)
        {
            throw new NotImplementedException();
        }

        public Task<SetQueueAttributesResponse> SetQueueAttributesAsync(string queueUrl, Dictionary<string, string> attributes, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<SetQueueAttributesResponse> SetQueueAttributesAsync(SetQueueAttributesRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<TagQueueResponse> TagQueueAsync(TagQueueRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<UntagQueueResponse> UntagQueueAsync(UntagQueueRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
