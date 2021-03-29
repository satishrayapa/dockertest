using Nest;
using System;
using System.Collections.Generic;

namespace TR.OGT.QueryExtract.Infrastructure.Elastic
{
    public class ChargeQuotaElasticDto
    {
        /// <summary>Quantity or Value for the quota</summary>
        [Keyword(Name = nameof(Level))]
        public string Level { get; set; }

        /// <summary>Describes (IMPORTVALUE | QUANTITY | NOLIMIT) what the level is</summary>
        [Keyword(Name = nameof(Type))]
        public string Type { get; set; }

        /// <summary>Unit of Measure for the level</summary>
        [Keyword(Name = nameof(UOM))]
        public string UOM { get; set; }

        /// <summary>Date that the quota is filled</summary>
        [Date(Name = nameof(FillDate))]
        public DateTime? FillDate { get; set; }

        /// <summary>Date the quota takes effect</summary>
        [Date(Name = nameof(QuotaStartDate))]
        public DateTime? QuotaStartDate { get; set; }

        /// <summary>Date the quote expires</summary>
        [Date(Name = nameof(QuotaEndDate))]
        public DateTime? QuotaEndDate { get; set; }

        /// <summary>Countries that are imposing the quota</summary>
        [Keyword(Name = nameof(IssuingCountries))]
        public IEnumerable<string> IssuingCountries { get; set; }

        /// <summary>Countries that the quota applies to</summary>
        [Keyword(Name = nameof(ApplicableCountries))]
        public IEnumerable<string> ApplicableCountries { get; set; }
    }
}