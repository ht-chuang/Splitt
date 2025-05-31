using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SplittDB.DTOs.CheckItem;
using SplittDB.Filters.CheckFilters;
using SplittDB.Filters.CheckItemFilters;
using SplittLib.Data;
using SplittLib.Models;

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

    // GET: api/v1/CheckItem/{id}
    [HttpGet("{id}")]
    [ServiceFilter(typeof(ValidateCheckItemIdAttribute))]
    public async Task<ActionResult<CheckItem>> GetCheckItem(int id)
    {
        var checkItem = await _context.CheckItem.FindAsync(id);
        if (checkItem == null)
            return NotFound();

        var output = GetCheckItemInfoDto(checkItem);
        return Ok(output);
    }

    // GET: api/v1/Check/{checkId}/CheckItems
    [HttpGet]
    [Route("/api/v1/check/{id}/checkitems")]
    [ServiceFilter(typeof(ValidateCheckIdAttribute))]
    public async Task<IActionResult> GetCheckItemsByCheck(int id)
    {
        var check = await _context.Check
            .Include(c => c.CheckItems)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (check == null)
            return NotFound("Check not found.");

        var checkItems = check.CheckItems;
        if (checkItems == null || !checkItems.Any())
            return NotFound("No check items found for this check.");

        var output = checkItems.Select(GetCheckItemInfoDto).ToList();
        return Ok(output);
    }

    // POST: api/v1/CheckItem
    [HttpPost]
    [ServiceFilter(typeof(ValidatePostCheckItemAttribute))]
    public async Task<ActionResult<CheckItem>> PostCheckItem([FromBody] PostCheckItemDto requestBody)
    {
        var postCheckItemDto = HttpContext.Items["ValidatedPostCheckItemDto"] as PostCheckItemDto;
        var checkItem = new CheckItem
        {
            Name = !string.IsNullOrWhiteSpace(postCheckItemDto!.Name) ? postCheckItemDto.Name : "New Item",
            Description = !string.IsNullOrWhiteSpace(postCheckItemDto.Description) ? postCheckItemDto.Description : string.Empty,
            CheckId = postCheckItemDto.CheckId,
            Quantity = postCheckItemDto.Quantity ?? 1,
            UnitPrice = postCheckItemDto.UnitPrice ?? 0.00m,
            TotalPrice = (postCheckItemDto.UnitPrice ?? 0.00m) * (postCheckItemDto.Quantity ?? 1)
        };

        _context.CheckItem.Add(checkItem);
        await _context.SaveChangesAsync();
        await _context.Entry(checkItem).ReloadAsync();
        var output = GetCheckItemInfoDto(checkItem);
        return CreatedAtAction(nameof(GetCheckItem), new { id = checkItem.Id }, output);
    }

    // POST: api/v1/CheckItem/bulk
    [HttpPost("bulk")]
    [ServiceFilter(typeof(ValidatePostBulkCheckItemAttribute))]
    public async Task<ActionResult<IEnumerable<CheckItem>>> PostCheckItems([FromBody] IEnumerable<PostCheckItemDto> requestBody)
    {
        var postCheckItemDtoList = HttpContext.Items["ValidatedPostCheckItemDtoList"] as IEnumerable<PostCheckItemDto>;

        var checkItems = postCheckItemDtoList!.Select(ciDto => new CheckItem
        {
            Name = !string.IsNullOrWhiteSpace(ciDto.Name) ? ciDto.Name : "New Item",
            Description = !string.IsNullOrWhiteSpace(ciDto.Description) ? ciDto.Description : string.Empty,
            CheckId = ciDto.CheckId,
            Quantity = ciDto.Quantity ?? 1,
            UnitPrice = ciDto.UnitPrice ?? 0.00m,
            TotalPrice = (ciDto.UnitPrice ?? 0.00m) * (ciDto.Quantity ?? 1)
        }).ToList();

        _context.CheckItem.AddRange(checkItems);
        await _context.SaveChangesAsync();
        var outputs = checkItems.Select(GetCheckItemInfoDto);
        return Created("", outputs);
    }

    // PATCH: api/v1/CheckItem/{id}
    [HttpPatch("{id}")]
    [ServiceFilter(typeof(ValidateCheckItemIdAttribute))]
    [ServiceFilter(typeof(ValidatePatchCheckItemAttribute))]
    public async Task<IActionResult> PatchCheckItem(int id, [FromBody] PatchCheckItemDto requestBody)
    {
        var checkItem = await _context.CheckItem.FindAsync(id);
        if (checkItem == null)
            return NotFound();

        var patchCheckItemDto = HttpContext.Items["ValidatedPatchCheckItemDto"] as PatchCheckItemDto;
        if (patchCheckItemDto!.Name != null)
            checkItem.Name = patchCheckItemDto.Name;
        if (patchCheckItemDto.Description != null)
            checkItem.Description = patchCheckItemDto.Description;
        if (patchCheckItemDto.Quantity != null)
            checkItem.Quantity = (int)patchCheckItemDto.Quantity;
        if (patchCheckItemDto.UnitPrice != null)
            checkItem.UnitPrice = (decimal)patchCheckItemDto.UnitPrice;
        if (patchCheckItemDto.Quantity != null || patchCheckItemDto.UnitPrice != null)
            checkItem.TotalPrice = checkItem.Quantity * checkItem.UnitPrice;
        _context.Entry(checkItem).State = EntityState.Modified;

        await _context.SaveChangesAsync();
        await _context.Entry(checkItem).ReloadAsync();
        var output = GetCheckItemInfoDto(checkItem);
        return Ok(output);
    }

    // PATCH: api/v1/CheckItem/bulk
    [HttpPatch("bulk")]
    [ServiceFilter(typeof(ValidatePatchBulkCheckItemAttribute))]
    public async Task<IActionResult> PatchBulkCheckItems([FromBody] IEnumerable<PatchBulkCheckItemDto> requestBody)
    {
        var patchBulkCheckItemDtoList = HttpContext.Items["ValidatedPatchCheckItemDtoList"] as IEnumerable<PatchBulkCheckItemDto>;

        List<CheckItem> checkItemsToUpdate = new List<CheckItem>();
        foreach (var patchCheckItemDto in patchBulkCheckItemDtoList!)
        {
            var checkItem = await _context.CheckItem.FindAsync(patchCheckItemDto.Id);
            if (checkItem == null)
                return NotFound($"Check item with ID {patchCheckItemDto.Id} not found.");

            if (patchCheckItemDto.Name != null)
                checkItem.Name = patchCheckItemDto.Name;
            if (patchCheckItemDto.Description != null)
                checkItem.Description = patchCheckItemDto.Description;
            if (patchCheckItemDto.Quantity != null)
                checkItem.Quantity = (int)patchCheckItemDto.Quantity;
            if (patchCheckItemDto.UnitPrice != null)
                checkItem.UnitPrice = (decimal)patchCheckItemDto.UnitPrice;
            if (patchCheckItemDto.Quantity != null || patchCheckItemDto.UnitPrice != null)
                checkItem.TotalPrice = checkItem.Quantity * checkItem.UnitPrice;

            checkItemsToUpdate.Add(checkItem);
            _context.Entry(checkItem).State = EntityState.Modified;
        }

        await _context.SaveChangesAsync();
        var outputs = checkItemsToUpdate.Select(GetCheckItemInfoDto).ToList();
        return Ok(outputs);
    }


    // DELETE: api/v1/CheckItem/{id}
    [HttpDelete("{id}")]
    [ServiceFilter(typeof(ValidateCheckItemIdAttribute))]
    public async Task<IActionResult> DeleteCheck(int id)
    {
        var check = await _context.Check.FindAsync(id);
        if (check == null)
            return NotFound();

        _context.Check.Remove(check);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/v1/CheckItem/bulk
    [HttpDelete("bulk")]
    [ServiceFilter(typeof(ValidateDeleteBulkCheckItemAttribute))]
    public async Task<IActionResult> DeleteBulkCheckItems([FromBody] IEnumerable<int> requestBody)
    {
        var checkItemIds = HttpContext.Items["ValidatedDeleteBulkCheckItemIds"] as IEnumerable<int>;

        var checkItems = await _context.CheckItem
            .Where(ci => checkItemIds!.Contains(ci.Id))
            .ToListAsync();

        _context.CheckItem.RemoveRange(checkItems);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    public CheckItemInfoDto GetCheckItemInfoDto(CheckItem checkItem)
    {
        return new CheckItemInfoDto
        {
            Id = checkItem.Id,
            Name = checkItem.Name,
            Description = checkItem.Description,
            CheckId = checkItem.CheckId,
            Quantity = checkItem.Quantity,
            UnitPrice = checkItem.UnitPrice,
            TotalPrice = checkItem.TotalPrice
        };
    }
}
