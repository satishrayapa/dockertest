using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using TR.OGT.ChangeLedger.Common;
using TR.OGT.ChangeLedger.Data.Dto;
using TR.OGT.ChangeLedger.Data.Entities;
using TR.OGT.ChangeLedger.Data.Interfaces;
using TR.OGT.ChangeLedger.Domain.Entities;
using TR.OGT.ChangeLedger.Domain.Options;
using TR.OGT.ChangeLedger.Infrastructure.Services;

namespace TR.OGT.ChangeLedger.Infrastructure.Tests
{
    [TestClass]
    public class ChangeManagerTest
    {
        private static Mock<ILogger<ChangeManager>> _managerLoggerMock
            = new Mock<ILogger<ChangeManager>>();

        private static Mock<ILogger<AmazonSQSQueueProducer>> _queueLoggerMock
            = new Mock<ILogger<AmazonSQSQueueProducer>>();

        private static IOptions<AWSConfig> _options;

        [ClassInitialize]
        public static void ClassInitialize(TestContext _)
        {
            _options = Options.Create(new AWSConfig() { SQSQueueName = "name" });
        }

        [TestMethod]
        public async Task StartChangeLedger_NoDataFound_QueuePushRecordsIsNotCalled()
        {
            var contentMock = new Mock<IContentUpdateRepository>();
            contentMock
                .Setup(foo => foo.GetContentAsync(new Dictionary<string, long>(), new Dictionary<string, long>()))
                .Returns(Task.FromResult<Result<IEnumerable<ChangeTrackingChange>>> (new List<ChangeTrackingChange>()));   // returns empty list

            var versionMock = new Mock<IVersionRepository>();
            versionMock
                .Setup(foo => foo.GetLatestVersionsAsync())
                .Returns(Task.FromResult(new Result<Dictionary<string, long>>(new Dictionary<string, long>())));

            var queueSenderMock = new Mock<IAmazonSQS>();
            queueSenderMock
                .Setup(foo => foo.GetQueueUrlAsync(It.IsAny<string>(), CancellationToken.None))
                .Returns(Task.FromResult(new GetQueueUrlResponse() { QueueUrl = It.IsAny<string>() }));

            var queue = new AmazonSQSQueueProducer(queueSenderMock.Object, _queueLoggerMock.Object);


            var manager = new ChangeManager(contentMock.Object, versionMock.Object, queue, _options, _managerLoggerMock.Object);
            bool result = await manager.StartChangeLedger(CancellationToken.None);


            queueSenderMock.Verify(foo => foo.SendMessageAsync(It.IsAny<SendMessageRequest>(), CancellationToken.None), Times.Never);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task StartChangeLedger_SingleEntryFound_RecordIsPushedToQueue()
        {
            IEnumerable<ChangeTrackingChange> data = new List<ChangeTrackingChange>()
            {
                new ChangeTrackingChange() { GuidChanged = Guid.NewGuid(), EventType = ChangeEventType.Insert }
            };

            var contentMock = new Mock<IContentUpdateRepository>();
            contentMock
                .Setup(foo => foo.GetContentAsync(new Dictionary<string, long>(), new Dictionary<string, long>()))
                .Returns(Task.FromResult(new Result<IEnumerable<ChangeTrackingChange>>(data)));

            var versionMock = new Mock<IVersionRepository>();
            versionMock
                .Setup(foo => foo.GetLatestVersionsAsync())
                .Returns(Task.FromResult(new Result<Dictionary<string, long>>(new Dictionary<string, long>())));

            var queueSenderMock = new Mock<IAmazonSQS>();
            queueSenderMock
                .Setup(foo => foo.GetQueueUrlAsync(It.IsAny<string>(), CancellationToken.None))
                .Returns(Task.FromResult(new GetQueueUrlResponse() { QueueUrl = It.IsAny<string>() }));
            queueSenderMock
                .Setup(foo => foo.SendMessageAsync(It.IsAny<SendMessageRequest>(), CancellationToken.None))
                .Returns(Task.FromResult(new SendMessageResponse()));

            var queue = new AmazonSQSQueueProducer(queueSenderMock.Object, _queueLoggerMock.Object);


            var manager = new ChangeManager(contentMock.Object, versionMock.Object, queue, _options, _managerLoggerMock.Object);
            bool result = await manager.StartChangeLedger(CancellationToken.None);


            queueSenderMock.Verify(foo => foo.SendMessageAsync(It.IsAny<SendMessageRequest>(), CancellationToken.None), Times.Once);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task StartChangeLedger_MultipleEntriesFound_RecordsArePushedToQueue()
        {
            IEnumerable<ChangeTrackingChange> data = new List<ChangeTrackingChange>()
            {
                new ChangeTrackingChange() { GuidChanged = Guid.NewGuid(), EventType = ChangeEventType.Insert },
                new ChangeTrackingChange() { GuidChanged = Guid.NewGuid(), EventType = ChangeEventType.Update },
                new ChangeTrackingChange() { GuidChanged = Guid.NewGuid(), EventType = ChangeEventType.Insert },
                new ChangeTrackingChange() { GuidChanged = Guid.NewGuid(), EventType = ChangeEventType.Delete },
            };

            var contentMock = new Mock<IContentUpdateRepository>();
            contentMock
                .Setup(foo => foo.GetContentAsync(new Dictionary<string, long>(), new Dictionary<string, long>()))
                .Returns(Task.FromResult(new Result<IEnumerable<ChangeTrackingChange>>(data)));

            var versionMock = new Mock<IVersionRepository>();
            versionMock
                .Setup(foo => foo.GetLatestVersionsAsync())
                .Returns(Task.FromResult(new Result<Dictionary<string, long>>(new Dictionary<string, long>())));

            var queueSenderMock = new Mock<IAmazonSQS>();
            queueSenderMock
                .Setup(foo => foo.GetQueueUrlAsync(It.IsAny<string>(), CancellationToken.None))
                .Returns(Task.FromResult(new GetQueueUrlResponse() { QueueUrl = It.IsAny<string>() }));
            queueSenderMock
                .Setup(foo => foo.SendMessageAsync(It.IsAny<SendMessageRequest>(), CancellationToken.None))
                .Returns(Task.FromResult(new SendMessageResponse()));

            var queue = new AmazonSQSQueueProducer(queueSenderMock.Object, _queueLoggerMock.Object);


            var manager = new ChangeManager(contentMock.Object, versionMock.Object, queue, _options, _managerLoggerMock.Object);
            bool result = await manager.StartChangeLedger(CancellationToken.None);


            queueSenderMock.Verify(foo => foo.SendMessageAsync(It.IsAny<SendMessageRequest>(), CancellationToken.None), Times.Exactly(4));
            Assert.IsTrue(result);
        }
    }
}
