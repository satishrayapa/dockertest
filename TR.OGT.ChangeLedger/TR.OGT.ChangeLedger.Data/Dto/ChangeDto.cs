using System;
using TR.OGT.ChangeLedger.Domain.Entities;

namespace TR.OGT.ChangeLedger.Data.Dto
{
    public class ChangeDto
    {
        public Guid Id { get; set; }
        public ChangeEventType EventType {get; set;}
    }
}
