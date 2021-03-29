using System;
using System.Collections.Generic;
using System.Text;

namespace TR.OGT.Content.NoSQL.Elastic.ContentData.HSDataDTOs
{
    /// <summary>Maximum limits (value or quantity) on specific goods</summary>
    public class QuotaDTO
    {
        /// <summary>Quantity or Value for the quota</summary>
        public String Level;
        /// <summary>Describes (IMPORTVALUE | QUANTITY | NOLIMIT) what the level is</summary>
        public String Type;
        /// <summary>Unit of Measure for the level</summary>
        public String UOM;
        /// <summary>Date that the quota is filled</summary>
        public DateTime? FillDate;
        /// <summary>Date the quota takes effect</summary>
        public DateTime? QuotaStartDate;
        /// <summary>Date the quote expires</summary>
        public DateTime? QuotaEndDate;
        /// <summary>Countries that are imposing the quota</summary>
        public List<String> IssuingCountries;
        /// <summary>Countries that the quota applies to</summary>
        public List<String> ApplicableCountries;
    }
}
