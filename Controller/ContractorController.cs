using Microsoft.AspNetCore.Mvc;
using Township_API.Models;
using Township_API.Data;
using Microsoft.EntityFrameworkCore;
using Township_API.Services;
using static Township_API.Models.commonTypes;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using System.Runtime.Serialization;

namespace Township_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractorController : Controller
    {
        private readonly AppDBContext _context;
        public ContractorController(AppDBContext context)
        {
            _context = context;
        }

        // PUT: api/products/5
        [HttpPost("{UpdateContractor}/{id}")]
        public async Task<IActionResult> UpdateContractor(int id, [FromBody] Contractor updatedContractor)
        {
            if (id != updatedContractor.ID)
            {
                return BadRequest("Contractor ID mismatch.");
            }

            //var existingContractor = await _service.UpdateDependentLandownerAsync(updatedContractor.ID, updatedContractor);
            var existingContractor = await _context.Contractors.FindAsync(updatedContractor.ID);
            if (existingContractor == null)
            {
                return NotFound();
            }


            existingContractor.CSN = updatedContractor.CSN;
            existingContractor.IDNumber = updatedContractor.IDNumber;
            existingContractor.TagNumber = updatedContractor.TagNumber;
            existingContractor.PANnumber = updatedContractor.PANnumber;
            existingContractor.PassportNo = updatedContractor.PassportNo;
            existingContractor.LicenseNo = updatedContractor.LicenseNo;
            existingContractor.ICEno = updatedContractor.ICEno;
            existingContractor.AadharCardId = updatedContractor.AadharCardId;
            existingContractor.VoterID = updatedContractor.VoterID;
            existingContractor.FirstName = updatedContractor.FirstName;
            existingContractor.MiddleName = updatedContractor.MiddleName;
            existingContractor.LastName = updatedContractor.LastName;
            existingContractor.ShortName = updatedContractor.ShortName;
            existingContractor.Gender = updatedContractor.Gender;
            existingContractor.BloodGroup = updatedContractor.BloodGroup;
            existingContractor.DOB = updatedContractor.DOB;
            existingContractor.EmailID = updatedContractor.EmailID;
            existingContractor.MobileNo = updatedContractor.MobileNo;
            existingContractor.LandLine = updatedContractor.LandLine;
            existingContractor.CardIssueDate = updatedContractor.CardIssueDate;
            existingContractor.CardPrintingDate = updatedContractor.CardPrintingDate;
            existingContractor.RegistrationIssueDate = updatedContractor.RegistrationIssueDate;
            existingContractor.LogicalDeleted = updatedContractor.LogicalDeleted;
            existingContractor.ContactPerson = updatedContractor.ContactPerson;
            existingContractor.RegistrationNumber = updatedContractor.RegistrationNumber;
            existingContractor.ValidFromDate = updatedContractor.ValidFromDate;
            existingContractor.Address = updatedContractor.Address;
            existingContractor.ContactorType = updatedContractor.ContactorType;
            existingContractor.Company = updatedContractor.Company;
            existingContractor.Agency = updatedContractor.Agency;
            existingContractor.NbrContractorLogo = updatedContractor.NbrContractorLogo;
            existingContractor.Nbr = updatedContractor.Nbr;

            _context.Entry(existingContractor).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            //add default access for contractor
            int moduleid = 0;
            var obj = updatedContractor;
            if (!string.IsNullOrEmpty(obj.DefaultNRDAccess))
            {
                var moduleData = await _context.ModuleData.Where(u => u.Name.ToString() == obj.DefaultNRDAccess.ToString() && u.TypeID == (int)ModuleTypes.NRD).FirstOrDefaultAsync();
                if (moduleData != null)
                {
                    moduleid = moduleData.ID;

                    var nrdAccess = await _context._userNRDAccess.Where(u => u.CardHolderID == obj.IDNumber && (DateTime.Now >= obj.ValidFromDate && DateTime.Now <= obj.ValidToDate) && u.isactive == "true").OrderBy(p => p.moduleID).FirstOrDefaultAsync();
                    if (nrdAccess != null)
                    {
                        List<UserNRDAccess> da = new List<UserNRDAccess>();
                        da.Add(new UserNRDAccess
                        {
                            CardHolderID = obj.IDNumber,
                            moduleID = moduleid,
                            sun = "true",
                            mon = "true",
                            tue = "true",
                            thu = "true",
                            fri = "true",
                            sat = "true",
                            wed = "true",
                            isactive = "true",
                            isdeleted = "false",
                            validTillDate = obj.ValidToDate
                        });
                        _context._userNRDAccess.Add(da[0]);
                        await _context.SaveChangesAsync();

                    }
                }
            }


            return Ok(new { Message = $"Contractor Updated Successfully", IDNumber = existingContractor.IDNumber });
        }

        [HttpPost("AddContractor")]
        public async Task<IActionResult> AddContractor([FromBody] Contractor obj)
        {
            //if (!ModelState.IsValid)
            //{
            //    var errors = ModelState
            //        .Where(ms => ms.Value.Errors.Any())
            //        .ToDictionary(
            //            kvp => kvp.Key,
            //            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            //        );

            //    return BadRequest(new { message = "Validation Failed", errors });
            //}

            var existingContractor = await _context.Contractors.FindAsync(0);
            if (existingContractor != null)
            {
                return BadRequest("Contractor Exists.");
            }
            var existsOBJ = await _context.Contractors.Where(p => p.FirstName == obj.FirstName && p.Agency == obj.Agency && p.ID != obj.ID).ToListAsync();
            if (existsOBJ != null)
            {
                if (existsOBJ.Count > 0)
                    return BadRequest("Contractor Name with this Agency is already Exists.");
            }


            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Contractor ON");
            _context.Add(obj);
            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Contractor OFF");

            await _context.SaveChangesAsync();


            int number = (int)AccessCardHolders.contractor;
            obj.IDNumber = number.ToString() + obj.ID.ToString("D5");
            await _context.SaveChangesAsync();


            //Default Access for Contractor
            //add default access for contractor
            int moduleid = 0;
            if (!string.IsNullOrEmpty(obj.DefaultNRDAccess))
            {
                var moduleData = await _context.ModuleData.Where(u => u.Name.ToString() == obj.DefaultNRDAccess.ToString() && u.TypeID == (int)ModuleTypes.NRD).FirstOrDefaultAsync();
                if (moduleData != null)
                {
                    moduleid = moduleData.ID;

                    var nrdAccess = await _context._userNRDAccess.Where(u => u.CardHolderID == obj.IDNumber && (DateTime.Now >= obj.ValidFromDate && DateTime.Now <= obj.ValidToDate) && u.isactive == "true").OrderBy(p => p.moduleID).FirstOrDefaultAsync();
                    if (nrdAccess != null)
                    {
                        List<UserNRDAccess> da = new List<UserNRDAccess>();
                        da.Add(new UserNRDAccess
                        {
                            CardHolderID = obj.IDNumber,
                            moduleID = moduleid,
                            sun = "true",
                            mon = "true",
                            tue = "true",
                            thu = "true",
                            fri = "true",
                            sat = "true",
                            wed = "true",
                            isactive = "true",
                            isdeleted = "false",
                            validTillDate = obj.ValidToDate
                        });
                        _context._userNRDAccess.Add(da[0]);
                        await _context.SaveChangesAsync();

                    }
                }
            }

            return Ok(new { Message = $"Contractor Added successfully", IDNumber = obj.IDNumber });
        }


        // GET: api/products 
        [HttpGet]
        public async Task<IActionResult> GetAllContractors()
        {
            try
            {
                var Contractors = await _context.Contractors.OrderByDescending(p => p.ID).ToListAsync();
                if (Contractors != null)
                {
                    if (Contractors.Count > 0)
                    {
                        //foreach (var res in Contractors)
                        //{
                        //    int mdl = (int)ModuleTypes.ContractorType;
                        //    var nr = await _context.ModuleData.Where(u => u.ID.ToString() == res.ContactorType.ToString() && u.TypeID.ToString() == mdl.ToString()).FirstOrDefaultAsync();
                        //    if (nr != null)
                        //    {
                        //        if (!string.IsNullOrEmpty(nr.Name))
                        //            res.ContractorTypeName = nr.Name.ToString();
                        //    }
                        //    var nrdAccess = await _context._userNRDAccess.Where(u => u.CardHolderID.ToString() == res.IDNumber.ToString() && (DateTime.Now >= res.ValidFromDate && DateTime.Now <= res.ValidToDate) && u.isactive.ToString() == "1").OrderBy(p => p.moduleID).FirstOrDefaultAsync();
                        //    if (nrdAccess != null)
                        //    {
                        //        if (!string.IsNullOrEmpty(nrdAccess.ModuleName))
                        //            res.DefaultNRDAccess = nr.Name.ToString();
                        //    }
                        //}


                        foreach (var res in Contractors)
                        {
                            int mdl = (int)ModuleTypes.ContractorType;
                            var nr = await _context.ModuleData.Where(u => u.ID.ToString() == res.ContactorType.ToString() && u.TypeID.ToString() == mdl.ToString()).FirstOrDefaultAsync();
                            if (nr != null)
                            {
                                if (!string.IsNullOrEmpty(nr.Name))
                                    res.ContractorTypeName = nr.Name.ToString();
                            }
                            var nrdAccess = await _context._userNRDAccess.Where(u => u.CardHolderID.ToString() == res.IDNumber.ToString() && (DateTime.Now >= res.ValidFromDate && DateTime.Now <= res.ValidToDate) && u.isactive.ToString() == "1").OrderBy(p => p.moduleID).FirstOrDefaultAsync();
                            if (nrdAccess != null)
                            {
                                if (!string.IsNullOrEmpty(nrdAccess.ModuleName))
                                    res.DefaultNRDAccess = nr.Name.ToString();
                            }

                            var nbrModule = await _context.ModuleData.Where(u => u.ID.ToString() == res.Nbr.ToString()).FirstOrDefaultAsync();
                            if (nbrModule != null)
                            {
                                if (!string.IsNullOrEmpty(nbrModule.Name))
                                    res.nbrName = nbrModule.Name.ToString();
                            }
                        }
                    }
                }
                return Ok(Contractors);
            }
            catch (Exception ex)
            {
                return BadRequest("Error :" + ex.Message.ToString());
            }
        }

        [HttpGet("GetContractor/{id}")]
        public async Task<IActionResult> GetContractor(int id)
        {
            try
            {
                var Contractors = await _context.Contractors.Where(p => p.ID == id).OrderByDescending(p => p.ID).ToListAsync();

                if (Contractors != null)
                {
                    if (Contractors.Count > 0)
                    {
                        foreach (var res in Contractors)
                        {
                            int mdl = (int)ModuleTypes.ContractorType;
                            var nr = await _context.ModuleData.Where(u => u.ID.ToString() == res.ContactorType.ToString() && u.TypeID.ToString() == mdl.ToString()).FirstOrDefaultAsync();
                            if (nr != null)
                            {
                                if (!string.IsNullOrEmpty(nr.Name))
                                    res.ContractorTypeName = nr.Name.ToString();
                            }
                        }
                    }
                }
                return Ok(Contractors);
            }
            catch (Exception ex)
            {
                return BadRequest("Error :" + ex.Message.ToString());
            }
        }

        // GET: api/products 
        [HttpGet("GetContractorDetails/{ID}")]
        public async Task<IActionResult> GetContractorDetails(int ID)
        {
            var Contractors = await _context.Contractors.Where(p => p.ID == ID).ToListAsync();
            string? IdNumber = Contractors[0].IDNumber;
            if (IdNumber != null)
            {
                try
                {
                    var jsonWrapper = new DependentJsonWrapper
                    {
                        Owners = Contractors,
                        DependentOwners = _context.DependentContractors.Where(p => p.PID == ID).ToList(),
                        Vehicles = _context.Vehicles.Where(p => p.TagUID == IdNumber).ToList(),
                        UserAllAccess = await _context._userAllAccess.Where(p => p.CardHolderID == IdNumber).ToListAsync(),
                        UserNRDAccess = _context._userNRDAccess.Where(p => p.CardHolderID != null && p.CardHolderID.ToString() == IdNumber).ToList(),
                        UserBuildingAccess = _context._userBuildingAccess.Where(p => p.CardHolderID != null && p.CardHolderID.ToString() == IdNumber).ToList(),
                        UserAminitiesAccess = _context._userAmenitiesAccess.Where(p => p.CardHolderID != null && p.CardHolderID.ToString() == IdNumber).ToList()
                    };

                    return Ok(jsonWrapper);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message.ToString());
                }
            }
            return Ok(new { message = $"Contractor records not found!" });
        }


        [HttpPost("{AddContractors}")]
        public async Task<IActionResult> AddContractors([FromBody] List<Contractor> Obj)
        {
            if (Obj == null || !Obj.Any())
                return BadRequest("No Data provided");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var objID in Obj)
                {
                    var existingobj = await _context.Contractors
                        .FirstOrDefaultAsync(p => p.ID == objID.ID);

                    if (existingobj != null)
                    {
                        //var existingContractor = await _service.UpdateContractorAsync(objID.ID, existingobj);
                        var existingContractor = await _context.Contractors.FindAsync(objID.ID);
                        if (existingContractor == null)
                        {
                            return NotFound();
                        }
                        existingContractor.CSN = objID.CSN;
                        existingContractor.IDNumber = objID.IDNumber;
                        existingContractor.TagNumber = objID.TagNumber;
                        existingContractor.PANnumber = objID.PANnumber;
                        existingContractor.PassportNo = objID.PassportNo;
                        existingContractor.LicenseNo = objID.LicenseNo;
                        existingContractor.ICEno = objID.ICEno;
                        existingContractor.AadharCardId = objID.AadharCardId;
                        existingContractor.VoterID = objID.VoterID;
                        existingContractor.FirstName = objID.FirstName;
                        existingContractor.MiddleName = objID.MiddleName;
                        existingContractor.LastName = objID.LastName;
                        existingContractor.ShortName = objID.ShortName;
                        existingContractor.Gender = objID.Gender;
                        existingContractor.BloodGroup = objID.BloodGroup;
                        existingContractor.DOB = objID.DOB;
                        existingContractor.EmailID = objID.EmailID;
                        existingContractor.MobileNo = objID.MobileNo;
                        existingContractor.LandLine = objID.LandLine;
                        existingContractor.CardIssueDate = objID.CardIssueDate;
                        existingContractor.CardPrintingDate = objID.CardPrintingDate;
                        existingContractor.RegistrationIssueDate = objID.RegistrationIssueDate;
                        existingContractor.LogicalDeleted = objID.LogicalDeleted;
                        existingContractor.ContactPerson = objID.ContactPerson;
                        existingContractor.RegistrationNumber = objID.RegistrationNumber;
                        existingContractor.ValidFromDate = objID.ValidFromDate;
                        existingContractor.Address = objID.Address;
                        existingContractor.ContactorType = objID.ContactorType;
                        existingContractor.Company = objID.Company;
                        existingContractor.Agency = objID.Agency;
                        existingContractor.NbrContractorLogo = objID.NbrContractorLogo;
                        existingContractor.Nbr = objID.Nbr;

                        _context.Entry(existingContractor).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        // Turn IDENTITY_INSERT OFF
                        _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Contractor  ON");
                        _context.Contractors.Add(objID); // Turn IDENTITY_INSERT ON
                        _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Contractor OFF");

                        await _context.SaveChangesAsync();


                        int number = (int)AccessCardHolders.contractor;
                        objID.IDNumber = number.ToString() + objID.ID.ToString("D5");
                        await _context.SaveChangesAsync();


                    }
                    await transaction.CommitAsync();
                }
                return Ok(new { message = $"{Obj.Count} Contractor processed successfully" });
            }
            catch (Exception ex)
            {
                //await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPatch("DeletePContractor/{id}")]
        public async Task<IActionResult> DeletePContractor(int id)
        {
            //var existingContractor = await _service.UpdateDependentLandownerAsync(updatedContractor.ID, updatedContractor);
            var existingContractor = await _context.Contractors.FindAsync(id);
            if (existingContractor == null)
            {
                return NotFound();
            }

            existingContractor.LogicalDeleted = 1;
            existingContractor.ValidToDate = DateTime.Now;   //not valid from today

            // Example records
            var filteredRecords = _context.DependentContractors.Where(r => r.PID == id && r.LogicalDeleted == 0).ToList();

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

            //_context.Entry(existingContractor).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { Message = $"Contractor with Dependent {filteredRecordsCount} data Deleted Successfully", IDNumber = existingContractor.IDNumber });
        }

        // GET: api/products 
        //[HttpGet]
        //public async Task<IActionResult> GetAllContractorCompany()
        //{
        //    //var ContractorCompany = new List<ContractorCompany>(); 
        //    //try
        //    //{

        //    //      ContractorCompany =   _context.Contractor_Company.OrderByDescending(p => p.Company).ToList();

        //    //        if (ContractorCompany != null)
        //    //    {
        //    //        if (ContractorCompany.Count > 0)
        //    //        { return Ok(ContractorCompany);
        //    //        }
        //    //    }


        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    return BadRequest("Error :" + ex.Message.ToString());
        //    //}
        //    //
        //    List<ContractorCompany> _ContractorCompany = new List<ContractorCompany>(); 

        //    try
        //    {
        //        _ContractorCompany = await _context.Contractor_Company.OrderBy(p => p.Company).ToListAsync();
        //        return Ok(_ContractorCompany);
        //    }
        //    catch (Exception ex)
        //    {
        //        //     await transaction.RollbackAsync();
        //        return StatusCode(500, new { error = ex.Message });
        //    }

        //}


        //// GET: api/products 
        //[HttpGet]
        //public async Task<IActionResult> GetAllContractorAgency()
        //{
        //    //var ContractorAgency = new List<ContractorAgency>();

        //    //try
        //    //{
        //    //      ContractorAgency =   _context.Contractor_Agency.OrderByDescending(p => p.Agency).ToList();


        //    //    if (ContractorAgency != null)
        //    //    {
        //    //        if (ContractorAgency.Count > 0)
        //    //        {
        //    //            return Ok(ContractorAgency);
        //    //        }
        //    //    }

        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    return BadRequest("Error :" + ex.Message.ToString());
        //    //}             
        //    List<ContractorAgency> _Contractor_Agency = new List<ContractorAgency>();

        //    try
        //    {
        //        _Contractor_Agency = await _context.Contractor_Agency.OrderBy(p => p.Agency).ToListAsync();
        //        return Ok(_Contractor_Agency);
        //    }
        //    catch (Exception ex)
        //    {
        //        //     await transaction.RollbackAsync();
        //        return StatusCode(500, new { error = ex.Message });
        //    }

        //}
    }


    [Route("api/[controller]")]
    [ApiController]
    public class DependentContractorController : Controller
    {
        private readonly AppDBContext _context;

        public DependentContractorController(AppDBContext context)
        {
            _context = context;
        }

        // PUT: api/products/5
        [HttpPost("{UpdateDependentContractor}/{id}")]
        public async Task<IActionResult> UpdateDependentContractor(int id, [FromBody] DependentContractor updatedDependentContractor)
        {
            if (id != updatedDependentContractor.ID)
            {
                return BadRequest("DependentContractor ID mismatch.");
            }

            //var existingDependentContractor = await _service.UpdateDependentContractorAsync(updatedDependentContractor.ID, updatedDependentContractor);
            var existingDependentContractor = await _context.PrimaryResidents.FindAsync(updatedDependentContractor.ID);

            if (existingDependentContractor == null)
            {
                return NotFound();
            }

            _context.Entry(existingDependentContractor).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(existingDependentContractor);
        }


        [HttpPost("AddDependentContractor")]
        public async Task<IActionResult> AddDependentContractor([FromBody] DependentContractor obj)
        {
            var existingDependentContractor = await _context.DependentContractors.FindAsync(0);
            if (existingDependentContractor != null)
            {
                return BadRequest("DependentContractor Exists.");
            }

            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT DependentContractor ON");
            _context.Add(obj);
            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT DependentContractor OFF");
            await _context.SaveChangesAsync();

            int number = (int)AccessCardHolders.DependentContractor;
            obj.IDNumber = number.ToString() + obj.ID.ToString("D4");

            await _context.SaveChangesAsync();


            return Ok();
        }


        // GET: api/products 
        [HttpGet]
        public async Task<IActionResult> GetAllDependentContractors()
        {
            var DependentContractors = await _context.DependentContractors.OrderByDescending(p => p.ID).ToListAsync();
            return Ok(DependentContractors);
        }


        [HttpPost("AddDependentContractors")]
        public async Task<IActionResult> AddDependentContractors([FromBody] List<DependentContractor> Obj)
        {
            if (Obj == null || !Obj.Any())
                return BadRequest("No Data provided");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var objID in Obj)
                {
                    var existingobj = await _context.DependentContractors
                        .FirstOrDefaultAsync(p => p.ID == objID.ID);

                    if (existingobj != null)
                    {
                        //  var existingDependentContractor = await _service.UpdateDependentContractorAsync(objID.ID, existingobj);
                        var existingDependentContractor = await _context.DependentContractors.FindAsync(0);
                        if (existingDependentContractor == null)
                        {
                            return NotFound();
                        }
                    }
                    else
                    {
                        _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT DependentContractor ON");
                        _context.DependentContractors.Add(objID);
                        _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT DependentContractor  OFF");
                        await _context.SaveChangesAsync();

                        int number = (int)AccessCardHolders.DependentContractor;
                        objID.IDNumber = number.ToString() + objID.ID.ToString("D4");

                        await _context.SaveChangesAsync();
                    }
                }



                await transaction.CommitAsync();

                return Ok(new { message = $"{Obj.Count} DependentContractor processed successfully" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteDependentContractor(int id)
        {
            //var existingContractor = await _service.UpdateDependentLandownerAsync(updatedContractor.ID, updatedContractor);
            var existingDependentContractor = await _context.DependentContractors.FindAsync(id);
            if (existingDependentContractor == null)
            {
                return NotFound();
            }

            existingDependentContractor.LogicalDeleted = 1;


            _context.Entry(existingDependentContractor).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { Message = $"DependentContractor Delete Successfully", IDNumber = existingDependentContractor.IDNumber });
        }


    }

}
