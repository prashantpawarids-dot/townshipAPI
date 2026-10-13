using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Township_API.Models.commonTypes;
using Township_API.Models;
using Township_API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Township_API.Services;


namespace Township_API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ANPRController : Controller
    {
        private readonly ApiSettings _apiSettings;
        private readonly AppDBContext _context;
        private readonly ANPRServices _apiService;
       // var token = "";
        public ANPRController(AppDBContext context, IOptions<ApiSettings> apiSettings, ANPRServices ANPRServices)
        { 
            _context = context;
            _apiSettings = apiSettings.Value;
            _apiService = ANPRServices;
        }

        //http://{{host}}/api/v1/sdk/vehicle
        //create get put post vehicle

        //[Authorize]
        //[HttpPost("vehicle/Create")]
        //public async Task<IActionResult> ANPR_CreateVehicle([FromBody] Vehicle obj)
        //{
        //    // var BaseURL = builder.Configuration.GetValue<string>("ANPRbaseUrl");
        //    //string token = "your-jwt-or-access-token"; // You may fetch from DB or Identity provider


        //   // var result = await _apiService.GetDataFromExternalApiAsync(token);


        //    List<AllCardHolder> OwnerName = await _context.AllCardHolders.Where(p => p.IDNumber == obj.TagUID).ToListAsync();

        //    ANPR_Vehicle ANPRVehicle = new ANPR_Vehicle();

        //    if (obj != null)
        //    {
        //          ANPRVehicle = new ANPR_Vehicle()
        //        {
        //            id = obj.ID,
        //            owner = OwnerName[0].shortname,
        //            number_plate = obj.RegNo.ToString(),
        //            vehicle_type = obj.vType.ToString(),
        //            auth_type = "whitelist",
        //            comment = null
        //        };
        //    }

        //    try {

        //     //   var response = await _apiService.RegisterANPR_Vehicle_inCamera(ANPRVehicle);

        //     //return Ok();
        //     //   // update set anpr_vehicle

        //     //  // var result=_context.ANPRs.Where





        //    }
        //    catch (Exception ex)
        //    {

        //  //      return BadRequest(new { message = "Validation Failed", ex.InnerException });
        //    }
        //}

        [Authorize]
        [HttpPost("vehicle/Add")]
        public async Task<IActionResult> ANPR_AddVehicle([FromBody] ANPR_Vehicle obj)
        {
            //var BaseURL = builder.Configuration.GetValue<string>("ANPRbaseUrl");
            var  token = "your-jwt-or-access-token"; // You may fetch from DB or Identity provider

            var rt=await _apiService.GetTokenAsync(token);

            //            var result = await _apiService.GetDataFromExternalApiAsync(token);
             


            if (obj != null)
            {
                obj = new ANPR_Vehicle()
                {
                    id = obj.id,
                    owner = obj.owner,
                    number_plate = obj.number_plate.ToString(),
                    vehicle_type = obj.vehicle_type.ToString(),
                    auth_type = "whitelist",
                    comment = null
                };
            }

            try
            {

                var response = await _apiService.RegisterANPR_Vehicle_inCamera(obj);

                return Ok();
            }
            catch (Exception ex)
            {

                return BadRequest(new { message = "Validation Failed", ex.InnerException });
            }
        }

            [Authorize]
        [HttpPost("vehicle/Blacklist")]
        public async Task<IActionResult> ANPR_BlacklistVehicle([FromBody] ANPR_Vehicle obj)
        {
            // var BaseURL = builder.Configuration.GetValue<string>("ANPRbaseUrl");
            //string token = "your-jwt-or-access-token"; // You may fetch from DB or Identity provider


            // var result = await _apiService.GetDataFromExternalApiAsync(token);


            //List<AllCardHolder> OwnerName = await _context.AllCardHolders.Where(p => p.IDNumber == obj.TagUID).ToListAsync();

            ANPR_Vehicle ANPRVehicle = new ANPR_Vehicle();

            if (obj != null)
            {
                ANPRVehicle = new ANPR_Vehicle()
                {
                    id = obj.id,
                    owner = obj.owner,
                    number_plate = obj.number_plate.ToString(),
                    vehicle_type = obj.vehicle_type.ToString(),
                    auth_type = "blacklist",
                    comment = null
                };
            }

            try
            {

                //var response = await _apiService.RegisterANPR_Vehicle_inCamera(ANPRVehicle);

                return Ok();
            }
            catch (Exception ex)
            {

                return BadRequest(new { message = "Validation Failed", ex.InnerException });
            }
        }


        [Authorize]
        [HttpDelete("vehicle/{ID}")]
        public async Task<IActionResult> ANPR_DeleteVehicle(string ID)
        {   

            try
            {

               // var response = await _apiService.DeleteANPR_Vehicle_inCamera(ID);

                return Ok( );
            }
            catch (Exception ex)
            {

                return BadRequest(new { message = "Validation Failed", ex.InnerException });
            }
        }
        [Authorize]
        [HttpGet("vehicle")]
        public async Task<IActionResult> ANPR_GetVehicleList()
        {

            try
            {

               // var response = await _apiService.GetProtectedDataAsync();

                return Ok( );
            }
            catch (Exception ex)
            {

                return BadRequest(new { message = "Validation Failed", ex.InnerException });
            }
        }




        [Authorize]
        [HttpPost("ANPRMessage")]
        public async Task<IActionResult> ANPRMessage([FromBody] ANPR obj)
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

            var existingANPR = await _context.ANPRs.FindAsync(0);
            if (existingANPR != null)
            {
                return BadRequest("Anpr Message Entry Exists.");
            }
            var existsOBJ = await _context.ANPRs.Where(p => p.number_plate == obj.number_plate && p.camera_id == obj.camera_id && p.timestamp == obj.timestamp).ToListAsync();
            if (existsOBJ != null)
            {
                if (existsOBJ.Count > 0)
                    return BadRequest("number_plate Name with this Agency is already Exists.");
            }


            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Contractor ON");
            _context.Add(obj);
            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Contractor OFF");

            await _context.SaveChangesAsync();

            return Ok();
        }

  


    }

}
