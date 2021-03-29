using System.Collections.Generic;
using System.Threading.Tasks;
using TR.OGT.ChangeLedger.Common;
using TR.OGT.ChangeLedger.Data.Dao.Abstractions;
using TR.OGT.ChangeLedger.Data.Interfaces;
using TR.OGT.ChangeLedger.Domain.Entities;

namespace TR.OGT.ChangeLedger.Data.Repositories
{
    public class RulingsRepository : BaseContentRepository, IRulingsRepository
    {
        private readonly IRulingEntityDao _rulingsDao;
        private readonly IRulingEntityDao _rulingsDescriptionDao;
        private readonly IRulingEntityDao _rulingsTypeDao;
        private readonly IRulingEntityDao _rulingsMapDao;

        private readonly DistinctCollection<ChangeTrackingChange> _deduplication;

        public RulingsRepository(
            IRulingEntityDao rulingsDao,
            IRulingEntityDao rulingsDescriptionDao,
            IRulingEntityDao rulingsTypeDao,
            IRulingEntityDao rulingsMapDao)
        {
            _rulingsDao = rulingsDao;
            _rulingsDescriptionDao = rulingsDescriptionDao;
            _rulingsTypeDao = rulingsTypeDao;
            _rulingsMapDao = rulingsMapDao;

            _deduplication = new DistinctCollection<ChangeTrackingChange>();
        }

        public override async Task<Result<IEnumerable<ChangeTrackingChange>>> GetContentAsync(
            Dictionary<string, long> lastVersions,
            Dictionary<string, long> currentLastVersions)
        {
            var mapChanges = await GetDaoContentAsync(_rulingsMapDao, lastVersions, currentLastVersions);
            _deduplication.AddRange(mapChanges);

            var rChanges = await GetDaoContentAsync(_rulingsDao, lastVersions, currentLastVersions);
            _deduplication.AddRange(rChanges);

            var rdChanges = await GetDaoContentAsync(_rulingsDescriptionDao, lastVersions, currentLastVersions);
            _deduplication.AddRange(rdChanges);

            var rtChanges = await GetDaoContentAsync(_rulingsTypeDao, lastVersions, currentLastVersions);
            _deduplication.AddRange(rtChanges);

            return _deduplication;
        }
    }
}
