namespace TR.OGT.ChangeLedger.Domain.Options
{
    public class AWSConfig
    {
        public string SQSQueueName { get; set; }
        public string DynamoDBTableName { get; set; }
    }
}
