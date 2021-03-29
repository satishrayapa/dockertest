using Nest;
using System;
using System.Collections.Generic;

namespace TR.OGT.QueryExtract.Infrastructure.Elastic
{
    public class HsDetailsElasticDto
    {
        [Ignore]
        public string Id { get; set; }
        /// <summary>Tariff number (can be fully qualified) for this set of information</summary>
        [Keyword(Name = nameof(HSNumber))]
        public string HSNumber { get; set; }

        [PropertyName(nameof(HSBreakout))]
        public HSBreakoutElasticDto HSBreakout { get; set; }

        /// <summary>Units of Measure that are relevant for this Tariff Number</summary>
        [Text(Name = nameof(UOMs))]
        public string UOMs { get; set; }

        /// <summary>Import or Export (or both)</summary>
        [Keyword(Name = nameof(Uses))]
        public string Uses { get; set; }

        /// <summary>Unique Identifier for this Tariff Number / Date Combination (from MSSQL)</summary>
        [Keyword(Name = nameof(ProdClassificationDetailGUID))]
        public string ProdClassificationDetailGUID { get; set; }

        /// <summary>Unique Identifier for this Tariff Schedule (from MSSQL)</summary>
        [Keyword(Name = nameof(ProdClassificationGUID))]
        public string ProdClassificationGUID { get; set; }

        /// <summary>Country Codes associated with the tariff schedule</summary>
        [Keyword(Name = nameof(CountryCodes))]
        public string CountryCodes { get; set; }

        /// <summary>Beginning of the period where tariff number takes effect</summary>
        [Date(Name = nameof(StartDate))]
        public DateTime? StartDate { get; set; }

        /// <summary>End of the period where tariff number expires or is replaced</summary>
        [Date(Name = nameof(EndDate))]
        public DateTime? EndDate { get; set; }

        [Nested]
        [PropertyName(nameof(RelatedHS))]
        public IEnumerable<RelatedHSElasticDto> RelatedHS { get; set; }

        [Nested]
        [PropertyName(nameof(Descriptions))]
        public IEnumerable<DescriptionHSElasticDto> Descriptions { get; set; }

        [Nested]
        [PropertyName(nameof(Quotas))]
        public IEnumerable<ChargeQuotaElasticDto> Quotas { get; set; }

        [Nested]
        [PropertyName(nameof(Notes))]
        public IEnumerable<PcNoteElasticDto> Notes { get; set; }

        [Nested]
        [PropertyName(nameof(Rulings))]
        public IEnumerable<BindingRullingsElasticDto> Rulings { get; set; }
    }
}
