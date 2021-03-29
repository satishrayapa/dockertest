using System;
using System.Collections.Generic;
using System.Text;

namespace TR.OGT.Content.NoSQL.Elastic.ContentData.HSDataDTOs
{
    /// <summary>Collection of data elements that is specific to a ruling</summary>
    public class RulingDTO
    {
        /// <summary>HS Number (full or partial) associated to the ruling.
        /// If this is a partial number as a parent of a full number, 
        /// go look for more data at the HSData (parent) level before returning from the API</summary>
        public String HSNumber;
        /// <summary>Countries that are imposing the ruling</summary>
        public List<String> IssuingCountries;
        /// <summary>Code issued by the publishing agency to identify unique ruling</summary>
        public String RulingReferenceCode;
        /// <summary>The type of ruling (CLASSIFICATION | ORIGIN)</summary>
        public String RulingType;
        /// <summary>Date for which the Ruling takes effect</summary>
        public DateTime? RulingStartDate;
        /// <summary>Date for which the Ruling expires</summary>
        public DateTime? RulingEndDate;
        /// <summary>Multilingual collection of ruling text</summary>
        public List<RulingTextDTO> Text;
    }

    public class RulingTextDTO
    {
        /// <summary>Language of the ruling text entry</summary>
        public String CultureCode;
        /// <summary>Section of the ruling text (e.g. Introduction or Keywords)</summary>
        public String TextType;
        /// <summary>Actual text of the ruling</summary>
        public String RulingText;
    }
}
