using System.Collections.Generic;
using System.Threading.Tasks;
using TR.OGT.ChangeLedger.Common;
using TR.OGT.ChangeLedger.Data.Dao.Abstractions;
using TR.OGT.ChangeLedger.Data.Interfaces;
using TR.OGT.ChangeLedger.Domain.Entities;

namespace TR.OGT.ChangeLedger.Data.Repositories
{
    public class ControlAgenciesRepository : BaseContentRepository, IControlAgenciesRepository
    {
        private readonly IControlAgencyEntityDao _agencyDao;
        private readonly IControlAgencyEntityDao _agencyDescriptionDao;
        private readonly IControlAgencyEntityDao _agencyMapDao;

        private readonly DistinctCollection<ChangeTrackingChange> _deduplication;

        public ControlAgenciesRepository(
            IControlAgencyEntityDao agencyDao, 
            IControlAgencyEntityDao agencyDescDao, 
            IControlAgencyEntityDao agencyMapDao)
        {
            _agencyDao = agencyDao;
            _agencyDescriptionDao = agencyDescDao;
            _agencyMapDao = agencyMapDao;

            _deduplication = new DistinctCollection<ChangeTrackingChange>();
        }

        public override async Task<Result<IEnumerable<ChangeTrackingChange>>> GetContentAsync(
            Dictionary<string, long> lastVersions,
            Dictionary<string, long> currentLastVersions)
        {
            var agencyMapChanges = await GetDaoContentAsync(_agencyMapDao, lastVersions, currentLastVersions);
            _deduplication.AddRange(agencyMapChanges);

            var agencyChanges = await GetDaoContentAsync(_agencyDao, lastVersions, currentLastVersions);
            _deduplication.AddRange(agencyChanges);

            var agencyDescriptionChanges = await GetDaoContentAsync(_agencyDescriptionDao, lastVersions, currentLastVersions);
            _deduplication.AddRange(agencyDescriptionChanges);

            return _deduplication;
        }
    }
}
