using System.Collections.Generic;
using TR.OGT.QueryExtract.Infrastructure.Sql;

namespace TR.OGT.QueryExtract.Data
{
    internal static class BindingRullingsSql
    {
        public static List<string> Columns => new List<string>
        {
            $"pcd.ProdClassificationDetailGUID as {BaseSqlDto.ProdClassificationDetailGUIDColumnName}",
            $"pcd.Number as {BaseSqlDto.HSNumberColumnName}",
            "r.BindingRulingGUID",
            "r.IssueCountry as IssuingCountries",
            "r.BindingRulingReferenceCode as RulingReferenceCode",
            "rt.BindingRulingType as RulingType",
            "r.EffectivityDate as RulingStartDate",
            "r.ExpirationDate as RulingEndDate"
        };

        public static List<string> Joins => new List<string>
        {
            "JOIN tPcProductClassificationDetail pcd (nolock) on tmp.Id = pcd.ProdClassificationDetailGUID",
            "JOIN [BindingRulings].[dbo].[tbrRulings] r (nolock) on pcd.[ProdClassificationGUID] = r.ProdClassificationGuid",
            "JOIN [BindingRulings].[dbo].[tbrPcMap] map (nolock) on r.BindingRulingGUID = map.BindingRulingGUID AND map.Number = pcd.Number",
            "JOIN [BindingRulings].[dbo].[tbrRulingsType] rt (nolock) on r.BindingRulingGuid = rt.BindingRulingGUID"
        };
    }
}