using System.Collections.Generic;

namespace TR.OGT.QueryExtract.Data
{
    internal static class BindingRullingsTextSql
    {
        public static List<string> Columns => new List<string>
        {
            "r.BindingRulingGUID",
            "r.CultureCode",
            "r.DescriptionType as TextType",
            "r.Description as RulingText"
        };

        public static List<string> Joins => new List<string>
        {
            "JOIN [BindingRulings].[dbo].[tbrDescription ] r (nolock) on tmp.Id = r.BindingRulingGUID"
        };
    }
}