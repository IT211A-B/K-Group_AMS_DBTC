using System.Text;
using System.Text.Json;

namespace Frontend.Services
{
    public class ApiService
    {
        private const string BackendBase = "https://k-group-ams-dbtc-11f4.onrender.com";

        public ApiService() { }

        public async Task<(bool Ok, string Body, int Status)> GetAsync(string path)
        {
            try
            {
                var url = $"{BackendBase}{path}";
                Console.WriteLine($"[GET] {url}");
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(30);
                var res = await client.GetAsync(url);
                var body = await res.Content.ReadAsStringAsync();
                return (res.IsSuccessStatusCode, body ?? "[]", (int)res.StatusCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GET] Error: {ex.Message}");
                return (false, "[]", 503);
            }
        }

        public async Task<(bool Ok, string Body, int Status)> PostAsync(string path, object dto)
        {
            try
            {
                var url = $"{BackendBase}{path}";
                var json = JsonSerializer.Serialize(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(30);
                var res = await client.PostAsync(url, content);
                var body = await res.Content.ReadAsStringAsync();
                return (res.IsSuccessStatusCode, body ?? "{}", (int)res.StatusCode);
            }
            catch (Exception ex)
            {
                return (false, "{}", 503);
            }
        }

        public async Task<(bool Ok, string Body, int Status)> PutAsync(string path, object dto)
        {
            try
            {
                var url = $"{BackendBase}{path}";
                var json = JsonSerializer.Serialize(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(30);
                var res = await client.PutAsync(url, content);
                var body = await res.Content.ReadAsStringAsync();
                return (res.IsSuccessStatusCode, body ?? "{}", (int)res.StatusCode);
            }
            catch (Exception ex)
            {
                return (false, "{}", 503);
            }
        }

        public async Task<(bool Ok, string Body, int Status)> DeleteAsync(string path)
        {
            try
            {
                var url = $"{BackendBase}{path}";
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(30);
                var res = await client.DeleteAsync(url);
                var body = await res.Content.ReadAsStringAsync();
                return (res.IsSuccessStatusCode, body ?? "{}", (int)res.StatusCode);
            }
            catch (Exception ex)
            {
                return (false, "{}", 503);
            }
        }
    }
}