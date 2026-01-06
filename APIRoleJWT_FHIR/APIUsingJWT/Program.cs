using APIUsingJWT.Data.DbEntities;
using APIUsingJWT.Middlewares;
using APIUsingJWT.StartUpDi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApiDbContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    var config = builder.Configuration;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = config["Jwt:Issuer"],
        ValidAudience = config["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!)),
        NameClaimType = ClaimTypes.Name,
        RoleClaimType = ClaimTypes.Role
    };
});
builder.Services.AddAuthorization();

// Add services to the container.
builder.Services.AddControllers().AddXmlSerializerFormatters();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//StartUp DI
builder.Services.AddRepositories();
builder.Services.AddServices();

//ILogger
// YOU CAN DELETE THESE:
//builder.Logging.ClearProviders(); // AddSerilog() handles this
//builder.Logging.AddConsole();    // Serilog will use its own Console sink instead
//builder.Logging.AddDebug();      // Serilog will use its own Debug sink instead

//Configuring Serilog
Log.Logger = new LoggerConfiguration().ReadFrom
    .Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

// imp -- tell the builder to use Serilog
builder.Services.AddSerilog();
//builder.Host.UseSerilog(); (traditional)

//Authurization policy
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
    {
        policy.RequireRole("Admin");
    });
    options.AddPolicy("StaffOnly", policy =>
    {
        policy.RequireRole("Staff");
    });
    options.AddPolicy("UserOnly", policy =>
    {
        policy.RequireRole("User");
    });
    options.AddPolicy("StaffsandAdmin", policy =>
    {
        policy.RequireRole("Admin", "Staff");
    });
    options.AddPolicy("Everyone", policy =>
    {
        policy.RequireRole("Admin", "Staff", "User");
    });
});

//for Rate limiting feature
builder.Services.AddRateLimiter(options =>
{
    //login
    options.AddPolicy("LoginPolicy", httpContext =>
    RateLimitPartition.GetFixedWindowLimiter
    (partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
    factory: _ => new FixedWindowRateLimiterOptions
    {
        PermitLimit = 5,
        Window = TimeSpan.FromMinutes(1),
        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
        QueueLimit = 0
    }));
    //refresh
    options.AddPolicy("RefreshPolicy", httpContext =>
    RateLimitPartition.GetFixedWindowLimiter
    (partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
    factory: _ => new FixedWindowRateLimiterOptions
    {
        PermitLimit = 5,
        Window = TimeSpan.FromMinutes(1),
        QueueLimit = 0
    }));
});

//Enable JWT Authorization in swagger - adding JWT security definition
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    //app.UseSwagger();
    //app.UseSwaggerUI();
//}
//to use swagger for production
app.UseSwagger();
app.UseSwaggerUI();

// Identify the route
// to ensure logs include the
// specific API controller/action being hit
app.UseRouting();

//Global Exception Middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

//Serilog Middleware - after auth so will know who made the req
app.UseSerilogRequestLogging();

app.MapControllers();

app.Run();
