using Nest;
using System;

namespace TR.OGT.QueryExtract.Infrastructure.Elastic
{

	/// <summary>Another HS that is associated with this HS</summary>
	public class RelatedHSElasticDto
	{
		/// <summary>HS Number</summary>
		[Keyword(Name = nameof(RelatedHSNum))]
		public string RelatedHSNum { get; set; }

		/// <summary>How it's related to this HS</summary>
		[Text(Name = nameof(Note))]
		public string Note { get; set; }

		/// <summary>Date for which this relationship takes effect</summary>
		[Date(Name = nameof(StartDate))]
		public DateTime? StartDate { get; set; }

		/// <summary>Date for which this relationship expires</summary>
		[Date(Name = nameof(EndDate))]
		public DateTime? EndDate { get; set; }
	}
}
