using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Township_API.Data;
using Township_API.Models;
using System.Runtime.CompilerServices;
using static Township_API.Models.commonTypes;
using System;
using System.Reflection;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Township_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class A_IDCardPrintController : Controller
    {
        private readonly AppDBContext _context;
        public A_IDCardPrintController(AppDBContext context)
        {
            _context = context;
        }

        [HttpGet("CardHolders")]
        public async Task<IActionResult> GetCardHoldersList(string NRD = null, string Building = null)
        {
            try
            {
                var query = _context.AllCardHolders.AsQueryable();

                if (!string.IsNullOrEmpty(NRD))
                    query = query.Where(x => x.NRD != null && x.NRD.Contains(NRD.ToString()));

                if (!string.IsNullOrEmpty(Building))
                    query = query.Where(x => x.Bld != null && x.Bld.Contains(Building));

                //var records = await query
                //    .Select(inOut => new AllCardHolder
                //    {
                //        templateName = inOut.templateName,
                //        CardType = inOut.CardType,
                //        id = inOut.id,
                //        pid = inOut.pid,
                //        IDNumber = inOut.IDNumber.ToString(),
                //        shortname = inOut.shortname.ToString(),
                //        CSN = inOut.CSN.ToString(),
                //        Building = inOut.Building.ToString(),
                //        FlatNumber = inOut.FlatNumber,
                //        CardPrintingDate = inOut.CardPrintingDate,
                //        CardIssueDate = inOut.CardIssueDate,
                //        validTill = inOut.validTill,
                //        BloodGroup = inOut.BloodGroup.ToString(),
                //        Bld = inOut.Bld.ToString(),
                //        NRD = inOut.NRD.ToString(),
                //        custom1 = inOut.custom1,
                //        custom2 = inOut.custom2
                //    })
                //    .ToListAsync();


                var records = await query
    .OrderByDescending(x => x.CardIssueDate)
    .Select(inOut => new AllCardHolder
    {
        templateName = inOut.templateName,
        CardType = inOut.CardType,
        id = inOut.id,
        pid = inOut.pid,
        IDNumber = inOut.IDNumber.ToString(),
        shortname = inOut.shortname.ToString(),
        CSN = inOut.CSN.ToString(),
        Building = inOut.Building.ToString(),
        FlatNumber = inOut.FlatNumber,
        CardPrintingDate = inOut.CardPrintingDate,
        CardIssueDate = inOut.CardIssueDate,
        validTill = inOut.validTill,
        BloodGroup = inOut.BloodGroup.ToString(),
        Bld = inOut.Bld.ToString(),
        NRD = inOut.NRD.ToString(),
        custom1 = inOut.custom1,
        custom2 = inOut.custom2
    })
    .ToListAsync();

                if (records != null && records.Count > 0)
                    return Ok(records);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Data Error: " + ex.Message });
            }

            return Ok(new { message = "Data not found" });
        }

        [HttpGet("IDCardData")]
        public async Task<IActionResult> GetIDCardHoldersList(string NRD = null, string Building = null)
        {
            try
            {
                var query = _context.IDCardData.AsQueryable();

                if (!string.IsNullOrEmpty(NRD))
                    query = query.Where(x => x.neighbourhood != null && x.neighbourhood.Contains(NRD.ToString()));

                if (!string.IsNullOrEmpty(Building))
                    query = query.Where(x => x.building != null && x.building.Contains(Building));

                var records = await query
                    .Select(inOut => new IDCardData
                    {
                        IDNumber = inOut.IDNumber,
                        name = inOut.name,
                        building = inOut.building,
                        flatNumber = inOut.flatNumber,
                        cardCSN = inOut.cardCSN,
                        cardDetails = inOut.cardDetails,
                        neighbourhood = inOut.neighbourhood,
                        isPrinted = inOut.isPrinted,
                        isPersonalised = inOut.isPersonalised,
                        photoUrl = inOut.photoUrl,
                        holderType = inOut.holderType
                    })
                    .ToListAsync();

                if (records != null && records.Count > 0)
                    return Ok(records);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Data Error: " + ex.Message });
            }

            return Ok(new { message = "Data not found" });
        }

        [HttpGet("GetNRDCardAccess")]
        public async Task<IActionResult> GetNRDCardAccess(string NRD = null, string Building = null)
        {
            try
            {
                var query = _context._userAllAccess.AsQueryable();
                query = query.Where(x => x.ModuleType == "NRD");

                if (!string.IsNullOrEmpty(NRD))
                    query = query.Where(x => x.ModuleName != null && x.ModuleName.Contains(NRD.ToString()));

                if (!string.IsNullOrEmpty(Building))
                    query = query.Where(x => x.ModuleName != null && x.ModuleName.Contains(Building));

                var records = await query
                    .Select(inOut => new UserALLAccess
                    {
                        id = inOut.id,
                        moduleID = inOut.moduleID,
                        ModuleName = inOut.ModuleName,
                        CardHolderID = inOut.CardHolderID,
                        validTillDate = inOut.validTillDate,
                        sun = inOut.sun,
                        mon = inOut.mon,
                        tue = inOut.tue,
                        wed = inOut.wed,
                        thu = inOut.thu,
                        fri = inOut.fri,
                        sat = inOut.sat,
                        isactive = inOut.isactive,
                        isdeleted = inOut.isdeleted,
                        ModuleType = inOut.ModuleType
                    })
                    .ToListAsync();

                if (records != null)
                    return Ok(records);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Data Error: " + ex.Message });
            }

            return Ok(new { message = "Data not found" });
        }

        [HttpPost("UpdateCardCSN")]
        public async Task<IActionResult> UpdateCardCSN(string IDNumber, string CardCSN = "", string? VehicleCSN = "", string contentType = "", byte[] imageBytes = null)
        {
            try
            {
                string filepath = IDNumber;
                var affectedRows = await _context.Database
                    .ExecuteSqlRawAsync("EXEC procUpdateCardData @IDNumber, @CardCSN,@VehicleCSN, @filepath, @imageBytes, @contentType",
                    new SqlParameter("@IDNumber", IDNumber),
                    new SqlParameter("@CardCSN", CardCSN),
                    new SqlParameter("@VehicleCSN", VehicleCSN),
                    new SqlParameter("@filepath", filepath),
                    new SqlParameter("@imageBytes", SqlDbType.VarBinary) { Value = (object)imageBytes ?? DBNull.Value },
                    new SqlParameter("@contentType", contentType));
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Data Error: " + ex.Message });
            }
        }
    }

    // ── CardPrintSettings Controller ──────────────────────────────────────────

    public class CardPrintSettingDto
    {
        public string HolderType { get; set; } = "";
        public string SettingsJson { get; set; } = "";
    }

    [Route("api/[controller]")]
    [ApiController]
    public class CardPrintSettingsController : ControllerBase
    {
        private readonly AppDBContext _context;
        public CardPrintSettingsController(AppDBContext context) => _context = context;

        [HttpGet("{holderType}")]
        public async Task<IActionResult> Get(string holderType)
        {
            try
            {
                var s = await _context.CardPrintSettings
                    .FirstOrDefaultAsync(x => x.HolderType == holderType);
                if (s == null) return NotFound();
                return Ok(new { s.HolderType, s.SettingsJson });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("Save")]
        public async Task<IActionResult> Save([FromBody] CardPrintSettingDto dto)
        {
            try
            {
                var existing = await _context.CardPrintSettings
                    .FirstOrDefaultAsync(x => x.HolderType == dto.HolderType);
                if (existing != null)
                {
                    existing.SettingsJson = dto.SettingsJson;
                    existing.UpdatedOn = DateTime.Now;
                }
                else
                {
                    _context.CardPrintSettings.Add(new CardPrintSetting
                    {
                        HolderType = dto.HolderType,
                        SettingsJson = dto.SettingsJson,
                        UpdatedOn = DateTime.Now
                    });
                }
                await _context.SaveChangesAsync();
                return Ok(new { message = "Saved" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}