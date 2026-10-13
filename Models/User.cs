
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.VisualBasic;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using Township_API.Models;


namespace Township_API.Models
{
    public class LoginModel
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }

    //User
    [Table("tblUser")]
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string uid { get; set; }

        [Required, MaxLength(250)]
        public string name { get; set; }

        [Required, MaxLength(50)]
        public string password { get; set; }

        [Required, EmailAddress]
        public string email { get; set; }

        [Required]
        public string? phone { get; set; }

        [ForeignKey("Company")]
        public int? CompanyID { get; set; } = 0;
        public bool? isactive { get; set; } = false;
        public bool? isdeleted { get; set; } = false;
        public int? createdby { get; set; }
        public DateTime? createdon { get; set; } = null;
        public int? updatedby { get; set; }
        public DateTime? updatedon { get; set; } = null;

        [ForeignKey("Role")]
        public int? roleID { get; set; }
        [NotMapped]
        public Role? Role { get; set; }


        [NotMapped]
        public string? ProfileName { get; set; }

        [NotMapped]
        public Company? Company { get; set; }
    }

    // Profile
    [Table("tblProfile")]
    public class Profile
    {
        // [ID],[profilename]     ,[uid]      ,[isactive]      ,[isdeleted]      ,[createdby]      ,[createdon]      ,[updatedby]      ,[updatedon]
        [Key]
        public int ID { get; set; }
        [Required, MaxLength(100)]
        public string? ProfileName { get; set; }
        public bool? isActive { get; set; } = false;
        public bool? isDeleted { get; set; } = false;
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; } = null;
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; } = null;
        public int? Companyid { get; set; } 
        public int? Uid { get; set; } = 0; 
        public int? RoleId { get; set; } = 0;  
    }

    //ProfileDetails tblProfileDetails
    [Table("tblProfileDetails")]
    public class ProfileDetails
    {
        [Key]
        public int id { get; set; }

        [ForeignKey("Profile")]
        public int? profileid { get; set; }
        public Profile? profile { get; set; }

        [ForeignKey("Module")]
        public int? moduleId { get; set; }
        public _module? module { get; set; }
        public bool? CanInsert { get; set; } = false;
        public bool? CanUpdate { get; set; } = false;
        public bool? CanDelete { get; set; } = false;
        public bool? CanView { get; set; } = true;
        public bool? isactive { get; set; } = false;
        public bool? isdeleted { get; set; } = false;
        public int? createdby { get; set; }
        public DateTime? createdon { get; set; } = null;
        public int? updatedby { get; set; }
        public DateTime? updatedon { get; set; } = null;
    }

    [Table("tblModules")]
    public class _module
    {
        [Key]
        public int ModuleID { get; set; }

        [Required, MaxLength(100)]
        public string? ModuleName { get; set; }
        public bool? Viewreadonly { get; set; } = false;

    }

    [Table("tblRole")]
    public class Role
    {
        [Key]
        public int RoleID { get; set; }

        [Required, MaxLength(100)]
        public string? RoleName { get; set; }
    }


    [Table("tblModuleData")]
    public class ModuleData
    {
        [Key]
        public int ID { get; set; }
        [Required, MaxLength(100)]
        public string? Name { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public int? TypeID { get; set; } = 0;

        public int ParentID { get; set; } = 0;
        public string? ModuleType { get; set; } = null;
        public string? Discriminator { get; set; } = null;

        public bool? isactive { get; set; } = false;
        public int? createdby { get; set; } = 0;
        public DateTime? createdon { get; set; } = null;
        public int? updatedby { get; set; } = 0;
        public DateTime? updatedon { get; set; } = null;

        [NotMapped]
        public string? ParentName { get; set; } = null;

    }
    public abstract class ModuleDataTemplate : ModuleData
    {
        [Key]
        public int ID { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; }
        [Required, MaxLength(50)]
        public string Code { get; set; }
        [Required]
        public int ParentID { get; set; } = 0;

        public int status { get; set; }
        public int isDeleted { get; set; }

        //  public int? TypeID { get; set; } = null;

    }
    public class NRD : ModuleData
    {
        //public List<Building> Buildings { get; set; }
    }
    public class Building : ModuleData
    {
    }
    public class ReaderType : ModuleData { }
    public class Amenities : ModuleData { }
    public class Phase : ModuleData { }
    public class ReaderLocation : ModuleData { }
    public class VehicleMake : ModuleData { }
    public class ReaderRelay : ModuleData { }
    public class ServiceType : ModuleData { }
    public class ContractorType : ModuleData { }
    public class VehicleType : ModuleData { }
    public class ReaderMode : ModuleData { }
    public class ReaderLocations : ModuleData { }

    [Table("tblEmployee")]
    public class Employee
    {
        [Key]
        public int ID { get; set; }

        [MaxLength(50)]
        public string? IDNumber { get; set; }
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = "";
        [MaxLength(50)]

        [DisplayName("MiddleName")]
        public string? MiddleName { get; set; } = "";
        [Required]
        [MaxLength(50)]
        public string? LastName { get; set; } = "";
        [MaxLength(10)]
        public string? Gender { get; set; } = "";
        [MaxLength(25)]
        public string? CardCSN { get; set; } = "";
        public string? MobileNo { get; set; }
        public string? ICEno { get; set; }
        [EmailAddress]
        public string? EmailID { get; set; }
        public DateTime? Dob { get; set; } = null;
        public DateTime? Doj { get; set; } = null;
        public int? isResident { get; set; } = 0;
        public int? ResidentID { get; set; } = 0;
        public int? SiteID { get; set; }
        public string? Role { get; set; }
        public int? isactive { get; set; } = 0;
        public int? isdeleted { get; set; } = 0;
        public int? createdby { get; set; } = 0;
        public DateTime? createdon { get; set; } = null;
        public int? updatedby { get; set; } = 0;
        public DateTime? updatedon { get; set; } = null;
        public string? BloodGroup { get; set; } = null;
        [NotMapped]
        public string? sitename { get; set; } = null;

        [NotMapped]
        public string? EmployeeCompany { get; set; } = null;

        public int? EmployeeCompanyID { get; set; }
    }

    //[Table("tblServiceProvider")]
    //public class Service_Provider
    //{
    //    public int ID { get; set; }
    //    [MaxLength(50)]
    //    public string? code { get; set; }
    //    [Required]
    //    [MaxLength(100)]
    //    public string? name { get; set; }
    //    public string? email { get; set; }
    //    public string? phone { get; set; }
    //    public string? ServiceProviderID { get; set; }
    //    public string? role { get; set; }
    //    public bool? isactive { get; set; } = false
    //    public bool? isdelete { get; set; } = false;
    //    public int? createdby { get; set; } = null;
    //    public DateTime? createdon { get; set; } = null;
    //    public int? updatedby { get; set; } = null;
    //    public DateTime? updatedon { get; set; } = null     
    //}

    [Table("tblServiceProvider")]
    public class Service_Provider
    {
        public int ID { get; set; }

        public string IDNumber { get; set; }

        public string FirstName { get; set; }

        public string? MiddleName { get; set; }

        public string LastName { get; set; }

        public string? ShortName { get; set; }

        public int ServiceTypeId { get; set; }

        public string? ICEno { get; set; }

        public string? AadharCardId { get; set; }

        public string? VoterID { get; set; }

        public string? EmailID { get; set; }

        public string? Gender { get; set; }

        public string? BloodGroup { get; set; }

        public string? MobileNo { get; set; }

        public string? CSN { get; set; }

        public DateTime? Doj { get; set; }

        public DateTime? CardIssueDate { get; set; }

        public DateTime? CardPrintingDate { get; set; }

        public DateTime? ValidFromDate { get; set; }

        public DateTime? ValidToDate { get; set; }

        public string? Address { get; set; }

        public string? Refrence1ID { get; set; }

        public string? Refrence1Name { get; set; }

        public string? Refrence1Mobile { get; set; }

        public string? Refrence2Name { get; set; }

        public string? Refrence2Mobile { get; set; }

        public string? Refrence2Details { get; set; }

        public int? isActive { get; set; } = 0;

        public int? isDeleted { get; set; } = 0;

        public DateTime? CreatedOn { get; set; }

        public int CreatedBy { get; set; } = 0;

        public DateTime? UpdatedOn { get; set; }

        public int? UpdatedBy { get; set; } = 0;
        [NotMapped]
        public string? serviceType { get; set; }
         
        public string? FlatNumber { get; set; }  

    }

    public class Service_Provider_view
    {
        public int ID { get; set; }

        public string IDNumber { get; set; }

        public string FirstName { get; set; }

        public string? MiddleName { get; set; }

        public string LastName { get; set; }

        public string? ShortName { get; set; }

        public int ServiceTypeId { get; set; }

        public string? ICEno { get; set; }

        public string? AadharCardId { get; set; }

        public string? VoterID { get; set; }

        public string? EmailID { get; set; }

        public string? Gender { get; set; }

        public string? BloodGroup { get; set; }

        public string? MobileNo { get; set; }

        public string? CSN { get; set; }

        public DateTime? Doj { get; set; }

        public DateTime? CardIssueDate { get; set; }

        public DateTime? CardPrintingDate { get; set; }

        public DateTime? ValidFromDate { get; set; }

        public DateTime? ValidToDate { get; set; }

        public string? Address { get; set; }

        public string? Refrence1ID { get; set; }

        public string? Refrence1Name { get; set; }

        public string? Refrence1Mobile { get; set; }

        public string? Refrence2Name { get; set; }

        public string? Refrence2Mobile { get; set; }

        public string? Refrence2Details { get; set; }

        public int? isActive { get; set; } = 0;

        public int? isDeleted { get; set; } = 0;

        public DateTime? CreatedOn { get; set; }

        public int CreatedBy { get; set; } = 0;

        public DateTime? UpdatedOn { get; set; }

        public int? UpdatedBy { get; set; } = 0;
        [NotMapped]
        public string? serviceType { get; set; }

        [NotMapped]
        public string? Neighbourhood { get; set; } = null;

        [NotMapped]
        public string? Buildings { get; set; } = null;

    }

    [Table("tblVehicle")]
    public class Vehicle
    {

        [Key]
        public int ID { get; set; }

        [Required, MaxLength(50)]
        public string? RegNo { get; set; }
        public string? vType { get; set; }
        public string? vMake { get; set; }
        public string? vColor { get; set; }

        public string? TagUID { get; set; }
        public string? PrintedTagID { get; set; }
        public DateTime? TagEncodingDate { get; set; }
        public int? Logical_Delete { get; set; }
        public string? StickerNo { get; set; }
        public int? isactive { get; set; } = 0;
        public int? createdby { get; set; } = null;
        public DateTime? createdon { get; set; } = null;
        public int? updatedby { get; set; } = null;
        public DateTime? updatedon { get; set; } = null;

        public string? IsUploadedToANPR { get; set; } = "N";

        /*
        [NotMapped]
        public int? OwnerID { get; set; }
        [NotMapped]
        public string? IDNumber { get; set; }
        [NotMapped]
        public string? shortname { get; set; }
        [NotMapped]
        public string? nrd { get; set; }
        [NotMapped]
        public string? Building { get; set; }
        [NotMapped]
        public string? FlatNumber { get; set; }
        */
    }

    public class vwVehicle
    {

        [Key]
        public int ID { get; set; }

        [Required, MaxLength(50)]
        public string? RegNo { get; set; }
        public string? vType { get; set; }
        public string? vMake { get; set; }
        public string? vColor { get; set; }

        public string? TagUID { get; set; }
        public string? PrintedTagID { get; set; }
        public DateTime? TagEncodingDate { get; set; }
        public int? Logical_Delete { get; set; }
        public string? StickerNo { get; set; }
        public int? isactive { get; set; } = 0;
        public int? createdby { get; set; } = null;
        public DateTime? createdon { get; set; } = null;
        public int? updatedby { get; set; } = null;
        public DateTime? updatedon { get; set; } = null;

        //public int? OwnerID { get; set; }
        public string? IDNumber { get; set; }
        public string? shortname { get; set; }
        public string? nrd { get; set; }
        public string? Building { get; set; }
        public string? FlatNumber { get; set; }

    }

    [Table("tblDoorAccess")]
    public class DoorAccess
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string moduleID { get; set; }
        [Required]
        public string CardHolderID { get; set; }
        public DateTime validTillDate { get; set; }
        public bool? sun { get; set; }
        public bool? mon { get; set; }
        public bool? tue { get; set; }
        public bool? wed { get; set; }
        public bool? thu { get; set; }
        public bool? fri { get; set; }
        public bool? sat { get; set; }
        public bool? isactive { get; set; } = true;
        public bool? isdeleted { get; set; } = false;
        public int? createdby { get; set; }
        public DateTime? createdon { get; set; }
        public int? updatedby { get; set; }
        public DateTime? updatedon { get; set; }
    }
    public class AllCardHolder
    {
        public string? CardType { get; set; }
        public int id { get; set; } = 0;
        public int pid { get; set; }= 0;
        public string? IDNumber { get; set; }
        public string? shortname { get; set; }
        public string? CSN { get; set; }
        public string? Building { get; set; }
        public string? FlatNumber { get; set; }
        public DateTime? CardPrintingDate { get; set; }=null;
        public DateTime? CardIssueDate { get; set; } = null;
        public string? Bld { get; set; }
        public string? NRD { get; set; }
        public string? BloodGroup { get; set; } = null;
        public DateTime? validTill { get; set; } = null;

        public string? templateName { get; set; } = null;
        public string? custom1 { get; set; } = null;
        public string? custom2 { get; set; } = null;

    }
    public class UserALLAccess
    {
        public int? id { get; set; }
        public string? ModuleName { get; set; }
        public string? moduleID { get; set; }
        public string? CardHolderID { get; set; }
        public DateTime? validTillDate { get; set; }
        public string? sun { get; set; }
        public string? mon { get; set; }
        public string? tue { get; set; }
        public string? wed { get; set; }
        public string? thu { get; set; }
        public string? fri { get; set; }
        public string? sat { get; set; }
        public string? isactive { get; set; }
        public string? isdeleted { get; set; }
        public string? ModuleType { get; set; }
    }

    public class UserNRDAccess
    {
        public string? ModuleName { get; set; }
        public int? moduleID { get; set; }
        public string? CardHolderID { get; set; }
        public DateTime? validTillDate { get; set; }
        public string? sun { get; set; }
        public string? mon { get; set; }
        public string? tue { get; set; }
        public string? wed { get; set; }
        public string? thu { get; set; }
        public string? fri { get; set; }
        public string? sat { get; set; }

        public string? isactive { get; set; }
        public string? isdeleted { get; set; }
    }
    public class UserAmenitiesAccess
    {
        public string? ModuleName { get; set; }
        public int? moduleID { get; set; }
        public string? CardHolderID { get; set; }
        public DateTime? validTillDate { get; set; }
        public string? sun { get; set; }
        public string? mon { get; set; }
        public string? tue { get; set; }
        public string? wed { get; set; }
        public string? thu { get; set; }
        public string? fri { get; set; }
        public string? sat { get; set; }
        public string? isactive { get; set; }
        public string? isdeleted { get; set; }
    }
    public class UserBuildingAccess
    {
        public string? ModuleName { get; set; }
        public int? moduleID { get; set; }
        public string? CardHolderID { get; set; }
        public DateTime? validTillDate { get; set; }
        public string? sun { get; set; }
        public string? mon { get; set; }
        public string? tue { get; set; }
        public string? wed { get; set; }
        public string? thu { get; set; }
        public string? fri { get; set; }
        public string? sat { get; set; }
        public string? isactive { get; set; }
        public string? isdeleted { get; set; }
    }
    public class VehicleAccessInout
    {
        public int Cameraid { get; set; }
        public string CameraLocation { get; set; }
        public string VehicleNumber { get; set; }
        public string InoutFlag { get; set; }
        public DateTime InoutTime { get; set; }
        public string TagUID { get; set; }
        public string IDNumber { get; set; }
        public string OwnerName { get; set; }
        public string Building { get; set; }
        public string CardType { get; set; }
    }

    public class NRDFlatDetails
    {
        public string NRD { get; set; }
        public string BuildingName { get; set; }
        public string Building { get; set; }
        public string FlatNumber { get; set; }

    }

    public class BuildingRequestDto
    {
        public List<string> BuildingFlats { get; set; }
    }
    public class CardRefBuildings
    {
        public int CardId { get; set; }
        public string BuildingFlats { get; set; } // stored as CSV
    }
    public class _Building
    {
        public int Id { get; set; }
        public string BuildingFlats { get; set; } // stored as CSV
    }

    
}