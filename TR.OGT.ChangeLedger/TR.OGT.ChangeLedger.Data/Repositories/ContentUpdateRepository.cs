using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TR.OGT.ChangeLedger.Common;
using TR.OGT.ChangeLedger.Data.Dao.Abstractions;
using TR.OGT.ChangeLedger.Data.Interfaces;
using TR.OGT.ChangeLedger.Domain.Entities;

namespace TR.OGT.ChangeLedger.Data.Repositories
{
    public class ContentUpdateRepository : BaseContentRepository, IContentUpdateRepository
    {
        private readonly ParentEntityDao _detailDao;
        private readonly IReadOnlyCollection<IChildEntityDao> _childDaos;
        private readonly IControlsRepository _controlsRepository;
        private readonly IRulingsRepository _rulingsRepository;

        private readonly DistinctCollection<ChangeTrackingChange> _deduplication;

        public ContentUpdateRepository(
            ParentEntityDao detailDao,
            IReadOnlyCollection<IChildEntityDao> childDaos,
            IControlsRepository controlsRepository,
            IRulingsRepository rulingsRepository)
        {
            _detailDao = detailDao;
            _childDaos = childDaos;
            _controlsRepository = controlsRepository;
            _rulingsRepository = rulingsRepository;

            _deduplication = new DistinctCollection<ChangeTrackingChange>();
        }

        public override async Task<Result<IEnumerable<ChangeTrackingChange>>> GetContentAsync(
            Dictionary<string, long> lastVersions, 
            Dictionary<string, long> currentLastVersion)
        {
            var parentResult = await FetchAndSaveParentDaos(lastVersions, currentLastVersion);
            if(parentResult.IsFailed)
            {
                return parentResult.Error;
            }

            var childDaosResult = await FetchAndSaveChildDaos(lastVersions, currentLastVersion);
            if(childDaosResult.IsFailed)
            {
                return childDaosResult.Error;
            }

            await FetchAndSaveChildRepositories(lastVersions, currentLastVersion);

            return new Result<IEnumerable<ChangeTrackingChange>>(_deduplication);
        }

        private async Task<Result> FetchAndSaveParentDaos(
            Dictionary<string, long> lastVersions,
            Dictionary<string, long> currentLastVersion)
        {
            long detailLastVersion = GetLastVersion(lastVersions, _detailDao.EntityName);
            long detailCurrentLastVersion = GetLastVersion(currentLastVersion, _detailDao.EntityName);

            var detailQueryResult = await _detailDao.GetContentAsync(detailLastVersion, detailCurrentLastVersion);
            var detailData = detailQueryResult.Value
                .Select(e => new ChangeTrackingChange() { GuidChanged = e.Id, EventType = e.EventType });
            _deduplication.AddRange(detailData);

            return default;
        }

        private async Task<Result> FetchAndSaveChildDaos(
            Dictionary<string, long> lastVersions, 
            Dictionary<string, long> currentLastVersion)
        {
            async Task<Result> SaveChildDaoContent(IChildEntityDao childDao)
            {
                var childChanges = await GetDaoContentAsync(childDao, lastVersions, currentLastVersion);
                _deduplication.AddRange(childChanges);

                return default;
            }

            var detailChildTasks = new List<Task<Result>>();
            foreach (var dao in _childDaos)
            {
                var daoCopy = dao;
                detailChildTasks.Add(SaveChildDaoContent(daoCopy));
            }
            var detailChildResults = await Task.WhenAll(detailChildTasks);

            if (detailChildResults.Any(r => r.IsFailed))
            {
                return detailChildResults
                    .First(r => r.IsFailed)
                    .Error;
            }

            return default;
        }

        private async Task FetchAndSaveChildRepositories(
            Dictionary<string, long> lastVersions,
            Dictionary<string, long> currentLastVersion)
        {
            var controlsResult = await _controlsRepository.GetContentAsync(lastVersions, currentLastVersion);
            _deduplication.AddRange(controlsResult.Value);

            var rulingsResult = await _rulingsRepository.GetContentAsync(lastVersions, currentLastVersion);
            _deduplication.AddRange(rulingsResult.Value);
        }
    }
}
