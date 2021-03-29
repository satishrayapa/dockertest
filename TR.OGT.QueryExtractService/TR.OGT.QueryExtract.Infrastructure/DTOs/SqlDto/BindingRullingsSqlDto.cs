using System;

namespace TR.OGT.QueryExtract.Infrastructure.Sql
{
    public class BindingRullingsSqlDto : BaseSqlDto
    {
        public Guid BindingRulingGUID { get; set; }
        /// <summary>Countries that are imposing the ruling</summary>
        public string IssuingCountries { get; set; }
        /// <summary>Code issued by the publishing agency to identify unique ruling</summary>
        public string RulingReferenceCode { get; set; }
        /// <summary>The type of ruling (CLASSIFICATION | ORIGIN)</summary>
        public string RulingType { get; set; }
        /// <summary>Date for which the Ruling takes effect</summary>
        public DateTime? RulingStartDate { get; set; }
        /// <summary>Date for which the Ruling expires</summary>
        public DateTime? RulingEndDate { get; set; }
    }
}