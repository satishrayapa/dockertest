using System;
using System.Collections.Generic;
using System.Text;

namespace TR.OGT.Content.NoSQL.Elastic.ContentData.HSDataDTOs
{
    public class ChargesDTO
    {
        /// <summary>Countries that are imposing the charges</summary>
        public List<String> IssuingCountries;
        /// <summary>Countries that the charges apply to</summary>
        public List<String> ApplicableCountries;
        /// <summary>Unique Identifier from MSSQL (Charge Header Record)</summary>
        public String ChargeDetailGUID;
        /// <summary>Rate as it displays in the UI</summary>
        public String UIRate;
        /// <summary>Currency in which the rate is published</summary>
        public String CurrencyCode;
        /// <summary>Collection of elements required for programmatic calculation of a charge</summary>
        public List<ChargeCalculationDTO> Calculation;
        /// <summary>Collection of elements for restricted destinations (i.e. countries, companies)</summary>
        public List<ChargeRestrictedDetailDTO> RestrictedDetails;
        /// <summary>Display rate for charge lower bound on the UI (if applicable)</summary>
        public String MinUICharge;
        /// <summary>Display rate for charge upper bound on the UI (if applicable)</summary>
        public String MaxUICharge;
        /// <summary>Date the charge becomes effective</summary>
        public DateTime? ChargeStartDate;
        /// <summary>Date the charge expires</summary>
        public DateTime? ChargeEndDate;
        /// <summary>Import or Export</summary>
        public String ChargeUse;
        /// <summary>Describes whether a charge is duty, VAT, tax, fee, etc</summary>
        public String ChargeType;
        /// <summary>Code for the specific type of charge under the Charge Type category -- MAIN, PREFER, EXCISE, etc</summary>
        public ChargeDetailTypeDTO ChargeDetailType;      // ---------- populate this from a lookup on ChargeDetailTypeCode for the descriptions
        /// <summary>Trade Program Description that the charge applies to (FTA, MFN, etc)</summary>
        public ChargeProgramDTO ChargeProgram;            // ---------- populate this from a lookup on ChargeProgramCode for the descriptions
        /// <summary>Whether there are Documents Associated with the Charge.  If this is Y then please use GTN Docs to search for them 
        ///     based on [ PartnerID = 1005 | ChargeDetailGUID | Charges | Content ] as the association.</summary>
        public Boolean AttachedDocuments;
        /// <summary>Collection of Notes Associated with the Charge</summary>
        public List<ChargeNoteDTO> Notes;
        /// <summary>Collection of Quotas Associated with the Charge</summary>
        public List<ChargeQuotaDTO> Quotas;

    }

    /// <summary>Elements required for programmatic calculation of a charge</summary>
    public class ChargeCalculationDTO
    {
        /// <summary>Collection of rates that make up a given charge</summary>
        public List<ChargeCalculationRateDTO> Rate;
    }

    /// <summary>Components that make up given rate</summary>
    public class ChargeCalculationRateDTO
    {
        /// <summary>Type of rate: Main Rate, Preferential, ADD/CVD, VAT/GST, Export or Other</summary>
        public String Category;
        /// <summary>Formula that describes how to put rate components together</summary>
        public String Formula;
        /// <summary>Components that make up given rate</summary>
        public List<ChargeCalculationRateComponentDTO> RateComponents;
    }

    /// <summary>Data elements for each rate component</summary>
    public class ChargeCalculationRateComponentDTO
    {
        /// <summary>Type (A, B, or C in current content)</summary>
        public String Type;
        /// <summary>Numeric element of the rate</summary>
        public Double Rate;
        /// <summary>How to treat numeric element (mathematical operator)</summary>
        public String Math;
        /// <summary>Unit of Measure for the rate component</summary>
        public String UOM;
        /// <summary>Quantity associated with the Unit of Measure (How many Pieces, Dozens, SqFt, KG, etc)</summary>
        public Double UOMAmount;
    }

    /// <summary>Company or Country related restrictions to a charge (e.g. ADD/CVD)</summary>
    public class ChargeRestrictedDetailDTO
    {
        /// <summary>The country that this restriction applies to</summary>
        public String CountryCode;
        /// <summary>Restriction group that applies</summary>
        public String RestrictedCode;
        /// <summary>Specific code for an individual restriction</summary>
        public String AdditionalCode;
        /// <summary>What is restricted</summary>
        public String Restriction;
        /// <summary>List of companies that the restriction applies to</summary>
        public List<ChargeRestrictionCompany> CompanyList;
    }

    /// <summary>Individual company object for restrictions</summary>
    public class ChargeRestrictionCompany
    {
        /// <summary>Language for the Company Name</summary>
        public String CultureCode;
        /// <summary>Actual name of the Company</summary>
        public String CompanyName;
    }

    /// <summary>Notes about a charge</summary>
    public class ChargeNoteDTO
    {
        /// <summary>Language for the Note</summary>
        public String CultureCode;
        /// <summary>Actual Text of the Note</summary>
        public String NoteText;
    }

    /// <summary>Maximum limits (value or quantity) on specific goods</summary>
    public class ChargeQuotaDTO
    {
        /// <summary>Quantity or Value for the quota</summary>
        public String Level;
        /// <summary>Describes (IMPORTVALUE | QUANTITY | NOLIMIT) what the level is</summary>
        public String Type;
        /// <summary>Unit of Measure for the level</summary>
        public String UOM;
        /// <summary>Date that the quota is filled</summary>
        public DateTime? FillDate;
        /// <summary>Date the quota takes effect</summary>
        public DateTime? QuotaStartDate;
        /// <summary>Date the quote expires</summary>
        public DateTime? QuotaEndDate;
    }

    /// <summary>Charge Type Details for Charge</summary>
    public class ChargeDetailTypeDTO
    {
        /// <summary>Short internal code assigned to each subcategory</summary>
        public String ChargeDetailTypeCode;
        /// <summary>Descriptions associated with this Charge Detail Type Code </summary>
        public List<DescriptionDTO> ChargeTypeDescriptions;

        //Dev Note: Only the ChargeDetailTypeCodes are stored in the ES database, 
        // we need to do separate lookups to populate description texts from the master list
        // before providing to the upstream API.
    }

    /// <summary>Charge Program Details for Charge</summary>
    public class ChargeProgramDTO
    {
        /// <summary>Charge Program Code (USCOLONE or similar)</summary>
        public String ChargeProgramCode;
        /// <summary>Descriptions associated with this Charge Program Code </summary>
        public List<DescriptionDTO> ChargeProgramDescriptions;

        //Dev Note: Only the ChargeProgramCodes are stored in the ES database, 
        // we need to do separate lookups to populate description texts from the master list
        // before providing to the upstream API.
    }
}
