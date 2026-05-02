using System.Text;
using System.Text.Json;
using Frontend.Models;

namespace Frontend.Services
{
    public class AuthService
    {
        private const string BackendBase = "https://k-group-ams-dbtc-11f4.onrender.com";

        public async Task<(bool Success, string Role, string Name, string UserId, string UserGroupId, string ErrorMessage)> AuthenticateAsync(LoginViewModel model)
        {
            try
            {
                using var client = new HttpClient();
                var payload = JsonSerializer.Serialize(new { email = model.Email, password = model.Password });
                var content = new StringContent(payload, Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"{BackendBase}/LogIn", content);

                if (response.IsSuccessStatusCode)
                {
                    // Determine role from email domain (matches backend logic)
                    var emailLower = model.Email.ToLower();
                    string role;
                    if (emailLower.Contains("@admin")) role = "admin";
                    else if (emailLower.Contains("@local")) role = "teacher";
                    else if (emailLower.Contains("@dbtc-cebu")) role = "student";
                    else role = "admin";

                    // Fetch user details to get the ULID user_ID
                    string userId = "";
                    string fullName = model.Email.Split('@')[0];
                    string userGroupId = "";

                    var userResponse = await client.GetAsync($"{BackendBase}/api/User");
                    if (userResponse.IsSuccessStatusCode)
                    {
                        var usersJson = await userResponse.Content.ReadAsStringAsync();
                        using var doc = JsonDocument.Parse(usersJson);
                        foreach (var user in doc.RootElement.EnumerateArray())
                        {
                            if (user.TryGetProperty("email", out var emailProp) &&
                                emailProp.GetString()?.ToLower() == emailLower)
                            {
                                if (user.TryGetProperty("user_ID", out var idProp))
                                    userId = idProp.GetString() ?? "";
                                if (user.TryGetProperty("full_Name", out var fnProp))
                                    fullName = fnProp.GetString() ?? fullName;
                                if (user.TryGetProperty("userGroup_ID", out var ugProp))
                                    userGroupId = ugProp.GetRawText().Trim('"');
                                break;
                            }
                        }
                    }

                    return (true, role, fullName, userId, userGroupId, "");
                }
                else
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    string errMsg = "Invalid email or password.";
                    try
                    {
                        using var doc = JsonDocument.Parse(errorBody);
                        if (doc.RootElement.TryGetProperty("message", out var mp))
                            errMsg = mp.GetString() ?? errMsg;
                    }
                    catch { errMsg = errorBody; }
                    return (false, "", "", "", "", errMsg);
                }
            }
            catch (Exception ex)
            {
                return (false, "", "", "", "", $"Cannot connect to server: {ex.Message}");
            }
        }
    }
}