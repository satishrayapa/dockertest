using System;
using System.Collections.Generic;
using System.Text;

namespace TR.OGT.Content.NoSQL.Elastic.ContentData.HSDataDTOs
{
    /// <summary>Generic object for holding a multicultural description</summary>
    public class DescriptionDTO
    {
        /// <summary>Language of the Description</summary>
        public String CultureCode;
        /// <summary>Text of the Description</summary>
        public String DescriptionText;
        /// <summary>Order in which the descriptions are arranged</summary>
        public String SortOrder;
        /// <summary>Whether or not this is available on screen</summary>
        public String DisplayFlag;
    }
}
