using Microsoft.AspNetCore.Mvc;
using Township_API.Models; 
using Township_API.Data;
using Microsoft.EntityFrameworkCore;
using Township_API.Services;
using static Township_API.Models.commonTypes;

namespace Township_API.Controllers
{ 
    [Route("api/[controller]")]
    [ApiController]
    public class  Controller1: Controller
    {
        private readonly AppDBContext _context;


        public  Controller1(AppDBContext context )
        {
            _context = context;   
        }
          
        
        // PUT: api/products/5 
        [HttpPost("{UpdateDependentResident}/{id}")]
        public async Task<IActionResult> UpdateDependentResident(int id, [FromBody] DependentResident updatedDResident)
        {
            if (id != updatedDResident.ID)
            {
                return BadRequest("DependentResident ID mismatch.");
            }

            //var existingDependentResident = await _service.UpdateDependentResidentAsync(updatedDResident.ID, updatedDResident);
            var existingDependentResident = await _context.DependentResidents.FindAsync(updatedDResident.ID);
            if (existingDependentResident == null)
            {
                return NotFound();
            }

            _context.Entry(existingDependentResident).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(existingDependentResident);
        }


        [HttpPost("AddDependentResident")]
        public async Task<IActionResult> AddDependentResident([FromBody] DependentResident obj)
        {
            var existingDependentResident = await _context.DependentResidents.FindAsync(0);
            if (existingDependentResident != null)
            {
                return BadRequest("DependentResident Exists.");
            }
            _context.Add(obj);
            await _context.SaveChangesAsync();

            int number = (int)AccessCardHolders.Resident;
            obj.IDNumber = number.ToString() + obj.ID.ToString("D5");
            await _context.SaveChangesAsync();
             return Ok(new { message = $"Dependent Resident Added successfully", IDNumber = obj.IDNumber });
        }


        // GET: api/products 
        [HttpGet]
        public async Task<IActionResult> GetAllDependentResidents()
        {
            var DependentResidents = await _context.DependentResidents.Where (p=>p.LogicalDeleted == 0).OrderByDescending(p => p.ID ).ToListAsync();
            
            
            return Ok(DependentResidents.OrderByDescending(e => e.ID));
        }         
    }
}
