using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TR.OGT.Content.NoSQL.Elastic.DataTransfer
{
    public static class Serialize
    {
        public static String ElasticCollection<T>(List<T> Items) where T : ElasticItem
        {
            if (Items.Count == 1) { return Items[0].SerializeForES(); }
            else
            {
                StringBuilder _Serialized = new StringBuilder();
                _Serialized.Append("[");
                for (int i = 0; i < (Items.Count - 2); i++)
                {
                    _Serialized.Append(Items[i].SerializeForES() + ",");
                }
                _Serialized.Append(Items[Items.Count - 1].SerializeForES());
                _Serialized.Append("]");
                return _Serialized.ToString();
            }
        }

        public static String TopLevelCollection<T>(List<T> Items) where T : ElasticItem
        {
            if (Items.Count == 1) { return Items[0].SerializeForES(); }
            else
            {
                StringBuilder _Serialized = new StringBuilder();
                for (int i = 0; i < (Items.Count - 2); i++)
                {
                    _Serialized.Append(Items[i].SerializeForES() + ",");
                }
                _Serialized.Append(Items[Items.Count - 1].SerializeForES());
                return _Serialized.ToString();
            }
        }

        public static String ListOfStrings(List<String> Items)
        {
            if (Items.Count == 1) { return Items[0]; }
            else
            {
                StringBuilder _Serialized = new StringBuilder();
                _Serialized.Append("[");
                for (int i = 0; i < (Items.Count - 2); i++)
                {
                    _Serialized.Append(JsonConvert.ToString(Items[i]) + ",");
                }
                _Serialized.Append(JsonConvert.ToString(Items[Items.Count - 1]));
                _Serialized.Append("]");
                return _Serialized.ToString();
            }
        }
    }
}
