using System;

namespace TR.OGT.QueryExtract.Domain
{
    public class RelatedHS
    {
        /// <summary>HS Number</summary>
        public string RelatedHSNum;
        /// <summary>How it's related to this HS</summary>
        public string Note;
        /// <summary>Date for which this relationship takes effect</summary>
        public DateTime? StartDate;
        /// <summary>Date for which this relationship expires</summary>
        public DateTime? EndDate;
    }
}
