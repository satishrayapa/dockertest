using System;

namespace TR.OGT.QueryExtract.Infrastructure.Sql
{
    public class BindingRullingsTextSqlDto
    {
        public Guid BindingRulingGUID { get; set; }
        /// <summary>Language of the ruling text entry</summary>
        public string CultureCode { get; set; }
        /// <summary>Section of the ruling text (e.g. Introduction or Keywords)</summary>
        public string TextType { get; set; }
        /// <summary>Actual text of the ruling</summary>
        public string RulingText { get; set; }
    }
}