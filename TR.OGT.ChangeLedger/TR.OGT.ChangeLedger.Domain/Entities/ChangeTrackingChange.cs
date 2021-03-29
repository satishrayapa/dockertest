using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace TR.OGT.ChangeLedger.Domain.Entities
{
    public class ChangeTrackingChange
    {
        public Guid GuidChanged { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ChangeEventType EventType { get; set; }

        public override int GetHashCode()
        {
            return this.GuidChanged.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }

            return ((ChangeTrackingChange)obj).GuidChanged == this.GuidChanged;
        }
    }
}
