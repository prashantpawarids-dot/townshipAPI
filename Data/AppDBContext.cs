
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data;
using System.Reflection;
using Township_API.Models;
using Microsoft.AspNetCore.Mvc;
using EventFlow.Entities;
using EventFlow.Core;

namespace Township_API.Data
{

    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }
        public DbSet<User> UserRegisters { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<_module> Modules { get; set; }
        //  public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<ProfileDetails> ProfileDetails { get; set; }
        public DbSet<profileRegister> profileRegisters { get; set; }
        public DbSet<CardMasterTemplate> CardMasterTemplates { get; set; }
        public DbSet<ModuleData> ModuleData { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<VehicleNRDAccess> VehicleNRDAccess { get; set; }
        public DbSet<vwVehicle> vwVehicles { get; set; }
        public DbSet<PrimaryLandowner> Landowners { get; set; }
        public DbSet<DependentLandOwner> DependentLandowners { get; set; }
        public DbSet<PrimaryTenent> PrimaryTenents { get; set; }
        public DbSet<DependentTenent> DependentTenents { get; set; }

        public DbSet<PrimaryResident> PrimaryResidents { get; set; }
        public DbSet<DependentResident> DependentResidents { get; set; }
        public DbSet<Contractor> Contractors { get; set; }
        public DbSet<DependentContractor> DependentContractors { get; set; }

        public DbSet<Service_Provider> ServiceProviders { get; set; }
        public DbSet<Employee> Employees { get; set; }

        public DbSet<ModuleData> ModuleDatas { get; set; }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Reader> Readers { get; set; }
        public DbSet<GuestMaster> GuestMasters { get; set; }
        public DbSet<VisitorMaster> VisitorMasters { get; set; }
        public DbSet<NRDTemplate> NRDTemplates { get; set; }
        public DbSet<ANPR> ANPRs { get; set; }
        public DbSet<ANPRCamera> ANPRControllers { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<CardPrintSetting> CardPrintSettings { get; set; }
        public IQueryable<NRD> NRDs => (IQueryable<NRD>)ModuleDatas.Where(n => n.TypeID == (int)commonTypes.ModuleTypes.NRD && n.ID > (int)commonTypes.ModuleTypes.NRD);
        public IQueryable<Building> Buildings => (IQueryable<Building>)ModuleDatas.Where(n => n.TypeID == (int)commonTypes.ModuleTypes.Building && n.ID > (int)commonTypes.ModuleTypes.Building);
        public IQueryable<VehicleType> VehicleTypes => (IQueryable<VehicleType>)ModuleDatas.Where(n => n.TypeID == (int)commonTypes.ModuleTypes.VehicleType && n.ID > (int)commonTypes.ModuleTypes.VehicleType);
        public IQueryable<ReaderType> ReaderTypes => (IQueryable<ReaderType>)ModuleDatas.Where(n => n.TypeID == (int)commonTypes.ModuleTypes.ReaderType && n.ID > (int)commonTypes.ModuleTypes.ReaderType);
        public IQueryable<ReaderMode> ReaderModes => (IQueryable<ReaderMode>)ModuleDatas.Where(n => n.TypeID == (int)commonTypes.ModuleTypes.ReaderMode && n.ID > (int)commonTypes.ModuleTypes.ReaderMode);
        public IQueryable<ContractorType> ContractorTypes => (IQueryable<ContractorType>)ModuleDatas.Where(n => n.TypeID == (int)commonTypes.ModuleTypes.ContractorType && n.ID > (int)commonTypes.ModuleTypes.ContractorType);
        public IQueryable<Amenities> Amenities_ => (IQueryable<Amenities>)ModuleDatas.Where(n => n.TypeID == (int)commonTypes.ModuleTypes.Amenities && n.ID > (int)commonTypes.ModuleTypes.Amenities);
        public IQueryable<Phase> Phases => (IQueryable<Phase>)ModuleDatas.Where(n => n.TypeID == (int)commonTypes.ModuleTypes.Phases && n.ID > (int)commonTypes.ModuleTypes.Phases);
        public IQueryable<ReaderLocation> ReaderLocations => (IQueryable<ReaderLocation>)ModuleDatas.Where(n => n.TypeID == (int)commonTypes.ModuleTypes.ReaderLocations && n.ID > (int)commonTypes.ModuleTypes.ReaderLocations);
        public IQueryable<ServiceType> ServiceTypes => (IQueryable<ServiceType>)ModuleDatas.Where(n => n.TypeID == (int)commonTypes.ModuleTypes.ServiceType && n.ID > (int)commonTypes.ModuleTypes.ServiceType);
        public IQueryable<ReaderRelay> ReaderRelays => (IQueryable<ReaderRelay>)ModuleDatas.Where(n => n.TypeID == (int)commonTypes.ModuleTypes.ReaderRelays && n.ID > (int)commonTypes.ModuleTypes.ReaderRelays);
        public IQueryable<VehicleMake> VehicleMakes => (IQueryable<VehicleMake>)ModuleDatas.Where(n => n.TypeID == (int)commonTypes.ModuleTypes.VehicleMake && n.ID > (int)commonTypes.ModuleTypes.VehicleMake);
        public DbSet<DoorAccess> _userDoorAccess { get; set; }
        public DbSet<AllCardHolder> AllCardHolders { get; set; }
        public DbSet<UserALLAccess> _userAllAccess { get; set; }

        public DbSet<UserAmenitiesAccess> _userAmenitiesAccess { get; set; }
        public DbSet<UserNRDAccess> _userNRDAccess { get; set; }

        public DbSet<UserBuildingAccess> _userBuildingAccess { get; set; }

        public DbSet<CurrentResidents> _CurrentResidents { get; set; }

        public DbSet<ServiceProviderOwners> ServiceProviderOwners { get; set; }

        public DbSet<AccessBlockRevoke_Register> AccessBlockRevokeRegister { get; set; }
        public DbSet<CardLostDamage_Register> CardLostDamage_Register { get; set; }

        public DbSet<CardAccessInout> CardAccessInout { get; set; }

        public DbSet<CardSummaryReport> CardSummaryReports { get; set; }
        public DbSet<CardAccessInout_Register> CardAccessInout_Register { get; set; }
        public DbSet<VehicleAccessInout_Log> VehicleAccessInout_logs { get; set; }

        public DbSet<VehicleAccessInout_Register> VehicleAccessInout_Registers { get; set; }

        public DbSet<VehicleUnathorisedAccessInout_Register> VehicleUnathorisedAccessInout_Register { get; set; }

        public DbSet<AccessBlockRevokeRegister> AccessBlockRevoke_Register_Report { get; set; }

        public DbSet<Dashboard> DashBords { get; set; }

        public DbSet<ContractorAgency> Contractor_Agency { get; set; }
        public DbSet<ContractorCompany> Contractor_Company { get; set; }



        // Reports Classes : CardEncodingLog CardPrintingLog CardLostDamage

        public DbSet<CardLostDamage> CardLostDamages { get; set; }
        public DbSet<CardPrintingLog> CardPrintingLogs { get; set; }
        public DbSet<CardEncodingLog> CardEncodingLogs { get; set; }

        public DbSet<AuthorityModule> AuthorityModules { get; set; }
        public DbSet<NRDFlatDetails> NRDFlatDetails { get; set; }


        #region cardHolderHistory
        public DbSet<IDCardData> IDCardData { get; set; }


        #endregion cardHolderHistory

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //// Configure the foreign key but don't include navigation property
            //modelBuilder.Entity<Vehicle>()
            //            .HasOne<AllCardHolder>(r => r.OwnerDtls ) // Role is not part of User model
            //            .WithMany()
            //            .HasForeignKey(r => r.TagUID)
            //            .OnDelete(DeleteBehavior.Restrict);     
            //modelBuilder.Entity<Vehicle>()
            //         .HasOne<AllCardHolder>(v => v.OwnerDtls) // Navigation property in Vehicle
            //         .WithMany()
            //         .HasForeignKey(v => v.TagUID)
            //         .OnDelete(DeleteBehavior.Restrict);  

            modelBuilder.Entity<IDCardData>(entity =>
            {
                entity.HasNoKey(); // Views often don’t have a primary key
                entity.ToView("vwIDCardData"); // Name of the view in SQL
                                                                    // Optional: configure column mapping if needed
                                                                    // entity.Property(e => e.Name).HasColumnName("SomeColumn");
            });


            modelBuilder.Entity<CardSummaryReport>().HasNoKey().ToView("vwCardSummaryReport");


            modelBuilder.Entity<vehicleAccessImageUpdateList>(entity =>
                        {
                            entity.HasNoKey(); // Views often don’t have a primary key
                            entity.ToView("vwGetVehicleAccessImageUpdateList"); // Name of the view in SQL
                            // Optional: configure column mapping if needed
                            // entity.Property(e => e.Name).HasColumnName("SomeColumn");
                        });
            modelBuilder.Entity<ContractorAgency>(entity =>
                                    {
                                        entity.HasNoKey(); // Views often don’t have a primary key
                                        entity.ToView("vwAgency"); // Name of the view in SQL
                                                                   // Optional: configure column mapping if needed
                                                                   // entity.Property(e => e.Name).HasColumnName("SomeColumn");
                                    });
            modelBuilder.Entity<ContractorCompany>(entity =>
                                    {
                                        entity.HasNoKey(); // Views often don’t have a primary key
                                        entity.ToView("vwContractorCompany"); // Name of the view in SQL
                                                                              // Optional: configure column mapping if needed
                                                                              // entity.Property(e => e.Name).HasColumnName("SomeColumn");
                                    });

            //modelBuilder.Entity<User>().Property<int?>("CompanyID"); // shadow property
            modelBuilder.Entity<User>()
                .HasOne<Company>()                // relationship target
                .WithMany()                       // no navigation back
                .HasForeignKey(u => u.CompanyID)  // foreign key field  
                .OnDelete(DeleteBehavior.Restrict);//optional: restrict delete

            modelBuilder.Entity<profileRegister>(entity =>
                        {
                            entity.HasNoKey(); // Views often don’t have a primary key
                            entity.ToView("profileRegister"); // Name of the view in SQL
                            // Optional: configure column mapping if needed
                            // entity.Property(e => e.Name).HasColumnName("SomeColumn");
                        });

            modelBuilder.Entity<NRDFlatDetails>(entity =>
                        {
                            entity.HasNoKey(); // Views often don’t have a primary key
                            entity.ToView("vwNRDFlatDetails"); // Name of the view in SQL
                            // Optional: configure column mapping if needed
                            // entity.Property(e => e.Name).HasColumnName("SomeColumn");
                        });
            modelBuilder.Entity<AuthorityModule>(entity =>
            {
                entity.HasNoKey(); // Views often don’t have a primary key
                entity.ToView("vwAuthorityModules"); // Name of the view in SQL
                // Optional: configure column mapping if needed
                // entity.Property(e => e.Name).HasColumnName("SomeColumn");
            });

            modelBuilder.Entity<CardAccessInout>(entity =>
            {
                entity.HasNoKey(); // Views often don’t have a primary key
                entity.ToView("vwCardAccessInout"); // Name of the view in SQL
                // Optional: configure column mapping if needed
                // entity.Property(e => e.Name).HasColumnName("SomeColumn");
            });


            modelBuilder.Entity<vwVehicle>(entity =>
            {
                entity.HasNoKey(); // Views often don’t have a primary key
                entity.ToView("vwVehicles"); // Name of the view in SQL
                // Optional: configure column mapping if needed
                // entity.Property(e => e.Name).HasColumnName("SomeColumn");
            });

            modelBuilder.Entity<UserNRDAccess>(entity =>
            {
                entity.HasNoKey(); // Views often don’t have a primary key
                entity.ToView("vwNRDDoorAccess"); // Name of the view in SQL
                // Optional: configure column mapping if needed
                // entity.Property(e => e.Name).HasColumnName("SomeColumn");
            });
            modelBuilder.Entity<UserBuildingAccess>(entity =>
            {
                entity.HasNoKey(); // Views often don’t have a primary key
                entity.ToView("vwBuildingDoorAccess"); // Name of the view in SQL
                                                       // Optional: configure column mapping if needed
                                                       // entity.Property(e => e.Name).HasColumnName("SomeColumn");
            });
            modelBuilder.Entity<UserAmenitiesAccess>(entity =>
            {
                entity.HasNoKey(); // Views often don’t have a primary key
                entity.ToView("vwAmenitiesDoorAccess"); // Name of the view in SQL
                                                        // Optional: configure column mapping if needed
                                                        // entity.Property(e => e.Name).HasColumnName("SomeColumn");
            });
            modelBuilder.Entity<UserALLAccess>(entity =>
            {
                entity.HasNoKey(); // Views often don’t have a primary key
                entity.ToView("vwAllDoorAccess"); // Name of the view in SQL
                                                  // Optional: configure column mapping if needed
                                                  // entity.Property(e => e.Name).HasColumnName("SomeColumn");
            });
            modelBuilder.Entity<AllCardHolder>(entity =>
            {
                entity.HasNoKey(); // Views often don’t have a primary key
                entity.ToView("vwCardHolders"); // Name of the view in SQL
                                                // Optional: configure column mapping if needed
                                                // entity.Property(e => e.Name).HasColumnName("SomeColumn");
            });

            modelBuilder.Entity<VehicleAccessInout_Log>(entity =>
            {
                entity.HasNoKey(); // Views often don’t have a primary key
                entity.ToView("vwVehicleAccessInout_Log"); // Name of the view in SQL
                                                           // Optional: configure column mapping if needed
                                                           // entity.Property(e => e.Name).HasColumnName("SomeColumn");
            });
            modelBuilder.Entity<VehicleAccessInout_Register>(entity =>
            {
                entity.HasNoKey(); // Views often don’t have a primary key
                entity.ToView("vwVehicleAccessInout_Register"); // Name of the view in SQL
                                                                // Optional: configure column mapping if needed
                                                                // entity.Property(e => e.Name).HasColumnName("SomeColumn");
            });
            modelBuilder.Entity<VehicleUnathorisedAccessInout_Register>(entity =>
            {
                entity.HasNoKey(); // Views often don’t have a primary key
                entity.ToView("vwVehicleUnathorisedAccessInout_Register"); // Name of the view in SQL
                                                                           // Optional: configure column mapping if needed
                                                                           // entity.Property(e => e.Name).HasColumnName("SomeColumn");
            });

            modelBuilder.Entity<AccessBlockRevokeRegister>(entity =>
            {
                entity.HasNoKey(); // Views often don’t have a primary key
                entity.ToView("vwAccessBlockRevoke_Register"); // Name of the view in SQL
                                                               // Optional: configure column mapping if needed
                                                               // entity.Property(e => e.Name).HasColumnName("SomeColumn");
            });
            modelBuilder.Entity<CardAccessInout_Register>(entity =>
            {
                entity.HasNoKey(); // Views often don’t have a primary key
                entity.ToView("vwCardAccessInout_Register"); // Name of the view in SQL
                                                             // Optional: configure column mapping if needed
                                                             // entity.Property(e => e.Name).HasColumnName("SomeColumn");
            });
            // vwCardAccessInout_Register
            //vwVehicleAccessInout_Register  
            //vwAccessBlockRevoke_Register

            // vwCardAccessInout_Register //vwVehicleAccessInout_Register  //vwAccessBlockRevoke_Register //vwDashBoard
            modelBuilder.Entity<Dashboard>(entity =>
            {
                entity.HasNoKey(); // Views often don’t have a primary key
                entity.ToView("vwDashBoard"); // Name of the view in SQL
                                              // Optional: configure column mapping if needed
                                              // entity.Property(e => e.Name).HasColumnName("SomeColumn");
            });

            //CardEncodingLog CardPrintingLog CardLostDamage
            modelBuilder.Entity<CardEncodingLog>(entity =>
            {
                entity.HasNoKey(); // Views often don’t have a primary key
                entity.ToView("vwCardEncodingLog"); // Name of the view in SQL
                                                    // Optional: configure column mapping if needed
                                                    // entity.Property(e => e.Name).HasColumnName("SomeColumn");
            });
            modelBuilder.Entity<CardPrintingLog>(entity =>
            {
                entity.HasNoKey(); // Views often don’t have a primary key
                entity.ToView("vwCardPrintingLog"); // Name of the view in SQL
                                                    // Optional: configure column mapping if needed
                                                    // entity.Property(e => e.Name).HasColumnName("SomeColumn");
            });
            modelBuilder.Entity<CardLostDamage>(entity =>
            {
                entity.HasNoKey(); // Views often don’t have a primary key
                entity.ToView("vwCardLostDamage"); // Name of the view in SQL
                                                   // Optional: configure column mapping if needed
                                                   // entity.Property(e => e.Name).HasColumnName("SomeColumn");
            });
        }
    }
}
