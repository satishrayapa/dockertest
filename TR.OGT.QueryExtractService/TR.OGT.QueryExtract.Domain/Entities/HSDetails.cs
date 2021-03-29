using System;
using System.Collections.Generic;
using System.Linq;

namespace TR.OGT.QueryExtract.Domain
{
	public class HSDetails
    {
		public HSDetails(Guid prodClassificationDetailGUID)
		{
            ProdClassificationDetailGUID = prodClassificationDetailGUID;
        }

        /// <summary>Tariff number (can be fully qualified) for this set of information</summary>
        public string HSNumber { get; set; }
        /// <summary>First two digits</summary>
        public string Chapter { get; set; }
        /// <summary>First four digits</summary>
        public string Heading { get; set; }
        /// <summary>First six digits</summary>
        public string Subheading { get; set; }
        /// <summary>Units of Measure that are relevant for this Tariff Number</summary>
        public string UOMs { get; set; }
        /// <summary>Import or Export (or both)</summary>
        public string Uses { get; set; }
        /// <summary>Unique Identifier for this Tariff Number / Date Combination (from MSSQL)</summary>
        public Guid ProdClassificationDetailGUID { get; set; }
        /// <summary>Unique Identifier for this Tariff Schedule (from MSSQL)</summary>
        public Guid ProdClassificationGUID { get; set; }
        /// <summary>Country Codes associated with the tariff schedule</summary>
        public string CountryCodes { get; set; }
        /// <summary>Beginning of the period where tariff number takes effect</summary>
        public DateTime? StartDate { get; set; }
        /// <summary>End of the period where tariff number expires or is replaced</summary>
        public DateTime? EndDate { get; set; }
        /// <summary>Collection of description objects</summary>
        public IEnumerable<HSDescription> Descriptions { get; set; } = new List<HSDescription>();
        /// <summary>Collection of related HS objects</summary>
        public IEnumerable<RelatedHS> RelatedHS { get; set; } = new List<RelatedHS>();
        public IEnumerable<ChargeQuota> Quotas { get; set; } = new List<ChargeQuota>();
        public IEnumerable<PcNote> Notes { get; set; } = new List<PcNote>();
        public IEnumerable<BindingRullings> Rullings { get; set; } = new List<BindingRullings>();

        public void Merge(HSDetails details)
        {
            this.HSNumber = this.HSNumber ?? details.HSNumber;
            this.Chapter = this.Chapter ?? details.Chapter;
            this.Heading = this.Heading ?? details.Heading;
            this.Subheading = this.Subheading ?? details.Subheading;
            this.UOMs = this.UOMs ?? details.UOMs;
            this.Uses = this.Uses ?? details.Uses;
            this.ProdClassificationGUID = this.ProdClassificationGUID == default
                ? details.ProdClassificationGUID
                : this.ProdClassificationGUID;
            this.CountryCodes = this.CountryCodes ?? details.CountryCodes;
            this.StartDate = this.StartDate ?? details.StartDate;
            this.EndDate = this.EndDate ?? details.EndDate;
            this.Descriptions = MergeCollections(this.Descriptions, details.Descriptions);
            this.RelatedHS = MergeCollections(this.RelatedHS, details.RelatedHS);
            this.Quotas = MergeCollections(this.Quotas, details.Quotas);
            this.Notes = MergeCollections(this.Notes, details.Notes);
            this.Rullings = MergeCollections(this.Rullings, details.Rullings);
        }

        private IEnumerable<T> MergeCollections<T>(IEnumerable<T> firstCollection, IEnumerable<T> secondCollection)
        {
            var firstCollectionEmptyOrNull = firstCollection == null || !firstCollection.Any();
            var secondCollectionEmptyOrNull = secondCollection == null || !secondCollection.Any();

            if (firstCollectionEmptyOrNull && secondCollectionEmptyOrNull)
                return Array.Empty<T>();

            if (firstCollectionEmptyOrNull && !secondCollectionEmptyOrNull)
                return secondCollection;

            if (!firstCollectionEmptyOrNull && secondCollectionEmptyOrNull)
                return firstCollection;

            return firstCollection.Concat(secondCollection).ToList();
        }
    }
}
