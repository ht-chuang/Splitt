using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SplittDB.DTOs.Check;
using SplittDB.Filters.CheckFilters;
using SplittLib.Data;
using SplittLib.Models;

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

    // GET: api/v1/Check/{id}
    [HttpGet("{id}")]
    [ServiceFilter(typeof(ValidateCheckIdAttribute))]
    public async Task<ActionResult<Check>> GetCheck(int id)
    {
        var check = await _context.Check.FindAsync(id);
        if (check == null)
            return NotFound();

        var output = GetCheckInfoDto(check);
        return Ok(output);
    }

    // POST: api/v1/Check
    [HttpPost]
    [ServiceFilter(typeof(ValidatePostCheckAttribute))]
    public async Task<ActionResult<Check>> PostCheck([FromBody] PostCheckDto requestBody)
    {
        var postCheckDto = HttpContext.Items["ValidatedPostCheckDto"] as PostCheckDto;
        var check = new Check
        {
            OwnerId = postCheckDto!.OwnerId,
            Title = !string.IsNullOrWhiteSpace(postCheckDto.Title) ? postCheckDto.Title : "New Check",
            Date = postCheckDto.Date ?? DateTime.UtcNow
        };

        _context.Check.Add(check);
        await _context.SaveChangesAsync();

        var output = GetCheckInfoDto(check);
        return CreatedAtAction(nameof(GetCheck), new { id = check.Id }, output);
    }

    // PUT: api/v1/Check/{id}
    [HttpPatch("{id}")]
    [ServiceFilter(typeof(ValidateCheckIdAttribute))]
    [ServiceFilter(typeof(ValidatePatchCheckAttribute))]
    public async Task<IActionResult> PatchCheck(int id, [FromBody] PatchCheckDto requestBody)
    {
        var check = await _context.Check.FindAsync(id);
        if (check == null)
            return NotFound();

        var patchCheckDto = HttpContext.Items["ValidatedPatchCheckDto"] as PatchCheckDto;
        if (patchCheckDto!.Title != null)
            check!.Title = patchCheckDto.Title;
        if (patchCheckDto.Date != null)
            check.Date = (DateTime)patchCheckDto.Date;
        if (patchCheckDto.Subtotal != null)
            check.Subtotal = (decimal)patchCheckDto.Subtotal;
        if (patchCheckDto.Tax != null)
            check.Tax = (decimal)patchCheckDto.Tax;
        if (patchCheckDto.Tip != null)
            check.Tip = (decimal)patchCheckDto.Tip;
        if (patchCheckDto.Total != null)
            check.Total = (decimal)patchCheckDto.Total;
        _context.Entry(check).State = EntityState.Modified;

        await _context.SaveChangesAsync();
        await _context.Entry(check).ReloadAsync();
        var output = GetCheckInfoDto(check);
        return Ok(output);
    }

    // DELETE: api/v1/Check/{id}
    [HttpDelete("{id}")]
    [ServiceFilter(typeof(ValidateCheckIdAttribute))]
    public async Task<IActionResult> DeleteCheck(int id)
    {
        var check = await _context.Check.FindAsync(id);
        if (check == null)
            return NotFound();

        _context.Check.Remove(check);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    public CheckInfoDto GetCheckInfoDto(Check check)
    {
        return new CheckInfoDto
        {
            Id = check.Id,
            Title = check.Title,
            OwnerId = check.OwnerId,
            Date = check.Date,
            Subtotal = check.Subtotal,
            Tax = check.Tax,
            Tip = check.Tip,
            Total = check.Total
        };
    }
}
