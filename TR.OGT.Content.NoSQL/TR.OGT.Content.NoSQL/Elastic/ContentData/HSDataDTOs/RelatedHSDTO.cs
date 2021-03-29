using System;
using System.Collections.Generic;
using System.Text;

namespace TR.OGT.Content.NoSQL.Elastic.ContentData.HSDataDTOs
{
    /// <summary>Another HS that is associated with this HS</summary>
    public class RelatedHSDTO
    {
        /// <summary>HS Number</summary>
        public String RelatedHSNum;
        /// <summary>How it's related to this HS</summary>
        public String Note;
        /// <summary>Date for which this relationship takes effect</summary>
        public DateTime? StartDate;
        /// <summary>Date for which this relationship expires</summary>
        public DateTime? EndDate;
    }
}
