using System;
using System.Collections.Generic;
using System.Text;

namespace TR.OGT.ChangeLedger.Data.Dto
{
    public class LastVersionDto
    {
        public string TableName { get; set; }
        public long LastVersion { get; set; }
    }
}
