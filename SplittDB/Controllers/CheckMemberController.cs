using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SplittDB.DTOs.CheckMember;
using SplittDB.Filters.CheckFilters;
using SplittDB.Filters.CheckMemberFilters;
using SplittLib.Data;
using SplittLib.Models;

namespace SplittDB.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CheckMemberController : ControllerBase
{
    private readonly AppDbContext _context;

    public CheckMemberController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/v1/CheckMember/{id}
    [HttpGet("{id}")]
    [ServiceFilter(typeof(ValidateCheckMemberIdAttribute))]
    public async Task<ActionResult<CheckMemberInfoDto>> GetCheckMember(int id)
    {
        var checkMember = await _context.CheckMember.FindAsync(id);
        if (checkMember == null)
            return NotFound();

        var output = GetCheckMemberInfoDto(checkMember);
        return Ok(output);
    }

    // GET: api/v1/Check/{checkId}/CheckMembers
    [HttpGet]
    [Route("/api/v1/check/{id}/checkmembers")]
    [ServiceFilter(typeof(ValidateCheckIdAttribute))]
    public async Task<ActionResult<IEnumerable<CheckMemberInfoDto>>> GetCheckMembersByCheck(int id)
    {
        var check = await _context.Check
            .Include(c => c.CheckMembers)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (check == null)
            return NotFound("Check not found.");

        var checkMembers = check.CheckMembers;
        if (checkMembers == null || !checkMembers.Any())
            return NotFound("No check members found for this check.");

        var output = checkMembers.Select(GetCheckMemberInfoDto).ToList();
        return Ok(output);
    }

    // POST: api/v1/CheckMember
    [HttpPost]
    [ServiceFilter(typeof(ValidatePostCheckMemberAttribute))]
    public async Task<ActionResult<CheckMemberInfoDto>> PostCheckMember([FromBody] PostCheckMemberDto requestBody)
    {
        var postCheckMemberDto = HttpContext.Items["ValidatedPostCheckMemberDto"] as PostCheckMemberDto;

        var checkMember = new CheckMember
        {
            Name = !string.IsNullOrWhiteSpace(postCheckMemberDto!.Name) ? postCheckMemberDto.Name : "New Friend",
            CheckId = postCheckMemberDto.CheckId,
            UserId = postCheckMemberDto.UserId,
            AmountOwed = postCheckMemberDto.AmountOwed ?? 0.00m
        };

        _context.CheckMember.Add(checkMember);
        await _context.SaveChangesAsync();
        await _context.Entry(checkMember).ReloadAsync();
        var output = GetCheckMemberInfoDto(checkMember);
        return CreatedAtAction(nameof(GetCheckMember), new { id = checkMember.Id }, output);
    }

    // POST: api/v1/CheckMember/bulk
    [HttpPost("bulk")]
    [ServiceFilter(typeof(ValidatePostBulkCheckMemberAttribute))]
    public async Task<ActionResult<IEnumerable<CheckMemberInfoDto>>> PostBulkCheckMembers([FromBody] IEnumerable<PostCheckMemberDto> requestBody)
    {
        var postCheckMemberDtoList = HttpContext.Items["ValidatedPostCheckMemberDtoList"] as IEnumerable<PostCheckMemberDto>;

        var checkMembers = postCheckMemberDtoList!.Select(dto => new CheckMember
        {
            Name = !string.IsNullOrWhiteSpace(dto.Name) ? dto.Name : "New Friend",
            CheckId = dto.CheckId,
            UserId = dto.UserId,
            AmountOwed = dto.AmountOwed ?? 0.00m
        }).ToList();

        _context.CheckMember.AddRange(checkMembers);
        await _context.SaveChangesAsync();
        var outputs = checkMembers.Select(GetCheckMemberInfoDto);
        return Created("", outputs);
    }

    // PATCH: api/v1/CheckMember/{id}
    [HttpPatch("{id}")]
    [ServiceFilter(typeof(ValidatePatchCheckMemberAttribute))]
    public async Task<ActionResult<CheckMemberInfoDto>> PatchCheckMember(int id, [FromBody] PatchCheckMemberDto requestBody)
    {
        var checkMember = await _context.CheckMember.FindAsync(id);
        if (checkMember == null)
            return NotFound();

        var patchCheckMemberDto = HttpContext.Items["ValidatedPatchCheckMemberDto"] as PatchCheckMemberDto;
        if (patchCheckMemberDto!.Name != null)
            checkMember.Name = patchCheckMemberDto.Name;
        if (patchCheckMemberDto.HasProperty(nameof(patchCheckMemberDto.UserId)))
            checkMember.UserId = patchCheckMemberDto.UserId;
        if (patchCheckMemberDto.AmountOwed != null)
            checkMember.AmountOwed = (decimal)patchCheckMemberDto.AmountOwed;
        _context.Entry(checkMember).State = EntityState.Modified;

        await _context.SaveChangesAsync();
        await _context.Entry(checkMember).ReloadAsync();
        var output = GetCheckMemberInfoDto(checkMember);
        return Ok(output);
    }

    // PATCH: api/v1/CheckMember/bulk
    [HttpPatch("bulk")]
    [ServiceFilter(typeof(ValidatePatchBulkCheckMemberAttribute))]
    public async Task<ActionResult<IEnumerable<CheckMemberInfoDto>>> PatchBulkCheckMembers([FromBody] IEnumerable<PatchBulkCheckMemberDto> requestBody)
    {
        var patchBulkCheckMemberDtoList = HttpContext.Items["ValidatedPatchCheckMemberDtoList"] as IEnumerable<PatchBulkCheckMemberDto>;

        List<CheckMember> checkMembersToUpdate = new List<CheckMember>();
        foreach (var patchCheckMemberDto in patchBulkCheckMemberDtoList!)
        {
            var checkMember = await _context.CheckMember.FindAsync(patchCheckMemberDto.Id);
            if (checkMember == null)
                return NotFound($"Check member with ID {patchCheckMemberDto.Id} not found.");

            if (patchCheckMemberDto.Name != null)
                checkMember.Name = patchCheckMemberDto.Name;
            if (patchCheckMemberDto.HasProperty(nameof(patchCheckMemberDto.UserId)))
                checkMember.UserId = patchCheckMemberDto.UserId;
            if (patchCheckMemberDto.AmountOwed != null)
                checkMember.AmountOwed = (decimal)patchCheckMemberDto.AmountOwed;

            checkMembersToUpdate.Add(checkMember);
            _context.Entry(checkMember).State = EntityState.Modified;
        }

        await _context.SaveChangesAsync();
        var outputs = checkMembersToUpdate.Select(GetCheckMemberInfoDto).ToList();
        return Ok(outputs);
    }

    // DELETE: api/v1/CheckMember/{id}
    [HttpDelete("{id}")]
    [ServiceFilter(typeof(ValidateCheckMemberIdAttribute))]
    public async Task<IActionResult> DeleteCheckMember(int id)
    {
        var checkMember = await _context.CheckMember.FindAsync(id);
        if (checkMember == null)
            return NotFound();

        _context.CheckMember.Remove(checkMember);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/v1/CheckMember/bulk
    [HttpDelete("bulk")]
    [ServiceFilter(typeof(ValidateDeleteBulkCheckMemberAttribute))]
    public async Task<IActionResult> DeleteBulkCheckMembers([FromBody] IEnumerable<int> requestBody)
    {
        var checkMemberIds = HttpContext.Items["ValidatedDeleteBulkCheckMemberIds"] as IEnumerable<int>;

        var checkMembers = await _context.CheckMember
            .Where(cm => checkMemberIds!.Contains(cm.Id))
            .ToListAsync();

        _context.CheckMember.RemoveRange(checkMembers);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    public CheckMemberInfoDto GetCheckMemberInfoDto(CheckMember checkMember)
    {
        return new CheckMemberInfoDto
        {
            Id = checkMember.Id,
            Name = checkMember.Name,
            CheckId = checkMember.CheckId,
            UserId = checkMember.UserId,
            AmountOwed = checkMember.AmountOwed,
        };
    }
}
