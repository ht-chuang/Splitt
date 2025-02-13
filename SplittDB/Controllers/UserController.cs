using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SplittLib.Data; // Assuming your DbContext is here
using SplittLib.Models; // Assuming your Check model is here

namespace SplittDB.Controllers;
[ApiController]
[Route("api/v1/[controller]")]
public class UserController : ControllerBase
{
    private readonly AppDbContext _context;

    public UserController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/v1/User
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        return await _context.User.ToListAsync();
    }

    // GET: api/v1/User/5
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        var user = await _context.User.FindAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return user;
    }
}
