using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using TR.OGT.QueryExtract.Common;
using TR.OGT.QueryExtract.Infrastructure.Sql;
using TR.OGT.QueryExtract.Infrastructure;
using TR.OGT.Common.Sql;
using static TR.OGT.QueryExtract.Common.ResultExtensions;

namespace TR.OGT.QueryExtract.Data
{
    public class BindingRullingsDao : BaseDao, IBindingRullingsDao
    {
        private readonly ILogger<BindingRullingsDao> _logger;

        public BindingRullingsDao(
            IConfiguration configuration,
            ILogger<BindingRullingsDao> logger)
            : base(configuration.GetConnectionString("ContentDb"))
        {
            _logger = logger;
        }

        public async Task<Result<List<BindingRullingsSqlDto>>> GetBindingRullings(string tempTableName)
        {
            var sql = new SqlBuilder()
                .Select(BindingRullingsSql.Columns)
                .From(tempTableName)
                .Join(BindingRullingsSql.Joins)
                .Where(string.Empty)
                .Build();

            Helper helper = new SecurityHelper(this.ConnectionString, sql);

            return await TryCatch(helper.QueryAsTypeAsync<BindingRullingsSqlDto>)
                .OnError(r => _logger.LogError(r.Error, $"Error executing SQL script for type {typeof(BindingRullingsSqlDto).Name}", sql));
        }

        public async Task<Result<List<BindingRullingsTextSqlDto>>> GetBindingRullingsText(string brGuidsTempTable)
        {
            var sql = new SqlBuilder()
                .Select(BindingRullingsTextSql.Columns)
                .From(brGuidsTempTable)
                .Join(BindingRullingsTextSql.Joins)
                .Where(string.Empty)
                .Build();

            Helper helper = new SecurityHelper(this.ConnectionString, sql);

            return await TryCatch(helper.QueryAsTypeAsync<BindingRullingsTextSqlDto>)
                .OnError(r => _logger.LogError(r.Error, $"Error executing SQL script for type {typeof(BindingRullingsTextSqlDto).Name}", sql));
        }
    }
}