using Microsoft.AspNetCore.Mvc;
using Township_API.Services;

namespace Township_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CameraLiveController : ControllerBase
    {
        private readonly ICameraPingService _cameraPing;

        public CameraLiveController(ICameraPingService cameraPing)
        {
            _cameraPing = cameraPing;
        }

        // GET /api/CameraLive/check?ip=192.168.31.9&port=80
        // Called when the user clicks Connect, before opening Live View / Tracking.
        [HttpGet("check")]
        public async Task<IActionResult> CheckCamera([FromQuery] string ip, [FromQuery] int port, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(ip))
                return BadRequest("ip is required");

            var result = await _cameraPing.CheckAsync(ip, port, ct);
            return Ok(result);
        }

        // GET /api/CameraLive/tracking?ip=192.168.31.9&port=80&date=2026-09-25
        // If "date" is not passed, defaults to today. Frontend calls this every few
        // seconds for "Live", or once with today's date for the "Today" filter.
        [HttpGet("tracking")]
        public async Task<IActionResult> GetTracking(
            [FromQuery] string ip,
            [FromQuery] string port,
            [FromQuery] string? date,
            [FromServices] ICameraAuthService auth,
            [FromServices] IHttpClientFactory httpFactory,
            CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(ip))
                return BadRequest("ip is required");

            string? token;
            try
            {
                token = await auth.GetTokenAsync(ip, port, ct);
            }
            catch (Exception ex)
            {
                return StatusCode(502, new { message = ex.Message });
            }
            if (token == null)
                return StatusCode(502, new { message = "Login succeeded but no token in response" });

            var targetDate = string.IsNullOrWhiteSpace(date)
                ? DateTime.Now.Date
                : DateTime.Parse(date);

            var start = targetDate.ToString("yyyy-MM-ddT00:00:00");
            var end = targetDate.ToString("yyyy-MM-ddT23:59:59");
            var url = $"http://{ip}:{port}/api/v1/analytics/events?start_date={Uri.EscapeDataString(start)}&end_date={Uri.EscapeDataString(end)}&page=1";

            var client = httpFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Token {token}");
            client.DefaultRequestHeaders.Add("x-cue", "34db55e07f7b39df480284397f7f42ec");
            client.DefaultRequestHeaders.Add("x-salt", "683239");
            client.DefaultRequestHeaders.Add("x-alpha", "21");

            static string? Str(System.Text.Json.JsonElement el, string name) =>
                el.TryGetProperty(name, out var v) && v.ValueKind == System.Text.Json.JsonValueKind.String ? v.GetString() : null;

            static bool Bool(System.Text.Json.JsonElement el, string name) =>
                el.TryGetProperty(name, out var v) && v.ValueKind == System.Text.Json.JsonValueKind.True;

            var rows = new List<object>();
            int page = 1;
            bool hasNext = true;

            while (hasNext && page <= 30)   // safety cap = about 600 rows
            {
                var response = await client.GetAsync(url.Replace("&page=1", $"&page={page}"), ct);
                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, new { message = "Camera events call failed" });

                using var doc = System.Text.Json.JsonDocument.Parse(await response.Content.ReadAsStreamAsync(ct));

                foreach (var e in doc.RootElement.GetProperty("results").EnumerateArray())
                {
                    var ts = e.GetProperty("timestamp").GetDateTimeOffset();

                    int vType = e.TryGetProperty("vehicle_type", out var vt)
                                && vt.ValueKind == System.Text.Json.JsonValueKind.Number ? vt.GetInt32() : 0;

                    string? evidence = null;
                    if (e.TryGetProperty("evidence_image_url", out var ev)
                        && ev.ValueKind == System.Text.Json.JsonValueKind.Array
                        && ev.GetArrayLength() > 0)
                        evidence = ev[0].GetString();

                    rows.Add(new
                    {
                        Date = ts.ToString("dd/MM/yyyy"),
                        Time = ts.ToString("hh:mm tt"),
                        Name = Str(e, "name") ?? "Unknown",
                        RegistrationNumber = Str(e, "number_plate"),
                        PlateImage = Str(e, "plate_image_url"),
                        EvidenceImage = evidence,
                        VehicleType = vType == 1 ? "two" : "four",
                        Auth = Bool(e, "auth"),
                        IsBlacklisted = Bool(e, "is_blacklisted"),
                        CameraName = Str(e, "camera_name"),
                        FlatNumber = Str(e, "flat_number")
                    });
                }

                hasNext = doc.RootElement.TryGetProperty("next", out var nx)
                          && nx.ValueKind == System.Text.Json.JsonValueKind.String;
                page++;
            }

            return Ok(rows);
        }
    }
}