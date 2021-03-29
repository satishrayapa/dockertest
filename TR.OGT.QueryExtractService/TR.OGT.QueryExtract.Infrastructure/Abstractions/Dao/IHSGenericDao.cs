using System.Collections.Generic;
using System.Threading.Tasks;
using TR.OGT.QueryExtract.Common;
using TR.OGT.QueryExtract.Domain;

namespace TR.OGT.QueryExtract.Infrastructure
{
    public interface IHSGenericDao<T>
    {
        Task<Result<IEnumerable<HSDetails>>> GetAll(string tempTableName);
    }
}
