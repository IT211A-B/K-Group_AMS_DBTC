using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Frontend.Models;

namespace Frontend.Services
{
    public class AuthService
    {
        private readonly string _backendBase;

        public AuthService(IConfiguration configuration)
        {
            _backendBase = configuration["BackendBase"]
                ?? "https://k-group-ams-dbtc-11f4.onrender.com";
        }

        private HttpClient CreateClient(string? token = null)
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(60);
            client.DefaultRequestHeaders.Add("Origin", _backendBase);
            client.DefaultRequestHeaders.Add("Referer", _backendBase + "/");
            client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        public async Task<(bool Success, string Role, string Name, string UserId, string Token, string ErrorMessage)>
            AuthenticateAsync(LoginViewModel model)
        {
            try
            {
                using var client = CreateClient();
                var payload = JsonSerializer.Serialize(new { email = model.Email, password = model.Password });
                var content = new StringContent(payload, Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"{_backendBase}/LogIn", content);

                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    return (false, "", "", "", "",
                        "Too many requests. Please wait a minute and try again.");
                }

                if (!response.IsSuccessStatusCode)
                {
                    var errBody = await response.Content.ReadAsStringAsync();
                    string errMsg = "Invalid email or password.";
                    try
                    {
                        using var errDoc = JsonDocument.Parse(errBody);
                        if (errDoc.RootElement.TryGetProperty("message", out var mp))
                            errMsg = mp.GetString() ?? errMsg;
                    }
                    catch { }
                    return (false, "", "", "", "", errMsg);
                }

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var token = doc.RootElement.TryGetProperty("token", out var tp) ? tp.GetString() ?? "" : "";

                if (string.IsNullOrEmpty(token))
                    return (false, "", "", "", "", "Server did not return a token.");

                var emailLower = model.Email.ToLower();
                string role;
                if (emailLower.Contains("@admin")) role = "admin";
                else if (emailLower.Contains("@local")) role = "teacher";
                else if (emailLower.Contains("@dbtc-cebu")) role = "student";
                else return (false, "", "", "", "", "Unrecognized email domain. Use @admin, @local, or @dbtc-cebu.");

                string userId = "";
                string fullName = model.Email.Split('@')[0];

                try
                {
                    using var authClient = CreateClient(token);
                    var userResponse = await authClient.GetAsync($"{_backendBase}/api/User");
                    if (userResponse.IsSuccessStatusCode)
                    {
                        var usersJson = await userResponse.Content.ReadAsStringAsync();
                        using var usersDoc = JsonDocument.Parse(usersJson);

                        var root = usersDoc.RootElement;
                        JsonElement arrayEl = root;
                        if (root.ValueKind == JsonValueKind.Object &&
                            root.TryGetProperty("$values", out var valProp))
                            arrayEl = valProp;

                        if (arrayEl.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var user in arrayEl.EnumerateArray())
                            {
                                var userEmail = user.TryGetProperty("email", out var eProp)
                                    ? eProp.GetString()?.ToLower() ?? "" : "";
                                if (userEmail != emailLower) continue;

                                userId = user.TryGetProperty("user_ID", out var idProp)
                                    ? idProp.GetString() ?? "" : "";
                                fullName = user.TryGetProperty("full_Name", out var nameProp)
                                    ? nameProp.GetString() ?? fullName : fullName;
                                break;
                            }
                        }
                    }
                }
                catch { }

                return (true, role, fullName, userId, token, "");
            }
            catch (TaskCanceledException)
            {
                return (false, "", "", "", "", "Cannot connect to server: Request timed out. The server may be sleeping (Render cold start). Please try again in 30 seconds.");
            }
            catch (Exception ex)
            {
                return (false, "", "", "", "", $"Cannot connect to server: {ex.Message}");
            }
        }
    }
}
