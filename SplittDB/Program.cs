using Microsoft.EntityFrameworkCore;
using SplittLib.Data;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext with PostgreSQL connection
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
                       ?? builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services.AddControllers();
var app = builder.Build();
app.MapControllers();
app.Run();