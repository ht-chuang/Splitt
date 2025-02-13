using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SplittLib.Data; // Assuming your DbContext is here
using SplittLib.Models; // Assuming your Check model is here

namespace SplittDB.Controllers;
[ApiController]
[Route("api/v1/[controller]")]
public class CheckController : ControllerBase
{
    private readonly AppDbContext _context;

    public CheckController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/v1/Check
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Check>>> GetChecks()
    {
        return await _context.Check.ToListAsync();
    }

    // GET: api/v1/Check/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Check>> GetCheck(int id)
    {
        var check = await _context.Check.FindAsync(id);

        if (check == null)
        {
            return NotFound();
        }

        return check;
    }

    // POST: api/v1/Check
    [HttpPost]
    public async Task<ActionResult<Check>> PostCheck([FromBody] CheckDto checkDto)
    {
        var check = new Check
        {
            Title = checkDto.Title,
            OwnerId = checkDto.OwnerId,
            Subtotal = checkDto.Subtotal,
            Tax = checkDto.Tax,
            Tip = checkDto.Tip,
            Total = checkDto.Total,
            Date = checkDto.Date
        };

        _context.Check.Add(check);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCheck), new { id = check.Id }, check);
    }

    // PUT: api/v1/Check/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCheck(int id, Check check)
    {
        if (id != check.Id)
        {
            return BadRequest();
        }

        _context.Entry(check).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Check.Any(e => e.Id == id))
            {
                return NotFound();
            }

            throw;
        }

        return NoContent();
    }

    // DELETE: api/v1/Check/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCheck(int id)
    {
        var check = await _context.Check.FindAsync(id);
        if (check == null)
        {
            return NotFound();
        }

        _context.Check.Remove(check);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
