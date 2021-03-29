namespace TR.OGT.QueryExtract.Queue
{
	public class SQSConfig
	{
		public string AwsAccessKeyId { get; set; }
		public string AwsSecretAccessKey { get; set; }
		public string AwsSessionToken { get; set; }
		public string QueueName { get; set; }
		public int BatchSize { get; set; }
		public int WaitTime { get; set; }
		public int VisibilityTimeout { get; set; }
	}
}
