using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using TR.OGT.QueryExtract.Common;
using TR.OGT.QueryExtract.Infrastructure.Sql;
using TR.OGT.QueryExtract.Infrastructure;
using TR.OGT.QueryExtract.Domain;

namespace TR.OGT.QueryExtract.Data
{
    public class ChargeQuotaDao : BaseRelatedEntitiesDao<ChargeQuotaSqlDto>
    {
        public ChargeQuotaDao(
            IConfiguration configuration,
            ILogger<ChargeQuotaDao> logger)
            : base(configuration.GetConnectionString("ContentDb"), logger)
        {
        }

        public override async Task<Result<IEnumerable<HSDetails>>> GetAll(string tempTableName)
            => await GetAll(
                tempTableName,
                ChargeQuotaSql.Columns,
                ChargeQuotaSql.Joins,
                ChargeQuotaSql.Where,
                HsDetailsSqlMapper.Map);
    }
}