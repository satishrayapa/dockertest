using Nest;

namespace TR.OGT.QueryExtract.Infrastructure.Elastic
{

	public class HSBreakoutElasticDto
	{
		/// <summary>First two digits</summary>
		[Keyword(Name = nameof(Chapter))]
		public string Chapter { get; set; }

		/// <summary>First four digits</summary>
		[Keyword(Name = nameof(Heading))]
		public string Heading { get; set; }

		/// <summary>First six digits</summary>
		[Keyword(Name = nameof(Subheading))]
		public string Subheading { get; set; }
	}
}
