using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TR.OGT.QueryExtract.Domain;
using TR.OGT.QueryExtract.Infrastructure.Elastic;

namespace TR.OGT.QueryExtract.Infrastructure.Tests
{
	[TestClass]
	public class ExtractServiceTests
	{
		private static Mock<ILogger<ExtractService>> LoggerMock = new Mock<ILogger<ExtractService>>();

		[TestMethod]
		public async Task HandleUpdateEvent_IdsIsNull_ResultIsFailed()
		{
			var hsDetails = new Mock<IHsDetailsElasticRepository>();
			var productDetails = new Mock<IHsDetailsSqlRepository>();
			var manager = new ExtractService(hsDetails.Object, productDetails.Object, LoggerMock.Object);

			var result = await manager.HandleUpdateEvent(null);

			Assert.IsTrue(result.IsFailed);
		}

		[TestMethod]
		public async Task HandleCreateEvent_IdsNotNull_HappyPath()
		{
			var hsDetails = new Mock<IHsDetailsElasticRepository>();
			var productDetails = new Mock<IHsDetailsSqlRepository>();
			var manager = new ExtractService(hsDetails.Object, productDetails.Object, LoggerMock.Object);

			productDetails
				.Setup(s => s.GetHSDetails(It.IsAny<Guid[]>()))
				.ReturnsAsync(new List<HSDetails>
				{
					new HSDetails(Guid.Empty)
				});

			hsDetails
				.Setup(s => s.GetActiveDocumentsByHsNumbers(It.IsAny<IEnumerable<string>>()))
				.ReturnsAsync(new List<HsDetailsElasticDto>());

			var result = await manager.HandleCreateEvent(new[] { Guid.NewGuid() });

			productDetails.Verify(v => v.GetHSDetails(It.IsAny<Guid[]>()), Times.Once);
			hsDetails.Verify(v => v.GetActiveDocumentsByHsNumbers(It.IsAny<IEnumerable<string>>()), Times.Once);
			hsDetails.Verify(v => v.UpdateDocuments(It.IsAny<IEnumerable<HsDetailsElasticDto>>()), Times.Once);
			hsDetails.Verify(v => v.SaveDocuments(It.IsAny<IEnumerable<HsDetailsElasticDto>>()), Times.Once);

			Assert.IsFalse(result.IsFailed);
		}

		[TestMethod]
		public async Task HandleUpdateEvent_IdsNotNull_HappyPath()
		{
			var hsDetails = new Mock<IHsDetailsElasticRepository>();
			var productDetails = new Mock<IHsDetailsSqlRepository>();
			var manager = new ExtractService(hsDetails.Object, productDetails.Object, LoggerMock.Object);

			productDetails
				.Setup(s => s.GetHSDetails(It.IsAny<Guid[]>()))
				.ReturnsAsync(new List<HSDetails>
				{
					new HSDetails(Guid.Empty)
				});

			hsDetails
				.Setup(s => s.GetActiveDocumentsByGuids(It.IsAny<IEnumerable<string>>()))
				.ReturnsAsync(new List<HsDetailsElasticDto>());

			var result = await manager.HandleUpdateEvent(new[] { Guid.NewGuid() });

			productDetails.Verify(v => v.GetHSDetails(It.IsAny<Guid[]>()), Times.Once);
			hsDetails.Verify(v => v.GetActiveDocumentsByGuids(It.IsAny<IEnumerable<string>>()), Times.Once);
			hsDetails.Verify(v => v.UpdateDocuments(It.IsAny<IEnumerable<HsDetailsElasticDto>>()), Times.Once);
			hsDetails.Verify(v => v.SaveDocuments(It.IsAny<IEnumerable<HsDetailsElasticDto>>()), Times.Once);

			Assert.IsFalse(result.IsFailed);
		}

		[TestMethod]
		public async Task HandleDeleteEvent_IdsNotNull_HappyPath()
		{
			var hsDetails = new Mock<IHsDetailsElasticRepository>();
			var productDetails = new Mock<IHsDetailsSqlRepository>();
			var manager = new ExtractService(hsDetails.Object, productDetails.Object, LoggerMock.Object);

			hsDetails
				.Setup(s => s.GetActiveDocumentsByGuids(It.IsAny<IEnumerable<string>>()))
				.ReturnsAsync(new List<HsDetailsElasticDto>
				{
					new HsDetailsElasticDto()
				});

			var result = await manager.HandleDeleteEvent(new[] { Guid.NewGuid() });

			hsDetails.Verify(v => v.GetActiveDocumentsByGuids(It.IsAny<IEnumerable<string>>()), Times.Once);
			hsDetails.Verify(v => v.UpdateDocuments(It.IsAny<IEnumerable<HsDetailsElasticDto>>()), Times.Once);

			Assert.IsFalse(result.IsFailed);
		}

		[TestMethod]
		public async Task HandleDeleteEvent_IdsNotNull_NothingToExpire()
		{
			var hsDetails = new Mock<IHsDetailsElasticRepository>();
			var productDetails = new Mock<IHsDetailsSqlRepository>();
			var manager = new ExtractService(hsDetails.Object, productDetails.Object, LoggerMock.Object);

			hsDetails
				.Setup(s => s.GetActiveDocumentsByGuids(It.IsAny<IEnumerable<string>>()))
				.ReturnsAsync(new List<HsDetailsElasticDto>());

			var result = await manager.HandleDeleteEvent(new[] { Guid.NewGuid() });

			hsDetails.Verify(v => v.GetActiveDocumentsByGuids(It.IsAny<IEnumerable<string>>()), Times.Once);
			hsDetails.Verify(v => v.UpdateDocuments(It.IsAny<IEnumerable<HsDetailsElasticDto>>()), Times.Never);

			Assert.IsFalse(result.IsFailed);
		}
	}
}
