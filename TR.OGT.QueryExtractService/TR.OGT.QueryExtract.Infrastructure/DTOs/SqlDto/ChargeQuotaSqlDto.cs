using System;

namespace TR.OGT.QueryExtract.Infrastructure.Sql
{
    public class ChargeQuotaSqlDto : BaseSqlDto
    {
        /// <summary>Quantity or Value for the quota</summary>
        public string Level { get; set; }
        /// <summary>Describes (IMPORTVALUE | QUANTITY | NOLIMIT) what the level is</summary>
        public string Type { get; set; }
        /// <summary>Unit of Measure for the level</summary>
        public string UOM { get; set; }
        /// <summary>Date that the quota is filled</summary>
        public DateTime? FillDate { get; set; }
        /// <summary>Date the quota takes effect</summary>
        public DateTime? QuotaStartDate { get; set; }
        /// <summary>Date the quote expires</summary>
        public DateTime? QuotaEndDate { get; set; }
        /// <summary>Countries that are imposing the quota</summary>
        public string IssuingCountries { get; set; }
        /// <summary>Countries that the quota applies to</summary>
        public string ApplicableCountries { get; set; }
    }
}