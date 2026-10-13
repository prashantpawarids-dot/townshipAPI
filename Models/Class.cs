using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Township_API.Models
{
    [Table("Company")]
    public class Company
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string companyCode { get; set; }
        [Required]
        public string companyName { get; set; }
        public string? city { get; set; }
        public string? address { get; set; }
        public string? GSTnumber { get; set; }
        public string? pannumber { get; set; }
        public string? mobileNo { get; set; }
        public string? landLine { get; set; }
        public string? ContactPerson { get; set; }
        public string? ContactMobile { get; set; }
        public DateTime? ValidFromDate { get; set; }
        public DateTime? ValidToDate { get; set; }
        public bool? isactive { get; set; } = true;
        public bool? isdeleted { get; set; } = false;
        public int? createdby { get; set; } = 0;
        public DateTime? createdon { get; set; }
        public int? updatedby { get; set; } = 0;
        public DateTime? updatedon { get; set; }

        public List<Project>? projects { get; set; }
    }

    [Table("Project")]
    public class Project
    {
        [Key]
        public int? id { get; set; }
        [Required]
        public string ProjectCode { get; set; }
        [Required]
        public string ProjectName { get; set; }

        [ForeignKey("Company")]
        public int CompanyID { get; set; } = 0;
        public bool? isactive { get; set; } = true;
        public bool? isdeleted { get; set; } = false;
        public int? createdby { get; set; } = 0;
        public DateTime? createdon { get; set; }
        public int? updatedby { get; set; } = 0;
        public DateTime? updatedon { get; set; }
        public Company? Company { get; set; } = null;
    }
}
