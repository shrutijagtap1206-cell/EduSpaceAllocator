using System.Text;
using EduSpaceAllocator.API.Auth;
using EduSpaceAllocator.API.Data;
using EduSpaceAllocator.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Consistent validation error response format
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(e => e.Value?.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value!.Errors.Select(x => x.ErrorMessage).ToArray()
            );

        var problem = new
        {
            success = false,
            statusCode = 400,
            message = "One or more validation errors occurred.",
            errors,
            timestamp = DateTime.UtcNow
        };

        return new BadRequestObjectResult(problem);
    };
});

builder.Services.AddOpenApi();

// Databases
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(connectionString));

// Identity & Role Configuration
builder.Services
    .AddIdentityCore<IdentityUser>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 6;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddSignInManager();

builder.Services.AddScoped<AuthService>();

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "EduSpaceAllocatorDevelopmentSecretKey2026VeryLong";
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "EduSpaceAllocator",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "EduSpaceAllocatorClient",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// Application services
builder.Services.AddScoped<SpaceService>();
builder.Services.AddScoped<LearningRequestService>();
builder.Services.AddScoped<CommunityService>();
builder.Services.AddScoped<AllocationService>();
builder.Services.AddScoped<PartnerService>();
builder.Services.AddScoped<CourseService>();
builder.Services.AddScoped<ScheduleService>();
builder.Services.AddScoped<AuditService>();
builder.Services.AddScoped<SocialImpactService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<SDGService>();

// Python AI Engine
builder.Services.AddHttpClient("AIEngine", client =>
{
    client.BaseAddress = new Uri("http://localhost:8000");
});

// CORS for React
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactDev", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseMiddleware<EduSpaceAllocator.API.Middleware.ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("AllowReactDev");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed Identity Roles and Users
using (var scope = app.Services.CreateScope())
{
    await IdentitySeeder.SeedAsync(scope.ServiceProvider);
}

app.Run();
