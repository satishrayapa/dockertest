using System.Collections.Generic;
using System.Threading.Tasks;
using TR.OGT.QueryExtract.Common;
using TR.OGT.QueryExtract.Infrastructure.Sql;

namespace TR.OGT.QueryExtract.Infrastructure
{
    public interface IBindingRullingsDao
    {
        Task<Result<List<BindingRullingsSqlDto>>> GetBindingRullings(string tempTableName);
        Task<Result<List<BindingRullingsTextSqlDto>>> GetBindingRullingsText(string brGuidsTempTable);
    }
}