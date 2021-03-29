using System;

namespace TR.OGT.ChangeLedger.Domain.Entities
{
    [Flags]
    public enum ChangeEventType
    {
        Insert = 1,
        Update = 2,
        Delete = 4
    }
}
