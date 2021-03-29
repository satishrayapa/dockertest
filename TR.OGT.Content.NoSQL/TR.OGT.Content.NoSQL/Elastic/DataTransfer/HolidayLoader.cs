using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TR.OGT.Content.NoSQL.Elastic.DataTransfer;

namespace TR.OGT.Content.NoSQL.Elastic.DataTransfer
{
    public class Holidays
    {
        public static void Load(String dbConnectionString, ElasticConfig ESSettings)
        {
            ElasticCollection<ContentData.HolidayDetails> HolidayCollection = new ElasticCollection<ContentData.HolidayDetails>();
            HolidayCollection.Settings = ESSettings;

            Console.WriteLine("-- " + ESSettings.AppName + " Started --");

            TimeSpan ElevenPM = new TimeSpan(23, 0, 0); //11 o'clock

            using (Collectors.HolidayCollector Collector = new Collectors.HolidayCollector(dbConnectionString))
            {
                DataTable HolidayCountries = Collector.GetCountries();
                DataTable AllTheHolidays = Collector.GetHolidays();
                //DataView DistinctNumbers = new DataView(HSUnfiltered);
                //DataTable HSList = DistinctNumbers.ToTable(true, "HS");

                Int32 i = 0;

                foreach (DataRow _row in HolidayCountries.Rows)
                {
                    if (i % 1000 == 0)
                    {
                        ElasticCollection<ContentData.HolidayDetails> HolidayCollectionHandoff = HolidayCollection;
                        HolidayCollection = new ElasticCollection<ContentData.HolidayDetails>();
                        HolidayCollection.Settings = ESSettings;
                        System.Threading.ThreadPool.QueueUserWorkItem(o => HolidayCollectionHandoff.UploadItems());
                        Console.WriteLine("------------" + System.DateTime.Now.ToString("g") + " ------------ " + i.ToString());

                        TimeSpan now = DateTime.Now.TimeOfDay;
                        if (now > ElevenPM)
                        {
                            //match found
                            Console.WriteLine("------------" + System.DateTime.Now.ToString("g") + " ------------ After 11pm, pausing for index maintenance");
                            System.Threading.Thread.Sleep(10800000);
                        }

                    }
                    Console.WriteLine(_row["CountryCode"].ToString() + _row["SubCountryCode"].ToString() + _row["HolidayYear"].ToString());

                    ContentData.HolidayDetails _NewDetail = new 
                        ContentData.HolidayDetails(_row["CountryCode"].ToString(), _row["SubCountryCode"].ToString(),
                        Int32.Parse(_row["HolidayYear"].ToString()), AllTheHolidays);

                    _NewDetail.LoadHolidaysFromDataTable();

                    HolidayCollection.Items.Add(_NewDetail);
                    i++;
                }
            }

            HolidayCollection.UploadItems();
        }
    }
}
