using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SplittLib.Data; // Assuming your DbContext is here
using SplittLib.Models; // Assuming your CheckItem model is here

namespace SplittDB.Controllers;
[ApiController]
[Route("api/v1/[controller]")]
public class CheckItemController : ControllerBase
{
    private readonly AppDbContext _context;

    public CheckItemController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/v1/CheckItem
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CheckItem>>> GetCheckItems()
    {
        return await _context.CheckItem.ToListAsync();
    }

    // GET: api/v1/CheckItem/5
    [HttpGet("{id}")]
    public async Task<ActionResult<CheckItem>> GetCheckItem(int id)
    {
        var checkItem = await _context.CheckItem.FindAsync(id);

        if (checkItem == null)
        {
            return NotFound();
        }

        return checkItem;
    }

    // POST: api/v1/CheckItem
    [HttpPost]
    public async Task<ActionResult<CheckItem>> PostCheck(CheckItem checkItem)
    {
        _context.CheckItem.Add(checkItem);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCheckItem), new { id = checkItem.Id }, checkItem);
    }

    // PUT: api/v1/CheckItem/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCheckItem(int id, CheckItem checkItem)
    {
        if (id != checkItem.Id)
        {
            return BadRequest();
        }

        _context.Entry(checkItem).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.CheckItem.Any(e => e.Id == id))
            {
                return NotFound();
            }

            throw;
        }

        return NoContent();
    }

    // DELETE: api/v1/CheckItem/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCheck(int id)
    {
        var checkItem = await _context.CheckItem.FindAsync(id);
        if (checkItem == null)
        {
            return NotFound();
        }

        _context.CheckItem.Remove(checkItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
