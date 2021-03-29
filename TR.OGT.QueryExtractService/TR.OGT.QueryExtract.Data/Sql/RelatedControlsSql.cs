using System.Collections.Generic;
using TR.OGT.QueryExtract.Infrastructure.Sql;

namespace TR.OGT.QueryExtract.Data
{
    internal static class RelatedControlsSql
    {
        public static List<string> Columns => new List<string>
        {
            $"pcd.ProdClassificationDetailGUID as {BaseSqlDto.ProdClassificationDetailGUIDColumnName}",
            $"pcd.Number as {BaseSqlDto.HSNumberColumnName}",
            "rc.RelatedControlChildNumber as RelatedHSNum",
            "rc.RelatedControlNote as Note",
            "rc.RelatedControlEffDate as StartDate",
            "rc.RelatedControlExpDate as EndDate"
        };

        public static List<string> Joins => new List<string>
        {
            "JOIN tPcProductClassificationDetail pcd (nolock) on tmp.Id = pcd.ProdClassificationDetailGUID",
            "JOIN tPcRelatedControls rc (nolock) on pcd.ProdClassificationGUID = rc.ParentProductClassificationGUID AND pcd.Number = rc.ParentNumber"
        };

        public static string Where => "RelatedControlType = 'HS'";
    }
}