namespace TR.OGT.QueryExtract.Infrastructure.Sql
{
    public class HSDescriptionSqlDto : BaseSqlDto
    {
        /// <summary>Language of the Description</summary>
        public string CultureCode { get; set; }
        /// <summary>Text of the Description</summary>
        public string DescriptionText { get; set; }
        /// <summary>Order in which the descriptions are arranged</summary>
        public int SortOrder { get; set; }
        /// <summary>Whether or not this is available on screen</summary>
        public string DisplayFlag { get; set; }
    }
}
