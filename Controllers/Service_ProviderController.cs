using Microsoft.AspNetCore.Mvc;
using Township_API.Data;
using Township_API.Services;
using Township_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Numerics;
using static Township_API.Models.commonTypes;
using System.Linq;
using Microsoft.IdentityModel.Tokens;

namespace Township_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Service_ProviderController : Controller
    {
        private readonly AppDBContext _context;
        public Service_ProviderController(AppDBContext context)
        {
            _context = context;
        }

        // PUT: api/products/5
        [HttpPost("UpdateServiceProvider")]
        public async Task<IActionResult> UpdateServiceProvider([FromBody] Service_Provider updatedServiceProvider)
        {
            try
            {
                //if (id != updatedServiceProvider.ID)
                //{
                //    return BadRequest("ServiceProvider ID mismatch.");
                //}

                //var existingServiceProvider = await _service.UpdateServiceProviderAsync(updatedServiceProvider.ID, updatedServiceProvider);
                var existingServiceProvider = await _context.ServiceProviders.FindAsync(updatedServiceProvider.ID);

                if (existingServiceProvider == null)
                {
                    return NotFound();
                }
                ///flatnumber validation                
                var existing = await _context.ServiceProviders.Where(p => p.ID != updatedServiceProvider.ID && p.FirstName == updatedServiceProvider.FirstName).ToListAsync();
                if (existing != null)
                {
                    if (existing.Count() > 0)
                        return BadRequest("Service provider is already register with other owner!");
                }
                //               existingServiceProvider.IDNumber = updatedServiceProvider.IDNumber;
                existingServiceProvider.FirstName = updatedServiceProvider.FirstName;
                existingServiceProvider.MiddleName = updatedServiceProvider.MiddleName;
                existingServiceProvider.LastName = updatedServiceProvider.LastName;
                existingServiceProvider.ShortName = updatedServiceProvider.ShortName;
                existingServiceProvider.ServiceTypeId = updatedServiceProvider.ServiceTypeId;
                existingServiceProvider.ICEno = updatedServiceProvider.ICEno;
                existingServiceProvider.AadharCardId = updatedServiceProvider.AadharCardId;
                existingServiceProvider.VoterID = updatedServiceProvider.VoterID;
                existingServiceProvider.EmailID = updatedServiceProvider.EmailID;
                existingServiceProvider.Gender = updatedServiceProvider.Gender;
                existingServiceProvider.BloodGroup = updatedServiceProvider.BloodGroup;
                existingServiceProvider.MobileNo = updatedServiceProvider.MobileNo;
                existingServiceProvider.CSN = updatedServiceProvider.CSN;
                existingServiceProvider.Doj = updatedServiceProvider.Doj;
                existingServiceProvider.CardIssueDate = updatedServiceProvider.CardIssueDate;
                existingServiceProvider.CardPrintingDate = updatedServiceProvider.CardPrintingDate;
                existingServiceProvider.ValidFromDate = updatedServiceProvider.ValidFromDate;
                existingServiceProvider.ValidToDate = updatedServiceProvider.ValidToDate;
                existingServiceProvider.Address = updatedServiceProvider.Address;
                existingServiceProvider.Refrence1ID = updatedServiceProvider.Refrence1ID;
                existingServiceProvider.Refrence1Name = updatedServiceProvider.Refrence1Name;
                existingServiceProvider.Refrence1Mobile = updatedServiceProvider.Refrence1Mobile;
                existingServiceProvider.Refrence2Name = updatedServiceProvider.Refrence2Name;
                existingServiceProvider.Refrence2Mobile = updatedServiceProvider.Refrence2Mobile;
                existingServiceProvider.Refrence2Details = updatedServiceProvider.Refrence2Details;
                existingServiceProvider.isActive = updatedServiceProvider.isActive;
                existingServiceProvider.isDeleted = updatedServiceProvider.isDeleted;
                existingServiceProvider.CreatedOn = updatedServiceProvider.CreatedOn;
                existingServiceProvider.CreatedBy = updatedServiceProvider.CreatedBy;
                existingServiceProvider.UpdatedOn = updatedServiceProvider.UpdatedOn;
                existingServiceProvider.UpdatedBy = updatedServiceProvider.UpdatedBy;

                _context.Entry(existingServiceProvider).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(new { Message = $"ServiceProvided Added successfully", IDNumber = existingServiceProvider.IDNumber });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpPost("AddServiceProvider")]
        public async Task<IActionResult> AddServiceProvider([FromBody] Service_Provider obj)
        {
            try
            {
                var existingServiceProvider = await _context.ServiceProviders.FindAsync(0);
                if (existingServiceProvider != null)
                {
                    return BadRequest("ServiceProvider Exists.");
                }

                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblServiceProvider  ON");
                if (obj.ValidFromDate is null)
                {
                    obj.ValidFromDate = DateTime.Now;
                    obj.ValidToDate = DateTime.Now.AddYears(25);
                }
                if (obj.Doj is null)
                    obj.Doj = Convert.ToDateTime("01/01/2000");


                _context.Add(obj);
                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblServiceProvider  OFF");
                await _context.SaveChangesAsync();

                int number = (int)AccessCardHolders.ServiceProvider;
                obj.IDNumber = number.ToString() + obj.ID.ToString("D5");

                await _context.SaveChangesAsync();

                return Ok(new { message = $"ServiceProvider processed successfully", IDnumber = obj.IDNumber });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        // GET: api/products 
        [HttpGet("GetAllResidentTenents")]
        public async Task<IActionResult> GetAllResidentTenents(string buildingid = null, string flatNumber = null)
        {

            var Residents = await _context.PrimaryResidents.Where(p => p.Building.ToString() == (String.IsNullOrEmpty(buildingid) ? p.Building.ToString() : buildingid.ToString()) && p.FlatNumber == (String.IsNullOrEmpty(flatNumber) ? p.FlatNumber : flatNumber)).Select(p => new { p.ShortName, p.MobileNo }).ToListAsync();
            var Tenants = await _context.PrimaryTenents.Where(p => p.Building.ToString() == (String.IsNullOrEmpty(buildingid) ? p.Building.ToString() : buildingid.ToString()) && p.FlatNumber == (String.IsNullOrEmpty(flatNumber) ? p.FlatNumber : flatNumber)).Select(p => new { p.ShortName, p.MobileNo }).ToListAsync();
            var result = new
            {
                residents = Residents,
                tenants = Tenants
            };

            return Ok(result);
        }

        // GET: api/products 
        [HttpGet("GetCurrentResidents/")]
        public async Task<IActionResult> GetCurrentResidents(string building = null, string flatNumber = null)
        {

            object result;
            if(  building != null && flatNumber != null)
                result = await _context._CurrentResidents.Where(p=>p.Building == building  && p.FlatNumber ==  flatNumber) .ToListAsync();
            else
                result = await _context._CurrentResidents.ToListAsync();

            //var result = await _context._CurrentResidents.Where(p=>p.Building== building  && p.FlatNumber== flatNumber ).ToListAsync();

            return Ok(result);
        }


        // GET: api/products 
        [HttpGet("GetAllServiceProviders")]
        public async Task<IActionResult> GetAllServiceProviders()
        {
            var ServiceProviders = await _context.ServiceProviders.Where(p => p.isActive == 1 && p.isDeleted == 0).OrderByDescending(p => p.ID).ToListAsync();
            foreach (var res in ServiceProviders)
            {
                var nr = await _context.ModuleData.Where(u => u.ID.ToString() == res.ServiceTypeId.ToString()).FirstOrDefaultAsync();
                if (nr != null)
                    res.serviceType = (string)nr.Name.ToString();
            }

            return Ok(ServiceProviders);
        }

        // GET: api/products 
        [HttpGet("{ID}")]
        public async Task<IActionResult> GetService_providerDetails(int ID)
        {
            var sProviders = await _context.ServiceProviders.Where(p => p.ID == ID).ToListAsync();
            string? IdNumber = sProviders[0].IDNumber;
            if (IdNumber != null)
            {
                try
                {
                    var jsonWrapper = new DependentJsonWrapper
                    {
                        Owners = sProviders,
                        DependentOwners = null,
                        Vehicles = _context.Vehicles.Where(p => p.TagUID == IdNumber).ToList(),
                        UserAllAccess = await _context._userAllAccess.Where(p => p.CardHolderID == IdNumber).ToListAsync()
                        //UserNRDAccess = _context._userNRDAccess.Where(p => p.CardHolderID != null && p.CardHolderID.ToString() == IdNumber).ToList(),
                        //UserBuildingAccess = _context._userBuildingAccess.Where(p => p.CardHolderID != null && p.CardHolderID.ToString() == IdNumber).ToList(),
                        //UserAminitiesAccess = _context._userAmenitiesAccess.Where(p => p.CardHolderID != null && p.CardHolderID.ToString() == IdNumber).ToList()
                    };

                    return Ok(jsonWrapper);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message.ToString());
                }
            }
            return Ok(new { message = $"Service provider records not found!" });
        }


        [HttpPost("AddServiceProviders")]
        public async Task<IActionResult> AddServiceProviders([FromBody] List<Service_Provider> Obj)
        {
            if (Obj == null || !Obj.Any())
                return BadRequest("No Data provided");


            try
            {
                foreach (var objID in Obj)
                {
                    var existingobj = await _context.ServiceProviders
                        .FirstOrDefaultAsync(p => p.ID == objID.ID);

                    if (existingobj != null)
                    {
                        //var existingServiceProvider = await _service.UpdateServiceProviderAsync(objID.ID, existingobj);
                        var existingServiceProvider = await _context.ServiceProviders.FindAsync(objID.ID);
                        if (existingServiceProvider == null)
                        {
                            return NotFound();
                        }

                        existingServiceProvider.IDNumber = objID.IDNumber;
                        existingServiceProvider.FirstName = objID.FirstName;
                        existingServiceProvider.MiddleName = objID.MiddleName;
                        existingServiceProvider.LastName = objID.LastName;
                        existingServiceProvider.ShortName = objID.ShortName;
                        existingServiceProvider.ServiceTypeId = objID.ServiceTypeId;
                        existingServiceProvider.ICEno = objID.ICEno;
                        existingServiceProvider.AadharCardId = objID.AadharCardId;
                        existingServiceProvider.VoterID = objID.VoterID;
                        existingServiceProvider.EmailID = objID.EmailID;
                        existingServiceProvider.Gender = objID.Gender;
                        existingServiceProvider.BloodGroup = objID.BloodGroup;
                        existingServiceProvider.MobileNo = objID.MobileNo;
                        existingServiceProvider.CSN = objID.CSN;
                        existingServiceProvider.Doj = objID.Doj;
                        existingServiceProvider.CardIssueDate = objID.CardIssueDate;
                        existingServiceProvider.CardPrintingDate = objID.CardPrintingDate;
                        existingServiceProvider.ValidFromDate = objID.ValidFromDate;
                        existingServiceProvider.ValidToDate = objID.ValidToDate;
                        existingServiceProvider.Address = objID.Address;
                        existingServiceProvider.Refrence1ID = objID.Refrence1ID;
                        existingServiceProvider.Refrence1Name = objID.Refrence1Name;
                        existingServiceProvider.Refrence1Mobile = objID.Refrence1Mobile;
                        existingServiceProvider.Refrence2Name = objID.Refrence2Name;
                        existingServiceProvider.Refrence2Mobile = objID.Refrence2Mobile;
                        existingServiceProvider.Refrence2Details = objID.Refrence2Details;
                        existingServiceProvider.isActive = objID.isActive;
                        existingServiceProvider.isDeleted = objID.isDeleted;
                        existingServiceProvider.CreatedOn = objID.CreatedOn;
                        existingServiceProvider.CreatedBy = objID.CreatedBy;
                        existingServiceProvider.UpdatedOn = objID.UpdatedOn;
                        existingServiceProvider.UpdatedBy = objID.UpdatedBy;
                    }
                    else
                    {
                        // Turn IDENTITY_INSERT ON
                        _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblServiceProvider ON");
                        _context.ServiceProviders.Add(objID);
                        // Turn IDENTITY_INSERT OFF
                        _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblServiceProvider  OFF");
                        await _context.SaveChangesAsync();
                        int number = (int)AccessCardHolders.ServiceProvider;
                        objID.IDNumber = number.ToString() + objID.ID.ToString("D5");
                        await _context.SaveChangesAsync();
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new { message = $"{Obj.Count} ServiceProvider processed successfully" });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { error = ex.Message });
            }
        }


    }

}
