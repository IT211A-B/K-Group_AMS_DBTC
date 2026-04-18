using System.Text;
using System.Text.Json;

namespace Frontend.Services
{
    public class ApiService
    {
        private readonly IHttpClientFactory _http;
        private const string BackendBase = "http://localhost:5096";

        public ApiService(IHttpClientFactory http) => _http = http;

        private HttpClient Client() => _http.CreateClient("backend");

        public async Task<(bool Ok, string Body, int Status)> GetAsync(string path)
        {
            try
            {
                var res = await Client().GetAsync($"{BackendBase}{path}");
                var body = await res.Content.ReadAsStringAsync();
                return (res.IsSuccessStatusCode, body, (int)res.StatusCode);
            }
            catch (Exception ex) { return (false, ex.Message, 503); }
        }

        public async Task<(bool Ok, string Body, int Status)> PostAsync(string path, object dto)
        {
            try
            {
                var json = JsonSerializer.Serialize(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var res = await Client().PostAsync($"{BackendBase}{path}", content);
                var body = await res.Content.ReadAsStringAsync();
                return (res.IsSuccessStatusCode, body, (int)res.StatusCode);
            }
            catch (Exception ex) { return (false, ex.Message, 503); }
        }

        public async Task<(bool Ok, string Body, int Status)> PutAsync(string path, object dto)
        {
            try
            {
                var json = JsonSerializer.Serialize(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var res = await Client().PutAsync($"{BackendBase}{path}", content);
                var body = await res.Content.ReadAsStringAsync();
                return (res.IsSuccessStatusCode, body, (int)res.StatusCode);
            }
            catch (Exception ex) { return (false, ex.Message, 503); }
        }

        public async Task<(bool Ok, string Body, int Status)> DeleteAsync(string path)
        {
            try
            {
                var res = await Client().DeleteAsync($"{BackendBase}{path}");
                var body = await res.Content.ReadAsStringAsync();
                return (res.IsSuccessStatusCode, body, (int)res.StatusCode);
            }
            catch (Exception ex) { return (false, ex.Message, 503); }
        }
    }
}