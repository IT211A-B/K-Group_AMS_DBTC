using Backend.Backend;
using Backend.Backend.Interface.RepositoryInterface;
using Backend.Backend.Interface.ServiceInterface;
using Backend.Backend.Repository;
using Backend.Backend.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SeedRole = Backend.Backend.Seeder.RolePermissionandPermission;
using Backend.Backend.Configuration;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://akrp.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DatabaseLibrary>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AttendanceDBString")));

builder.Services.AddSwaggerGen(options =>
{
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
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddScoped<IProgramRepository, ProgramRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();


// Adding DI for the Controller: so controller can use this automatically 
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IRolePermissionService, RolePermissionService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<IProgramService, ProgramService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();


var app = builder.Build();

// Using 'using' to despose	a logic / code block if done once
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var context = services.GetRequiredService<DatabaseLibrary>();

    await SeedRole.SeedRolePermissionNPermissionAsync(roleManager, context);
}

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
	// Allow the use og swagger both in production and Local dev
	app.UseSwagger();
	app.UseSwaggerUI();
}

// Local Dev
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection(); // Authomatically redirect Http request to Https
}

app.UseAuthorization();

app.MapControllers();

app.Run();
