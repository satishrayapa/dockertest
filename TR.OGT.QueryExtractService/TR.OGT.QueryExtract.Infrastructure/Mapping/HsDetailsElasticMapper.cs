using System.Collections.Generic;
using System.Linq;
using TR.OGT.QueryExtract.Domain;
using TR.OGT.QueryExtract.Infrastructure.Elastic;

namespace TR.OGT.QueryExtract.Infrastructure
{
    public static class HsDetailsElasticMapper
    {
        public static HsDetailsElasticDto Map(HSDetails source)
            => new HsDetailsElasticDto
            {
                ProdClassificationGUID = source.ProdClassificationGUID.ToString(),
                ProdClassificationDetailGUID = source.ProdClassificationDetailGUID.ToString(),
                HSBreakout = new HSBreakoutElasticDto
                {
                    Heading = source.Heading,
                    Subheading = source.Subheading,
                    Chapter = source.Chapter,
                },
                CountryCodes = source.CountryCodes,
                HSNumber = source.HSNumber,
                UOMs = source.UOMs,
                Uses = source.Uses,
                StartDate = source.StartDate,
                EndDate = source.EndDate,
                Descriptions = source.Descriptions.Select(Map),
                RelatedHS = source.RelatedHS.Select(Map),
                Quotas = source.Quotas.Select(Map),
                Notes = source.Notes.Select(Map),
                Rulings = source.Rullings.Select(Map)
            };

        public static DescriptionHSElasticDto Map(HSDescription hSDescription)
            => new DescriptionHSElasticDto
            {
                CultureCode = hSDescription.CultureCode,
                DescriptionText = hSDescription.DescriptionText,
                DisplayFlag = hSDescription.DisplayFlag,
                SortOrder = hSDescription.SortOrder.ToString()
            };

        public static RelatedHSElasticDto Map(RelatedHS relatedHs)
            => new RelatedHSElasticDto
            {
                RelatedHSNum = relatedHs.RelatedHSNum,
                Note = relatedHs.Note,
                StartDate = relatedHs.StartDate,
                EndDate = relatedHs.EndDate
            };

        public static ChargeQuotaElasticDto Map(ChargeQuota quota)
            => new ChargeQuotaElasticDto
            {
                Level = quota.Level,
                Type = quota.Type,
                UOM = quota.UOM,
                FillDate = quota.FillDate,
                QuotaStartDate = quota.QuotaStartDate,
                QuotaEndDate = quota.QuotaEndDate,
                IssuingCountries = quota.IssuingCountries,
                ApplicableCountries = quota.ApplicableCountries
            };

        public static PcNoteElasticDto Map(PcNote pcNote)
            => new PcNoteElasticDto
            {
                HSNumber = pcNote.HSNumber,
                NoteNumber = pcNote.NoteNumber,
                NoteType = pcNote.NoteType,
                NoteStartDate = pcNote.NoteStartDate,
                NoteEndDate = pcNote.NoteEndDate,
                Text = pcNote.Text.Select(Map)
            };

        public static PcNoteTextElasticDto Map(PcNoteText pcNoteText)
            => new PcNoteTextElasticDto
            {
                CultureCode = pcNoteText.CultureCode,
                NoteText = pcNoteText.NoteText
            };

        public static BindingRullingsElasticDto Map(BindingRullings bindingRullings)
            => new BindingRullingsElasticDto
            {
                HSNumber = bindingRullings.HSNumber,
                IssuingCountries = bindingRullings.IssuingCountries,
                RulingEndDate = bindingRullings.RulingEndDate,
                RulingReferenceCode = bindingRullings.RulingReferenceCode,
                RulingStartDate = bindingRullings.RulingStartDate,
                RulingType = bindingRullings.RulingType,
                Text = bindingRullings.Text.Select(Map)
            };

        public static BindingRullingsTextElasticDto Map(BindingRullingsText bindingRullingsText)
            => new BindingRullingsTextElasticDto
            {
                CultureCode = bindingRullingsText.CultureCode,
                RulingText = bindingRullingsText.RulingText,
                TextType = bindingRullingsText.TextType
            };

        public static IEnumerable<HsDetailsElasticDto> Map(IEnumerable<HSDetails> source)
            => source.Select(Map);
    }
}
