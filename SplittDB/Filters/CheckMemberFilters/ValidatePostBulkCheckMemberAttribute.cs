using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using SplittDB.DTOs.CheckMember;
using SplittLib.Data;


namespace SplittDB.Filters.CheckMemberFilters
{
    public class ValidatePostBulkCheckMemberAttribute : ValidationFilterAttribute
    {
        public ValidatePostBulkCheckMemberAttribute(AppDbContext context, ILogger<ValidatePostBulkCheckMemberAttribute> logger, ProblemDetailsFactory problemDetailsFactory)
            : base(context, logger, problemDetailsFactory)
        {
        }

        protected override async Task<bool> ValidateAsync(ActionExecutingContext context)
        {
            if (!context.ActionArguments.TryGetValue("requestBody", out var requestBodyObj))
            {
                AddModelError(context, "", "Request body is required");
                SetContextResult(context, StatusCodes.Status400BadRequest);
                return false;
            }

            if (requestBodyObj is not IEnumerable<PostCheckMemberDto> postCheckMemberDtoList)
            {
                AddModelError(context, "", "Invalid request body format");
                SetContextResult(context, StatusCodes.Status400BadRequest);
                return false;
            }

            if (!postCheckMemberDtoList.Any())
            {
                AddModelError(context, "", "Request body cannot be empty");
                SetContextResult(context, StatusCodes.Status400BadRequest);
                return false;
            }

            var uniqueCheckIds = postCheckMemberDtoList.Select(item => (int)item.CheckId!).Distinct().ToList();
            var existingCheckIds = await _context.Check
                .Where(c => uniqueCheckIds.Contains(c.Id))
                .Select(c => c.Id)
                .ToListAsync();
            var invalidCheckIds = uniqueCheckIds.Except(existingCheckIds).ToList();
            if (invalidCheckIds.Any())
            {
                AddModelError(context, "CheckId", "Check(s) not found: " + string.Join(", ", invalidCheckIds));
                SetContextResult(context, StatusCodes.Status404NotFound);
                return false;
            }

            var uniqueUserIds = postCheckMemberDtoList
                .Where(item => item.UserId.HasValue)
                .Select(item => (int)item.UserId!)
                .Distinct()
                .ToList();
            var existingUserIds = await _context.User
                .Where(u => uniqueUserIds.Contains(u.Id))
                .Select(u => u.Id)
                .ToListAsync();
            var invalidUserIds = uniqueUserIds.Except(existingUserIds).ToList();
            if (invalidUserIds.Any())
            {
                AddModelError(context, "UserId", "User(s) not found: " + string.Join(", ", invalidUserIds));
                SetContextResult(context, StatusCodes.Status404NotFound);
                return false;
            }

            context.HttpContext.Items["ValidatedPostCheckMemberDtoList"] = postCheckMemberDtoList;
            return true;
        }
    }
}
