using System.Net.Http.Headers;
using System.Text;

namespace Frontend.Services
{
    public class ApiService
    {
        private readonly SecureTokenStorage _tokenStorage;
        private readonly string _backendBase;

        public ApiService(SecureTokenStorage tokenStorage, IConfiguration config)
        {
            _tokenStorage = tokenStorage;
            _backendBase = config["BackendBase"]?.TrimEnd('/')
                ?? "https://k-group-ams-dbtc-11f4.onrender.com";
        }

        private async Task<HttpClient> CreateClientAsync()
        {
            var client = new HttpClient { BaseAddress = new Uri(_backendBase), Timeout = TimeSpan.FromSeconds(60) };
            var token = await _tokenStorage.GetAsync();
            if (!string.IsNullOrWhiteSpace(token))
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        public async Task<(bool Ok, string Body, int Status)> GetAsync(string path)
        {
            try
            {
                using var client = await CreateClientAsync();
                var res = await client.GetAsync(path);
                var body = await res.Content.ReadAsStringAsync();
                return (res.IsSuccessStatusCode, body.Length > 0 ? body : "[]", (int)res.StatusCode);
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
                return (res.IsSuccessStatusCode, body.Length > 0 ? body : "{}", (int)res.StatusCode);
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
                using var client = await CreateClientAsync();
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