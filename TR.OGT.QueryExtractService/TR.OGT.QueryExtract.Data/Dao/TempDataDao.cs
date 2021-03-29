using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Threading.Tasks;
using TR.OGT.Common.Sql;
using TR.OGT.QueryExtract.Common;
using TR.OGT.QueryExtract.Infrastructure;
using static TR.OGT.QueryExtract.Common.ResultExtensions;

namespace TR.OGT.QueryExtract.Data
{
    public class TempDataDao : BaseDao, ITempDataDao
    {
        private readonly ILogger<TempDataDao> _logger;

        public TempDataDao(
            IConfiguration configuration,
            ILogger<TempDataDao> logger)
            : base(configuration.GetConnectionString("ContentDb"))
        {
            _logger = logger;
        }

        public async Task<Result<int>> DropTempTable(string tempTableName)
        {
            var sql = $"DROP TABLE {tempTableName}";
            Helper helper = new SecurityHelper(this.ConnectionString, sql);

            return await TryCatch(helper.ExecuteQueryAsync)
                .OnError(e => _logger.LogError(e.Error, "DropTempTable", sql));
        }

        public async Task<Result> LoadTempTable(Guid[] ids, string tableName)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var dataTableWithIds = CreateDataTable(ids);

            using var connection = this.CreateConnection();
            using var bulkCopy = new SqlBulkCopy(connection)
            {
                DestinationTableName = tableName
            };

            var command = new SqlCommand($"CREATE TABLE {tableName} ([Id] uniqueidentifier PRIMARY KEY)", connection);

            return await TryCatch(connection.OpenAsync)
                .Next(command.ExecuteNonQueryAsync)
                .Next(() => bulkCopy.WriteToServerAsync(dataTableWithIds))
                .OnError(r => _logger.LogError(r.Error, "LoadTempTable", command, ids));
        }

        private DataTable CreateDataTable(Guid[] ids)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Id", typeof(SqlGuid));

            foreach (var id in ids)
            {
                dataTable.Rows.Add(id);
            }

            return dataTable;
        }
    }
}
