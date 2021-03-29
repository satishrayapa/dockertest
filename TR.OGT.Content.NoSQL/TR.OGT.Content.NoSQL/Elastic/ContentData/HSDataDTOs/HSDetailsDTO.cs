using System;
using System.Collections.Generic;
using System.Text;

namespace TR.OGT.Content.NoSQL.Elastic.ContentData.HSDataDTOs
{
    /// <summary>Top level object for HS Details -- this will be our base DTO</summary>
    public class HSDetailsDTO
    {
        /// <summary>Tariff number (can be fully qualified) for this set of information</summary>
        public String HSNumber;
        /// <summary>Tariff number broken out by Chapter/Subchapter/Heading</summary>
        public HSBreakoutDTO HSBreakout;
        /// <summary>Units of Measure that are relevant for this Tariff Number</summary>
        public List<String> UOMs;
        /// <summary>Import or Export (or both)</summary>
        public List<String> Uses;
        /// <summary>Unique Identifier for this Tariff Number / Date Combination (from MSSQL)</summary>
        public String ProdClassificationDetailGUID;
        /// <summary>Unique Identifier for this Tariff Schedule (from MSSQL)</summary>
        public String ProdClassificationGUID;
        /// <summary>Country Codes associated with the tariff schedule</summary>
        public List<String> CountryCodes;
        /// <summary>Beginning of the period where tariff number takes effect</summary>
        public DateTime? StartDate;
        /// <summary>End of the period where tariff number expires or is replaced</summary>
        public DateTime? EndDate;

        //If the serializer proves too slow, we can change these to ElasticCollections and use string templates

        /// <summary>Collection of ECN numbers for which this tariff is linked</summary>
        public List<ECNHSCorrelationDTO> ECNHSCorrelation;
        /// <summary>Collection of additional codes that are declared with this tariff number</summary>
        public List<AdditionalCodeDTO> AdditionalCodes;
        /// <summary>Collection of data elements that are required for this tariff</summary>
        public List<DeclarableElementDTO> DeclarableElements;
        //TODO: Actually figure out what privilege codes are.
        /// <summary>Collection of preferential rates?</summary>
        public List<PrivilegeCodeDTO> PrivilegeCodes;
        /// <summary>Collection of other tariff numbers that are related</summary>
        public List<RelatedHSDTO> RelatedHS;
        /// <summary>Collection of country specific controls that apply to this tariff</summary>
        public List<ControlDTO> Controls;
        /// <summary>Collection of country specific quotas that apply to this tariff</summary>
        public List<QuotaDTO> Quotas;
        /// <summary>Collection of notes that assist in classification or shipment of goods</summary>
        public List<NoteDTO> Notes;
        /// <summary>Collection of rulings that set precedence classification of goods (for informational purposes)</summary>
        public List<RulingDTO> Rulings;
        /// <summary>Information that allows duty, tax, fee, and supplemental charge calculations</summary>
        public List<ChargesDTO> Charge;
        /// <summary>Description of the Tariff Number (possible in multiple languages -- 
        /// may contain multiple ordered entries for a single language)</summary>
        public List<DescriptionDTO> Descriptions;
    }
}
