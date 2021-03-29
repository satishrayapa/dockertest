using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TR.OGT.Common.Sql;
using TR.OGT.QueryExtract.Common;
using TR.OGT.QueryExtract.Domain;
using TR.OGT.QueryExtract.Infrastructure;
using TR.OGT.QueryExtract.Infrastructure.Sql;
using static TR.OGT.QueryExtract.Common.ResultExtensions;

namespace TR.OGT.QueryExtract.Data
{
    public abstract class BaseRelatedEntitiesDao<T> : BaseDao, IHSGenericDao<T>
        where T: BaseSqlDto
    {
        protected readonly ILogger Logger;

        public BaseRelatedEntitiesDao(
            string connectionString,
            ILogger logger)
            : base(connectionString)
        {
            Logger = logger;
        }

        public abstract Task<Result<IEnumerable<HSDetails>>> GetAll(string tempTableName);

        protected async Task<Result<IEnumerable<HSDetails>>> GetAll(
            string tempTableName,
            IEnumerable<string> columnsToSelect,
            IEnumerable<string> joins,
            string where,
            Func<IEnumerable<T>, IEnumerable<HSDetails>> map)
        {
            var sql = new SqlBuilder()
                .Select(columnsToSelect)
                .From(tempTableName)
                .Join(joins)
                .Where(where)
                .Build();

            Helper helper = new SecurityHelper(this.ConnectionString, sql);

            return await TryCatch(helper.QueryAsTypeAsync<T>)
                .Map(map)
                .OnError(r => Logger.LogError(r.Error, $"Error executing SQL script for type {typeof(T).Name}", sql));
        }
    }
}
