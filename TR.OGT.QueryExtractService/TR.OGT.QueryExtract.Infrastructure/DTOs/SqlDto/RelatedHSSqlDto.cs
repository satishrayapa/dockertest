using System;

namespace TR.OGT.QueryExtract.Infrastructure.Sql
{
    public class RelatedHSSqlDto : BaseSqlDto
    {
        /// <summary>HS Number</summary>
        public string RelatedHSNum { get; set; }
        /// <summary>How it's related to this HS</summary>
        public string Note { get; set; }
        /// <summary>Date for which this relationship takes effect</summary>
        public DateTime? StartDate { get; set; }
        /// <summary>Date for which this relationship expires</summary>
        public DateTime? EndDate { get; set; }
    }
}