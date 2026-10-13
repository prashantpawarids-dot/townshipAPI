using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System;
using System.Buffers.Text;
using System.Data;
using Township_API.Data;
using Township_API.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Township_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly AppDBContext _context;
        public ReportController(AppDBContext context)
        {
            _context = context;
        }


       

        [HttpGet("InoutRegister")]
        public async Task<IActionResult> GetInoutRegister(
     int readerID = 0,
     DateTime? fromDateTime = null,
     DateTime? toDateTime = null)
        {
            try
            {
                IQueryable<CardAccessInout_Register> query =
                    from inOut in _context.CardAccessInout_Register
                    join owner in _context.AllCardHolders
                        on inOut.IDNumber equals owner.IDNumber
                    where (readerID == 0 || inOut.ReaderID == readerID || inOut.ReaderID == null)
                    select new CardAccessInout_Register
                    {
                        ReaderID = inOut.ReaderID,
                        ReaderLocation = inOut.ReaderLocation,
                        CSNNumber = inOut.CSNNumber,
                        Accessmode = inOut.Accessmode,
                        InoutTime = inOut.InoutTime,
                        IDNumber = inOut.IDNumber,
                        OwnerName = inOut.OwnerName,
                        OwnerBuilding = inOut.OwnerBuilding,
                        OwnerFlatNumber = inOut.OwnerFlatNumber,
                        CardType = inOut.CardType,
                        nrdName = owner.NRD,
                        buildingName = owner.Bld
                    };

                if (fromDateTime.HasValue && toDateTime.HasValue)
                {
                    var fromDate = fromDateTime.Value.Date;
                    var toDateInclusive = toDateTime.Value.Date.AddDays(1).AddTicks(-1);
                    query = query.Where(p =>
                        p.InoutTime >= fromDate &&
                        p.InoutTime <= toDateInclusive);
                }

                var records = query.ToList();

                if (!records.Any())
                    return Ok(new { message = "Data not found" });

                return Ok(records);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Data Error: " + ex.Message, stack = ex.StackTrace, inner = ex.InnerException?.Message });
            }
        }

        // GET: api/products/vehicle-inout-register
        [HttpGet("CardAccessInout")]
        public async Task<IActionResult> GetCardAccessInout(int readerID, DateTime fromDateTime, DateTime toDateTime)
        {

            try
            {
                var records = await _context.CardAccessInout.Where(p => 1 == 1).ToListAsync();

                if (records.Count > 0 && fromDateTime != null)
                {
                    if (Convert.ToDateTime(fromDateTime) <= Convert.ToDateTime("01 - 01 - 0001 00:00:00") || Convert.ToDateTime(toDateTime) <= Convert.ToDateTime("01 - 01 - 0001 00:00:00"))
                    {
                        records =
            (from inOut in _context.CardAccessInout
             join owner in _context.AllCardHolders
                on inOut.IDNumber equals owner.IDNumber
             where (inOut.IN_ReaderID == 0 || inOut.IN_ReaderID == readerID)
             select new CardAccessInout
             {
                 IDNumber = inOut.IDNumber,
                 OwnerName = inOut.OwnerName,
                 Nrd = inOut.Nrd,
                 OwnerBuilding = inOut.OwnerBuilding,
                 OwnerFlatNumber = inOut.OwnerFlatNumber,
                 CardType = inOut.CardType,
                 IN_ReaderID = inOut.IN_ReaderID,
                 IN_ReaderLocation = inOut.IN_ReaderLocation,
                 IN_AccessTime = inOut.IN_AccessTime,
                 IN_Accessmode = inOut.IN_Accessmode,
                 OUT_ReaderID = inOut.OUT_ReaderID,
                 OUT_ReaderLocation = inOut.OUT_ReaderLocation,
                 OUT_AccessTime = inOut.OUT_AccessTime,
                 OUT_Accessmode = inOut.OUT_Accessmode

             }).ToList();

                    }
                    else
                    {

                        records =
            (from inOut in _context.CardAccessInout
             join owner in _context.AllCardHolders
                on inOut.IDNumber equals owner.IDNumber
             where (inOut.IN_ReaderID == 0 || inOut.IN_ReaderID == readerID)
             && (inOut.IN_AccessTime >= fromDateTime && inOut.IN_AccessTime <= toDateTime)
             select new CardAccessInout
             {
                 IDNumber = inOut.IDNumber,
                 OwnerName = inOut.OwnerName,
                 Nrd = inOut.Nrd,
                 OwnerBuilding = inOut.OwnerBuilding,
                 OwnerFlatNumber = inOut.OwnerFlatNumber,
                 CardType = inOut.CardType,
                 IN_ReaderID = inOut.IN_ReaderID,
                 IN_ReaderLocation = inOut.IN_ReaderLocation,
                 IN_AccessTime = inOut.IN_AccessTime,
                 IN_Accessmode = inOut.IN_Accessmode,
                 OUT_ReaderID = inOut.OUT_ReaderID,
                 OUT_ReaderLocation = inOut.OUT_ReaderLocation,
                 OUT_AccessTime = inOut.OUT_AccessTime,
                 OUT_Accessmode = inOut.OUT_Accessmode
             }).ToList();

                    }

                    if (records == null || !records.Any())
                    {
                        return Ok(new { message = "Data not found" });
                    }

                    return Ok(records);
                }

                return Ok(new { message = "Data not found" });

            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Data Error: " + ex.Message });
            }
        }

        [HttpGet("CardSummaryReport")]
        public async Task<IActionResult> Get_CardSummaryReport(
     string status = "ALL",
     string nrd = "",
     string building = "",
     string cardType = "",
     string search = "",
     int pageNumber = 1,
     int pageSize = 25)
        {
            try
            {
                // view heavy hai, default 30s kam padta hai
                _context.Database.SetCommandTimeout(180);

                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1) pageSize = 25;

                IQueryable<CardSummaryReport> query = _context.CardSummaryReports.AsNoTracking();

                if (!string.IsNullOrWhiteSpace(nrd) && nrd.ToUpper() != "ALL")
                {
                    var n = nrd.Trim();
                    query = query.Where(p => p.NRD != null && p.NRD.Trim() == n);
                }

                if (!string.IsNullOrWhiteSpace(building) && building.ToUpper() != "ALL")
                {
                    var b = building.Trim();
                    query = query.Where(p => p.Building != null && p.Building.Trim() == b);
                }

                if (!string.IsNullOrWhiteSpace(cardType) && cardType.ToUpper() != "ALL")
                {
                    var c = cardType.Trim();
                    query = query.Where(p => p.CardType == c);
                }

                if (!string.IsNullOrWhiteSpace(search))
                {
                    var s = search.Trim().ToUpper();
                    query = query.Where(p =>
                        (p.IDNumber != null && p.IDNumber.ToUpper().Contains(s)) ||
                        (p.Name != null && p.Name.ToUpper().Contains(s)) ||
                        (p.CardCsn != null && p.CardCsn.ToUpper().Contains(s)) ||
                        (p.FlatNumber != null && p.FlatNumber.ToUpper().Contains(s)));
                }

                // ONE hit on the view (no ORDER BY in SQL). Counts, status filter,
                // sorting and paging are done in memory.
                var all = await query.ToListAsync();

                var summary = new
                {
                    total = all.Count,
                    issued = all.Count(x => x.Status == "Issued"),
                    notIssued = all.Count(x => x.Status == "Not Issued"),
                    blocked = all.Count(x => x.Status == "Blocked")
                };

                var st = (status ?? "ALL").Trim().ToUpper();
                IEnumerable<CardSummaryReport> filtered = all;
                if (st == "ISSUED") filtered = filtered.Where(p => p.Status == "Issued");
                else if (st == "NOT_ISSUED") filtered = filtered.Where(p => p.Status == "Not Issued");
                else if (st == "BLOCKED") filtered = filtered.Where(p => p.Status == "Blocked");

                var list = filtered.ToList();
                var totalCount = list.Count;

                var records = list
                    .OrderBy(p => p.NRD)
                    .ThenBy(p => p.Name)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return Ok(new { data = records, totalCount, summary, pageNumber, pageSize });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Data Error: " + ex.Message });
            }
        }

        [HttpGet("CardSummaryFilters")]
        public async Task<IActionResult> Get_CardSummaryFilters(string nrd = "")
        {
            try
            {
                _context.Database.SetCommandTimeout(180);

                // plain SELECT of 3 columns (no DISTINCT / ORDER BY in SQL)
                var rows = await _context.CardSummaryReports
                    .AsNoTracking()
                    .Select(p => new { p.NRD, p.Building, p.CardType })
                    .ToListAsync();

                var nrds = rows
                    .Where(r => !string.IsNullOrEmpty(r.NRD))
                    .Select(r => r.NRD!)
                    .Distinct().OrderBy(x => x).ToList();

                var bRows = rows.Where(r => !string.IsNullOrEmpty(r.Building));
                if (!string.IsNullOrWhiteSpace(nrd) && nrd.ToUpper() != "ALL")
                {
                    var n = nrd.Trim();
                    bRows = bRows.Where(r => r.NRD == n);
                }
                var buildings = bRows.Select(r => r.Building!).Distinct().OrderBy(x => x).ToList();

                var cardTypes = rows
                    .Where(r => !string.IsNullOrEmpty(r.CardType))
                    .Select(r => r.CardType!)
                    .Distinct().OrderBy(x => x).ToList();

                return Ok(new { nrds, buildings, cardTypes });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Data Error: " + ex.Message });
            }
        }

        //    [HttpGet("vehicleInoutLog")]
        //    public async Task<IActionResult> GetVehicleInoutLog(
        //int cameraId = 0,
        //DateTime? fromDateTime = null,
        //DateTime? toDateTime = null,
        //int pageNumber = 1,
        //int pageSize = 0)
        //    {
        //        try
        //        {
        //            var query = _context.VehicleAccessInout_logs
        //                .AsNoTracking()
        //                .Where(p => cameraId == 0 || p.Cameraid == cameraId)
        //                .AsQueryable();

        //            if (fromDateTime.HasValue && toDateTime.HasValue)
        //            {
        //                var from = fromDateTime.Value.Date;
        //                var to = toDateTime.Value.Date.AddDays(1);
        //                query = query.Where(p => p.InoutTime >= from && p.InoutTime < to);
        //            }

        //            query = query.OrderByDescending(p => p.InoutTime);

        //            var totalCount = await query.CountAsync();

        //            if (pageSize > 0)
        //            {
        //                query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        //            }

        //            var records = await query.ToListAsync();

        //            if (records == null || !records.Any())
        //                return Ok(new { message = "Data not found" });

        //            if (pageSize > 0)
        //                return Ok(new { data = records, totalCount, pageNumber, pageSize });

        //            return Ok(records);
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(new { error = "Data Error: " + ex.Message });
        //        }
        //    }


        [HttpGet("vehicleInoutLog")]
        public async Task<IActionResult> GetVehicleInoutLog(int cameraId = 0, DateTime? fromDateTime = null, DateTime? toDateTime = null, int pageNumber = 1, int pageSize = 0)
        {
            try
            {
                var query = _context.VehicleAccessInout_logs
                    .AsNoTracking()
                    .Where(p => cameraId == 0 || p.Cameraid == cameraId)
                    .AsQueryable();

                if (fromDateTime.HasValue && toDateTime.HasValue)
                {
                    var from = fromDateTime.Value.Date;
                    var to = toDateTime.Value.Date.AddDays(1);
                    query = query.Where(p => p.InoutTime >= from && p.InoutTime < to);
                }

                query = query.OrderByDescending(p => p.InoutTime);

                var totalCount = await query.CountAsync();

                if (pageSize > 0)
                {
                    query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
                }

                // No image columns selected here -> much faster for large date ranges
                var records = await query.Select(p => new
                {
                    p.VehicleNumber,
                    p.OwnerName,
                    p.Building,
                    p.FlatNumber,
                    p.CardType,
                    p.CameraLocation,
                    p.InoutFlag,
                    p.InoutTime,
                    nrd = p.NRD,
                    vType = p.vType,
                    vMake = p.vMake,
                    p.StickerNo,
                    vColor = p.vColor
                }).ToListAsync();

                if (records == null || !records.Any())
                    return Ok(new { message = "Data not found" });

                if (pageSize > 0)
                    return Ok(new { data = records, totalCount, pageNumber, pageSize });

                return Ok(records);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Data Error: " + ex.Message });
            }
        }







        //old code 12 oct 2026
        //        [HttpGet("vehicleInoutRegister")]
        //        public async Task<IActionResult> GetVehicleInoutRegister(
        //DateTime? fromDateTime = null,
        //DateTime? toDateTime = null,
        //string? vehicleType = null,
        //string? inOut = null,
        //string? nrd = null,
        //string? searchType = null,      // 👈 NEW
        //string? searchValue = null,     // 👈 NEW
        //int pageNumber = 1,
        //int pageSize = 50)
        //        {
        //            try
        //            {
        //                if (pageSize <= 0) pageSize = 50;
        //                if (pageSize > 500) pageSize = 500;
        //                if (pageNumber <= 0) pageNumber = 1;

        //                IQueryable<VehicleAccessInout_Register> query = _context.VehicleAccessInout_Registers.AsNoTracking();

        //                // ===== OLD CODE (commented - was rejecting rows where InTime is NULL) =====
        //                // if (fromDateTime.HasValue && toDateTime.HasValue
        //                //     && fromDateTime.Value > DateTime.MinValue && toDateTime.Value > DateTime.MinValue)
        //                // {
        //                //     var from = fromDateTime.Value.Date;
        //                //     var to = toDateTime.Value.Date.AddDays(1);
        //                //     query = query.Where(p => p.InTime >= from && p.InTime < to);
        //                // }

        //                // ===== NEW CODE (fix: include rows where InTime is NULL but OutTime falls in range) =====
        //                if (fromDateTime.HasValue && toDateTime.HasValue
        //                    && fromDateTime.Value > DateTime.MinValue && toDateTime.Value > DateTime.MinValue)
        //                {
        //                    var from = fromDateTime.Value.Date;
        //                    var to = toDateTime.Value.Date.AddDays(1);
        //                    query = query.Where(p =>
        //                        (p.InTime != null && p.InTime >= from && p.InTime < to) ||
        //                        (p.InTime == null && p.OutTime != null && p.OutTime >= from && p.OutTime < to)
        //                    );
        //                }

        //                if (!string.IsNullOrWhiteSpace(vehicleType))
        //                {
        //                    var types = vehicleType.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        //                    if (types.Length > 0)
        //                        query = query.Where(p => types.Contains(p.vType));
        //                }

        //                if (!string.IsNullOrWhiteSpace(nrd))
        //                {
        //                    var nrds = nrd.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        //                    if (nrds.Length > 0)
        //                        query = query.Where(p => nrds.Contains(p.NRD));
        //                }

        //                if (!string.IsNullOrWhiteSpace(inOut))
        //                {
        //                    var modes = inOut.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        //                    bool wantIn = modes.Contains("IN");
        //                    bool wantOut = modes.Contains("OUT");

        //                    if (wantIn && !wantOut)
        //                        query = query.Where(p => p.OutTime == null);
        //                    else if (wantOut && !wantIn)
        //                        query = query.Where(p => p.OutTime != null);
        //                }

        //                // 👇 search filter
        //                if (!string.IsNullOrWhiteSpace(searchType) && !string.IsNullOrWhiteSpace(searchValue))
        //                {
        //                    var val = searchValue.Trim();
        //                    switch (searchType)
        //                    {
        //                        case "vehicleNumber":
        //                            query = query.Where(p => p.VehicleNumber != null && p.VehicleNumber.Contains(val));
        //                            break;
        //                        case "ownerName":
        //                            query = query.Where(p => p.OwnerName != null && p.OwnerName.Contains(val));
        //                            break;
        //                        case "flatNumber":
        //                            query = query.Where(p => p.FlatNumber != null && p.FlatNumber.Contains(val));
        //                            break;
        //                    }
        //                }

        //                // ===== OLD CODE (commented - InTime null rows would sort incorrectly) =====
        //                // query = query.OrderByDescending(p => p.InTime).ThenByDescending(p => p.InUniqueId);

        //                // ===== NEW CODE (fix: fall back to OutTime/OutUniqueId when InTime is null) =====
        //                query = query.OrderByDescending(p => p.InTime ?? p.OutTime)
        //                              .ThenByDescending(p => p.InUniqueId ?? p.OutUniqueId);

        //                var totalCount = await query.CountAsync();

        //                var records = await query
        //                    .Skip((pageNumber - 1) * pageSize)
        //                    .Take(pageSize)
        //                    .Select(p => new
        //                    {
        //                        p.VehicleNumber,
        //                        p.TagUID,
        //                        p.IDNumber,
        //                        p.OwnerName,
        //                        p.NRD,
        //                        p.Building,
        //                        p.CardType,
        //                        p.FlatNumber,
        //                        p.vType,
        //                        p.InTime,
        //                        p.OutTime,
        //                        p.InCameraLocation,
        //                        p.OutCameraLocation,
        //                        p.InUniqueId,
        //                        p.OutUniqueId
        //                    })
        //                    .ToListAsync();

        //                return Ok(new { totalCount, pageNumber, pageSize, data = records });
        //            }
        //            catch (Exception ex)
        //            {
        //                return BadRequest(new { error = "Data Error: " + ex.Message });
        //            }
        //        }

        //// Overall Not Exited unknown vehicles (last 30 days), location wise COUNT only
        //[HttpGet("UnAuthorisedNotExitedOverall")]
        //public async Task<IActionResult> Get_UnAuthorisedNotExitedOverall()
        //{
        //    var conn = _context.Database.GetDbConnection();
        //    var openedHere = false;
        //    try
        //    {
        //        var since = DateTime.Today.AddDays(-30);

        //        if (conn.State != System.Data.ConnectionState.Open)
        //        {
        //            await conn.OpenAsync();
        //            openedHere = true;
        //        }

        //        using var cmd = conn.CreateCommand();
        //        cmd.CommandTimeout = 60;
        //        cmd.CommandText = @"
        //          WITH LastIn AS (
        //              SELECT VehicleNumber,
        //                     CASE WHEN LTRIM(RTRIM(CameraLocation)) = 'Falcon' THEN 'Falcon Towers'
        //                          ELSE LTRIM(RTRIM(CameraLocation)) END AS Loc,
        //                     MAX(InoutTime) AS LastInTime
        //              FROM tblANPR_VehicleInoutRegister
        //              WHERE InoutFlag = 'IN' AND InoutTime >= @Since
        //              GROUP BY VehicleNumber,
        //                       CASE WHEN LTRIM(RTRIM(CameraLocation)) = 'Falcon' THEN 'Falcon Towers'
        //                            ELSE LTRIM(RTRIM(CameraLocation)) END
        //          )
        //          SELECT ISNULL(L.Loc, 'Unknown') AS location, COUNT(*) AS cnt
        //          FROM LastIn L
        //          WHERE NOT EXISTS (SELECT 1 FROM tblVehicle V
        //                            WHERE REPLACE(V.RegNo, ' ', '') = REPLACE(L.VehicleNumber, ' ', ''))
        //            AND NOT EXISTS (SELECT 1 FROM tblANPR_VehicleInoutRegister O
        //                            WHERE O.VehicleNumber = L.VehicleNumber
        //                              AND O.InoutFlag = 'OUT'
        //                              AND CASE WHEN LTRIM(RTRIM(O.CameraLocation)) = 'Falcon' THEN 'Falcon Towers'
        //                                       ELSE LTRIM(RTRIM(O.CameraLocation)) END = L.Loc
        //                              AND O.InoutTime > L.LastInTime)
        //          GROUP BY ISNULL(L.Loc, 'Unknown')
        //          ORDER BY cnt DESC";

        //        var p = cmd.CreateParameter();
        //        p.ParameterName = "@Since";
        //        p.Value = since;
        //        cmd.Parameters.Add(p);

        //        var rows = new List<object>();
        //        var total = 0;
        //        using (var rd = await cmd.ExecuteReaderAsync())
        //        {
        //            while (await rd.ReadAsync())
        //            {
        //                var loc = rd.GetString(0);
        //                var cnt = rd.GetInt32(1);
        //                total += cnt;
        //                rows.Add(new { location = loc, count = cnt });
        //            }
        //        }

        //        return Ok(new { total, data = rows });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { error = "Data Error: " + ex.Message });
        //    }
        //    finally
        //    {
        //        if (openedHere) await conn.CloseAsync();
        //    }
        //}



        // Not Exited unknown vehicles (last 30 days), location wise COUNT only.
        // Returns Overall (all 30 days) and Today (last IN is today) in ONE response.


        [HttpGet("vehicleInoutRegister")]
        public async Task<IActionResult> GetVehicleInoutRegister(
DateTime? fromDateTime = null,
DateTime? toDateTime = null,
string? vehicleType = null,
string? inOut = null,
string? nrd = null,
string? searchType = null,
string? searchValue = null,
int pageNumber = 1,
int pageSize = 50)
        {
            try
            {
                if (pageSize <= 0) pageSize = 50;
                if (pageSize > 500) pageSize = 500;
                if (pageNumber <= 0) pageNumber = 1;

                // ===== OLD (view: har click par poori raw table par pairing, ~8-15 sec) =====
                // IQueryable<VehicleAccessInout_Register> query = _context.VehicleAccessInout_Registers.AsNoTracking();
                //
                // if (fromDateTime.HasValue && toDateTime.HasValue
                //     && fromDateTime.Value > DateTime.MinValue && toDateTime.Value > DateTime.MinValue)
                // {
                //     var from = fromDateTime.Value.Date;
                //     var to = toDateTime.Value.Date.AddDays(1);
                //     query = query.Where(p =>
                //         (p.InTime != null && p.InTime >= from && p.InTime < to) ||
                //         (p.InTime == null && p.OutTime != null && p.OutTime >= from && p.OutTime < to)
                //     );
                // }

                // ===== NEW (ready table, EventTime = ISNULL(InTime, OutTime), clustered index par) =====
                IQueryable<VehicleAccessInout_Register> query;

                if (fromDateTime.HasValue && toDateTime.HasValue
                    && fromDateTime.Value > DateTime.MinValue && toDateTime.Value > DateTime.MinValue)
                {
                    var fromDate = fromDateTime.Value.Date;
                    var toExclusive = toDateTime.Value.Date.AddDays(1);

                    query = _context.VehicleAccessInout_Registers
                        .FromSqlInterpolated($@"SELECT * FROM dbo.tblVehicleInoutRegisterReport
                                                WHERE EventTime >= {fromDate} AND EventTime < {toExclusive}")
                        .AsNoTracking();
                }
                else
                {
                    query = _context.VehicleAccessInout_Registers
                        .FromSqlInterpolated($@"SELECT * FROM dbo.tblVehicleInoutRegisterReport")
                        .AsNoTracking();
                }

                if (!string.IsNullOrWhiteSpace(vehicleType))
                {
                    var types = vehicleType.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    if (types.Length > 0)
                        query = query.Where(p => types.Contains(p.vType));
                }

                if (!string.IsNullOrWhiteSpace(nrd))
                {
                    var nrds = nrd.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    if (nrds.Length > 0)
                        query = query.Where(p => nrds.Contains(p.NRD));
                }

                if (!string.IsNullOrWhiteSpace(inOut))
                {
                    var modes = inOut.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    bool wantIn = modes.Contains("IN");
                    bool wantOut = modes.Contains("OUT");

                    if (wantIn && !wantOut)
                        query = query.Where(p => p.OutTime == null);
                    else if (wantOut && !wantIn)
                        query = query.Where(p => p.OutTime != null);
                }

                // search filter
                if (!string.IsNullOrWhiteSpace(searchType) && !string.IsNullOrWhiteSpace(searchValue))
                {
                    var val = searchValue.Trim();
                    switch (searchType)
                    {
                        case "vehicleNumber":
                            query = query.Where(p => p.VehicleNumber != null && p.VehicleNumber.Contains(val));
                            break;
                        case "ownerName":
                            query = query.Where(p => p.OwnerName != null && p.OwnerName.Contains(val));
                            break;
                        case "flatNumber":
                            query = query.Where(p => p.FlatNumber != null && p.FlatNumber.Contains(val));
                            break;
                    }
                }

                // sorting same (InTime null ho to OutTime)
                query = query.OrderByDescending(p => p.InTime ?? p.OutTime)
                              .ThenByDescending(p => p.InUniqueId ?? p.OutUniqueId);

                var totalCount = await query.CountAsync();

                var records = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new
                    {
                        p.VehicleNumber,
                        p.TagUID,
                        p.IDNumber,
                        p.OwnerName,
                        p.NRD,
                        p.Building,
                        p.CardType,
                        p.FlatNumber,
                        p.vType,
                        p.InTime,
                        p.OutTime,
                        p.InCameraLocation,
                        p.OutCameraLocation,
                        p.InUniqueId,
                        p.OutUniqueId
                    })
                    .ToListAsync();

                return Ok(new { totalCount, pageNumber, pageSize, data = records });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Data Error: " + ex.Message });
            }
        }

        [HttpGet("UnAuthorisedNotExitedOverall")]
        public async Task<IActionResult> Get_UnAuthorisedNotExitedOverall()
        {
            var conn = _context.Database.GetDbConnection();
            var openedHere = false;
            try
            {
                var since = DateTime.Today.AddDays(-30);
                var today = DateTime.Today;

                if (conn.State != System.Data.ConnectionState.Open)
                {
                    await conn.OpenAsync();
                    openedHere = true;
                }

                using var cmd = conn.CreateCommand();
                cmd.CommandTimeout = 60;
                cmd.CommandText = @"
                  WITH LastIn AS (
                      SELECT VehicleNumber,
                             CASE WHEN LTRIM(RTRIM(CameraLocation)) = 'Falcon' THEN 'Falcon Towers'
                                  ELSE LTRIM(RTRIM(CameraLocation)) END AS Loc,
                             MAX(InoutTime) AS LastInTime
                      FROM tblANPR_VehicleInoutRegister
                      WHERE InoutFlag = 'IN' AND InoutTime >= @Since
                      GROUP BY VehicleNumber,
                               CASE WHEN LTRIM(RTRIM(CameraLocation)) = 'Falcon' THEN 'Falcon Towers'
                                    ELSE LTRIM(RTRIM(CameraLocation)) END
                  )
                  SELECT ISNULL(L.Loc, 'Unknown') AS location,
                         COUNT(*) AS cnt,
                         ISNULL(SUM(CASE WHEN L.LastInTime >= @Today THEN 1 ELSE 0 END), 0) AS todayCnt
                  FROM LastIn L
                  WHERE NOT EXISTS (SELECT 1 FROM tblVehicle V
                                    WHERE REPLACE(V.RegNo, ' ', '') = REPLACE(L.VehicleNumber, ' ', ''))
                    AND NOT EXISTS (SELECT 1 FROM tblANPR_VehicleInoutRegister O
                                    WHERE O.VehicleNumber = L.VehicleNumber
                                      AND O.InoutFlag = 'OUT'
                                      AND CASE WHEN LTRIM(RTRIM(O.CameraLocation)) = 'Falcon' THEN 'Falcon Towers'
                                               ELSE LTRIM(RTRIM(O.CameraLocation)) END = L.Loc
                                      AND O.InoutTime > L.LastInTime)
                  GROUP BY ISNULL(L.Loc, 'Unknown')
                  ORDER BY cnt DESC";

                var p = cmd.CreateParameter();
                p.ParameterName = "@Since";
                p.Value = since;
                cmd.Parameters.Add(p);

                var p2 = cmd.CreateParameter();
                p2.ParameterName = "@Today";
                p2.Value = today;
                cmd.Parameters.Add(p2);

                var rows = new List<object>();
                var todayList = new List<(string location, int count)>();
                var total = 0;
                var todayTotal = 0;

                using (var rd = await cmd.ExecuteReaderAsync())
                {
                    while (await rd.ReadAsync())
                    {
                        var loc = rd.GetString(0);
                        var cnt = rd.GetInt32(1);
                        var todayCnt = rd.GetInt32(2);

                        total += cnt;
                        rows.Add(new { location = loc, count = cnt });

                        if (todayCnt > 0)
                        {
                            todayTotal += todayCnt;
                            todayList.Add((loc, todayCnt));
                        }
                    }
                }

                var todayRows = todayList
                    .OrderByDescending(x => x.count)
                    .Select(x => new { location = x.location, count = x.count })
                    .ToList();

                return Ok(new
                {
                    total,
                    data = rows,
                    today = new { total = todayTotal, data = todayRows }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Data Error: " + ex.Message });
            }
            finally
            {
                if (openedHere) await conn.CloseAsync();
            }
        }





        //        [HttpGet("UnAuthorisedVehicleInoutRegister")]
        //        public async Task<IActionResult> Get_UnAuthorisedVehicleInoutRegister(
        //DateTime fromDateTime,
        //DateTime toDateTime,
        //int pageNumber = 1,
        //int pageSize = 25,
        //string searchVehicle = "",
        //string neighbourhood = "",
        //string inOut = "",
        //string exitStatus = "ALL")
        //        {
        //            try
        //            {
        //                IQueryable<VehicleUnathorisedAccessInout_Register> query;

        //                if (fromDateTime == DateTime.MinValue || toDateTime == DateTime.MinValue)
        //                {
        //                    query = _context.VehicleUnathorisedAccessInout_Register;
        //                }
        //                else
        //                {
        //                    //var toDateEnd = toDateTime.Date.AddDays(1).AddTicks(-1);

        //                    //query = _context.VehicleUnathorisedAccessInout_Register
        //                    //    .Where(p =>
        //                    //        p.InTime >= fromDateTime.Date &&
        //                    //        p.InTime <= toDateEnd);


        //                    var toDateEnd = toDateTime.Date.AddDays(1).AddTicks(-1);
        //                    var fromDate = fromDateTime.Date;

        //                    query = _context.VehicleUnathorisedAccessInout_Register
        //                        .Where(p =>
        //                            (p.InTime != null && p.InTime >= fromDate && p.InTime <= toDateEnd) ||
        //                            (p.InTime == null && p.OutTime != null && p.OutTime >= fromDate && p.OutTime <= toDateEnd));
        //                }

        //                if (!string.IsNullOrWhiteSpace(searchVehicle))
        //                {
        //                    var search = searchVehicle.Trim().ToUpper();

        //                    query = query.Where(p =>
        //                        p.VehicleNumber != null &&
        //                        p.VehicleNumber.ToUpper().Contains(search));
        //                }

        //                if (!string.IsNullOrWhiteSpace(neighbourhood) && neighbourhood.ToUpper() != "ALL")
        //                {
        //                    var nb = neighbourhood.Trim().ToUpper();

        //                    query = query.Where(p =>
        //                        p.InCameraLocation != null &&
        //                        p.InCameraLocation.ToUpper().Contains(nb));
        //                }

        //                if (!string.IsNullOrWhiteSpace(inOut))
        //                {
        //                    var flags = inOut
        //                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
        //                        .Select(f => f.Trim().ToUpper())
        //                        .ToList();

        //                    if (flags.Count > 0 && !flags.Contains("ALL"))
        //                    {
        //                        var wantIn = flags.Contains("IN");
        //                        var wantOut = flags.Contains("OUT");

        //                        if (wantIn && !wantOut)
        //                        {
        //                            query = query.Where(p => p.OutTime == null);
        //                        }
        //                        else if (wantOut && !wantIn)
        //                        {
        //                            query = query.Where(p => p.OutTime != null);
        //                        }
        //                        // agar dono ya koi nahi selected, to filter skip (sab dikhao)
        //                    }
        //                }

        //                // Exit Status: NOT_EXITED = OUT nahi mila, EXITED = OUT mil gaya
        //                var exitFlag = (exitStatus ?? "ALL").Trim().ToUpper();
        //                if (exitFlag == "NOT_EXITED")
        //                {
        //                    query = query.Where(p => p.OutTime == null);
        //                }
        //                else if (exitFlag == "EXITED")
        //                {
        //                    query = query.Where(p => p.OutTime != null);
        //                }
        //                else if (exitFlag == "EXIT_ONLY")
        //                {
        //                    query = query.Where(p => p.InTime == null && p.OutTime != null);
        //                }

        //                // Project only the fields the list view needs — NO images here.
        //                // Images are heavy (base64 binary) and were causing 6000+ rows
        //                // to bloat the response to hundreds of MB, hanging the browser.
        //                var projected = query
        //    .AsNoTracking()
        //    .Select(p => new
        //    {
        //        p.VehicleNumber,
        //        p.TagUID,
        //        p.IDNumber,
        //        p.OwnerName,
        //        p.Building,
        //        p.CardType,
        //        p.FlatNumber,
        //        p.InCameraLocation,
        //        p.OutCameraLocation,
        //        p.InTime,
        //        p.OutTime,
        //        p.InUniqueId,     // NEW
        //        p.OutUniqueId,   // NEW
        //        p.VehicleTypeLabel  // NEW - 'Two Wheeler' / 'Four Wheeler' / 'Unknown'
        //    })
        //    .Distinct();

        //                var totalCount = await projected.CountAsync();

        //                // Pagination now happens in SQL Server itself (Skip/Take),
        //                // so only the current page's rows are ever pulled into memory.
        //                var records = await projected
        //                   .OrderByDescending(p => p.InTime ?? p.OutTime)
        //                    .Skip((pageNumber - 1) * pageSize)
        //                    .Take(pageSize)
        //                    .ToListAsync();

        //                if (!records.Any())
        //                {
        //                    return Ok(new
        //                    {
        //                        message = "Data not found",
        //                        data = new List<object>(),
        //                        totalCount = 0,
        //                        pageNumber,
        //                        pageSize
        //                    });
        //                }

        //                return Ok(new
        //                {
        //                    data = records,
        //                    totalCount,
        //                    pageNumber,
        //                    pageSize
        //                });
        //            }
        //            catch (Exception ex)
        //            {
        //                return BadRequest(new
        //                {
        //                    error = "Data Error: " + ex.Message
        //                });
        //            }
        //        }




        [HttpGet("UnAuthorisedVehicleInoutRegister")]
        public async Task<IActionResult> Get_UnAuthorisedVehicleInoutRegister(
   DateTime fromDateTime,
   DateTime toDateTime,
  int pageNumber = 1,
   int pageSize = 25,
string searchVehicle = "",
string neighbourhood = "",
string inOut = "",
string exitStatus = "ALL")
        {
            try
            {
                IQueryable<VehicleUnathorisedAccessInout_Register> query;

                if (fromDateTime == DateTime.MinValue || toDateTime == DateTime.MinValue)
                {
                    query = _context.VehicleUnathorisedAccessInout_Register;
                }
                else
                {
                    // CHANGED: view ki jagah date-filtered SQL function use ho rahi hai.
                    // Function pehle sirf chuni hui dates ki rows uthati hai, phir dedup/pairing karti hai.
                    // @To exclusive hai (agle din ki shuruaat), isliye toDate.AddDays(1).
                    var fromDate = fromDateTime.Date;
                    var toExclusive = toDateTime.Date.AddDays(1);

                    // OLD
                    // query = _context.VehicleUnathorisedAccessInout_Register
                    //     .FromSqlInterpolated($"SELECT * FROM dbo.fnUnathorisedVehicleInout_Register({fromDate}, {toExclusive})");

                    // NEW
                    query = _context.VehicleUnathorisedAccessInout_Register
                        .FromSqlInterpolated($@"SELECT * FROM dbo.tblUnauthorisedVehicleReport
                            WHERE EventTime >= {fromDate} AND EventTime < {toExclusive}");
                }

                if (!string.IsNullOrWhiteSpace(searchVehicle))
                {
                    var search = searchVehicle.Trim().ToUpper();

                    query = query.Where(p =>
                        p.VehicleNumber != null &&
                        p.VehicleNumber.ToUpper().Contains(search));
                }

                if (!string.IsNullOrWhiteSpace(neighbourhood) && neighbourhood.ToUpper() != "ALL")
                {
                    var nb = neighbourhood.Trim().ToUpper();

                    query = query.Where(p =>
                        p.InCameraLocation != null &&
                        p.InCameraLocation.ToUpper().Contains(nb));
                }

                if (!string.IsNullOrWhiteSpace(inOut))
                {
                    var flags = inOut
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(f => f.Trim().ToUpper())
                        .ToList();

                    if (flags.Count > 0 && !flags.Contains("ALL"))
                    {
                        var wantIn = flags.Contains("IN");
                        var wantOut = flags.Contains("OUT");

                        if (wantIn && !wantOut)
                        {
                            query = query.Where(p => p.OutTime == null);
                        }
                        else if (wantOut && !wantIn)
                        {
                            query = query.Where(p => p.OutTime != null);
                        }
                        // agar dono ya koi nahi selected, to filter skip (sab dikhao)
                    }
                }

                // Exit Status: NOT_EXITED = OUT nahi mila, EXITED = OUT mil gaya
                var exitFlag = (exitStatus ?? "ALL").Trim().ToUpper();
                if (exitFlag == "NOT_EXITED")
                {
                    query = query.Where(p => p.OutTime == null);
                }
                else if (exitFlag == "EXITED")
                {
                    query = query.Where(p => p.OutTime != null);
                }
                else if (exitFlag == "EXIT_ONLY")
                {
                    query = query.Where(p => p.InTime == null && p.OutTime != null);
                }

                // Project only the fields the list view needs — NO images here.
                // Images are heavy (base64 binary) and were causing 6000+ rows
                // to bloat the response to hundreds of MB, hanging the browser.
                var projected = query
    .AsNoTracking()
    .Select(p => new
    {
        p.VehicleNumber,
        p.TagUID,
        p.IDNumber,
        p.OwnerName,
        p.Building,
        p.CardType,
        p.FlatNumber,
        p.InCameraLocation,
        p.OutCameraLocation,
        p.InTime,
        p.OutTime,
        p.InUniqueId,     // NEW
        p.OutUniqueId,   // NEW
        p.VehicleTypeLabel  // NEW - 'Two Wheeler' / 'Four Wheeler' / 'Unknown'
    })
    .Distinct();

                var totalCount = await projected.CountAsync();

                // Pagination now happens in SQL Server itself (Skip/Take),
                // so only the current page's rows are ever pulled into memory.
                var records = await projected
                   .OrderByDescending(p => p.InTime ?? p.OutTime)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                if (!records.Any())
                {
                    return Ok(new
                    {
                        message = "Data not found",
                        data = new List<object>(),
                        totalCount = 0,
                        pageNumber,
                        pageSize
                    });
                }

                return Ok(new
                {
                    data = records,
                    totalCount,
                    pageNumber,
                    pageSize
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = "Data Error: " + ex.Message
                });
            }
        }

        [HttpGet("VehicleImage/{uniqueId}")]
        public async Task<IActionResult> GetVehicleImage(string uniqueId)
        {
            if (string.IsNullOrWhiteSpace(uniqueId))
                return BadRequest(new { error = "uniqueId is required" });

            try
            {
                byte[]? img = null;

                using (var conn = new SqlConnection(_context.Database.GetConnectionString()))
                using (var cmd = new SqlCommand(
                    "SELECT Data FROM dbo.tblANPR_TRImages WHERE unique_id = @uid AND UPPER(Imagetype) = 'FRAME'",
                    conn))
                {
                    cmd.Parameters.AddWithValue("@uid", uniqueId);
                    await conn.OpenAsync();

                    using var reader = await cmd.ExecuteReaderAsync();
                    if (await reader.ReadAsync())
                    {
                        img = reader["Data"] as byte[];
                    }
                }

                if (img == null)
                    return NotFound(new { error = "Image not found for this unique_id" });

                return Ok(new { image = Convert.ToBase64String(img) });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Data Error: " + ex.Message });
            }
        }
        // GET: api/products/vehicle-inout-register
        [HttpGet("CardAccessBlockInvokeRegister")]
        public async Task<IActionResult> CardAccessBlockInvokeRegister(DateTime fromDateTime, DateTime toDateTime)
        {
            try
            {
                var records = await _context.AccessBlockRevoke_Register_Report.Where(p => 1 == 1)
                      .ToListAsync();

                if (fromDateTime != null)
                {
                    if (Convert.ToDateTime(fromDateTime) <= Convert.ToDateTime("01 - 01 - 0001 00:00:00") || Convert.ToDateTime(toDateTime) <= Convert.ToDateTime("01 - 01 - 0001 00:00:00"))
                    {

                    }
                    else
                    {
                        records = await _context.AccessBlockRevoke_Register_Report
                  .Where(p => p.BlockDate >= fromDateTime && p.BlockDate <= toDateTime)
                  .ToListAsync();
                    }
                }

                if (records != null)
                {
                    if (records.Count > 0)
                    {

                    }
                    else
                    {
                        return Ok(new { message = "Data not found" });
                    }
                }
                return Ok(records);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Data Error: " + ex.Message });
            }
        }



        // GET: api/products/vehicle-inout-register
        [HttpGet("Dashboard")]
        public async Task<IActionResult> DashBoard()
        {
            try
            {

                var records = await _context.DashBords.Where(p => 1 == 1)
                      .ToListAsync();

                return Ok(records);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Data Error: " + ex.Message });
            }
        }

        // GET: api/products/vehicle-inout-register
        [HttpGet("CardEncodingLog")]
        public async Task<IActionResult> CardEncodingLogs(DateTime fromDateTime, DateTime toDateTime)
        {
            try
            {
                //var records = await _context.CardEncodingLogs.Where(p => 1 == 1)
                //      .ToListAsync();
                var records = await _context.CardEncodingLogs.Where(p => 1 == 2).ToListAsync();
                if (fromDateTime != null)
                {
                    if (Convert.ToDateTime(fromDateTime) <= Convert.ToDateTime("01 - 01 - 0001 00:00:00") || Convert.ToDateTime(toDateTime) <= Convert.ToDateTime("01 - 01 - 0001 00:00:00"))
                    {

                    }
                    else
                    {
                        records = await _context.CardEncodingLogs
                .Where(p => p.CardIssueDate >= fromDateTime.Date && p.CardIssueDate <= toDateTime.Date.AddDays(1).AddTicks(-1))
                  .ToListAsync();
                    }
                }
                else
                {
                    return Ok(new { message = "Data not found" });
                }

                return Ok(records);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Data Error: " + ex.Message });
            }
        }

        [HttpGet("CardPrintingLog")]
        public async Task<IActionResult> CardPrintingLogs(DateTime fromDateTime, DateTime toDateTime)
        {
            try
            {
                //var records = await _context.CardPrintingLogs.Where(p => 1 == 1)
                //      .ToListAsync();

                var records = await _context.CardPrintingLogs.Where(p => 1 == 2).ToListAsync();
                if (fromDateTime != null)
                {
                    if (Convert.ToDateTime(fromDateTime) <= Convert.ToDateTime("01 - 01 - 0001 00:00:00") || Convert.ToDateTime(toDateTime) <= Convert.ToDateTime("01 - 01 - 0001 00:00:00"))
                    {

                    }
                    else
                    {
                        records = await _context.CardPrintingLogs
              .Where(p => p.CardPrintingDate >= fromDateTime.Date && p.CardPrintingDate <= toDateTime.Date.AddDays(1).AddTicks(-1))
                  .ToListAsync();
                    }
                }
                else
                {
                    return Ok(new { message = "Data not found" });
                }

                return Ok(records);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Data Error: " + ex.Message });
            }
        }

        [HttpGet("CardLostDamaged")]
        public async Task<IActionResult> CardLostDamaged(DateTime fromDateTime, DateTime toDateTime)
        {
            try
            {
                var records = await _context.CardLostDamages.Where(p => 1 == 1).ToListAsync();

                if (fromDateTime != null)
                {

                    if (Convert.ToDateTime(fromDateTime) <= Convert.ToDateTime("01 - 01 - 0001 00:00:00") || Convert.ToDateTime(toDateTime) <= Convert.ToDateTime("01 - 01 - 0001 00:00:00"))
                    {

                    }
                    else
                    {
                        records = await _context.CardLostDamages
                            .Where(p => p.reporteddate >= fromDateTime && p.reporteddate <= toDateTime)
                             .ToListAsync();

                    }
                }

                if (records != null)
                {
                    if (records.Count > 0)
                    {

                    }

                    else
                    {
                        return Ok(new { message = "Data not found" });
                    }
                }
                return Ok(records);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Data Error: " + ex.Message });
            }
        }

    }
}
