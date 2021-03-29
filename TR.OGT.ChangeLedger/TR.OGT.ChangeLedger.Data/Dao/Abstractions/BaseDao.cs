using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TR.OGT.ChangeLedger.Common;
using TR.OGT.Common.Sql;

namespace TR.OGT.ChangeLedger.Data.Dao.Abstractions
{
    public abstract class BaseDao
    {
		private readonly string _connectionString;
		private readonly ILogger<BaseDao> _logger;

		public BaseDao(string connectionString, ILogger<BaseDao> logger)
		{
			_connectionString = connectionString;
			_logger = logger;
		}
		
		protected Helper CreateConnectionHelper(string query) => new SecurityHelper(_connectionString, query);

		protected async Task<Result<IEnumerable<T>>> GetEntityContentAsync<T>(string query, long lastVersion, long currentLastVersion)
		{
			if (currentLastVersion > lastVersion)
			{
				Helper helper = CreateConnectionHelper(query);
				helper.AddParameter("@prevLastVersion", lastVersion, DbType.Int64);
				helper.AddParameter("@currentLastVersion", currentLastVersion, DbType.Int64);

				List<T> changes = await helper.QueryAsTypeAsync<T>();

				// incorrect query
				if (changes == null)
				{
					_logger.LogError("get content query failed");
					throw new ArgumentException(nameof(query));
				}


				_logger.LogInformation("get content query succeeded");
				return changes;
			}

			_logger.LogInformation("current version is equal or lower than last version. empty collection returned.");
			var empty = Enumerable.Empty<T>();
			return new Result<IEnumerable<T>>(empty);
		}
	}
}
