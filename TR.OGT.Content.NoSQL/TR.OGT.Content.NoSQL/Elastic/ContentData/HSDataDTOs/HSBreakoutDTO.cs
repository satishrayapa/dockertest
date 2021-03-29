using System;
using System.Collections.Generic;
using System.Text;

namespace TR.OGT.Content.NoSQL.Elastic.ContentData.HSDataDTOs
{
    /// <summary>Object storing the Chapter/Heading/Subheading of this HS</summary>
    public class HSBreakoutDTO
    {
        /// <summary>First two digits</summary>
        public String Chapter;
        /// <summary>First four digits</summary>
        public String Heading;
        /// <summary>First six digits</summary>
        public String Subheading;
    }
}
