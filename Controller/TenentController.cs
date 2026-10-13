using Microsoft.AspNetCore.Mvc;
using Township_API.Models;
using Township_API.Services;
using Township_API.Data;
using Microsoft.EntityFrameworkCore; 
using static Township_API.Models.commonTypes;

namespace Township_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenentController : ControllerBase
    {
        private readonly AppDBContext _context;

        public TenentController(AppDBContext context)
        {
            _context = context;
        }


        // PUT: api/products/5
        //[HttpPost("{UpdateTenent}/{id}")]
        //public async Task<IActionResult> UpdateTenent(int id, [FromBody] PrimaryTenent updatedTenent)
        //{
        //    try
        //    {
        //        if (id != updatedTenent.ID)
        //        {
        //            return BadRequest("Tenent ID mismatch.");
        //        }

        //        //var existingTenent = await _service.UpdatePrimaryTenentAsync(updatedTenent.ID, updatedTenent);
        //        var existingTenent = await _context.PrimaryTenents.FindAsync(updatedTenent.ID);
        //        if (existingTenent == null)
        //        {
        //            return NotFound();
        //        }
        //        // existingTenent.ID = updatedTenent.ID;
        //        existingTenent.RID = updatedTenent.RID;
        //        existingTenent.CSN = updatedTenent.CSN;
        //        // existingTenent.IDNumber = updatedTenent.IDNumber;
        //        existingTenent.TagNumber = updatedTenent.TagNumber;
        //        existingTenent.PANnumber = updatedTenent.PANnumber;
        //        existingTenent.PassportNo = updatedTenent.PassportNo;
        //        existingTenent.LicenseNo = updatedTenent.LicenseNo;
        //        existingTenent.ICEno = updatedTenent.ICEno;
        //        existingTenent.AadharCardId = updatedTenent.AadharCardId;
        //        existingTenent.VoterID = updatedTenent.VoterID;
        //        existingTenent.FirstName = updatedTenent.FirstName;
        //        existingTenent.MiddleName = updatedTenent.MiddleName;
        //        existingTenent.LastName = updatedTenent.LastName;
        //        existingTenent.ShortName = updatedTenent.ShortName;
        //        existingTenent.Gender = updatedTenent.Gender;
        //        existingTenent.BloodGroup = updatedTenent.BloodGroup;
        //        existingTenent.DOB = updatedTenent.DOB;
        //        existingTenent.EmailID = updatedTenent.EmailID;
        //        existingTenent.MobileNo = updatedTenent.MobileNo;
        //        existingTenent.LandLine = updatedTenent.LandLine;
        //        existingTenent.NRD = updatedTenent.NRD;
        //        existingTenent.Building = updatedTenent.Building;
        //        existingTenent.FlatNumber = updatedTenent.FlatNumber;
        //        existingTenent.TenentType = updatedTenent.TenentType;
        //        existingTenent.CardIssueDate = updatedTenent.CardIssueDate;
        //        existingTenent.CardPrintingDate = updatedTenent.CardPrintingDate;
        //        existingTenent.RegistrationIssueDate = updatedTenent.RegistrationIssueDate;
        //        existingTenent.Aggreement_From = updatedTenent.Aggreement_From;
        //        existingTenent.Aggreement_To = updatedTenent.Aggreement_To;
        //        existingTenent.LogicalDeleted = updatedTenent.LogicalDeleted;


        //        _context.Entry(existingTenent).State = EntityState.Modified;
        //        await _context.SaveChangesAsync();
        //        return Ok(new { Message = $"Tenent Updated successfully", IDNumber = existingTenent.IDNumber });

        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest("Data Error : " + ex.Message.ToString().ToString());
        //    }
        //}



        [HttpPost("{UpdateTenent}/{id}")]
        public async Task<IActionResult> UpdateTenent(int id, [FromBody] PrimaryTenent updatedTenent)
        {
            try
            {
                if (id != updatedTenent.ID)
                {
                    return BadRequest("Tenent ID mismatch.");
                }

                //var existingTenent = await _service.UpdatePrimaryTenentAsync(updatedTenent.ID, updatedTenent);
                var existingTenent = await _context.PrimaryTenents.FindAsync(updatedTenent.ID);
                if (existingTenent == null)
                {
                    return NotFound();
                }

                bool isDuplicate = await _context.PrimaryTenents.AnyAsync(p =>
                    p.ID != updatedTenent.ID &&
                    p.LogicalDeleted == 0 &&
                    p.NRD == updatedTenent.NRD &&
                    p.Building == updatedTenent.Building &&
                    p.FlatNumber == updatedTenent.FlatNumber);

                if (isDuplicate)
                {
                    return BadRequest("Tenent already exists for this Neighbourhood, Building and Flat Number.");
                }

                // existingTenent.ID = updatedTenent.ID;
                existingTenent.RID = updatedTenent.RID;
                existingTenent.CSN = updatedTenent.CSN;
                // existingTenent.IDNumber = updatedTenent.IDNumber;
                existingTenent.TagNumber = updatedTenent.TagNumber;
                existingTenent.PANnumber = updatedTenent.PANnumber;
                existingTenent.PassportNo = updatedTenent.PassportNo;
                existingTenent.LicenseNo = updatedTenent.LicenseNo;
                existingTenent.ICEno = updatedTenent.ICEno;
                existingTenent.AadharCardId = updatedTenent.AadharCardId;
                existingTenent.VoterID = updatedTenent.VoterID;
                existingTenent.FirstName = updatedTenent.FirstName;
                existingTenent.MiddleName = updatedTenent.MiddleName;
                existingTenent.LastName = updatedTenent.LastName;
                existingTenent.ShortName = updatedTenent.ShortName;
                existingTenent.Gender = updatedTenent.Gender;
                existingTenent.BloodGroup = updatedTenent.BloodGroup;
                existingTenent.DOB = updatedTenent.DOB;
                existingTenent.EmailID = updatedTenent.EmailID;
                existingTenent.MobileNo = updatedTenent.MobileNo;
                existingTenent.LandLine = updatedTenent.LandLine;
                existingTenent.NRD = updatedTenent.NRD;
                existingTenent.Building = updatedTenent.Building;
                existingTenent.FlatNumber = updatedTenent.FlatNumber;
                existingTenent.TenentType = updatedTenent.TenentType;
                existingTenent.CardIssueDate = updatedTenent.CardIssueDate;
                existingTenent.CardPrintingDate = updatedTenent.CardPrintingDate;
                existingTenent.RegistrationIssueDate = updatedTenent.RegistrationIssueDate;
                existingTenent.Aggreement_From = updatedTenent.Aggreement_From;
                existingTenent.Aggreement_To = updatedTenent.Aggreement_To;
                existingTenent.LogicalDeleted = updatedTenent.LogicalDeleted;


                _context.Entry(existingTenent).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(new { Message = $"Tenent Updated successfully", IDNumber = existingTenent.IDNumber });

            }
            catch (Exception ex)
            {
                return BadRequest("Data Error : " + ex.Message.ToString().ToString());
            }
        }


        //[HttpPost("AddTenent")]
        //public async Task<IActionResult> AddTenent([FromBody] PrimaryTenent obj)
        //{
        //    try
        //    {
        //        var existingTenent = await _context.PrimaryTenents.FindAsync(0);
        //        if (existingTenent != null)
        //        {
        //            return BadRequest("Tenent Exists.");
        //        }
        //        _context.Add(obj);
        //        await _context.SaveChangesAsync();

        //        int number = (int)AccessCardHolders.Tenent;
        //        obj.IDNumber = number.ToString() + obj.ID.ToString("D5");
        //        await _context.SaveChangesAsync();

        //        return Ok(new { Message = $"Tenent Updated successfully", IDNumber = obj.IDNumber });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest("Data Error : " + ex.Message.ToString().ToString());
        //    }
        //}

        [HttpPost("AddTenent")]
        public async Task<IActionResult> AddTenent([FromBody] PrimaryTenent obj)
        {
            try
            {
                var existingTenent = await _context.PrimaryTenents.FindAsync(0);
                if (existingTenent != null)
                {
                    return BadRequest("Tenent Exists.");
                }

                bool isDuplicate = await _context.PrimaryTenents.AnyAsync(p =>
                    p.LogicalDeleted == 0 &&
                    p.NRD == obj.NRD &&
                    p.Building == obj.Building &&
                    p.FlatNumber == obj.FlatNumber);

                if (isDuplicate)
                {
                    return BadRequest("Tenent already exists for this Neighbourhood, Building and Flat Number.");
                }

                _context.Add(obj);
                await _context.SaveChangesAsync();

                int number = (int)AccessCardHolders.Tenent;
                obj.IDNumber = number.ToString() + obj.ID.ToString("D5");
                await _context.SaveChangesAsync();

                return Ok(new { Message = $"Tenent Updated successfully", IDNumber = obj.IDNumber });
            }
            catch (Exception ex)
            {
                return BadRequest("Data Error : " + ex.Message.ToString().ToString());
            }
        }



        // GET: api/products 
        [HttpGet]
        public async Task<IActionResult> GetAllTenents()
        {
            var Tenents = await _context.PrimaryTenents.OrderByDescending(p => p.ID).ToListAsync();
            foreach (var res in Tenents)
            {
                var nr = await _context.ModuleData.Where(u => u.ID.ToString() == res.NRD.ToString()).FirstOrDefaultAsync();
                if (nr != null)
                    res.NRDName = (string)nr.Name.ToString();
                nr = await _context.ModuleData.Where(u => u.ID.ToString() == res.Building.ToString()).FirstOrDefaultAsync();
                if (nr != null)
                    res.BuildingName = (string)nr.Name.ToString();
            }

            return Ok(Tenents);
        }

        // GET: api/products 
        [HttpGet("{ID}")]
        public async Task<IActionResult> GetTenentDetails(int ID)
        {
            var Tenents = await _context.PrimaryTenents.Where(p => p.ID == ID).ToListAsync();
            string? IdNumber = Tenents[0].IDNumber;
            if (IdNumber != null)
            {
                try
                {
                    var jsonWrapper = new DependentJsonWrapper
                    {
                        Owners = Tenents,
                        DependentOwners = await _context.DependentTenents.Where(p => p.PID == ID).ToListAsync(),
                        Vehicles = await _context.Vehicles.Where(p => p.TagUID == IdNumber).ToListAsync(),
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
            return Ok(new { message = $"Tenent records not found!" });
        }


        [HttpPost("{AddTenents}")]
        public async Task<IActionResult> AddTenents([FromBody] List<PrimaryTenent> Obj)
        {


            if (Obj == null || !Obj.Any())
                return BadRequest("No Data provided");

            using var transaction = await _context.Database.BeginTransactionAsync();
            // Turn IDENTITY_INSERT ON
            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT PrimaryTenent ON");

            try
            {
                foreach (var objID in Obj)
                {
                    var existingobj = await _context.PrimaryTenents
                        .FirstOrDefaultAsync(p => p.ID == objID.ID);

                    if (existingobj != null)
                    {
                        //var existingTenent = await _service.UpdatePrimaryTenentAsync(objID.ID, existingobj);
                        var existingTenent = await _context.PrimaryTenents.FindAsync(objID.ID);
                        if (existingTenent == null)
                        {
                            return NotFound();
                        }
                        existingTenent.RID = objID.RID;
                        existingTenent.CSN = objID.CSN;
                        existingTenent.TagNumber = objID.TagNumber;
                        existingTenent.PANnumber = objID.PANnumber;
                        existingTenent.PassportNo = objID.PassportNo;
                        existingTenent.LicenseNo = objID.LicenseNo;
                        existingTenent.ICEno = objID.ICEno;
                        existingTenent.AadharCardId = objID.AadharCardId;
                        existingTenent.VoterID = objID.VoterID;
                        existingTenent.FirstName = objID.FirstName;
                        existingTenent.MiddleName = objID.MiddleName;
                        existingTenent.LastName = objID.LastName;
                        existingTenent.ShortName = objID.ShortName;
                        existingTenent.Gender = objID.Gender;
                        existingTenent.BloodGroup = objID.BloodGroup;
                        existingTenent.DOB = objID.DOB;
                        existingTenent.EmailID = objID.EmailID;
                        existingTenent.MobileNo = objID.MobileNo;
                        existingTenent.LandLine = objID.LandLine;
                        existingTenent.NRD = objID.NRD;
                        existingTenent.Building = objID.Building;
                        existingTenent.FlatNumber = objID.FlatNumber;
                        existingTenent.TenentType = objID.TenentType;
                        existingTenent.CardIssueDate = objID.CardIssueDate;
                        existingTenent.CardPrintingDate = objID.CardPrintingDate;
                        existingTenent.RegistrationIssueDate = objID.RegistrationIssueDate;
                        existingTenent.Aggreement_From = objID.Aggreement_From;
                        existingTenent.Aggreement_To = objID.Aggreement_To;
                        existingTenent.LogicalDeleted = objID.LogicalDeleted;

                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        _context.PrimaryTenents.Add(objID);

                        int number = (int)AccessCardHolders.Tenent;
                        objID.IDNumber = number.ToString() + objID.ID.ToString("D5");
                        await _context.SaveChangesAsync();

                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                // Turn IDENTITY_INSERT OFF
                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT PrimaryTenent  OFF");

                return Ok(new { message = $"Tenent created successfully" });

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("DeletePTenent/{id}")]
        public async Task<IActionResult> DeletePTenent(int id)
        {
            var existingTenant = await _context.PrimaryTenents.FindAsync(id);
            if (existingTenant == null)
            {
                return NotFound();
            }
            existingTenant.LogicalDeleted = 1;

            // Example records
            var filteredRecords = _context.DependentTenents.Where(r => r.PID == id && r.LogicalDeleted == 0).ToList();
            int filteredRecordsCount = 0;
            if (filteredRecords != null)
            {
                if (filteredRecords.Count > 0)
                {
                    filteredRecordsCount = filteredRecords.Count;
                    (from p in _context.DependentTenents
                     where p.PID == id && p.LogicalDeleted == 0
                     select p).ToList()
                     .ForEach(x => x.LogicalDeleted = 1);

                    //foreach (var record in filteredRecords)
                    //{
                    //    record.LogicalDeleted = 1; // Update status                        
                    //}
                    //_context.Entry(filteredRecords).State = EntityState.Modified;
                }
            }

            // _context.Entry(existingLandowner).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { Message = $"Tenant with Dependent {filteredRecordsCount} data Deleted Successfully", IDNumber = existingTenant.IDNumber });
        }

        //[HttpPatch("DeleletPTenent/{id}")]
        //public async Task<IActionResult> DeleteTenent(int id)
        //{
        //    try
        //    {
        //        //if(updatedTenent==null)
        //         var   updatedTenent = await _context.PrimaryTenents.FindAsync(id);

        //        if (updatedTenent == null)
        //        {
        //            return NotFound();
        //        }

        //        updatedTenent.Aggreement_To = DateTime.Now;
        //        updatedTenent.LogicalDeleted = 1;


        //        //_context.Entry(existingTenent).State = EntityState.Modified;

        //        // Get all dependents for this tenant
        //        var filteredRecords = await _context.DependentTenents
        //            .Where(r => r.PID == id && r.LogicalDeleted == 0)
        //            .ToListAsync();

        //        string result = "Tenant data deleted successfully";
        //        int filteredRecordsCount = 0;
        //        if (filteredRecords != null)
        //        {
        //            if (filteredRecords.Count > 0)
        //            {
        //                filteredRecordsCount = filteredRecords.Count;
        //                foreach (var record in filteredRecords)
        //                {
        //                    record.LogicalDeleted = 1;
        //                }
        //                //_context.Entry(filteredRecords).State = EntityState.Modified;
        //                result = $"Tenant with {filteredRecordsCount} dependent records deleted successfully";
        //            }
        //        }
        //         //_context.Entry(updatedTenent).State = EntityState.Modified; 
        //        await _context.SaveChangesAsync();

        //        return Ok(new
        //        {
        //            Message = result,
        //            IDNumber = updatedTenent.IDNumber
        //        });
        //    }
        //    catch (Exception Ex)
        //    {
        //        return Ok(new
        //        {
        //            Message = "Exception : " + Ex.Message.ToString(),
        //            IDNumber = 0
        //        });
        //    }
        //}


    }


    [Route("api/[controller]")]
    [ApiController]
    public class DependentTenentController : Controller
    {
        private readonly AppDBContext _context;
        public DependentTenentController(AppDBContext context)
        {
            _context = context;
        }


        // PUT: api/products/5
        [HttpPost("{UpdateDependentTenent}/{id}")]
        public async Task<IActionResult> UpdateDependentTenent(int id, [FromBody] DependentTenent updatedDTenent)
        {
            try
            {
                if (id != updatedDTenent.ID)
                {
                    return BadRequest("Tenent ID mismatch.");
                }

                //var existingDependentTenent = await _service.UpdateDependentTenentAsync(id, updatedDTenent);
                //nana sir code
                //var existingDependentTenent = await _context.DependentTenents.FindAsync(0);
                //if (existingDependentTenent == null)
                //{
                //    return NotFound();
                //}



                //var existingDependentTenent = await _service.UpdateDependentTenentAsync(id, updatedDTenent);
                //Prashant Code
                var existingDependentTenent = await _context.DependentTenents.FindAsync(id);
                if (existingDependentTenent == null)
                {
                    return NotFound();
                }
                existingDependentTenent.PID = updatedDTenent.PID;
                existingDependentTenent.CSN = updatedDTenent.CSN;
                existingDependentTenent.TagNumber = updatedDTenent.TagNumber;
                existingDependentTenent.PANnumber = updatedDTenent.PANnumber;
                existingDependentTenent.PassportNo = updatedDTenent.PassportNo;
                existingDependentTenent.LicenseNo = updatedDTenent.LicenseNo;
                existingDependentTenent.ICEno = updatedDTenent.ICEno;
                existingDependentTenent.AadharCardId = updatedDTenent.AadharCardId;
                existingDependentTenent.VoterID = updatedDTenent.VoterID;
                existingDependentTenent.FirstName = updatedDTenent.FirstName;
                existingDependentTenent.MiddleName = updatedDTenent.MiddleName;
                existingDependentTenent.LastName = updatedDTenent.LastName;
                existingDependentTenent.ShortName = updatedDTenent.ShortName;
                existingDependentTenent.Gender = updatedDTenent.Gender;
                existingDependentTenent.BloodGroup = updatedDTenent.BloodGroup;
                existingDependentTenent.DOB = updatedDTenent.DOB;
                existingDependentTenent.EmailID = updatedDTenent.EmailID;
                existingDependentTenent.MobileNo = updatedDTenent.MobileNo;
                existingDependentTenent.LandLine = updatedDTenent.LandLine;
                existingDependentTenent.Building = updatedDTenent.Building;
                existingDependentTenent.FlatNumber = updatedDTenent.FlatNumber;
                existingDependentTenent.CardIssueDate = updatedDTenent.CardIssueDate;
                existingDependentTenent.CardPrintingDate = updatedDTenent.CardPrintingDate;
                existingDependentTenent.RegistrationIssueDate = updatedDTenent.RegistrationIssueDate;
                existingDependentTenent.Aggreement_From = updatedDTenent.Aggreement_From;
                existingDependentTenent.Aggreement_To = updatedDTenent.Aggreement_To;
                existingDependentTenent.LogicalDeleted = updatedDTenent.LogicalDeleted;
                await _context.SaveChangesAsync();
                _context.Entry(existingDependentTenent).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new { message = $"{existingDependentTenent.ID} Depenent Tenent updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest("Data Error : " + ex.Message.ToString().ToString());
            }
        }


        [HttpPost("AddDependentTenent")]
        public async Task<IActionResult> AddDependentTenent([FromBody] DependentTenent obj)
        {
            try
            {
                var existingDependentTenent = await _context.DependentTenents.FindAsync(0);
                if (existingDependentTenent != null)
                {
                    return BadRequest("DependentTenent Exists.");
                }

                // Turn IDENTITY_INSERT OFF
                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT DependentTenent  ON");
                _context.Add(obj);
                await _context.SaveChangesAsync();

                // Turn IDENTITY_INSERT OFF
                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT DependentTenent  OFF");
                int number = (int)AccessCardHolders.DependentTenent;
                obj.IDNumber = number.ToString() + obj.ID.ToString("D5");
                await _context.SaveChangesAsync();
                return Ok(new { message = $"Depenent Tenent created successfully", idnumber = obj.IDNumber });

            }
            catch (Exception ex)
            {

                return BadRequest("Data Error : " + ex.Message.ToString().ToString());
            }
        }


        // GET: api/products 
        [HttpGet]
        public async Task<IActionResult> GetAllDependentTenents()
        {
            var DependentTenents = await _context.DependentTenents.OrderByDescending(p => p.ID).ToListAsync();
            return Ok(DependentTenents);
        }



        [HttpPost("AddDependentTenents")]
        public async Task<IActionResult> AddTenents([FromBody] List<DependentTenent> Obj)
        {
            if (Obj == null || !Obj.Any())
                return BadRequest("No Data provided");

            using var transaction = await _context.Database.BeginTransactionAsync();
            // Turn IDENTITY_INSERT ON
            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT DependentTenent ON");

            try
            {
                foreach (var objID in Obj)
                {
                    var existingobj = await _context.DependentTenents
                        .FirstOrDefaultAsync(p => p.ID == objID.ID);

                    if (existingobj != null)
                    {
                        //var existingTenent = await _service.UpdateDependentTenentAsync(objID.ID, existingobj);
                        var existingTenent = await _context.DependentTenents.FindAsync(objID.ID);
                        if (existingTenent == null)
                        {
                            return NotFound();
                        }
                        existingobj.PID = objID.PID;
                        existingobj.CSN = objID.CSN;
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
                        existingobj.Aggreement_From = objID.Aggreement_From;
                        existingobj.Aggreement_To = objID.Aggreement_To;
                        existingobj.LogicalDeleted = objID.LogicalDeleted;
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        _context.DependentTenents.Add(objID);
                        int number = (int)AccessCardHolders.DependentTenent;
                        objID.IDNumber = number.ToString() + objID.ID.ToString("D5");
                        await _context.SaveChangesAsync();
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                // Turn IDENTITY_INSERT OFF
                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT DependentTenent  OFF");

                return Ok(new { message = $"{Obj.Count} DependentTenent processed successfully" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteDependentTenent(int id)
        {
            var existingDependentTenent = await _context.DependentTenents.FindAsync(id);
            if (existingDependentTenent == null)
            {
                return NotFound();
            }

            existingDependentTenent.LogicalDeleted = 1;


            _context.Entry(existingDependentTenent).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { Message = $"DependentTenent Delete Successfully", IDNumber = existingDependentTenent.IDNumber });
        }

    }

}
