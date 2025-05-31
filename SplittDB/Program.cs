using Microsoft.EntityFrameworkCore;
using SplittDB.Extensions;
using SplittLib.Data;

var builder = WebApplication.CreateBuilder(args);

// Add logging
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();  // For console output
    logging.AddDebug();    // For debug output

    // Optional: Set minimum log level
    if (builder.Environment.IsDevelopment())
    {
        logging.SetMinimumLevel(LogLevel.Debug);
    }
    else
    {
        logging.SetMinimumLevel(LogLevel.Information);
    }
});

// Add DbContext with PostgreSQL connection
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
                       ?? builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddValidationFilters();

var app = builder.Build();

// Seed data for development environment
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<AppDbContext>();
        var logger = services.GetRequiredService<ILogger<DbInitializer>>();

        var dbInitializer = new DbInitializer(
            seed: 1337,
            config: new SeedConfiguration(),
            logger: logger
        );

        await dbInitializer.SeedAsync(dbContext);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database");
        throw;
    }
}

// Routing
app.MapControllers();

// Configure the HTTP request pipeline
// app.UseHttpsRedirection(); // failed to determine https port for redirect
// app.UseRouting();
// app.UseAuthentication();
// app.UseAuthorization();

app.Run();