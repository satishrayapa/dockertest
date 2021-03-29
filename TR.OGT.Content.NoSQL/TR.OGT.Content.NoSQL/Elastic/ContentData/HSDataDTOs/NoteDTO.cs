using System;
using System.Collections.Generic;
using System.Text;

namespace TR.OGT.Content.NoSQL.Elastic.ContentData.HSDataDTOs
{
    /// <summary>Generic object for holding notes</summary>
    public class NoteDTO
    {
        /// <summary>HS Number (full or partial) associated to the Note.
        /// If this is a partial number as a parent of a full number, 
        /// go look for more data at the HSData (parent) level before returning from the API</summary>
        public String HSNumber;
        /// <summary>Sequential number for the note (think display order) by the publishing agency</summary>
        public String NoteNumber;
        /// <summary>Depends on Agency (e.g. HS Note, UOM Note, etc)</summary>
        public String NoteType;
        /// <summary>Date that the note takes effect</summary>
        public DateTime? NoteStartDate;
        /// <summary>Date that the note expires</summary>
        public DateTime? NoteEndDate;
        /// <summary>List of translations for this note</summary>
        public List<NoteTextDTO> Text;
    }

    /// <summary>A translations for a note</summary>
    public class NoteTextDTO
    {
        /// <summary>The same thing every culture code is everywhere, pinky.</summary>
        public String CultureCode;
        /// <summary>Actual text of the note.</summary>
        public String NoteText;
    }
}
