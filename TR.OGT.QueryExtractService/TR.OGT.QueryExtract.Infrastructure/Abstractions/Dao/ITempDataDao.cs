using System;
using System.Threading.Tasks;
using TR.OGT.QueryExtract.Common;

namespace TR.OGT.QueryExtract.Infrastructure
{
    public interface ITempDataDao
    {
        Task<Result> LoadTempTable(Guid[] ids, string tableName);
        Task<Result<int>> DropTempTable(string tempTableName);
    }
}
