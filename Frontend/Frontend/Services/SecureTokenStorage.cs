using Microsoft.AspNetCore.DataProtection;

namespace Frontend.Services
{
    /// <summary>
    /// Stores JWT in server session and an HttpOnly protected cookie (backup).
    /// Token never goes to browser localStorage — only the server uses it for backend API calls.
    /// </summary>
    public class SecureTokenStorage
    {
        private const string SessionKey = "JwtToken";
        private const string CookieName = ".DBTC.Auth";
        private readonly IHttpContextAccessor _http;
        private readonly IDataProtector _protector;

        public SecureTokenStorage(IHttpContextAccessor http, IDataProtectionProvider dataProtection)
        {
            _http = http;
            _protector = dataProtection.CreateProtector("AMS.Frontend.JwtToken.v1");
        }

        public async Task SaveAsync(string token)
        {
            var ctx = _http.HttpContext ?? throw new InvalidOperationException("No HttpContext");
            await ctx.Session.LoadAsync();

            ctx.Session.SetString(SessionKey, token);

            var options = new CookieOptions
            {
                HttpOnly = true,
                Secure = ctx.Request.IsHttps,
                SameSite = SameSiteMode.Lax,
                Path = "/",
                IsEssential = true,
                MaxAge = TimeSpan.FromMinutes(30)
            };

            ctx.Response.Cookies.Append(CookieName, _protector.Protect(token), options);
            await ctx.Session.CommitAsync();
        }

        public async Task<string?> GetAsync()
        {
            var ctx = _http.HttpContext;
            if (ctx == null) return null;

            await ctx.Session.LoadAsync();

            var fromSession = ctx.Session.GetString(SessionKey);
            if (!string.IsNullOrWhiteSpace(fromSession))
                return fromSession;

            if (!ctx.Request.Cookies.TryGetValue(CookieName, out var protectedValue))
                return null;

            try
            {
                var token = _protector.Unprotect(protectedValue);
                ctx.Session.SetString(SessionKey, token);
                await ctx.Session.CommitAsync();
                return token;
            }
            catch
            {
                return null;
            }
        }

        public async Task ClearAsync()
        {
            var ctx = _http.HttpContext;
            if (ctx == null) return;

            await ctx.Session.LoadAsync();
            ctx.Session.Remove(SessionKey);
            ctx.Response.Cookies.Delete(CookieName);
            await ctx.Session.CommitAsync();
        }
    }
}
