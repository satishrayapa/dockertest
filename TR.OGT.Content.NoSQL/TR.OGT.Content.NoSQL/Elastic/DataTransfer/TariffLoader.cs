using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TR.OGT.Content.NoSQL.Elastic.DataTransfer;

namespace TR.OGT.Content.NoSQL.Elastic.DataTransfer
{
    public class Tariff
    {
        public static void LoadCountry(String dbConnectionString, ElasticConfig ESSettings, String Country)
        {
            ElasticCollection<ContentData.HSDetails> HSCollection = new ElasticCollection<ContentData.HSDetails>();
            HSCollection.Settings = ESSettings;

            Console.WriteLine("-- " + ESSettings.AppName + " Started --");

            TimeSpan ElevenPM = new TimeSpan(23, 0, 0); //11 o'clock

            using (Collectors.TariffCollector Collector = new Collectors.TariffCollector(dbConnectionString))
            {
                DataTable HSUnfiltered = Collector.ListHSForCountry(Country);
                //DataView DistinctNumbers = new DataView(HSUnfiltered);
                //DataTable HSList = DistinctNumbers.ToTable(true, "HS");

                Int32 i = 0;

                foreach (DataRow _row in HSUnfiltered.Rows)
                {
                    if (i % 100 == 0)
                    {
                        ElasticCollection<ContentData.HSDetails> HSCollectionHandoff = HSCollection;
                        HSCollection = new ElasticCollection<ContentData.HSDetails>();
                        HSCollection.Settings = ESSettings;
                        System.Threading.ThreadPool.QueueUserWorkItem(o => HSCollectionHandoff.UploadItems());
                        Console.WriteLine("------------" + System.DateTime.Now.ToString("g") + " ------------ " + i.ToString());

                        //TimeSpan now = DateTime.Now.TimeOfDay;
                        //if (now > ElevenPM)
                        //{
                        //    //match found
                        //    Console.WriteLine("------------" + System.DateTime.Now.ToString("g") + " ------------ After 11pm, pausing for index maintenance");
                        //    System.Threading.Thread.Sleep(10800000);
                        //}

                    }
                    Console.WriteLine(_row["HS"]);

                    ContentData.HSDetails _NewDetail = new ContentData.HSDetails(_row["HS"].ToString(), Country, Collector);

                    _NewDetail.Country = Country.ToLower();
                    _NewDetail.ProdClassificationDetailGUID = _row["ProdClassificationDetailGUID"].ToString();
                    _NewDetail.UOMs = _row["UOMs"].ToString();
                    _NewDetail.Uses = _row["Uses"].ToString();
                    _NewDetail.CountryCodes = _row["CountryCodes"].ToString();
                    _NewDetail.StartDate = DateTime.Parse(_row["StartDate"].ToString());
                    _NewDetail.EndDate = DateTime.Parse(_row["EndDate"].ToString());

                    _NewDetail.LoadRateGroupsFromDataTable();
                    _NewDetail.LoadDescriptionsFromDataTable();

                    HSCollection.Items.Add(_NewDetail);
                    i++;
                }
            }

            //String _Serialized = IP.ES.BulkLoader.Serialize.TopLevelCollection<HSDetails>(HSCollection.Items);
            //byte[] _FileOutput = new UTF8Encoding(true).GetBytes(_Serialized);

            //using (FileStream fs = new FileStream(@"D:\ContentTemp\" + System.Guid.NewGuid().ToString() + ".json", FileMode.OpenOrCreate))
            //{
            //    fs.Write(_FileOutput, 0, _FileOutput.Length);
            //}

            HSCollection.UploadItems();

        }

    }
}
