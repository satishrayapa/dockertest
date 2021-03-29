using System;

namespace TR.OGT.QueryExtract.Infrastructure.Sql
{
    public class BaseSqlDto
    {
        public static string ProdClassificationDetailGUIDColumnName = "ProdClassificationDetailGUID";
        public static string HSNumberColumnName = "HSNumber";

        /// <summary>Unique Identifier for this Tariff Number / Date Combination (from MSSQL)</summary>
        public Guid ProdClassificationDetailGUID { get; set; }
        /// <summary>Tariff number (can be fully qualified) for this set of information</summary>
        public string HSNumber { get; set; }
    }
}
