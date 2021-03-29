using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;
using TR.OGT.QueryExtract.Common;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using TR.OGT.QueryExtract.Infrastructure;
using TR.OGT.QueryExtract.Infrastructure.Sql;
using TR.OGT.QueryExtract.Domain;

namespace TR.OGT.QueryExtract.Data.Tests.Repositories
{
    [TestClass]
    public class IHsDetailsSqlRepositoryTests
    {
        private static Mock<ILogger<HsDetailsSqlRepository>> LoggerMock
            = new Mock<ILogger<HsDetailsSqlRepository>>();
        private readonly Guid[] DefaultMethodArgument = new[] { Guid.Empty };

        [TestMethod]
        public async Task GetHSDetails_NullIdsArray_ReturnsFailedResult()
        {
            var hsTempDataDao = new Mock<ITempDataDao>();
            var hsDetailsDao = new Mock<IHSGenericDao<HSDetailsSqlDto>>();
            var hsDescriptionsDao = new Mock<IHSGenericDao<HSDescriptionSqlDto>>();
            var relatedHSDao = new Mock<IHSGenericDao<RelatedHSSqlDto>>();
            var quotasDao = new Mock<IHSGenericDao<ChargeQuotaSqlDto>>();
            var notesDao = new Mock<IHSGenericDao<PcNoteSqlDto>>();
            var bindingRullings = new Mock<IBindingRullingsDao>();

            var hsDetailsSqlRepository = new HsDetailsSqlRepository(
                hsTempDataDao.Object,
                hsDetailsDao.Object,
                hsDescriptionsDao.Object,
                relatedHSDao.Object,
                quotasDao.Object,
                notesDao.Object,
                bindingRullings.Object,
                LoggerMock.Object);
            var result = await hsDetailsSqlRepository.GetHSDetails(null);

            Assert.IsTrue(result.IsFailed);

            hsTempDataDao
                .Verify(i => i.LoadTempTable(It.IsAny<Guid[]>(), It.IsAny<string>()), Times.Never);
            hsTempDataDao
                .Verify(i => i.DropTempTable(It.IsAny<string>()), Times.Never);
            hsDetailsDao
                .Verify(i => i.GetAll(It.IsAny<string>()), Times.Never);
            hsDescriptionsDao
                .Verify(i => i.GetAll(It.IsAny<string>()), Times.Never);
            relatedHSDao
                .Verify(i => i.GetAll(It.IsAny<string>()), Times.Never);
            quotasDao
                .Verify(i => i.GetAll(It.IsAny<string>()), Times.Never);
            notesDao
                .Verify(i => i.GetAll(It.IsAny<string>()), Times.Never);
            bindingRullings
                .Verify(i => i.GetBindingRullings(It.IsAny<string>()), Times.Never);
            bindingRullings
                .Verify(i => i.GetBindingRullingsText(It.IsAny<string>()), Times.Never);

            hsTempDataDao.VerifyAll();
            hsDetailsDao.VerifyAll();
            hsDescriptionsDao.VerifyAll();
            relatedHSDao.VerifyAll();
            quotasDao.VerifyAll();
        }

        [TestMethod]
        public async Task GetHSDetails_EmptyIdsArray_ReturnsEmptyResult()
        {
            var hsTempDataDao = new Mock<ITempDataDao>();
            var hsDetailsDao = new Mock<IHSGenericDao<HSDetailsSqlDto>>();
            var hsDescriptionsDao = new Mock<IHSGenericDao<HSDescriptionSqlDto>>();
            var relatedHSDao = new Mock<IHSGenericDao<RelatedHSSqlDto>>();
            var quotasDao = new Mock<IHSGenericDao<ChargeQuotaSqlDto>>();
            var notesDao = new Mock<IHSGenericDao<PcNoteSqlDto>>();
            var bindingRullings = new Mock<IBindingRullingsDao>();

            var hsDetailsSqlRepository = new HsDetailsSqlRepository(
                hsTempDataDao.Object,
                hsDetailsDao.Object,
                hsDescriptionsDao.Object,
                relatedHSDao.Object,
                quotasDao.Object,
                notesDao.Object,
                bindingRullings.Object,
                LoggerMock.Object);
            var result = await hsDetailsSqlRepository.GetHSDetails(new Guid[0]);

            Assert.IsTrue(result.IsOk);
            Assert.AreEqual(0, result.Value.ToList().Count());
        }

        [TestMethod]
        public async Task GetHSDetails_NoProductClassificationFound_ReturnsEmptyResult()
        {
            var hsTempDataDao = new Mock<ITempDataDao>();
            var hsDetailsDao = new Mock<IHSGenericDao<HSDetailsSqlDto>>();
            var hsDescriptionsDao = new Mock<IHSGenericDao<HSDescriptionSqlDto>>();
            var relatedHSDao = new Mock<IHSGenericDao<RelatedHSSqlDto>>();
            var quotasDao = new Mock<IHSGenericDao<ChargeQuotaSqlDto>>();
            var notesDao = new Mock<IHSGenericDao<PcNoteSqlDto>>();
            var bindingRullings = new Mock<IBindingRullingsDao>();

            hsTempDataDao
                .Setup(i => i.LoadTempTable(It.IsAny<Guid[]>(), It.IsAny<string>()))
                .ReturnsAsync(default(Result));
            hsTempDataDao
                .Setup(i => i.DropTempTable(It.IsAny<string>()))
                .ReturnsAsync(default(Result<int>));
            hsDetailsDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            hsDescriptionsDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            relatedHSDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            quotasDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            notesDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            bindingRullings
                .Setup(i => i.GetBindingRullings(It.IsAny<string>()))
                .ReturnsAsync(new List<BindingRullingsSqlDto>());
            bindingRullings
                .Setup(i => i.GetBindingRullingsText(It.IsAny<string>()))
                .ReturnsAsync(new List<BindingRullingsTextSqlDto>());


            var hsDetailsSqlRepository = new HsDetailsSqlRepository(
                hsTempDataDao.Object,
                hsDetailsDao.Object,
                hsDescriptionsDao.Object,
                relatedHSDao.Object,
                quotasDao.Object,
                notesDao.Object,
                bindingRullings.Object,
                LoggerMock.Object);
            var result = await hsDetailsSqlRepository.GetHSDetails(new Guid[1] { new Guid("775F462C-1FB5-48A9-A007-000061D5D212") });


            hsTempDataDao
                .Verify(i => i.LoadTempTable(It.IsAny<Guid[]>(), It.IsAny<string>()), Times.AtLeastOnce);
            hsTempDataDao
                .Verify(i => i.DropTempTable(It.IsAny<string>()), Times.AtLeastOnce);
            hsDetailsDao
                .Verify(i => i.GetAll(It.IsAny<string>()), Times.Once);
            hsDescriptionsDao
                .Verify(i => i.GetAll(It.IsAny<string>()), Times.Once);
            relatedHSDao
                .Verify(i => i.GetAll(It.IsAny<string>()), Times.Once);
            quotasDao
                .Verify(i => i.GetAll(It.IsAny<string>()), Times.Once);
            notesDao
                .Verify(i => i.GetAll(It.IsAny<string>()), Times.Once);
            bindingRullings
                .Verify(i => i.GetBindingRullings(It.IsAny<string>()), Times.Once);
            bindingRullings
                .Verify(i => i.GetBindingRullingsText(It.IsAny<string>()), Times.Once);
            //TODO: verify GetAll call for other nested entities

            Assert.IsTrue(result.IsOk);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(0, result.Value.ToList().Count());

            hsTempDataDao.VerifyAll();
            hsDetailsDao.VerifyAll();
            hsDescriptionsDao.VerifyAll();
            relatedHSDao.VerifyAll();
            quotasDao.VerifyAll();
            notesDao.VerifyAll();
            bindingRullings.VerifyAll();
        }

        [TestMethod]
        public async Task GetHSDetails_ProductClassificationFound_MapsToEntity()
        {
            var dataToReturn = new List<HSDetails>() {
                new HSDetails(new Guid("775F462C-1FB5-48A9-A007-000061D5D212"))
                {
                    HSNumber = "123",
                    Chapter = "Chapter",
                    Heading = "Heading",
                    Subheading = "Subheading",
                    UOMs = "UOMs",
                    Uses = "Uses",
                    ProdClassificationGUID = new Guid("775F462C-1FB5-48A9-A007-000061D5D213"),
                    CountryCodes = "CountryCodes",
                    StartDate = new DateTime(2021, 1, 1),
                    EndDate = new DateTime(2021, 1, 2)
                }
            };

            var hsTempDataDao = new Mock<ITempDataDao>();
            var hsDetailsDao = new Mock<IHSGenericDao<HSDetailsSqlDto>>();
            var hsDescriptionsDao = new Mock<IHSGenericDao<HSDescriptionSqlDto>>();
            var relatedHSDao = new Mock<IHSGenericDao<RelatedHSSqlDto>>();
            var quotasDao = new Mock<IHSGenericDao<ChargeQuotaSqlDto>>();
            var notesDao = new Mock<IHSGenericDao<PcNoteSqlDto>>();
            var bindingRullings = new Mock<IBindingRullingsDao>();

            hsTempDataDao
                .Setup(i => i.LoadTempTable(It.IsAny<Guid[]>(), It.IsAny<string>()))
                .ReturnsAsync(default(Result));
            hsTempDataDao
                .Setup(i => i.DropTempTable(It.IsAny<string>()))
                .ReturnsAsync(default(Result<int>));
            hsDetailsDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(dataToReturn);
            hsDescriptionsDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            relatedHSDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            quotasDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            notesDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            bindingRullings
                .Setup(i => i.GetBindingRullings(It.IsAny<string>()))
                .ReturnsAsync(new List<BindingRullingsSqlDto>());
            bindingRullings
                .Setup(i => i.GetBindingRullingsText(It.IsAny<string>()))
                .ReturnsAsync(new List<BindingRullingsTextSqlDto>());

            var hsDetailsSqlRepository = new HsDetailsSqlRepository(
                hsTempDataDao.Object,
                hsDetailsDao.Object,
                hsDescriptionsDao.Object,
                relatedHSDao.Object,
                quotasDao.Object,
                notesDao.Object,
                bindingRullings.Object,
                LoggerMock.Object);
            var result = await hsDetailsSqlRepository.GetHSDetails(new Guid[1] { new Guid("775F462C-1FB5-48A9-A007-000061D5D212") });

            Assert.IsTrue(result.IsOk);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(dataToReturn.Count, result.Value.ToList().Count());
            Assert.AreEqual(new Guid("775F462C-1FB5-48A9-A007-000061D5D212"), result.Value.ToList()[0].ProdClassificationDetailGUID);
            Assert.AreEqual("123", result.Value.ToList()[0].HSNumber);
            Assert.AreEqual("Chapter", result.Value.ToList()[0].Chapter);
            Assert.AreEqual("Heading", result.Value.ToList()[0].Heading);
            Assert.AreEqual("Subheading", result.Value.ToList()[0].Subheading);
            Assert.AreEqual("UOMs", result.Value.ToList()[0].UOMs);
            Assert.AreEqual("Uses", result.Value.ToList()[0].Uses);
            Assert.AreEqual(new Guid("775F462C-1FB5-48A9-A007-000061D5D213"), result.Value.ToList()[0].ProdClassificationGUID);
            Assert.AreEqual("CountryCodes", result.Value.ToList()[0].CountryCodes);
            Assert.AreEqual(new DateTime(2021, 1, 1), result.Value.ToList()[0].StartDate);
            Assert.AreEqual(new DateTime(2021, 1, 2), result.Value.ToList()[0].EndDate);
        }

        [TestMethod]
        public async Task GetHSDetails_ProductClassificationDetailsFound_MapsToEntity()
        {
            var hsDetailToReturn = new List<HSDetails>() {
                new HSDetails(new Guid("775F462C-1FB5-48A9-A007-000061D5D212"))
                {
                    HSNumber = "123"
                }
            };

            var hsDescriptionToReturn = new List<HSDetails>() {
                new HSDetails(new Guid("775F462C-1FB5-48A9-A007-000061D5D212"))
                {
                    HSNumber = "123",
                    Descriptions = new List<HSDescription>
                    {
                        new HSDescription
                        {
                            CultureCode = "CultureCode",
                            DescriptionText = "DescriptionText",
                            DisplayFlag = "Y",
                            SortOrder = 1
                        }
                    }
                }
            };

            var hsTempDataDao = new Mock<ITempDataDao>();
            var hsDetailsDao = new Mock<IHSGenericDao<HSDetailsSqlDto>>();
            var hsDescriptionsDao = new Mock<IHSGenericDao<HSDescriptionSqlDto>>();
            var relatedHSDao = new Mock<IHSGenericDao<RelatedHSSqlDto>>();
            var quotasDao = new Mock<IHSGenericDao<ChargeQuotaSqlDto>>();
            var notesDao = new Mock<IHSGenericDao<PcNoteSqlDto>>();
            var bindingRullings = new Mock<IBindingRullingsDao>();

            hsTempDataDao
                .Setup(i => i.LoadTempTable(It.IsAny<Guid[]>(), It.IsAny<string>()))
                .ReturnsAsync(default(Result));
            hsDetailsDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(hsDetailToReturn);
            hsDescriptionsDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(hsDescriptionToReturn);
            relatedHSDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            quotasDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            notesDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            bindingRullings
                .Setup(i => i.GetBindingRullings(It.IsAny<string>()))
                .ReturnsAsync(new List<BindingRullingsSqlDto>());
            bindingRullings
                .Setup(i => i.GetBindingRullingsText(It.IsAny<string>()))
                .ReturnsAsync(new List<BindingRullingsTextSqlDto>());

            var hsDetailsSqlRepository = new HsDetailsSqlRepository(
                hsTempDataDao.Object,
                hsDetailsDao.Object,
                hsDescriptionsDao.Object,
                relatedHSDao.Object,
                quotasDao.Object,
                notesDao.Object,
                bindingRullings.Object,
                LoggerMock.Object);
            var result = await hsDetailsSqlRepository.GetHSDetails(new Guid[1] { new Guid("775F462C-1FB5-48A9-A007-000061D5D212") });

            Assert.IsTrue(result.IsOk);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(hsDetailToReturn.Count, result.Value.ToList().Count());
            Assert.AreEqual(hsDescriptionToReturn.Count, result.Value.ToList()[0].Descriptions.Count());

            Assert.AreEqual("CultureCode", result.Value.ToList()[0].Descriptions.ToList()[0].CultureCode);
            Assert.AreEqual("DescriptionText", result.Value.ToList()[0].Descriptions.ToList()[0].DescriptionText);
            Assert.AreEqual("Y", result.Value.ToList()[0].Descriptions.ToList()[0].DisplayFlag);
            Assert.AreEqual(1, result.Value.ToList()[0].Descriptions.ToList()[0].SortOrder);
        }

        [TestMethod]
        public async Task GetHSDetails_RelatedControlsFound_MapsToEntity()
        {
            var hsDetailToReturn = new List<HSDetails>() {
                new HSDetails(new Guid("775F462C-1FB5-48A9-A007-000061D5D212"))
                {
                    HSNumber = "123"
                }
            };

            var relatedHSToReturn = new List<HSDetails>() {
                new HSDetails(new Guid("775F462C-1FB5-48A9-A007-000061D5D212"))
                {
                    HSNumber = "123",
                    RelatedHS = new List<RelatedHS>
                    {
                        new RelatedHS
                        {
                            RelatedHSNum = "RelatedHSNum",
                            Note = "Note",
                            StartDate = new DateTime(2021, 1, 1),
                            EndDate = new DateTime(2021, 1, 2)
                        }
                    }
                },
            };

            var hsTempDataDao = new Mock<ITempDataDao>();
            var hsDetailsDao = new Mock<IHSGenericDao<HSDetailsSqlDto>>();
            var hsDescriptionsDao = new Mock<IHSGenericDao<HSDescriptionSqlDto>>();
            var relatedHSDao = new Mock<IHSGenericDao<RelatedHSSqlDto>>();
            var quotasDao = new Mock<IHSGenericDao<ChargeQuotaSqlDto>>();
            var notesDao = new Mock<IHSGenericDao<PcNoteSqlDto>>();
            var bindingRullings = new Mock<IBindingRullingsDao>();

            hsTempDataDao
                .Setup(i => i.LoadTempTable(It.IsAny<Guid[]>(), It.IsAny<string>()))
                .ReturnsAsync(default(Result));
            hsDetailsDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(hsDetailToReturn);
            hsDescriptionsDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            relatedHSDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(relatedHSToReturn);
            quotasDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            notesDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            bindingRullings
                .Setup(i => i.GetBindingRullings(It.IsAny<string>()))
                .ReturnsAsync(new List<BindingRullingsSqlDto>());
            bindingRullings
                .Setup(i => i.GetBindingRullingsText(It.IsAny<string>()))
                .ReturnsAsync(new List<BindingRullingsTextSqlDto>());

            var hsDetailsSqlRepository = new HsDetailsSqlRepository(
                hsTempDataDao.Object,
                hsDetailsDao.Object,
                hsDescriptionsDao.Object,
                relatedHSDao.Object,
                quotasDao.Object,
                notesDao.Object,
                bindingRullings.Object,
                LoggerMock.Object);
            var result = await hsDetailsSqlRepository.GetHSDetails(new Guid[1] { new Guid("775F462C-1FB5-48A9-A007-000061D5D212") });

            Assert.IsTrue(result.IsOk);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(hsDetailToReturn.Count, result.Value.ToList().Count());
            Assert.AreEqual(relatedHSToReturn.Count, result.Value.ToList()[0].RelatedHS.Count());

            Assert.AreEqual("RelatedHSNum", result.Value.ToList()[0].RelatedHS.ToList()[0].RelatedHSNum);
            Assert.AreEqual("Note", result.Value.ToList()[0].RelatedHS.ToList()[0].Note);
            Assert.AreEqual(new DateTime(2021, 1, 1), result.Value.ToList()[0].RelatedHS.ToList()[0].StartDate);
            Assert.AreEqual(new DateTime(2021, 1, 2), result.Value.ToList()[0].RelatedHS.ToList()[0].EndDate);
        }

        [TestMethod]
        public async Task GetHSDetails_QuotasFound_MapsToEntity()
        {
            var hsDetailToReturn = new List<HSDetails>() {
                new HSDetails(new Guid("775F462C-1FB5-48A9-A007-000061D5D212"))
                {
                    HSNumber = "123"
                }
            };

            var quotasToReturn = new List<HSDetails>() {
                new HSDetails(new Guid("775F462C-1FB5-48A9-A007-000061D5D212"))
                {
                    HSNumber = "123",
                    Quotas = new List<ChargeQuota>
                    {
                        new ChargeQuota
                        {
                            Level = "1000",
                            Type = "type",
                            UOM = "kg",
                        }
                    }
                },
            };

            var hsTempDataDao = new Mock<ITempDataDao>();
            var hsDetailsDao = new Mock<IHSGenericDao<HSDetailsSqlDto>>();
            var hsDescriptionsDao = new Mock<IHSGenericDao<HSDescriptionSqlDto>>();
            var relatedHSDao = new Mock<IHSGenericDao<RelatedHSSqlDto>>();
            var quotasDao = new Mock<IHSGenericDao<ChargeQuotaSqlDto>>();
            var notesDao = new Mock<IHSGenericDao<PcNoteSqlDto>>();
            var bindingRullings = new Mock<IBindingRullingsDao>();

            hsTempDataDao
                .Setup(i => i.LoadTempTable(It.IsAny<Guid[]>(), It.IsAny<string>()))
                .ReturnsAsync(default(Result));
            hsDetailsDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(hsDetailToReturn);
            hsDescriptionsDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            relatedHSDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            quotasDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(quotasToReturn);
            notesDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            bindingRullings
                .Setup(i => i.GetBindingRullings(It.IsAny<string>()))
                .ReturnsAsync(new List<BindingRullingsSqlDto>());
            bindingRullings
                .Setup(i => i.GetBindingRullingsText(It.IsAny<string>()))
                .ReturnsAsync(new List<BindingRullingsTextSqlDto>());

            var hsDetailsSqlRepository = new HsDetailsSqlRepository(
                hsTempDataDao.Object,
                hsDetailsDao.Object,
                hsDescriptionsDao.Object,
                relatedHSDao.Object,
                quotasDao.Object,
                notesDao.Object,
                bindingRullings.Object,
                LoggerMock.Object);
            var result = await hsDetailsSqlRepository.GetHSDetails(new Guid[1] { new Guid("775F462C-1FB5-48A9-A007-000061D5D212") });

            Assert.IsTrue(result.IsOk);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(hsDetailToReturn.Count, result.Value.ToList().Count());
            Assert.AreEqual(quotasToReturn.Count, result.Value.ToList()[0].Quotas.Count());

            Assert.AreEqual("1000", result.Value.ToList()[0].Quotas.ToList()[0].Level);
            Assert.AreEqual("type", result.Value.ToList()[0].Quotas.ToList()[0].Type);
            Assert.AreEqual("kg", result.Value.ToList()[0].Quotas.ToList()[0].UOM);
            Assert.IsNull(result.Value.ToList()[0].Quotas.ToList()[0].FillDate);
            Assert.IsNull(result.Value.ToList()[0].Quotas.ToList()[0].QuotaStartDate);
            Assert.IsNull(result.Value.ToList()[0].Quotas.ToList()[0].QuotaEndDate);
            Assert.IsNull(result.Value.ToList()[0].Quotas.ToList()[0].IssuingCountries);
            Assert.IsNull(result.Value.ToList()[0].Quotas.ToList()[0].ApplicableCountries);
        }

        [TestMethod]
        public async Task GetHSDetails_NotesFound_MapsToEntity()
        {
            var prodClassificationDetailGUID = Guid.NewGuid();
            var hsDetailToReturn = new List<HSDetails>() {
                new HSDetails(prodClassificationDetailGUID)
                {
                    HSNumber = "123"
                }
            };
            var bindingRulingGUID1 = Guid.NewGuid();
            var bindingRulingGUID2 = Guid.NewGuid();

            var notesToReturn = new List<BindingRullingsSqlDto>
            {
                new BindingRullingsSqlDto
                {
                    ProdClassificationDetailGUID = prodClassificationDetailGUID,
                    BindingRulingGUID = bindingRulingGUID1
                },
                new BindingRullingsSqlDto
                {
                    ProdClassificationDetailGUID = prodClassificationDetailGUID,
                    BindingRulingGUID = bindingRulingGUID2
                }
            };

            var notesTextsToReturn = new List<BindingRullingsTextSqlDto>
            {
                new BindingRullingsTextSqlDto
                {
                    BindingRulingGUID = bindingRulingGUID1
                },
                new BindingRullingsTextSqlDto
                {
                    BindingRulingGUID = bindingRulingGUID1
                },
                new BindingRullingsTextSqlDto
                {
                    BindingRulingGUID = bindingRulingGUID2
                }
            };

            var hsTempDataDao = new Mock<ITempDataDao>();
            var hsDetailsDao = new Mock<IHSGenericDao<HSDetailsSqlDto>>();
            var hsDescriptionsDao = new Mock<IHSGenericDao<HSDescriptionSqlDto>>();
            var relatedHSDao = new Mock<IHSGenericDao<RelatedHSSqlDto>>();
            var quotasDao = new Mock<IHSGenericDao<ChargeQuotaSqlDto>>();
            var notesDao = new Mock<IHSGenericDao<PcNoteSqlDto>>();
            var bindingRullings = new Mock<IBindingRullingsDao>();

            hsTempDataDao
                .Setup(i => i.LoadTempTable(It.IsAny<Guid[]>(), It.IsAny<string>()))
                .ReturnsAsync(default(Result));
            hsDetailsDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(hsDetailToReturn);
            hsDescriptionsDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            relatedHSDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            quotasDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            notesDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            bindingRullings
                .Setup(i => i.GetBindingRullings(It.IsAny<string>()))
                .ReturnsAsync(notesToReturn);
            bindingRullings
                .Setup(i => i.GetBindingRullingsText(It.IsAny<string>()))
                .ReturnsAsync(notesTextsToReturn);

            var hsDetailsSqlRepository = new HsDetailsSqlRepository(
                hsTempDataDao.Object,
                hsDetailsDao.Object,
                hsDescriptionsDao.Object,
                relatedHSDao.Object,
                quotasDao.Object,
                notesDao.Object,
                bindingRullings.Object,
                LoggerMock.Object);
            var result = await hsDetailsSqlRepository.GetHSDetails(new Guid[1] { prodClassificationDetailGUID });

            Assert.IsTrue(result.IsOk);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(hsDetailToReturn.Count, result.Value.ToList().Count());
            Assert.AreEqual(notesToReturn.Count(), result.Value.ToList()[0].Rullings.Count());

            var returnedRullings = result.Value.ToList()[0].Rullings.ToList();
            Assert.AreEqual(prodClassificationDetailGUID, returnedRullings[0].ProdClassificationDetailGUID);
            Assert.AreEqual(2, returnedRullings[0].Text.Count());
            Assert.AreEqual(prodClassificationDetailGUID, returnedRullings[1].ProdClassificationDetailGUID);
            Assert.AreEqual(1, returnedRullings[1].Text.Count());
        }

        [TestMethod]
        public async Task GetHSDetails_RullingsFound_MapsToEntity()
        {
            var hsDetailToReturn = new List<HSDetails>() {
                new HSDetails(new Guid("775F462C-1FB5-48A9-A007-000061D5D212"))
                {
                    HSNumber = "123"
                }
            };

            var rullingsToReturn = new List<HSDetails>() {
                new HSDetails(new Guid("775F462C-1FB5-48A9-A007-000061D5D212"))
                {
                    HSNumber = "123",
                    Notes = new List<PcNote>
                    {
                        new PcNote
                        {
                            HSNumber = "123",
                            NoteType = "node_type"
                        },
                        new PcNote
                        {
                            HSNumber = "123",
                            NoteType = "node_type_2"
                        },
                    }
                },
            };

            var hsTempDataDao = new Mock<ITempDataDao>();
            var hsDetailsDao = new Mock<IHSGenericDao<HSDetailsSqlDto>>();
            var hsDescriptionsDao = new Mock<IHSGenericDao<HSDescriptionSqlDto>>();
            var relatedHSDao = new Mock<IHSGenericDao<RelatedHSSqlDto>>();
            var quotasDao = new Mock<IHSGenericDao<ChargeQuotaSqlDto>>();
            var notesDao = new Mock<IHSGenericDao<PcNoteSqlDto>>();
            var bindingRullings = new Mock<IBindingRullingsDao>();

            hsTempDataDao
                .Setup(i => i.LoadTempTable(It.IsAny<Guid[]>(), It.IsAny<string>()))
                .ReturnsAsync(default(Result));
            hsDetailsDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(hsDetailToReturn);
            hsDescriptionsDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            relatedHSDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            quotasDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            notesDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(rullingsToReturn);
            bindingRullings
                .Setup(i => i.GetBindingRullings(It.IsAny<string>()))
                .ReturnsAsync(new List<BindingRullingsSqlDto>());
            bindingRullings
                .Setup(i => i.GetBindingRullingsText(It.IsAny<string>()))
                .ReturnsAsync(new List<BindingRullingsTextSqlDto>());

            var hsDetailsSqlRepository = new HsDetailsSqlRepository(
                hsTempDataDao.Object,
                hsDetailsDao.Object,
                hsDescriptionsDao.Object,
                relatedHSDao.Object,
                quotasDao.Object,
                notesDao.Object,
                bindingRullings.Object,
                LoggerMock.Object);
            var result = await hsDetailsSqlRepository.GetHSDetails(new Guid[1] { new Guid("775F462C-1FB5-48A9-A007-000061D5D212") });

            Assert.IsTrue(result.IsOk);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(hsDetailToReturn.Count, result.Value.ToList().Count());
            Assert.AreEqual(rullingsToReturn[0].Notes.Count(), result.Value.ToList()[0].Notes.Count());

            Assert.AreEqual("123", result.Value.ToList()[0].Notes.ToList()[0].HSNumber);
            Assert.AreEqual("node_type", result.Value.ToList()[0].Notes.ToList()[0].NoteType);
            Assert.IsNull(result.Value.ToList()[0].Notes.ToList()[0].NoteNumber);
            Assert.IsNull(result.Value.ToList()[0].Notes.ToList()[0].NoteStartDate);
            Assert.IsNull(result.Value.ToList()[0].Notes.ToList()[0].NoteEndDate);
            Assert.IsNull(result.Value.ToList()[0].Notes.ToList()[0].Text);
        }

        [TestMethod]
        public async Task GetHSDetails_RullingsGetBindingRullingsThrowsException_ResultFailed()
        {
            var hsTempDataDao = new Mock<ITempDataDao>();
            var hsDetailsDao = new Mock<IHSGenericDao<HSDetailsSqlDto>>();
            var hsDescriptionsDao = new Mock<IHSGenericDao<HSDescriptionSqlDto>>();
            var relatedHSDao = new Mock<IHSGenericDao<RelatedHSSqlDto>>();
            var quotasDao = new Mock<IHSGenericDao<ChargeQuotaSqlDto>>();
            var notesDao = new Mock<IHSGenericDao<PcNoteSqlDto>>();
            var bindingRullings = new Mock<IBindingRullingsDao>();

            hsTempDataDao
                .Setup(i => i.LoadTempTable(It.IsAny<Guid[]>(), It.IsAny<string>()))
                .ReturnsAsync(default(Result));
            hsDetailsDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            hsDescriptionsDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            relatedHSDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            quotasDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            notesDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            bindingRullings
                .Setup(i => i.GetBindingRullings(It.IsAny<string>()))
                .ThrowsAsync(new Exception());
            bindingRullings
                .Setup(i => i.GetBindingRullingsText(It.IsAny<string>()))
                .ReturnsAsync(new List<BindingRullingsTextSqlDto>());

            var hsDetailsSqlRepository = new HsDetailsSqlRepository(
                hsTempDataDao.Object,
                hsDetailsDao.Object,
                hsDescriptionsDao.Object,
                relatedHSDao.Object,
                quotasDao.Object,
                notesDao.Object,
                bindingRullings.Object,
                LoggerMock.Object);
            var result = await hsDetailsSqlRepository.GetHSDetails(new Guid[1] { new Guid("775F462C-1FB5-48A9-A007-000061D5D212") });

            bindingRullings
                .Verify(i => i.GetBindingRullingsText(It.IsAny<string>()), Times.Never);

            Assert.IsTrue(result.IsFailed);
        }

        [TestMethod]
        public async Task GetHSDetails_RullingsGetBindingRullingsTextThrowsException_ResultFailed()
        {
            var hsTempDataDao = new Mock<ITempDataDao>();
            var hsDetailsDao = new Mock<IHSGenericDao<HSDetailsSqlDto>>();
            var hsDescriptionsDao = new Mock<IHSGenericDao<HSDescriptionSqlDto>>();
            var relatedHSDao = new Mock<IHSGenericDao<RelatedHSSqlDto>>();
            var quotasDao = new Mock<IHSGenericDao<ChargeQuotaSqlDto>>();
            var notesDao = new Mock<IHSGenericDao<PcNoteSqlDto>>();
            var bindingRullings = new Mock<IBindingRullingsDao>();

            hsTempDataDao
                .Setup(i => i.LoadTempTable(It.IsAny<Guid[]>(), It.IsAny<string>()))
                .ReturnsAsync(default(Result));
            hsDetailsDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            hsDescriptionsDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            relatedHSDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            quotasDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            notesDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            bindingRullings
                .Setup(i => i.GetBindingRullings(It.IsAny<string>()))
                .ReturnsAsync(new List<BindingRullingsSqlDto>());
            bindingRullings
                .Setup(i => i.GetBindingRullingsText(It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            var hsDetailsSqlRepository = new HsDetailsSqlRepository(
                hsTempDataDao.Object,
                hsDetailsDao.Object,
                hsDescriptionsDao.Object,
                relatedHSDao.Object,
                quotasDao.Object,
                notesDao.Object,
                bindingRullings.Object,
                LoggerMock.Object);
            var result = await hsDetailsSqlRepository.GetHSDetails(new Guid[1] { new Guid("775F462C-1FB5-48A9-A007-000061D5D212") });

            Assert.IsTrue(result.IsFailed);
        }

        [TestMethod]
        public async Task GetHSDetails_LoadTempTableThrowsException_ReturnsFailedResult()
        {
            var hsTempDataDao = new Mock<ITempDataDao>();
            var hsDetailsDao = new Mock<IHSGenericDao<HSDetailsSqlDto>>();
            var hsDescriptionsDao = new Mock<IHSGenericDao<HSDescriptionSqlDto>>();
            var relatedHSDao = new Mock<IHSGenericDao<RelatedHSSqlDto>>();
            var quotasDao = new Mock<IHSGenericDao<ChargeQuotaSqlDto>>();
            var notesDao = new Mock<IHSGenericDao<PcNoteSqlDto>>();
            var bindingRullings = new Mock<IBindingRullingsDao>();

            hsTempDataDao
                .Setup(i => i.LoadTempTable(It.IsAny<Guid[]>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            var productClassificationRepository = new HsDetailsSqlRepository(
                hsTempDataDao.Object,
                hsDetailsDao.Object,
                hsDescriptionsDao.Object,
                relatedHSDao.Object,
                quotasDao.Object,
                notesDao.Object,
                bindingRullings.Object,
                LoggerMock.Object);
            var result = await productClassificationRepository.GetHSDetails(DefaultMethodArgument);

            Assert.IsTrue(result.IsFailed);
        }

        [TestMethod]
        public async Task GetHSDetails_GetProductClassificationThrowsException_ReturnsFailedResult()
        {
            var hsTempDataDao = new Mock<ITempDataDao>();
            var hsDetailsDao = new Mock<IHSGenericDao<HSDetailsSqlDto>>();
            var hsDescriptionsDao = new Mock<IHSGenericDao<HSDescriptionSqlDto>>();
            var relatedHSDao = new Mock<IHSGenericDao<RelatedHSSqlDto>>();
            var quotasDao = new Mock<IHSGenericDao<ChargeQuotaSqlDto>>();
            var notesDao = new Mock<IHSGenericDao<PcNoteSqlDto>>();
            var bindingRullings = new Mock<IBindingRullingsDao>();

            hsTempDataDao
                .Setup(i => i.LoadTempTable(It.IsAny<Guid[]>(), It.IsAny<string>()))
                .ReturnsAsync(default(Result));
            hsDetailsDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            var productClassificationRepository = new HsDetailsSqlRepository(
                hsTempDataDao.Object,
                hsDetailsDao.Object,
                hsDescriptionsDao.Object,
                relatedHSDao.Object,
                quotasDao.Object,
                notesDao.Object,
                bindingRullings.Object,
                LoggerMock.Object);
            var result = await productClassificationRepository.GetHSDetails(DefaultMethodArgument);

            hsTempDataDao
                .Verify(i => i.DropTempTable(It.IsAny<string>()), Times.Once);

            Assert.IsTrue(result.IsFailed);
        }

        [TestMethod]
        public async Task GetHSDetails_GetProductClassificationDetailsThrowsException_ReturnsFailedResult()
        {
            var hsTempDataDao = new Mock<ITempDataDao>();
            var hsDetailsDao = new Mock<IHSGenericDao<HSDetailsSqlDto>>();
            var hsDescriptionsDao = new Mock<IHSGenericDao<HSDescriptionSqlDto>>();
            var relatedHSDao = new Mock<IHSGenericDao<RelatedHSSqlDto>>();
            var quotasDao = new Mock<IHSGenericDao<ChargeQuotaSqlDto>>();
            var notesDao = new Mock<IHSGenericDao<PcNoteSqlDto>>();
            var bindingRullings = new Mock<IBindingRullingsDao>();

            hsTempDataDao
                .Setup(i => i.LoadTempTable(It.IsAny<Guid[]>(), It.IsAny<string>()))
                .ReturnsAsync(default(Result));
            hsDetailsDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            hsDescriptionsDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            var productClassificationRepository = new HsDetailsSqlRepository(
                hsTempDataDao.Object,
                hsDetailsDao.Object,
                hsDescriptionsDao.Object,
                relatedHSDao.Object,
                quotasDao.Object,
                notesDao.Object,
                bindingRullings.Object,
                LoggerMock.Object);
            var result = await productClassificationRepository.GetHSDetails(DefaultMethodArgument);

            hsTempDataDao
                .Verify(i => i.DropTempTable(It.IsAny<string>()), Times.Once);

            Assert.IsTrue(result.IsFailed);
        }

        [TestMethod]
        public async Task GetHSDetails_GetRelatedControlsThrowsException_ReturnsFailedResult()
        {
            var hsTempDataDao = new Mock<ITempDataDao>();
            var hsDetailsDao = new Mock<IHSGenericDao<HSDetailsSqlDto>>();
            var hsDescriptionsDao = new Mock<IHSGenericDao<HSDescriptionSqlDto>>();
            var relatedHSDao = new Mock<IHSGenericDao<RelatedHSSqlDto>>();
            var quotasDao = new Mock<IHSGenericDao<ChargeQuotaSqlDto>>();
            var notesDao = new Mock<IHSGenericDao<PcNoteSqlDto>>();
            var bindingRullings = new Mock<IBindingRullingsDao>();

            hsTempDataDao
                .Setup(i => i.LoadTempTable(It.IsAny<Guid[]>(), It.IsAny<string>()))
                .ReturnsAsync(default(Result));
            hsDetailsDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            hsDescriptionsDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            relatedHSDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            var productClassificationRepository = new HsDetailsSqlRepository(
                hsTempDataDao.Object,
                hsDetailsDao.Object,
                hsDescriptionsDao.Object,
                relatedHSDao.Object,
                quotasDao.Object,
                notesDao.Object,
                bindingRullings.Object,
                LoggerMock.Object);
            var result = await productClassificationRepository.GetHSDetails(DefaultMethodArgument);

            hsTempDataDao
                .Verify(i => i.DropTempTable(It.IsAny<string>()), Times.Once);

            Assert.IsTrue(result.IsFailed);
        }

        [TestMethod]
        public async Task GetHSDetails_GetQuotasThrowsException_ReturnsFailedResult()
        {
            var hsTempDataDao = new Mock<ITempDataDao>();
            var hsDetailsDao = new Mock<IHSGenericDao<HSDetailsSqlDto>>();
            var hsDescriptionsDao = new Mock<IHSGenericDao<HSDescriptionSqlDto>>();
            var relatedHSDao = new Mock<IHSGenericDao<RelatedHSSqlDto>>();
            var quotasDao = new Mock<IHSGenericDao<ChargeQuotaSqlDto>>();
            var notesDao = new Mock<IHSGenericDao<PcNoteSqlDto>>();
            var bindingRullings = new Mock<IBindingRullingsDao>();

            hsTempDataDao
                .Setup(i => i.LoadTempTable(It.IsAny<Guid[]>(), It.IsAny<string>()))
                .ReturnsAsync(default(Result));
            hsDetailsDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            hsDescriptionsDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            relatedHSDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            quotasDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            var productClassificationRepository = new HsDetailsSqlRepository(
                hsTempDataDao.Object,
                hsDetailsDao.Object,
                hsDescriptionsDao.Object,
                relatedHSDao.Object,
                quotasDao.Object,
                notesDao.Object,
                bindingRullings.Object,
                LoggerMock.Object);
            var result = await productClassificationRepository.GetHSDetails(DefaultMethodArgument);

            hsTempDataDao
                .Verify(i => i.DropTempTable(It.IsAny<string>()), Times.Once);

            Assert.IsTrue(result.IsFailed);
        }

        [TestMethod]
        public async Task GetHSDetails_GetNotesThrowsException_ReturnsFailedResult()
        {
            var hsTempDataDao = new Mock<ITempDataDao>();
            var hsDetailsDao = new Mock<IHSGenericDao<HSDetailsSqlDto>>();
            var hsDescriptionsDao = new Mock<IHSGenericDao<HSDescriptionSqlDto>>();
            var relatedHSDao = new Mock<IHSGenericDao<RelatedHSSqlDto>>();
            var quotasDao = new Mock<IHSGenericDao<ChargeQuotaSqlDto>>();
            var notesDao = new Mock<IHSGenericDao<PcNoteSqlDto>>();
            var bindingRullings = new Mock<IBindingRullingsDao>();

            hsTempDataDao
                .Setup(i => i.LoadTempTable(It.IsAny<Guid[]>(), It.IsAny<string>()))
                .ReturnsAsync(default(Result));
            hsDetailsDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            hsDescriptionsDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            relatedHSDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            quotasDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            notesDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            var productClassificationRepository = new HsDetailsSqlRepository(
                hsTempDataDao.Object,
                hsDetailsDao.Object,
                hsDescriptionsDao.Object,
                relatedHSDao.Object,
                quotasDao.Object,
                notesDao.Object,
                bindingRullings.Object,
                LoggerMock.Object);
            var result = await productClassificationRepository.GetHSDetails(DefaultMethodArgument);

            hsTempDataDao
                .Verify(i => i.DropTempTable(It.IsAny<string>()), Times.Once);

            Assert.IsTrue(result.IsFailed);
        }

        [TestMethod]
        public async Task GetHSDetails_DropTempTableThrowsException_ReturnsFailedResult()
        {
            var hsTempDataDao = new Mock<ITempDataDao>();
            var hsDetailsDao = new Mock<IHSGenericDao<HSDetailsSqlDto>>();
            var hsDescriptionsDao = new Mock<IHSGenericDao<HSDescriptionSqlDto>>();
            var relatedHSDao = new Mock<IHSGenericDao<RelatedHSSqlDto>>();
            var quotasDao = new Mock<IHSGenericDao<ChargeQuotaSqlDto>>();
            var notesDao = new Mock<IHSGenericDao<PcNoteSqlDto>>();
            var bindingRullings = new Mock<IBindingRullingsDao>();

            hsTempDataDao
                .Setup(i => i.LoadTempTable(It.IsAny<Guid[]>(), It.IsAny<string>()))
                .ReturnsAsync(default(Result));
            hsTempDataDao
                .SetupSequence(i => i.DropTempTable(It.IsAny<string>()))
                .ThrowsAsync(new Exception())
                .ReturnsAsync(default(Result<int>))
                .ReturnsAsync(default(Result<int>));
            hsDetailsDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            hsDescriptionsDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            relatedHSDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            quotasDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            notesDao
                .Setup(i => i.GetAll(It.IsAny<string>()))
                .ReturnsAsync(new List<HSDetails>());
            bindingRullings
                .Setup(i => i.GetBindingRullings(It.IsAny<string>()))
                .ReturnsAsync(new List<BindingRullingsSqlDto>());
            bindingRullings
                .Setup(i => i.GetBindingRullingsText(It.IsAny<string>()))
                .ReturnsAsync(new List<BindingRullingsTextSqlDto>());

            var productClassificationRepository = new HsDetailsSqlRepository(
                hsTempDataDao.Object,
                hsDetailsDao.Object,
                hsDescriptionsDao.Object,
                relatedHSDao.Object,
                quotasDao.Object,
                notesDao.Object,
                bindingRullings.Object,
                LoggerMock.Object);
            var result = await productClassificationRepository.GetHSDetails(DefaultMethodArgument);

            hsTempDataDao
                .Verify(i => i.DropTempTable(It.IsAny<string>()), Times.AtLeastOnce);
            hsTempDataDao.Verify();

            Assert.IsTrue(result.IsFailed);
        }

        [TestMethod]
        public async Task GetHSDetails_LoadTempTableThrowsException_DropTempTableCalled_ReturnsFailedResult()
        {
            var hsTempDataDao = new Mock<ITempDataDao>();
            var hsDetailsDao = new Mock<IHSGenericDao<HSDetailsSqlDto>>();
            var hsDescriptionsDao = new Mock<IHSGenericDao<HSDescriptionSqlDto>>();
            var relatedHSDao = new Mock<IHSGenericDao<RelatedHSSqlDto>>();
            var quotasDao = new Mock<IHSGenericDao<ChargeQuotaSqlDto>>();
            var notesDao = new Mock<IHSGenericDao<PcNoteSqlDto>>();
            var bindingRullings = new Mock<IBindingRullingsDao>();

            hsTempDataDao
                .Setup(i => i.LoadTempTable(It.IsAny<Guid[]>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            var productClassificationRepository = new HsDetailsSqlRepository(
                hsTempDataDao.Object,
                hsDetailsDao.Object,
                hsDescriptionsDao.Object,
                relatedHSDao.Object,
                quotasDao.Object,
                notesDao.Object,
                bindingRullings.Object,
                LoggerMock.Object);
            var result = await productClassificationRepository.GetHSDetails(DefaultMethodArgument);

            hsTempDataDao
                .Verify(i => i.DropTempTable(It.IsAny<string>()), Times.Once);

            Assert.IsTrue(result.IsFailed);
        }
    }
}
