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

// Seed data for development environment
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var dbInitializer = new DbInitializer(1337);
        dbInitializer.Seed(dbContext);
    }
}

app.MapControllers();
app.Run();