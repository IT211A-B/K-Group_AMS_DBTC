using System.Net.Http.Headers;
using System.Text;

namespace Frontend.Services
{
    public class ApiService
    {
<<<<<<< HEAD:Frontend/Services/ApiService.cs
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _backendBase;

        public ApiService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _backendBase = configuration["BackendBase"]
=======
        private readonly SecureTokenStorage _tokenStorage;
        private readonly string _backendBase;

        public ApiService(SecureTokenStorage tokenStorage, IConfiguration config)
        {
            _tokenStorage = tokenStorage;
            _backendBase = config["BackendBase"]?.TrimEnd('/')
>>>>>>> e184fcbcfe06e47564902f542f8e3d52da1323aa:Frontend/Frontend/Services/ApiService.cs
                ?? "https://k-group-ams-dbtc-11f4.onrender.com";
        }

        private async Task<HttpClient> CreateClientAsync()
        {
<<<<<<< HEAD:Frontend/Services/ApiService.cs
            var handler = new HttpClientHandler();
            var client = new HttpClient(handler);
            client.BaseAddress = new Uri(_backendBase);
            client.Timeout = TimeSpan.FromSeconds(60);

            client.DefaultRequestHeaders.Add("Origin", _backendBase);
            client.DefaultRequestHeaders.Add("Referer", _backendBase + "/");
            client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");

            var token = GetToken();
            if (!string.IsNullOrEmpty(token))
=======
            var client = new HttpClient { BaseAddress = new Uri(_backendBase), Timeout = TimeSpan.FromSeconds(60) };
            var token = await _tokenStorage.GetAsync();
            if (!string.IsNullOrWhiteSpace(token))
>>>>>>> e184fcbcfe06e47564902f542f8e3d52da1323aa:Frontend/Frontend/Services/ApiService.cs
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
                using var client = await CreateClientAsync();
                var res = await client.GetAsync(path);
                var body = await res.Content.ReadAsStringAsync();
                return MapResponse(res, body, "[]");
            }
            catch (Exception ex)
            {
                return (false, $"{{\"message\":\"{ex.Message}\"}}", 503);
            }
        }

        public async Task<(bool Ok, string Body, int Status)> PostAsync(string path, string rawJson)
        {
            try
            {
                using var client = await CreateClientAsync();
                var content = new StringContent(rawJson, Encoding.UTF8, "application/json");
                var res = await client.PostAsync(path, content);
                var body = await res.Content.ReadAsStringAsync();
                return MapResponse(res, body, "{}");
            }
            catch (Exception ex)
            {
                return (false, $"{{\"message\":\"{ex.Message}\"}}", 503);
            }
        }

        public async Task<(bool Ok, string Body, int Status)> PutAsync(string path, string rawJson)
        {
            try
            {
                using var client = await CreateClientAsync();
                var content = new StringContent(rawJson, Encoding.UTF8, "application/json");
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
                using var client = await CreateClientAsync();
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
                using var client = await CreateClientAsync();
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
