using System.Collections.Generic;
using System.Linq;
using TR.OGT.QueryExtract.Domain;
using TR.OGT.QueryExtract.Infrastructure.Sql;

namespace TR.OGT.QueryExtract.Infrastructure
{
    public static class BindingRullingsSqlMapper
    {
        public static IEnumerable<HSDetails> Map(
            IEnumerable<BindingRullingsSqlDto> rullingsDtos,
            IEnumerable<BindingRullingsTextSqlDto> rullingsTextDtos)
        {
            var rullingsText = rullingsTextDtos
                .GroupBy(g => g.BindingRulingGUID)
                .ToDictionary(k => k.Key, v => v.Select(Map));

            return rullingsDtos
                .Select(r => Map(r, rullingsText.GetValueOrDefault(r.BindingRulingGUID)))
                .GroupBy(g => g.ProdClassificationDetailGUID)
                .Select(g => new HSDetails(g.Key)
                {
                    Rullings = g
                });
        }

        private static BindingRullings Map(BindingRullingsSqlDto dto, IEnumerable<BindingRullingsText> bindingRullingsTexts)
            => new BindingRullings
            {
                ProdClassificationDetailGUID = dto.ProdClassificationDetailGUID,
                HSNumber = dto.HSNumber,
                IssuingCountries = dto.IssuingCountries,
                RulingEndDate = dto.RulingEndDate,
                RulingReferenceCode = dto.RulingReferenceCode,
                RulingStartDate = dto.RulingStartDate,
                RulingType = dto.RulingType,
                Text = bindingRullingsTexts
            };

        private static BindingRullingsText Map(BindingRullingsTextSqlDto dto)
            => new BindingRullingsText
            {
                CultureCode = dto.CultureCode,
                RulingText = dto.RulingText,
                TextType = dto.TextType
            };
    }
}
