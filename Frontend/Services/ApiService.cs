using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Frontend.Services
{
    public class ApiService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string BackendBase = "https://k-group-ams-dbtc-11f4.onrender.com";

        public ApiService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private string? GetToken()
            => _httpContextAccessor.HttpContext?.Session.GetString("JwtToken");

        private HttpClient CreateClient()
        {
            var handler = new HttpClientHandler();
            var client = new HttpClient(handler);
            client.BaseAddress = new Uri(BackendBase);
            client.Timeout = TimeSpan.FromSeconds(60);

            client.DefaultRequestHeaders.Add("Origin", BackendBase);
            client.DefaultRequestHeaders.Add("Referer", BackendBase + "/");
            client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");

            var token = GetToken();
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

            return client;
        }

        public async Task<(bool Ok, string Body, int Status)> GetAsync(string path)
        {
            try
            {
                using var client = CreateClient();
                var res = await client.GetAsync(path);
                var body = await res.Content.ReadAsStringAsync();
                return (res.IsSuccessStatusCode, body.Length > 0 ? body : "[]", (int)res.StatusCode);
            }
            catch (Exception ex)
            {
                return (false, $"{{\"message\":\"{ex.Message}\"}}", 503);
            }
        }

        public async Task<(bool Ok, string Body, int Status)> PostAsync(string path, object dto)
        {
            try
            {
                using var client = CreateClient();
                var opts = new JsonSerializerOptions { PropertyNamingPolicy = null };
                var json = JsonSerializer.Serialize(dto, opts);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var res = await client.PostAsync(path, content);
                var body = await res.Content.ReadAsStringAsync();
                return (res.IsSuccessStatusCode, body.Length > 0 ? body : "{}", (int)res.StatusCode);
            }
            catch (Exception ex)
            {
                return (false, $"{{\"message\":\"{ex.Message}\"}}", 503);
            }
        }

        public async Task<(bool Ok, string Body, int Status)> PutAsync(string path, object dto)
        {
            try
            {
                using var client = CreateClient();
                var opts = new JsonSerializerOptions { PropertyNamingPolicy = null };
                var json = JsonSerializer.Serialize(dto, opts);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var res = await client.PutAsync(path, content);
                var body = await res.Content.ReadAsStringAsync();
                return (res.IsSuccessStatusCode, body.Length > 0 ? body : "{}", (int)res.StatusCode);
            }
            catch (Exception ex)
            {
                return (false, $"{{\"message\":\"{ex.Message}\"}}", 503);
            }
        }

        public async Task<(bool Ok, string Body, int Status)> DeleteAsync(string path)
        {
            try
            {
                using var client = CreateClient();
                var res = await client.DeleteAsync(path);
                var body = await res.Content.ReadAsStringAsync();
                return (res.IsSuccessStatusCode, body.Length > 0 ? body : "{}", (int)res.StatusCode);
            }
            catch (Exception ex)
            {
                return (false, $"{{\"message\":\"{ex.Message}\"}}", 503);
            }
        }

        public async Task<(bool Ok, byte[] Data, string ContentType)> GetBytesAsync(string path)
        {
            try
            {
                using var client = CreateClient();
                var res = await client.GetAsync(path);
                if (res.IsSuccessStatusCode)
                {
                    var data = await res.Content.ReadAsByteArrayAsync();
                    var ct = res.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
                    return (true, data, ct);
                }
                return (false, Array.Empty<byte>(), "");
            }
            catch
            {
                return (false, Array.Empty<byte>(), "");
            }
        }
    }
}