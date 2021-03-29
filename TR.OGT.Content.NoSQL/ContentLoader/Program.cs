using System;

namespace ContentLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            String dbConnectionString = "UID=IP_Content_RO;PWD=gIrQG5n1y3#@aimr8V{BYvFOQ;Initial Catalog=Content_InProcess;Server=CLTLABCONTDB01;MultipleActiveResultSets=True";
            TR.OGT.Content.NoSQL.Elastic.DataTransfer.ElasticConfig ESSettings = new TR.OGT.Content.NoSQL.Elastic.DataTransfer.ElasticConfig("ContentLoader");

            TR.OGT.Content.NoSQL.Elastic.DataTransfer.Tariff.LoadCountry(dbConnectionString, ESSettings, "AE");

            Console.WriteLine("Finish");

        }
    }
}
