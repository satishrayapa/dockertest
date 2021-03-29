using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TR.OGT.ChangeLedger.Common;
using TR.OGT.ChangeLedger.Data.Dao.Abstractions;
using TR.OGT.ChangeLedger.Data.Entities;
using TR.OGT.ChangeLedger.Domain.Options;

namespace TR.OGT.ChangeLedger.Data.Dao
{
    public class VersionsDynamoDbDao : IVersionsDynamoDbDao
    {
        private readonly ILogger<VersionsDynamoDbDao> _logger;
        private readonly IAmazonDynamoDB _dynamoClient;

        private DynamoDBOperationConfig _dynamoConfig;

        public VersionsDynamoDbDao(IAmazonDynamoDB dynamoClient, IOptions<AWSConfig> options, ILogger<VersionsDynamoDbDao> logger)
        {
            _logger = logger;
            _dynamoClient = dynamoClient;

            _dynamoConfig = new DynamoDBOperationConfig()
            {
                ConsistentRead = true,
                OverrideTableName = options.Value.DynamoDBTableName
            };
        }

        public async Task<Result<Dictionary<string, long>>> GetLastVersionsAsync()
        {
            var result = new List<TableLastVersion>();

            using (var context = new DynamoDBContext(_dynamoClient))
            {
                var scanAsyncRequest = context.ScanAsync<TableLastVersion>(Enumerable.Empty<ScanCondition>(), _dynamoConfig);
                result = await scanAsyncRequest.GetRemainingAsync();
            }

            _logger.LogInformation($"fetched last versions: {result.Count} values are found");

            var data = result
                .ToDictionary(key => key.TableName, value => value.LastVersion);

            return data;
        }

        public async Task<Result> SaveLastVersionsAsync(Dictionary<string, long> versions)
        {
            if (versions == null)
            {
                return new ArgumentNullException(nameof(versions));
            }

            var data = versions
                .Select(e => new TableLastVersion() { TableName = e.Key, LastVersion = e.Value });

            using (var context = new DynamoDBContext(_dynamoClient))
            {
                var batchRequest = context.CreateBatchWrite<TableLastVersion>(_dynamoConfig);
                batchRequest.AddPutItems(data);
                await batchRequest.ExecuteAsync();
            }

            _logger.LogInformation("last versions updated");

            return default;
        }
    }
}
