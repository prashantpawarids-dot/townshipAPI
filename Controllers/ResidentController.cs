using Microsoft.AspNetCore.Mvc;
using Township_API.Models;
using Township_API.Services;
using Township_API.Data;
using Microsoft.EntityFrameworkCore;
using static Township_API.Models.commonTypes; 
using System;

namespace Township_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResidentController : Controller
    {
        private readonly AppDBContext _context;
        public ResidentController(AppDBContext context)
        {
            _context = context;
        }

        // PUT: api/UpdateResident/5
        [HttpPost("UpdateResident/{id}")]
        public async Task<IActionResult> UpdateResident(int id, [FromBody] PrimaryResident updatedResident)
        {

            try
            {
                if (id != updatedResident.ID)
                {
                    return BadRequest("Resident ID mismatch.");
                }

                var existingResident = await _context.PrimaryResidents.FindAsync(id);
                if (existingResident == null)
                {
                    return NotFound();
                }
                //flatnumber validation                
                var existing = await _context.PrimaryResidents.Where(p => p.FlatNumber == updatedResident.FlatNumber && p.Building == updatedResident.Building && p.ID != updatedResident.ID).ToListAsync();
                if (existing != null)
                {
                    if (existing.Count() > 0)
                        return BadRequest("FlatNumber is already register with other owner!");
                }

                //var existingResident = await _service.UpdatePrimaryResidentAsync(updatedResident.ID, updatedResident); 
                //existingResident.ID = updatedResident.ID;
                existingResident.CSN = updatedResident.CSN;
                // existingResident.IDNumber = updatedResident.IDNumber;
                existingResident.TagNumber = updatedResident.TagNumber;
                existingResident.PANnumber = updatedResident.PANnumber;
                existingResident.PassportNo = updatedResident.PassportNo;
                existingResident.LicenseNo = updatedResident.LicenseNo;
                existingResident.ICEno = updatedResident.ICEno;
                existingResident.AadharCardId = updatedResident.AadharCardId;
                existingResident.VoterID = updatedResident.VoterID;
                existingResident.FirstName = updatedResident.FirstName;
                existingResident.MiddleName = updatedResident.MiddleName;
                existingResident.LastName = updatedResident.LastName;
                existingResident.ShortName = updatedResident.ShortName;
                existingResident.Gender = updatedResident.Gender;
                existingResident.BloodGroup = updatedResident.BloodGroup;
                existingResident.DOB = updatedResident.DOB;
                existingResident.EmailID = updatedResident.EmailID;
                existingResident.MobileNo = updatedResident.MobileNo;
                existingResident.LandLine = updatedResident.LandLine;
                existingResident.NRD = updatedResident.NRD;
                existingResident.Building = updatedResident.Building;
                existingResident.FlatNumber = updatedResident.FlatNumber;
                existingResident.CardIssueDate = updatedResident.CardIssueDate;
                existingResident.CardPrintingDate = updatedResident.CardPrintingDate;
                existingResident.RegistrationIssueDate = updatedResident.RegistrationIssueDate;


                _context.Entry(existingResident).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new { Message = $"Resident updated successfully", IDNumber = existingResident.IDNumber });

            }
            catch (Exception ex)
            {
                return BadRequest();

            }
        }


        [HttpPost("AddResident")]
        public async Task<IActionResult> AddResident([FromBody] PrimaryResident obj)
        {
            var existingResident = await _context.PrimaryResidents.FindAsync(0);
            if (existingResident != null)
            {
                return BadRequest("Resident Exists.");
            }
            // Turn IDENTITY_INSERT ON
            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT PrimaryResident ON");
            _context.Add(obj);
            // Turn IDENTITY_INSERT OFF
            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT PrimaryResident OFF");
            await _context.SaveChangesAsync();

            int number = (int)AccessCardHolders.Resident;
            obj.IDNumber = number.ToString() + obj.ID.ToString("D5");
            await _context.SaveChangesAsync();
            return Ok(new { Message = $"Resident created successfully", IDNumber = obj.IDNumber });

        }


        // GET: api/products 
        [HttpGet]
        public async Task<IActionResult> GetAllResidents()
        {
            var Residents = await _context.PrimaryResidents.OrderByDescending(p => p.ID).ToListAsync();
            if (Residents != null && Residents.Count>0)
            {
                foreach (var res in Residents)
                {
                    var nr = await _context.ModuleData.Where(u => u.ID.ToString() == res.NRD.ToString()).FirstOrDefaultAsync();
                    if (nr != null)
                        res.NRDName = (string)nr.Name.ToString();

                    var bld = await _context.ModuleData.Where(u => u.ID.ToString() == res.Building.ToString()).FirstOrDefaultAsync();
                    if (bld != null)
                        res.BuildingName = (string)bld.Name.ToString();
                }
            }


            return Ok(Residents);
        }


        // GET: api/products 

        [HttpGet("{ID}")]
        public async Task<IActionResult> GetResidentDetails(int ID)
        {
            var Residents = await _context.PrimaryResidents.Where(p => p.ID == ID).ToListAsync();
            string? IdNumber = Residents[0].IDNumber;
            if (IdNumber != null)
            {
                try
                {
                    var jsonWrapper = new DependentJsonWrapper
                    {
                        Owners = Residents,
                        DependentOwners =await _context.DependentResidents.Where(p => p.PID == ID).ToListAsync(),
                        Vehicles = await _context.Vehicles.Where(p => p.TagUID == IdNumber).ToListAsync() ,
                        UserAllAccess = await _context._userAllAccess.Where(p => p.CardHolderID == IdNumber).ToListAsync(),
                        UserNRDAccess = await _context._userNRDAccess.Where(p => p.CardHolderID != null && p.CardHolderID.ToString() == IdNumber).ToListAsync(),
                         UserBuildingAccess = await _context._userBuildingAccess.Where(p => p.CardHolderID != null && p.CardHolderID.ToString() == IdNumber).ToListAsync(),
                         UserAminitiesAccess = await _context._userAmenitiesAccess.Where(p => p.CardHolderID != null && p.CardHolderID.ToString() == IdNumber).ToListAsync()
                   };

                    return Ok(jsonWrapper);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message.ToString());
                }
            }
            return Ok(new { message = $"Resident records not found!" });
        }

        [HttpPost("AddPrimaryResidents")]
        public async Task<IActionResult> AddPrimaryResidents([FromBody] List<PrimaryResident> Obj)
        {
            if (Obj == null || !Obj.Any())
                return BadRequest("No Data provided");

            using var transaction = await _context.Database.BeginTransactionAsync();


            try
            {
                foreach (var objID in Obj)
                {
                    var existingobj = await _context.PrimaryResidents
                        .FirstOrDefaultAsync(p => p.ID == objID.ID);

                    if (existingobj != null)
                    {   
                       existingobj.CSN = objID.CSN;
                       //existingobj.IDNumber = objID.IDNumber;
                       existingobj.TagNumber = objID.TagNumber;
                       existingobj.PANnumber = objID.PANnumber;
                       existingobj.PassportNo = objID.PassportNo;
                       existingobj.LicenseNo = objID.LicenseNo;
                       existingobj.ICEno = objID.ICEno;
                       existingobj.AadharCardId = objID.AadharCardId;
                       existingobj.VoterID = objID.VoterID;
                       existingobj.FirstName = objID.FirstName;
                       existingobj.MiddleName = objID.MiddleName;
                       existingobj.LastName = objID.LastName;
                       existingobj.ShortName = objID.ShortName;
                       existingobj.Gender = objID.Gender;
                       existingobj.BloodGroup = objID.BloodGroup;
                       existingobj.DOB = objID.DOB;
                       existingobj.EmailID = objID.EmailID;
                       existingobj.MobileNo = objID.MobileNo;
                       existingobj.LandLine = objID.LandLine;
                        existingobj.NRD = objID.NRD;
                        existingobj.Building = objID.Building;
                       existingobj.FlatNumber = objID.FlatNumber;
                       existingobj.CardIssueDate = objID.CardIssueDate;
                       existingobj.CardPrintingDate = objID.CardPrintingDate;
                       existingobj.RegistrationIssueDate = objID.RegistrationIssueDate;

                        await _context.SaveChangesAsync();

                    }
                    else
                    {
                        // Turn IDENTITY_INSERT ON
                        _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT PrimaryResident ON");
                        _context.PrimaryResidents.Add(objID);
                        // Turn IDENTITY_INSERT OFF
                        _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT PrimaryResident  OFF");
                        await _context.SaveChangesAsync();

                        int number = (int)AccessCardHolders.Resident;
                        objID.IDNumber = number.ToString() + objID.ID.ToString("D5");
                        await _context.SaveChangesAsync();

                    }
                }

                await transaction.CommitAsync();

                return Ok(new { message = $"{Obj.Count} PrimaryResident processed successfully" });
            }
            catch (Exception ex)
            {
                //await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }



    [Route("api/[controller]")]
    [ApiController]
    public class DependentResidentController : Controller
    {
        private readonly AppDBContext _context;

        public DependentResidentController(AppDBContext context)
        {
            _context = context;
        }

        // PUT: api/products/5
        [HttpPost("{UpdateDependentResident}/{id}")]
        public async Task<IActionResult> UpdateDependentResident(int id, [FromBody] DependentResident updatedDependentResident)
        {
            if (id != updatedDependentResident.ID)
            {
                return BadRequest("DependentResident ID mismatch.");
            }

            //var existingDependentResident = await _service.UpdateDependentResidentAsync(updatedDependentResident.ID, updatedDependentResident);
            var existingDependentResident = await _context.DependentResidents.FindAsync(updatedDependentResident.ID);
              if (existingDependentResident == null)
            {
                return NotFound();
            } 
            existingDependentResident.PID = updatedDependentResident.PID;
            existingDependentResident.CSN = updatedDependentResident.CSN;
           // existingDependentResident.IDNumber = updatedDependentResident.IDNumber;
            existingDependentResident.TagNumber = updatedDependentResident.TagNumber;
            existingDependentResident.PANnumber = updatedDependentResident.PANnumber;
            existingDependentResident.PassportNo = updatedDependentResident.PassportNo;
            existingDependentResident.LicenseNo = updatedDependentResident.LicenseNo;
            existingDependentResident.ICEno = updatedDependentResident.ICEno;
            existingDependentResident.AadharCardId = updatedDependentResident.AadharCardId;
            existingDependentResident.VoterID = updatedDependentResident.VoterID;
            existingDependentResident.FirstName = updatedDependentResident.FirstName;
            existingDependentResident.MiddleName = updatedDependentResident.MiddleName;
            existingDependentResident.LastName = updatedDependentResident.LastName;
            existingDependentResident.ShortName = updatedDependentResident.ShortName;
            existingDependentResident.Gender = updatedDependentResident.Gender;
            existingDependentResident.BloodGroup = updatedDependentResident.BloodGroup;
            existingDependentResident.DOB = updatedDependentResident.DOB;
            existingDependentResident.EmailID = updatedDependentResident.EmailID;
            existingDependentResident.MobileNo = updatedDependentResident.MobileNo;
            existingDependentResident.LandLine = updatedDependentResident.LandLine;
            existingDependentResident.Building = updatedDependentResident.Building;
            existingDependentResident.FlatNumber = updatedDependentResident.FlatNumber;
            existingDependentResident.CardIssueDate = updatedDependentResident.CardIssueDate;
            existingDependentResident.CardPrintingDate = updatedDependentResident.CardPrintingDate;
            existingDependentResident.RegistrationIssueDate = updatedDependentResident.RegistrationIssueDate;
            existingDependentResident.LogicalDeleted = updatedDependentResident.LogicalDeleted; 

            await _context.SaveChangesAsync();

            return Ok(existingDependentResident);
        }


        [HttpPost("AddDependentResident")]
        public async Task<IActionResult> AddDependentResident([FromBody] DependentResident obj)
        {
            var existingDependentResident = await _context.DependentResidents.FindAsync(0);
            if (existingDependentResident != null)
            {
                return BadRequest("DependentResident Exists.");
            }

            // Turn IDENTITY_INSERT OFF
            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT DependentResident  ON");
            _context.Add(obj);
            // Turn IDENTITY_INSERT OFF
            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT DependentResident  OFF");
            await _context.SaveChangesAsync(); 

            int number = (int)AccessCardHolders.DependentResident;
            obj.IDNumber = number.ToString() + obj.ID.ToString("D5");
            await _context.SaveChangesAsync();
            return Ok(new { message = $"Depenent Resident created successfully", idnumber = obj.IDNumber });

        }


        // GET: api/products 
        [HttpGet]
        public async Task<IActionResult> GetAllDependentResidents()
        {
            var DependentResidents = await _context.DependentResidents.OrderByDescending(p => p.ID).ToListAsync();
            return Ok(DependentResidents);
        }


        [HttpPost("AddDependentResidents")]
        public async Task<IActionResult> AddDependentResidents([FromBody] List<DependentResident> Obj)
        {
            if (Obj == null || !Obj.Any())
                return BadRequest("No Data provided");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var objID in Obj)
                {
                    var existingobj = await _context.DependentResidents
                        .FirstOrDefaultAsync(p => p.ID == objID.ID);

                    if (existingobj != null)
                    {
                        //  var existingdependentResident = await _service.UpdateDependentResidentAsync(objID.ID, existingobj);
                        //var existingdependentResident = await _context.DependentResidents.FindAsync(0);
                        //if (existingdependentResident == null)
                        //{
                        //    return NotFound();
                        //}
                        existingobj.ID = objID.ID;
                        existingobj.PID = objID.PID;
                        existingobj.CSN = objID.CSN;
                     //   existingobj.IDNumber = objID.IDNumber;
                        existingobj.TagNumber = objID.TagNumber;
                        existingobj.PANnumber = objID.PANnumber;
                        existingobj.PassportNo = objID.PassportNo;
                        existingobj.LicenseNo = objID.LicenseNo;
                        existingobj.ICEno = objID.ICEno;
                        existingobj.AadharCardId = objID.AadharCardId;
                        existingobj.VoterID = objID.VoterID;
                        existingobj.FirstName = objID.FirstName;
                        existingobj.MiddleName = objID.MiddleName;
                        existingobj.LastName = objID.LastName;
                        existingobj.ShortName = objID.ShortName;
                        existingobj.Gender = objID.Gender;
                        existingobj.BloodGroup = objID.BloodGroup;
                        existingobj.DOB = objID.DOB;
                        existingobj.EmailID = objID.EmailID;
                        existingobj.MobileNo = objID.MobileNo;
                        existingobj.LandLine = objID.LandLine;
                        existingobj.Building = objID.Building;
                        existingobj.FlatNumber = objID.FlatNumber;
                        existingobj.CardIssueDate = objID.CardIssueDate;
                        existingobj.CardPrintingDate = objID.CardPrintingDate;
                        existingobj.RegistrationIssueDate = objID.RegistrationIssueDate;
                        existingobj.LogicalDeleted = objID.LogicalDeleted;

                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        // Turn IDENTITY_INSERT ON
                        _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT DependentResident ON");
                        _context.DependentResidents.Add(objID);
                        // Turn IDENTITY_INSERT OFF
                        _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT DependentResident  OFF");
                        await _context.SaveChangesAsync();

                        int number = (int)AccessCardHolders.DependentResident;
                        objID.IDNumber = number.ToString() + objID.ID.ToString("D5");
                        await _context.SaveChangesAsync();
                    }
                }

                await transaction.CommitAsync();

                return Ok(new { message = $"{Obj.Count} DependentResident processed successfully" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }


    }





}
