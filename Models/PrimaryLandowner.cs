using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Township_API.Models
{

    [Table("PrimaryLandowner")]
    public class PrimaryLandowner
    {
        [Key]
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
        [Required]
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string? ShortName { get; set; }
        public string? Gender { get; set; }
        public string? BloodGroup { get; set; }
        public DateTime? DOB { get; set; }
        public string? EmailID { get; set; }
        public string? MobileNo { get; set; }
        public string? LandLine { get; set; }
       
        public string? NRD { get; set; }

        public string? Building { get; set; }
        public string? FlatNumber { get; set; }
        public DateTime? CardIssueDate { get; set; }
        public DateTime? CardPrintingDate { get; set; }
        public int? LogicalDeleted { get; set; } = 0;
        public DateTime? LandOwnerIssueDate { get; set; }
        [NotMapped]
        public string? nrdName { get; set; } 
       [NotMapped] 
        public string? buildingName { get; set; }
     } 
    [Table("DependentLandowner")]
    public class DependentLandOwner
    {
        [Required]
        public int PID { get; set; }
        [Key]
        public int ID { get; set; }
        [MaxLength(20)]
        public string? CSN { get; set; } 
        [Required] 
        [MaxLength(20)] 
        public string? IDNumber { get; set; }
        [MaxLength(20)]
        public string? TagNumber { get; set; }
        [MaxLength(20)]
        public string? PANnumber { get; set; }
        [MaxLength(20)]
        public string? PassportNo { get; set; }
        [MaxLength(20)]
        public string? LicenseNo { get; set; }
        [MaxLength(20)]
        public string? AadharCardId { get; set; }
        [MaxLength(20)]
        public string? VoterID { get; set; } 
        [Required] 
        [MaxLength(50)] 
        public string? FirstName { get; set; }
        [MaxLength(50)]
        public string? MiddleName { get; set; }
        [MaxLength(50)]
        [Required]
        public string? LastName { get; set; }
        [MaxLength(150)]
        public string? ShortName { get; set; }
        public string? Gender { get; set; }
        [MaxLength(10)]
        public string? BloodGroup { get; set; }
        public DateTime? DOB { get; set; }
        public string? MobileNo { get; set; }
        public DateTime? CardIssueDate { get; set; }
        public DateTime? CardPrintingDate { get; set; } 
        public int? LogicalDeleted { get; set; } = 0;   
        public DateTime? DependLandOwnerIssueDate { get; set; } 
         
    }


}
