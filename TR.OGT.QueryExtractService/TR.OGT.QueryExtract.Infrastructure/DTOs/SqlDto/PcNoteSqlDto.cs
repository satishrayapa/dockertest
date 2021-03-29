using System;

namespace TR.OGT.QueryExtract.Infrastructure.Sql
{
    public class PcNoteSqlDto : BaseSqlDto
    {
        /// <summary>Sequential number for the note (think display order) by the publishing agency</summary>
        public string NoteNumber { get; set; }
        /// <summary>Depends on Agency (e.g. HS Note, UOM Note, etc)</summary>
        public string NoteType { get; set; }
        /// <summary>Date that the note takes effect</summary>
        public DateTime? NoteStartDate { get; set; }
        /// <summary>Date that the note expires</summary>
        public DateTime? NoteEndDate { get; set; }
        /// <summary>The same thing every culture code is everywhere, pinky.</summary>
        public string CultureCode { get; set; }
        /// <summary>Actual text of the note.</summary>
        public string NoteText { get; set; }
    }
}