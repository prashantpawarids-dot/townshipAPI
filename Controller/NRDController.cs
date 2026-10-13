using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using Township_API.Data;
using Township_API.Models;
using Township_API.Services;
using static Township_API.Models.commonTypes;

namespace Township_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NRDController : Controller
    {
        private readonly AppDBContext _context;

        //private readonly iService _service;
        public NRDController(AppDBContext context)//, iService srv
        {
            _context = context;
        }

        // PUT: api/products/5
        [HttpPost("{UpdateNRD}/{id}")]
        public async Task<IActionResult> UpdateNRD(int id, [FromBody] NRD updatedOBJ)
        {
            try
            {
                if (id != updatedOBJ.ID)
                {
                    return BadRequest("NRD ID mismatch.");
                }
                var existingOBJ = await _context.ModuleData.FindAsync(id);
                if (existingOBJ == null)
                {
                    return NotFound();
                }
                int t = (int)ModuleTypes.NRD;
                var existsOBJ = await _context.ModuleData.Where(p => p.Name == updatedOBJ.Name && p.TypeID.ToString() == t.ToString() && p.ID != updatedOBJ.ID).ToListAsync();
                if (existsOBJ != null)
                {
                    if (existsOBJ.Count > 0)
                        return BadRequest("NRD ID Already Exists.");
                }

                // Update properties
                existingOBJ.Name = updatedOBJ.Name;
                existingOBJ.Code = updatedOBJ.Code;
                existingOBJ.ParentID = updatedOBJ.ParentID;
                existingOBJ.isactive = updatedOBJ.isactive;

                _context.Entry(existingOBJ).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(existingOBJ);
            }
            catch (Exception ex)
            {
                //     await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpPost("AddNRD")]
        public async Task<IActionResult> AddNRD([FromBody] NRD obj)
        {
            try
            {
                var existingOBJ = await _context.ModuleData.FindAsync(0);
                if (existingOBJ != null)
                {
                    return BadRequest("NRD Exists.");
                }
                var t = (int)ModuleTypes.NRD;
                var existOBJ = await _context.ModuleData.Where(p => p.Name == obj.Name && obj.TypeID.ToString() == t.ToString()).ToListAsync();
                if (existOBJ != null)
                {
                    if (existOBJ.Count > 0)
                        return BadRequest("NRD ID Already Exists.");
                }

                // Turn IDENTITY_INSERT ON
                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblModuleData ON");
                _context.Add(obj);

                // Turn IDENTITY_INSERT ON
                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblModuleData OFF");
                await _context.SaveChangesAsync();

                return Ok(new { message = $"NRD processed successfully" });
            }
            catch (Exception ex)
            {
                //     await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // GET: api/products 
        //[HttpGet]
        //public async Task<IActionResult> GetAllNRDs()
        //{
        //    try
        //    {
        //        int objval = (int)commonTypes.ModuleTypes.NRD;

        //        //var OBJs = await _context.ModuleData.Where(n => n.TypeID == objval && n.ID > objval && n.isactive==true).OrderByDescending(p => p.ID).ToListAsync();

        //        //Nana Bhai Ka Code
        //        //var OBJs =  from Mod in _context.ModuleData
        //        //           join Prnt in _context.ModuleData on Mod.ParentID equals Prnt.ID
        //        //           where Mod.ID > objval && Mod.isactive == true && Mod.ModuleType == "NRD"
        //        //           select new
        //        //           {
        //        //               Mod.ID,
        //        //               Mod.Name,
        //        //               Mod.Code,
        //        //               Mod.TypeID,
        //        //               Mod.ParentID,
        //        //               Mod.ModuleType,
        //        //               Parent = Prnt.Name,
        //        //               Mod.isactive,
        //        //               Mod.createdby,
        //        //               Mod.createdon,
        //        //               Mod.updatedby,
        //        //               Mod.updatedon
        //        //           };

        //        var OBJs = from Mod in _context.ModuleData
        //                   join Prnt in _context.ModuleData on Mod.ParentID equals Prnt.ID into ParentJoin
        //                   from Prnt in ParentJoin.DefaultIfEmpty()
        //                   where Mod.ID > objval && Mod.isactive == true && Mod.ModuleType == "NRD"
        //                   select new
        //                   {
        //                       Mod.ID,
        //                       Mod.Name,
        //                       Mod.Code,
        //                       Mod.TypeID,
        //                       Mod.ParentID,
        //                       Mod.ModuleType,
        //                       Parent = Prnt != null ? Prnt.Name : null,
        //                       Mod.isactive,
        //                       Mod.createdby,
        //                       Mod.createdon,
        //                       Mod.updatedby,
        //                       Mod.updatedon
        //                   };
        //        // Id,Name,code,typeid,parentid,ModuleType,Discriminator,isactive,createdby,createdon,updatedby,updatedon
        //        return Ok(OBJs);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { error = ex.Message });

        //    }
        //}


        // GET: api/products 
        [HttpGet]
        public async Task<IActionResult> GetAllNRDs([FromQuery] int? companyId = null)
        {
            try
            {
                int objval = (int)commonTypes.ModuleTypes.NRD;

                var query = from Mod in _context.ModuleData
                            join Prnt in _context.ModuleData on Mod.ParentID equals Prnt.ID into ParentJoin
                            from Prnt in ParentJoin.DefaultIfEmpty()
                            where Mod.ID > objval && Mod.isactive == true && Mod.ModuleType == "NRD"
                            select new
                            {
                                Mod.ID,
                                Mod.Name,
                                Mod.Code,
                                Mod.TypeID,
                                Mod.ParentID,
                                Mod.ModuleType,
                                Parent = Prnt != null ? Prnt.Name : null,
                                Mod.isactive,
                                Mod.createdby,
                                Mod.createdon,
                                Mod.updatedby,
                                Mod.updatedon
                            };

                if (companyId.HasValue && companyId.Value > 0)
                {
                    query = query.Where(x => x.ParentID == companyId.Value);
                }

                var OBJs = await query.ToListAsync();
                return Ok(OBJs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // GET: api/products 
        [HttpGet("GetAllProjectNRDs/{ProjectID}")]
        public async Task<IActionResult> GetAllProjectNRDs(int ProjectID)
        {
            try
            {
                int objval = (int)commonTypes.ModuleTypes.NRD;

                var OBJs = await _context.ModuleData.Where(n => n.TypeID == objval && n.ID > objval && n.ParentID == ProjectID).OrderByDescending(p => p.ID).ToListAsync();

                return Ok(OBJs);
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("GetNRD/{ID}")]
        public async Task<IActionResult> GetNRD(int id)
        {
            try
            {
                int objval = (int)commonTypes.ModuleTypes.NRD;

                var OBJs = await _context.ModuleData.Where(n => n.TypeID == objval && n.ID > objval && n.ID == id).OrderByDescending(p => p.ID).ToListAsync();

                return Ok(OBJs);
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { error = ex.Message });

            }
        }


        [HttpGet("NRDBuildingDetails")]
        public async Task<IActionResult> GetAllNRDData()
        {
            try
            {
                List<tmpNRD> nrdlist = new List<tmpNRD>();
                int objval = (int)commonTypes.ModuleTypes.NRD;
                var OBJs = await _context.ModuleData.Where(n => n.TypeID == objval && n.ID > objval).OrderByDescending(p => p.ID).ToListAsync();


                foreach (var NRDItem in OBJs)
                {

                    tmpNRD _nrd = new tmpNRD { id = NRDItem.ID, nrdname =""+ NRDItem.Name , code = NRDItem.Code, buildingList = null };

                    objval = (int)commonTypes.ModuleTypes.Building;
                    var OBJ2s = await _context.ModuleData.Where(n => n.TypeID == objval && n.ID > objval && n.ParentID == NRDItem.ID).ToListAsync();
                    List<tmpbuilding> buildings = new List<tmpbuilding>();
                    foreach (var Item in OBJ2s)
                    {
                        tmpbuilding b = new tmpbuilding { id = Item.ID, name =""+ Item.Name, code = Item.Code };
                        buildings.Add(b);
                    }
                    _nrd.buildingList = buildings;
                    nrdlist.Add(_nrd); // attach manually
                }
                return Ok(nrdlist);
            }
            catch (Exception ex)
            {
                //     await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }
        public class tmpNRD { public int id { get; set; } public string nrdname { get; set; } public string code { get; set; } public List<tmpbuilding> buildingList { get; set; } }
        public class tmpbuilding { public int id { get; set; } public string name { get; set; } public string code { get; set; } }

        [HttpPost("bulkSaveNRD")]
        public async Task<IActionResult> BulkSave([FromBody] List<NRD> Obj)
        {

            //using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var updatedObj in Obj)
                {
                    var existingOBJ = await _context.ModuleData
                        .FirstOrDefaultAsync(v => v.ID == updatedObj.ID);

                    if (existingOBJ != null)
                    {
                        // Update properties 
                        existingOBJ.Code = updatedObj.Code;
                        existingOBJ.Name = updatedObj.Name;
                        existingOBJ.ParentID = updatedObj.ParentID;
                        existingOBJ.ModuleType = updatedObj.ModuleType;
                        existingOBJ.TypeID = updatedObj.TypeID;
                        existingOBJ.isactive = updatedObj.isactive;
                        existingOBJ.createdby = 0;
                        existingOBJ.updatedby = 0;
                        existingOBJ.updatedon = DateTime.Now;
                        await _context.SaveChangesAsync();
                        //  await transaction.CommitAsync();
                    }
                    else
                    {
                        // Turn IDENTITY_INSERT ON
                        _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblModuleData ON");
                        _context.ModuleData.Add(updatedObj);
                        //  await transaction.CommitAsync();
                        // Turn IDENTITY_INSERT ON
                        _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblModuleData OFF");
                        await _context.SaveChangesAsync();
                    }
                }


                return Ok(new { message = $"NRD processed successfully" });
            }
            catch (Exception ex)
            {
                //     await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("DeleteID/{ID}")]
        public async Task<IActionResult> DeleteID(int ID)
        {
            try
            {  
                int t = (int)ModuleTypes.NRD;
                string ModuleName = ModuleTypes.NRD.ToString();
                var existsOBJ = await _context.ModuleData.Where(p => p.TypeID == t && p.ID == ID && p.ID > 20).FirstOrDefaultAsync();
                 if (existsOBJ == null)
                {
                    return BadRequest($"{ModuleName.ToString()} Data not Exists with this ID {ID}.");
                }

                // Update properties 
                existsOBJ.isactive = false;
                existsOBJ.updatedon = DateTime.Now;
                existsOBJ.updatedby = 1;

                _context.Entry(existsOBJ).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(new { message = $"{ModuleName.ToString()} Data {existsOBJ.Name} inactive successfully" });
            }
            catch (Exception ex)
            {
                //     await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }
 
        [HttpGet("NRDFlats")]
        public async Task<IActionResult> GetNRDFlats()
        {
            try
            {
                var OBJs = await _context.NRDFlatDetails.OrderBy(p => p.BuildingName).ThenBy(p => p.FlatNumber).ToListAsync();
                 return Ok(OBJs);
            }
            catch (Exception ex)
            {
                //     await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }
       
    }
    // ADD THIS HERE
    public class VehicleNRDAccessRequest
    {
        public int VehicleID { get; set; }

        public List<int> NRDIDs { get; set; } = new List<int>();
    }

    [Route("api/[controller]")]
    [ApiController]
    public class PhaseController : Controller
    {
        private readonly AppDBContext _context;

        public PhaseController(AppDBContext context)
        {
            _context = context;
        }

 
        // PUT: api/products/5
        [HttpPost("{UpdatePhase}/{id}")]
        public async Task<IActionResult> UpdatePhase(int id, [FromBody] Phase updatedObj)
        {
            try
            {
                if (id != updatedObj.ID)
                {
                    return BadRequest("{updatedObj.code} ID mismatch.");
                }

                var existingOBJ = await _context.ModuleData.FindAsync(id);
                if (existingOBJ == null)
                {
                    return NotFound();
                }
                var t = (int)ModuleTypes.Phases;
                var existOBJ = await _context.ModuleData.Where(p => p.ID.ToString() != id.ToString() && p.Name.ToString() == updatedObj.Name.ToString() && updatedObj.TypeID.ToString() == t.ToString()).ToListAsync();
                if (existOBJ != null)
                {
                    if (existOBJ.Count > 0)
                        return BadRequest("Phase ID Already Exists.");
                }
                // Update properties
                existingOBJ.Name = updatedObj.Name;
                existingOBJ.Code = updatedObj.Code;
                existingOBJ.ParentID = updatedObj.ParentID;
                existingOBJ.isactive = updatedObj.isactive;


                _context.Entry(existingOBJ).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(existingOBJ);
            }
            catch (Exception ex)
            {
                //     await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpPost("AddPhase")]
        public async Task<IActionResult> AddPhase([FromBody] Phase obj)
        {
            try
            {
                var existingPhase = await _context.ModuleData.FindAsync(0);
                if (existingPhase != null)
                {
                    return BadRequest("Phase Exists.");
                }

                var t = (int)ModuleTypes.Phases;
                var existOBJ = await _context.ModuleData.Where(p => p.Name.ToString() == obj.Name.ToString() && obj.TypeID.ToString() == t.ToString()).ToListAsync();
                if (existOBJ != null)
                {
                    if (existOBJ.Count > 0)
                        return BadRequest("Phase ID Already Exists.");
                }

                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblModuleData ON");
                _context.Add(obj);
                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblModuleData OFF");
                await _context.SaveChangesAsync();

                return Ok(new { message = $"{obj.ID} Phase processed successfully" });
            }
            catch (Exception ex)
            {
                // await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }


        // GET: api/products 
        [HttpGet]
        public async Task<IActionResult> GetAllPhases()
        {
            var Phases = await _context.ModuleData.Where(p => (p.TypeID == (int)commonTypes.ModuleTypes.Phases && p.ID > (int)commonTypes.ModuleTypes.Phases)).OrderByDescending(p => p.ID).ToListAsync();
            return Ok(Phases);
        }

        // GET: api/products 
        [HttpGet("GetPhase/{id}")]
        public async Task<IActionResult> GetPhase(int id)
        {
            var Phases = await _context.ModuleData.Where(p => (p.TypeID == (int)commonTypes.ModuleTypes.Phases && p.ID == id && p.ID > (int)commonTypes.ModuleTypes.Phases)).OrderByDescending(p => p.ID).ToListAsync();
            return Ok(Phases);
        }

        [HttpPost("bulkSavePhase")]
        public async Task<IActionResult> BulkSave([FromBody] List<Phase> Obj)
        {
            //if (Vehicles == null || !Vehicles.Any())
            //     return "BadRequest("No Vehicles provided")";

            //  using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var updatedObj in Obj)
                {
                    var existingOBJ = await _context.ModuleData
                        .FirstOrDefaultAsync(v => v.ID == updatedObj.ID);

                    if (existingOBJ != null)
                    {
                        // Update properties 
                        existingOBJ.Name = existingOBJ.Name;
                        existingOBJ.Code = existingOBJ.Code;
                        existingOBJ.ParentID = existingOBJ.ParentID;
                        existingOBJ.isactive = existingOBJ.isactive;
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblModuleData ON");
                        _context.ModuleData.Add(updatedObj);
                        _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblModuleData OFF");
                        await _context.SaveChangesAsync();
                    }
                }
                return Ok(new { message = $"{Obj.Count} Phase(s) processed successfully" });
            }
            catch (Exception ex)
            {
                // await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("DeleteID/{ID}")]
        public async Task<IActionResult> DeleteID(int ID)
        {
            try
            {
                int t = (int)ModuleTypes.Phases;
                string ModuleName = ModuleTypes.Phases.ToString();
                var existsOBJ = await _context.ModuleData.Where(p => p.TypeID == t && p.ID == ID && p.ID > 20).FirstOrDefaultAsync();
                if (existsOBJ == null)
                {
                    return BadRequest($"{ModuleName.ToString()} Data not Exists with this ID {ID}.");
                }

                // Update properties 
                existsOBJ.isactive = false;
                existsOBJ.updatedon = DateTime.Now;
                existsOBJ.updatedby = 1;

                _context.Entry(existsOBJ).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(new { message = $"{ModuleName.ToString()} Data {existsOBJ.Name} inactive successfully" });
            }
            catch (Exception ex)
            {
                //     await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }

    }


    [Route("api/[controller]")]
    [ApiController]
    public class ContractorTypeController : Controller
    {
        private readonly AppDBContext _context;

        public ContractorTypeController(AppDBContext context)
        {
            _context = context;
        }

        // PUT: api/products/5
        [HttpPost("{UpdateContractorType}/{id}")]
        public async Task<IActionResult> UpdateContractorType(int id, [FromBody] ContractorType updatedContractorType)
        {
            try
            {
                if (id != updatedContractorType.ID)
                {
                    return BadRequest("ContractorType ID mismatch.");
                }

                var existingContractorType = await _context.ModuleData.FindAsync(id);
                if (existingContractorType == null)
                {
                    return NotFound();
                }
                int t = (int)ModuleTypes.ContractorType;
                var existOBJ = await _context.ModuleData.Where(p => p.ID != id && p.Name.ToString() == updatedContractorType.Name.ToString() && updatedContractorType.TypeID.ToString() == t.ToString()).ToListAsync();
                if (existOBJ != null)
                {
                    if (existOBJ.Count > 0)
                        return BadRequest("Contractor Type Already Exists.");
                }
                // Update properties
                existingContractorType.Name = updatedContractorType.Name;
                existingContractorType.Code = updatedContractorType.Code;
                existingContractorType.ParentID = updatedContractorType.ParentID;
                existingContractorType.isactive = updatedContractorType.isactive;

                _context.Entry(existingContractorType).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(existingContractorType);
            }
            catch (Exception ex)
            {
                // await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpPost("AddContractorType")]
        public async Task<IActionResult> AddContractorType([FromBody] ContractorType obj)
        {
            try
            {
                var existingContractorType = await _context.ModuleData.FindAsync(0);
                if (existingContractorType != null)
                {
                    return BadRequest("ContractorType Exists.");
                }
                int t = (int)ModuleTypes.ContractorType;
                var existOBJ = await _context.ModuleData.Where(p => p.Name.ToString() == obj.Name.ToString() && obj.TypeID.ToString() == t.ToString()).ToListAsync();
                if (existOBJ != null)
                {
                    if (existOBJ.Count > 0)
                        return BadRequest("Contractor Type Already Exists.");
                }
                // Turn IDENTITY_INSERT ON
                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblModuleData ON");
                _context.Add(obj);
                // Turn IDENTITY_INSERT ON
                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblModuleData OFF");
                await _context.SaveChangesAsync();
                return Ok(new { message = $"{obj.ID} Contractor Type processed successfully" });
            }
            catch (Exception ex)
            {
                // await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }


        // GET: api/products 
        [HttpGet]
        public async Task<IActionResult> GetAllContractorTypes()
        {
            //var ContractorTypes = await _context.ContractorTypes.ToListAsync();
            int objval = (int)commonTypes.ModuleTypes.ContractorType;
            var OBJs = await _context.ModuleData.Where(n => n.TypeID == objval && n.ID > objval).OrderByDescending(p => p.ID).ToListAsync();
            return Ok(OBJs);
        }

        // GET: api/products 
        [HttpGet("ContractorType/{id}")]
        public async Task<IActionResult> ContractorType(int id)
        {
            //var ContractorTypes = await _context.ContractorTypes.ToListAsync();
            int objval = (int)commonTypes.ModuleTypes.ContractorType;
            var OBJs = await _context.ModuleData.Where(n => n.TypeID == objval && n.ID == id && n.ID > objval).OrderByDescending(p => p.ID).ToListAsync();
            return Ok(OBJs);
        }

        [HttpPost("bulkSaveContractorType")]
        public async Task<IActionResult> BulkSave([FromBody] List<ContractorType> Obj)
        {
            //if (Vehicles == null || !Vehicles.Any())
            //     return "BadRequest("No Vehicles provided")";

            //  using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var updatedObj in Obj)
                {
                    var existingOBJ = await _context.ModuleData
                        .FirstOrDefaultAsync(v => v.ID == updatedObj.ID);

                    if (existingOBJ != null)
                    {
                        // Update properties 
                        existingOBJ.Code = updatedObj.Code;
                        existingOBJ.Name = updatedObj.Name;
                        existingOBJ.isactive = updatedObj.isactive;
                        existingOBJ.createdby = 0;
                        existingOBJ.updatedby = 0;
                        existingOBJ.createdon = DateTime.Now;
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblModuleData ON");
                        _context.ModuleData.Add(updatedObj);
                        _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblModuleData OFF");
                        await _context.SaveChangesAsync();

                    }
                }

                // await transaction.CommitAsync();

                return Ok(new { message = $"{Obj.Count} Contractor Type(s) processed successfully" });
            }
            catch (Exception ex)
            {
                // await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpPost("DeleteID/{ID}")]
        public async Task<IActionResult> DeleteID(int ID)
        {
            try
            {
                int t = (int)ModuleTypes.ContractorType;
                string ModuleName = ModuleTypes.ContractorType.ToString();
                var existsOBJ = await _context.ModuleData.Where(p => p.TypeID == t && p.ID == ID && p.ID > 20).FirstOrDefaultAsync();
                if (existsOBJ == null)
                {
                    return BadRequest($"{ModuleName.ToString()} Data not Exists with this ID {ID}.");
                }

                // Update properties 
                existsOBJ.isactive = false;
                existsOBJ.updatedon = DateTime.Now;
                existsOBJ.updatedby = 1;

                _context.Entry(existsOBJ).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(new { message = $"{ModuleName.ToString()} Data {existsOBJ.Name} inactive successfully" });
            }
            catch (Exception ex)
            {
                //     await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }

    }

    [Route("api/[controller]")]
    [ApiController]
    public class ReaderController : Controller
    {
        private readonly AppDBContext _context;

        public ReaderController(AppDBContext context)
        {
            _context = context;
        }

        // PUT: api/products/5
        [HttpPost("{UpdateReader}/{id}")]
        public async Task<IActionResult> UpdateReader(int id, [FromBody] Reader updatedObj)
        {
            try
            {
                if (id != updatedObj.ID)
                {
                    return BadRequest("{updatedObj.code} ID mismatch.");
                }

                var existingOBJ = await _context.Readers.FindAsync(id);
                if (existingOBJ == null)
                {
                    return NotFound();
                }
                var existOBJ = await _context.Readers.Where(p => p.ID.ToString() != id.ToString()
                && p.code.ToString() == updatedObj.code.ToString()).ToListAsync();
                if (existOBJ != null)
                {
                    if (existOBJ.Count > 0)
                        return BadRequest("Reader with same code Already Exists.");
                }

                // Update properties         
                existingOBJ.code = updatedObj.code;
                existingOBJ.readerip = updatedObj.readerip;
                existingOBJ.readerport = updatedObj.readerport; ;
                existingOBJ.readermode = updatedObj.readermode; ;
                existingOBJ.readerLocation = updatedObj.readerLocation;
                existingOBJ.readertype = updatedObj.readertype;
                existingOBJ.isActive = updatedObj.isActive;
                existingOBJ.IsDeleted = updatedObj.IsDeleted;
 
                _context.Entry(existingOBJ).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(existingOBJ);
            }
            catch (Exception ex)
            {
                //     await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpPost("AddReader")]
        public async Task<IActionResult> AddReader([FromBody] Reader obj)
        {
            try
            {
                var existingReader = await _context.Readers.FindAsync(0);
                if (existingReader != null)
                {
                    return BadRequest("Reader Exists.");
                }
                 
                var existOBJ = await _context.Readers.Where(p =>( p.code.ToString() == obj.code.ToString()  && p.ID !=obj.ID)).ToListAsync();
                if (existOBJ != null)
                {
                    if (existOBJ.Count > 0)
                        return BadRequest("Reader code Already Exists.");
                }

                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblReader ON");
                _context.Add(obj);
                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblReader OFF");
                await _context.SaveChangesAsync();

                return Ok(new { message = $"{obj.ID} Reader processed successfully" });
            }
            catch (Exception ex)
            {
                // await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }


        // GET: api/products 
        //[HttpGet]
        //public async Task<IActionResult> GetAllReaders()
        //{

        //    // var Readers = await _context.Readers.Where(p => (p.isActive==true)).OrderByDescending(p => p.code).ToListAsync();

        //     var OBJs = await _context.Readers.OrderByDescending(p => p.code).ToListAsync();
        //    if (OBJs != null && OBJs.Count > 0)
        //    {
        //        int objval = (int)commonTypes.ModuleTypes.ReaderLocations;
        //        foreach (var res in OBJs)
        //        {
        //            //var nr = await _context.ModuleData.Where(u => u.Name.ToString() == res.readerLocation.ToString()).FirstOrDefaultAsync();
        //            //if (nr != null)
        //            //    res.ParentName = (string)nr.Name.ToString();

        //            var tOBJs = from Mod in _context.ModuleData
        //                       join Prnt in _context.ModuleData on Mod.ParentID equals Prnt.ID
        //                       where Mod.Name ==res.readerLocation && Mod.ModuleType == "LOC"
        //                       select new
        //                       {
        //                          nrdName = Prnt.Name
        //                       };

        //        }
        //    }




        //    return Ok(OBJs);
        //}

        // GET: api/products 
        //[HttpGet]
        //public async Task<IActionResult> GetAllReaders()
        //{
        //    var OBJs = await (
        //        from r in _context.Readers
        //        join loc in _context.ModuleData on r.readerLocation equals loc.Name into locJoin
        //        from loc in locJoin.DefaultIfEmpty()
        //        join nrd in _context.ModuleData on loc.ParentID equals nrd.ID into nrdJoin
        //        from nrd in nrdJoin.DefaultIfEmpty()
        //        select new
        //        {
        //            r.ID,
        //            r.code,
        //            r.readerip,
        //            r.readerport,
        //            r.readermode,
        //            r.readerLocation,
        //            r.readertype,
        //            r.isActive,
        //            r.IsDeleted,
        //            nrdName = nrd != null ? nrd.Name : null
        //        }
        //    ).OrderByDescending(p => p.code).ToListAsync();

        //    // Live TCP check for each reader, run in parallel
        //    var checkTasks = OBJs.Select(async r =>
        //    {
        //        bool online = await IsPortReachableAsync(r.readerip, r.readerport, 1200);
        //        return new
        //        {
        //            r.ID,
        //            r.code,
        //            r.readerip,
        //            r.readerport,
        //            r.readermode,
        //            r.readerLocation,
        //            r.readertype,
        //            r.isActive,
        //            r.IsDeleted,
        //            r.nrdName,
        //            status = online ? "Connected" : "Disconnected"
        //        };
        //    }).ToList();

        //    var results = await Task.WhenAll(checkTasks);

        //    return Ok(results);
        //}

        [HttpGet]
        public async Task<IActionResult> GetAllReaders()
        {
            var OBJs = await (
                from r in _context.Readers

                    // Only match Reader Location records (LOC)
                join loc in _context.ModuleData
                    .Where(x => x.ModuleType == "LOC")
                    on r.readerLocation equals loc.Name into locJoin

                from loc in locJoin.DefaultIfEmpty()

                    // Get NRD/Parent using Reader Location's ParentID
                join nrd in _context.ModuleData
                    on loc.ParentID equals nrd.ID into nrdJoin

                from nrd in nrdJoin.DefaultIfEmpty()

                select new
                {
                    r.ID,
                    r.code,
                    r.readerip,
                    r.readerport,
                    r.readermode,
                    r.readerLocation,
                    r.readertype,
                    r.isActive,
                    r.IsDeleted,

                    nrdName = nrd != null ? nrd.Name : null
                }
            )
            .OrderByDescending(p => p.code)
            .ToListAsync();

            // Live TCP check for each reader, run in parallel
            var checkTasks = OBJs.Select(async r =>
            {
                bool online = await IsPortReachableAsync(
                    r.readerip,
                    r.readerport,
                    1200
                );

                return new
                {
                    r.ID,
                    r.code,
                    r.readerip,
                    r.readerport,
                    r.readermode,
                    r.readerLocation,
                    r.readertype,
                    r.isActive,
                    r.IsDeleted,
                    r.nrdName,
                    status = online ? "Connected" : "Disconnected"
                };
            }).ToList();

            var results = await Task.WhenAll(checkTasks);

            return Ok(results);
        }

        private async Task<bool> IsPortReachableAsync(string ip, string port, int timeoutMs)
        {
            if (string.IsNullOrWhiteSpace(ip) || string.IsNullOrWhiteSpace(port) || !int.TryParse(port, out int portNum))
                return false;

            try
            {
                using var client = new System.Net.Sockets.TcpClient();
                var connectTask = client.ConnectAsync(ip, portNum);
                var timeoutTask = Task.Delay(timeoutMs);
                var completed = await Task.WhenAny(connectTask, timeoutTask);

                return completed == connectTask && client.Connected;
            }
            catch
            {
                return false;
            }
        }


        [HttpGet("AllDeviceStatus")]
        public async Task<IActionResult> GetAllDeviceStatus()
        {
            var devices = new List<object>();
            var conn = _context.Database.GetDbConnection();
            bool wasClosed = conn.State != System.Data.ConnectionState.Open;
            if (wasClosed) await conn.OpenAsync();

            try
            {
                // ── Controllers (Readers) — status from tblReaderStatus ──
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                SELECT r.ID, r.code, r.readerip, r.readerport, r.readermode,
                       r.readerLocation, r.readertype,
                       ISNULL(s.CurrentStatus, 'Disconnected') AS Status,
                       s.StatusOn
                FROM tblReader r
                LEFT JOIN tblReaderStatus s ON s.code = r.code
                WHERE r.IsDeleted = 0";
                    using var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        devices.Add(new
                        {
                            DeviceCategory = "Controller",
                            ID = reader["ID"],
                            Code = reader["code"],
                            IP = reader["readerip"],
                            Port = reader["readerport"],
                            Mode = reader["readermode"],
                            Location = reader["readerLocation"],
                            Type = reader["readertype"],
                            Status = reader["Status"].ToString(),
                            LastUpdated = reader["StatusOn"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["StatusOn"]
                        });
                    }
                }

                // ── Cameras — status derived from last_updatedOn recency ──
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                SELECT ID, ANPRTerminalID, ANPRIP, ANPRport, ANPRLocation,
                       ANPRMode, isActive, IsDeleted, last_downloadedOn, last_updatedOn
                FROM tblANPR_Controller
                WHERE IsDeleted = 0";
                    using var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        DateTime? lastUpdated = reader["last_updatedOn"] == DBNull.Value
                            ? (DateTime?)null : (DateTime)reader["last_updatedOn"];
                        bool isOnline = lastUpdated.HasValue && (DateTime.Now - lastUpdated.Value).TotalMinutes <= 5;

                        devices.Add(new
                        {
                            DeviceCategory = "Camera",
                            ID = reader["ID"],
                            Code = reader["ANPRTerminalID"],
                            IP = reader["ANPRIP"],
                            Port = reader["ANPRport"],
                            Mode = reader["ANPRMode"],
                            Location = reader["ANPRLocation"],
                            Type = "ANPR",
                            Status = isOnline ? "Connected" : "Disconnected",
                            LastUpdated = lastUpdated
                        });
                    }
                }
            }
            finally
            {
                if (wasClosed) await conn.CloseAsync();
            }

            return Ok(devices);
        }

        // GET: api/products 
        [HttpGet("GetReader/{id}")]
        public async Task<IActionResult> GetReader(int id)
        {
            var Readers = await _context.Readers.Where(p => (p.ID == id )).OrderByDescending(p => p.ID).ToListAsync();
            return Ok(Readers);
        }

        [HttpPost("bulkSaveReader")]
        public async Task<IActionResult> BulkSave([FromBody] List<Reader> Obj)
        {
            //if (Vehicles == null || !Vehicles.Any())
            //     return "BadRequest("No Vehicles provided")";

            //  using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var updatedObj in Obj)
                {
                    var existingOBJ = await _context.Readers
                        .FirstOrDefaultAsync(v => v.ID == updatedObj.ID);

                   

                    if (existingOBJ != null)
                    {
                        var existOBJ = await _context.Readers.Where(p =>p.ID!=updatedObj.ID && p.code.ToString() == updatedObj.code.ToString()).ToListAsync();
                        if (existOBJ != null)
                        {
                            if (existOBJ.Count > 0)
                                return BadRequest("Reader code Already Exists.");
                        }

                        // Update properties 
                        existingOBJ.code = updatedObj.code;
                        existingOBJ.readerip = updatedObj.readerip;
                        existingOBJ.readerport = updatedObj.readerport; ;
                        existingOBJ.readermode = updatedObj.readermode; ;
                        existingOBJ.readerLocation = updatedObj.readerLocation;
                        existingOBJ.readertype = updatedObj.readertype;
                        existingOBJ.isActive = updatedObj.isActive;
                        existingOBJ.IsDeleted = updatedObj.IsDeleted;

                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblReader ON");
                        _context.Readers.Add(updatedObj);
                        _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblReader OFF");
                        await _context.SaveChangesAsync();
                    }
                }
                return Ok(new { message = $"{Obj.Count} Reader(s) processed successfully" });
            }
            catch (Exception ex)
            {
                // await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("DeleteReader/{ID}")] 
        public async Task<IActionResult> DeleteReader(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Reader ID mismatch.");
                }
                var existingOBJ = await _context.Readers.FindAsync(id);
                if (existingOBJ == null)
                {
                    return NotFound();
                }
                existingOBJ.isActive = false;
                existingOBJ.IsDeleted = true;   

                _context.Entry(existingOBJ).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new { message = $"device with ID {existingOBJ.readerip.ToString()}  inactive & deleted successfully" });
            }
            catch (Exception ex)
            {
                //     await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }


    }


    [Route("api/[controller]")]
    [ApiController]
    public class VehicleMakeController : Controller
    {
        private readonly AppDBContext _context;

        public VehicleMakeController(AppDBContext context)
        {
            _context = context;
        }


        // PUT: api/products/5
        [HttpPost("{UpdateVehicleMake}/{id}")]
        public async Task<IActionResult> UpdateVehicleMake(int id, [FromBody] VehicleMake updatedVehicleMake)
        {
            if (id != updatedVehicleMake.ID)
            {
                return BadRequest("VehicleMake ID mismatch.");
            }

            var existingVehicleMake = await _context.ModuleData.FindAsync(id);
            if (existingVehicleMake == null)
            {
                return NotFound();
            }
            int t = (int)ModuleTypes.VehicleMake;
            var existOBJ = await _context.ModuleData.Where(p => p.ID != id && p.Name.ToString() == updatedVehicleMake.Name.ToString() && updatedVehicleMake.TypeID.ToString() == t.ToString()).ToListAsync();
            if (existOBJ != null)
            {
                return BadRequest("Vehicle Make Already Exists.");
            }

            // Update properties
            existingVehicleMake.Name = updatedVehicleMake.Name;
            existingVehicleMake.Code = updatedVehicleMake.Code;
            existingVehicleMake.ParentID = updatedVehicleMake.ParentID;
            existingVehicleMake.isactive = updatedVehicleMake.isactive;
            existingVehicleMake.updatedby = updatedVehicleMake.updatedby;
            existingVehicleMake.updatedon = DateTime.Now;

            _context.Entry(existingVehicleMake).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(existingVehicleMake);
        }


        [HttpPost("AddVehicleMake")]
        public async Task<IActionResult> AddVehicleMake([FromBody] VehicleMake obj)
        {
            var existingVehicleMake = await _context.ModuleData.FindAsync(0);
            if (existingVehicleMake != null)
            {
                return BadRequest("VehicleMake Exists.");
            }
            int t = (int)ModuleTypes.VehicleMake;
            var existOBJ = await _context.ModuleData.Where(p => p.Name.ToString() == obj.Name.ToString() && obj.TypeID.ToString() == t.ToString()).ToListAsync();
            if (existOBJ != null)
            {
                return BadRequest("Vehicle Make Already Exists.");
            }
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT tblModuleData ON");
                _context.Add(obj);
                await _context.SaveChangesAsync();
                await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT tblModuleData OFF");
                await transaction.CommitAsync();
            }

            return Ok();
        }

        // GET: api/products 
        [HttpGet]
        public async Task<IActionResult> GetAllVehicleMakes()
        {
            int objval = (int)commonTypes.ModuleTypes.VehicleMake;
            var OBJs = await _context.ModuleData.Where(n => n.TypeID == objval && n.ID > objval).OrderByDescending(p => p.ID).ToListAsync();
            return Ok(OBJs);
        }

        // GET: api/products 
        [HttpGet("GetVehicle/{id}")]
        public async Task<IActionResult> GetVehicle(int id)
        {
            int objval = (int)commonTypes.ModuleTypes.VehicleMake;
            var OBJs = await _context.ModuleData.Where(n => n.TypeID == objval && n.ID == id && n.ID > objval).OrderByDescending(p => p.ID).ToListAsync();
            return Ok(OBJs);
        }


        [HttpPost("bulkSaveVehicleMake")]
        public async Task<IActionResult> BulkSave([FromBody] List<VehicleMake> Obj)
        {
            //if (Vehicles == null || !Vehicles.Any())
            //     return "BadRequest("No Vehicles provided")";

            //  using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var updatedObj in Obj)
                {
                    var existingOBJ = await _context.ModuleData
                        .FirstOrDefaultAsync(v => v.ID == updatedObj.ID);

                    if (existingOBJ != null)
                    {
                        // Update properties 

                        existingOBJ.Code = updatedObj.Code;
                        existingOBJ.Name = updatedObj.Name;
                        existingOBJ.isactive = updatedObj.isactive;
                        existingOBJ.createdby = 0;
                        existingOBJ.updatedby = 0;
                        existingOBJ.createdon = DateTime.Now;
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblModuleData ON");
                        _context.ModuleData.Add(updatedObj);
                        _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblModuleData OFF");
                        await _context.SaveChangesAsync();

                    }
                }

                // await transaction.CommitAsync();

                return Ok(new { message = $"{Obj.Count} Vehicle Make processed successfully" });
            }
            catch (Exception ex)
            {
                // await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("DeleteID/{ID}")]
        public async Task<IActionResult> DeleteID(int ID)
        {
            try
            {
                int t = (int)ModuleTypes.VehicleMake;
                string ModuleName = ModuleTypes.VehicleMake.ToString();
                var existsOBJ = await _context.ModuleData.Where(p => p.TypeID == t && p.ID == ID && p.ID > 20).FirstOrDefaultAsync();
                if (existsOBJ == null)
                {
                    return BadRequest($"{ModuleName.ToString()} Data not Exists with this ID {ID}.");
                }

                // Update properties 
                existsOBJ.isactive = false;
                existsOBJ.updatedon = DateTime.Now;
                existsOBJ.updatedby = 1;

                _context.Entry(existsOBJ).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(new { message = $"{ModuleName.ToString()} Data {existsOBJ.Name} inactive successfully" });
            }
            catch (Exception ex)
            {
                //     await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }

  }

    [Route("api/[controller]")]
    [ApiController]
    public class ReaderLocationController : Controller
    {
        private readonly AppDBContext _context;

        public ReaderLocationController(AppDBContext context)
        {
            _context = context;
        }


        // PUT: api/products/5
        [HttpPost("{UpdateReaderLocation}/{id}")]
        public async Task<IActionResult> UpdateReaderLocation(int id, [FromBody] ReaderLocation updatedReaderLocation)
        {
            if (id != updatedReaderLocation.ID)
            {
                return BadRequest("ReaderLocation ID mismatch.");
            }

            var existingReaderLocation = await _context.ModuleData.FindAsync(id);
            if (existingReaderLocation == null)
            {
                return NotFound();
            }
            //int t = (int)ModuleTypes.ReaderLocations;
            //var existOBJ = await _context.ModuleData.Where(p => p.ID != id && p.Name.ToString() == updatedReaderLocation.Name.ToString() && updatedReaderLocation.TypeID.ToString() == t.ToString()).ToListAsync();
            int t = (int)ModuleTypes.ReaderLocations;
            var existOBJ = await _context.ModuleData.Where(p => p.ID != id && p.TypeID == t && p.Name.ToString() == updatedReaderLocation.Name.ToString()).ToListAsync();
            if (existOBJ != null)
            {
                if (existOBJ.Count>0)
                {
                    return BadRequest("Reader location Already Exists with another ID.");
                }
            }
            // Update properties
            existingReaderLocation.Name = updatedReaderLocation.Name;
            existingReaderLocation.Code = updatedReaderLocation.Code;
            existingReaderLocation.ParentID = updatedReaderLocation.ParentID;
            existingReaderLocation.isactive = updatedReaderLocation.isactive;
            existingReaderLocation.updatedon = DateTime.Now;
            existingReaderLocation.updatedby = updatedReaderLocation.updatedby;

            _context.Entry(existingReaderLocation).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(existingReaderLocation);
        }


        //[HttpPost("AddReaderLocation")]
        //public async Task<IActionResult> AddReaderLocation([FromBody] ReaderLocation obj)
        //{
        //    var existingReaderLocation = await _context.ModuleData.FindAsync(0);
        //    if (existingReaderLocation != null)
        //    {
        //        return BadRequest("ReaderLocation Exists.");
        //    }
        //    int t = (int)ModuleTypes.ReaderLocations;
        //    var existOBJ = await _context.ModuleData.Where(p =>p.ID!=obj.ID && p.Name.ToString() == obj.Name.ToString() && obj.TypeID.ToString() == t.ToString()).ToListAsync();
        //    if (existOBJ != null)
        //    {
        //        if(existOBJ.Count>0)
        //        return BadRequest("Reader Location Already Exists.");
        //    }

        //    _context.Add(obj);
        //    await _context.SaveChangesAsync();

        //    return Ok();
        //}


        [HttpPost("AddReaderLocation")]
        public async Task<IActionResult> AddReaderLocation([FromBody] ReaderLocation obj)
        {
            int t = (int)ModuleTypes.ReaderLocations;

            var existOBJ = await _context.ModuleData.Where(p =>
                p.TypeID == t &&
                p.Name.ToString() == obj.Name.ToString())
                .ToListAsync();

            if (existOBJ.Count > 0)
            {
                return BadRequest("Reader Location Already Exists.");
            }

            _context.Add(obj);
            await _context.SaveChangesAsync();

            return Ok();
        }


        // GET: api/products 
        [HttpGet]
        public async Task<IActionResult> GetAllReaderLocations()
        {
            int objval = (int)commonTypes.ModuleTypes.ReaderLocations;

            //var OBJs = await _context.ModuleData.Where(n => n.TypeID == objval && n.ID > objval).OrderByDescending(p => p.ID).ToListAsync();
            var OBJs = from Mod in _context.ModuleData
                       join Prnt in _context.ModuleData on Mod.ParentID equals Prnt.ID
                       where Mod.ID > objval && Mod.ModuleType == "LOC"
                       select new
                       {
                           Mod.ID,
                           Mod.Name,
                           Mod.Code,
                           Mod.TypeID,
                           Mod.ParentID,
                           Mod.ModuleType,
                           nrdName = Prnt.Name,
                           Mod.isactive,
                           Mod.createdby,
                           Mod.createdon,
                           Mod.updatedby,
                           Mod.updatedon
                       };


            return Ok(OBJs);
        }

        // GET: api/products 
        [HttpGet("GetReaderLocation/{id}")]
        public async Task<IActionResult> GetReaderLocation(int id)
        {
            int objval = (int)commonTypes.ModuleTypes.ReaderLocations;
            var OBJs = await _context.ModuleData.Where(n => n.TypeID == objval && n.ID == id && n.ID > objval).OrderByDescending(p => p.ID).ToListAsync();
            return Ok(OBJs);
        }


        [HttpPost("bulkSaveReaderLocation")]
        public async Task<IActionResult> BulkSave([FromBody] List<ReaderLocation> Obj)
        {
            try
            {
                foreach (var updatedObj in Obj)
                {
                    var existingOBJ = await _context.ModuleData
                        .FirstOrDefaultAsync(v => v.ID == updatedObj.ID);

                    if (existingOBJ != null)
                    {
                        // Update properties 
                        existingOBJ.Code = updatedObj.Code;
                        existingOBJ.Name = updatedObj.Name;
                        existingOBJ.isactive = updatedObj.isactive;
                        existingOBJ.updatedby = updatedObj.updatedby;
                        existingOBJ.updatedon = DateTime.Now;
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblModuleData ON");
                        _context.ModuleData.Add(updatedObj);
                        _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblModuleData OFF");
                        await _context.SaveChangesAsync();
                    }
                }

                // await transaction.CommitAsync();

                return Ok(new { message = $"{Obj.Count} Reader Locations processed successfully" });
            }
            catch (Exception ex)
            {
                // await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("DeleteID/{ID}")]
        public async Task<IActionResult> DeleteID(int ID)
        {
            try
            {
                int t = (int)ModuleTypes.ReaderLocations;
                string ModuleName = ModuleTypes.ReaderLocations.ToString();
                var existsOBJ = await _context.ModuleData.Where(p => p.TypeID == t && p.ID == ID && p.ID > 20).FirstOrDefaultAsync();
                if (existsOBJ == null)
                {
                    return BadRequest($"{ModuleName.ToString()} Data not Exists with this ID {ID}.");
                }

                // Update properties 
                existsOBJ.isactive = false;
                existsOBJ.updatedon = DateTime.Now;
                existsOBJ.updatedby = 1;

                _context.Entry(existsOBJ).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(new { message = $"{ModuleName.ToString()} Data {existsOBJ.Name} inactive successfully" });
            }
            catch (Exception ex)
            {
                //     await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }

    }

    [Route("api/[controller]")]
    [ApiController]
    public class ReaderRelayController : Controller
    {
        private readonly AppDBContext _context;

        public ReaderRelayController(AppDBContext context)
        {
            _context = context;
        }


        // PUT: api/products/5
        [HttpPost("{UpdateReaderRelay}/{id}")]
        public async Task<IActionResult> UpdateReaderRelay(int id, [FromBody] ReaderRelay updatedReaderRelay)
        {
            if (id != updatedReaderRelay.ID)
            {
                return BadRequest("ReaderRelay ID mismatch.");
            }

            var existingReaderRelay = await _context.ModuleData.FindAsync(id);
            if (existingReaderRelay == null)
            {
                return NotFound();
            }
            int t = (int)ModuleTypes.ReaderRelays;
            var existOBJ = await _context.ModuleData.Where(p => p.ID == id && p.Name.ToString() == updatedReaderRelay.Name.ToString() && updatedReaderRelay.TypeID.ToString() == t.ToString()).ToListAsync();
            if (existOBJ != null)
            {
                if (existOBJ.Count > 0)
                    return BadRequest("Reader Location Already Exists.");
            }


            // Update properties
            existingReaderRelay.Name = updatedReaderRelay.Name;
            existingReaderRelay.Code = updatedReaderRelay.Code;
            existingReaderRelay.ParentID = updatedReaderRelay.ParentID;
            existingReaderRelay.isactive = updatedReaderRelay.isactive;
            existingReaderRelay.updatedby = updatedReaderRelay.updatedby;
            existingReaderRelay.updatedon = DateTime.Now;

            _context.Entry(existingReaderRelay).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(existingReaderRelay);
        }


        [HttpPost("AddReaderRelay")]
        public async Task<IActionResult> AddReaderRelay([FromBody] ReaderRelay obj)
        {
            try
            {
                var existingReaderRelay = await _context.ModuleData.FindAsync(0);
                if (existingReaderRelay != null)
                {
                    return BadRequest("ReaderRelay Exists.");
                }
                int t = (int)ModuleTypes.ReaderRelays;
                var existOBJ = await _context.ModuleData.Where(p => p.Name.ToString() == obj.Name.ToString() && p.TypeID.ToString() == t.ToString()).ToListAsync();
                if (existOBJ != null)
                {
                    if (existOBJ.Count > 0)
                        return BadRequest("Reader relay Already Exists.");
                }
                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblModuleData ON");
                _context.Add(obj);
                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblModuleData OFF");
                await _context.SaveChangesAsync();
                return Ok(new { message = $"{obj.ID} Reader Relay created successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        // GET: api/products 
        [HttpGet]
        public async Task<IActionResult> GetAllReaderRelays()
        {
            int objval = (int)commonTypes.ModuleTypes.ReaderRelays;
            var OBJs = await _context.ModuleData.Where(n => n.TypeID == objval && n.ID > objval).OrderByDescending(p => p.ID).ToListAsync();
            return Ok(OBJs);
        }

        [HttpGet("GetReaderRelay/{id}")]
        public async Task<IActionResult> GetReaderRelay(int id)
        {
            int objval = (int)commonTypes.ModuleTypes.ReaderRelays;
            var OBJs = await _context.ModuleData.Where(n => n.TypeID == objval && n.ID == id && n.ID > objval).OrderByDescending(p => p.ID).ToListAsync();
            return Ok(OBJs);
        }

        [HttpPost("bulkSaveReaderRelay")]
        public async Task<IActionResult> BulkSave([FromBody] List<ReaderRelay> Obj)
        {
            //if (Vehicles == null || !Vehicles.Any())
            //     return "BadRequest("No Vehicles provided")";

            //  using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var updatedObj in Obj)
                {
                    var existingOBJ = await _context.ModuleData
                        .FirstOrDefaultAsync(v => v.ID == updatedObj.ID);

                    if (existingOBJ != null)
                    {
                        // Update properties 
                        existingOBJ.Code = updatedObj.Code;
                        existingOBJ.Name = updatedObj.Name;
                        existingOBJ.isactive = updatedObj.isactive;
                        existingOBJ.updatedby = updatedObj.updatedby;
                        existingOBJ.updatedon = DateTime.Now;
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblModuleData ON");
                        _context.ModuleData.Add(updatedObj);
                        _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblModuleData OFF");
                        await _context.SaveChangesAsync();
                    }
                }
                // await transaction.CommitAsync();

                return Ok(new { message = $"{Obj.Count} Vehicles processed successfully" });
            }
            catch (Exception ex)
            {
                // await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("DeleteID/{ID}")]
        public async Task<IActionResult> DeleteID(int ID)
        {
            try
            {
                int t = (int)ModuleTypes.ReaderRelays;
                string ModuleName = ModuleTypes.ReaderRelays.ToString();
                var existsOBJ = await _context.ModuleData.Where(p => p.TypeID == t && p.ID == ID && p.ID > 20).FirstOrDefaultAsync();
                if (existsOBJ == null)
                {
                    return BadRequest($"{ModuleName.ToString()} Data not Exists with this ID {ID}.");
                }

                // Update properties 
                existsOBJ.isactive = false;
                existsOBJ.updatedon = DateTime.Now;
                existsOBJ.updatedby = 1;

                _context.Entry(existsOBJ).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(new { message = $"{ModuleName.ToString()} Data {existsOBJ.Name} inactive successfully" });
            }
            catch (Exception ex)
            {
                //     await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class ServiceTypeController : Controller
    {
        private readonly AppDBContext _context;

        public ServiceTypeController(AppDBContext context)
        {
            _context = context;
        }


        // PUT: api/products/5
        [HttpPost("{UpdateServiceType}/{id}")]
        public async Task<IActionResult> UpdateServiceType(int id, [FromBody] ServiceType updatedServiceType)
        {
            if (id != updatedServiceType.ID)
            {
                return BadRequest("ServiceType ID mismatch.");
            }

            var existingServiceType = await _context.ModuleData.FindAsync(id);
            if (existingServiceType == null)
            {
                return NotFound();
            }
            int t = (int)ModuleTypes.ServiceType;
            var existOBJ = await _context.ModuleData.Where(p => p.ID != id && p.Name.ToString() == updatedServiceType.Name.ToString() && p.TypeID.ToString() == t.ToString()).ToListAsync();
            if (existOBJ != null)
            {
                if (existOBJ.Count > 0)
                    return BadRequest("Service Type Already Exists.");
            }
            // Update properties
            existingServiceType.Name = updatedServiceType.Name;
            existingServiceType.Code = updatedServiceType.Code;
            existingServiceType.ParentID = updatedServiceType.ParentID;
            existingServiceType.isactive = updatedServiceType.isactive;

            _context.Entry(existingServiceType).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(existingServiceType);
        }


        [HttpPost("AddServiceType")]
        public async Task<IActionResult> AddServiceType([FromBody] ServiceType obj)
        {
            try
            {
                var existingServiceType = await _context.ModuleData.FindAsync(0);
                if (existingServiceType != null)
                {
                    return BadRequest("ServiceType Exists.");
                }
                int t = (int)ModuleTypes.ServiceType;
                var existOBJ = await _context.ModuleData.Where(p => p.Name.ToString() == obj.Name.ToString() && p.TypeID.ToString() == t.ToString()).ToListAsync();
                if (existOBJ != null)
                {
                    if (existOBJ.Count > 0)
                        return BadRequest("Service Type Already Exists.");
                }
                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblModuleData ON");
                _context.Add(obj);
                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblModuleData OFF");
                await _context.SaveChangesAsync();

                return Ok(new { message = $"{obj.ID} ServiceType processed successfully" });
            }
            catch (Exception ex)
            {
                // await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }


        // GET: api/products 
        [HttpGet]
        public async Task<IActionResult> GetAllServiceTypes()
        {
            int objval = (int)commonTypes.ModuleTypes.ServiceType;
            var OBJs = await _context.ModuleData.Where(n => n.TypeID == objval && n.ID > objval).OrderByDescending(p => p.ID).ToListAsync();
            return Ok(OBJs);
        }

        [HttpGet("GetServiceType/{id}")]
        public async Task<IActionResult> GetServiceType(int id)
        {
            int objval = (int)commonTypes.ModuleTypes.ServiceType;
            var OBJs = await _context.ModuleData.Where(n => n.TypeID == objval && n.ID == id && n.ID > objval).OrderByDescending(p => p.ID).ToListAsync();
            return Ok(OBJs);
        }


        [HttpPost("bulkSaveServiceType")]
        public async Task<IActionResult> BulkSave([FromBody] List<ServiceType> Obj)
        {
            //if (Obj == null || !Obj.Any())
            //    return "BadRequest("No Service Type provided")";

            //  using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var updatedObj in Obj)
                {
                    var existingOBJ = await _context.ModuleData
                        .FirstOrDefaultAsync(v => v.ID == updatedObj.ID);

                    if (existingOBJ != null)
                    {
                        // Update properties 
                        existingOBJ.Code = updatedObj.Code;
                        existingOBJ.Name = updatedObj.Name;
                        existingOBJ.isactive = updatedObj.isactive;
                        existingOBJ.updatedby = updatedObj.updatedby;
                        existingOBJ.updatedon = DateTime.Now;
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblModuleData ON");
                        _context.ModuleData.Add(updatedObj);
                        _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblModuleData OFF");
                        await _context.SaveChangesAsync();
                    }
                }

                // await transaction.CommitAsync();

                return Ok(new { message = $"{Obj.Count} Vehicles processed successfully" });
            }
            catch (Exception ex)
            {
                // await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("DeleteID/{ID}")]
        public async Task<IActionResult> DeleteID(int ID)
        {
            try
            {
                int t = (int)ModuleTypes.ServiceType;
                string ModuleName = ModuleTypes.ServiceType.ToString();
                var existsOBJ = await _context.ModuleData.Where(p => p.TypeID == t && p.ID == ID && p.ID > 20).FirstOrDefaultAsync();
                if (existsOBJ == null)
                {
                    return BadRequest($"{ModuleName.ToString()} Data not Exists with this ID {ID}.");
                }

                // Update properties 
                existsOBJ.isactive = false;
                existsOBJ.updatedon = DateTime.Now;
                existsOBJ.updatedby = 1;

                _context.Entry(existsOBJ).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(new { message = $"{ModuleName.ToString()} Data {existsOBJ.Name} inactive successfully" });
            }
            catch (Exception ex)
            {
                //     await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }


    }

    [Route("api/[controller]")]
    [ApiController]
    public class BuildingController : Controller
    {
        private readonly AppDBContext _context;

        public BuildingController(AppDBContext context)
        {
            _context = context;
        }

        // PUT: api/products/5
        [HttpPost("{UpdateBuilding}/{id}")]
        public async Task<IActionResult> UpdateBuilding(int id, [FromBody] Building updatedBuilding)
        {
            try
            {
                if (id != updatedBuilding.ID)
                {
                    return BadRequest("Building ID mismatch.");
                }

                var existingBuilding = await _context.ModuleData.FindAsync(id);
                if (existingBuilding == null)
                {
                    return NotFound();
                }
                int t = (int)ModuleTypes.Building;
                var existOBJ = await _context.ModuleData.Where(p => p.ID != id && p.Name.ToString() == updatedBuilding.Name.ToString() && p.ParentID == updatedBuilding.ParentID && p.TypeID.ToString() == t.ToString()).ToListAsync();
                if (existOBJ != null)
                {
                    if (existOBJ.Count > 0)
                        return BadRequest("Building Name Already Exists.");
                }

                // Update properties
                existingBuilding.Name = updatedBuilding.Name;
                existingBuilding.Code = updatedBuilding.Code;
                existingBuilding.ParentID = updatedBuilding.ParentID;
                existingBuilding.isactive = updatedBuilding.isactive;
                existingBuilding.updatedby = updatedBuilding.updatedby;
                existingBuilding.updatedon = DateTime.Now;

                _context.Entry(existingBuilding).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(existingBuilding);
            }
            catch (Exception ex)
            {
                // await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message.ToString() });
            }
        }


        [HttpPost("AddBuilding")]
        public async Task<IActionResult> AddBuilding([FromBody] Building obj)
        {
            try
            {
                var existingBuilding = await _context.ModuleData.FindAsync(0);
                if (existingBuilding != null)
                {
                    return BadRequest("Building Exists.");
                }
                int t = (int)ModuleTypes.Building;
                var existOBJ = await _context.ModuleData.Where(p => p.Name.ToString() == obj.Name.ToString() && p.TypeID.ToString() == t.ToString()).ToListAsync();
                if (existOBJ != null)
                {
                    if (existOBJ.Count > 0)
                        return BadRequest("Building Name Already Exists.");
                }

                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblModuleData ON");
                _context.Add(obj);
                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblModuleData OFF");
                await _context.SaveChangesAsync();

                return Ok(new { message = $"{obj.ID} Building created successfully" });
            }
            catch (Exception ex)
            {
                // await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message.ToString() });
            }
        }


        // GET: api/products 
        [HttpGet]
        public async Task<IActionResult> GetAllBuildings()
        {
            int objval = (int)commonTypes.ModuleTypes.Building;
            //var OBJs = await _context.ModuleData.Where(n => n.TypeID == objval && n.ID > objval).OrderByDescending(p => p.ID).ToListAsync();
            var OBJs = from Mod in _context.ModuleData
                       join Prnt in _context.ModuleData on Mod.ParentID equals Prnt.ID
                       where Mod.ID > objval   && Mod.ModuleType == "BLD"
                       select new
                       {
                           Mod.ID,
                           Mod.Name,
                           Mod.Code,
                           Mod.TypeID,
                           Mod.ParentID,
                           Mod.ModuleType,
                           nrdName = Prnt.Name,
                           Mod.isactive,
                           Mod.createdby,
                           Mod.createdon,
                           Mod.updatedby,
                           Mod.updatedon
                       };



            //if (OBJs != null && OBJs.Count > 0)
            //{
            //    foreach (var res in OBJs)
            //    {
            //        var nr = await _context.ModuleData.Where(u => u.ID.ToString() == res.ParentID.ToString()).FirstOrDefaultAsync();
            //        if (nr != null)
            //            res.ParentName = (string)nr.Name.ToString();
            //    }
            //}

            return Ok(OBJs);
        }

        // GET: api/products 
        [HttpGet("GetBuilding/{id}")]
        public async Task<IActionResult> GetBuilding(int id)
        {
            int objval = (int)commonTypes.ModuleTypes.Building;
            var OBJs = await _context.ModuleData.Where(n => n.TypeID == objval && n.ID > objval && n.ID == id).OrderByDescending(p => p.ID).ToListAsync();
             
            return Ok(OBJs);
        }


        [HttpPost("bulkSaveBuilding")]
        public async Task<IActionResult> BulkSave([FromBody] List<Building> Obj)
        {
            //if (Vehicles == null || !Vehicles.Any())
            //     return "BadRequest("No Vehicles provided")";

            //using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var updatedObj in Obj)
                {
                    var existingOBJ = await _context.ModuleData
                        .FirstOrDefaultAsync(v => v.ID == updatedObj.ID);

                    if (existingOBJ != null)
                    {
                        // Update properties 
                        existingOBJ.Name = updatedObj.Name;
                        existingOBJ.Code = updatedObj.Code;
                        existingOBJ.ParentID = updatedObj.ParentID;
                        existingOBJ.isactive = updatedObj.isactive;
                        existingOBJ.updatedby = updatedObj.updatedby;
                        existingOBJ.updatedon = DateTime.Now;
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblModuleData ON");
                        _context.ModuleData.Add(updatedObj);
                        _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblModuleData OFF");
                        await _context.SaveChangesAsync();
                    }
                }

                return Ok(new { message = $"{Obj.Count} Records of Building processed successfully" });
            }
            catch (Exception ex)
            {
                //await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("DeleteID/{ID}")]
        public async Task<IActionResult> DeleteID(int ID)
        {
            try
            {
                int t = (int)ModuleTypes.Building;
                string ModuleName = ModuleTypes.Building.ToString();
                var existsOBJ = await _context.ModuleData.Where(p => p.TypeID == t && p.ID == ID && p.ID > 20).FirstOrDefaultAsync();
                if (existsOBJ == null)
                {
                    return BadRequest($"{ModuleName.ToString()} Data not Exists with this ID {ID}.");
                }

                // Update properties 
                existsOBJ.isactive = false;
                existsOBJ.updatedon = DateTime.Now;
                existsOBJ.updatedby = 1;

                _context.Entry(existsOBJ).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(new { message = $"{ModuleName.ToString()} Data {existsOBJ.Name} inactive successfully" });
            }
            catch (Exception ex)
            {
                //     await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }

    }

    [Route("api/[controller]")]
    [ApiController]
    public class AmenitiesController : ControllerBase
    {
        private readonly AppDBContext _context;

        public AmenitiesController(AppDBContext context)
        {
            _context = context;
        }


        // PUT: api/products/5
        [HttpPost("{UpdateAmenities}/{id}")]
        public async Task<IActionResult> UpdateAmenities(int id, [FromBody] Amenities updatedAmenities)
        {
            if (id != updatedAmenities.ID)
            {
                return BadRequest("Amenities ID mismatch.");
            }

            var existingAmenities = await _context.ModuleData.FindAsync(id);
            if (existingAmenities == null)
            {
                return NotFound();
            }
            var _existingAmenities = await _context.ModuleData.Where(p => p.ID != updatedAmenities.ID && p.Name == updatedAmenities.Name && p.ParentID == updatedAmenities.ParentID).ToListAsync();
            if (_existingAmenities != null)
            {
                if (_existingAmenities.Count() > 0)
                    return BadRequest("Amenities already registered with this NRD!");
            }

            // Update properties
            existingAmenities.Name = updatedAmenities.Name;
            existingAmenities.Code = updatedAmenities.Code;
            existingAmenities.ParentID = updatedAmenities.ParentID;
            existingAmenities.isactive = updatedAmenities.isactive;
            existingAmenities.updatedby = updatedAmenities.updatedby;
            existingAmenities.updatedon = DateTime.Now;

            _context.Entry(existingAmenities).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(existingAmenities);
        }


        [HttpPost("AddAmenities")]
        public async Task<IActionResult> AddAmenities([FromBody] Amenities obj)
        {
            try
            {
                var existingAmenities = await _context.ModuleData.FindAsync(0);
                if (existingAmenities != null)
                {
                    return BadRequest("Amenities Exists.");
                }
                int t = (int)ModuleTypes.Amenities;
                var _existingAmenities = await _context.ModuleData.Where(p => p.Name == obj.Name && p.ParentID == obj.ParentID && p.TypeID.ToString() == t.ToString()).ToListAsync();
                if (_existingAmenities != null)
                {
                    if (_existingAmenities.Count > 0)
                        return BadRequest("Amenities already registered with this NRD!");
                }

                // Turn IDENTITY_INSERT ON
                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblModuleData ON");
                _context.Add(obj); //_context.Add(obj);await _context.SaveChangesAsync();
                _context.Database.ExecuteSqlRaw("SET  IDENTITY_INSERT tblModuleData OFF");
                await _context.SaveChangesAsync();
                return Ok(new { message = $"{obj.ID} Records of Amenities processed successfully" });
            }
            catch (Exception ex)
            {
                //await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }


        // GET: api/products 
        [HttpGet]
        public async Task<IActionResult> GetAllAmenities()
        {
            int objval = (int)commonTypes.ModuleTypes.Amenities;
            var OBJs = await _context.ModuleData.Where(n => n.TypeID == objval && n.ID > objval).OrderByDescending(p => p.ID).ToListAsync();
            return Ok(OBJs);
        }

        [HttpGet("GetAmenity/{id}")]
        public async Task<IActionResult> GetAllAmenities(int id)
        {
            int objval = (int)commonTypes.ModuleTypes.Amenities;
            var OBJs = await _context.ModuleData.Where(n => n.TypeID == objval && n.ID > objval && n.ID == id).OrderByDescending(p => p.ID).ToListAsync();
            return Ok(OBJs);
        }


        [HttpPost("bulkSaveAmenities")]
        public async Task<IActionResult> BulkSave([FromBody] List<Amenities> Obj)
        {
            //if (Vehicles == null || !Vehicles.Any())
            //     return "BadRequest("No Vehicles provided")";

            //    using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var updatedObj in Obj)
                {
                    var existingOBJ = await _context.ModuleData
                        .FirstOrDefaultAsync(v => v.ID == updatedObj.ID);

                    var _existingAmenities = await _context.ModuleData.Where(p => p.ID != updatedObj.ID && p.Name == updatedObj.Name && p.ParentID == updatedObj.ParentID).ToListAsync();
                    if (_existingAmenities != null)
                    {
                        if (_existingAmenities.Count() > 0)
                            return BadRequest("Amenities already registered with this NRD!");
                    }

                    if (existingOBJ != null)
                    {
                        // Update properties 
                        existingOBJ.Name = updatedObj.Name;
                        existingOBJ.Code = updatedObj.Code;
                        existingOBJ.ParentID = updatedObj.ParentID;
                        existingOBJ.isactive = updatedObj.isactive;
                        existingOBJ.updatedby = updatedObj.updatedby;
                        existingOBJ.updatedon = DateTime.Now;
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        // Turn IDENTITY_INSERT ON
                        _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblModuleData ON");
                        _context.Add(updatedObj); //_context.Add(obj);await _context.SaveChangesAsync();
                        _context.Database.ExecuteSqlRaw("SET  IDENTITY_INSERT tblModuleData OFF");
                        await _context.SaveChangesAsync();
                    }
                }
                //        await transaction.CommitAsync();
                return Ok(new { message = $"{Obj.Count} bulk records processed successfully" });
            }
            catch (Exception ex)
            {
                // await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("DeleteID/{ID}")]
        public async Task<IActionResult> DeleteID(int ID)
        {
            try
            {
                int t = (int)ModuleTypes.Amenities;
                string ModuleName = ModuleTypes.Amenities.ToString();
                var existsOBJ = await _context.ModuleData.Where(p => p.TypeID == t && p.ID == ID && p.ID > 20).FirstOrDefaultAsync();
                if (existsOBJ == null)
                {
                    return BadRequest($"{ModuleName.ToString()} Data not Exists with this ID {ID}.");
                }

                // Update properties 
                existsOBJ.isactive = false;
                existsOBJ.updatedon = DateTime.Now;
                existsOBJ.updatedby = 1;

                _context.Entry(existsOBJ).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(new { message = $"{ModuleName.ToString()} Data {existsOBJ.Name} inactive successfully" });
            }
            catch (Exception ex)
            {
                //     await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }

    }
    
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : Controller
    {
        private readonly AppDBContext _context;

        public VehicleController(AppDBContext context)
        {
            _context = context;
        }

        // PUT: api/products/5
        [HttpPost("{UpdateVehicle}/{id}")]
        public async Task<IActionResult> UpdateVehicle(int id, [FromBody] Vehicle updatedVehicle)
        {
            if (id != updatedVehicle.ID)
            {
                return BadRequest("Vehicle ID mismatch.");
            }
            var existingVehicle = await _context.Vehicles.FindAsync(id);
            if (existingVehicle == null)
            {
                return NotFound();
            }
            var existingVeh = await _context.Vehicles.Where(p => p.RegNo == updatedVehicle.RegNo && p.ID != updatedVehicle.ID).ToListAsync();
            if (existingVeh != null)
            {
                if (existingVeh.Count() > 0)
                    return BadRequest("Vehicle RegNo already registered with another user!");
            }

            existingVehicle.RegNo = updatedVehicle.RegNo;
            existingVehicle.vType = updatedVehicle.vType;
            existingVehicle.vMake = updatedVehicle.vMake;
            existingVehicle.vColor = updatedVehicle.vColor;
            existingVehicle.TagUID = updatedVehicle.TagUID;
            existingVehicle.PrintedTagID = updatedVehicle.PrintedTagID;
            existingVehicle.TagEncodingDate = updatedVehicle.TagEncodingDate;
            existingVehicle.Logical_Delete = updatedVehicle.Logical_Delete;
            existingVehicle.StickerNo = updatedVehicle.StickerNo;
            existingVehicle.isactive = updatedVehicle.isactive;
            existingVehicle.updatedby = updatedVehicle.updatedby;
            existingVehicle.updatedon = updatedVehicle.updatedon;

            _context.Entry(existingVehicle).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(existingVehicle);
        }


        [HttpPost("AddVehicle")]
        public async Task<IActionResult> AddVehicle([FromBody] Vehicle obj)
        {
            var existingVehicle = await _context.Vehicles.FindAsync(0);
            if (existingVehicle != null)
            {
                return BadRequest("Vehicle record Exists.");
            }

            var existingVeh = await _context.Vehicles.Where(p => p.RegNo == obj.RegNo && p.ID != obj.ID && p.Logical_Delete == 0).ToListAsync();
            if (existingVeh != null)
            {
                if (existingVeh.Count() > 0)
                    return BadRequest("Vehicle RegNo already registered with another user!");
            }
            obj.createdon = DateTime.Now;
            obj.Logical_Delete = 0;

            // Turn IDENTITY_INSERT ON
            //_context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblVehicle ON");
            //_context.Add(obj);
            // Turn IDENTITY_INSERT ON
            //_context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblVehicle OFF");
            //await _context.SaveChangesAsync();
            //    return Ok();
            //}


            // Turn IDENTITY_INSERT ON
            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblVehicle ON");
            _context.Add(obj);
            // Turn IDENTITY_INSERT ON
            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblVehicle OFF");
            await _context.SaveChangesAsync();
            return Ok(new { id = obj.ID, regNo = obj.RegNo });
        }

        // GET: api/products 
        [HttpGet]
        public async Task<IActionResult> GetAllVehicles()
        {
            var Vehicles =await _context.vwVehicles.OrderByDescending(p => p.ID).ToListAsync();
            return Ok(Vehicles);
        }

        [HttpPost("bulkSaveVehicles")]
        public async Task<IActionResult> BulkSave([FromBody] List<Vehicle> Obj)
        {
            //if (Vehicles == null || !Vehicles.Any())
            //     return "BadRequest("No Vehicles provided")";

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var updatedObj in Obj)
                {
                    var existingVeh = await _context.Vehicles.Where(p => p.RegNo == updatedObj.RegNo && p.ID != updatedObj.ID).ToListAsync();
                    if (existingVeh != null)
                    {
                        if (existingVeh.Count() > 0)
                            return BadRequest("Vehicle RegNo {" + updatedObj.RegNo + "} already registered with another user!");
                    }
                    var existingOBJ = await _context.Vehicles
                        .FirstOrDefaultAsync(v => v.ID == updatedObj.ID);

                    if (existingOBJ != null)
                    {
                        // Update properties 
                        existingOBJ.RegNo = updatedObj.RegNo;
                        existingOBJ.vType = updatedObj.vType;
                        existingOBJ.vMake = updatedObj.vMake;
                        existingOBJ.vColor = updatedObj.vColor;
                        existingOBJ.TagUID = updatedObj.TagUID;
                        existingOBJ.TagEncodingDate = updatedObj.TagEncodingDate;
                        existingOBJ.Logical_Delete = updatedObj.Logical_Delete;
                        existingOBJ.PrintedTagID = updatedObj.PrintedTagID;
                        existingOBJ.StickerNo = updatedObj.StickerNo;
                        existingOBJ.updatedby = updatedObj.updatedby;
                        existingOBJ.updatedon = DateTime.Now;
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        // Turn IDENTITY_INSERT OFF
                        _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblVehicle ON");
                        _context.Vehicles.Add(updatedObj);
                        // Turn IDENTITY_INSERT OFF
                        _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblVehicle OFF");
                        await _context.SaveChangesAsync();
                    }
                }
                await transaction.CommitAsync();
                return Ok(new { message = $"{Obj.Count} Vehicles processed successfully" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpDelete("DeleteVehicle/{ID}")]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Vehicle ID mismatch.");
                }
                var existingOBJ = await _context.Vehicles.FindAsync(id);
                if (existingOBJ == null)
                {
                    return BadRequest("Vehicle ID not found.");
                }
                existingOBJ.isactive = 0;
                existingOBJ.Logical_Delete = 1;

                _context.Entry(existingOBJ).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new { message = $"device with ID {existingOBJ.RegNo}  inactive & deleted successfully" });
            }
            catch (Exception ex)
            {
                //     await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }

        //[HttpPost("SaveVehicleNRDAccess")]
        //public async Task<IActionResult> SaveVehicleNRDAccess([FromBody] VehicleNRDAccessRequest req)
        //{
        //    var existingAccess = await _context.VehicleNRDAccess.Where(x => x.VehicleID == req.VehicleID).ToListAsync();
        //    _context.VehicleNRDAccess.RemoveRange(existingAccess);
        //    await _context.SaveChangesAsync();

        //    if (req.NRDIDs != null && req.NRDIDs.Any())
        //    {
        //        foreach (var nrdId in req.NRDIDs)
        //        {
        //            _context.VehicleNRDAccess.Add(new VehicleNRDAccess
        //            {
        //                VehicleID = req.VehicleID,
        //                NRDID = nrdId,
        //                IsActive = true,
        //                CreatedOn = DateTime.Now
        //            });
        //        }
        //        await _context.SaveChangesAsync();
        //    }

        //    return Ok(new { message = "NRD access saved", vehicleId = req.VehicleID, count = req.NRDIDs?.Count ?? 0 });
        //}

        [HttpPost("SaveVehicleNRDAccess")]
        public async Task<IActionResult> SaveVehicleNRDAccess([FromBody] VehicleNRDAccessRequest req)
        {
            var selected = (req.NRDIDs ?? new List<int>()).Distinct().ToList();
            var existing = await _context.VehicleNRDAccess.Where(x => x.VehicleID == req.VehicleID).ToListAsync();
            bool newRows = false;

            // unselected -> deactivate, keep the row
            foreach (var r in existing.Where(x => x.IsActive && !selected.Contains(x.NRDID)))
                r.IsActive = false;

            // re-selected after removal -> needs upload again
            foreach (var r in existing.Where(x => !x.IsActive && selected.Contains(x.NRDID)))
            { r.IsActive = true; r.IsUploaded = "N"; newRows = true; }

            // brand-new project
            foreach (var nrdId in selected.Except(existing.Select(x => x.NRDID)))
            {
                _context.VehicleNRDAccess.Add(new VehicleNRDAccess
                {
                    VehicleID = req.VehicleID,
                    NRDID = nrdId,
                    IsActive = true,
                    IsUploaded = "N",
                    CreatedOn = DateTime.Now
                });
                newRows = true;
            }
            await _context.SaveChangesAsync();

            // only wake the sync when something new needs uploading
            if (newRows)
            {
                var veh = await _context.Vehicles.FindAsync(req.VehicleID);
                if (veh != null) { veh.IsUploadedToANPR = "N"; await _context.SaveChangesAsync(); }
            }
            return Ok(new { message = "NRD access saved", vehicleId = req.VehicleID, count = selected.Count });
        }
        public async Task<IActionResult> GetVehicleNRDs(int vehicleId)
        {
            var access = await _context.VehicleNRDAccess
                .Where(x => x.VehicleID == vehicleId && x.IsActive)
                .Select(x => x.NRDID)
                .ToListAsync();
            return Ok(access);
        }
    }
}
