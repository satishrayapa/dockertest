using Nest;
using System;
using System.Collections.Generic;

namespace TR.OGT.QueryExtract.Infrastructure.Elastic
{
    public class BindingRullingsElasticDto
    {
        [Keyword(Name = nameof(HSNumber))]
        public string HSNumber { get; set; }

        /// <summary>Countries that are imposing the ruling</summary>
        [Keyword(Name = nameof(IssuingCountries))]
        public string IssuingCountries { get; set; }

        /// <summary>Code issued by the publishing agency to identify unique ruling</summary>
        [Keyword(Name = nameof(RulingReferenceCode))]
        public string RulingReferenceCode { get; set; }

        /// <summary>The type of ruling (CLASSIFICATION | ORIGIN)</summary>
        [Keyword(Name = nameof(RulingType))]
        public string RulingType { get; set; }

        /// <summary>Date for which the Ruling takes effect</summary>
        [Date(Name = nameof(RulingStartDate))]
        public DateTime? RulingStartDate { get; set; }

        /// <summary>Date for which the Ruling expires</summary>
        [Date(Name = nameof(RulingEndDate))]
        public DateTime? RulingEndDate { get; set; }

        [Nested]
        [PropertyName(nameof(Text))]
        public IEnumerable<BindingRullingsTextElasticDto> Text { get; set; }
    }

    public class BindingRullingsTextElasticDto
    {
        /// <summary>Language of the ruling text entry</summary>
        [Keyword(Name = nameof(CultureCode))]
        public string CultureCode { get; set; }

        /// <summary>Section of the ruling text (e.g. Introduction or Keywords)</summary>
        [Keyword(Name = nameof(TextType))]
        public string TextType { get; set; }

        /// <summary>Actual text of the ruling</summary>
        [Text(Name = nameof(RulingText))]
        public string RulingText { get; set; }
    }
}