using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace TR.OGT.Content.NoSQL
{
    public partial class Collectors
    {

        public class HolidayCollector : IDisposable
        {
            HolidaySQL Loader;
            public HolidayCollector(String ConnectionString)
            {
                Loader = new HolidaySQL(ConnectionString);
            }

            public void Dispose()
            {
                Loader.Dispose();
            }

            public DataTable GetCountries()
            {
                DataTable dt = Loader.RunTheQuery("GetCountries");
                return dt;
                throw new NotImplementedException();
            }

            public DataTable GetHolidays()
            {
                DataTable dt = Loader.RunTheQuery("GetALLTheThings");
                return dt;
                throw new NotImplementedException();
            }

        }

        public class TariffCollector : IDisposable
        {
            TariffSQL Loader;
            public TariffCollector(String ConnectionString)
            {
                Loader = new TariffSQL(ConnectionString);
            }

            public void Dispose()
            {
                Loader.Dispose();
            }

            public void DoTheThings(String CountryCode, String HSNumber) //This is temporary -- it's just for testing.
            {
                DataTable dt1 = Loader.RunTheQuery(CountryCode, HSNumber, "CountryGroups");
                DataTable dt2 = Loader.RunTheQuery(CountryCode, HSNumber, "HSForACountry");
                DataTable dt3 = Loader.RunTheQuery(CountryCode, HSNumber, "RatesForClassification");
                DataTable dt4 = Loader.RunTheQuery(CountryCode, HSNumber, "NotesForRateGroup");
                DataTable dt5 = Loader.RunTheQuery(CountryCode, HSNumber, "DescriptionsForHS");

                Console.WriteLine("Query1: " + dt1.Rows.Count.ToString());
                Console.WriteLine("Query2: " + dt2.Rows.Count.ToString());
                Console.WriteLine("Query3: " + dt3.Rows.Count.ToString());
                Console.WriteLine("Query4: " + dt4.Rows.Count.ToString());
                Console.WriteLine("Query5: " + dt5.Rows.Count.ToString());

                Console.ReadLine();
            }

            public DataTable GetMainCountryGroup(String CountryCode)
            {
                DataTable dt = Loader.RunTheQuery(CountryCode, "", "CountryGroups");
                return dt;
            }

            public DataTable ListHSForCountry(String CountryCode)
            {
                DataTable dt = Loader.RunTheQuery(CountryCode, "", "HSForACountry");
                return dt;
            }

            public DataTable GetRatesForClassification(String CountryCode, String HSNumber)
            {
                DataTable dt = Loader.RunTheQuery(CountryCode, HSNumber, "RatesForClassification");
                return dt;
            }

            public DataTable GetNotesForRateGroup(String CountryCode, String HSNumber)
            {
                DataTable dt = Loader.RunTheQuery(CountryCode, HSNumber, "NotesForRateGroup");
                return dt;
            }

            public DataTable GetDescriptionsForHS(String CountryCode, String HSNumber)
            {
                DataTable dt = Loader.RunTheQuery(CountryCode, HSNumber, "DescriptionsForHS");
                return dt;
            }

            //public HSDetails GetHSDetail(DataTable RowsForHSNum, String CountryCode)
            //{
            //    HSDetails _Item = new HSDetails(RowsForHSNum.Rows[0]["Number"].ToString(), CountryCode);



            //    return _Item;
            //}
        }
    }
}
