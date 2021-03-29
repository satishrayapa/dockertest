using System.Collections.Generic;
using TR.OGT.QueryExtract.Infrastructure.Sql;

namespace TR.OGT.QueryExtract.Data
{
    internal static class PcNotesSql
    {
        public static List<string> Columns => new List<string>
        {
            $"n.ParentNoteGUID as {BaseSqlDto.ProdClassificationDetailGUIDColumnName}",
            "n.NoteGUID",
            "n.HSNumber",
            "n.NoteNumber",
            "n.NoteType",
            "n.EffectivityDate as NoteStartDate",
            "n.ExpirationDate as NoteEndDate",
            "n.CultureCode",
            "n.NoteText"
        };

        public static List<string> Joins => new List<string>
        {
            "JOIN tPcNotes n (nolock) on tmp.Id = n.ParentNoteGUID"
        };

        public static string Where => "n.ParentNoteType = 'prodclassificationdetailguid'";
    }
}
