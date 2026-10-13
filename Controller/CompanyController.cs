
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net;
using Township_API.Data;
using Township_API.Models;
using static Township_API.Models.commonTypes;

namespace Township_API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : Controller
    {
        private readonly AppDBContext _context;
        public CompanyController(AppDBContext context)
        {
            _context = context;
        }

        [HttpPost("UpdateCompany/{id}")]
        public async Task<IActionResult> UpdateCompany(int id, [FromBody] Company updatedOBJ)
        {
            try
            {
                if (id != updatedOBJ.id)
                {
                    return BadRequest("Company ID mismatch.");
                }
                var existingOBJ = await _context.Companies.FindAsync(id);
                if (existingOBJ == null)
                {
                    return NotFound();
                }
                var existOBJ = await _context.Companies.Where(p => (p.companyName == updatedOBJ.companyName || p.companyCode == updatedOBJ.companyCode) && p.id != updatedOBJ.id).ToListAsync();
                
                if (existOBJ != null)
                {
                    if (existOBJ.Count > 0)
                        return BadRequest("Company name/code already exists!");
                }

                existingOBJ.companyCode = updatedOBJ.companyCode;
                existingOBJ.companyName = updatedOBJ.companyName;
                existingOBJ.city = updatedOBJ.city;
                existingOBJ.address = updatedOBJ.address;
                existingOBJ.GSTnumber = updatedOBJ.GSTnumber;
                existingOBJ.pannumber = updatedOBJ.pannumber;
                existingOBJ.mobileNo = updatedOBJ.mobileNo;
                existingOBJ.landLine = updatedOBJ.landLine;
                existingOBJ.ContactPerson = updatedOBJ.ContactPerson;
                existingOBJ.ContactMobile = updatedOBJ.ContactMobile;
                existingOBJ.ValidFromDate = updatedOBJ.ValidFromDate;
                existingOBJ.ValidToDate = updatedOBJ.ValidToDate;
                existingOBJ.isactive = updatedOBJ.isactive;
                existingOBJ.isdeleted = updatedOBJ.isdeleted;
                existingOBJ.updatedby = updatedOBJ.updatedby;
                existingOBJ.updatedon = updatedOBJ.updatedon;

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


        [HttpPost("AddCompany")]
        public async Task<IActionResult> AddCompany([FromBody] Company obj)
        {
            try
            {
                var existingOBJ = await _context.Companies.FindAsync(0);
                if (existingOBJ != null)
                {
                    return BadRequest("Company data Exists.");
                }
                var existOBJ = await _context.Companies.Where(p =>p.companyName  == obj.companyName  || p.companyCode == obj.companyCode ).ToListAsync();
                
                if (existOBJ != null)
                {
                    if (existOBJ.Count > 0)
                        return BadRequest("Company name/code already exists!");
                }

                // Turn IDENTITY_INSERT ON
                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Company ON");
                _context.Add(obj);

                // Turn IDENTITY_INSERT ON
                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Company OFF");
                await _context.SaveChangesAsync();

                return Ok(new { message = $"Company processed successfully" });
            }
            catch (Exception ex)
            {
                //     await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpPost("DeleteCompany/{id}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Company ID mismatch.");
                }
                var existingOBJ = await _context.Companies.FindAsync(id);
                if (existingOBJ == null)
                {
                    return NotFound();
                }


                existingOBJ.isdeleted = true;
                existingOBJ.updatedby = 1;
                existingOBJ.updatedon = DateTime.Now;

                _context.Entry(existingOBJ).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new { message = $"{existingOBJ.companyName} Data deleted successfully" });

            }
            catch (Exception ex)
            {
                //     await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAllCompanies()
        {
            try
            {
                var OBJs = await _context.Companies.IgnoreAutoIncludes().OrderByDescending(p => p.id).ToListAsync();


                return Ok(OBJs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("GetCompany/{ID}")]
        public async Task<IActionResult> GetCompany(int id)
        {
            try
            {
                var OBJs = await _context.Companies.Where(n => n.id == id).IgnoreAutoIncludes().OrderByDescending(p => p.id).ToListAsync();
                return Ok(OBJs);
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        [HttpPost("bulkSaveCompany")]
        public async Task<IActionResult> BulkSave([FromBody] List<Company> Obj)
        {

            //using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var updatedObj in Obj)
                {
                    var existingOBJ = await _context.Companies
                        .FirstOrDefaultAsync(v => v.companyName == updatedObj.companyName);

                    if (existingOBJ != null)
                    {
                        // Update properties 
                        existingOBJ.companyCode = updatedObj.companyCode;
                        existingOBJ.companyName = updatedObj.companyName;
                        existingOBJ.city = updatedObj.city;
                        existingOBJ.address = updatedObj.address;
                        existingOBJ.GSTnumber = updatedObj.GSTnumber;
                        existingOBJ.pannumber = updatedObj.pannumber;
                        existingOBJ.mobileNo = updatedObj.mobileNo;
                        existingOBJ.landLine = updatedObj.landLine;
                        existingOBJ.ContactPerson = updatedObj.ContactPerson;
                        existingOBJ.ContactMobile = updatedObj.ContactMobile;
                        existingOBJ.ValidFromDate = updatedObj.ValidFromDate;
                        existingOBJ.ValidToDate = updatedObj.ValidToDate;
                        existingOBJ.isactive = updatedObj.isactive;
                        existingOBJ.isdeleted = updatedObj.isdeleted;
                        existingOBJ.updatedby = updatedObj.updatedby;
                        existingOBJ.updatedon = DateTime.Now;
                        await _context.SaveChangesAsync();
                        //  await transaction.CommitAsync();
                    }
                    else
                    {
                        // Turn IDENTITY_INSERT ON
                        _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT company ON");
                        _context.Companies.Add(updatedObj);
                        //  await transaction.CommitAsync();
                        // Turn IDENTITY_INSERT ON
                        _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT company OFF");
                        await _context.SaveChangesAsync();
                    }
                }


                return Ok(new { message = $"company data processed successfully" });
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
    public class ProjectController : Controller
    {
        private readonly AppDBContext _context;
        public ProjectController(AppDBContext context)
        {
            _context = context;
        }

        [HttpPost("UpdateProject/{id}")]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] Project updatedOBJ)
        {
            try
            {
                if (id != updatedOBJ.id)
                {
                    return BadRequest("Project ID mismatch.");
                }
                var existingOBJ = await _context.Projects.FindAsync(id);
                if (existingOBJ == null)
                {
                    return NotFound();
                }
                var existsOBJ = await _context.Projects.Where(p => p.ProjectName == updatedOBJ.ProjectName && p.id != updatedOBJ.id).ToListAsync();
                if (existsOBJ != null)
                {
                    if (existsOBJ.Count > 0)
                        return BadRequest("Project Name is already Exists.");
                }
                existingOBJ.ProjectCode = updatedOBJ.ProjectCode;
                existingOBJ.ProjectName = updatedOBJ.ProjectName;
                existingOBJ.CompanyID = updatedOBJ.CompanyID;
                existingOBJ.isactive = updatedOBJ.isactive;
                existingOBJ.isdeleted = updatedOBJ.isdeleted;
                existingOBJ.updatedby = updatedOBJ.updatedby;
                existingOBJ.updatedon = updatedOBJ.updatedon;

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

        [HttpPost("DeleteProject/{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Project ID mismatch.");
                }
                var existingOBJ = await _context.Projects.FindAsync(id);
                if (existingOBJ == null)
                {
                    return NotFound();
                }

                existingOBJ.isdeleted = true;
                existingOBJ.updatedby = 1;
                existingOBJ.updatedon = DateTime.Now;

                _context.Entry(existingOBJ).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new { message = $"{existingOBJ.ProjectName} Data deleted successfully" });

            }
            catch (Exception ex)
            {
                //     await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpPost("AddProject")]
        public async Task<IActionResult> AddProject([FromBody] Project obj)
        {
            try
            {
                var existingOBJ = await _context.Projects.FindAsync(0);
                if (existingOBJ != null)
                {
                    return BadRequest("Project data Exists.");
                }
                var existOBJ = await _context.Projects.Where(p => p.ProjectName.ToString() == obj.ProjectName.ToString()).ToListAsync();
                if (existOBJ != null)
                {
                    if (existOBJ.Count > 0)
                        return BadRequest("Project name already exists!");
                }
                var project = new Project
                {
                    ProjectCode = obj.ProjectCode,
                    ProjectName = obj.ProjectName,
                    CompanyID = obj.CompanyID,
                    isactive = obj.isactive,
                    isdeleted = obj.isdeleted,
                    createdby = obj.createdby,
                    createdon = obj.createdon,
                    updatedby = obj.updatedby,
                    updatedon = obj.updatedon
                };


                //_context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Project ON");
                _context.Add(project);
                //_context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Project OFF");
                // Turn IDENTITY_INSERT ON
                await _context.SaveChangesAsync();
                return Ok(new { message = $"Project processed successfully" }); ;
            }
            catch (Exception ex)
            {
                //     await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllComapnyProjects(int companyID)
        {
            try
            {
                object OBJs;

                if (companyID > 0)
                    OBJs = await _context.Projects.Where(p => p.CompanyID == companyID).OrderByDescending(p => p.id).ToListAsync();
                else
                    OBJs = await _context.Projects.Where(p => p.isactive == true && p.isdeleted == false).IgnoreAutoIncludes().OrderByDescending(p => p.id).ToListAsync();

                return Ok(OBJs);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        [HttpGet("GetProject/{ID}")]
        public async Task<IActionResult> GetProject(int id)
        {
            try
            {
                var OBJs = await _context.Projects.Where(n => n.id == id).IgnoreAutoIncludes().OrderByDescending(p => p.id).ToListAsync();

                return Ok(OBJs);
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        [HttpPost("bulkSaveProject")]
        public async Task<IActionResult> BulkSave([FromBody] List<Project> Obj)
        {

            //using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var updatedObj in Obj)
                {
                    var existingOBJ = await _context.Projects
                        .FirstOrDefaultAsync(v => v.ProjectName == updatedObj.ProjectName || v.id == updatedObj.id);

                    if (existingOBJ != null)
                    {
                        // Update properties 
                        existingOBJ.ProjectCode = updatedObj.ProjectCode;
                        existingOBJ.ProjectName = updatedObj.ProjectName;
                        existingOBJ.CompanyID = updatedObj.CompanyID;
                        existingOBJ.isactive = updatedObj.isactive;
                        existingOBJ.isdeleted = updatedObj.isdeleted;
                        existingOBJ.updatedby = updatedObj.updatedby;
                        existingOBJ.updatedon = updatedObj.updatedon;
                        existingOBJ.updatedon = DateTime.Now;
                        await _context.SaveChangesAsync();
                        //  await transaction.CommitAsync();
                    }
                    else
                    {
                        // Turn IDENTITY_INSERT ON
                        _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Project ON");
                        _context.Projects.Add(updatedObj);
                        //  await transaction.CommitAsync();
                        // Turn IDENTITY_INSERT ON
                        _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Project OFF");
                        await _context.SaveChangesAsync();
                    }
                }


                return Ok(new { message = $"Project data processed successfully" });
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
    public class AccessRightsController : Controller
    {
        private readonly AppDBContext _context;
        public AccessRightsController(AppDBContext context)
        {
            _context = context;
        }

        [HttpPost("AddUpdateAccessBlockRevoke")]
        public async Task<IActionResult> AddUpdateAccessBlockRevok([FromBody] AccessBlockRevoke_Register updatedOBJ)
        {
            string Errr = "";
            try
            {
                Errr = "0";
                    var existingOBJ = await _context.AccessBlockRevokeRegister
                   .Where(c =>
        c.IDnumber == updatedOBJ.IDnumber &&
        (
            (c.fromdate >= updatedOBJ.fromdate && c.fromdate <= updatedOBJ.todate) ||
            (c.todate >= updatedOBJ.fromdate && c.todate <= updatedOBJ.todate)
        )
    )
    .FirstOrDefaultAsync();

                Errr = "1";
                if (existingOBJ != null)
                {
                    Errr = Errr+",2";
                    if (existingOBJ.id > 0)
                    {

                        existingOBJ.IDnumber = updatedOBJ.IDnumber;
                        existingOBJ.CardCsn = updatedOBJ.CardCsn;
                        existingOBJ.BlockRevokType = updatedOBJ.BlockRevokType;
                        if (updatedOBJ.BlockRevokType == "R")
                        {
                            existingOBJ.updatedOn = updatedOBJ.updatedOn;
                            existingOBJ.updatedby = updatedOBJ.updatedby;
                            existingOBJ.todate = DateTime.Now;
                            Errr = Errr + ",3";
                        }
                        else
                        {
                            existingOBJ.fromdate = updatedOBJ.fromdate;
                            if (existingOBJ.todate != null)
                                existingOBJ.todate = updatedOBJ.todate;

                            if (updatedOBJ.todate <= Convert.ToDateTime(updatedOBJ.fromdate.Value.AddHours(1)))
                                updatedOBJ.todate = Convert.ToDateTime(updatedOBJ.fromdate.Value.AddDays(1).Date);
                            Errr = Errr + ",4";
                        }

                        _context.Entry(existingOBJ).State = EntityState.Modified;
                        await _context.SaveChangesAsync();

                        return Ok(existingOBJ);
                    }
                    return Ok(null);
                }
                else
                {
                    DateTime frdate=DateTime.Today ;
                    if (updatedOBJ.fromdate!=null)
                        frdate = updatedOBJ.fromdate.Value.AddDays(1).Date;

                    Errr = Errr + ",6";
                    if (updatedOBJ.todate == null || updatedOBJ.todate <= Convert.ToDateTime(frdate))
                    {
                        updatedOBJ.todate = Convert.ToDateTime(frdate);

                        Errr = Errr + ",7";
                    }
                    else
                    {
                        if (updatedOBJ.todate <= Convert.ToDateTime(updatedOBJ.fromdate))
                            updatedOBJ.todate = Convert.ToDateTime(frdate);

                        Errr = Errr + ",8";
                    }
                    // Turn IDENTITY_INSERT ON
                    _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT AccessBlockRevoke_Register ON");
                    _context.AccessBlockRevokeRegister.Add(updatedOBJ);


                    Errr = Errr + ",9";
                    //  await transaction.CommitAsync();
                    // Turn IDENTITY_INSERT ON
                    _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT AccessBlockRevoke_Register OFF");
                    await _context.SaveChangesAsync();

                    Errr = Errr + ",10";
                    return Ok(new { message = $"Access processed successfully" }); ;
                }
            }
            catch (Exception ex)
            {
                //     await transaction.RollbackAsync();
                return StatusCode(500, new { error =Errr.ToString()+"-"+  ex.Message });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAccessBlockRevoke_Register(DateTime Fromdate, DateTime Todate)
        {
            try
            {
                if (Fromdate == null)
                    Fromdate = DateTime.Now.Date;

                if (Fromdate <= Todate)
                    Todate = Fromdate.AddDays(1);

                object OBJs;

                OBJs = await _context.AccessBlockRevokeRegister.Where(p => p.fromdate >= Fromdate && p.todate <= Todate).OrderByDescending(p => p.id).ToListAsync();

                return Ok(OBJs);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }

    [Route("api/[controller]")]
    [ApiController]
    public class CardLostDamageRegisterController : Controller
    {
        private readonly AppDBContext _context;
        public CardLostDamageRegisterController(AppDBContext context)
        {
            _context = context;
        }

        [HttpPost("AddUpdateCardLostDamageRegister/{id}")]
        public async Task<IActionResult> AddUpdateCardLostDamageRegister(int id, [FromBody] CardLostDamage_Register updatedOBJ)
        {
            try
            {
                if (updatedOBJ.id > 0)
                {

                    var existingOBJ = await _context.CardLostDamage_Register.FindAsync(id);
                    if (existingOBJ == null)
                    {
                        return NotFound();
                    }

                    existingOBJ.IDnumber = updatedOBJ.IDnumber;
                    existingOBJ.CardCsn = updatedOBJ.CardCsn;
                    existingOBJ.LostDamageType = updatedOBJ.LostDamageType;
                    existingOBJ.updatedOn = updatedOBJ.updatedOn;
                    existingOBJ.updatedby = updatedOBJ.updatedby;
                    existingOBJ.reporteddate = updatedOBJ.reporteddate;
                    if (existingOBJ.blockeddate != null)
                        existingOBJ.blockeddate = updatedOBJ.blockeddate;

                    _context.Entry(existingOBJ).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    return Ok(existingOBJ);
                }
                else
                {
                    // Turn IDENTITY_INSERT ON
                    _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT CardLostDamage_Register ON");
                    _context.CardLostDamage_Register.Add(updatedOBJ);
                    //  await transaction.CommitAsync();
                    // Turn IDENTITY_INSERT ON
                    _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT CardLostDamage_Register OFF");
                    await _context.SaveChangesAsync();
                    return Ok(new { message = $"CardLostDamage processed successfully" }); ;
                }
            }
            catch (Exception ex)
            {
                //     await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetCardLostDamageRegister(DateTime Fromdate, DateTime Todate)
        {
            try
            {
                if (Fromdate <= Todate)
                    Todate = Fromdate.AddDays(1);

                object OBJs;

                OBJs = await _context.CardLostDamage_Register.OrderByDescending(p => p.id).ToListAsync();

                return Ok(OBJs);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }

}

