using System.Collections.Concurrent;
using System.Net.Http.Json;
using System.Text.Json;

namespace Township_API.Services
{
    public interface ICameraAuthService
    {
        Task<string?> GetTokenAsync(string ip, string port, CancellationToken ct);
    }

    // Each camera/ANPR box has its own local login API (confirmed via GateGuard:
    // POST /api/v1/sdk/login -> { token }). Token cached in memory per camera IP,
    // no DB involved.
    public class CameraAuthService : ICameraAuthService
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly IConfiguration _config;
        private static readonly ConcurrentDictionary<string, string> _tokenCache = new();

        public CameraAuthService(IHttpClientFactory httpFactory, IConfiguration config)
        {
            _httpFactory = httpFactory;
            _config = config;
        }

        public async Task<string?> GetTokenAsync(string ip, string port, CancellationToken ct)
        {
            var cacheKey = $"{ip}:{port}";
            if (_tokenCache.TryGetValue(cacheKey, out var cached))
                return cached;

            var username = _config["CameraAuth:Username"] ?? "admin";
            var password = _config["CameraAuth:Password"] ?? "admin";
            var url = $"http://{ip}:{port}/api/v1/sdk/login";

            var client = _httpFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("X-Cue", "34db55e07f7b39df480284397f7f42ec");
            request.Headers.Add("X-Salt", "683239");
            request.Headers.Add("X-Alpha", "21");
            request.Content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(new { username, password }),
                System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response;
            try
            {
                response = await client.SendAsync(request, ct);
            }
            catch (Exception ex)
            {
                throw new Exception($"Login request to {url} threw: {ex.Message}");
            }

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(ct);
                throw new Exception($"Login to {url} returned {(int)response.StatusCode}: {body}");
            }

            using var doc = JsonDocument.Parse(await response.Content.ReadAsStreamAsync(ct));
            if (!doc.RootElement.TryGetProperty("token", out var tokenEl))
                return null;

            var token = tokenEl.GetString();
            if (!string.IsNullOrEmpty(token))
                _tokenCache[cacheKey] = token;

            return token;
        }
    }
}