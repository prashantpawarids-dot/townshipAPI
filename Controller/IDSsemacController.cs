using Microsoft.AspNetCore.Mvc;
using Township_API.Services;

namespace Township_API.Controllers
{
    [ApiController]
    [Route("api/idscontroller")]
    public class IDSsemacController : ControllerBase
    {
        private readonly SemacControllerService _service;
        public IDSsemacController(SemacControllerService service) => _service = service;

        [HttpGet("getuser")]
        public async Task<IActionResult> GetUser(string readerCode, uint userId)
        {
            var r = await _service.GetUserAsync(readerCode, userId);
            if (!r.Success) return BadRequest(r.ErrorMessage);
            return Ok(new { r.UserID, r.UserName, r.CardNo, r.EmployeeID, r.Status });
        }

        [HttpPost("saveuser")]
        public async Task<IActionResult> SaveUser([FromBody] SaveUserRequestDto dto)
        {
            var r = await _service.SaveUserAsync(dto.ReaderCode, dto.UserID, dto.UserName, dto.CardNo, dto.EmployeeID, dto.Activate);
            if (!r.Success) return BadRequest(r.ErrorMessage);
            return Ok(new { message = "Success" });
        }

        [HttpPost("opendoor")]
        public async Task<IActionResult> OpenDoor(string readerCode)
        {
            var r = await _service.OpenDoorAsync(readerCode);
            if (!r.Success) return BadRequest(r.ErrorMessage);
            return Ok(new { message = "Door Opened" });
        }

        [HttpPost("accesslogupload")]
        public async Task<IActionResult> AccessLogUpload(string readerCode = null)
        {
            int count = await _service.PushCardHoldersAsync(readerCode);
            return Ok(new { count });
        }

        [HttpPost("accesslogdownload")]
        public IActionResult AccessLogDownload(string readerCode)
        {
            _service.DownloadAccessLog(readerCode);
            return Ok(new { message = "Download triggered" });
        }
    }

    public class SaveUserRequestDto
    {
        public string ReaderCode { get; set; }
        public uint UserID { get; set; }
        public string UserName { get; set; }
        public string CardNo { get; set; }
        public string EmployeeID { get; set; }
        public bool Activate { get; set; }
    }
}