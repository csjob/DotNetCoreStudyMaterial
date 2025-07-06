using DotNetCoreWebAPI.DI;
using DotNetCoreWebAPI.Middleware;
using DotNetCoreWebAPI.Model.Db;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.
//builder.Services.AddControllers();

builder.Services.AddControllers()
    .AddFluentValidation(fv =>
        fv.RegisterValidatorsFromAssemblyContaining<Program>());

builder.Services.Configure<ApiBehaviorOptions>(options =>
options.InvalidModelStateResponseFactory = context =>
{
    var errors = context.ModelState
    .Where(x => x.Value.Errors.Count > 0)
    .ToDictionary(
        e => e.Key,
        e => e.Value.Errors.Select(x => x.ErrorMessage).ToArray()
        );

    return new BadRequestObjectResult(new
    {
        Message = "Validation failed",
        Errors = errors
    });

});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Log.Logger = new LoggerConfiguration()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Dependency Injection
builder.Services.AddScoped<IMessageService, MessageService>();

// JWT Authentication Configuration
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config["Jwt:Issuer"],
            ValidAudience = config["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(config["Jwt:Key"]))
        };
    });

// if we want to show Authorize in Swagger UI. then add this code.
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Your API", Version = "v1" });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token.\n\nExample: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6..."
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

//builder.Services.AddDbContext<AppDbContext>(options =>
//options.UseInMemoryDatabase("TestDb"));

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MustBeManager", policy =>
        policy.RequireClaim("Department", "Management"));
});

// EF Core + MySQL Configuration
var connectionString = builder.Configuration.GetConnectionString("MySqlConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddAutoMapper(typeof(Program));// or typeof(AutoMapperProfile)

builder.Services.AddMemoryCache(); // Add this first
//builder.Services.AddSession();     // Then session depends on it

builder.Services.AddResponseCaching();

var app = builder.Build();


// Ensure HTTPS redirection occurs early
app.UseHttpsRedirection();

// Custom request logging middleware
app.Use(async (context, next) =>
{
    Console.WriteLine("Request Incoming: " + context.Request.Path);
    await next();
    Console.WriteLine("Response Outgoing");
});

// Custom middleware
app.UseMiddleware<MyCustomMiddleware>();

app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline for development mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Authentication & Authorization order matters!
app.UseAuthentication();
//app.UseSession();
app.UseAuthorization();

// Map controllers for handling requests
app.MapControllers();

app.UseResponseCaching();

app.Run();

