using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TR.OGT.Content.NoSQL.Elastic.DataTransfer;
using Newtonsoft.Json;
using System.Linq;

namespace TR.OGT.Content.NoSQL.Elastic.ContentData
{
    public class HSDetails : ElasticItem
    {
        public HSDataDTOs.HSDetailsDTO _HSDetails;


        public ElasticCollection<ChargesGroup> RateGroups;
        public ElasticCollection<Description> Descriptions;

        private Collectors.TariffCollector _Collector;
        private DataTable _RatesForClassification;
        private DataTable _NotesForRateGroups;
        private DataTable _DescriptionsForHS;

        public HSDetails(String HSNumber, String IndexCountry, Collectors.TariffCollector Collector)
        {
            this._Collector = Collector;
            this._HSDetails.HSNumber = HSNumber;
            this._HSDetails.HSBreakdown = new HSBreakout(HSNumber);
            this.IndexCountry = IndexCountry;
            //this.UOMs = new List<String>();
            this.RateGroups = new ElasticCollection<ChargesGroup>();
            this.Descriptions = new ElasticCollection<Description>();
            this.ESIndex = "content_tariff_" + _Collector.GetMainCountryGroup(IndexCountry).Rows[0]["CountryGroupCode"].ToString().ToLower();

            _RatesForClassification = _Collector.GetRatesForClassification(IndexCountry, HSNumber);
            _NotesForRateGroups = _Collector.GetNotesForRateGroup(IndexCountry, HSNumber);
            _DescriptionsForHS = _Collector.GetDescriptionsForHS(IndexCountry, HSNumber);
        }

        public override String SerializeForES()
        {
            HSBreakdown = new HSBreakout(HSNumber);

            StringBuilder sb = new StringBuilder();
            sb.Append("\n{\"index\" : { \"_index\" : " + JsonConvert.ToString(ESIndex) + " }}");
            sb.Append(Environment.NewLine);

            sb.Append("{\"HSNumber\":" + JsonConvert.ToString(HSNumber));
            sb.Append(", \"HSBreakout\":" + HSBreakdown.SerializeForES());
            if (!String.IsNullOrEmpty(Country)) { sb.Append(", \"Country\":" + JsonConvert.ToString(Country.ToUpper())); }
            if (!String.IsNullOrEmpty(ProdClassificationDetailGUID)) { sb.Append(", \"ProdClassificationDetailGUID\":" + JsonConvert.ToString(ProdClassificationDetailGUID)); }
            if (!String.IsNullOrEmpty(UOMs)) { sb.Append(", \"UOMs\":" + UOMs); }
            if (!String.IsNullOrEmpty(Uses)) { sb.Append(", \"Uses\":" + Uses); }
            if (!String.IsNullOrEmpty(CountryCodes)) { sb.Append(", \"CountryCodes\":" + CountryCodes); }
            if (!StartDate.Equals(DateTime.Parse("1/1/1900 12:00:00 AM"))) { sb.Append(", \"StartDate\":" + JsonConvert.ToString(StartDate.ToUniversalTime().ToString("s"))); }
            if (!EndDate.Equals(DateTime.Parse("1/1/1900 12:00:00 AM"))) { sb.Append(", \"EndDate\":" + JsonConvert.ToString(EndDate.ToUniversalTime().ToString("s"))); }
            if (RateGroups.Items.Count > 0) { sb.Append(", \"RateGroups\":" + RateGroups.SerializeListItems()); }
            if (Descriptions.Items.Count > 0) { sb.Append(", \"Descriptions\":" + Descriptions.SerializeListItems()); }
            sb.Append(" }");
            return sb.ToString();
        }

        public void LoadRateGroupsFromDataTable()
        {
            foreach (DataRow _row in _RatesForClassification.Rows)
            {
                ChargesGroup _Charge = new ChargesGroup()
                {
                    ChargeDetailGUID = _row["ChargeDetailGUID"].ToString(),
                    GroupCode = _row["GroupCode"].ToString(),
                    UIRate = _row["UIRate"].ToString(),
                    MinUICharge = _row["MinUICharge"].ToString(),
                    MaxUICharge = _row["MaxUICharge"].ToString(),
                    ChargeStartDate = DateTime.Parse(_row["ChargeStartDate"].ToString()),
                    ChargeEndDate = DateTime.Parse(_row["ChargeEndDate"].ToString()),
                    ChargeUse = _row["ChargeUse"].ToString(),
                    ChargeType = _row["ChargeType"].ToString(),
                    ChargeDetailCode = _row["ChargeDetailCode"].ToString(),
                    ChargeDescription = _row["ChargeDescription"].ToString(),
                    ExclusionCountries = _row["ExclusionCountries"].ToString(),
                    Calculation = new CalculationBreakout
                    {
                        Formula = _row["Formula"].ToString(),
                        RateA = Single.Parse(_row["RateA"].ToString()),
                        RateAMath = _row["RateAMath"].ToString(),
                        RateAUOM = _row["RateAUOM"].ToString(),
                        RateAUOMAmount = Single.Parse(_row["RateAUOMAmount"].ToString()),
                        RateB = Single.Parse(_row["RateB"].ToString()),
                        RateBMath = _row["RateBMath"].ToString(),
                        RateBUOM = _row["RateBUOM"].ToString(),
                        RateBUOMAmount = Single.Parse(_row["RateBUOMAmount"].ToString()),
                        RateC = Single.Parse(_row["RateC"].ToString()),
                        RateCMath = _row["RateCMath"].ToString(),
                        RateCUOM = _row["RateCUOM"].ToString(),
                        RateCUOMAmount = Single.Parse(_row["RateCUOMAmount"].ToString()),
                        MinRateA = Single.Parse(_row["MinRateA"].ToString()),
                        MinRateAMath = _row["MinRateAMath"].ToString(),
                        MinRateAUOM = _row["MinRateAUOM"].ToString(),
                        MinRateAUOMAmount = Single.Parse(_row["MinRateAUOMAmount"].ToString()),
                        MinRateB = Single.Parse(_row["MinRateB"].ToString()),
                        MinRateBMath = _row["MinRateBMath"].ToString(),
                        MinRateBUOM = _row["MinRateBUOM"].ToString(),
                        MinRateBUOMAmount = Single.Parse(_row["MinRateBUOMAmount"].ToString()),
                        MinRateC = Single.Parse(_row["MinRateC"].ToString()),
                        MinRateCMath = _row["MinRateCMath"].ToString(),
                        MinRateCUOM = _row["MinRateCUOM"].ToString(),
                        MinRateCUOMAmount = Single.Parse(_row["MinRateCUOMAmount"].ToString()),
                        MaxRateA = Single.Parse(_row["MaxRateA"].ToString()),
                        MaxRateAMath = _row["MaxRateAMath"].ToString(),
                        MaxRateAUOM = _row["MaxRateAUOM"].ToString(),
                        MaxRateAUOMAmount = Single.Parse(_row["MaxRateAUOMAmount"].ToString()),
                        MaxRateB = Single.Parse(_row["MaxRateB"].ToString()),
                        MaxRateBMath = _row["MaxRateBMath"].ToString(),
                        MaxRateBUOM = _row["MaxRateBUOM"].ToString(),
                        MaxRateBUOMAmount = Single.Parse(_row["MaxRateBUOMAmount"].ToString()),
                        MaxRateC = Single.Parse(_row["MaxRateC"].ToString()),
                        MaxRateCMath = _row["MaxRateCMath"].ToString(),
                        MaxRateCUOM = _row["MaxRateCUOM"].ToString(),
                        MaxRateCUOMAmount = Single.Parse(_row["MaxRateCUOMAmount"].ToString())
                    }
                };


                _Charge.LoadNotesFromDataTable(_NotesForRateGroups);

                RateGroups.Items.Add(_Charge);
            }
        }

        public void LoadDescriptionsFromDataTable()
        {
            foreach (DataRow _row in _DescriptionsForHS.Rows)
            {
                Description _Description = new Description(_row["CultureCode"].ToString(), _row["Description"].ToString());
                Descriptions.Items.Add(_Description);
            }
        }
    }

    public class HSBreakout : ElasticItem
    {
        public String Chapter;
        public String Heading;
        public String Subheading;
        public String Tariff;

        public HSBreakout(String HSNumber)
        {
            this.Chapter = HSNumber.Substring(0, 2);
            this.Heading = HSNumber.Substring(2, 2);
            this.Subheading = HSNumber.Substring(4, 2);
            this.Tariff = HSNumber.Substring(6, 2);
        }

        public override String SerializeForES()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{\"Chapter\":" + JsonConvert.ToString(Chapter));
            sb.Append(", \"Heading\":" + JsonConvert.ToString(Heading));
            sb.Append(", \"Subheading\":" + JsonConvert.ToString(Subheading));
            sb.Append(", \"Tariff\":" + JsonConvert.ToString(Tariff));
            sb.Append(" }");
            return sb.ToString();
        }
    }

    public class CalculationBreakout : ElasticItem
    {
        public String Formula;
        public Single RateA;
        public String RateAMath;
        public String RateAUOM;
        public Single RateAUOMAmount;
        public Single RateB;
        public String RateBMath;
        public String RateBUOM;
        public Single RateBUOMAmount;
        public Single RateC;
        public String RateCMath;
        public String RateCUOM;
        public Single RateCUOMAmount;
        public Single MinRateA;
        public String MinRateAMath;
        public String MinRateAUOM;
        public Single MinRateAUOMAmount;
        public Single MinRateB;
        public String MinRateBMath;
        public String MinRateBUOM;
        public Single MinRateBUOMAmount;
        public Single MinRateC;
        public String MinRateCMath;
        public String MinRateCUOM;
        public Single MinRateCUOMAmount;
        public Single MaxRateA;
        public String MaxRateAMath;
        public String MaxRateAUOM;
        public Single MaxRateAUOMAmount;
        public Single MaxRateB;
        public String MaxRateBMath;
        public String MaxRateBUOM;
        public Single MaxRateBUOMAmount;
        public Single MaxRateC;
        public String MaxRateCMath;
        public String MaxRateCUOM;
        public Single MaxRateCUOMAmount;

        public override String SerializeForES()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{\"Formula\":" + JsonConvert.ToString(Formula));
            if (!(RateA == 0)) { sb.Append(", \"RateA\":" + RateA); }
            if (!String.IsNullOrEmpty(RateAMath)) { sb.Append(", \"RateAMath\":" + JsonConvert.ToString(RateAMath)); }
            if (!String.IsNullOrEmpty(RateAUOM)) { sb.Append(", \"RateAUOM\":" + JsonConvert.ToString(RateAUOM)); }
            if (!(RateAUOMAmount == 0)) { sb.Append(", \"RateAUOMAmount\":" + RateAUOMAmount); }
            if (!(RateB == 0)) { sb.Append(", \"RateB\":" + RateB); }
            if (!String.IsNullOrEmpty(RateBMath)) { sb.Append(", \"RateBMath\":" + JsonConvert.ToString(RateBMath)); }
            if (!String.IsNullOrEmpty(RateBUOM)) { sb.Append(", \"RateBUOM\":" + JsonConvert.ToString(RateBUOM)); }
            if (!(RateBUOMAmount == 0)) { sb.Append(", \"RateBUOMAmount\":" + RateBUOMAmount); }
            if (!(RateC == 0)) { sb.Append(", \"RateC\":" + RateC); }
            if (!String.IsNullOrEmpty(RateCMath)) { sb.Append(", \"RateCMath\":" + JsonConvert.ToString(RateCMath)); }
            if (!String.IsNullOrEmpty(RateCUOM)) { sb.Append(", \"RateCUOM\":" + JsonConvert.ToString(RateCUOM)); }
            if (!(RateCUOMAmount == 0)) { sb.Append(", \"RateCUOMAmount\":" + RateCUOMAmount); }
            if (!(MinRateA == 0)) { sb.Append(", \"MinRateA\":" + MinRateA); }
            if (!String.IsNullOrEmpty(MinRateAMath)) { sb.Append(", \"MinRateAMath\":" + JsonConvert.ToString(MinRateAMath)); }
            if (!String.IsNullOrEmpty(MinRateAUOM)) { sb.Append(", \"MinRateAUOM\":" + JsonConvert.ToString(MinRateAUOM)); }
            if (!(MinRateAUOMAmount == 0)) { sb.Append(", \"MinRateAUOMAmount\":" + MinRateAUOMAmount); }
            if (!(MinRateB == 0)) { sb.Append(", \"MinRateB\":" + MinRateB); }
            if (!String.IsNullOrEmpty(MinRateBMath)) { sb.Append(", \"MinRateBMath\":" + JsonConvert.ToString(MinRateBMath)); }
            if (!String.IsNullOrEmpty(MinRateBUOM)) { sb.Append(", \"MinRateBUOM\":" + JsonConvert.ToString(MinRateBUOM)); }
            if (!(MinRateBUOMAmount == 0)) { sb.Append(", \"MinRateBUOMAmount\":" + MinRateBUOMAmount); }
            if (!(MinRateC == 0)) { sb.Append(", \"MinRateC\":" + MinRateC); }
            if (!String.IsNullOrEmpty(MinRateCMath)) { sb.Append(", \"MinRateCMath\":" + JsonConvert.ToString(MinRateCMath)); }
            if (!String.IsNullOrEmpty(MinRateCUOM)) { sb.Append(", \"MinRateCUOM\":" + JsonConvert.ToString(MinRateCUOM)); }
            if (!(MinRateCUOMAmount == 0)) { sb.Append(", \"MinRateCUOMAmount\":" + MinRateCUOMAmount); }
            if (!(MaxRateA == 0)) { sb.Append(", \"MaxRateA\":" + MaxRateA); }
            if (!String.IsNullOrEmpty(MaxRateAMath)) { sb.Append(", \"MaxRateAMath\":" + JsonConvert.ToString(MaxRateAMath)); }
            if (!String.IsNullOrEmpty(MaxRateAUOM)) { sb.Append(", \"MaxRateAUOM\":" + JsonConvert.ToString(MaxRateAUOM)); }
            if (!(MaxRateAUOMAmount == 0)) { sb.Append(", \"MaxRateAUOMAmount\":" + MaxRateAUOMAmount); }
            if (!(MaxRateB == 0)) { sb.Append(", \"MaxRateB\":" + MaxRateB); }
            if (!String.IsNullOrEmpty(MaxRateBMath)) { sb.Append(", \"MaxRateBMath\":" + JsonConvert.ToString(MaxRateBMath)); }
            if (!String.IsNullOrEmpty(MaxRateBUOM)) { sb.Append(", \"MaxRateBUOM\":" + JsonConvert.ToString(MaxRateBUOM)); }
            if (!(MaxRateBUOMAmount == 0)) { sb.Append(", \"MaxRateBUOMAmount\":" + MaxRateBUOMAmount); }
            if (!(MaxRateC == 0)) { sb.Append(", \"MaxRateC\":" + MaxRateC); }
            if (!String.IsNullOrEmpty(MaxRateCMath)) { sb.Append(", \"MaxRateCMath\":" + JsonConvert.ToString(MaxRateCMath)); }
            if (!String.IsNullOrEmpty(MaxRateCUOM)) { sb.Append(", \"MaxRateCUOM\":" + JsonConvert.ToString(MaxRateCUOM)); }
            if (!(MaxRateCUOMAmount == 0)) { sb.Append(", \"MaxRateCUOMAmount\":" + MaxRateCUOMAmount); }
            sb.Append(" }");
            return sb.ToString();
        }
    }

    public class ChargesGroup : ElasticItem
    {
        public String ChargeDetailGUID;
        public String GroupCode;
        public String UIRate;
        public CalculationBreakout Calculation;
        public String MinUICharge;
        public String MaxUICharge;
        public DateTime ChargeStartDate;
        public DateTime ChargeEndDate;
        public String ChargeUse;
        public String ChargeType;
        public String ChargeDetailCode;
        public String ChargeDescription;
        public String ExclusionCountries;
        public ElasticCollection<ChargesNote> Notes;

        public ChargesGroup()
        {
            //this.ExclusionCountries = new List<String>();
            this.Notes = new ElasticCollection<ChargesNote>();
        }

        public override String SerializeForES()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{\"GroupCode\":" + JsonConvert.ToString(GroupCode));
            if (!String.IsNullOrEmpty(ChargeDetailGUID)) { sb.Append(", \"ChargeDetailGUID\":" + JsonConvert.ToString(ChargeDetailGUID)); }
            if (!String.IsNullOrEmpty(UIRate)) { sb.Append(", \"UIRate\":" + JsonConvert.ToString(UIRate)); }
            if (!String.IsNullOrEmpty(Calculation.Formula)) { sb.Append(", \"Calculation\":" + Calculation.SerializeForES()); }
            if (!String.IsNullOrEmpty(MinUICharge)) { sb.Append(", \"MinUICharge\":" + JsonConvert.ToString(MinUICharge)); }
            if (!String.IsNullOrEmpty(MaxUICharge)) { sb.Append(", \"MaxUICharge\":" + JsonConvert.ToString(MaxUICharge)); }
            if (!ChargeStartDate.Equals(DateTime.Parse("1/1/1900 12:00:00 AM"))) { sb.Append(", \"ChargeStartDate\":" + JsonConvert.ToString(ChargeStartDate.ToUniversalTime().ToString("s"))); }
            if (!ChargeEndDate.Equals(DateTime.Parse("1/1/1900 12:00:00 AM"))) { sb.Append(", \"ChargeEndDate\":" + JsonConvert.ToString(ChargeEndDate.ToUniversalTime().ToString("s"))); }
            if (!String.IsNullOrEmpty(ChargeUse)) { sb.Append(", \"ChargeUse\":" + JsonConvert.ToString(ChargeUse)); }
            if (!String.IsNullOrEmpty(ChargeType)) { sb.Append(", \"ChargeType\":" + JsonConvert.ToString(ChargeType)); }
            if (!String.IsNullOrEmpty(ChargeDetailCode)) { sb.Append(", \"ChargeDetailCode\":" + JsonConvert.ToString(ChargeDetailCode)); }
            if (!String.IsNullOrEmpty(ChargeDescription)) { sb.Append(", \"ChargeDescription\":" + JsonConvert.ToString(ChargeDescription)); }

            if (!ExclusionCountries.Equals("[\"\"]")) { sb.Append(", \"ExclusionCountries\":" + ExclusionCountries); }
            if (Notes.Items.Count > 0) { sb.Append(", \"Notes\":" + Notes.SerializeListItems()); }
            sb.Append("}");
            return sb.ToString();
        }

        public void LoadNotesFromDataTable(DataTable _NotesForRateGroups)
        {
            String FilterString = String.Format("ChargeDetailGUID = '{0}'", ChargeDetailGUID);
            List<DataRow> NotesForThisRateGroup = _NotesForRateGroups.Select(FilterString).ToList<DataRow>();

            foreach (DataRow _row in NotesForThisRateGroup)
            {
                ChargesNote _Note = new ChargesNote(_row["CultureCode"].ToString(), _row["NoteText"].ToString());
                Notes.Items.Add(_Note);
            }
        }

    }

    public class ChargesNote : ElasticItem
    {
        public String CultureCode;
        public String NoteText;

        public ChargesNote(String CultureCode, String NoteText)
        {
            this.CultureCode = CultureCode;
            this.NoteText = NoteText;
        }

        public override String SerializeForES()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{\"CultureCode\":" + JsonConvert.ToString(CultureCode));
            sb.Append(", \"NoteText\":" + JsonConvert.ToString(NoteText) + "}");
            return sb.ToString();
        }
    }

    public class Description : ElasticItem
    {
        public String CultureCode;
        public String DescriptionText;

        public Description(String CultureCode, String DescriptionText)
        {
            this.CultureCode = CultureCode;
            this.DescriptionText = DescriptionText;
        }

        public override String SerializeForES()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{\"CultureCode\":" + JsonConvert.ToString(CultureCode));
            sb.Append(", \"DescriptionText\":" + JsonConvert.ToString(DescriptionText) + "}");
            return sb.ToString();
        }
    }
}
