using System;
using System.Collections.Generic;

namespace TR.OGT.QueryExtract.Domain
{
    public class BindingRullings
    {
        public string HSNumber { get; set; }
        public Guid ProdClassificationDetailGUID { get; set; }
        /// <summary>Countries that are imposing the ruling</summary>
        public string IssuingCountries { get; set; }
        /// <summary>Code issued by the publishing agency to identify unique ruling</summary>
        public string RulingReferenceCode { get; set; }
        /// <summary>The type of ruling (CLASSIFICATION | ORIGIN)</summary>
        public string RulingType { get; set; }
        /// <summary>Date for which the Ruling takes effect</summary>
        public DateTime? RulingStartDate { get; set; }
        /// <summary>Date for which the Ruling expires</summary>
        public DateTime? RulingEndDate { get; set; }
        public IEnumerable<BindingRullingsText> Text { get; set; }
    }

    public class BindingRullingsText
    {
        /// <summary>Language of the ruling text entry</summary>
        public string CultureCode { get; set; }
        /// <summary>Section of the ruling text (e.g. Introduction or Keywords)</summary>
        public string TextType { get; set; }
        /// <summary>Actual text of the ruling</summary>
        public string RulingText { get; set; }
    }
}
