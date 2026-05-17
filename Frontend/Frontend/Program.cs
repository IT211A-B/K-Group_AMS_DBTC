
﻿using Frontend.Services;
using Microsoft.AspNetCore.HttpOverrides;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(options =>
{
    options.SuppressMapClientErrors = true;
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

app.Use(async (ctx, next) => {
    var path = ctx.Request.Path.Value;
    if (path != null && path.Length > 1 && path.EndsWith("/"))
        ctx.Request.Path = path.TrimEnd('/');
    await next();
});

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