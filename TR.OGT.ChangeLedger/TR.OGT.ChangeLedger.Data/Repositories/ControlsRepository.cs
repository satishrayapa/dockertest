using System.Collections.Generic;
using System.Threading.Tasks;
using TR.OGT.ChangeLedger.Common;
using TR.OGT.ChangeLedger.Data.Dao.Abstractions;
using TR.OGT.ChangeLedger.Data.Interfaces;
using TR.OGT.ChangeLedger.Domain.Entities;

namespace TR.OGT.ChangeLedger.Data.Repositories
{
    public class ControlsRepository : BaseContentRepository, IControlsRepository
    {
        private readonly IControlAgenciesRepository _agenciesRepository;

        private readonly IControlEntityDao _detailControlDao;
        private readonly IControlEntityDao _detailControlMapDao;
        private readonly IControlEntityDao _documentControlMapDao;

        private readonly DistinctCollection<ChangeTrackingChange> _deduplication;

        public ControlsRepository(
            IControlEntityDao detailControlDao,
            IControlEntityDao detailControlMapDao,
            IControlEntityDao documentControlDao,
            IControlAgenciesRepository agenciesRepository)
        {
            _detailControlDao = detailControlDao;
            _detailControlMapDao = detailControlMapDao;
            _documentControlMapDao = documentControlDao;

            _agenciesRepository = agenciesRepository;

            _deduplication = new DistinctCollection<ChangeTrackingChange>();
        }

        public override async Task<Result<IEnumerable<ChangeTrackingChange>>> GetContentAsync(
            Dictionary<string, long> lastVersions,
            Dictionary<string, long> currentLastVersions)
        {
            var controlChanges = await GetDaoContentAsync(_detailControlDao, lastVersions, currentLastVersions);
            _deduplication.AddRange(controlChanges);

            var controlMapChanges = await GetDaoContentAsync(_detailControlMapDao, lastVersions, currentLastVersions);
            _deduplication.AddRange(controlMapChanges);

            var controlDocumentChanges = await GetDaoContentAsync(_documentControlMapDao, lastVersions, currentLastVersions);
            _deduplication.AddRange(controlDocumentChanges);

            var agenciesResult = await _agenciesRepository.GetContentAsync(lastVersions, currentLastVersions);
            _deduplication.AddRange(agenciesResult.Value);

            return _deduplication;
        }
    }
}
