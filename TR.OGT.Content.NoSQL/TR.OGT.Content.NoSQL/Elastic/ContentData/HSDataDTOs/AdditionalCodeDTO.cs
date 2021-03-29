using System;
using System.Collections.Generic;
using System.Text;

namespace TR.OGT.Content.NoSQL.Elastic.ContentData.HSDataDTOs
{
    /// <summary>Additional codes that supplement the tariff number in classification (may be appended or just reported)</summary>
    public class AdditionalCodeDTO
    {
        /// <summary>Additional code that is added for additional detail</summary>
        public String AdditionalCode;
        /// <summary>Country Codes that this Additional Code applies to</summary>
        public List<String> ApplicableCountries;
        /// <summary>Indicates whether to append the code to the tariff number</summary>
        public Boolean Append;
        /// <summary>Indicates whether the Additional Code is mandatory for reporting</summary>
        public Boolean Mandatory;
        /// <summary>Naming convention specified by publishing agency</summary>
        public String Type;
        /// <summary>Date the additional code becomes effective</summary>
        public DateTime? StartDate;
        /// <summary>Date the additional code expires</summary>
        public DateTime? EndDate;
        /// <summary>Plain language description of the additional code</summary>
        public List<DescriptionDTO> Description;
    }   
}
