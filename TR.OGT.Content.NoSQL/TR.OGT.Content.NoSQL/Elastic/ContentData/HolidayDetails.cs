using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TR.OGT.Content.NoSQL.Elastic.DataTransfer;
using Newtonsoft.Json;

namespace TR.OGT.Content.NoSQL.Elastic.ContentData
{
    public class HolidayDetails : ElasticItem
    {
        public String CountryCode;
        public String SubCountryCode;
        public Int32 HolidayYear;
        public ElasticCollection<Holiday> HolidaysObserved;

        //private Collectors.HolidayCollector _Collector;
        private DataTable _HolidaysForCountryandSubCountry;

        public HolidayDetails(String CountryCode, String SubCountryCode, Int32 HolidayYear, DataTable Holidays)
        {
            //this._Collector = Collector;
            this.CountryCode = CountryCode;
            this.SubCountryCode = SubCountryCode;
            this.HolidayYear = HolidayYear;
            //this.UOMs = new List<String>();
            this.HolidaysObserved = new ElasticCollection<Holiday>();
            this.ESIndex = "content_holidays";

            _HolidaysForCountryandSubCountry = Holidays.Select(
                String.Format("CountryCode = '{0}' AND SubCountryCode = '{1}' AND HolidayYear = {2}", CountryCode, SubCountryCode, HolidayYear.ToString())
                ).CopyToDataTable<DataRow>();
        }

        public override String SerializeForES()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\n{\"index\" : { \"_index\" : " + JsonConvert.ToString(ESIndex) + " }}");
            sb.Append(Environment.NewLine);

            sb.Append("{\"CountryCode\":" + JsonConvert.ToString(CountryCode));
            if (!String.IsNullOrEmpty(SubCountryCode)) { sb.Append(", \"SubCountryCode\":" + JsonConvert.ToString(SubCountryCode)); }
            if (HolidayYear != 0) { sb.Append(", \"HolidayYear\":" + HolidayYear.ToString()); }
            if (HolidaysObserved.Items.Count > 0) { sb.Append(", \"HolidaysObserved\":" + HolidaysObserved.SerializeListItems()); }
            sb.Append(" }");
            return sb.ToString();
        }

        public void LoadHolidaysFromDataTable()
        {
            foreach (DataRow _row in _HolidaysForCountryandSubCountry.Rows)
            {
                Holiday _Holiday = new Holiday
                {
                    Name = _row["HolidayName"].ToString(),
                    StartDate = DateTime.Parse(_row["EffectivityDate"].ToString()),
                    EndDate = DateTime.Parse(_row["ExpirationDate"].ToString())
                };
                HolidaysObserved.Items.Add(_Holiday);
            }
        }
    }
}
