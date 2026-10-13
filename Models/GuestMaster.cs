using Microsoft.AspNetCore.Antiforgery;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Township_API.Models
{
    [Table("GuestMaster")] 
    public class GuestMaster
    {
        [Key]
        public int ID { get; set; }
        public int? RID { get; set; }
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
        public DateTime? DOB { get; set; } = null;
        public string? EmailID { get; set; }
        public string? MobileNo { get; set; }
        public string? LandLine { get; set; }
        public string? Building { get; set; }
        public string? NRD { get; set; }
        public string? FlatNumber { get; set; }
        public DateTime? CardIssueDate { get; set; } = null;
        public DateTime? CardPrintingDate { get; set; } = null;

        public DateTime? ValidFrom { get; set; } = DateTime.Now;
        public DateTime? ValidTill { get; set; } = DateTime.Now;
        public int LogicalDeleted { get; set; } = 0;
        [NotMapped]
        public string? BuildingName { get; set; } = "";
        [NotMapped]
        public string? NRDName { get; set; } = "";


        [NotMapped]
        public string? ReferenceName { get; set; } = "";

    }

    [Table("VisitorMaster")]

    public class VisitorMaster
    {
        public int ID { get; set; }
        public string? HID { get; set; }
        public string? CSN { get; set; }
        public string? IDNumber { get; set; }
        public string? TagNumber { get; set; }
        public string? PANnumber { get; set; }
        public string? PassportNo { get; set; }
        public string? LicenseNo { get; set; }
        public string? ICEno { get; set; }
        public string? AadharCardId { get; set; }
        public string? VoterID { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        [Required]
        public string? LastName { get; set; }
        public string? ShortName { get; set; }
        public string? Gender { get; set; }
        public string? EmailID { get; set; }
        public string? MobileNo { get; set; }
        public string? LandLine { get; set; }
        [Required]
        public string? Building { get; set; }
        [Required]
        public string? NRD { get; set; }
        [Required] 
        public string? FlatNumber { get; set; }
        public DateTime? CardIssueDate { get; set; }
        public DateTime? CardPrintingDate { get; set; }
        public int? LogicalDeleted { get; set; }=1;
        [Required] 
        public DateTime? visitStartTime { get; set; }
        public DateTime? visitEndTime { get; set; }
        public string? visitPurpose { get; set; }
        public string? visitDesription { get; set; }

        public string? visitStatus { get; set; } = string.Empty; //close/complete/pending


        [NotMapped]
        public string? NRDName { get; set; } = null;
        [NotMapped]
        public string? BuildingName { get; set; } = null;
        [NotMapped]
        [DataType(DataType.Date)]
        public string? visitStartDate { get; set; }
        [NotMapped]
        [DataType(DataType.Date)]
        public string? visitEndDate { get; set; }  
          
    }

}
