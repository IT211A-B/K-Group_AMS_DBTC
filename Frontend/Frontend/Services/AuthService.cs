using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Frontend.Models;

namespace Frontend.Services
{
    public class AuthService
    {
        private readonly string _backendBase;

        public AuthService(IConfiguration config)
        {
            _backendBase = config["BackendBase"]?.TrimEnd('/')
                ?? "https://k-group-ams-dbtc-11f4.onrender.com";
        }

        private static HttpClient CreateClient(string? token = null)
        {
            var client = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
            if (!string.IsNullOrWhiteSpace(token))
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        private static string? ExtractToken(JsonElement root)
        {
            foreach (var name in new[] { "token", "Token", "accessToken", "access_token" })
            {
                if (root.TryGetProperty(name, out var prop) && prop.ValueKind == JsonValueKind.String)
                {
                    var v = prop.GetString();
                    if (!string.IsNullOrWhiteSpace(v)) return v;
                }
            }
            return null;
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

                if (!response.IsSuccessStatusCode)
                {
                    var errBody = await response.Content.ReadAsStringAsync();
                    var errMsg = "Invalid email or password.";
                    try
                    {
                        using var errDoc = JsonDocument.Parse(errBody);
                        var root = errDoc.RootElement;
                        if (root.TryGetProperty("detail", out var d)) errMsg = d.GetString() ?? errMsg;
                        else if (root.TryGetProperty("Detail", out var d2)) errMsg = d2.GetString() ?? errMsg;
                        else if (root.TryGetProperty("message", out var m)) errMsg = m.GetString() ?? errMsg;
                    }
                    catch { }
                    return (false, "", "", "", "", errMsg);
                }

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var token = ExtractToken(doc.RootElement);

                if (string.IsNullOrWhiteSpace(token))
                    return (false, "", "", "", "", "Server did not return a token.");

                var (userId, jwtRole, jwtEmail, jwtName) = JwtPayloadReader.Read(token);

                var role = !string.IsNullOrEmpty(jwtRole)
                    ? jwtRole
                    : JwtPayloadReader.InferRoleFromEmail(model.Email);

                if (string.IsNullOrEmpty(role))
                    return (false, "", "", "", "",
                        "Your account has no role assigned. Contact an administrator.");

                var fullName = !string.IsNullOrWhiteSpace(jwtName)
                    ? jwtName
                    : model.Email.Split('@')[0];

                if (string.IsNullOrEmpty(userId))
                    userId = model.Email;

                if (role == "admin" && !string.IsNullOrEmpty(token))
                {
                    try
                    {
                        using var authClient = CreateClient(token);
                        var userResponse = await authClient.GetAsync($"{_backendBase}/api/User");
                        if (userResponse.IsSuccessStatusCode)
                        {
                            var usersJson = await userResponse.Content.ReadAsStringAsync();
                            userId = ResolveUserIdFromList(usersJson, model.Email) ?? userId;
                            fullName = ResolveUserNameFromList(usersJson, model.Email) ?? fullName;
                        }
                    }
                    catch { }
                }

                return (true, role, fullName, userId, token, "");
            }
            catch (TaskCanceledException)
            {
                return (false, "", "", "", "",
                    "Cannot connect to server: Request timed out. The server may be sleeping (Render cold start). Please try again in 30 seconds.");
            }
            catch (Exception ex)
            {
                return (false, "", "", "", "", $"Cannot connect to server: {ex.Message}");
            }
        }

        private static string? ResolveUserIdFromList(string json, string email)
        {
            try
            {
                var arr = ApiJsonHelper.UnwrapToArrayElement(json);
                if (arr.ValueKind != JsonValueKind.Array) return null;
                var emailLower = email.ToLowerInvariant();
                foreach (var user in arr.EnumerateArray())
                {
                    var userEmail = user.TryGetProperty("email", out var e) ? e.GetString()?.ToLowerInvariant() ?? "" : "";
                    if (userEmail != emailLower) continue;
                    if (user.TryGetProperty("user_ID", out var id)) return id.GetString();
                    if (user.TryGetProperty("id", out var id2)) return id2.GetString();
                }
            }
            catch { }
            return null;
        }

        private static string? ResolveUserNameFromList(string json, string email)
        {
            try
            {
                var arr = ApiJsonHelper.UnwrapToArrayElement(json);
                if (arr.ValueKind != JsonValueKind.Array) return null;
                var emailLower = email.ToLowerInvariant();
                foreach (var user in arr.EnumerateArray())
                {
                    var userEmail = user.TryGetProperty("email", out var e) ? e.GetString()?.ToLowerInvariant() ?? "" : "";
                    if (userEmail != emailLower) continue;
                    if (user.TryGetProperty("full_Name", out var n)) return n.GetString();
                }
            }
            catch { }
            return null;
        }
    }
}
