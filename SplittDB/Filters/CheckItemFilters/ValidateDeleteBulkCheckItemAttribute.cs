using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using SplittLib.Data;

namespace SplittDB.Filters.CheckItemFilters
{
    public class ValidateDeleteBulkCheckItemAttribute : ValidationFilterAttribute
    {
        public ValidateDeleteBulkCheckItemAttribute(AppDbContext context, ILogger<ValidateDeleteBulkCheckItemAttribute> logger, ProblemDetailsFactory problemDetailsFactory)
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
                AddModelError(context, "Id", "Invalid Check Item ID(s): " + string.Join(", ", invalidIds));
                SetContextResult(context, StatusCodes.Status400BadRequest);
                return false;
            }

            var uniqueIds = idList.Distinct().ToList();
            if (uniqueIds.Count != idList.Count())
            {
                AddModelError(context, "Id", "Duplicate check item IDs found");
                SetContextResult(context, StatusCodes.Status400BadRequest);
                return false;
            }

            var existingCheckItemIds = _context.CheckItem
                .Where(ci => uniqueIds.Contains(ci.Id))
                .Select(ci => ci.Id)
                .ToList();
            var invalidCheckItemIds = uniqueIds.Except(existingCheckItemIds).ToList();
            if (invalidCheckItemIds.Any())
            {
                AddModelError(context, "Id", "Check item(s) not found: " + string.Join(", ", invalidCheckItemIds));
                SetContextResult(context, StatusCodes.Status404NotFound);
                return false;
            }

            context.HttpContext.Items["ValidatedDeleteBulkCheckItemIds"] = idList;
            return true;
        }
    }
}