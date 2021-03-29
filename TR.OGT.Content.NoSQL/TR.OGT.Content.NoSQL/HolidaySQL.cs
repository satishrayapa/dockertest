using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace TR.OGT.Content.NoSQL
{
    public class HolidaySQL : IDisposable
    {
        SqlConnection dbConnection;

        public HolidaySQL(String ConnectionString)
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

        internal DataTable RunTheQuery(String QueryTarget)
        {
            DataTable dt = new DataTable();

            using (SqlCommand command = new SqlCommand("", dbConnection))
            {
                switch (QueryTarget)
                {
                    case "GetCountries":
                        command.CommandText = Countries;
                        break;
                    case "GetALLTheThings":
                        command.CommandText = HolidayDetails;
                        break;
                    default:
                        throw new Exception("QueryTarget is invalid, please fix your code.");
                }

                try
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dt);
                }
                catch { throw; }
            }
            return dt;
        }

        static String Countries = @"use Content_InProcess
                                        select distinct
                                              ch.CountryCode,
                                              ch.SubCountryCode,
                                              ch.HolidayYear
                                         from tGcCountryHoliday ch with (nolock)";

        static String HolidayDetails = @"use Content_InProcess
                                        select
                                              ch.CountryCode,
                                              ch.SubCountryCode,
                                              ch.HolidayYear,
                                              ch.HolidayName,
                                              ch.EffectivityDate,
                                              ch.ExpirationDate
                                         from tGcCountryHoliday ch with (nolock)";
    }
}
