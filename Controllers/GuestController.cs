using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Township_API.Models.commonTypes;
using Township_API.Data;
using Township_API.Models;

namespace Township_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuestController : Controller
    {
        
        private readonly AppDBContext _context;
       
        public GuestController(AppDBContext context)
        {
            _context = context;
        }


        // PUT: api/products/5
        [HttpPost("{UpdateGuest}/{id}")]
        public async Task<IActionResult> UpdateGuest(int id, [FromBody] GuestMaster updatedGuest)
        {
            try
            {
                if (id != updatedGuest.ID)
                {
                    return BadRequest("Guest ID mismatch.");
                }

                //var existingTenent = await _service.UpdatePrimaryTenentAsync(updatedGuest.ID, updatedGuest);
                var existingGuest = await _context.GuestMasters.FindAsync(updatedGuest.ID);
                if (existingGuest == null)
                {
                    return NotFound();
                }

                var existGuest = await _context.GuestMasters.Where(p => (p.FirstName == updatedGuest.FirstName && p.LastName == updatedGuest.LastName && p.Building == updatedGuest.Building && p.FlatNumber == updatedGuest.FlatNumber) && p.ID != updatedGuest.ID).ToListAsync();
                if (existGuest != null)
                {
                    if (existGuest.Count > 0)
                        return BadRequest("Guest with name already Exists for this flat.");
                }

                existingGuest.RID = updatedGuest.RID;
                existingGuest.CSN = updatedGuest.CSN;
                existingGuest.IDNumber = updatedGuest.IDNumber;
                existingGuest.TagNumber = updatedGuest.TagNumber;
                existingGuest.PANnumber = updatedGuest.PANnumber;
                existingGuest.PassportNo = updatedGuest.PassportNo;
                existingGuest.LicenseNo = updatedGuest.LicenseNo;
                existingGuest.ICEno = updatedGuest.ICEno;
                existingGuest.AadharCardId = updatedGuest.AadharCardId;
                existingGuest.VoterID = updatedGuest.VoterID;
                existingGuest.FirstName = updatedGuest.FirstName;
                existingGuest.MiddleName = updatedGuest.MiddleName;
                existingGuest.LastName = updatedGuest.LastName;
                existingGuest.ShortName = updatedGuest.ShortName;
                existingGuest.Gender = updatedGuest.Gender;
                existingGuest.BloodGroup = updatedGuest.BloodGroup;
                existingGuest.DOB = updatedGuest.DOB;
                existingGuest.EmailID = updatedGuest.EmailID;
                existingGuest.MobileNo = updatedGuest.MobileNo;
                existingGuest.LandLine = updatedGuest.LandLine;
                existingGuest.Building = updatedGuest.Building;
                existingGuest.NRD = updatedGuest.NRD;
                existingGuest.FlatNumber = updatedGuest.FlatNumber;
                existingGuest.CardIssueDate = updatedGuest.CardIssueDate;
                existingGuest.CardPrintingDate = updatedGuest.CardPrintingDate;
                existingGuest.ValidFrom = updatedGuest.ValidFrom;
                existingGuest.ValidTill = updatedGuest.ValidTill;
                existingGuest.LogicalDeleted = updatedGuest.LogicalDeleted;


                _context.Entry(existingGuest).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new { message = $"Guest Updated successfully", IDNumber = existingGuest.IDNumber });

            }
            catch (Exception ex)
            {
                return BadRequest("Data Error : " + ex.Message.ToString().ToString());
            }
        }


        [HttpPost("AddGuest")]
        public async Task<IActionResult> AddGuest([FromBody] GuestMaster obj)
        {
            try
            {
                var existingGuest = await _context.GuestMasters.FindAsync(0);
                if (existingGuest != null)
                {
                    return BadRequest("Guest Exists.");
                }
                _context.Add(obj);
                await _context.SaveChangesAsync();

                int number = (int)AccessCardHolders.Guest;
                obj.IDNumber = number.ToString() + obj.ID.ToString("D5");
                await _context.SaveChangesAsync();
                 
                return Ok(new { message = $"Guest Added successfully", IDNumber = obj.IDNumber });

            }
            catch (Exception ex)
            {
                return BadRequest("Data Error : " + ex.Message.ToString().ToString());
            }
        }


        // GET: api/products 
        [HttpGet]
        public async Task<IActionResult> GetAllGuests()
        {
            var Guests = await _context.GuestMasters.OrderByDescending(p => p.ID).ToListAsync();
            foreach (var res in Guests)
            {
                var nr = await _context.ModuleData.Where(u => u.ID.ToString() == res.NRD.ToString()).FirstOrDefaultAsync();
                if (nr != null)
                    res.NRDName = (string)nr.Name.ToString();
                nr = await _context.ModuleData.Where(u => u.ID.ToString() == res.Building.ToString()).FirstOrDefaultAsync();
                if (nr != null)
                    res.BuildingName = (string)nr.Name.ToString();
            }

            return Ok(Guests);
        }

        // GET: api/products 
        [HttpGet("{ID}")]
        public async Task<IActionResult> GetGuestDetails(int ID)
        {
            var Guests = await _context.GuestMasters.Where(p => p.ID == ID).ToListAsync();
            string? IdNumber = Guests[0].IDNumber;
            if (IdNumber != null)
            {
                try
                {
                    var jsonWrapper = new DependentJsonWrapper
                    {
                        Owners = Guests,
                      //  DependentOwners = await _context.DependentGuests.Where(p => p.PID == ID).ToListAsync(),
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
            return Ok(new { message = $"Guest records not found!" });
        }


        [HttpPost("{AddGuests}")]
        public async Task<IActionResult> AddGuests([FromBody] List<GuestMaster> updatedGuest)
        {


            if (updatedGuest == null || !updatedGuest.Any())
                return BadRequest("No Data provided");

           // using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var objID in updatedGuest)
                {
                    var existingobj = await _context.GuestMasters
                        .FirstOrDefaultAsync(p => p.ID == objID.ID);

                    if (existingobj != null)
                    {
                        //var existingGuest = await _service.UpdatePrimaryGuestAsync(objID.ID, existingobj);
                        var existingGuest = await _context.GuestMasters.FindAsync(objID.ID);
                        if (existingGuest == null)
                        {
                            return NotFound();
                        }

                        existingGuest.RID = objID.RID;
                        existingGuest.CSN = objID.CSN;
                        existingGuest.IDNumber = objID.IDNumber;
                        existingGuest.TagNumber = objID.TagNumber;
                        existingGuest.PANnumber = objID.PANnumber;
                        existingGuest.PassportNo = objID.PassportNo;
                        existingGuest.LicenseNo = objID.LicenseNo;
                        existingGuest.ICEno = objID.ICEno;
                        existingGuest.AadharCardId = objID.AadharCardId;
                        existingGuest.VoterID = objID.VoterID;
                        existingGuest.FirstName = objID.FirstName;
                        existingGuest.MiddleName = objID.MiddleName;
                        existingGuest.LastName = objID.LastName;
                        existingGuest.ShortName = objID.ShortName;
                        existingGuest.Gender = objID.Gender;
                        existingGuest.BloodGroup = objID.BloodGroup;
                        existingGuest.DOB = objID.DOB;
                        existingGuest.EmailID = objID.EmailID;
                        existingGuest.MobileNo = objID.MobileNo;
                        existingGuest.LandLine = objID.LandLine;
                        existingGuest.Building = objID.Building;
                        existingGuest.NRD = objID.NRD;
                        existingGuest.FlatNumber = objID.FlatNumber;
                        existingGuest.CardIssueDate = objID.CardIssueDate;
                        existingGuest.CardPrintingDate = objID.CardPrintingDate;
                        existingGuest.ValidFrom = objID.ValidFrom;
                        existingGuest.ValidTill = objID.ValidTill;
                        existingGuest.LogicalDeleted = objID.LogicalDeleted;  

                        await _context.SaveChangesAsync();
                    }
                    else
                    {

                        // Turn IDENTITY_INSERT ON
                        _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT GuestMaster ON");
                        _context.Add(objID);
                        // Turn IDENTITY_INSERT OFF
                        _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT GuestMaster OFF");
                        await _context.SaveChangesAsync();

                        int number = (int)AccessCardHolders.Guest;
                        objID.IDNumber = number.ToString() + objID.ID.ToString("D5");
                        await _context.SaveChangesAsync();

                    }
                }
                  
                return Ok(new { message = $"{updatedGuest.Count} Guest processed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


    }


}
