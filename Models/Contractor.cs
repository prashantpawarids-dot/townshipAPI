using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Township_API.Models
{
    //  Contractor
    [Table("Contractor")]
    public class Contractor
    {
        public int ID { get; set; }
        public string? CSN { get; set; }
        public string? IDNumber { get; set; }
        public string? TagNumber { get; set; }
        public string? PANnumber { get; set; }
        public string? PassportNo { get; set; }
        public string? LicenseNo { get; set; }
        public string? ICEno { get; set; }
        public string? AadharCardId { get; set; }
        public string? VoterID { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? ShortName { get; set; }
        public string? Gender { get; set; }
        public string? BloodGroup { get; set; }
        public DateTime? DOB { get; set; }
        public string? EmailID { get; set; }
        public string? MobileNo { get; set; }
        public string? LandLine { get; set; }
        public DateTime? CardIssueDate { get; set; }
        public DateTime? CardPrintingDate { get; set; }
        public DateTime? RegistrationIssueDate { get; set; }
        public int? LogicalDeleted { get; set; } = 0;
        public string? ContactPerson { get; set; } = null;
        public string? RegistrationNumber { get; set; } = null;
        public DateTime? ValidFromDate { get; set; }
        public DateTime? ValidToDate { get; set; }
        public string? Address { get; set; }
        public int? ContactorType { get; set; } = 0;
        public string? Company { get; set; }
        public string? Agency { get; set; }
        public int? Nbr { get; set; }

        public string? NbrContractorLogo { get; set; }

        [NotMapped]
        public string? ContractorTypeName { get; set; } = null;

        [NotMapped]
        public string? DefaultNRDAccess { get; set; } = null;

        [NotMapped]
        public string? nbrName { get; set; } = null;


    }

    public class ContractorAgency
    {
        //[Key]
        public string? Agency { get; set; } = "";
    }

    public class ContractorCompany
    {
        //[Key]
        public string? Company { get; set; } = "";
    }


    [Table("DependentContractor")]
    public class DependentContractor
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public int PID { get; set; }        //Primary-ResidentID
        public string? CSN { get; set; }
        public string? IDNumber { get; set; }
        public string? TagNumber { get; set; }
        public string? PANnumber { get; set; }
        public string? PassportNo { get; set; }
        public string? LicenseNo { get; set; }
        public string? AadharCardId { get; set; }
        public string? VoterID { get; set; }
        [Required]
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        [Required]
        public string? LastName { get; set; }
        public string? ShortName { get; set; }
        public string? Gender { get; set; }
        public string? BloodGroup { get; set; }
        public DateTime? DOB { get; set; }
        public string? EmailID { get; set; }
        public string? MobileNo { get; set; }
        public string? LandLine { get; set; }
        public DateTime? CardIssueDate { get; set; }
        public DateTime? CardPrintingDate { get; set; }
        public DateTime? RegistrationIssueDate { get; set; }
        public int LogicalDeleted { get; set; }
    }

}
