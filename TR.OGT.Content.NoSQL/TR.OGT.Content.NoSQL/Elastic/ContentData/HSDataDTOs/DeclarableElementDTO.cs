using System;
using System.Collections.Generic;
using System.Text;

namespace TR.OGT.Content.NoSQL.Elastic.ContentData.HSDataDTOs
{
    /// <summary>Specific questions or data elements tied to a classification (e.g. CAS number)</summary>
    public class DeclarableElementDTO
    {
        /// <summary>Language that a Declarable Element is published in</summary>
        public String CultureCode;
        /// <summary>What additional information needs to be reported</summary>
        public String DeclarableElement;
        /// <summary>Order in which the elements appear on screen or in reports</summary>
        public String SortOrder;
        /// <summary>Date that the element takes effect</summary>
        public DateTime? StartDate;
        /// <summary>Date that the element expires</summary>
        public DateTime? EndDate;
    }
}
