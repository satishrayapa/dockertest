using System;

namespace TR.OGT.QueryExtract.Infrastructure.Sql
{
	/// <summary>
	/// HS Details top level flat entity
	/// </summary>
	public class HSDetailsSqlDto : BaseSqlDto
    {        
        /// <summary>First two digits</summary>
        public string Chapter { get; set; }
        /// <summary>First four digits</summary>
        public string Heading { get; set; }
        /// <summary>First six digits</summary>
        public string Subheading { get; set; }
        /// <summary>Units of Measure that are relevant for this Tariff Number</summary>
        public string UOMs { get; set; }
        /// <summary>Import or Export (or both)</summary>
        public string Uses { get; set; }
        /// <summary>Unique Identifier for this Tariff Schedule (from MSSQL)</summary>
        public Guid ProdClassificationGUID { get; set; }
        /// <summary>Country Codes associated with the tariff schedule</summary>
        public string CountryCodes { get; set; }
        /// <summary>Beginning of the period where tariff number takes effect</summary>
        public DateTime? StartDate { get; set; }
        /// <summary>End of the period where tariff number expires or is replaced</summary>
        public DateTime? EndDate { get; set; }
    }
}
