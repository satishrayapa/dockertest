using System.Collections.Generic;
using TR.OGT.QueryExtract.Infrastructure.Sql;

namespace TR.OGT.QueryExtract.Data
{
    internal static class ChargeQuotaSql
    {
        public static List<string> Columns => new List<string>
        {
            $"pcd.ProdClassificationDetailGUID as {BaseSqlDto.ProdClassificationDetailGUIDColumnName}",
            "CONVERT(nvarchar(max), cq.QuotaLevel) as [Level]",
            "cq.QuotaType as [Type]",
            "cq.QuotaUOM as UOM",
            "cq.QuotaFillDate as FillDate",
            "cq.EffectivityDate as QuotaStartDate",
            "cq.ExpirationDate as QuotaEndDate",
            @"isnull(STUFF(replace((select distinct '#!' + ltrim(rtrim(h.CountryCode)) as 'data()'
                from vid_CountriesPerCountryGroup h with (nolock)
                where h.CountryGroupGUID = cq.ShipToCountryGroupGUID
                for xml path('')),' #!',','),1,2,''),'') as IssuingCountries",
            @"isnull(STUFF(replace((select distinct '#!' + ltrim(rtrim(h.CountryCode)) as 'data()'
                from vid_CountriesPerCountryGroup h with (nolock)
                where h.CountryGroupGUID = cq.ApplicableCountryGroupGUID
                for xml path('')),' #!',','),1,2,''),'') as ApplicableCountries",
        };

        public static List<string> Joins => new List<string>
        {
            "JOIN tPcProductClassificationDetail pcd (nolock) on tmp.Id = pcd.ProdClassificationDetailGUID",
            "JOIN tChChargeQuota cq with (nolock) on cq.[ProdClassificationGUID] = pcd.[ProdClassificationGUID]",
            "JOIN tChChargeQuotaNumberMap nmap with (nolock) on cq.ChargeQuotaGUID = nmap.ChargeQuotaGUID and nmap.Number = pcd.Number"
        };

        public static string Where => "cq.ChargeDetailTypeGUID = '00000000-0000-0000-0000-000000000000'";
    }
}
