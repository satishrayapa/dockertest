using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TR.OGT.ChangeLedger.Common;
using TR.OGT.ChangeLedger.Data.Dao.Abstractions;
using TR.OGT.ChangeLedger.Domain.Entities;

namespace TR.OGT.ChangeLedger.Data.Repositories
{
    public abstract class BaseContentRepository
    {
        private const long DefaultLastVersion = -1;

        protected const ChangeEventType DefaultDetailChildEventType = ChangeEventType.Update;

        protected long GetLastVersion(IReadOnlyDictionary<string, long> versions, string entity)
        {
            long version = DefaultLastVersion;
            if (versions.ContainsKey(entity))
            {
                version = versions[entity];
            }

            return version;
        }

        protected async Task<IEnumerable<ChangeTrackingChange>> GetDaoContentAsync(
            IChildEntityDao childDao,
            Dictionary<string, long> lastVersions,
            Dictionary<string, long> currentLastVersions)
        {
            long lastVersion = GetLastVersion(lastVersions, childDao.EntityName);
            long currentLastVersion = GetLastVersion(currentLastVersions, childDao.EntityName);

            var changesResult = await childDao.GetParentEntityAsync(lastVersion, currentLastVersion);
            IEnumerable<ChangeTrackingChange> changes = changesResult.Value
                .Select(e => new ChangeTrackingChange() { GuidChanged = e.Id, EventType = DefaultDetailChildEventType });

            return changes;
        }

        public abstract Task<Result<IEnumerable<ChangeTrackingChange>>> GetContentAsync(
            Dictionary<string, long> lastVersions,
            Dictionary<string, long> currentLastVersions);
    }
}
