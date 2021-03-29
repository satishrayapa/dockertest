using Nest;

namespace TR.OGT.QueryExtract.Infrastructure.Elastic
{
    /// <summary>Generic object for holding a multicultural description</summary>
    public class DescriptionHSElasticDto
    {
        /// <summary>Language of the Description</summary>
        [Keyword(Name = nameof(CultureCode))]
        public string CultureCode { get; set; }

        /// <summary>Text of the Description</summary>
        [Keyword(Name = nameof(DescriptionText))]
        public string DescriptionText { get; set; }

        /// <summary>Order in which the descriptions are arranged</summary>
        [Keyword(Name = nameof(SortOrder))]
        public string SortOrder { get; set; }

        /// <summary>Whether or not this is available on screen</summary>
        [Keyword(Name = nameof(DisplayFlag))]
        public string DisplayFlag { get; set; }
    }
}
