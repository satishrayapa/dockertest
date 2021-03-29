using System;
using System.Data.SqlClient;

namespace TR.OGT.QueryExtract.Data
{
    public abstract class BaseDao
    {
        public string ConnectionString { get; }

        public BaseDao(string connectionString)
        {
            ConnectionString = connectionString;
        }

        protected SqlConnection CreateConnection() => new SqlConnection(ConnectionString);
    }
}
