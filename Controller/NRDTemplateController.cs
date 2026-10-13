using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Township_API.Data;
using Township_API.Models;

namespace Township_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NRDTemplateController : ControllerBase
    {
        private readonly AppDBContext _context;
        private readonly string _templateFolder = @"D:\IDS\NRDTemplates";

        public NRDTemplateController(AppDBContext context)
        {
            _context = context;
        }

        [HttpPost("Upload")]
        public async Task<IActionResult> Upload(int nrdId, string nrdName, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new { error = "File is required" });

                string ext = Path.GetExtension(file.FileName).ToLower();
                if (ext != ".jpg" && ext != ".jpeg" && ext != ".png")
                    return BadRequest(new { error = "Only jpg/png allowed" });

                string fileName = $"nrd_{nrdId}{ext}";
                string filePath = Path.Combine(_templateFolder, fileName);

                if (!Directory.Exists(_templateFolder))
                    Directory.CreateDirectory(_templateFolder);

                byte[] fileBytes;
                using (var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms);
                    fileBytes = ms.ToArray();
                }
                await System.IO.File.WriteAllBytesAsync(filePath, fileBytes);

                var existing = await _context.NRDTemplates
                    .FirstOrDefaultAsync(x => x.NRDId == nrdId);

                if (existing != null)
                {
                    existing.NRDName = nrdName;
                    existing.FilePath = filePath;
                    existing.ContentType = file.ContentType;
                    existing.ImageKitUrl = "";
                    existing.UpdatedOn = DateTime.Now;
                    existing.RecordStatus = "U";
                }
                else
                {
                    _context.NRDTemplates.Add(new NRDTemplate
                    {
                        NRDId = nrdId,
                        NRDName = nrdName,
                        FilePath = filePath,
                        ContentType = file.ContentType,
                        ImageKitUrl = "",
                        UpdatedOn = DateTime.Now,
                        RecordStatus = "A"
                    });
                }

                await _context.SaveChangesAsync();
                return Ok(new { message = "Template saved successfully", filePath });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("Get/{nrdId}")]
        public async Task<IActionResult> Get(int nrdId)
        {
            try
            {
                var tmpl = await _context.NRDTemplates
                    .FirstOrDefaultAsync(x => x.NRDId == nrdId && x.RecordStatus != "D");

                if (tmpl == null)
                    return NotFound(new { error = "Template not found" });

                if (!System.IO.File.Exists(tmpl.FilePath))
                    return NotFound(new { error = "File not found on disk" });

                byte[] bytes = await System.IO.File.ReadAllBytesAsync(tmpl.FilePath);
                string base64 = $"data:{tmpl.ContentType};base64,{Convert.ToBase64String(bytes)}";

                return Ok(new
                {
                    tmpl.NRDId,
                    tmpl.NRDName,
                    tmpl.ContentType,
                    tmpl.UpdatedOn,
                    imageKitUrl = (string?)null,
                    base64
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var list = await _context.NRDTemplates
                    .Where(x => x.RecordStatus != "D")
                    .OrderBy(x => x.NRDName)
                    .Select(x => new
                    {
                        x.Id,
                        x.NRDId,
                        x.NRDName,
                        x.ContentType,
                        x.ImageKitUrl,
                        x.UpdatedOn
                    })
                    .ToListAsync();

                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("Delete/{nrdId}")]
        public async Task<IActionResult> Delete(int nrdId)
        {
            try
            {
                var tmpl = await _context.NRDTemplates
                    .FirstOrDefaultAsync(x => x.NRDId == nrdId);

                if (tmpl == null)
                    return NotFound(new { error = "Template not found" });

                tmpl.RecordStatus = "D";
                tmpl.UpdatedOn = DateTime.Now;
                await _context.SaveChangesAsync();
                return Ok(new { message = "Template deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}