using Nest;
using System;
using System.Collections.Generic;

namespace TR.OGT.QueryExtract.Infrastructure.Elastic
{
    public class PcNoteElasticDto
    {
        [Keyword(Name = nameof(HSNumber))]
        public string HSNumber { get; set; }

        /// <summary>Sequential number for the note (think display order) by the publishing agency</summary>
        [Keyword(Name = nameof(NoteNumber))]
        public string NoteNumber { get; set; }

        /// <summary>Depends on Agency (e.g. HS Note, UOM Note, etc)</summary>
        [Keyword(Name = nameof(NoteType))]
        public string NoteType { get; set; }

        /// <summary>Date that the note takes effect</summary>
        [Date(Name = nameof(NoteStartDate))]
        public DateTime? NoteStartDate { get; set; }

        /// <summary>Date that the note expires</summary>
        [Date(Name = nameof(NoteEndDate))]
        public DateTime? NoteEndDate { get; set; }

        /// <summary>List of translations for this note</summary>
        [Nested]
        [PropertyName(nameof(Text))]
        public IEnumerable<PcNoteTextElasticDto> Text { get; set; }
    }

    public class PcNoteTextElasticDto
    {
        /// <summary>The same thing every culture code is everywhere, pinky.</summary>
        [Keyword(Name = nameof(CultureCode))]
        public string CultureCode { get; set; }
        /// <summary>Actual text of the note.</summary>
        [Text(Name = nameof(NoteText))]
        public string NoteText { get; set; }
    }
}