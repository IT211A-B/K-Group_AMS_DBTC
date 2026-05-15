<<<<<<< HEAD:Frontend/Program.cs
﻿using System.Threading.RateLimiting;
using Frontend.Services;
using Microsoft.AspNetCore.RateLimiting;
=======
﻿using Frontend.Services;
using Microsoft.AspNetCore.HttpOverrides;
>>>>>>> e184fcbcfe06e47564902f542f8e3d52da1323aa:Frontend/Frontend/Program.cs

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

<<<<<<< HEAD:Frontend/Program.cs
var loginRateLimit = builder.Configuration.GetSection("RateLimiting");
var loginPermitLimit = loginRateLimit.GetValue("LoginPermitLimit", 15);
var loginWindowSeconds = loginRateLimit.GetValue("LoginWindowSeconds", 60);

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            message = "Too many login attempts. Please wait and try again."
        }, token);
    };

    options.AddPolicy("login", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = loginPermitLimit,
                Window = TimeSpan.FromSeconds(loginWindowSeconds),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));
=======
builder.Services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(options =>
{
    options.SuppressMapClientErrors = true;
>>>>>>> e184fcbcfe06e47564902f542f8e3d52da1323aa:Frontend/Frontend/Program.cs
});

builder.Services.AddCors(options => {
    options.AddPolicy("AllowFrontend", policy => {
        policy.WithOrigins(
            "https://k-group-ams-dbtc-11f4.onrender.com",
            "http://localhost:5288",
            "https://localhost:7258"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

builder.Services.AddDataProtection();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".DBTC.Session";
    options.Cookie.Path = "/";
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<StudentService>();
builder.Services.AddScoped<TeacherService>();
builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped<SecureTokenStorage>();
builder.Services.AddSingleton<NotificationStoreService>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

<<<<<<< HEAD:Frontend/Program.cs
app.UseRateLimiter();
=======
app.Use(async (ctx, next) => {
    var path = ctx.Request.Path.Value;
    if (path != null && path.Length > 1 && path.EndsWith("/"))
        ctx.Request.Path = path.TrimEnd('/');
    await next();
});
>>>>>>> e184fcbcfe06e47564902f542f8e3d52da1323aa:Frontend/Frontend/Program.cs

app.UseCors("AllowFrontend");
app.UseSession();
app.Use(async (context, next) =>
{
    if (context.Session.IsAvailable)
        await context.Session.LoadAsync();
    await next();
});
app.UseAuthorization();
app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();