using Xunit;
using SplittLib.Models;
using SplittLib.Data;
using SplittDB.Controllers;
// using Moq;
using Microsoft.EntityFrameworkCore;

public class CheckControllerTests
{
    private AppDbContext _context;
    private readonly CheckController _controller;

    public CheckControllerTests()
    {

        var options = new DbContextOptionsBuilder<AppDbContext>().UseNpgsql("Host=localhost;Database=splitt;Username=aaronl;Password=password");
        _context = new AppDbContext(options.Options);

        // _context = new AppDbContext(
        //     options);
        _controller = new CheckController(_context);
    }

    // Seed data before each test
    private void SeedDatabase()
    {
        User user1 = new User { Name = "Ron", Username = "Ron", UsernameTag = "Ronn", Email = "ron@gmail.com", Phone = "123-456-7890" };
        User user2 = new User { Name = "Tom", Username = "Rash", UsernameTag = "Tomm", Email = "tom@gmail.com", Phone = "111-222-3333" };
        User user3 = new User { Name = "Bon", Username = "Maple", UsernameTag = "Tale", Email = "bon@gmail.com", Phone = "333-222-1111" };

        _context.User.AddRange(
            user1,
            user2,
            user3
        );
        DateTime currentDateTimeWithTimeZone = DateTime.UtcNow;
        Check check1 = new Check { Title = "Check 1", Owner = user1, Date = currentDateTimeWithTimeZone, Subtotal = 100, Tax = 9.38m, Tip = 0, Total = 109.38m };
        Check check2 = new Check { Title = "Check 2", Owner = user2, Date = currentDateTimeWithTimeZone, Subtotal = 50, Tax = 4.69m, Tip = 5, Total = 59.69m };
        Check check3 = new Check { Title = "Check 3", Owner = user3, Date = currentDateTimeWithTimeZone, Subtotal = 613, Tax = 57.50m, Tip = 30, Total = 700.50m };


        _context.Check.AddRange(
            check1,
            check2,
            check3
        );
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetChecks_ReturnsAllChecks()
    {
        // Arrange
        var controller = new CheckController(_context);

        SeedDatabase();

        // Act
        var result = await controller.GetChecks();

        // Assert
        Assert.NotNull(result.Value);
        Assert.Equal(3, result.Value.Count());
    }
}
