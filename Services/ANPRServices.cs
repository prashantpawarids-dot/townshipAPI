using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Township_API.Controllers;
using Township_API.Models;
using static Township_API.Models.commonTypes;

namespace Township_API.Services
{
    public class ANPRServices
    {
        private readonly HttpClient _httpClient;
        private readonly ApiSettings _apiSettings;

        public ANPRServices(HttpClient httpClient, IOptions<ApiSettings> apiSettings)
        {
            _httpClient = httpClient;
            _apiSettings = apiSettings.Value;
            _httpClient = httpClient;
            _apiSettings = apiSettings.Value;
        }

        //[HttpGet]
         public async Task<string> GetTokenAsync(string _username= "Admin", string _password= "Admin@1923")
        {
            var loginData = new { username = _username, password = _password};

            var jsonBody = new StringContent(JsonSerializer.Serialize(loginData), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_apiSettings.AuthUrl, jsonBody);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var tokenObj = JsonDocument.Parse(json);
            return tokenObj.RootElement.GetProperty("token").GetString();
        }

        //[HttpPost]
        //[Authorize]
        public async Task<string> RegisterANPR_Vehicle_inCamera([FromBody] ANPR_Vehicle vehicle)
        {
            try
            {
                var token = await GetTokenAsync();
                Console.WriteLine("received Token:" + token.ToString());

                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);



                // 3. Convert to JSON
                var jsonBody = new StringContent(
                    JsonSerializer.Serialize(vehicle),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync(_apiSettings.DataUrl, jsonBody);

                response.EnsureSuccessStatusCode();

                Console.WriteLine("Responce received : "+response.ToString());


                return await response.Content.ReadAsStringAsync();

            }
            catch (HttpRequestException ex)
            {
                // API call failed (bad request, unauthorized, etc.)
                return $"Request failed: {ex.Message}";
            }
            catch (Exception ex)
            {
                // Other errors (network, serialization, etc.)
                return $"Unexpected error: {ex.Message}";
            }


        }
        
        //[HttpPost]
        //[Authorize]
        public async Task<string> DeleteANPR_Vehicle_inCamera(string id)
        {
            try
            {
                var token = await GetTokenAsync();
                Console.WriteLine("received Token:" + token.ToString());

                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
 
                var response = await _httpClient.DeleteAsync(_apiSettings.DataUrl+"/"+id);

                response.EnsureSuccessStatusCode();

                Console.WriteLine("Responce received : " + response.ToString());


                return await response.Content.ReadAsStringAsync();

            }
            catch (HttpRequestException ex)
            {
                // API call failed (bad request, unauthorized, etc.)
                return $"Request failed: {ex.Message}";
            }
            catch (Exception ex)
            {
                // Other errors (network, serialization, etc.)
                return $"Unexpected error: {ex.Message}";
            }


        }
        
        //[HttpGet]
        //[Authorize]
        public async Task<string> GetProtectedDataAsync()
        {
            try
            {
                var token = await GetTokenAsync();

                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync(_apiSettings.DataUrl);

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                // API call failed (bad request, unauthorized, etc.)
                return $"Request failed: {ex.Message}";
            }
            catch (Exception ex)
            {
                // Other errors (network, serialization, etc.)
                return $"Unexpected error: {ex.Message}";
            }
        }
       
    }


}
