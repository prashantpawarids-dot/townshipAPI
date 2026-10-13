using EventFlow.Core;
using EventFlow.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Township_API.Data;
using Township_API.Models;

namespace Township_API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ServiceProviderOwnersController : Controller
    {

        private readonly AppDBContext _context;
        public ServiceProviderOwnersController(AppDBContext context)
        {
            _context = context;
        }



        [HttpPost("AddUPdateServiceProviderOwners")]
        public async Task<IActionResult> AddUPdateServiceProviderOwners([FromBody] ServiceProviderOwners obj)
        {
            try
            {

                var ExistsSproviders = await _context.ServiceProviderOwners.Where(p => p.Id != obj.Id && p.SProviderID == obj.SProviderID && p.Hid == obj.Hid).ToListAsync();
                if (ExistsSproviders != null)
                {
                    if (ExistsSproviders.Count > 0)
                        return BadRequest("ServiceProvider with this owner allready Exists.");
                }
                var ExistsSprovider = await _context.ServiceProviderOwners.Where(p => p.SProviderID == obj.SProviderID && p.Hid == obj.Hid).FirstOrDefaultAsync();

                if (ExistsSprovider != null)
                {
                    if (ExistsSprovider.Id > 0)
                    {
                        ExistsSprovider.SProviderID = obj.SProviderID;
                        ExistsSprovider.Hid = obj.Hid;
                        ExistsSprovider.OwnerDetails = obj.OwnerDetails;
                        ExistsSprovider.isActive = obj.isActive;


                        _context.Entry(ExistsSprovider).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                        return Ok(ExistsSprovider);

                    }

                }
                if (obj != null)
                {
                    _context.Add(obj);
                    await _context.SaveChangesAsync();
                    return Ok(new { message = $"{obj.Id}  Owner added successfully" });
                }
                return BadRequest("unable to save data!");
            }
            catch (Exception ex)
            {
                return BadRequest("Data Error : " + ex.Message.ToString().ToString());
            }
        }

        [HttpDelete("DeleteServiceProviderOwner/{id}")]
        public async Task<IActionResult> DeleteServiceProviderOwner(int id)
        {
            try
            {
                var existing = await _context.ServiceProviderOwners.FindAsync(id);
                if (existing == null)
                {
                    return NotFound();
                }

                _context.ServiceProviderOwners.Remove(existing);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Service provider owner deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        // GET: api/products 
        [HttpGet("GetAllServiceProviderOwners/{ServiceProviderId}")]
        public async Task<IActionResult> GetAllServiceProviderOwners(string ServiceProviderId)
        {
            try
            {
                var Sproviders = await _context.ServiceProviderOwners.Where(p => p.SProviderID == ServiceProviderId).OrderByDescending(p => p.Id).ToListAsync();

                //OwnerName building Neighbourhood FlatNumber
                foreach (var res in Sproviders)
                {
                    //var nr = await _context.AllCardHolders.Where(u => u.IDNumber.ToString() == res.Hid.ToString()).FirstOrDefaultAsync();
                    //if (nr != null)
                    //{
                    //    res.OwnerName = nr.shortname.ToString();
                    //    res.building = nr.NRD.ToString();
                    //    res.Neighbourhood = nr.Bld.ToString();
                    //    res.FlatNumber = nr.FlatNumber.ToString();
                    //}

                    var nr = await _context.AllCardHolders.Where(u => u.IDNumber.ToString() == res.Hid.ToString()).FirstOrDefaultAsync();
                    if (nr != null)
                    {
                        res.OwnerName = nr.shortname.ToString();
                        res.Neighbourhood = nr.NRD.ToString();
                        res.building = nr.Bld.ToString();
                        res.FlatNumber = nr.FlatNumber.ToString();
                    }
                }
                return Ok(Sproviders);
            }
            catch (Exception ex)
            {
                return BadRequest("Data Error : " + ex.Message.ToString().ToString());
            }
        }
         
    }


    [Route("api/[controller]")]
    [ApiController]
    public class ServiceProviderRefrenceFlatsController : Controller
    {

        private readonly AppDBContext _context;
        public ServiceProviderRefrenceFlatsController(AppDBContext context)
        {
            _context = context;
        }
        //DbSetting already maintain in ServiceProvider Table Refrences
        //CREATE TABLE Buildings(
        //    Id INT PRIMARY KEY IDENTITY,
        //    BuildingFlats NVARCHAR(MAX)
        //)
        //BuildingA-101,BuildingA-102,BuildingB-201
        //public class BuildingRequestDto
        //{
        //    public string IdNumber { get; set; }
        //    public string BuildingFlatsList { get; set; }
        //}
        public class RefrenceFlats
        {
            public string IdNumber { get; set; }
            public string? FlatNumber { get; set; } // stored as CSV
        }
        [HttpPost("Add")]
        public async Task<IActionResult> ServiceProviderRefrenceFlatsAdd(RefrenceFlats RF)
        {
            //var entity = new RefrenceFlats
            //{
            //    IdNumber = RF.IdNumber,
            //    BuildingFlats =  RF.BuildingFlats
            //};

            try
            {
                var existingServiceProvider = await _context.ServiceProviders.Where(p => p.IDNumber == RF.IdNumber).FirstOrDefaultAsync();

                if (existingServiceProvider == null)
                {
                    return NotFound();
                }
                existingServiceProvider.FlatNumber = RF.FlatNumber;
                _context.Entry(existingServiceProvider).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(new { Message = $"RefrenceFlats added successfully", IDNumber = existingServiceProvider.IDNumber });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("Get")]
        public async Task<IActionResult> ServiceProviderRefrenceFlatsGet(int id)
        {

            try
            {
                var existingServiceProvider = await _context.ServiceProviders.Where(p => p.ID == id).FirstOrDefaultAsync();

                if (existingServiceProvider == null)
                {
                    return NotFound();
                }

                var result = new RefrenceFlats();

                result.IdNumber = existingServiceProvider.IDNumber;
                result.FlatNumber = existingServiceProvider.FlatNumber;

                //if (existingServiceProvider.FlatNumber != null)
                //{
                //    if (existingServiceProvider.FlatNumber != "") 
                //        result.FlatNumber = existingServiceProvider.FlatNumber;
                //}
                //if (result != null && result.IdNumber != "")
                //{
                //    //result.IdNumber = existingServiceProvider.IDNumber;
                //    //result.BuildingFlats = existingServiceProvider.FlatNumber.ToString();
                //    //if (_entity.FlatNumber != null || !string .IsNullOrEmpty( _entity.FlatNumber))
                //    //{
                //    //    result.BuildingFlats = _entity.FlatNumber.ToString();
                //    //}
                //    //else
                //    //{
                //    //    result.BuildingFlats = "";
                //    //}

                //    //var result = new RefrenceFlats
                //    //{
                //    //    IdNumber = _entity.IDNumber,
                //    //    BuildingFlats = _entity.FlatNumber.ToString()
                //    //};

                //    return Ok(result);


                //    //var result = new BuildingRequestDto
                //    //{
                //    //    BuildingFlatsList = "BuildingA-101,BuildingA-102,BuildingB-201"
                //    //            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                //    //            .Select(x => x.Trim())
                //    //            .ToList()
                //    //};
                //}
                return Ok(result);



            }
            catch (Exception ex)
            {
                //     await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }



        [HttpGet("GetAll")]
        public async Task<IActionResult> ServiceProviderRefrenceFlatsGetAll()
        {

            try
            {
                var existingServiceProvider = await _context.ServiceProviders.Where(p =>1 == 1).ToListAsync();

                if (existingServiceProvider == null)
                {
                    return NotFound();
                }
                List<RefrenceFlats> result =new List<RefrenceFlats>();
                foreach (var item in existingServiceProvider)
                {
                  var RefrenceFlat  = new RefrenceFlats();
                    RefrenceFlat.FlatNumber=item.FlatNumber;
                    RefrenceFlat.IdNumber = item.IDNumber; 
                    result.Add(RefrenceFlat);
                } 
                 
                return Ok(result);



            }
            catch (Exception ex)
            {
                //     await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }

        ////Search
        //var data = _context.Buildings
        //    .Where(x => x.BuildingFlats.Contains("BuildingA-101"))
        //    .ToList();

    }


}
