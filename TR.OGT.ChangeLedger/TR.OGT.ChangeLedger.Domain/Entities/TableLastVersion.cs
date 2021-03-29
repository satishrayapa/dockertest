using Amazon.DynamoDBv2.DataModel;

namespace TR.OGT.ChangeLedger.Data.Entities
{
    // table name is overwriten when query is sent
    [DynamoDBTable("default")]
    public class TableLastVersion
    {
        [DynamoDBHashKey]
        public string TableName { get; set; }
        [DynamoDBProperty]
        public long LastVersion { get; set; }
    }
}
