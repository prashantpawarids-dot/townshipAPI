using Microsoft.AspNetCore.Mvc;
using Township_API.Data;
using Township_API.Models;
using Microsoft.EntityFrameworkCore;
[Route("api/[controller]")]
[ApiController]
public class CardMasterTemplateController : ControllerBase
{
    private readonly AppDBContext _context;
    private readonly IWebHostEnvironment _env;

    public CardMasterTemplateController(AppDBContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    [HttpPost("Upload")]
    public async Task<IActionResult> Upload(IFormFile file, [FromForm] string holderType)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file provided");
        if (string.IsNullOrEmpty(holderType))
            return BadRequest("holderType is required");

        try
        {
            var uploadsFolder = Path.Combine(_env.ContentRootPath, "CardTemplates");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"master_template_{holderType}{Path.GetExtension(file.FileName)}";
            var fullPath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var existing = await _context.CardMasterTemplates.FirstOrDefaultAsync(x => x.HolderType == holderType);
            if (existing != null)
            {
                existing.FilePath = fileName;
                existing.ContentType = file.ContentType;
                existing.UpdatedOn = DateTime.Now;
            }
            else
            {
                _context.CardMasterTemplates.Add(new CardMasterTemplate
                {
                    HolderType = holderType,
                    FilePath = fileName,
                    ContentType = file.ContentType,
                    UpdatedOn = DateTime.Now
                });
            }
            await _context.SaveChangesAsync();

            return Ok(new { message = "Template uploaded successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("Get/{holderType}")]
    public async Task<IActionResult> Get(string holderType)
    {
        var tmpl = await _context.CardMasterTemplates.FirstOrDefaultAsync(x => x.HolderType == holderType);
        if (tmpl == null || string.IsNullOrEmpty(tmpl.FilePath))
            return NotFound();

        var fullPath = Path.Combine(_env.ContentRootPath, "CardTemplates", tmpl.FilePath);
        if (!System.IO.File.Exists(fullPath))
            return NotFound();

        var bytes = await System.IO.File.ReadAllBytesAsync(fullPath);
        var base64 = Convert.ToBase64String(bytes);

        string? logoBase64 = null;
        if (!string.IsNullOrEmpty(tmpl.LogoPath))
        {
            var logoFullPath = Path.Combine(_env.ContentRootPath, "CardTemplates", tmpl.LogoPath);
            if (System.IO.File.Exists(logoFullPath))
            {
                var logoBytes = await System.IO.File.ReadAllBytesAsync(logoFullPath);
                logoBase64 = $"data:{tmpl.LogoContentType};base64,{Convert.ToBase64String(logoBytes)}";
            }
        }

        return Ok(new { base64 = $"data:{tmpl.ContentType};base64,{base64}", logoBase64, updatedOn = tmpl.UpdatedOn });
    }

    [HttpPost("UploadLogo")]
    public async Task<IActionResult> UploadLogo(IFormFile file, [FromForm] string holderType)
    {
        if (file == null || file.Length == 0) return BadRequest("No file provided");
        if (string.IsNullOrEmpty(holderType)) return BadRequest("holderType is required");

        var tmpl = await _context.CardMasterTemplates.FirstOrDefaultAsync(x => x.HolderType == holderType);
        if (tmpl == null) return NotFound(new { error = "Upload the card template first" });

        var uploadsFolder = Path.Combine(_env.ContentRootPath, "CardTemplates");
        Directory.CreateDirectory(uploadsFolder);
        var fileName = $"logo_{holderType}{Path.GetExtension(file.FileName)}";
        var fullPath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(fullPath, FileMode.Create))
            await file.CopyToAsync(stream);

        tmpl.LogoPath = fileName;
        tmpl.LogoContentType = file.ContentType;
        tmpl.UpdatedOn = DateTime.Now;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Logo uploaded successfully" });
    }
    [HttpDelete("DeleteLogo/{holderType}")]
    public async Task<IActionResult> DeleteLogo(string holderType)
    {
        var tmpl = await _context.CardMasterTemplates.FirstOrDefaultAsync(x => x.HolderType == holderType);
        if (tmpl == null) return NotFound(new { error = "Template not found" });

        if (!string.IsNullOrEmpty(tmpl.LogoPath))
        {
            var logoFullPath = Path.Combine(_env.ContentRootPath, "CardTemplates", tmpl.LogoPath);
            if (System.IO.File.Exists(logoFullPath))
                System.IO.File.Delete(logoFullPath);
        }

        tmpl.LogoPath = null;
        tmpl.LogoContentType = null;
        tmpl.UpdatedOn = DateTime.Now;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Logo deleted successfully" });
    }
}