using System;
using System.Collections.Generic;

namespace TR.OGT.QueryExtract.Domain
{
    public class PcNote
    {
        public string HSNumber { get; set; }
        /// <summary>Sequential number for the note (think display order) by the publishing agency</summary>
        public string NoteNumber { get; set; }
        /// <summary>Depends on Agency (e.g. HS Note, UOM Note, etc)</summary>
        public string NoteType { get; set; }
        /// <summary>Date that the note takes effect</summary>
        public DateTime? NoteStartDate { get; set; }
        /// <summary>Date that the note expires</summary>
        public DateTime? NoteEndDate { get; set; }
        /// <summary>List of translations for this note</summary>
        public IEnumerable<PcNoteText> Text { get; set; }
    }

    public class PcNoteText
    {
        /// <summary>The same thing every culture code is everywhere, pinky.</summary>
        public string CultureCode { get; set; }
        /// <summary>Actual text of the note.</summary>
        public string NoteText { get; set; }
    }
}
