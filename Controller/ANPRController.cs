using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net.NetworkInformation;
using Township_API.Data;
using Township_API.Models;
using System.Data.SqlClient;
using System.Linq;
using Township_API.Services;
using static Township_API.Models.commonTypes;

namespace Township_API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ANPRController : Controller
    {
        private readonly ApiSettings _apiSettings;
        private readonly AppDBContext _context;
       // private readonly ANPRServices _apiService;
       // var token = "";
        public ANPRController(AppDBContext context, IOptions<ApiSettings> apiSettings)//, ANPRServices ANPRServices
        { 
            _context = context;
            _apiSettings = apiSettings.Value;
           // _apiService = ANPRServices;
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

        //[Authorize]
        [HttpPost("vehicle/Add")]
        public async Task<IActionResult> ANPR_AddVehicle([FromBody] ANPR_Vehicle obj)
        {
            ////var BaseURL = builder.Configuration.GetValue<string>("ANPRbaseUrl");
            //var  token = "your-jwt-or-access-token"; // You may fetch from DB or Identity provider

            //var rt=await _apiService.GetTokenAsync(token);

            ////            var result = await _apiService.GetDataFromExternalApiAsync(token);



            //if (obj != null)
            //{
            //    obj = new ANPR_Vehicle()
            //    {
            //        id = obj.id,
            //        owner = obj.owner,
            //        number_plate = obj.number_plate.ToString(),
            //        vehicle_type = obj.vehicle_type.ToString(),
            //        auth_type = "whitelist",
            //        comment = null
            //    };
            //}

            //try
            //{

            //    var response = await _apiService.RegisterANPR_Vehicle_inCamera(obj);

            //    return Ok();
            //}
            //catch (Exception ex)
            //{

            //    return BadRequest(new { message = "Validation Failed", ex.InnerException });
            //}
    return Ok();
        }

        //[Authorize]
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



        //[Authorize]
        [HttpPost("vehicle/{ID}")]
        public async Task<IActionResult> ANPR_DeleteVehicle(string ID)
        {   

            try
            {
                 //var response = await _apiService.DeleteANPR_Vehicle_inCamera(ID);

                return Ok( );
            }
            catch (Exception ex)
            {

                return BadRequest(new { message = "Validation Failed", ex.InnerException });
            }
        }

        //[Authorize]
        //[HttpGet("vehicle")]
        //public async Task<IActionResult> ANPR_GetVehicleList()
        //{

        //    try
        //    {

        //        // var response = await _apiService.GetProtectedDataAsync();
        //        // return Ok( );
        //        var Vehicles = await _context.ANPRs.OrderByDescending(p => p.number_plate).ToListAsync();
        //        return Ok(Vehicles);

        //    }
        //    catch (Exception ex)
        //    {

        //        return BadRequest(new { message = "Validation Failed", ex.InnerException });
        //    }
        //}


        // [Authorize]
        [HttpGet("vehicle")]
        public async Task<IActionResult> ANPR_GetVehicleList()
        {
            try
            {
                var result = new List<object>();
                string sql = @"
            SELECT v.RegNo AS regNo,
                   ISNULL(v.vType,'Two Wheeler') AS vType,
                   ch.shortname AS shortname,
                   ch.IDNumber AS idNumber,
                   LTRIM(RTRIM(ISNULL(ch.NRD,''))) AS nrd,
                   ch.Bld AS building,
                   ch.FlatNumber AS flatNumber,
                   v.TagUID AS tagUID,
                   v.isactive AS isactive,
                   v.StickerNo as stickerNo, 
                   v.Logical_Delete AS logicalDelete,
                   v.createdon AS createdon
            FROM dbo.tblVehicle v
            INNER JOIN dbo.vwCardHolders ch ON ch.IDNumber = v.TagUID";

                using (var conn = new SqlConnection(_context.Database.GetConnectionString()))
                using (var cmd = new SqlCommand(sql, conn))
                {
                    await conn.OpenAsync();
                    using var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        string regNo = (reader["regNo"] as string ?? "").Replace(" ", "").ToUpper();
                        bool isActive = reader["isactive"] == DBNull.Value || Convert.ToInt32(reader["isactive"]) > 0;
                        bool isDeleted = reader["logicalDelete"] != DBNull.Value && Convert.ToInt32(reader["logicalDelete"]) == 1;

                        bool validFormat = System.Text.RegularExpressions.Regex.IsMatch(regNo,
                            @"^([A-Za-z]{2}\d{1,2}[A-Za-z]{1,3}\d{2,4}|\d{2}(bh|BH|bH|Bh)\d{4}[A-Za-z]{1,2}|[A-Za-z]{2}\d{6}|[A-Za-z]{3}\d{4})$");

                        string status = isDeleted ? "Deleted"
                                      : !isActive ? "Inactive"
                                      : !validFormat ? "InvalidFormat"
                                      : "Active";

                        result.Add(new
                        {
                            regNo = reader["regNo"],
                            vType = reader["vType"],
                            shortname = reader["shortname"],
                            idNumber = reader["idNumber"],
                            nrd = reader["nrd"],
                            building = reader["building"],
                            flatNumber = reader["flatNumber"],
                            tagUID = reader["tagUID"],
                            stickerNo = reader["stickerNo"] == DBNull.Value
                        ? null
                        : reader["stickerNo"],
                            createdon = reader["createdon"],
                            status = status
                        });
                    }
                }

                //// Only genuine, camera-matching vehicles by default
                //var activeOnly = result.Where(r => (string)((dynamic)r).status == "Active")
                //                        .OrderByDescending(r => ((dynamic)r).regNo)
                //                        .ToList();

                //return Ok(activeOnly);
                var ordered = result.OrderByDescending(r => ((dynamic)r).regNo).ToList();
                return Ok(ordered);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Validation Failed", ex.InnerException });
            }
        }
        // Lazy-load images: only called when user clicks a row to view the image popup.
        // Returns BOTH Plate and Frame images for the given uniqueId in a single call.
        [HttpGet("vehicleImage")]
        public async Task<IActionResult> GetVehicleImage(string uniqueId)
        {
            if (string.IsNullOrWhiteSpace(uniqueId))
                return BadRequest(new { message = "uniqueId is required" });

            try
            {
                string? plateImage = null;
                string? frameImage = null;

                using (var conn = new SqlConnection(_context.Database.GetConnectionString()))
                using (var cmd = new SqlCommand(
                    "SELECT Imagetype, Data FROM dbo.tblANPR_TRImages WHERE unique_id = @uid AND UPPER(Imagetype) IN ('PLATE','FRAME')",
                    conn))
                {
                    cmd.Parameters.AddWithValue("@uid", uniqueId);
                    await conn.OpenAsync();

                    using var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var type = (reader["Imagetype"] as string)?.ToUpper();
                        var bytes = reader["Data"] as byte[];
                        if (bytes == null) continue;

                        var base64 = $"data:image/jpeg;base64,{Convert.ToBase64String(bytes)}";

                        if (type == "PLATE") plateImage = base64;
                        else if (type == "FRAME") frameImage = base64;
                    }
                }

                return Ok(new { plateImage, frameImage });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Validation Failed", ex.Message });
            }
        }
        //[Authorize]
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



        // 1) GET — saare cameras, live Connected/Disconnected status ke saath
        [HttpGet("camera/list")]
        public async Task<IActionResult> ANPR_GetCameraList()
        {
            var cameras = await _context.ANPRControllers
                .Where(c => c.IsDeleted == false)
                .OrderBy(c => c.ANPRLocation).ThenBy(c => c.ANPRMode)
                .ToListAsync();

            // Sab cameras ko parallel mein ping karo - taaki 8 cameras pe bhi list load fast rahe
            var pingTasks = cameras.Select(async cam =>
            {
                cam.Status = await IsCameraOnlineAsync(cam.ANPRIP) ? "Connected" : "Disconnected";
                return cam;
            });

            var result = await Task.WhenAll(pingTasks);
            return Ok(result);
        }

        [HttpPost("camera/add")]
        public async Task<IActionResult> ANPR_AddCamera([FromBody] ANPRCamera obj)
        {
            if (string.IsNullOrWhiteSpace(obj.ANPRIP))
                return BadRequest(new { message = "ANPRIP is required" });

            bool exists = await _context.ANPRControllers
                .AnyAsync(c => c.ANPRIP == obj.ANPRIP && c.IsDeleted == false);
            if (exists)
                return BadRequest(new { message = "Camera with this IP already exists" });

            obj.isActive = true;
            obj.IsDeleted = false;
            _context.ANPRControllers.Add(obj);
            await _context.SaveChangesAsync();

            return Ok(obj);
        }

        // 3) PUT — existing camera update karo
        [HttpPut("camera/update/{id}")]
        public async Task<IActionResult> ANPR_UpdateCamera(int id, [FromBody] ANPRCamera obj)
        {
            var camera = await _context.ANPRControllers.FindAsync(id);
            if (camera == null)
                return NotFound(new { message = "Camera not found" });

            camera.ANPRIP = obj.ANPRIP;
            camera.ANPRport = obj.ANPRport;
            camera.ANPRLocation = obj.ANPRLocation;
            camera.ANPRMode = obj.ANPRMode;
            camera.ANPRTerminalID = obj.ANPRTerminalID;
            camera.isActive = obj.isActive;

            await _context.SaveChangesAsync();
            return Ok(camera);
        }

        // Helper — asli live check, timestamp pe depend nahi karta
        private async Task<bool> IsCameraOnlineAsync(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip)) return false;
            try
            {
                using var ping = new Ping();
                var reply = await ping.SendPingAsync(ip, 800); // 800ms timeout - list fast load ho
                return reply.Status == IPStatus.Success;
            }
            catch
            {
                return false;
            }
        }



    }

}
