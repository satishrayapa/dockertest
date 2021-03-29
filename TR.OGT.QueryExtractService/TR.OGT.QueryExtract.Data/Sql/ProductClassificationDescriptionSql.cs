using System.Collections.Generic;
using TR.OGT.QueryExtract.Infrastructure.Sql;

namespace TR.OGT.QueryExtract.Data
{
    internal static class ProductClassificationDescriptionSql
    {
        public static List<string> Columns => new List<string>
        {
            $"pcdesc.Number as {BaseSqlDto.HSNumberColumnName}",
            $"pcdesc.ProdClassificationDetailGUID as {BaseSqlDto.ProdClassificationDetailGUIDColumnName}",
            "pcdesc.CultureCode",
            "pcdesc.Description as DescriptionText",
            "pcdesc.SortOrder",
            "isnull(pcdesc.GovernmentIssuedID,'Y') as DisplayFlag"
        };

        public static List<string> Joins => new List<string>
        {
            "JOIN tPcProductClassificationDetail pcd (nolock) on tmp.Id = pcd.ProdClassificationDetailGUID",
            "JOIN tPcProductClassificationDescription pcdesc (nolock) on pcdesc.ProdClassificationDetailGUID = pcd.ProdClassificationDetailGUID"
        };

        public static string Where => "pcdesc.DescTypeCode = 'SHORT'";
    }
}
