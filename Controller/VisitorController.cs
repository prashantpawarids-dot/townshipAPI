using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Township_API.Models.commonTypes;
using Township_API.Data;
using Township_API.Models;
using System.Security.Cryptography;

namespace Township_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VisitorController : Controller
    {
        private readonly AppDBContext _context;

        public VisitorController(AppDBContext context)
        {
            _context = context;
        }


        // PUT: api/products/5
        [HttpPost("UpdateVisitor/{id}")]
        public async Task<IActionResult> UpdateVisitor(int id, [FromBody] VisitorMaster UpdatedVisitor)
        {
            try
            {
                if (id != UpdatedVisitor.ID)
                {
                    return BadRequest("Visitor ID mismatch.");
                }

                //var existingTenent = await _service.UpdatePrimaryTenentAsync(updatedVisitor.ID, updatedVisitor);
                var ExistingVisitor = await _context.VisitorMasters.FindAsync(UpdatedVisitor.ID);
                if (ExistingVisitor == null)
                {
                    return NotFound();
                }

                var existVisitor = await _context.VisitorMasters.Where(p => (p.FirstName == UpdatedVisitor.FirstName && p.LastName == UpdatedVisitor.LastName && p.Building == UpdatedVisitor.Building && p.FlatNumber == UpdatedVisitor.FlatNumber) && p.ID != UpdatedVisitor.ID).ToListAsync();
                if (existVisitor != null)
                {
                    if (existVisitor.Count > 0)
                        return BadRequest("Visitor with name already Exists for this flat.");
                }

                ExistingVisitor.ID = UpdatedVisitor.ID;
                ExistingVisitor.HID = UpdatedVisitor.HID;
                ExistingVisitor.CSN = UpdatedVisitor.CSN;
                ExistingVisitor.IDNumber = UpdatedVisitor.IDNumber;
                ExistingVisitor.TagNumber = UpdatedVisitor.TagNumber;
                ExistingVisitor.PANnumber = UpdatedVisitor.PANnumber;
                ExistingVisitor.PassportNo = UpdatedVisitor.PassportNo;
                ExistingVisitor.LicenseNo = UpdatedVisitor.LicenseNo;
                ExistingVisitor.ICEno = UpdatedVisitor.ICEno;
                ExistingVisitor.AadharCardId = UpdatedVisitor.AadharCardId;
                ExistingVisitor.VoterID = UpdatedVisitor.VoterID;
                ExistingVisitor.FirstName = UpdatedVisitor.FirstName;
                ExistingVisitor.MiddleName = UpdatedVisitor.MiddleName;
                ExistingVisitor.LastName = UpdatedVisitor.LastName;
                ExistingVisitor.ShortName = UpdatedVisitor.ShortName;
                ExistingVisitor.Gender = UpdatedVisitor.Gender;
                ExistingVisitor.EmailID = UpdatedVisitor.EmailID;
                ExistingVisitor.MobileNo = UpdatedVisitor.MobileNo;
                ExistingVisitor.LandLine = UpdatedVisitor.LandLine;
                ExistingVisitor.Building = UpdatedVisitor.Building;
                ExistingVisitor.NRD = UpdatedVisitor.NRD;
                ExistingVisitor.FlatNumber = UpdatedVisitor.FlatNumber;
                ExistingVisitor.CardIssueDate = UpdatedVisitor.CardIssueDate;
                ExistingVisitor.CardPrintingDate = UpdatedVisitor.CardPrintingDate;
                ExistingVisitor.LogicalDeleted = UpdatedVisitor.LogicalDeleted;
                ExistingVisitor.visitStartTime = UpdatedVisitor.visitStartTime;
                ExistingVisitor.visitEndTime = UpdatedVisitor.visitEndTime;
                ExistingVisitor.visitPurpose = UpdatedVisitor.visitPurpose;
                ExistingVisitor.visitDesription = UpdatedVisitor.visitDesription;
                ExistingVisitor.visitStatus = UpdatedVisitor.visitStatus;



                _context.Entry(ExistingVisitor).State = EntityState.Modified;
                await _context.SaveChangesAsync();

               // return Ok(new { message = $"{ExistingVisitor.ID}   Visitor updated successfully" });

                return Ok(new { Message = $"Visitor Updated successfully", IDNumber = ExistingVisitor.IDNumber });
            }
            catch (Exception ex)
            {
                return BadRequest("Data Error : " + ex.Message.ToString().ToString());
            }
        }


        [HttpPost("AddVisitor")]
        public async Task<IActionResult> AddVisitor([FromBody] VisitorMaster obj)
        {
            try
            {
                var existingVisitor = await _context.VisitorMasters.FindAsync(0);
                if (existingVisitor != null)
                {
                    return BadRequest("Visitor Exists.");
                }
                _context.Add(obj);
                await _context.SaveChangesAsync();

                int number = (int)AccessCardHolders.Visitor;
                obj.IDNumber = number.ToString() + obj.ID.ToString("D4");
                await _context.SaveChangesAsync();
                 
                return Ok(new { Message = $"Visitor Added successfully", IDNumber = obj.IDNumber });

            }
            catch (Exception ex)
            {
                return BadRequest("Data Error : " + ex.Message.ToString().ToString());
            }
        }


        // GET: api/products 
        [HttpGet]
        public async Task<IActionResult> GetAllVisitors()
        {
            var Visitors = await _context.VisitorMasters.OrderByDescending(p => p.ID).ToListAsync();

            foreach (var res in Visitors)
            {
                var nr = await _context.ModuleData.Where(u => u.ID.ToString() == res.NRD.ToString()).FirstOrDefaultAsync();
                if (nr != null)
                    res.NRDName = (string)nr.Name.ToString();
                nr = await _context.ModuleData.Where(u => u.ID.ToString() == res.Building.ToString()).FirstOrDefaultAsync();
                if (nr != null)
                    res.BuildingName = (string)nr.Name.ToString();

                res.visitStartDate = (string)res.visitStartTime.Value.ToString("dd-MM-yyyy");
                res.visitEndDate = (string)res.visitStartTime.Value.ToString("dd-MM-yyyy");
            }

            return Ok(Visitors);
        }

        // GET: api/products 
        [HttpGet("{ID}")]
        public async Task<IActionResult> GetVisitorDetails(int ID)
        {
            var Visitors = await _context.VisitorMasters.Where(p => p.ID == ID).ToListAsync();
            if (Visitors != null && Visitors.Count > 0)
            {
                foreach (var res in Visitors)
                {
                    var nr = await _context.ModuleData.Where(u => u.ID.ToString() == res.NRD.ToString()).FirstOrDefaultAsync();
                    if (nr != null)
                        res.NRDName = (string)nr.Name.ToString();

                    var bld = await _context.ModuleData.Where(u => u.ID.ToString() == res.Building.ToString()).FirstOrDefaultAsync();
                    if (bld != null)
                        res.BuildingName = (string)bld.Name.ToString();
                }
            }

            string? IdNumber = Visitors[0].IDNumber;

            if (IdNumber != null)
            {
                try
                {
                    var jsonWrapper = new DependentJsonWrapper
                    {
                        Owners = Visitors,
                        //  DependentOwners = await _context.DependentVisitors.Where(p => p.PID == ID).ToListAsync(),
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
            return Ok(new { message = $"Visitor records not found!" });
        }



    }

}
