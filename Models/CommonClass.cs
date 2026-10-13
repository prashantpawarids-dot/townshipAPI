using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Microsoft.VisualBasic;

namespace Township_API.Models
{
    public class commonTypes
    {
        public class ANPR_Vehicle
        {
            //id, owner, number_plate, vehicle_type, auth_type, comment
            public long id { get; set; }
            public string owner { get; set; }
            public string number_plate { get; set; }
            public string vehicle_type { get; set; }
            public string auth_type { get; set; } = "whitelist";
            public string comment { get; set; } = null;

        }
        public class ApiSettings
        {
            public string AuthUrl { get; set; }
            public string DataUrl { get; set; }
            public string UserID { get; set; }
            public string Pwd { get; set; }
        }
        public enum ModuleTypes
        {
            NRD = 2,
            Building = 3,
            VehicleType = 4,
            ReaderType = 5,
            ReaderMode = 6,
            ContractorType = 7,
            Amenities = 8,
            Phases = 9,
            ReaderLocations = 10,
            VehicleMake = 11,
            ReaderRelays = 12,
            ServiceType = 13,
            EmployeeCompany = 14



        };
        public enum AccessCardHolders
        {
            Resident = 1,
            DependentResident = 2,
            Landowner = 3,
            DependentLandowner = 4,
            Tenent = 5,
            DependentTenent = 6,
            ServiceProvider = 7,
            contractor = 8,
            Guest = 9,
            Employee = 11,
            DependentContractor = 12,
            Visitor = 13
        };
    }
    public class profileRegister
    {
        public int? profileID { get; set; } = 0;
        public int? ModuleID { get; set; } = 0;
        public string? ModuleName { get; set; } = "";
        public string? ProfileName { get; set; } = "";
        public Boolean? CanInsert { get; set; } = false;
        public Boolean? CanUpdate { get; set; } = false;
        public Boolean? CanDelete { get; set; } = false;
        public Boolean? CanView { get; set; } = false;
        public Boolean? Viewreadonly { get; set; } = false;
        public string? User { get; set; } = "";
        public string? RoleName { get; set; } = "";
        public int? uid { get; set; } = 0;
    }
    public class DependentJsonWrapper
    {
        public object? Owners { get; set; }
        public object? DependentOwners { get; set; }
        public object? Vehicles { get; set; }
        public object? UserAllAccess { get; set; }
        public object? UserNRDAccess { get; set; }
        public object? UserBuildingAccess { get; set; }
        public object? UserAminitiesAccess { get; set; }
    }

    [Table("images")]
    public class Image
    {
        [Key]
        public int ImageId { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Data { get; set; } // BLOB
    }

    [Table("ServiceProviderOwners")]
    public class ServiceProviderOwners
    {
        //Id, SProviderID, 
        [Key]
        public int Id { get; set; }
        [Required]
        public string SProviderID { get; set; }
        [Required]
        public string Hid { get; set; }

        public string? OwnerDetails { get; set; } = "";
        public bool isActive { get; set; } = true;

        [NotMapped]
        public string? OwnerName { get; set; } = "";
        [NotMapped]
        public string? building { get; set; } = "";
        [NotMapped]
        public string? Neighbourhood { get; set; } = "";
        [NotMapped]
        public string? FlatNumber { get; set; } = "";

    }
    public class CurrentResidents
    {
        public int ID { get; set; }
        [Required]
        public int RID { get; set; }        //Primary-ResidentID
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
        public string? BloodGroup { get; set; }
        public DateTime? DOB { get; set; }
        public string? EmailID { get; set; }
        public string? MobileNo { get; set; }
        public string? LandLine { get; set; }
        [Required]
        public string NRD { get; set; }
        [Required]
        public string Building { get; set; }
        [Required]
        public string FlatNumber { get; set; }
        [Required]
        public int? TenentType { get; set; }
        public DateTime? CardIssueDate { get; set; }
        public DateTime? CardPrintingDate { get; set; }
        public DateTime? RegistrationIssueDate { get; set; }
        public DateTime? Aggreement_From { get; set; }
        public DateTime? Aggreement_To { get; set; }
        public int? LogicalDeleted { get; set; } = 0;
        [NotMapped]
        public string? BuildingName { get; set; } = "";
        [NotMapped]
        public string? NRDName { get; set; } = "";
    }

    [Table("AccessBlockRevoke_Register")]
    public class AccessBlockRevoke_Register
    {
        //IDnumber, CardCsn, Name, BlockRevokType, FromDate, todate, createdOn,createdby, updatedOn, updatedBy
        [Key]
        public int id { get; set; }
        public int IDnumber { get; set; }
        public string CardCsn { get; set; }
        public string BlockRevokType { get; set; }
        public DateTime? fromdate { get; set; }
        public DateTime? todate { get; set; }
        public DateTime? createdOn { get; set; }
        public int? createdby { get; set; } = 0;
        public DateTime updatedOn { get; set; }
        public int updatedby { get; set; } = 0;
    }

    [Table("CardLostDamage_Register")]
    public class CardLostDamage_Register
    {
        [Key]
        public int id { get; set; }
        public int IDnumber { get; set; }
        public string? CardCsn { get; set; }
        public string? description { get; set; }
        public string? LostDamageType { get; set; }
        public DateTime? reporteddate { get; set; }
        public DateTime? blockeddate { get; set; }
        public DateTime? createdOn { get; set; }
        public int? createdby { get; set; } = 0;
        public DateTime updatedOn { get; set; }
        public int? updatedby { get; set; } = 0;


    }

    //[Table("vwCardAccessInout_Register")]
    public class CardAccessInout_Register
    {
        [Key]
        public int? ReaderID { get; set; }
        public string? ReaderLocation { get; set; }
        public string? CSNNumber { get; set; }
        public string? Accessmode { get; set; }

        public DateTime? InoutTime { get; set; }
        public string? IDNumber { get; set; }
        public string? OwnerName { get; set; }
        public string? OwnerBuilding { get; set; }
        public string? OwnerFlatNumber { get; set; }
        public string? CardType { get; set; }

        [NotMapped]
        public string? buildingName { get; set; }

        [NotMapped]
        public string? nrdName { get; set; }
    }

    //[Table("vwCardAccessInout")]
    public class CardAccessInout
    {
        //IDNumber OwnerName   Nrd OwnerBuilding   OwnerFlatNumber CardType    IN_ReaderID IN_ReaderLocation   IN_AccessTime IN_Accessmode   OUT_ReaderID OUT_ReaderLocation  OUT_AccessTime OUT_Accessmode
        public string IDNumber { get; set; } = "";
        public string OwnerName { get; set; } = "";
        public string Nrd { get; set; } = "";
        public string OwnerBuilding { get; set; } = "";
        public string OwnerFlatNumber { get; set; } = "";
        public string CardType { get; set; } = "";
        public int IN_ReaderID { get; set; } = 0;
        public string IN_ReaderLocation { get; set; } = "";
        public DateTime? IN_AccessTime { get; set; } = null;
        public string IN_Accessmode { get; set; } = "";
        public int? OUT_ReaderID { get; set; } = 0;
        public string? OUT_ReaderLocation { get; set; } = "";
        public DateTime? OUT_AccessTime { get; set; } = null;
        public string? OUT_Accessmode { get; set; } = "";
    }


    //  [Table("vwVehicleAccessInout_Register")]

    // Cameraid,  CameraLocation, CameraImage, Vehicl
    // ber, InoutFlag, InoutTime,TagUID, IDNumber, OwnerName, Building,CardType,FlatNumber
    //Cameraid	CameraLocation	CameraImage	VehicleNumber	InoutFlag	InoutTime	TagUID	IDNumber	OwnerName	NRD	Building	CardType	FlatNumber	vType

    public class VehicleAccessInout_Log
    {
        //CardAccessInout_Register
        [Key]
        public int? Cameraid { get; set; } = null;
        public string? CameraLocation { get; set; } = null;
        public byte[]? CameraImage { get; set; } = null;
        public string? VehicleNumber { get; set; } = null;
        public string? vType { get; set; } = null;
        public string? InoutFlag { get; set; } = "In";
        public DateTime? InoutTime { get; set; } = null;
        public string? TagUID { get; set; } = null;
        public string? IDNumber { get; set; } = null;
        public string? OwnerName { get; set; } = null;
        public string? NRD { get; set; } = null;
        public string? Building { get; set; } = null;
        public string? CardType { get; set; } = null;
        public string? FlatNumber { get; set; } = null;
        public string? vMake { get; set; } = null;
        public string? StickerNo { get; set; } = null;
        public string? vColor { get; set; } = null;
        public byte[]? CameraFrameImage { get; set; } = null;

    }
    public class VehicleAccessInout_Register
    {

        public string? VehicleNumber { get; set; } = null; 
        public string? TagUID { get; set; } = null;
        public string? IDNumber { get; set; } = null;
        public string? OwnerName { get; set; } = null;

        public string? NRD { get; set; } = null;
        public string? Building { get; set; } = null;
        public string? CardType { get; set; } = null;
        public string? FlatNumber { get; set; } = null;
        //public int? InCameraid { get; set; } = null;
        //public string? InCameraLocation { get; set; } = null;
        //public byte[]? InCameraImage { get; set; } = null;
        //public int? OutCameraid { get; set; } = null;
        //public string? OutCameraLocation { get; set; } = null;
        //public byte[]? OutCameraImage { get; set; } = null;

        //public DateTime? InTime { get; set; } = null;
        //public DateTime? OutTime { get; set; } = null;
        //public byte[]? InCameraFrameImage { get; set; } = null;
        //public byte[]? OutCameraFrameImage { get; set; } = null;

        public int? InCameraid { get; set; } = null;
        public string? InCameraLocation { get; set; } = null;
        public string? InUniqueId { get; set; } = null;
        public int? OutCameraid { get; set; } = null;
        public string? OutCameraLocation { get; set; } = null;
        public string? OutUniqueId { get; set; } = null;

        public DateTime? InTime { get; set; } = null;
        public DateTime? OutTime { get; set; } = null;

        public string? vType { get; set; } = null;

        //trDate VehicleNumber   TagUID IDNumber    OwnerName NRD Building CardType   
        //FlatNumber InCameraid  InCameraLocation            
        //InCameraImage   OutCameraid OutCameraLocation   OutCameraImage InTime  OutTime InCameraFrameImage  OutCameraFrameImage


    }

    [Table("tblANPR_Controller")]
    public class ANPRCamera
    {
        [Key]
        public int ID { get; set; }
        public string ANPRTerminalID { get; set; }
        public string ANPRIP { get; set; }
        public string ANPRport { get; set; }
        public string ANPRLocation { get; set; }
        public string ANPRMode { get; set; }   // IN / OUT
        public bool isActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime? last_downloadedOn { get; set; }
        public DateTime? last_updatedOn { get; set; }

        [NotMapped]
        public string Status { get; set; }   // "Connected" / "Disconnected" - live, DB mein save nahi hota
    }
    //public class VehicleUnathorisedAccessInout_Register
    //{
    //    public string? VehicleNumber { get; set; } = null;

    //    public string? TagUID { get; set; } = null;
    //    public string? IDNumber { get; set; } = null;
    //    public string? OwnerName { get; set; } = null;
    //    public string? Building { get; set; } = null;
    //    public string? CardType { get; set; } = null;
    //    public string? FlatNumber { get; set; } = null;
    //    public int? InCameraid { get; set; } = null;
    //    public string? InCameraLocation { get; set; } = null;
    //    public byte[]? InCameraImage { get; set; } = null;
    //    public int? OutCameraid { get; set; } = null;
    //    public string? OutCameraLocation { get; set; } = null;
    //    public byte[]? OutCameraImage { get; set; } = null;

    //    public DateTime? InTime { get; set; } = null;
    //    public DateTime? OutTime { get; set; } = null;

    //    public byte[]? InCameraFrameImage { get; set; } = null;
    //    public byte[]? OutCameraFrameImage { get; set; } = null; 
    //}



    public class VehicleUnathorisedAccessInout_Register
    {
        public string? VehicleNumber { get; set; } = null;
        public string? TagUID { get; set; } = null;
        public string? IDNumber { get; set; } = null;
        public string? OwnerName { get; set; } = null;
        public string? Building { get; set; } = null;
        public string? CardType { get; set; } = null;
        public string? FlatNumber { get; set; } = null;
        public int? InCameraid { get; set; } = null;
        public string? InCameraLocation { get; set; } = null;
        public int? OutCameraid { get; set; } = null;
        public string? OutCameraLocation { get; set; } = null;
        public DateTime? InTime { get; set; } = null;
        public DateTime? OutTime { get; set; } = null;
        public string? InUniqueId { get; set; } = null;    // NEW — image fetch ke liye pointer
        public string? OutUniqueId { get; set; } = null;   // NEW — image fetch ke liye pointer

        public string? VehicleTypeLabel { get; set; } = null;
    }

    //[Table("vwAccessBlockRevokeRegister")]
    public class AccessBlockRevokeRegister
    {
        //IDNumber	CSN	Name	Building	FlatNumber	BlockDate	RevokeDate	DurationMinutes 
        public string IDnumber { get; set; }
        public string? CSN { get; set; }
        public string? Name { get; set; }
        public string? NRD { get; set; }
        public string? Building { get; set; }
        public string? FlatNumber { get; set; }
        public DateTime? BlockDate { get; set; }
        public DateTime? RevokeDate { get; set; }
        public int? DurationMinutes { get; set; }
    }



    public class CardSummaryReport
    {
        public string? IDNumber { get; set; }
        public string? Name { get; set; }
        public string? CardType { get; set; }
        public string? NRD { get; set; }
        public string? Building { get; set; }
        public string? FlatNumber { get; set; }
        public string? CardCsn { get; set; }
        public string? Status { get; set; }
    }
    public class Dashboard
    {

        public string? GroupID { get; set; }
        //vwDashBoard
        public string? KeyName { get; set; }
        public string? DisplayData { get; set; }
        public int? Total { get; set; } = 0;
    }
    public class CardEncodingLog()
    {
        public string? IdNumber { get; set; } = null;
        public string? shortname { get; set; } = null;
        public string? CSN { get; set; } = null;
        public string? CardType { get; set; } = null;
        public DateTime? CardIssueDate { get; set; } = null;
        public DateTime? validTill { get; set; } = null;
        public string? Status { get; set; } = null;
    }
    public class CardPrintingLog()
    {
        public string? IdNumber { get; set; } = null;
        public string? shortname { get; set; } = null;
        public string? CSN { get; set; } = null;
        public string? CardType { get; set; } = null;
        public DateTime? CardPrintingDate { get; set; } = null;
        public string? Building { get; set; } = null;
        public string? FlatNumber { get; set; } = null;
        public string? Status { get; set; } = null;
    }
    public class CardLostDamage()
    {
        //[IdNumber]      ,[shortname]      ,[CSN]      ,[CardType]      ,[CardPrintingDate]      ,[CardIssueDate]      ,[validTill]
        //,[NRD]      ,[building]      ,[FlatNumber]      ,[LostDamageType]      ,[reporteddate]      ,[description]
        public string? IdNumber { get; set; } = null;
        public string? shortname { get; set; } = null;
        public string? CSN { get; set; } = null;
        public string? CardType { get; set; } = null;
        public DateTime? CardPrintingDate { get; set; } = null;
        public DateTime? validTill { get; set; } = null;
        public string? NRD { get; set; } = null;
        public string? Building { get; set; } = null;
        public string? FlatNumber { get; set; } = null;
        public string? LostDamageType { get; set; } = null;
        public DateTime? reporteddate { get; set; } = null;
        public string? description { get; set; } = null;

    }

    public class AuthorityModule
    {
        public int? profileid { get; set; } = 0;
        public string? profilename { get; set; } = "";
        public string ModuleName { get; set; } = "";
        public int? moduleId { get; set; } = 0;
        public int? CompanyID { get; set; } = 0;
        public string? companyName { get; set; } = "";
        public string? UserName { get; set; } = "";
        public string? UID { get; set; } = "";
        public int? Authority { get; set; } = 0;
        public Boolean? canDelete { get; set; } = false;
        public Boolean? CanUpdate { get; set; } = false;
        public Boolean? CanInsert { get; set; } = false;
        public Boolean? canview { get; set; } = false;
        public Boolean? Viewreadonly { get; set; } = false;
    }

    [Table("tblNRDTemplate")]
    public class NRDTemplate
    {
        public int Id { get; set; }
        public int NRDId { get; set; }
        public string? NRDName { get; set; }
        public string? FilePath { get; set; }
        public string? ContentType { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string? ImageKitUrl { get; set; }
        public string? RecordStatus { get; set; }
    }


    [Table("tblCardPrintSettings")]
    public class CardPrintSetting
    {
        public int Id { get; set; }
        public string HolderType { get; set; } = "";
        public string SettingsJson { get; set; } = "";
        public DateTime UpdatedOn { get; set; }
    }

    [Table("tblCardMasterTemplate")]
    public class CardMasterTemplate
    {
        public int Id { get; set; }
        public string? HolderType { get; set; }
        public string? FilePath { get; set; }
        public string? ContentType { get; set; }
        public string? LogoPath { get; set; }
        public string? LogoContentType { get; set; }
        public DateTime UpdatedOn { get; set; }
    }

    public class vehicleAccessImageUpdateList
    {
        public string? unique_id { get; set; }
        public int? Cameraid { get; set; }
        public string? CameraLocation { get; set; }
        public string? shortname { get; set; }
        public string? VehicleNumber { get; set; }
        public string? InoutFlag { get; set; }
        public string? InoutTime { get; set; }
    }

}
