using System;
using System.Collections.Generic;
using System.Text;

namespace TR.OGT.Content.NoSQL.Elastic.DataTransfer
{
    /// <summary>
    /// The class that you actually extend to implement business objects.
    /// </summary>
    public abstract class ElasticItem
    {
        public String ESIndex;
        public String ESId;

        /// <summary>
        /// Use the stringbuilder to generate a two-line ndjson as per the Elastic bulk API spec.
        /// Example at: https://ipwiki.integrationpoint.net/index.php?title=ElasticItem_Sample_Class
        /// </summary>
        /// <returns></returns>
        abstract public String SerializeForES();

    }
}
