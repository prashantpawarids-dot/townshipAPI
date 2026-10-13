using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Township_API.Data; 
using Township_API.Models;

namespace Township_API.Services
{
    public class UserService 
    {
        private readonly AppDBContext _context;

        public UserService(AppDBContext context)
        {
            _context = context;
        } 
        public void AddUser(User user)
        {
             _context.UserRegisters.Add(user);
            _context.SaveChanges();
        }

        public   List<User> GetAllUsers()
        {
            return   _context.UserRegisters.ToList();
        }


        // Find by Id
        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.UserRegisters.FindAsync(id);
        }

        // Insert
        public async Task<User> CreateAsync(User user)
        {
            _context.UserRegisters.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        // Update
        public async Task<User?> UpdateAsync(int id, User updatedUser)
        {
            var existingUser = await _context.UserRegisters.FindAsync(id);
            if (existingUser == null) return null;

            // Update properties
            existingUser.Id = updatedUser.Id;
            existingUser.name = updatedUser.name;
            existingUser.email = updatedUser.email;
            existingUser.phone = updatedUser.phone;
            existingUser.password = updatedUser.password;
            existingUser.Role = updatedUser.Role;

            await _context.SaveChangesAsync();
            return existingUser;
        }

        // Delete
        public async Task<bool> DeleteAsync(int id)
        {
            var user  = await _context.UserRegisters.FindAsync(id);
             if (user == null) return false;
            _context.UserRegisters.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        // Get all
        public async Task<List<User>> GetAllAsync()
        {
            return await _context.UserRegisters.ToListAsync();
        }

    }

    public class iService
    { 
        private readonly AppDBContext _context;
        public iService(AppDBContext context)
        {
            _context = context;
        }

        #region NRDs
        public void AddNRD(NRD nrd)
        {
            _context.ModuleData.Add(nrd);
            _context.SaveChanges();
        } 

        public async Task<IActionResult> GetAll()
        { 
            return  (IActionResult) _context.NRDs.ToList();
        }
         
        // Find by Id
        public async Task<ModuleData?> GetByIdAsync(int id)
        { 
            return await _context.ModuleData.FindAsync(id);
        }

        // Insert
        public async Task<NRD> CreateAsync(NRD nrd)
        {
            await _context.SaveChangesAsync();
            return nrd;
        }
        // Update
        public async Task<ModuleData?> UpdateAsync(int id, NRD updatedNRD)
        {
             
            var existingNRD = await _context.ModuleData.FindAsync(id);
            if (existingNRD == null) return null;

            // Update properties
            existingNRD.ID = updatedNRD.ID;
            existingNRD.Code = updatedNRD.Code;
            existingNRD.Name = updatedNRD.Name;
            existingNRD.ParentID = updatedNRD.ParentID;
            await _context.SaveChangesAsync();
            return existingNRD;
        }
          // Delete
        public async Task<bool> DeleteAsync(int id)
        {
            var NRD = await _context.ModuleData.FindAsync(id);
            if (NRD == null) return false;
            _context.ModuleData.Remove(NRD);
            await _context.SaveChangesAsync();
            return true;
        }

        // Get all
        public async Task<List<NRD>> GetAllNRDs()
        {
            return await _context.NRDs.ToListAsync();
        }


        public async Task<IActionResult> BulkSave([FromBody] List<NRD> NRD)
        {
            //if (NRDs == null || !NRDs.Any())
            //     return "BadRequest("No NRDs provided")";

            //using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var updatedNRD in NRD)
                {
                    var existingNRD = await _context.NRDs
                        .FirstOrDefaultAsync(v => v.ID == updatedNRD.ID);

                    if (existingNRD != null)
                    {
                        // Update properties 
                        existingNRD.ID = updatedNRD.ID;
                        existingNRD.Code = updatedNRD.Code;
                        existingNRD.Name = updatedNRD.Name;
                        existingNRD.ParentID = updatedNRD.ParentID;
                    }
                    else
                    {
                        _context.ModuleData.Add(updatedNRD);
                    }
                }

                await _context.SaveChangesAsync();
                //await transaction.CommitAsync();

                return null;//Ok(new { message = $"{NRDs.Count} NRDs processed successfully" });
            }
            catch (Exception ex)
            {
                //await transaction.RollbackAsync();
                return null;//StatusCode(500, new { error = ex.Message });
            }
        }


        #endregion NRDs


        #region Buildings
        public void AddBuilding(Building bld)
        {
            _context.ModuleData.Add(bld);
            _context.SaveChanges();
        }

        public List<Building> GetAllBuildings()
        {
            return _context.Buildings.ToList();
        }



        // Find by Id
        public async Task<ModuleData?> GetByBldIdAsync(int id)
        {
            return await _context.ModuleData.FindAsync(id);
        }

        // Insert
        public async Task<ModuleData> CreateBldAsync(Building Building)
        {
            _context.ModuleData.Add(Building);
            await _context.SaveChangesAsync();
            return Building;
        }

        // Update
        public async Task<ModuleData?> UpdateBldAsync(int id, Building updatedBuilding)
        {
            var existingBuilding = await _context.ModuleData.FindAsync(id);
            if (existingBuilding == null) return null;

            // Update properties
            existingBuilding.ID = updatedBuilding.ID;
            existingBuilding.Code = updatedBuilding.Code;
            existingBuilding.Name = updatedBuilding.Name;
            existingBuilding.ParentID = updatedBuilding.ParentID;

            await _context.SaveChangesAsync();
            return existingBuilding;
        }

        // Delete
        public async Task<bool> DeleteBldAsync(int id)
        {
            var Building = await _context.ModuleData.FindAsync(id);
            if (Building == null) return false;
            _context.ModuleData.Remove(Building);
            await _context.SaveChangesAsync();
            return true;
        }

        // Get all
        public async Task<List<Building>> GetAllBldAsync()
        {
            return await _context.Buildings.ToListAsync();
        }

        public async Task<IActionResult> BulkSave([FromBody] List<Building> Building)
        {
            //if (Buildings == null || !Buildings.Any())
            //     return "BadRequest("No Buildings provided")";

           // using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var updatedBuilding in Building)
                {
                    var existingBuilding = await _context.Buildings
                        .FirstOrDefaultAsync(v => v.ID == updatedBuilding.ID);

                    if (existingBuilding != null)
                    {
                        // Update properties 
                        existingBuilding.ID = updatedBuilding.ID;
                        existingBuilding.Code = updatedBuilding.Code;
                        existingBuilding.Name = updatedBuilding.Name;
                        existingBuilding.ParentID = updatedBuilding.ParentID;
                    }
                    else
                    {
                        _context.ModuleData.Add(updatedBuilding);
                    }
                }

                await _context.SaveChangesAsync();
                //await transaction.CommitAsync();

                return null;//Ok(new { message = $"{Buildings.Count} Buildings processed successfully" });
            }
            catch (Exception ex)
            {
               // await transaction.RollbackAsync();
                return null;//StatusCode(500, new { error = ex.Message });
            }
        }


        #endregion Buildings

        #region Phases
        public void AddPhase(Phase phase)
        {
            _context.ModuleData.Add(phase);
            _context.SaveChanges();
        }

        public List<Phase> GetAllPhases()
        {
            return _context.Phases.ToList();
        }



        // Find by Id
        public async Task<ModuleData?> GetByPhaseIdAsync(int id)
        {
            return await _context.ModuleData.FindAsync(id);
        }

        // Insert
        public async Task<Phase> CreatePhaseAsync(Phase Phase)
        {
            _context.ModuleData.Add(Phase);
            await _context.SaveChangesAsync();
            return Phase;
        }

        // Update
        public async Task<ModuleData?> UpdatePhaseAsync(int id, Phase updatedPhase)
        {
            var existingPhase = await _context.ModuleData.FindAsync(id);
            if (existingPhase == null) return null;

            // Update properties
            existingPhase.ID = updatedPhase.ID;
            existingPhase.Code = updatedPhase.Code;
            existingPhase.Name = updatedPhase.Name;
            existingPhase.ParentID = updatedPhase.ParentID;

            await _context.SaveChangesAsync();
            return existingPhase;
        }

        // Delete
        public async Task<bool> DeletePhaseAsync(int id)
        {
            var Phase = await _context.ModuleData.FindAsync(id);
            if (Phase == null) return false;
            _context.ModuleData.Remove(Phase);
            await _context.SaveChangesAsync();
            return true;
        }

        // Get all
        public async Task<List<Phase>> GetAllPhaseAsync()
        {
            return await _context.Phases.ToListAsync();
        }

        public async Task<IActionResult> BulkSave([FromBody] List<Phase> Phase)
        {
            //if (Phases == null || !Phases.Any())
            //     return "BadRequest("No Phases provided")";

           // using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var updatedPhase in Phase)
                {
                    var existingPhase = await _context.Phases
                        .FirstOrDefaultAsync(v => v.ID == updatedPhase.ID);

                    if (existingPhase != null)
                    {
                        // Update properties 
                        existingPhase.ID = updatedPhase.ID;
                        existingPhase.Code = updatedPhase.Code;
                        existingPhase.Name = updatedPhase.Name;
                        existingPhase.ParentID = updatedPhase.ParentID;
                    }
                    else
                    {
                        _context.ModuleData.Add(updatedPhase);
                    }
                }

                await _context.SaveChangesAsync();
               // await transaction.CommitAsync();

                return null;//Ok(new { message = $"{Phases.Count} Phases processed successfully" });
            }
            catch (Exception ex)
            {
                //await transaction.RollbackAsync();
                return null;//StatusCode(500, new { error = ex.Message });
            }
        }


        #endregion Phases


        #region Amenities
        public void AddAmenities(Amenities Amenities)
        {
            _context.ModuleData.Add(Amenities);
            _context.SaveChanges();
        }

        public List<Amenities> GetAllAmenitiess()
        {
            return _context.Amenities_.ToList();
        }



        // Find by Id
        public async Task<ModuleData?> GetByAmenitiesIdAsync(int id)
        {
            return await _context.ModuleData.FindAsync(id);
        }

        // Insert
        public async Task<ModuleData> CreateAmenitiesAsync(Amenities Amenities)
        {
            _context.ModuleData.Add(Amenities);
            await _context.SaveChangesAsync();
            return Amenities;
        }

        // Update
        public async Task<ModuleData?> UpdateAmenitiesAsync(int id, Amenities updatedAmenities)
        {
            var existingAmenities = await _context.ModuleData.FindAsync(id);
            if (existingAmenities == null) return null;

            // Update properties
            existingAmenities.ID = updatedAmenities.ID;
            existingAmenities.Code = updatedAmenities.Code;
            existingAmenities.Name = updatedAmenities.Name;
            existingAmenities.ParentID = updatedAmenities.ParentID;

            await _context.SaveChangesAsync();
            return existingAmenities;
        }

        // Delete
        public async Task<bool> DeleteAmenitiesAsync(int id)
        {
            var Amenities = await _context.ModuleData.FindAsync(id);
            if (Amenities == null) return false;
            _context.ModuleData.Remove(Amenities);
            await _context.SaveChangesAsync();
            return true;
        }

        // Get all
        public async Task<List<Amenities>> GetAllAmenitiesAsync()
        {
            return await _context.Amenities_.ToListAsync();
        }

         
        public async Task<IActionResult> BulkSave([FromBody] List<Amenities> Amenities)
        {
            //if (Amenitiess == null || !Amenitiess.Any())
            //     return "BadRequest("No Amenitiess provided")";

           // using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var updatedAmenities in Amenities)
                {
                    var existingAmenities = await _context.Amenities_
                        .FirstOrDefaultAsync(v => v.ID == updatedAmenities.ID);

                    if (existingAmenities != null)
                    {
                        // Update properties 
                        existingAmenities.ID = updatedAmenities.ID;
                        existingAmenities.Code = updatedAmenities.Code;
                        existingAmenities.Name = updatedAmenities.Name;
                        existingAmenities.ParentID = updatedAmenities.ParentID;
                    }
                    else
                    {
                        _context.ModuleData.Add(updatedAmenities);
                    }
                }

                await _context.SaveChangesAsync();
              //  await transaction.CommitAsync();

                return null;//Ok(new { message = $"{Amenitiess.Count} Amenitiess processed successfully" });
            }
            catch (Exception ex)
            {
              //  await transaction.RollbackAsync();
                return null;//StatusCode(500, new { error = ex.Message });
            }
        }

        #endregion Amenities 


        #region Vehicles
        public void AddVehicle(Vehicle Vehicle)
        {
            _context.Vehicles.Add(Vehicle);
            _context.SaveChanges();
        }

        public List<Vehicle> GetAllVehicles()
        {
            return _context.Vehicles.ToList();
        }



        // Find by Id
        public async Task<Vehicle?> GetByVehicleIdAsync(int id)
        {
            return await _context.Vehicles.FindAsync(id);
        }

        // Insert
        public async Task<Vehicle> CreateVehicleAsync(Vehicle Vehicle)
        {
            _context.Vehicles.Add(Vehicle);
            await _context.SaveChangesAsync();
            return Vehicle;
        }

        // Update
        public async Task<Vehicle?> UpdateVehicleAsync(int id, Vehicle updatedVehicle)
        {
            var existingVehicle = await _context.Vehicles.FindAsync(id);
            if (existingVehicle == null) return null;

            // Update properties
            existingVehicle.ID = updatedVehicle.ID;
            existingVehicle.RegNo = updatedVehicle.RegNo;
            existingVehicle.vType = updatedVehicle.vType;
            existingVehicle.vMake = updatedVehicle.vMake;
            existingVehicle.vColor = updatedVehicle.vColor;
            existingVehicle.TagUID = updatedVehicle.TagUID;
            existingVehicle.TagEncodingDate = updatedVehicle.TagEncodingDate;
            existingVehicle.Logical_Delete = updatedVehicle.Logical_Delete;
            existingVehicle.PrintedTagID = updatedVehicle.PrintedTagID;
            existingVehicle.StickerNo = updatedVehicle.StickerNo;

            await _context.SaveChangesAsync();
            return existingVehicle;
        }

        // Delete
        public async Task<bool> DeleteVehicleAsync(int id)
        {
            var Vehicle = await _context.Vehicles.FindAsync(id);
            if (Vehicle == null) return false;
            _context.Vehicles.Remove(Vehicle);
            await _context.SaveChangesAsync();
            return true;
        }

        // Get all
        public async Task<List<Vehicle>> GetAllVehicleAsync()
        {
            return await _context.Vehicles.ToListAsync();
        }


        [HttpPost("bulk-save")]
        public async Task<IActionResult> BulkSave([FromBody] List<Vehicle>Vehicles)
        {
            //if (Vehicles == null || !Vehicles.Any())
            //     return "BadRequest("No Vehicles provided")";

            //using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var updatedVehicle in Vehicles)
                {
                    var existingVehicle = await _context.Vehicles
                        .FirstOrDefaultAsync(v => v.ID == updatedVehicle.ID);

                    if (existingVehicle != null)
                    {
                        // Update properties 
                        existingVehicle.RegNo = updatedVehicle.RegNo;
                        existingVehicle.vType = updatedVehicle.vType;
                        existingVehicle.vMake = updatedVehicle.vMake;
                        existingVehicle.vColor = updatedVehicle.vColor;
                        existingVehicle.TagUID = updatedVehicle.TagUID;
                        existingVehicle.TagEncodingDate = updatedVehicle.TagEncodingDate;
                        existingVehicle.Logical_Delete = updatedVehicle.Logical_Delete;
                        existingVehicle.PrintedTagID = updatedVehicle.PrintedTagID;
                        existingVehicle.StickerNo = updatedVehicle.StickerNo;
                    }
                    else
                    {
                        _context.Vehicles.Add(updatedVehicle);
                    }
                }

                await _context.SaveChangesAsync();
              //  await transaction.CommitAsync();

                return null;//Ok(new { message = $"{Vehicles.Count} Vehicles processed successfully" });
            }
            catch (Exception ex)
            {
              //  await transaction.RollbackAsync();
                return null;//StatusCode(500, new { error = ex.Message });
            }
        }


        #endregion Vehicles

        #region PrimaryLandowners
        public void AddPrimaryLandowner(PrimaryLandowner PLandowner)
        {
            _context.Landowners.Add(PLandowner);
            _context.SaveChanges();
        }

        public List<PrimaryLandowner> GetAllPrimaryLandowners()
        {
            return _context.Landowners.ToList();
        }
         
        // Find by Id
        public async Task<PrimaryLandowner?> GetByPrimaryLandownerIdAsync(int id)
        {
            return await _context.Landowners.FindAsync(id);
        }

        // Insert
        public async Task<PrimaryLandowner> CreatePrimaryLandownerAsync(PrimaryLandowner PrimaryLandowner)
        {
            _context.Landowners.Add(PrimaryLandowner);
            await _context.SaveChangesAsync();
            return PrimaryLandowner;
        }

        // Update
        public async Task<PrimaryLandowner?> UpdateLandownerAsync(int id, PrimaryLandowner updatedPrimaryLandowner)
        {
            var existingPrimaryLandowner = await _context.Landowners.FindAsync(id);
            if (existingPrimaryLandowner == null) return null;

            // Update properties 
            existingPrimaryLandowner.ID = updatedPrimaryLandowner.ID;
            existingPrimaryLandowner.CSN = updatedPrimaryLandowner.CSN;
            existingPrimaryLandowner.IDNumber = updatedPrimaryLandowner.IDNumber;
            existingPrimaryLandowner.TagNumber = updatedPrimaryLandowner.TagNumber;
            existingPrimaryLandowner.PANnumber = updatedPrimaryLandowner.PANnumber;
            existingPrimaryLandowner.PassportNo = updatedPrimaryLandowner.PassportNo;
            existingPrimaryLandowner.LicenseNo = updatedPrimaryLandowner.LicenseNo;
            existingPrimaryLandowner.AadharCardId = updatedPrimaryLandowner.AadharCardId;
            existingPrimaryLandowner.VoterID = updatedPrimaryLandowner.VoterID;
            existingPrimaryLandowner.FirstName = updatedPrimaryLandowner.FirstName;
            existingPrimaryLandowner.MiddleName = updatedPrimaryLandowner.MiddleName;
            existingPrimaryLandowner.LastName = updatedPrimaryLandowner.LastName;
            existingPrimaryLandowner.ShortName = updatedPrimaryLandowner.ShortName;
            existingPrimaryLandowner.Gender = updatedPrimaryLandowner.Gender;
            existingPrimaryLandowner.BloodGroup = updatedPrimaryLandowner.BloodGroup;
            existingPrimaryLandowner.DOB = updatedPrimaryLandowner.DOB;
            existingPrimaryLandowner.MobileNo = updatedPrimaryLandowner.MobileNo;
            existingPrimaryLandowner.CardIssueDate = updatedPrimaryLandowner.CardIssueDate;
            existingPrimaryLandowner.CardPrintingDate = updatedPrimaryLandowner.CardPrintingDate;
            existingPrimaryLandowner.LogicalDeleted = updatedPrimaryLandowner.LogicalDeleted;
            existingPrimaryLandowner.LandOwnerIssueDate = updatedPrimaryLandowner.LandOwnerIssueDate; 
            await _context.SaveChangesAsync();
            return existingPrimaryLandowner;
        }

        // Delete
        public async Task<bool> DeletePrimaryLandownerAsync(int id)
        {
            var Landowner = await _context.Landowners.FindAsync(id);
            if (Landowner == null) return false;
            _context.Landowners.Remove(Landowner);
            await _context.SaveChangesAsync();
            return true;
        }

        // Get all
        public async Task<List<PrimaryLandowner>> GetAllPrimaryLandownerAsync()
        {
            return await _context.Landowners.ToListAsync();
        }
         
        public async Task<IActionResult> BulkSave([FromBody] List<PrimaryLandowner> PrimaryLandowner)
        {
            //if (PrimaryLandowners == null || !PrimaryLandowners.Any())
            //     return "BadRequest("No PrimaryLandowners provided")";

           // using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var updatedPrimaryLandowner in PrimaryLandowner)
                {
                    var existingPrimaryLandowner = await _context.Landowners
                        .FirstOrDefaultAsync(v => v.ID == updatedPrimaryLandowner.ID);

                    if (existingPrimaryLandowner != null)
                    {
                        // Update properties 
                        existingPrimaryLandowner.ID = updatedPrimaryLandowner.ID;
                        existingPrimaryLandowner.CSN = updatedPrimaryLandowner.CSN;
                        existingPrimaryLandowner.IDNumber = updatedPrimaryLandowner.IDNumber;
                        existingPrimaryLandowner.TagNumber = updatedPrimaryLandowner.TagNumber;
                        existingPrimaryLandowner.PANnumber = updatedPrimaryLandowner.PANnumber;
                        existingPrimaryLandowner.PassportNo = updatedPrimaryLandowner.PassportNo;
                        existingPrimaryLandowner.LicenseNo = updatedPrimaryLandowner.LicenseNo;
                        existingPrimaryLandowner.AadharCardId = updatedPrimaryLandowner.AadharCardId;
                        existingPrimaryLandowner.VoterID = updatedPrimaryLandowner.VoterID;
                        existingPrimaryLandowner.FirstName = updatedPrimaryLandowner.FirstName;
                        existingPrimaryLandowner.MiddleName = updatedPrimaryLandowner.MiddleName;
                        existingPrimaryLandowner.LastName = updatedPrimaryLandowner.LastName;
                        existingPrimaryLandowner.ShortName = updatedPrimaryLandowner.ShortName;
                        existingPrimaryLandowner.Gender = updatedPrimaryLandowner.Gender;
                        existingPrimaryLandowner.BloodGroup = updatedPrimaryLandowner.BloodGroup;
                        existingPrimaryLandowner.DOB = updatedPrimaryLandowner.DOB;
                        existingPrimaryLandowner.MobileNo = updatedPrimaryLandowner.MobileNo;
                        existingPrimaryLandowner.CardIssueDate = updatedPrimaryLandowner.CardIssueDate;
                        existingPrimaryLandowner.CardPrintingDate = updatedPrimaryLandowner.CardPrintingDate;
                        existingPrimaryLandowner.LogicalDeleted = updatedPrimaryLandowner.LogicalDeleted;
                        existingPrimaryLandowner.LandOwnerIssueDate = updatedPrimaryLandowner.LandOwnerIssueDate;
                    }
                    else
                    {
                        _context.Landowners.Add(updatedPrimaryLandowner);
                    }
                }

                await _context.SaveChangesAsync();
               // await transaction.CommitAsync();

                return null;//Ok(new { message = $"{PrimaryLandowners.Count} PrimaryLandowners processed successfully" });
            }
            catch (Exception ex)
            {
               // await transaction.RollbackAsync();
                return null;//StatusCode(500, new { error = ex.Message });
            }
        }

        #endregion PrimaryLandowners

        #region DependentLandowners
        public void AddDependentLandowner(DependentLandOwner dlandowner)
        {
            _context.DependentLandowners.Add(dlandowner);
            _context.SaveChanges();
        }

        public List<DependentLandOwner> GetAllDependentLandowners()
        {
            return _context.DependentLandowners.ToList();
        }

      
        // Find by Id
        public async Task<DependentLandOwner?> GetByDependentLandownerIdAsync(int id)
        {
            return await _context.DependentLandowners.FindAsync(id);
        }

        // Insert
        public async Task<DependentLandOwner> CreateDependentLandownerAsync(DependentLandOwner DependentLandowner)
        {
            _context.DependentLandowners.Add(DependentLandowner);
            await _context.SaveChangesAsync();
            return DependentLandowner;
        }

        // Update
        public async Task<DependentLandOwner?> UpdateDependentLandownerAsync(int id, DependentLandOwner updatedDependentLandowner)
        {
            var existingDependentLandowner = await _context.DependentLandowners.FindAsync(id);
            if (existingDependentLandowner == null) return null;

            // Update properties 

            existingDependentLandowner.PID = updatedDependentLandowner.PID;
            existingDependentLandowner.ID = updatedDependentLandowner.ID;
            existingDependentLandowner.CSN = updatedDependentLandowner.CSN;
            existingDependentLandowner.IDNumber = updatedDependentLandowner.IDNumber;
            existingDependentLandowner.TagNumber = updatedDependentLandowner.TagNumber;
            existingDependentLandowner.PANnumber = updatedDependentLandowner.PANnumber;
            existingDependentLandowner.PassportNo = updatedDependentLandowner.PassportNo;
            existingDependentLandowner.LicenseNo = updatedDependentLandowner.LicenseNo;
            existingDependentLandowner.AadharCardId = updatedDependentLandowner.AadharCardId;
            existingDependentLandowner.VoterID = updatedDependentLandowner.VoterID;
            existingDependentLandowner.FirstName = updatedDependentLandowner.FirstName;
            existingDependentLandowner.MiddleName = updatedDependentLandowner.MiddleName;
            existingDependentLandowner.LastName = updatedDependentLandowner.LastName;
            existingDependentLandowner.ShortName = updatedDependentLandowner.ShortName;
            existingDependentLandowner.Gender = updatedDependentLandowner.Gender;
            existingDependentLandowner.BloodGroup = updatedDependentLandowner.BloodGroup;
            existingDependentLandowner.DOB = updatedDependentLandowner.DOB;
            existingDependentLandowner.MobileNo = updatedDependentLandowner.MobileNo;
            existingDependentLandowner.CardIssueDate = updatedDependentLandowner.CardIssueDate;
            existingDependentLandowner.CardPrintingDate = updatedDependentLandowner.CardPrintingDate;
            existingDependentLandowner.LogicalDeleted = updatedDependentLandowner.LogicalDeleted;
            existingDependentLandowner.DependLandOwnerIssueDate = updatedDependentLandowner.DependLandOwnerIssueDate;
            await _context.SaveChangesAsync();
            return existingDependentLandowner;
        }

        // Delete
        public async Task<bool> DeleteDependentLandownerAsync(int id)
        {
            var Landowner = await _context.Landowners.FindAsync(id);
            if (Landowner == null) return false;
            _context.Landowners.Remove(Landowner);
            await _context.SaveChangesAsync();
            return true;
        }

        // Get all
        public async Task<List<DependentLandOwner>> GetAllDependentLandownerAsync()
        {
            return await _context.DependentLandowners.ToListAsync();
        }

        public async Task<IActionResult> BulkSave([FromBody] List<DependentLandOwner> DependentLandOwner)
        {
            //if (DependentLandOwners == null || !DependentLandOwners.Any())
            //     return "BadRequest("No DependentLandOwners provided")";

            //using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var updatedDependentLandOwner in DependentLandOwner)
                {
                    var existingDependentLandOwner = await _context.DependentLandowners
                        .FirstOrDefaultAsync(v => v.ID == updatedDependentLandOwner.ID);

                    if (existingDependentLandOwner != null)
                    {
                        // Update properties 
                        existingDependentLandOwner.ID = updatedDependentLandOwner.ID;
                        existingDependentLandOwner.CSN = updatedDependentLandOwner.CSN;
                        existingDependentLandOwner.IDNumber = updatedDependentLandOwner.IDNumber;
                        existingDependentLandOwner.TagNumber = updatedDependentLandOwner.TagNumber;
                        existingDependentLandOwner.PANnumber = updatedDependentLandOwner.PANnumber;
                        existingDependentLandOwner.PassportNo = updatedDependentLandOwner.PassportNo;
                        existingDependentLandOwner.LicenseNo = updatedDependentLandOwner.LicenseNo;
                        existingDependentLandOwner.AadharCardId = updatedDependentLandOwner.AadharCardId;
                        existingDependentLandOwner.VoterID = updatedDependentLandOwner.VoterID;
                        existingDependentLandOwner.FirstName = updatedDependentLandOwner.FirstName;
                        existingDependentLandOwner.MiddleName = updatedDependentLandOwner.MiddleName;
                        existingDependentLandOwner.LastName = updatedDependentLandOwner.LastName;
                        existingDependentLandOwner.ShortName = updatedDependentLandOwner.ShortName;
                        existingDependentLandOwner.Gender = updatedDependentLandOwner.Gender;
                        existingDependentLandOwner.BloodGroup = updatedDependentLandOwner.BloodGroup;
                        existingDependentLandOwner.DOB = updatedDependentLandOwner.DOB;
                        existingDependentLandOwner.MobileNo = updatedDependentLandOwner.MobileNo;
                        existingDependentLandOwner.CardIssueDate = updatedDependentLandOwner.CardIssueDate;
                        existingDependentLandOwner.CardPrintingDate = updatedDependentLandOwner.CardPrintingDate;
                        existingDependentLandOwner.LogicalDeleted = updatedDependentLandOwner.LogicalDeleted;
                        existingDependentLandOwner.DependLandOwnerIssueDate = updatedDependentLandOwner.DependLandOwnerIssueDate;
                    }
                    else
                    {
                        _context.DependentLandowners.Add(updatedDependentLandOwner);
                    }
                }

                await _context.SaveChangesAsync();
               // await transaction.CommitAsync();

                return null;//Ok(new { message = $"{DependentLandOwners.Count} DependentLandOwners processed successfully" });
            }
            catch (Exception ex)
            {
             //   await transaction.RollbackAsync();
                return null;//StatusCode(500, new { error = ex.Message });
            }
        }


        #endregion DependentLandowners

        #region PrimaryResident
        public void AddPrimaryResident(PrimaryResident PResident)
        {
            _context.PrimaryResidents.Add(PResident);
            _context.SaveChanges();
        }

        public List<PrimaryResident> GetAllPrimaryResidents()
        {
            return _context.PrimaryResidents.ToList();
        }

        // Find by Id
        public async Task<PrimaryResident?> GetByPrimaryResidentIdAsync(int id)
        {
            return await _context.PrimaryResidents.FindAsync(id);
        }

        // Insert
        public async Task<PrimaryResident> CreatePrimaryResidentAsync(PrimaryResident PrimaryResident)
        {
            _context.PrimaryResidents.Add(PrimaryResident);
            await _context.SaveChangesAsync();
            return PrimaryResident;
        }

        // Update
        public async Task<PrimaryResident?> UpdatePrimaryResidentAsync(int id, PrimaryResident updatedPrimaryResident)
        {
            var existingPrimaryResident = await _context.PrimaryResidents.FindAsync(id);
            if (existingPrimaryResident == null) return null;

            // Update properties 
            existingPrimaryResident.ID = updatedPrimaryResident.ID;
            existingPrimaryResident.CSN = updatedPrimaryResident.CSN;
            existingPrimaryResident.IDNumber = updatedPrimaryResident.IDNumber;
            existingPrimaryResident.TagNumber = updatedPrimaryResident.TagNumber;
            existingPrimaryResident.PANnumber = updatedPrimaryResident.PANnumber;
            existingPrimaryResident.PassportNo = updatedPrimaryResident.PassportNo;
            existingPrimaryResident.LicenseNo = updatedPrimaryResident.LicenseNo;
            existingPrimaryResident.AadharCardId = updatedPrimaryResident.AadharCardId;
            existingPrimaryResident.VoterID = updatedPrimaryResident.VoterID;
            existingPrimaryResident.FirstName = updatedPrimaryResident.FirstName;
            existingPrimaryResident.MiddleName = updatedPrimaryResident.MiddleName;
            existingPrimaryResident.LastName = updatedPrimaryResident.LastName;
            existingPrimaryResident.ShortName = updatedPrimaryResident.ShortName;
            existingPrimaryResident.Gender = updatedPrimaryResident.Gender;
            existingPrimaryResident.BloodGroup = updatedPrimaryResident.BloodGroup;
            existingPrimaryResident.DOB = updatedPrimaryResident.DOB;
            existingPrimaryResident.MobileNo = updatedPrimaryResident.MobileNo;
            existingPrimaryResident.CardIssueDate = updatedPrimaryResident.CardIssueDate;
            existingPrimaryResident.CardPrintingDate = updatedPrimaryResident.CardPrintingDate;
            existingPrimaryResident.LogicalDeleted = updatedPrimaryResident.LogicalDeleted;
            existingPrimaryResident.RegistrationIssueDate = updatedPrimaryResident.RegistrationIssueDate;
            await _context.SaveChangesAsync();
            return existingPrimaryResident;
        }

        // Delete
        public async Task<bool> DeletePrimaryResidentAsync(int id)
        {
            var PrimaryResident = await _context.PrimaryResidents.FindAsync(id);
            if (PrimaryResident == null) return false;
            _context.PrimaryResidents.Remove(PrimaryResident);
            await _context.SaveChangesAsync();
            return true;
        }

        // Get all
        public async Task<List<PrimaryResident>> GetAllPrimaryResidentAsync()
        {
            return await _context.PrimaryResidents.ToListAsync();
        }

        public async Task<IActionResult> BulkSave([FromBody] List<PrimaryResident> PrimaryResident)
        {
            //if (PrimaryResidents == null || !PrimaryResidents.Any())
            //     return "BadRequest("No PrimaryResidents provided")";

          //  using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var updatedPrimaryResident in PrimaryResident)
                {
                    var existingPrimaryResident = await _context.PrimaryResidents
                        .FirstOrDefaultAsync(v => v.ID == updatedPrimaryResident.ID);

                    if (existingPrimaryResident != null)
                    {
                        // Update properties 
                        existingPrimaryResident.ID = updatedPrimaryResident.ID;
                        existingPrimaryResident.CSN = updatedPrimaryResident.CSN;
                        existingPrimaryResident.IDNumber = updatedPrimaryResident.IDNumber;
                        existingPrimaryResident.TagNumber = updatedPrimaryResident.TagNumber;
                        existingPrimaryResident.PANnumber = updatedPrimaryResident.PANnumber;
                        existingPrimaryResident.PassportNo = updatedPrimaryResident.PassportNo;
                        existingPrimaryResident.LicenseNo = updatedPrimaryResident.LicenseNo;
                        existingPrimaryResident.AadharCardId = updatedPrimaryResident.AadharCardId;
                        existingPrimaryResident.VoterID = updatedPrimaryResident.VoterID;
                        existingPrimaryResident.FirstName = updatedPrimaryResident.FirstName;
                        existingPrimaryResident.MiddleName = updatedPrimaryResident.MiddleName;
                        existingPrimaryResident.LastName = updatedPrimaryResident.LastName;
                        existingPrimaryResident.ShortName = updatedPrimaryResident.ShortName;
                        existingPrimaryResident.Gender = updatedPrimaryResident.Gender;
                        existingPrimaryResident.BloodGroup = updatedPrimaryResident.BloodGroup;
                        existingPrimaryResident.DOB = updatedPrimaryResident.DOB;
                        existingPrimaryResident.MobileNo = updatedPrimaryResident.MobileNo;
                        existingPrimaryResident.CardIssueDate = updatedPrimaryResident.CardIssueDate;
                        existingPrimaryResident.CardPrintingDate = updatedPrimaryResident.CardPrintingDate;
                        existingPrimaryResident.LogicalDeleted = updatedPrimaryResident.LogicalDeleted;
                        existingPrimaryResident.RegistrationIssueDate = updatedPrimaryResident.RegistrationIssueDate;
                    }
                    else
                    {
                        _context.PrimaryResidents.Add(updatedPrimaryResident);
                    }
                }

                await _context.SaveChangesAsync();
                //await transaction.CommitAsync();

                return null;//Ok(new { message = $"{PrimaryResidents.Count} PrimaryResidents processed successfully" });
            }
            catch (Exception ex)
            {
               // await transaction.RollbackAsync();
                return null;//StatusCode(500, new { error = ex.Message });
            }
        }

        #endregion PrimaryResidents

        #region DependentResident
        public void AddDependentResident(DependentResident PResident)
        {
            _context.DependentResidents.Add(PResident);
            _context.SaveChanges();
        }

        public List<DependentResident> GetAllDependentResidents()
        {
            return _context.DependentResidents.ToList();
        }

        // Find by Id
        public async Task<DependentResident?> GetByDependentResidentIdAsync(int id)
        {
            return await _context.DependentResidents.FindAsync(id);
        }

        // Insert
        public async Task<DependentResident> CreateDependentResidentAsync(DependentResident DependentResident)
        {
            _context.DependentResidents.Add(DependentResident);
            await _context.SaveChangesAsync();
            return DependentResident;
        }

        // Update
        public async Task<DependentResident?> UpdateDependentResidentAsync(int id, DependentResident updatedDependentResident)
        {
            var existingDependentResident = await _context.DependentResidents.FindAsync(id);
            if (existingDependentResident == null) return null;

            // Update properties 
            existingDependentResident.ID = updatedDependentResident.ID;
            existingDependentResident.PID = updatedDependentResident.PID;
            existingDependentResident.CSN = updatedDependentResident.CSN;
            existingDependentResident.IDNumber = updatedDependentResident.IDNumber;
            existingDependentResident.TagNumber = updatedDependentResident.TagNumber;
            existingDependentResident.PANnumber = updatedDependentResident.PANnumber;
            existingDependentResident.PassportNo = updatedDependentResident.PassportNo;
            existingDependentResident.LicenseNo = updatedDependentResident.LicenseNo;
            existingDependentResident.AadharCardId = updatedDependentResident.AadharCardId;
            existingDependentResident.VoterID = updatedDependentResident.VoterID;
            existingDependentResident.FirstName = updatedDependentResident.FirstName;
            existingDependentResident.MiddleName = updatedDependentResident.MiddleName;
            existingDependentResident.LastName = updatedDependentResident.LastName;
            existingDependentResident.ShortName = updatedDependentResident.ShortName;
            existingDependentResident.Gender = updatedDependentResident.Gender;
            existingDependentResident.BloodGroup = updatedDependentResident.BloodGroup;
            existingDependentResident.DOB = updatedDependentResident.DOB;
            existingDependentResident.MobileNo = updatedDependentResident.MobileNo;
            existingDependentResident.CardIssueDate = updatedDependentResident.CardIssueDate;
            existingDependentResident.CardPrintingDate = updatedDependentResident.CardPrintingDate;
            existingDependentResident.LogicalDeleted = updatedDependentResident.LogicalDeleted;
            existingDependentResident.RegistrationIssueDate = updatedDependentResident.RegistrationIssueDate;
            await _context.SaveChangesAsync();
            return existingDependentResident;
        }

        // Delete
        public async Task<bool> DeleteDependentResidentAsync(int id)
        {
            var DependentResident = await _context.DependentResidents.FindAsync(id);
            if (DependentResident == null) return false;
            _context.DependentResidents.Remove(DependentResident);
            await _context.SaveChangesAsync();
            return true;
        }

        // Get all
        public async Task<List<DependentResident>> GetAllDependentResidentAsync()
        {
            return await _context.DependentResidents.ToListAsync();
        }

        public async Task<IActionResult> BulkSave([FromBody] List<DependentResident> DependentResident)
        {
            //if (DependentResidents == null || !DependentResidents.Any())
            //     return "BadRequest("No DependentResidents provided")";

           // using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var updatedDependentResident in DependentResident)
                {
                    var existingDependentResident = await _context.DependentResidents
                        .FirstOrDefaultAsync(v => v.ID == updatedDependentResident.ID);

                    if (existingDependentResident != null)
                    {
                        // Update properties 
                        existingDependentResident.ID = updatedDependentResident.ID;
                        existingDependentResident.PID = updatedDependentResident.PID;
                        existingDependentResident.CSN = updatedDependentResident.CSN;
                        existingDependentResident.IDNumber = updatedDependentResident.IDNumber;
                        existingDependentResident.TagNumber = updatedDependentResident.TagNumber;
                        existingDependentResident.PANnumber = updatedDependentResident.PANnumber;
                        existingDependentResident.PassportNo = updatedDependentResident.PassportNo;
                        existingDependentResident.LicenseNo = updatedDependentResident.LicenseNo;
                        existingDependentResident.AadharCardId = updatedDependentResident.AadharCardId;
                        existingDependentResident.VoterID = updatedDependentResident.VoterID;
                        existingDependentResident.FirstName = updatedDependentResident.FirstName;
                        existingDependentResident.MiddleName = updatedDependentResident.MiddleName;
                        existingDependentResident.LastName = updatedDependentResident.LastName;
                        existingDependentResident.ShortName = updatedDependentResident.ShortName;
                        existingDependentResident.Gender = updatedDependentResident.Gender;
                        existingDependentResident.BloodGroup = updatedDependentResident.BloodGroup;
                        existingDependentResident.DOB = updatedDependentResident.DOB;
                        existingDependentResident.MobileNo = updatedDependentResident.MobileNo;
                        existingDependentResident.CardIssueDate = updatedDependentResident.CardIssueDate;
                        existingDependentResident.CardPrintingDate = updatedDependentResident.CardPrintingDate;
                        existingDependentResident.LogicalDeleted = updatedDependentResident.LogicalDeleted;
                        existingDependentResident.RegistrationIssueDate = updatedDependentResident.RegistrationIssueDate;
                    }
                    else
                    {
                        _context.DependentResidents.Add(updatedDependentResident);
                    }
                }

                await _context.SaveChangesAsync();
             //   await transaction.CommitAsync();

                return null;//Ok(new { message = $"{DependentResidents.Count} DependentResidents processed successfully" });
            }
            catch (Exception ex)
            {
              //  await transaction.RollbackAsync();
                return null;//StatusCode(500, new { error = ex.Message });
            }
        }

        #endregion DependentResidents


        #region PrimaryTenent
        public void AddPrimaryTenent(PrimaryTenent PTenent)
        {
            _context.PrimaryTenents.Add(PTenent);
            _context.SaveChanges();
        }

        public List<PrimaryTenent> GetAllPrimaryTenents()
        {
            return _context.PrimaryTenents.ToList();
        }

        // Find by Id
        public async Task<PrimaryTenent?> GetByPrimaryTenentIdAsync(int id)
        {
            return await _context.PrimaryTenents.FindAsync(id);
        }

        // Insert
        public async Task<PrimaryTenent> CreatePrimaryTenentAsync(PrimaryTenent PrimaryTenent)
        {
            _context.PrimaryTenents.Add(PrimaryTenent);
            await _context.SaveChangesAsync();
            return PrimaryTenent;
        }

        // Update
        public async Task<PrimaryTenent?> UpdatePrimaryTenentAsync(int id, PrimaryTenent updatedPrimaryTenent)
        {
            var existingPrimaryTenent = await _context.PrimaryTenents.FindAsync(id);
            if (existingPrimaryTenent == null) return null;

            // Update properties 
            existingPrimaryTenent.ID = updatedPrimaryTenent.ID; 
            existingPrimaryTenent.RID = updatedPrimaryTenent.RID;
            existingPrimaryTenent.CSN = updatedPrimaryTenent.CSN;
            existingPrimaryTenent.IDNumber = updatedPrimaryTenent.IDNumber;
            existingPrimaryTenent.TagNumber = updatedPrimaryTenent.TagNumber;
            existingPrimaryTenent.PANnumber = updatedPrimaryTenent.PANnumber;
            existingPrimaryTenent.PassportNo = updatedPrimaryTenent.PassportNo;
            existingPrimaryTenent.LicenseNo = updatedPrimaryTenent.LicenseNo;
            existingPrimaryTenent.AadharCardId = updatedPrimaryTenent.AadharCardId;
            existingPrimaryTenent.VoterID = updatedPrimaryTenent.VoterID;
            existingPrimaryTenent.FirstName = updatedPrimaryTenent.FirstName;
            existingPrimaryTenent.MiddleName = updatedPrimaryTenent.MiddleName;
            existingPrimaryTenent.LastName = updatedPrimaryTenent.LastName;
            existingPrimaryTenent.ShortName = updatedPrimaryTenent.ShortName;
            existingPrimaryTenent.Gender = updatedPrimaryTenent.Gender;
            existingPrimaryTenent.BloodGroup = updatedPrimaryTenent.BloodGroup;
            existingPrimaryTenent.DOB = updatedPrimaryTenent.DOB;
            existingPrimaryTenent.MobileNo = updatedPrimaryTenent.MobileNo;
            
            existingPrimaryTenent.TenentType = updatedPrimaryTenent.TenentType;
            existingPrimaryTenent.CardIssueDate = updatedPrimaryTenent.CardIssueDate;
            existingPrimaryTenent.CardPrintingDate = updatedPrimaryTenent.CardPrintingDate;
            existingPrimaryTenent.LogicalDeleted = updatedPrimaryTenent.LogicalDeleted;
            existingPrimaryTenent.RegistrationIssueDate = updatedPrimaryTenent.RegistrationIssueDate;
            existingPrimaryTenent.Aggreement_From = updatedPrimaryTenent.Aggreement_From;
            existingPrimaryTenent.Aggreement_To = updatedPrimaryTenent.Aggreement_To; 

            await _context.SaveChangesAsync();
            return existingPrimaryTenent;
        }

        // Delete
        public async Task<bool> DeletePrimaryTenentAsync(int id)
        {
            var PrimaryTenent = await _context.PrimaryTenents.FindAsync(id);
            if (PrimaryTenent == null) return false;
            _context.PrimaryTenents.Remove(PrimaryTenent);
            await _context.SaveChangesAsync();
            return true;
        }

        // Get all
        public async Task<List<PrimaryTenent>> GetAllPrimaryTenentAsync()
        {
            return await _context.PrimaryTenents.ToListAsync();
        }

        public async Task<IActionResult> BulkSave([FromBody] List<PrimaryTenent> PrimaryTenent)
        {
            //if (PrimaryTenents == null || !PrimaryTenents.Any())
            //     return "BadRequest("No PrimaryTenents provided")";

           // using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var updatedPrimaryTenent in PrimaryTenent)
                {
                    var existingPrimaryTenent = await _context.PrimaryTenents
                        .FirstOrDefaultAsync(v => v.ID == updatedPrimaryTenent.ID);

                    if (existingPrimaryTenent != null)
                    {
                        // Update properties 
                        existingPrimaryTenent.ID = updatedPrimaryTenent.ID; 
                        existingPrimaryTenent.RID = updatedPrimaryTenent.RID;
                        existingPrimaryTenent.CSN = updatedPrimaryTenent.CSN;
                        existingPrimaryTenent.IDNumber = updatedPrimaryTenent.IDNumber;
                        existingPrimaryTenent.TagNumber = updatedPrimaryTenent.TagNumber;
                        existingPrimaryTenent.PANnumber = updatedPrimaryTenent.PANnumber;
                        existingPrimaryTenent.PassportNo = updatedPrimaryTenent.PassportNo;
                        existingPrimaryTenent.LicenseNo = updatedPrimaryTenent.LicenseNo;
                        existingPrimaryTenent.AadharCardId = updatedPrimaryTenent.AadharCardId;
                        existingPrimaryTenent.VoterID = updatedPrimaryTenent.VoterID;
                        existingPrimaryTenent.FirstName = updatedPrimaryTenent.FirstName;
                        existingPrimaryTenent.MiddleName = updatedPrimaryTenent.MiddleName;
                        existingPrimaryTenent.LastName = updatedPrimaryTenent.LastName;
                        existingPrimaryTenent.ShortName = updatedPrimaryTenent.ShortName;
                        existingPrimaryTenent.Gender = updatedPrimaryTenent.Gender;
                        existingPrimaryTenent.BloodGroup = updatedPrimaryTenent.BloodGroup;
                        existingPrimaryTenent.DOB = updatedPrimaryTenent.DOB;
                        existingPrimaryTenent.MobileNo = updatedPrimaryTenent.MobileNo;


                        existingPrimaryTenent.TenentType = updatedPrimaryTenent.TenentType;
                        existingPrimaryTenent.CardIssueDate = updatedPrimaryTenent.CardIssueDate;
                        existingPrimaryTenent.CardPrintingDate = updatedPrimaryTenent.CardPrintingDate;
                        existingPrimaryTenent.LogicalDeleted = updatedPrimaryTenent.LogicalDeleted;
                        existingPrimaryTenent.RegistrationIssueDate = updatedPrimaryTenent.RegistrationIssueDate;
                        existingPrimaryTenent.Aggreement_From = updatedPrimaryTenent.Aggreement_From;
                        existingPrimaryTenent.Aggreement_To = updatedPrimaryTenent.Aggreement_To;
                    }
                    else
                    {
                        _context.PrimaryTenents.Add(updatedPrimaryTenent);
                    }
                }

                await _context.SaveChangesAsync();
              //  await transaction.CommitAsync();

                return null;//Ok(new { message = $"{PrimaryTenents.Count} PrimaryTenents processed successfully" });
            }
            catch (Exception ex)
            {
              //  await transaction.RollbackAsync();
                return null;//StatusCode(500, new { error = ex.Message });
            }
        }

        #endregion PrimaryTenent

        #region DependentTenent
        public void AddDependentTenent(DependentTenent PTenent)
        {
            _context.DependentTenents.Add(PTenent);
            _context.SaveChanges();
        }

        public List<DependentTenent> GetAllDependentTenents()
        {
            return _context.DependentTenents.ToList();
        }

        // Find by Id
        public async Task<DependentTenent?> GetByDependentTenentIdAsync(int id)
        {
            return await _context.DependentTenents.FindAsync(id);
        }

        // Insert
        public async Task<DependentTenent> CreateDependentTenentAsync(DependentTenent DependentTenent)
        {
            _context.DependentTenents.Add(DependentTenent);
            await _context.SaveChangesAsync();
            return DependentTenent;
        }

        // Update
        public async Task<DependentTenent?> UpdateDependentTenentAsync(int id, DependentTenent updatedDependentTenent)
        {
            var existingDependentTenent = await _context.DependentTenents.FindAsync(id);
            if (existingDependentTenent == null) return null;

            // Update properties 
            existingDependentTenent.ID = updatedDependentTenent.ID;
            existingDependentTenent.PID = updatedDependentTenent.PID;
            existingDependentTenent.CSN = updatedDependentTenent.CSN;
            existingDependentTenent.IDNumber = updatedDependentTenent.IDNumber;
            existingDependentTenent.TagNumber = updatedDependentTenent.TagNumber;
            existingDependentTenent.PANnumber = updatedDependentTenent.PANnumber;
            existingDependentTenent.PassportNo = updatedDependentTenent.PassportNo;
            existingDependentTenent.LicenseNo = updatedDependentTenent.LicenseNo;
            existingDependentTenent.AadharCardId = updatedDependentTenent.AadharCardId;
            existingDependentTenent.VoterID = updatedDependentTenent.VoterID;
            existingDependentTenent.FirstName = updatedDependentTenent.FirstName;
            existingDependentTenent.MiddleName = updatedDependentTenent.MiddleName;
            existingDependentTenent.LastName = updatedDependentTenent.LastName;
            existingDependentTenent.ShortName = updatedDependentTenent.ShortName;
            existingDependentTenent.Gender = updatedDependentTenent.Gender;
            existingDependentTenent.BloodGroup = updatedDependentTenent.BloodGroup;
            existingDependentTenent.DOB = updatedDependentTenent.DOB;
            existingDependentTenent.MobileNo = updatedDependentTenent.MobileNo;
            existingDependentTenent.CardIssueDate = updatedDependentTenent.CardIssueDate;
            existingDependentTenent.CardPrintingDate = updatedDependentTenent.CardPrintingDate;
            existingDependentTenent.LogicalDeleted = updatedDependentTenent.LogicalDeleted;
            existingDependentTenent.RegistrationIssueDate = updatedDependentTenent.RegistrationIssueDate;
            existingDependentTenent.Aggreement_From = updatedDependentTenent.Aggreement_From;
            existingDependentTenent.Aggreement_To = updatedDependentTenent.Aggreement_To;

            await _context.SaveChangesAsync();
            return existingDependentTenent;
        }

        // Delete
        public async Task<bool> DeleteDependentTenentAsync(int id)
        {
            var DependentTenent = await _context.DependentTenents.FindAsync(id);
            if (DependentTenent == null) return false;
            _context.DependentTenents.Remove(DependentTenent);
            await _context.SaveChangesAsync();
            return true;
        }

        // Get all
        public async Task<List<DependentTenent>> GetAllDependentTenentAsync()
        {
            return await _context.DependentTenents.ToListAsync();
        }

        public async Task<IActionResult> BulkSave([FromBody] List<DependentTenent> DependentTenent)
        {
            //if (DependentTenents == null || !DependentTenents.Any())
            //     return "BadRequest("No DependentTenents provided")";

         //   using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var updatedDependentTenent in DependentTenent)
                {
                    var existingDependentTenent = await _context.DependentTenents
                        .FirstOrDefaultAsync(v => v.ID == updatedDependentTenent.ID);

                    if (existingDependentTenent != null)
                    {
                        // Update properties 
                        existingDependentTenent.ID = updatedDependentTenent.ID;
                        existingDependentTenent.PID = updatedDependentTenent.PID;
                        existingDependentTenent.CSN = updatedDependentTenent.CSN;
                        existingDependentTenent.IDNumber = updatedDependentTenent.IDNumber;
                        existingDependentTenent.TagNumber = updatedDependentTenent.TagNumber;
                        existingDependentTenent.PANnumber = updatedDependentTenent.PANnumber;
                        existingDependentTenent.PassportNo = updatedDependentTenent.PassportNo;
                        existingDependentTenent.LicenseNo = updatedDependentTenent.LicenseNo;
                        existingDependentTenent.AadharCardId = updatedDependentTenent.AadharCardId;
                        existingDependentTenent.VoterID = updatedDependentTenent.VoterID;
                        existingDependentTenent.FirstName = updatedDependentTenent.FirstName;
                        existingDependentTenent.MiddleName = updatedDependentTenent.MiddleName;
                        existingDependentTenent.LastName = updatedDependentTenent.LastName;
                        existingDependentTenent.ShortName = updatedDependentTenent.ShortName;
                        existingDependentTenent.Gender = updatedDependentTenent.Gender;
                        existingDependentTenent.BloodGroup = updatedDependentTenent.BloodGroup;
                        existingDependentTenent.DOB = updatedDependentTenent.DOB;
                        existingDependentTenent.MobileNo = updatedDependentTenent.MobileNo;
                        existingDependentTenent.CardIssueDate = updatedDependentTenent.CardIssueDate;
                        existingDependentTenent.CardPrintingDate = updatedDependentTenent.CardPrintingDate;
                        existingDependentTenent.LogicalDeleted = updatedDependentTenent.LogicalDeleted;
                        existingDependentTenent.RegistrationIssueDate = updatedDependentTenent.RegistrationIssueDate;

                        existingDependentTenent.Aggreement_From = updatedDependentTenent.Aggreement_From;
                        existingDependentTenent.Aggreement_To = updatedDependentTenent.Aggreement_To;
                    }
                    else
                    {
                        _context.DependentTenents.Add(updatedDependentTenent);
                    }
                }

                await _context.SaveChangesAsync();
              //  await transaction.CommitAsync();

                return null;//Ok(new { message = $"{DependentTenents.Count} DependentTenents processed successfully" });
            }
            catch (Exception ex)
            {
              //  await transaction.RollbackAsync();
                return null;//StatusCode(500, new { error = ex.Message });
            }
        }

        #endregion DependentTenent
 
    }
}




 