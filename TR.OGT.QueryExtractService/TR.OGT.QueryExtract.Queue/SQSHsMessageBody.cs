using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace TR.OGT.QueryExtract.Queue
{
	public class SQSHsMessageBody
	{
		public Guid GuidChanged { get; set; }

		[JsonConverter(typeof(StringEnumConverter))]
		public EventType EventType { get; set; }
	}

	public enum EventType
	{
		Insert = 0,
		Update = 1,
		Delete = 2,
	}
}
