using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TR.OGT.ChangeLedger.Common;
using TR.OGT.ChangeLedger.Data.Dao.Abstractions;
using TR.OGT.ChangeLedger.Data.Interfaces;

namespace TR.OGT.ChangeLedger.Data.Repositories
{
    public class VersionRepository : IVersionRepository
    {
        private readonly IVersionsDynamoDbDao _dynamoDbDao;
        private readonly IVersionsSqlDao _contentDbDao;

        public VersionRepository(IVersionsDynamoDbDao dynamoDao, IVersionsSqlDao contentDbDao)
        {
            _dynamoDbDao = dynamoDao;
            _contentDbDao = contentDbDao;
        }

        public async Task<Result<Dictionary<string, long>>> GetCurrentLastVersionsAsync()
        {
            var versionsResult = await _contentDbDao.GetLastVersionsAsync();
            var data = versionsResult.Value
                .ToDictionary(key => key.TableName, val => val.LastVersion);

            return data;
        }

        public Task<Result<Dictionary<string, long>>> GetLatestVersionsAsync()
        {
            return _dynamoDbDao.GetLastVersionsAsync();
        }

        public Task<Result> SaveLatestVersionsAsync(Dictionary<string, long> versions)
        {
            return _dynamoDbDao.SaveLastVersionsAsync(versions);
        }
    }
}
