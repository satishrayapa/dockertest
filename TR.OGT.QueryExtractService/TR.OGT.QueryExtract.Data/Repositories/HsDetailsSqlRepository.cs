using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TR.OGT.QueryExtract.Common;
using TR.OGT.QueryExtract.Domain;
using TR.OGT.QueryExtract.Infrastructure;
using TR.OGT.QueryExtract.Infrastructure.Sql;
using static TR.OGT.QueryExtract.Common.ResultExtensions;

namespace TR.OGT.QueryExtract.Data
{
    public class HsDetailsSqlRepository : IHsDetailsSqlRepository
    {
        private readonly ITempDataDao _tempDataDao;
        private readonly IHSGenericDao<HSDetailsSqlDto> _hSDetailsDao;
        private readonly IHSGenericDao<HSDescriptionSqlDto> _hSDescriptionDao;
        private readonly IHSGenericDao<RelatedHSSqlDto> _relatedHSDao;
        private readonly IHSGenericDao<ChargeQuotaSqlDto> _quotasDao;
        private readonly IHSGenericDao<PcNoteSqlDto> _pcNotesDao;
        private readonly IBindingRullingsDao _bindingRullingsDao;

        private readonly ILogger<HsDetailsSqlRepository> _logger;

        public HsDetailsSqlRepository(
            ITempDataDao tempDataDao,
            IHSGenericDao<HSDetailsSqlDto> hSDetailsDao,
            IHSGenericDao<HSDescriptionSqlDto> hSDescriptionDao,
            IHSGenericDao<RelatedHSSqlDto> relatedHSDao,
            IHSGenericDao<ChargeQuotaSqlDto> quotasDao,
            IHSGenericDao<PcNoteSqlDto> pcNotesDao,
            IBindingRullingsDao bindingRullingsDao,
            ILogger<HsDetailsSqlRepository> logger)
        {
            _tempDataDao = tempDataDao;
            _hSDetailsDao = hSDetailsDao;
            _hSDescriptionDao = hSDescriptionDao;
            _relatedHSDao = relatedHSDao;
            _quotasDao = quotasDao;
            _pcNotesDao = pcNotesDao;
            _bindingRullingsDao = bindingRullingsDao;

            _logger = logger;
        }

        public async Task<Result<IReadOnlyCollection<HSDetails>>> GetHSDetails(Guid[] ids)
        {
            if (ids == null)
            {
                return new ArgumentNullException();
            }

            if (!ids.Any())
            {
                return Array.Empty<HSDetails>();
            }

            var tempTableName = $"TEMP_QUERY_EXTRACTOR_{Guid.NewGuid():N}";

            return await TryCatch(() => _tempDataDao.LoadTempTable(ids, tempTableName))
                .Next(() => BuildHSDetails(tempTableName))
                .Map(MergeHsDetails)
                .Tap(() => _tempDataDao.DropTempTable(tempTableName))
                .OnError(async result =>
                {
                    _logger.LogError(result.Error, "Attempting to drop table.");
                    await _tempDataDao.DropTempTable(tempTableName);
                });
        }

        private async Task<Result<IEnumerable<HSDetails>>> GetBindingRullings(string tempTableName)
        {
            var rullingsTempTableName = $"TEMP_QUERY_EXTRACTOR_RULLINGS{Guid.NewGuid():N}";
            var rullingsTask = _bindingRullingsDao.GetBindingRullings(tempTableName);

            return await TryCatch(() => rullingsTask)
                .Map(result => result.Select(i => i.BindingRulingGUID).Distinct().ToArray())
                .Bind(g => _tempDataDao.LoadTempTable(g, rullingsTempTableName))
                .Next(() => _bindingRullingsDao.GetBindingRullingsText(rullingsTempTableName))
                .Tap(() => _tempDataDao.DropTempTable(rullingsTempTableName))
                .Map(rullingTexts => BindingRullingsSqlMapper.Map(rullingsTask.Result.Value, rullingTexts))
                .OnError(async result =>
                {
                    _logger.LogError(result.Error, "Failed to pull rullings texts; Dropping temp table;");
                    await _tempDataDao.DropTempTable(rullingsTempTableName);
                });
        }

        private async Task<Result<IEnumerable<HSDetails>>> BuildHSDetails(string tempTableName)
        {
            var daoResult = new List<HSDetails>();
            var tasks = new List<Task<Result<IEnumerable<HSDetails>>>>
            {
                _hSDetailsDao.GetAll(tempTableName),
                _hSDescriptionDao.GetAll(tempTableName),
                _relatedHSDao.GetAll(tempTableName),
                _quotasDao.GetAll(tempTableName),
                _pcNotesDao.GetAll(tempTableName),
                GetBindingRullings(tempTableName)
            };

            while (tasks.Count != 0)
            {
                var completedTask = await Task.WhenAny(tasks);
                tasks.Remove(completedTask);

                var taskResult = await completedTask;

                if (taskResult.IsFailed)
                    return taskResult.Error;

                daoResult.AddRange(taskResult.Value);
            }
            return daoResult;
        }

        private IReadOnlyCollection<HSDetails> MergeHsDetails(IEnumerable<HSDetails> details)
        {
            var result = new List<HSDetails>();
            var group = details.GroupBy(g => g.ProdClassificationDetailGUID);
            foreach (var groupItem in group)
            {
                var entity = new HSDetails(groupItem.Key);
                foreach (var item in groupItem)
                {
                    entity.Merge(item);
                }
                result.Add(entity);
            }

            return result;
        }
    }
}
