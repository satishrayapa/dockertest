using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;
using Nest;
using System.Linq;
using System.Threading;
using TR.OGT.QueryExtract.Infrastructure.Elastic;

namespace TR.OGT.QueryExtract.Data.Tests.Repositories
{
	[TestClass]
	public class HsDetailsElasticRepositoryTests
	{
		private readonly string[] DefaultMethodArgument = new[] { Guid.Empty.ToString() };

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public async Task GetLatestDocuments_ArgumentNull_ExceptionThrown()
		{
			var elasticClientMock = new Mock<IElasticClient>();
			var repository = new HsDetailsElasticRepository(elasticClientMock.Object);

			await repository.GetActiveDocumentsByGuids(null);
		}

		[TestMethod]
		public async Task GetLatestDocuments_ArgumentArrayIsEmpty_EmptyCollectionReturned()
		{
			var elasticClientMock = new Mock<IElasticClient>();
			var repository = new HsDetailsElasticRepository(elasticClientMock.Object);

			var result = await repository.GetActiveDocumentsByGuids(Enumerable.Empty<string>());

			Assert.IsNotNull(result);
			Assert.AreEqual(Array.Empty<HsDetailsElasticDto>(), result);
		}

		[TestMethod]
		public async Task GetLatestDocuments_ValidArgument_DataReturned()
		{
			const string ExpectedId = "1";
			var sourceDto = new HsDetailsElasticDto
			{
				HSNumber = "2"
			};

			var expectedDto = new HsDetailsElasticDto
			{
				Id = ExpectedId,
				HSNumber = "2"
			};

			var hitMock= new Mock<IHit<HsDetailsElasticDto>>();
			hitMock
				.Setup(s => s.Source)
				.Returns(sourceDto);
			hitMock
				.Setup(s => s.Id)
				.Returns(ExpectedId);

			var resultMock = new Mock<ISearchResponse<HsDetailsElasticDto>>();
			resultMock
				.Setup(s => s.Hits)
				.Returns(new[] { hitMock.Object });

			var elasticClientMock = new Mock<IElasticClient>();
			elasticClientMock
				.Setup(s => s.SearchAsync(
					It.IsAny<Func<SearchDescriptor<HsDetailsElasticDto>, ISearchRequest>>(),
					It.IsAny<CancellationToken>()))
				.ReturnsAsync(resultMock.Object);

			var repository = new HsDetailsElasticRepository(elasticClientMock.Object);

			var result = await repository.GetActiveDocumentsByGuids(DefaultMethodArgument);

			Assert.IsNotNull(result);
			Assert.IsTrue(result.Any());
			Assert.AreEqual(expectedDto.Id, result.First().Id);
			Assert.AreEqual(expectedDto.HSNumber, result.First().HSNumber);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public async Task SaveDocuments_ArgumentNull_ExceptionThrown()
		{
			var elasticClientMock = new Mock<IElasticClient>();
			var repository = new HsDetailsElasticRepository(elasticClientMock.Object);

			await repository.SaveDocuments(null);
		}

		[TestMethod]
		public async Task SaveDocuments_ArgumentArrayIsEmpty_NoExceptions()
		{
			var elasticClientMock = new Mock<IElasticClient>();
			var repository = new HsDetailsElasticRepository(elasticClientMock.Object);

			await repository.SaveDocuments(Enumerable.Empty<HsDetailsElasticDto>());
		}

		[TestMethod]
		public async Task SaveDocuments_ArgumentValid_BulkAsyncIsCalled_NoExceptions()
		{
			var elasticClientMock = new Mock<IElasticClient>();
			var repository = new HsDetailsElasticRepository(elasticClientMock.Object);

			await repository.SaveDocuments(new[] { new HsDetailsElasticDto() });

			elasticClientMock
				.Verify(v => v
					.BulkAsync(It.IsAny<BulkRequest>(), It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public async Task UpdateDocuments_ArgumentNull_ExceptionThrown()
		{
			var elasticClientMock = new Mock<IElasticClient>();
			var repository = new HsDetailsElasticRepository(elasticClientMock.Object);

			await repository.UpdateDocuments(null);
		}

		[TestMethod]
		public async Task UpdateDocuments_ArgumentArrayIsEmpty_NoExceptions()
		{
			var elasticClientMock = new Mock<IElasticClient>();
			var repository = new HsDetailsElasticRepository(elasticClientMock.Object);

			await repository.UpdateDocuments(Enumerable.Empty<HsDetailsElasticDto>());
		}

		[TestMethod]
		public async Task UpdateDocuments_ArgumentValid_BulkAsyncIsCalled_NoExceptions()
		{
			var elasticClientMock = new Mock<IElasticClient>();
			var repository = new HsDetailsElasticRepository(elasticClientMock.Object);

			await repository.UpdateDocuments(new[] { new HsDetailsElasticDto() });

			elasticClientMock
				.Verify(v => v
					.BulkAsync(It.IsAny<Func<BulkDescriptor, IBulkRequest>>(), It.IsAny<CancellationToken>()), Times.Once);
		}
	}
}
