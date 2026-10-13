using Microsoft.AspNetCore.Mvc;
using Township_API.Models;
using Township_API.Services;

namespace Township_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BackupController : ControllerBase
    {
        private readonly IBackupConfigStore _store;
        private readonly IBackupService _backupService;

        public BackupController(IBackupConfigStore store, IBackupService backupService)
        {
            _store = store;
            _backupService = backupService;
        }

        [HttpGet("settings")]
        public IActionResult GetSettings() => Ok(_store.GetSettings());

        [HttpPut("settings")]
        public IActionResult SaveSettings([FromBody] BackupSettings settings)
        {
            var existing = _store.GetSettings();
            settings.LastRunTime = existing.LastRunTime;
            settings.LastRunStatus = existing.LastRunStatus;
            settings.LastRunDateOnly = existing.LastRunDateOnly;
            _store.SaveSettings(settings);
            return Ok(new { message = "Settings saved." });
        }

        [HttpGet("history")]
        public IActionResult GetHistory() => Ok(_store.GetHistory());

        [HttpPost("run-now")]
        public async Task<IActionResult> RunNow()
        {
            var settings = _store.GetSettings();
            var entry = await _backupService.RunBackupAsync(settings, "Manual");

            settings.LastRunTime = entry.RunTime;
            settings.LastRunStatus = entry.Success ? "Success" : "Failed";
            _store.SaveSettings(settings);
            _store.AddHistory(entry);

            return entry.Success ? Ok(entry) : StatusCode(500, entry);
        }

        // Simple folder browser so Angular can build a folder-picker UI
        [HttpGet("browse-folders")]
        public IActionResult BrowseFolders([FromQuery] string? path = null)
        {
            try
            {
                // Empty path = show "This PC" (all drives)
                if (string.IsNullOrWhiteSpace(path))
                {
                    var drives = DriveInfo.GetDrives()
                        .Where(d => d.IsReady)
                        .Select(d => new { name = $"{d.Name} ({d.DriveType})", path = d.RootDirectory.FullName });
                    return Ok(new { currentPath = "", folders = drives });
                }

                var dirs = Directory.GetDirectories(path).Select(d => new { name = Path.GetFileName(d), path = d });
                return Ok(new { currentPath = path, folders = dirs });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}