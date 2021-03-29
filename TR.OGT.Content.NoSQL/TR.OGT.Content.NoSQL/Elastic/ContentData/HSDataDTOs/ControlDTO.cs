using System;
using System.Collections.Generic;
using System.Text;

namespace TR.OGT.Content.NoSQL.Elastic.ContentData.HSDataDTOs
{
    /// <summary>Data elements for describing a control (i.e. OGA/PGA)</summary>
    public class ControlDTO
    {
        /// <summary>HS Number (full or partial) associated to the control
        /// If this is a partial number as a parent of a full number, 
        /// go look for more data at the HSData (parent) level before returning from the API</summary>
        public String HSNumber;
        /// <summary>What control it is ( IMPORT | EXPORT | COUNTRYIMPORT | COUNTRYEXPORT )</summary>
        public String Control;
        /// <summary>Internal Code for distinguishing a unique control (from Content SQL)</summary>
        public String ControlType;
        /// <summary>List of countries imposing the control</summary>
        public List<String> IssuingCountries;
        /// <summary>List of countries that the control applies to</summary>
        public List<String> ApplicableCountries;
        /// <summary>Whether there are Documents Associated with the Control.  If this is Y then please use GTN Docs to search for them 
        ///     based on [ PartnerID = 1005 | ControlType | Control | Content ] as the association.</summary>
        public Boolean AttachedDocuments;
        /// <summary>Date that the control takes effect</summary>
        public DateTime? StartDate;
        /// <summary>Date that the control expires</summary>
        public DateTime? EndDate;
        /// <summary>Agencies imposing the control</summary>
        public List<ControlAgencyDTO> Agencies;
        // Descriptions are only stored on Control Type, this needs to be populated from a separate lookup after Control Type is available for Lookup.
        /// <summary> Descriptions for this ControlType. </summary>
        public List<DescriptionDTO> Descriptions;  // ---------- populate this from a lookup on ControlType for the descriptions
    }

    /// <summary>Instance of an Agency that imposes a control</summary>
    public class ControlAgencyDTO
    {
        /// <summary>Country that agency belongs to</summary>
        public String Country;
        /// <summary>Acronym for which the agency identifies itself</summary>
        public String Acronym;
        /// <summary>Unique Identifier for an Agency</summary>
        public String AgencyGUID;
    }
}
