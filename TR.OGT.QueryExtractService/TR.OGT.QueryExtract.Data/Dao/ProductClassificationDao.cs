using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TR.OGT.QueryExtract.Common;
using TR.OGT.QueryExtract.Domain;
using TR.OGT.QueryExtract.Infrastructure;
using TR.OGT.QueryExtract.Infrastructure.Sql;

namespace TR.OGT.QueryExtract.Data
{
    public class ProductClassificationDao : BaseRelatedEntitiesDao<HSDetailsSqlDto>
    {
        public ProductClassificationDao(
            IConfiguration configuration,
            ILogger<ProductClassificationDao> logger)
            : base(configuration.GetConnectionString("ContentDb"), logger)
        {
        }

        public override async Task<Result<IEnumerable<HSDetails>>> GetAll(string tempTableName)
            => await GetAll(
                tempTableName,
                ProductClassificationSql.Columns,
                ProductClassificationSql.Joins,
                string.Empty,
                HsDetailsSqlMapper.Map);
    }
}