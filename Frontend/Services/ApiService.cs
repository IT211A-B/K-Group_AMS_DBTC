using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Frontend.Services
{
    public class ApiService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _backendBase;

        public ApiService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _backendBase = configuration["BackendBase"]
                ?? "https://k-group-ams-dbtc-11f4.onrender.com";
        }

        private string? GetToken()
            => _httpContextAccessor.HttpContext?.Session.GetString("JwtToken");

        private HttpClient CreateClient()
        {
            var handler = new HttpClientHandler();
            var client = new HttpClient(handler);
            client.BaseAddress = new Uri(_backendBase);
            client.Timeout = TimeSpan.FromSeconds(60);

            client.DefaultRequestHeaders.Add("Origin", _backendBase);
            client.DefaultRequestHeaders.Add("Referer", _backendBase + "/");
            client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");

            var token = GetToken();
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

            return client;
        }

        private static (bool Ok, string Body, int Status) MapResponse(
            HttpResponseMessage res, string body, string emptyFallback)
        {
            var status = (int)res.StatusCode;
            if (status == StatusCodes.Status429TooManyRequests)
            {
                return (false,
                    "{\"message\":\"Too many requests. Please wait and try again.\"}",
                    status);
            }

            return (res.IsSuccessStatusCode, body.Length > 0 ? body : emptyFallback, status);
        }

        public async Task<(bool Ok, string Body, int Status)> GetAsync(string path)
        {
            try
            {
                using var client = CreateClient();
                var res = await client.GetAsync(path);
                var body = await res.Content.ReadAsStringAsync();
                return MapResponse(res, body, "[]");
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
                return MapResponse(res, body, "{}");
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
                return MapResponse(res, body, "{}");
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
                return MapResponse(res, body, "{}");
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
