using System.Collections.Generic;
using TR.OGT.QueryExtract.Infrastructure.Sql;

namespace TR.OGT.QueryExtract.Data
{

    internal static class ProductClassificationSql
    {
        public static List<string> Columns => new List<string>
        {
            $"pcd.Number as {BaseSqlDto.HSNumberColumnName}",
            "LEFT(pcd.Number,2) as Chapter",
            "LEFT(pcd.Number,4) as Heading",
            "LEFT(pcd.Number,6) as Subheading",
            $"pcd.ProdClassificationDetailGUID as {BaseSqlDto.ProdClassificationDetailGUIDColumnName}",
            "pc.ProdClassificationGUID",
            "pc.ProdClassificationName",
            "pcd.EffectivityDate as StartDate",
            "pcd.ExpirationDate as EndDate",
            //CTE candidates
            @"isnull(STUFF(replace((select distinct '#!' + ltrim(rtrim(replace(h.RptQtyUOM,',',''))) as 'data()'
                from tPcReportUnitofMeasure h with (nolock)
                where h.ProdClassificationDetailGUID = pcd.ProdClassificationDetailGUID
                for xml path('')),' #!',','),1,2,''),'') as UOMs",

            @"isnull(STUFF(replace((select distinct '#!' + ltrim(rtrim(h.ProdClassificationUse)) as 'data()'
                from tPcProductClassificationUse h with (nolock)
                where h.ProdClassificationGUID = pcd.ProdClassificationGUID
                for xml path('')),' #!',','),1,2,''),'') as Uses",

            @"isnull(STUFF(replace((select distinct '#!' + ltrim(rtrim(h.CountryCode)) as 'data()'
                from vid_CountriesPerCountryGroup h with (nolock)
                where h.CountryGroupGUID = pc.CountryGroupGUID
                for xml path('')),' #!',','),1,2,''),'') as CountryCodes"
        };

        public static List<string> Joins => new List<string>
        {
            "JOIN tPcProductClassificationDetail pcd (nolock) on tmp.Id = pcd.ProdClassificationDetailGUID",
            "JOIN tPcProductClassification pc (nolock) on pcd.ProdClassificationGUID = pc.ProdClassificationGUID"
        };
    }
}
