
using Microsoft.AspNetCore.Mvc;
using Township_API.Models;
using Township_API.Services;
using Township_API.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Township_API.Models.commonTypes;
using EventFlow.Jobs;

namespace Township_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LandownerController : Controller
    {
        private readonly AppDBContext _context;
        public LandownerController(AppDBContext context)
        {
            _context = context;
        }

        // PUT: api/products/5
        [HttpPost("{UpdateLandowner}/{id}")]
        public async Task<IActionResult> UpdateLandowner(int id, [FromBody] PrimaryLandowner updatedLandowner)
        {
            try
            {
                if (id != updatedLandowner.ID)
                {
                    return BadRequest("Landowner ID mismatch.");
                }


                //flatnumber validation                
                var existing = await _context.Landowners.Where(p => p.FirstName == updatedLandowner.FirstName && p.LastName== updatedLandowner.LastName && p.ID != updatedLandowner.ID).ToListAsync();
                if (existing != null)
                {
                    if (existing.Count() > 0)
                        return BadRequest("Entry already register with other owner!");
                }


                var existingLandowner = await _context.Landowners.FindAsync(id);
                // var existingLandowner = await _service.UpdateLandownerAsync(updatedLandowner.ID, updatedLandowner);
                if (existingLandowner == null)
                {
                    return NotFound();
                }

                existingLandowner.CSN = updatedLandowner.CSN;
                // existingLandowner.IDNumber = updatedLandowner.IDNumber;
                existingLandowner.TagNumber = updatedLandowner.TagNumber;
                existingLandowner.PANnumber = updatedLandowner.PANnumber;
                existingLandowner.PassportNo = updatedLandowner.PassportNo;
                existingLandowner.LicenseNo = updatedLandowner.LicenseNo;
                existingLandowner.ICEno = updatedLandowner.ICEno;
                existingLandowner.AadharCardId = updatedLandowner.AadharCardId;
                existingLandowner.VoterID = updatedLandowner.VoterID;
                existingLandowner.FirstName = updatedLandowner.FirstName;
                existingLandowner.MiddleName = updatedLandowner.MiddleName;
                existingLandowner.LastName = updatedLandowner.LastName;
                existingLandowner.ShortName = updatedLandowner.ShortName;
                existingLandowner.Gender = updatedLandowner.Gender;
                existingLandowner.BloodGroup = updatedLandowner.BloodGroup;
                existingLandowner.DOB = updatedLandowner.DOB;
                existingLandowner.EmailID = updatedLandowner.EmailID;
                existingLandowner.MobileNo = updatedLandowner.MobileNo;
                existingLandowner.LandLine = updatedLandowner.LandLine;
                existingLandowner.NRD = updatedLandowner.NRD;
                existingLandowner.Building = updatedLandowner.Building;
                existingLandowner.FlatNumber = updatedLandowner.FlatNumber;
                existingLandowner.CardIssueDate = updatedLandowner.CardIssueDate;
                existingLandowner.CardPrintingDate = updatedLandowner.CardIssueDate;
                existingLandowner.LogicalDeleted = updatedLandowner.LogicalDeleted;
                existingLandowner.LandOwnerIssueDate = updatedLandowner.LandOwnerIssueDate;
                _context.Entry(existingLandowner).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                return Ok(new { Message = $"Landowner Updated successfully", IDNumber = existingLandowner.IDNumber });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Error: " + ex.Message.ToString());
                return BadRequest("Error :" + ex.Message.ToString());

            }
        }


        [HttpPost("AddLandowner")]
        public async Task<IActionResult> AddLandowner([FromBody] PrimaryLandowner obj)
        {
            try
            {
                var existingLandowner = await _context.Landowners.FindAsync(0);
                if (existingLandowner != null)
                {
                    return BadRequest("Landowner Exists.");
                }

                //flatnumber validation                
                var existing = await _context.Landowners.Where(p => p.FirstName == obj.FirstName && p.LastName == obj.LastName  ).ToListAsync();
                if (existing != null)
                {
                    if (existing.Count() > 0)
                        return BadRequest("Entry already register same First and Last Name!");
                }

                _context.Add(obj); 
                await _context.SaveChangesAsync();                
                int number = (int)AccessCardHolders.Landowner;
                obj.IDNumber = number.ToString() + obj.ID.ToString("D5");
                await _context.SaveChangesAsync();
                return Ok(new { Message = $"Landowner created successfully", IDNumber = obj.IDNumber });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Error: " + ex.Message.ToString());
                return BadRequest(ex.Message.ToString());
            }
        }

        // GET: api/products 
        [HttpGet]
        public async Task<IActionResult> GetAllLandowners()
        {
            try
            {
                var Landowners = await _context.Landowners.Where( p=> p.LogicalDeleted == 0) .OrderByDescending(p => p.ID ).ToListAsync();
                foreach (var res in Landowners)
                {
                    var nr = await _context.ModuleData.Where(u => u.ID.ToString() == res.NRD.ToString()).FirstOrDefaultAsync();
                    if (nr != null)
                        res.nrdName = (string)nr.Name.ToString(); 
                      nr = await _context.ModuleData.Where(u => u.ID.ToString() == res.Building.ToString()).FirstOrDefaultAsync();
                    if (nr != null)
                        res.buildingName = (string)nr.Name.ToString();

                }

                    return Ok(Landowners);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message,
                    stackTrace = ex.StackTrace,
                    innerException = ex.InnerException?.Message,
                    innerStackTrace = ex.InnerException?.StackTrace
                });
            }
        }


        // GET: api/products 
        [HttpGet("{ID}")]
        public async Task<IActionResult> GetLandownerDetails(int ID)
        {
            var Landowners = await _context.Landowners.Where(p => p.ID == ID ).ToListAsync();
            if (Landowners != null && Landowners.Count > 0)
            {

                string? IdNumber = Landowners[0].IDNumber;
                if (IdNumber != null)
                {
                    try
                    {
                        var jsonWrapper = new DependentJsonWrapper
                        {
                            Owners = Landowners,
                            DependentOwners = await _context.DependentLandowners.Where(p => p.PID == ID).ToListAsync(),
                            Vehicles = await _context.Vehicles.Where(p => p.TagUID == IdNumber).ToListAsync(),
                            UserAllAccess = await _context._userAllAccess.Where(p => p.CardHolderID == IdNumber).ToListAsync(),
                            UserNRDAccess = await _context._userNRDAccess.Where(p => p.CardHolderID == IdNumber).ToListAsync(),
                            UserBuildingAccess = await _context._userBuildingAccess.Where(p => p.CardHolderID == IdNumber).ToListAsync(),
                            UserAminitiesAccess = await _context._userAmenitiesAccess.Where(p => p.CardHolderID == IdNumber).ToListAsync()
                        };


                        return Ok(jsonWrapper);
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(ex.Message.ToString());
                    }
                }
            }
            return Ok(new { message = $"Landowner records not found!" });
        }

        [HttpPost("AddLandOwners")]
        public async Task<IActionResult> AddLandOwners([FromBody] List<PrimaryLandowner> Obj)
        {
            if (Obj == null || !Obj.Any())
                return BadRequest("No Data provided");

            //using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var objID in Obj)
                {
                    var existingobj = await _context.Landowners
                        .FirstOrDefaultAsync(p => p.ID == objID.ID);

                    if (existingobj != null)
                    {
                        // var existingLandOwner = await _service.UpdateLandownerAsync(objID.ID, existingobj);
                        var existingLandOwner = await _context.Landowners.FindAsync(objID.ID);
                        if (existingLandOwner == null)
                        {
                            return NotFound();
                        }

                        existingLandOwner.CSN = objID.CSN;
                        //  existingLandOwner.IDNumber = objID.IDNumber;
                        existingLandOwner.TagNumber = objID.TagNumber;
                        existingLandOwner.PANnumber = objID.PANnumber;
                        existingLandOwner.PassportNo = objID.PassportNo;
                        existingLandOwner.LicenseNo = objID.LicenseNo;
                        existingLandOwner.ICEno = objID.ICEno;
                        existingLandOwner.AadharCardId = objID.AadharCardId;
                        existingLandOwner.VoterID = objID.VoterID;
                        existingLandOwner.FirstName = objID.FirstName;
                        existingLandOwner.MiddleName = objID.MiddleName;
                        existingLandOwner.LastName = objID.LastName;
                        existingLandOwner.ShortName = objID.ShortName;
                        existingLandOwner.Gender = objID.Gender;
                        existingLandOwner.BloodGroup = objID.BloodGroup;
                        existingLandOwner.DOB = objID.DOB;
                        existingLandOwner.EmailID = objID.EmailID;
                        existingLandOwner.MobileNo = objID.MobileNo;
                        existingLandOwner.LandLine = objID.LandLine;
                        existingLandOwner.NRD = objID.NRD;
                        existingLandOwner.Building = objID.Building;
                        existingLandOwner.FlatNumber = objID.FlatNumber;
                        existingLandOwner.CardIssueDate = objID.CardIssueDate;
                        existingLandOwner.CardPrintingDate = objID.CardIssueDate;
                        existingLandOwner.LogicalDeleted = objID.LogicalDeleted;
                        existingLandOwner.LandOwnerIssueDate = objID.LandOwnerIssueDate;

                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        _context.Landowners.Add(objID);
                        await _context.SaveChangesAsync();

                        int number = (int)AccessCardHolders.Landowner;
                        objID.IDNumber = number.ToString() + objID.ID.ToString("D5");
                        await _context.SaveChangesAsync();
                    }
                }

                //   await transaction.CommitAsync();

                return Ok(new { message = $"{Obj.Count} Records of Landowners processed successfully" });
            }
            catch (Exception ex)
            {
                //   await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpPost("DeletePLandowner/{id}")]
        public async Task<IActionResult> DeletePLandowner(int id)
        { 
            var existingLandowner = await _context.Landowners.FindAsync(id);
            if (existingLandowner == null)
            {
                return NotFound();
            } 
            existingLandowner.LogicalDeleted = 1;
           
             // Example records
            var filteredRecords = _context.DependentLandowners.Where(r => r.PID == id && r.LogicalDeleted==0).ToList();
           int filteredRecordsCount = 0;
           if (filteredRecords != null)
            {
                if (filteredRecords.Count > 0)
                {
                    filteredRecordsCount = filteredRecords.Count;
                    foreach (var record in filteredRecords)
                    {
                        record.LogicalDeleted = 1; // Update status
                    }
                    //_context.Entry(filteredRecords).State = EntityState.Modified;
                }
            }

           // _context.Entry(existingLandowner).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { Message = $"LandOwner with Dependent {filteredRecordsCount} data Deleted Successfully", IDNumber = existingLandowner.IDNumber });
        }
    }


    [Route("api/[controller]")]
    [ApiController]
    public class DependentLandOwnerController : Controller
    {
        private readonly AppDBContext _context;

        public DependentLandOwnerController(AppDBContext context)
        {
            _context = context;
        }

        // PUT: api/products/5 
        [HttpPost("{UpdateDependentLandOwner}/{id}")]
        public async Task<IActionResult> UpdateDependentLandOwner(int id, [FromBody] DependentLandOwner updatedDLandOwner)
        {
            if (id != updatedDLandOwner.ID)
            {
                return BadRequest("DependentLandOwner ID mismatch.");
            }

            //var existingDependentLandOwner = await _service.UpdateDependentLandownerAsync(updatedDLandOwner.ID, updatedDLandOwner);

            var existingDependentLandOwner = await _context.DependentLandowners.FindAsync(updatedDLandOwner.ID);
            if (existingDependentLandOwner == null)
            {
                return NotFound();
            }
            existingDependentLandOwner.PID = updatedDLandOwner.PID;
            //existingDependentLandOwner.id = existingDependentLandOwner
            existingDependentLandOwner.CSN = updatedDLandOwner.CSN;
            //existingDependentLandOwner.IDNumber = updatedDLandOwner.IDNumber;
            existingDependentLandOwner.TagNumber = updatedDLandOwner.TagNumber;
            existingDependentLandOwner.PANnumber = updatedDLandOwner.PANnumber;
            existingDependentLandOwner.PassportNo = updatedDLandOwner.PassportNo;
            existingDependentLandOwner.LicenseNo = updatedDLandOwner.LicenseNo;
            existingDependentLandOwner.AadharCardId = updatedDLandOwner.AadharCardId;
            existingDependentLandOwner.VoterID = updatedDLandOwner.VoterID;
            existingDependentLandOwner.FirstName = updatedDLandOwner.FirstName;
            existingDependentLandOwner.MiddleName = updatedDLandOwner.MiddleName;
            existingDependentLandOwner.LastName = updatedDLandOwner.LastName;
            existingDependentLandOwner.ShortName = updatedDLandOwner.ShortName;

            existingDependentLandOwner.Gender = updatedDLandOwner.Gender;
            existingDependentLandOwner.BloodGroup = updatedDLandOwner.BloodGroup;
            existingDependentLandOwner.DOB = updatedDLandOwner.DOB;
            existingDependentLandOwner.MobileNo = updatedDLandOwner.MobileNo;
            existingDependentLandOwner.CardIssueDate = updatedDLandOwner.CardIssueDate;
            existingDependentLandOwner.CardPrintingDate = updatedDLandOwner.CardPrintingDate;
            existingDependentLandOwner.LogicalDeleted = updatedDLandOwner.LogicalDeleted;
            existingDependentLandOwner.DependLandOwnerIssueDate = updatedDLandOwner.DependLandOwnerIssueDate;
            _context.Entry(existingDependentLandOwner).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(existingDependentLandOwner);
        }

        [HttpPost("AddDependentLandOwner")]
        public async Task<IActionResult> AddDependentLandOwner([FromBody] DependentLandOwner obj)
        {
            var existingDependentLandOwner = await _context.DependentLandowners.FindAsync(0);
            if (existingDependentLandOwner != null)
            {
                return BadRequest("Dependent LandOwner Exists.");
            }
            _context.Add(obj);
            await _context.SaveChangesAsync();

            int number = (int)AccessCardHolders.DependentLandowner;
            obj.IDNumber = number.ToString() + obj.ID.ToString("D5");
            await _context.SaveChangesAsync();  

            return Ok(new { message = $"Depenent Landowner created successfully", idnumber = obj.IDNumber });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDependentLandowners()
        {
            var DependentLandOwners = await _context.DependentLandowners.Where(p => p.LogicalDeleted == 0).OrderByDescending(p => p.ID).ToListAsync();
            return Ok(DependentLandOwners);
        }

        [HttpPost("AddDependentLandOwners")]
        public async Task<IActionResult> AddDependentLandOwners([FromBody] List<DependentLandOwner> Obj)
        {
            if (Obj == null || !Obj.Any())
                return BadRequest("No Data provided");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {

                foreach (var objID in Obj)
                {
                    var existingobj = await _context.DependentLandowners
                        .FirstOrDefaultAsync(p => p.ID == objID.ID);

                    if (existingobj != null)
                    {
                        //var existingDependentLandOwner = await _service.UpdateDependentLandownerAsync(objID.ID, objID);

                        var existingDependentLandOwner = await _context.DependentLandowners.FindAsync(objID.ID);
                        if (existingDependentLandOwner == null)
                        {
                            return NotFound();
                        }
                        existingDependentLandOwner.PID = objID.PID;
                        //existingDependentLandOwner.id = existingDependentLandOwner
                        existingDependentLandOwner.CSN = objID.CSN;
                        //a existingDependentLandOwner.IDNumber = objID.IDNumber;
                        existingDependentLandOwner.TagNumber = objID.TagNumber;
                        existingDependentLandOwner.PANnumber = objID.PANnumber;
                        existingDependentLandOwner.PassportNo = objID.PassportNo;
                        existingDependentLandOwner.LicenseNo = objID.LicenseNo;
                        existingDependentLandOwner.AadharCardId = objID.AadharCardId;
                        existingDependentLandOwner.VoterID = objID.VoterID;
                        existingDependentLandOwner.FirstName = objID.FirstName;
                        existingDependentLandOwner.MiddleName = objID.MiddleName;
                        existingDependentLandOwner.LastName = objID.LastName;
                        existingDependentLandOwner.ShortName = objID.ShortName;
                        existingDependentLandOwner.Gender = objID.Gender;
                        existingDependentLandOwner.BloodGroup = objID.BloodGroup;
                        existingDependentLandOwner.DOB = objID.DOB;
                        existingDependentLandOwner.MobileNo = objID.MobileNo;
                        existingDependentLandOwner.CardIssueDate = objID.CardIssueDate;
                        existingDependentLandOwner.CardPrintingDate = objID.CardPrintingDate;
                        existingDependentLandOwner.LogicalDeleted = objID.LogicalDeleted;
                        existingDependentLandOwner.DependLandOwnerIssueDate = objID.DependLandOwnerIssueDate;
                        _context.Entry(existingDependentLandOwner).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        // Turn IDENTITY_INSERT ON
                        _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT DependentLandowner ON");
                        _context.DependentLandowners.Add(objID);
                        // Turn IDENTITY_INSERT OFF
                        _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT DependentLandowner OFF");
                        await _context.SaveChangesAsync();

                        int number = (int)AccessCardHolders.DependentLandowner;
                        objID.IDNumber = number.ToString() + objID.ID.ToString("D5");
                        await _context.SaveChangesAsync();
                    }
                }


                await transaction.CommitAsync();

                return Ok(new { message = $"{Obj.Count} Dependent Landowners processed successfully" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteDependentLandOwner(int id)
        {
             var existingDependentLandOwner = await _context.DependentLandowners.FindAsync(id);
            if (existingDependentLandOwner == null)
            {
                return NotFound();
            }

            existingDependentLandOwner.LogicalDeleted = 1;


            _context.Entry(existingDependentLandOwner).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { Message = $"DependentLandOwner Delete Successfully", IDNumber = existingDependentLandOwner.IDNumber });
        }

    }

}
