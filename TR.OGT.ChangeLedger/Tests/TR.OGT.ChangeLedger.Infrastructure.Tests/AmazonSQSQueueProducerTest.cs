using Amazon.SQS;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TR.OGT.ChangeLedger.Common;
using TR.OGT.ChangeLedger.Domain.Entities;
using TR.OGT.ChangeLedger.Infrastructure.Services;
using TR.OGT.ChangeLedger.Infrastructure.Tests.Fakes;

namespace TR.OGT.ChangeLedger.Infrastructure.Tests
{
    [TestClass]
    public class AmazonSQSQueueProducerTest
    {
		// todo: in case of scenarion when we send multiple messages to diffirent queues, add additional tests
		// todo: implement tests for batch sending as soos as we implement that in client
		// todo: add tests for multiple sends

		private static Mock<ILogger<AmazonSQSQueueProducer>> _loggerMock
			= new Mock<ILogger<AmazonSQSQueueProducer>>();

		[TestMethod]
		public void PushRecordsAsync_NullQueueName_ReturnsException()
        {
			string queueName = null;
			IEnumerable<ChangeTrackingChange> data = new List<ChangeTrackingChange>()
			{
				new ChangeTrackingChange() { GuidChanged = Guid.NewGuid(), EventType = ChangeEventType.Insert }
			};

			var queueStub = new AmazonSQSClientFake();
			var queue = new AmazonSQSQueueProducer(queueStub, _loggerMock.Object);


			Task<Result<bool>> task = queue.PushRecordsAsync(queueName, data);

			Assert.IsTrue(task.Result.IsFailed);
		}

		[TestMethod]
		public void PushRecordsAsync_NullRecords_ReturnsException()
        {
			string queueName = "queue-name-1";
			IEnumerable<ChangeTrackingChange> data = null;

			var queueStub = new AmazonSQSClientFake();
			var queue = new AmazonSQSQueueProducer(queueStub, _loggerMock.Object);

			Task<Result<bool>> task = queue.PushRecordsAsync(queueName, data);

			Assert.IsTrue(task.Result.IsFailed);
		}

		[TestMethod]
		public async Task PushRecordsAsync_EmptyMessage_IsNotSent()
        {
			string queueName = "queue-name-1";
			IEnumerable<ChangeTrackingChange> data = new List<ChangeTrackingChange>()
			{
				// empty
			};

			var queueStub = new AmazonSQSClientFake();
			var queue = new AmazonSQSQueueProducer(queueStub, _loggerMock.Object);

			await queue.PushRecordsAsync(queueName, data);
			int methodCallNumber = queueStub.GetNumberOfSendCalls();

			Assert.AreEqual(methodCallNumber, 0);
		}

        [TestMethod]
        public async Task PushRecordsAsync_SingleMessage_IsSent()
        {
            string queueName = "queue-name-1";
			IEnumerable<ChangeTrackingChange> data = new List<ChangeTrackingChange>()
			{
				new ChangeTrackingChange() { GuidChanged = Guid.NewGuid(), EventType = ChangeEventType.Insert }
			};

			var queueStub = new AmazonSQSClientFake();
            var queue = new AmazonSQSQueueProducer(queueStub, _loggerMock.Object);

            string expectedJson = JsonConvert.SerializeObject(data);
            
			await queue.PushRecordsAsync(queueName, data);

            var sentMessages = queueStub.DeserializeLocalStorage<ChangeTrackingChange>(queueName);
            string actualJson = JsonConvert.SerializeObject(sentMessages);
            int numberOfSendCalls = queueStub.GetNumberOfSendCalls();

			Assert.AreEqual(numberOfSendCalls, 1);
            Assert.AreEqual(expectedJson, actualJson);
        }
    }
}
