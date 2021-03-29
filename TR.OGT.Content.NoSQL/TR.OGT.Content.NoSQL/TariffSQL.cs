using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace TR.OGT.Content.NoSQL
{
    public class TariffSQL : IDisposable
    {
        SqlConnection dbConnection;

        public TariffSQL(String ConnectionString)
        {
            dbConnection = new SqlConnection(ConnectionString);
            try
            {
                dbConnection.Open();
            }
            catch { throw; }
        }

        public void Dispose()
        {
            dbConnection.Close();
            dbConnection.Dispose();
        }

        internal DataTable RunTheQuery(String CountryCode, String HSNumber, String QueryTarget)
        {
            DataTable dt = new DataTable();

            using (SqlCommand command = new SqlCommand("", dbConnection))
            {
                switch (QueryTarget)
                {
                    case "CountryGroups":
                        command.CommandText = Query1;
                        break;
                    case "HSForACountry":
                        command.CommandText = SQLMainQuery + Query2;
                        break;
                    case "RatesForClassification":
                        command.CommandText = SQLMainQuery + Query3;
                        break;
                    case "NotesForRateGroup":
                        command.CommandText = SQLMainQuery + Query4;
                        break;
                    case "DescriptionsForHS":
                        command.CommandText = SQLMainQuery + Query5;
                        break;
                    default:
                        throw new Exception("QueryTarget is invalid, please fix your code.");
                }

                command.Parameters.Add("@CountryCode", SqlDbType.VarChar);
                command.Parameters["@CountryCode"].Value = CountryCode;
                command.Parameters.Add("@HSNumber", SqlDbType.VarChar);
                command.Parameters["@HSNumber"].Value = HSNumber;
                try
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dt);
                }
                catch { throw; }
            }
            return dt;
        }

        static String Query1 = @"
                --Find all the country / group relationships
                with AllCountryGroups as (                            
                --CountryGroups with Allowable = Y include only the listed countries
                select distinct
                      gc.CountryGroupGuid,
                      gc.CountryGroupCode,
                      cg.CountryCode
                from tGcGroupCode gc with (nolock)
                inner join tGcCountryGroup cg with (nolock)
                      on gc.CountryGroupGuid = cg.CountryGroupGuid
                inner join tGcCountry c with (nolock)
                      on cg.CountryCode = c.CountryCode
                where gc.Allowable = 'Y'
                union all
                --CountryGroups with Allowable = N include all countries except those listed
                select distinct
                      gc.CountryGroupGuid,
                      gc.CountryGroupCode,
                      c.CountryCode
                from tGcGroupCode gc with (nolock)
                inner join tGcCountry c with (nolock)
                      on 1 = 1
                where gc.Allowable = 'N'
                and not exists (select top 1 1
                                        from tGcCountryGroup cg with (nolock)
                                        where gc.CountryGroupGuid = cg.CountryGroupGuid
                                        and c.CountryCode = cg.CountryCode)
                union all         
                --CountryGroups with Allowable = Y and no associated countries apply to ALL countries
                select distinct
                      gc.CountryGroupGuid,
                      gc.CountryGroupCode,
                      c.CountryCode
                from tGcGroupCode gc with (nolock)
                inner join tGcCountry c with (nolock)  --master list of all countries
                      on 1 = 1
                where gc.Allowable = 'Y'
                and not exists (select top 1 1
                                        from tGcCountryGroup cg with (nolock)
                                        where gc.CountryGroupGuid = cg.CountryGroupGuid)),

                --Narrow down list to only the groups including specified country
                FilteredCountryGroups as (
                select *
                from AllCountryGroups cg with (nolock)
                where cg.CountryCode = @CountryCode),

                --Narrow down list to only the groups related to tariff schedule, and count how many countries are part of each
                FilteredGroupsForTariffs as (
                select
                      acg.CountryGroupCode,
                      acg.CountryGroupGuid,
                      count(*) as TotalCountryCodes
                from AllCountryGroups acg with (nolock)
                inner join FilteredCountryGroups fcg with (nolock)
                      on acg.CountryGroupGuid = fcg.CountryGroupGuid
                where exists (select top 1 1
                                  from tPcProductClassification pc with (nolock)
                                  where acg.CountryGroupGuid = pc.CountryGroupGuid
                                  and pc.ClientViewable = 'Y'
                                  and acg.CountryGroupCode not in ('CAGR','CBGR','WCO'))
                group by
                      acg.CountryGroupCode,
                      acg.CountryGroupGuid)
      
                --Return only the largest country group for a tariff schedule of the specified country
                select top 1 *
                from FilteredGroupsForTariffs fgft with (nolock)
                order by fgft.TotalCountryCodes desc";

        static String Query2 = @"select distinct   
                                	hsd.ProdClassificationDetailGUID as ProdClassificationDetailGUID,   
                                	hsd.Number as HS,   
                                	'' as HSBreakout,   
                                	'[""' + isnull(STUFF(replace((select distinct '#!' + ltrim(rtrim(replace(h.UOM,',',''))) as 'data()'   
                                							from HSDetails h with (nolock)   
                                							where h.ProdClassificationDetailGUID = hsd.ProdClassificationDetailGUID   
                                							for xml path('')),' #!','"",""'),1,2,'') + '""]','') as UOMs,   
                                	'[""' + isnull(STUFF(replace((select distinct '#!' + ltrim(rtrim(h.ProdClassificationUse)) as 'data()'   
                                							from HSDetails h with (nolock)   
                                							where h.ProdClassificationDetailGUID = hsd.ProdClassificationDetailGUID   
                                							for xml path('')),' #!','"",""'),1,2,'') + '""]','') as Uses,   
                                	'[""' + @CountryCode + '""' + case when hsd.CountryGroupCode = @CountryCode then '' else + ',""' + hsd.CountryGroupCode + '""' end + ']' as CountryCodes,   
                                	hsd.EffectivityDate as StartDate,   
                                	hsd.ExpirationDate as EndDate   
                                from HSDetails hsd with (nolock)";


        static String Query3 = @"select distinct   
                                	q.*,   
                                	'[""' + isnull(STUFF(replace((select distinct '#!' + ltrim(rtrim(c.ExclusionCountries)) as 'data()'   
                                								from Charges c with (nolock)   
                                								where q.ChargeDetailGuid = c.ChargeDetailGuid   
                                								for xml path('')),' #!','"",""'),1,2,'') + '""]','') as ExclusionCountries   
                                from   
                                (select distinct   
                                	c.ChargeDetailGuid,		--Not part of the visual query layout, but this GUID is the link between charges and notes   
                                	c.CountryGroupCode as GroupCode,   
                                	c.UIRate,   
                                	c.CurrencyCode,   
                                	--Calculation:   
                                		c.Formula,   
                                		c.RateA,   
                                		c.RateAMath,   
                                		c.RateAUOM,   
                                		c.RateAUOMAmount,   
                                		c.RateB,   
                                		c.RateBMath,   
                                		c.RateBUOM,   
                                		c.RateBUOMAmount,   
                                		c.RateC,   
                                		c.RateCMath,   
                                		c.RateCUOM,   
                                		c.RateCUOMAmount,   
                                		isnull(mmin.Formula,'') as MinFormula,   
                                		isnull(mmin.RateA,0) as MinRateA,   
                                		isnull(mmin.RateAMath,'') as MinRateAMath,   
                                		isnull(mmin.RateAUOM,'') as MinRateAUOM,   
                                		isnull(mmin.RateAUOMAmount,0) as MinRateAUOMAmount,   
                                		isnull(mmin.RateB,0) as MinRateB,   
                                		isnull(mmin.RateBMath,'') as MinRateBMath,   
                                		isnull(mmin.RateBUOM,'') as MinRateBUOM,   
                                		isnull(mmin.RateBUOMAmount,0) as MinRateBUOMAmount,   
                                		isnull(mmin.RateC,0) as MinRateC,   
                                		isnull(mmin.RateCMath,'') as MinRateCMath,   
                                		isnull(mmin.RateCUOM,'') as MinRateCUOM,   
                                		isnull(mmin.RateCUOMAmount,0) as MinRateCUOMAmount,   
                                		isnull(mmax.Formula,'') as MaxFormula,   
                                		isnull(mmax.RateA,0) as MaxRateA,   
                                		isnull(mmax.RateAMath,'') as MaxRateAMath,   
                                		isnull(mmax.RateAUOM,'') as MaxRateAUOM,   
                                		isnull(mmax.RateAUOMAmount,0) as MaxRateAUOMAmount,   
                                		isnull(mmax.RateB,0) as MaxRateB,   
                                		isnull(mmax.RateBMath,'') as MaxRateBMath,   
                                		isnull(mmax.RateBUOM,'') as MaxRateBUOM,   
                                		isnull(mmax.RateBUOMAmount,0) as MaxRateBUOMAmount,   
                                		isnull(mmax.RateC,0) as MaxRateC,   
                                		isnull(mmax.RateCMath,'') as MaxRateCMath,   
                                		isnull(mmax.RateCUOM,'') as MaxRateCUOM,   
                                		isnull(mmax.RateCUOMAmount,0) as MaxRateCUOMAmount,   
                                	REPLACE(SUBSTRING(dbo.fnGetMaxMin(c.ChargeDetailGuid),CHARINDEX('MIN',dbo.fnGetMaxMin(c.ChargeDetailGuid),0),CHARINDEX(';',dbo.fnGetMaxMin(c.ChargeDetailGuid),0)),';','') as MinUICharge,   
                                	SUBSTRING(dbo.fnGetMaxMin(c.ChargeDetailGuid),CHARINDEX('MAX',dbo.fnGetMaxMin(c.ChargeDetailGuid),0),LEN(dbo.fnGetMaxMin(c.ChargeDetailGuid))) as MaxUICharge,   
                                	c.EffectivityDate as ChargeStartDate,   
                                	c.ExpirationDate as ChargeEndDate,   
                                	c.ChargeUse,   
                                	c.ChargeType,   
                                	c.ChargeDetailTypeCode as ChargeDetailCode,   
                                	c.ChargeDescription   
                                from Charges c with (nolock)   
                                left join MaxMin mmin with (nolock)   
                                	on c.ChargeDetailGuid = mmin.ChargeDetailGuid   
                                	and mmin.MaxOrMinType = 'MIN'   
                                left join MaxMin mmax with (nolock)   
                                	on c.ChargeDetailGuid = mmax.ChargeDetailGuid 
                                   and mmax.MaxOrMinType = 'MAX') q";


        static String Query4 = @"select distinct
	                                n.ChargeDetailGuid,		
	                                n.CultureCode,
	                                n.NoteText
	                                from Notes n with (nolock)";


        static String Query5 = @"select distinct
	                                d.CultureCode,
	                                d.Description
                                from Descriptions d with (nolock)";

        static String SQLMainQuery = @"
            use Content_InProcess;

            --Examples:	US 4407950000
            --			RE 7108120000 (excluded countries)
            --			IT 0203295530
            --			DE 2204309600
            --			IN 61103010
            --			RS 2402209000 (min/max rates)

            --declare @CountryCode as varchar(2) = 'RE';
            --declare @HSNumber as varchar(20) = '7108120000';

            --Sorting out the Country Groups up front is easier than trying to do it in every query
            --We might want to make this a cross-reference table, but I'll use a CTE
            with CountryGroups as (					
            --CountryGroups with Allowable = Y include only the listed countries
            select distinct
	            gc.CountryGroupGuid,
	            gc.CountryGroupCode,
	            cg.CountryCode
            from tGcGroupCode gc with (nolock)
            inner join tGcCountryGroup cg with (nolock)
	            on gc.CountryGroupGuid = cg.CountryGroupGuid
            inner join tGcCountry c with (nolock)
	            on cg.CountryCode = c.CountryCode
            where gc.Allowable = 'Y'
            union all
            --CountryGroups with Allowable = N include all countries except those listed
            select distinct
	            gc.CountryGroupGuid,
	            gc.CountryGroupCode,
	            c.CountryCode
            from tGcGroupCode gc with (nolock)
            inner join tGcCountry c with (nolock)
	            on 1 = 1
            where gc.Allowable = 'N'
            and not exists (select top 1 1
				            from tGcCountryGroup cg with (nolock)
				            where gc.CountryGroupGuid = cg.CountryGroupGuid
				            and c.CountryCode = cg.CountryCode)
            union all		
            --CountryGroups with Allowable = Y and no associated countries apply to ALL countries
            select distinct
	            gc.CountryGroupGuid,
	            gc.CountryGroupCode,
	            c.CountryCode
            from tGcGroupCode gc with (nolock)
            inner join tGcCountry c with (nolock)  --master list of all countries
	            on 1 = 1
            where gc.Allowable = 'Y'
            and not exists (select top 1 1
				            from tGcCountryGroup cg with (nolock)
				            where gc.CountryGroupGuid = cg.CountryGroupGuid)),

            --1)  All tariffs for a given country

            TariffSchedules as (
            select
	            cg.CountryGroupCode,
	            cg.CountryCode,
	            pc.ProdClassificationGUID,
	            pc.ProdClassificationName,
	            pcu.ProdClassificationUse  --some HS valid for only import or export within the same country, so this is important
            from tPcProductClassification pc with (nolock)  --master list of tariff schedules
            inner join vid_AllCountryCode cg with (nolock)
	            on pc.CountryGroupGuid = cg.CountryGroupGuid
            inner join tPcProductClassificationUse pcu with (nolock)
	            on pc.ProdClassificationGUID = pcu.ProdClassificationGUID
	            --find whether the tariff schedule is for import, export, or both
            where pc.ClientViewable = 'Y'
            and pc.ProdClassificationGUID <> '7D55BEFF-D372-42DC-AE3B-51A990653708'  --excluding the 'Global HS' tariff schedule
            and pcu.Priority <> 0  --Priority 0 means the tariff schedule is for reference only
            and cg.CountryCode = @CountryCode),

            --2)  All HS details for a given country

            HSDetails as (
            select distinct
	            pcd.ProdClassificationGUID,
	            pcd.ProdClassificationDetailGUID,
	            pcd.Number,
	            ts.ProdClassificationUse,  --indicates whether the HS is valid for import or export (can be valid for both)
	            isnull(uom.RptQtyUom,'') as UOM,
	            ts.CountryCode,
	            ts.CountryGroupCode,
	            pcd.EffectivityDate,
	            pcd.ExpirationDate
            from TariffSchedules ts with (nolock)
            inner join tPcProductClassificationDetail pcd with (nolock)
	            on ts.ProdClassificationGUID = pcd.ProdClassificationGUID
	            --find HS existing on each tariff schedule from first query
            left join tPcReportUnitofMeasure uom with (nolock)
	            on pcd.ProdClassificationDetailGUID = uom.ProdClassificationDetailGuid
	            --find UOMs for each HS (may want to add IncludeUOM = AND then relying on UOM alias cross-reference)
            where pcd.CustomsDeclarable = 'Y'
            and (pcd.ExpirationDate > pcd.EffectivityDate or pcd.ExpirationDate = '')),
            --when HS are updated in the system, the old version has ExpirationDate = EffectivityDate - 1
            --this clause excludes those records to show only real changes in eff/exp dates

            --3) All rates / charges for the given tariff schedule / HS

            Charges as (
            select distinct
	            hsd.ProdClassificationDetailGUID,
	            cd.ChargeDetailGuid,
	            gc.CountryGroupCode,
	            isnull(dbo.fnGetRates(cd.ChargeDetailGuid),'SEE NOTE') as UIRate,
	            cd.CurrencyCode,  --probably need to add currency code, since some rates are perunit
	            cf.Formula,  --formula would be helpful for seeing how the calculation is performed
	            isnull(drA.Rate,0) as RateA,
	            isnull(drA.RateMath,'') as RateAMath,
	            isnull(drA.SpecificRptQtyUom,'') as RateAUOM,
	            isnull(drA.UomAmount,0) as RateAUOMAmount,
	            isnull(drB.Rate,0) as RateB,
	            isnull(drB.RateMath,'') as RateBMath,
	            isnull(drB.SpecificRptQtyUom,'') as RateBUOM,
	            isnull(drB.UomAmount,0) as RateBUOMAmount,
	            isnull(drC.Rate,0) as RateC,
	            isnull(drC.RateMath,'') as RateCMath,
	            isnull(drC.SpecificRptQtyUom,'') as RateCUOM,
	            isnull(drC.UomAmount,0) as RateCUOMAmount,
	            cd.EffectivityDate,
	            cd.ExpirationDate,
	            cdt.ChargeUse,  --should probably add this to differentiate import/export charges
	            cdt.ChargeType,
	            cdt.ChargeDetailTypeCode,
	            cdt.Description as ChargeDescription,
	            isnull(cde.ExcludedGroup,'') as ExclusionCountries
            from HSDetails hsd with (nolock)
            inner join tChPcMap map with (nolock)
	            on hsd.ProdClassificationDetailGUID = map.ProdClassificationDetailGuid
	            --maps HS number to charges
            inner join tChChargeDetail cd with (nolock)
	            on map.ChargeDetailGuid = cd.ChargeDetailGuid
            inner join tChChargeDetailType cdt with (nolock)
	            on cd.ChargeDetailTypeGuid = cdt.ChargeDetailTypeGuid
	            --pulls in the charge type, use, description, etc.
            inner join CountryGroups gc with (nolock)
	            on cd.ShipFromCountryGroupGuid = gc.CountryGroupGuid
	            --finds the countries in the 'Ship To' country group (reversed for EXPORT charges)
            left join tChChargeFormula cf with (nolock)
	            on cd.FormulaGUID = cf.FormulaGuid
	            --finds the formula to calculate the rate
            left join tChDetailRate drA with (nolock)
	            on cd.ChargeDetailGuid = drA.ParentGuid
	            and drA.RateType = 'A'
	            --first piece of the rate calculation
            left join tChDetailRate drB with (nolock)
	            on cd.ChargeDetailGuid = drB.ParentGuid
	            and drB.RateType = 'B'
	            --second pieces of the rate calculation, if applicable
            left join tChDetailRate drC with (nolock)
	            on cd.ChargeDetailGuid = drC.ParentGuid
	            and drC.RateType = 'C'
	            --third piece of the rate calculation, if applicable
            left join tChChargeDetailExclusion cde with (nolock)
	            on cd.ChargeDetailGuid = cde.ChargeDetailGuid
            where hsd.Number = @HSNumber
            and gc.CountryCode = @CountryCode
            --'Ship To' country is a misnomer, as it is actually the country from which the charge originates
            --For import charges, this actually is the 'Ship To' country
            --For export charges, this is reversed
            --Therefore, if we match the 'Ship To' country to the @CountryCode variable, we'll see all the applicable import/export charges
            --Country groups can overlap, so this excludes records with the same shipfrom/shipto
            and not exists (select top 1 1
				            from tChChargeDetailExclusion cde with (nolock)
				            where cd.ChargeDetailGuid = cde.ChargeDetailGuid
				            and gc.CountryCode = cde.ExcludedGroup)),
				            --instead of storing a separate field for 'Exclusion', we can exclude these countries dynamically

            --4) Min/Max rates for each charge (where applicable)

            MaxMin as (
            select distinct
	            c.ChargeDetailGuid,
	            cf.Formula,
	            mm.MaxOrMinType,
	            isnull(drA.Rate,0) as RateA,
	            isnull(drA.RateMath,'') as RateAMath,
	            isnull(drA.SpecificRptQtyUom,'') as RateAUOM,
	            isnull(drA.UomAmount,0) as RateAUOMAmount,
	            isnull(drB.Rate,0) as RateB,
	            isnull(drB.RateMath,'') as RateBMath,
	            isnull(drB.SpecificRptQtyUom,'') as RateBUOM,
	            isnull(drB.UomAmount,0) as RateBUOMAmount,
	            isnull(drC.Rate,0) as RateC,
	            isnull(drC.RateMath,'') as RateCMath,
	            isnull(drC.SpecificRptQtyUom,'') as RateCUOM,
	            isnull(drC.UomAmount,0) as RateCUOMAmount
            from Charges c with (nolock)
            inner join tChDetailMaxMin mm with (nolock)
	            on c.ChargeDetailGuid = mm.ChargeDetailGuid
            inner join tChChargeFormula cf with (nolock)
	            on mm.FormulaGuid = cf.FormulaGuid
            left join tChDetailRate drA with (nolock)
	            on mm.DetailMaxMinGuid = drA.ParentGuid
	            and drA.RateType = 'A'
	            --first piece of the rate calculation
            left join tChDetailRate drB with (nolock)
	            on mm.DetailMaxMinGuid = drB.ParentGuid
	            and drB.RateType = 'B'
	            --second pieces of the rate calculation, if applicable
            left join tChDetailRate drC with (nolock)
	            on mm.DetailMaxMinGuid = drC.ParentGuid
	            and drC.RateType = 'C'
	            --third piece of the rate calculation, if applicable
            where 1 = 1),

            --5) Notes/languages for each charge

            Notes as (
            select distinct
	            c.ChargeDetailGuid,
	            isnull(n.CultureCode,'') as CultureCode,
	            isnull(n.NoteText,'') as NoteText
            from Charges c with (nolock)
            inner join tGcNotes n with (nolock)
	            on n.NoteSource = 'tChChargeDetail'
	            and c.ChargeDetailGuid = n.ParentContentNoteGuid),
	            --not all charges have notes
	            --notes table houses multiple note types, so specify NoteSource

            --6) Descriptions/Languages for tariff schedule and HS

            Descriptions as (
            select distinct
	            hsd.ProdClassificationDetailGUID,
	            hsd.Number,
	            pcdesc.CultureCode,
	            pcdesc.Description
            from HSDetails hsd with (nolock)
            inner join tPcProductClassificationDescription pcdesc with (nolock)
	            on hsd.ProdClassificationDetailGUID = pcdesc.ProdClassificationDetailGUID
            where hsd.Number = @HSNumber
            and pcdesc.DescTypeCode = 'LONGTXT')
            ";
    }
}
