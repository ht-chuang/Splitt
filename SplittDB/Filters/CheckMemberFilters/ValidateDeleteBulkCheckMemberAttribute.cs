using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using SplittLib.Data;

namespace SplittDB.Filters.CheckMemberFilters
{
    public class ValidateDeleteBulkCheckMemberAttribute : ValidationFilterAttribute
    {
        public ValidateDeleteBulkCheckMemberAttribute(AppDbContext context, ILogger<ValidateDeleteBulkCheckMemberAttribute> logger, ProblemDetailsFactory problemDetailsFactory)
            : base(context, logger, problemDetailsFactory)
        {
        }

        protected override bool Validate(ActionExecutingContext context)
        {
            if (!context.ActionArguments.TryGetValue("requestBody", out var requestBodyObj))
            {
                AddModelError(context, "", "Request body is required");
                SetContextResult(context, StatusCodes.Status400BadRequest);
                return false;
            }

            if (requestBodyObj is not IEnumerable<int> idList)
            {
                AddModelError(context, "", "Invalid request body format");
                SetContextResult(context, StatusCodes.Status400BadRequest);
                return false;
            }

            if (!idList.Any())
            {
                AddModelError(context, "", "Request body cannot be empty");
                SetContextResult(context, StatusCodes.Status400BadRequest);
                return false;
            }

            var invalidIds = idList.Where(id => id <= 0).ToList();
            if (invalidIds.Any())
            {
                AddModelError(context, "Id", "Invalid check member ID(s): " + string.Join(", ", invalidIds));
                SetContextResult(context, StatusCodes.Status400BadRequest);
                return false;
            }

            var uniqueIds = idList.Distinct().ToList();
            if (uniqueIds.Count != idList.Count())
            {
                AddModelError(context, "Id", "Duplicate check member IDs found");
                SetContextResult(context, StatusCodes.Status400BadRequest);
                return false;
            }

            var existingCheckMemberIds = _context.CheckMember
                .Where(cm => uniqueIds.Contains(cm.Id))
                .Select(cm => cm.Id)
                .ToList();
            var invalidCheckMemberIds = uniqueIds.Except(existingCheckMemberIds).ToList();
            if (invalidCheckMemberIds.Any())
            {
                AddModelError(context, "Id", "Check member(s) not found: " + string.Join(", ", invalidCheckMemberIds));
                SetContextResult(context, StatusCodes.Status404NotFound);
                return false;
            }

            context.HttpContext.Items["ValidatedDeleteBulkCheckMemberIds"] = idList;
            return true;
        }
    }
}