using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using SplittDB.DTOs.CheckItem;
using SplittLib.Data;


namespace SplittDB.Filters.CheckItemFilters
{
    public class ValidatePostBulkCheckItemAttribute : ValidationFilterAttribute
    {
        public ValidatePostBulkCheckItemAttribute(AppDbContext context, ILogger<ValidatePostBulkCheckItemAttribute> logger, ProblemDetailsFactory problemDetailsFactory)
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

            if (requestBodyObj is not IEnumerable<PostCheckItemDto> postCheckItemDtoList)
            {
                AddModelError(context, "", "Invalid request body format");
                SetContextResult(context, StatusCodes.Status400BadRequest);
                return false;
            }

            if (!postCheckItemDtoList.Any())
            {
                AddModelError(context, "", "Request body cannot be empty");
                SetContextResult(context, StatusCodes.Status400BadRequest);
                return false;
            }

            var uniqueCheckIds = postCheckItemDtoList.Select(item => (int)item.CheckId!).Distinct().ToList();
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

            context.HttpContext.Items["ValidatedPostCheckItemDtoList"] = postCheckItemDtoList;
            return true;
        }
    }
}
