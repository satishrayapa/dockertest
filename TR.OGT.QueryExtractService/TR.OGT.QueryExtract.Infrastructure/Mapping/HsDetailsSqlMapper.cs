using System;
using System.Collections.Generic;
using System.Linq;
using TR.OGT.QueryExtract.Domain;
using TR.OGT.QueryExtract.Infrastructure.Sql;

namespace TR.OGT.QueryExtract.Infrastructure
{
    public static class HsDetailsSqlMapper
    {
        private static string SplitDelimeter = ",";

        private static HSDetails Map(HSDetailsSqlDto source)
            => new HSDetails(source.ProdClassificationDetailGUID)
            {
                ProdClassificationGUID = source.ProdClassificationGUID,
                Chapter = source.Chapter,
                Heading = source.Heading,
                Subheading = source.Subheading,
                CountryCodes = source.CountryCodes,
                HSNumber = source.HSNumber,
                UOMs = source.UOMs,
                Uses = source.Uses,
                StartDate = source.StartDate,
                EndDate = source.EndDate,
            };

        private static HSDescription Map(HSDescriptionSqlDto hSDescription)
            => new HSDescription
            {
                CultureCode = hSDescription.CultureCode,
                DescriptionText = hSDescription.DescriptionText,
                DisplayFlag = hSDescription.DisplayFlag,
                SortOrder = hSDescription.SortOrder
            };

        private static RelatedHS Map(RelatedHSSqlDto relatedHs)
            => new RelatedHS
            {
                RelatedHSNum = relatedHs.RelatedHSNum,
                Note = relatedHs.Note,
                StartDate = relatedHs.StartDate,
                EndDate = relatedHs.EndDate
            };

        private static ChargeQuota Map(ChargeQuotaSqlDto quotaDto)
            => new ChargeQuota
            {
                Level = quotaDto.Level,
                Type = quotaDto.Type,
                UOM = quotaDto.UOM,
                FillDate = quotaDto.FillDate,
                QuotaStartDate = quotaDto.QuotaStartDate,
                QuotaEndDate = quotaDto.QuotaEndDate,
                IssuingCountries = quotaDto.IssuingCountries?.Split(SplitDelimeter),
                ApplicableCountries = quotaDto.ApplicableCountries?.Split(SplitDelimeter)
            };

        private static PcNote Map(IGrouping<(string HSNumber, string NoteNumber, string NoteType), PcNoteSqlDto> noteGroup)
        {
            var noteDto = noteGroup.First();
            return new PcNote
            {
                HSNumber = noteDto.HSNumber,
                NoteNumber = noteDto.NoteNumber,
                NoteType = noteDto.NoteType,
                NoteEndDate = noteDto.NoteEndDate,
                NoteStartDate = noteDto.NoteStartDate,
                Text = noteGroup
                    .Where(i => !string.IsNullOrEmpty(i.CultureCode) || !string.IsNullOrEmpty(i.NoteText))
                    .Select(MapNoteText)
            };
        }

        private static PcNoteText MapNoteText(PcNoteSqlDto dto)
            => new PcNoteText
            {
                CultureCode = dto.CultureCode,
                NoteText = dto.NoteText
            };

        public static IReadOnlyCollection<HSDetails> Map(IEnumerable<HSDetailsSqlDto> dtos)
            => dtos.Select(Map).ToList();

        public static IReadOnlyCollection<HSDetails> Map(IEnumerable<PcNoteSqlDto> dtos)
        {
            var result = new List<HSDetails>();
            foreach (var group in Group(dtos))
            {
                var noteGroups = group.GroupBy(g => (g.HSNumber, g.NoteNumber, g.NoteType));
                var hsHedails = new HSDetails(group.Key)
                {
                    Notes = noteGroups.Select(Map).ToList()
                };
                result.Add(hsHedails);
            }

            return result;
        }

        public static IReadOnlyCollection<HSDetails> Map(IEnumerable<HSDescriptionSqlDto> dtos)
            => Group(dtos)
                .Select(gr => ToHSDetails(gr, g => g.Select(Map).ToList(), (hs, hsDesc) => hs.Descriptions = hsDesc))
                .ToList();

        public static IReadOnlyCollection<HSDetails> Map(IEnumerable<RelatedHSSqlDto> dtos)
            => Group(dtos)
                .Select(gr => ToHSDetails(gr, g => g.Select(Map).ToList(), (hs, relatedHs) => hs.RelatedHS = relatedHs))
                .ToList();

        public static IReadOnlyCollection<HSDetails> Map(IEnumerable<ChargeQuotaSqlDto> dtos)
            => Group(dtos)
                .Select(gr => ToHSDetails(gr, g => g.Select(Map).ToList(), (hs, quotas) => hs.Quotas = quotas))
                .ToList();

        private static HSDetails ToHSDetails<T, R>(
            IGrouping<Guid, T> group,
            Func<IEnumerable<T>, IReadOnlyCollection<R>> map,
            Action<HSDetails, IReadOnlyCollection<R>> apply)
            where T : BaseSqlDto
        {
            var result = new HSDetails(group.Key);

            apply(result, map(group));

            return result;
        }

        private static IEnumerable<IGrouping<Guid, T>> Group<T>(IEnumerable<T> collection)
            where T : BaseSqlDto
            => collection.GroupBy(g => g.ProdClassificationDetailGUID);
    }
}
