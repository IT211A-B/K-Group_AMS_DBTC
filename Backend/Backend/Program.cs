using Backend.Backend;
using Backend.Backend.Configuration;
using Backend.Backend.Interface.ConfigureInterface;
using Backend.Backend.Interface.RepositoryInterface;
using Backend.Backend.Interface.ServiceInterface;
using Backend.Backend.Model;
using Backend.Backend.Repository;
using Backend.Backend.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using Backend.Backend.Helper;
using Microsoft.OpenApi.Models;

using SeedRole = Backend.Backend.Seeder.RolePermissionandPermission;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Add TimeOnly Converter to Minimize TimeOnly Becoming a String
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter());
    });

// Learn more about configuring Swagger/OpenAPI at https://akrp.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DatabaseLibrary>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AttendanceDBString")));

//  enable authorize (authorization services)
builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
{
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        new string[] {}
    }
    });
    var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
	options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// Add JWT
// so that, can inject config anywhere, DI pattern
builder.Services.Configure<ConfigurationModel>(
    builder.Configuration.GetSection("Jwt"));

//register cors and allow mvc origin
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMvcClient",
        policy => policy.WithOrigins("https://localhost:5001") // MVC app origin
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});

// Repository DI for the Service: so service can use this automatically 
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<ITeacherRepository, TeacherRepository>();
builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<ISectionRepository, SectionRepository>();
builder.Services.AddScoped<IProgramRepository, ProgramRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();
builder.Services.AddScoped<IAttendanceStudentRepository, AttendanceStudentRepository>();


// Adding DI for the Controller: so controller can use this automatically 
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IRolePermissionService, RolePermissionService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<ISectionService, SectionService>();
builder.Services.AddScoped<IProgramService, ProgramService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAttendanceStudentService, AttendanceStudentService>();

// Adding DI for the custom authorization system
// singleton as it does not rely on each request but once and dies if app closes
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
//  evaluates user claims compared to those policies per request
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddScoped<IClaimService,ClaimService>();
builder.Services.AddScoped<IJwtService,JwtService>();

builder.Services
    .AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<DatabaseLibrary>()
    .AddDefaultTokenProviders();
    
var jwtSection = builder.Configuration.GetSection("Jwt"); //get all hwt section, use to grab the key:value pair of the config, which is not hardcoded but in environment

var jwtKey = jwtSection["Key"]!; // gimme value for key
var issuer = jwtSection["Issuer"]; // for issuer
var audience = jwtSection["Audience"];  // and for audience 

// will add environment validation for https enforcement
var env = builder.Environment;

// helps to get config once
builder.Services
    .AddAuthentication(options => // this will check if the controller has Authorized or such to check for validity
    {   
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // Get the string bearer
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // Give Status Code of 401 if not authorized (when authentication fails)
    })
    .AddJwtBearer(options => // Validate the authentication token
    {
        options.RequireHttpsMetadata = !env.IsDevelopment(); // Will false if run locally but available at prod

        options.SaveToken = true;            // optional: keeps token in HttpContext

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true, 
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)), // check signature if its valid by recalculating it again

            ValidateIssuer = true,
            ValidIssuer = issuer, // check issuer

            ValidateAudience = true,
            ValidAudience = audience, // check audience

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero // strict expiration
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // Authorization Header even if its default, we make it directly insure correct handling
                // * Main priority
                // Valid check bearer and extract the token from it
                var authHeader = context.Request.Headers["Authorization"].FirstOrDefault(); // Get Headers
                if (!string.IsNullOrWhiteSpace(authHeader) &&
                    authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)) // Insure bearer 
                {
                    context.Token = authHeader.Substring("Bearer ".Length).Trim();
                    return Task.CompletedTask;
                }

                // (for browser clients)
                // if saved from cookie, it will extract it in the cookie
                var cookieToken = context.Request.Cookies["accessToken"];
                if (!string.IsNullOrWhiteSpace(cookieToken))
                {
                    context.Token = cookieToken;
                    return Task.CompletedTask;
                }

                //No token found, let pipeline handle Unauthorized
                return Task.CompletedTask;
            }
        };
    });

var app = builder.Build();

// Using 'using' to despose	a logic / code block if done once
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var context = services.GetRequiredService<DatabaseLibrary>();

    await SeedRole.SeedRolePermissionNPermissionAsync(roleManager, context);
}

// Local Dev
    //app.UseHttpsRedirection(); // Authomatically redirect Http request to Https

// Pre Middleware hooks that asks before every transactions
app.UseAuthentication();  // Authenticate user
app.UseAuthorization();   // after authenticate enforce authorize rule in the controller, to open the controller if authenticated


    // Allow the use og swagger both in production and Local dev
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
    c.RoutePrefix = "swagger";
});

var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";

app.Urls.Clear();
app.Urls.Add($"http://0.0.0.0:{port}");

app.MapControllers();

app.Run();
