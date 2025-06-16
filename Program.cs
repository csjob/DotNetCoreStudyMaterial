using DotNetCoreWebAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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

// Configure the HTTP request pipeline for development mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Authentication should come before authorization (if applicable)
// app.UseAuthentication(); // Uncomment if needed
app.UseAuthorization();

// Map controllers for handling requests
app.MapControllers();

app.Run();

