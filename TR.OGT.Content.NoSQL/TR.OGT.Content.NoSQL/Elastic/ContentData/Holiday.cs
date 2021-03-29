using System;
using System.Collections.Generic;
using TR.OGT.Content.NoSQL.Elastic.DataTransfer;
using System.Text;
using Newtonsoft.Json;

namespace TR.OGT.Content.NoSQL.Elastic.ContentData
{
    public class Holiday : ElasticItem
    {
        public String Name;
        public DateTime StartDate;
        public DateTime EndDate;

        public Holiday() { }

        public override String SerializeForES()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{\"Name\":" + JsonConvert.ToString(Name));
            sb.Append(", \"StartDate\":" + JsonConvert.ToString(StartDate));
            sb.Append(", \"EndDate\":" + JsonConvert.ToString(EndDate));
            sb.Append(" }");
            return sb.ToString();
        }
    }
}
