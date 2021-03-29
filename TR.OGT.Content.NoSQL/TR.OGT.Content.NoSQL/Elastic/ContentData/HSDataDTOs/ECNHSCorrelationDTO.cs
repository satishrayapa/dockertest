using System;
using System.Collections.Generic;
using System.Text;

namespace TR.OGT.Content.NoSQL.Elastic.ContentData.HSDataDTOs
{
    /// <summary>ECN - HS Correlation for a given HS (on the top level ProdClassificationDetailGUID)</summary>
    public class ECNHSCorrelationDTO
    {
        /// <summary>ECN Number that's correlated to this HS</summary>
        public String ECNNum;
        /// <summary>ID of the list that the ECN comes from (in Content SQL)</summary>
        public String RegListID;
        /// <summary>Name of the list that the ECN comes from (in Content SQL)</summary>
        public String RegListName;
        /// <summary>Country of the list that the ECN comes from (in Content SQL)</summary>
        public String CountryCode;
        /// <summary>Date that the correlation becomes effective</summary>
        public DateTime? StartDate;
        /// <summary>Date that the correlation expires</summary>
        public DateTime? EndDate;
    }
}
