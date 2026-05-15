using System.Text;
using System.Text.Json;

namespace Frontend.Services
{
    public static class JwtPayloadReader
    {
        private static readonly string[] RoleClaimTypes =
        {
            "role",
            "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
        };

        private static readonly string[] IdClaimTypes =
        {
            "sub",
            "nameid",
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
        };

        private static readonly string[] EmailClaimTypes =
        {
            "email",
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"
        };

        private static readonly string[] NameClaimTypes =
        {
            "name",
            "unique_name",
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"
        };

        public static (string UserId, string Role, string Email, string UserName) Read(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return ("", "", "", "");

            try
            {
                var parts = token.Split('.');
                if (parts.Length < 2) return ("", "", "", "");

                var payload = parts[1];
                var pad = payload.Length % 4;
                if (pad == 2) payload += "==";
                else if (pad == 3) payload += "=";

                var json = Encoding.UTF8.GetString(
                    Convert.FromBase64String(payload.Replace('-', '+').Replace('_', '/')));

                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                var userId = FirstClaim(root, IdClaimTypes);
                var role = NormalizeRole(FirstClaim(root, RoleClaimTypes));
                var email = FirstClaim(root, EmailClaimTypes);
                var userName = FirstClaim(root, NameClaimTypes);

                return (userId, role, email, userName);
            }
            catch
            {
                return ("", "", "", "");
            }
        }

        private static string FirstClaim(JsonElement root, string[] types)
        {
            foreach (var prop in root.EnumerateObject())
            {
                var key = prop.Name;
                if (!types.Any(t => string.Equals(t, key, StringComparison.OrdinalIgnoreCase)))
                    continue;

                if (prop.Value.ValueKind == JsonValueKind.String)
                    return prop.Value.GetString() ?? "";

                if (prop.Value.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in prop.Value.EnumerateArray())
                    {
                        if (item.ValueKind == JsonValueKind.String)
                            return item.GetString() ?? "";
                    }
                }
            }
            return "";
        }

        private static string NormalizeRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role)) return "";
            return role.Trim().ToLowerInvariant() switch
            {
                "admin" or "administrator" => "admin",
                "teacher" => "teacher",
                "student" => "student",
                _ => role.Trim().ToLowerInvariant()
            };
        }

        public static string InferRoleFromEmail(string email)
        {
            var e = email.ToLowerInvariant();
            if (e.Contains("@admin")) return "admin";
            if (e.Contains("@local")) return "teacher";
            if (e.Contains("@dbtc-cebu")) return "student";
            return "";
        }
    }
}
