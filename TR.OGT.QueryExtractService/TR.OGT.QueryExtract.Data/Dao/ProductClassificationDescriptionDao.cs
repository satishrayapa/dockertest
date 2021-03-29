using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using TR.OGT.QueryExtract.Common;
using TR.OGT.QueryExtract.Domain;
using TR.OGT.QueryExtract.Infrastructure;
using TR.OGT.QueryExtract.Infrastructure.Sql;

namespace TR.OGT.QueryExtract.Data
{
    public class ProductClassificationDescriptionDao : BaseRelatedEntitiesDao<HSDescriptionSqlDto>
    {
        public ProductClassificationDescriptionDao(
            IConfiguration configuration,
            ILogger<ProductClassificationDescriptionDao> logger)
            : base(configuration.GetConnectionString("ContentDb"), logger)
        {
        }

        public override async Task<Result<IEnumerable<HSDetails>>> GetAll(string tempTableName)
            => await GetAll(
                tempTableName,
                ProductClassificationDescriptionSql.Columns,
                ProductClassificationDescriptionSql.Joins,
                ProductClassificationDescriptionSql.Where,
                HsDetailsSqlMapper.Map);
    }
}
