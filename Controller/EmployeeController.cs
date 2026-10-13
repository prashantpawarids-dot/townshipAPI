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
using System.Reflection.Metadata.Ecma335;

namespace Township_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : Controller
    {
        private readonly AppDBContext _context;
        public EmployeeController(AppDBContext context)
        {
            _context = context;
        }

        // PUT: api/products/5
        [HttpPost("UpdateEmployee/{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] Employee updatedEmployee)
        {
            try
            {
                if (id != updatedEmployee.ID)
                {
                    return BadRequest("Employee ID mismatch.");
                }

                //var existingEmployee = await _service.UpdateEmployeeAsync(updatedEmployee.ID, updatedEmployee);
                var existingEmployee = await _context.Employees.FindAsync(updatedEmployee.ID);

                if (existingEmployee == null)
                {
                    return NotFound();
                }
                var existEmployee = await _context.Employees.Where(p => (p.FirstName == updatedEmployee.FirstName && p.LastName == updatedEmployee.LastName) && p.ID!= updatedEmployee.ID).ToListAsync();
                if (existEmployee != null)
                {
                    if(existEmployee.Count>0)
                    return BadRequest("Employee with name already Exists.");
                }
                  existEmployee = await _context.Employees.Where(p => (p.IDNumber == updatedEmployee.IDNumber) && p.ID != updatedEmployee.ID).ToListAsync();
                if (existEmployee != null)
                {
                    if (existEmployee.Count > 0)
                        return BadRequest("Employee with this Idnumber already Exists.");
                }


                existingEmployee.IDNumber = updatedEmployee.IDNumber;
                existingEmployee.FirstName = updatedEmployee.FirstName;
                existingEmployee.MiddleName = updatedEmployee.MiddleName;
                existingEmployee.LastName = updatedEmployee.LastName;
                existingEmployee.EmailID = updatedEmployee.EmailID; 
                existingEmployee.MobileNo = updatedEmployee.MobileNo;               
                existingEmployee.ICEno = updatedEmployee.ICEno;
                existingEmployee.Role = updatedEmployee.Role;
                existingEmployee.Gender = updatedEmployee.Gender;
                existingEmployee.CardCSN = updatedEmployee.CardCSN;
                existingEmployee.SiteID = updatedEmployee.SiteID;
                existingEmployee.Role = updatedEmployee.Role;
                existingEmployee.Dob = updatedEmployee.Dob;
                existingEmployee.Doj = updatedEmployee.Doj;
                existingEmployee.isResident = updatedEmployee.isResident;
                existingEmployee.ResidentID = updatedEmployee.ResidentID;
                existingEmployee.isactive = updatedEmployee.isactive; 
                existingEmployee.updatedby = updatedEmployee.updatedby;
                existingEmployee.updatedon = updatedEmployee.updatedon;
                 
                existingEmployee.BloodGroup = updatedEmployee.BloodGroup; 
                existingEmployee.EmployeeCompanyID = updatedEmployee.EmployeeCompanyID;

                _context.Entry(existingEmployee).State = EntityState.Modified;
                await _context.SaveChangesAsync();


                return Ok(new { message = $"Employee Updated successfully", IDNumber = existingEmployee.IDNumber });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpPost("AddEmployee")]
        public async Task<IActionResult> AddEmployee([FromBody] Employee obj)
        {
            try
            {
                var existingEmployee = await _context.Employees.FindAsync(0);
                if (existingEmployee != null)
                {
                    return BadRequest("Employee Exists.");
                }
                var existEmployee = await _context.Employees.Where(p=>p.FirstName==obj.FirstName && p.LastName == obj.LastName ).ToListAsync();
                if (existingEmployee != null)
                {
                    return BadRequest("Employee with name already Exists.");
                }

                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblEmployee ON");
                _context.Add(obj);
                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblEmployee OFF");
                await _context.SaveChangesAsync();
                //int number = (int)AccessCardHolders.Employee;
                //obj.IDNumber = number.ToString() + obj.ID.ToString("D4");
                //await _context.SaveChangesAsync();
                return Ok(new { message = $"Employee Added successfully" ,IDNumber= obj.IDNumber });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        // GET: api/products 
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var Employees = await _context.Employees.OrderByDescending(p => p.ID ).ToListAsync();
            foreach (var emp in Employees)
            {
                var nr = await _context.Projects.Where(u => u.id.ToString() == emp.SiteID.ToString()).FirstOrDefaultAsync();
                if (nr != null)
                    emp.sitename = (string)nr.ProjectName.ToString();

                var cmp = await _context.ModuleData.Where(u => u.ID.ToString() == emp.EmployeeCompanyID.ToString()).FirstOrDefaultAsync();
                if (cmp != null)
                    emp.EmployeeCompany = (string)cmp.Name.ToString();
            }

            return Ok(Employees);
        }


        // GET: api/products 
        [HttpGet("GetEmployeeByID/{id}")]
        public async Task<IActionResult> GetEmployeeByID(int id)
        {
            var Employees = await _context.Employees.Where(p => p.ID == id && p.isdeleted == 0  ).OrderByDescending(p => p.ID).ToListAsync();
            if (Employees == null)
                return BadRequest("No Data found");

            foreach (var emp in Employees)
            {
                var nr = await _context.Projects.Where(u => u.id.ToString() == emp.SiteID.ToString()).FirstOrDefaultAsync();
                if (nr != null)
                    emp.sitename = (string)nr.ProjectName.ToString();
                
                var cmp = await _context.ModuleData.Where(u => u.ID.ToString() == emp.EmployeeCompanyID.ToString()).FirstOrDefaultAsync();
                if (cmp != null)
                    emp.EmployeeCompany = (string)cmp.Name.ToString();  
            }

            return Ok(Employees);
        }

        // GET: api/products 
        [HttpGet("GetEmployeeAccessDetails/{ID}")]
        public async Task<IActionResult> GetEmployeeAccessDetails(int ID)
        {
            var Employees = await _context.Employees.Where(p => p.ID == ID ).ToListAsync();
            string? IdNumber = Employees[0].IDNumber;
            if (IdNumber != null)
            {
                try
                {
                    var jsonWrapper = new DependentJsonWrapper
                    {
                        Owners = Employees,
                        Vehicles = _context.Vehicles.Where(p => p.TagUID == IdNumber).ToList(),
                        UserAllAccess = await _context._userAllAccess.Where(p => p.CardHolderID == IdNumber).ToListAsync()
                     };

                    return Ok(jsonWrapper);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message.ToString());
                }
            }
            return Ok(new { message = $"Employee records not found!" });
        }

        [HttpPost("AddEmployees")]
        public async Task<IActionResult> AddEmployees([FromBody] List<Employee> Obj)
        {
            if (Obj == null || !Obj.Any())
                return BadRequest("No Data provided");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var objID in Obj)
                {
                    var existingobj = await _context.Employees
                        .FirstOrDefaultAsync(p => p.ID == objID.ID);

                    if (existingobj != null)
                    {
                        //  var existingEmployee = await _service.UpdateEmployeeAsync(objID.ID, existingobj);
                        var existingEmployee = await _context.Employees.FindAsync(0);
                        if (existingEmployee == null)
                        {
                            return NotFound();
                        }

                    }
                    else
                    {
                        _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblEmployee ON");
                        _context.Employees.Add(objID);
                        _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblEmployee  OFF");
                        await _context.SaveChangesAsync();
                        int number = (int)AccessCardHolders.Employee;
                        objID.IDNumber = number.ToString() + objID.ID.ToString("D4");
                        await _context.SaveChangesAsync();
                    }
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { message = $"{Obj.Count} Employee processed successfully" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var existingEmployee = await _context.Employees.FindAsync(id);
            if (existingEmployee == null)
            {
                return NotFound();
            }

            existingEmployee.isdeleted = 1;
            existingEmployee.updatedby = 1;
            existingEmployee.updatedon = DateTime.Now;


            _context.Entry(existingEmployee).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { Message = $"Employee Deleted Successfully", IDNumber = existingEmployee.IDNumber });
        }
    }
}
