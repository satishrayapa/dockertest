using System;
using System.Collections.Generic;
using System.Text;
using TR.OGT.ChangeLedger.Domain.Entities;

namespace TR.OGT.ChangeLedger.Domain.Extensions
{
    public static class ChangeEventTypeExtensions
    {
        public static string ToWherePredicate(this ChangeEventType eventType)
        {
            var sb = new StringBuilder(" ct.Operation in (");

            if(eventType.HasFlag(ChangeEventType.Insert))
                sb.Append("'I',");

            if(eventType.HasFlag(ChangeEventType.Update))
                sb.Append("'U',");

            if(eventType.HasFlag(ChangeEventType.Delete))
                sb.Append("'D',");

            sb.Remove(sb.Length - 1, 1);
            sb.Append(")");

            return sb.ToString();
        }
    }
}
