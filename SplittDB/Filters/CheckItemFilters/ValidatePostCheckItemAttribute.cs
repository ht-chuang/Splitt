using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using SplittDB.DTOs.CheckItem;
using SplittLib.Data;

namespace SplittDB.Filters.CheckFilters
{
    public class ValidatePostCheckItemAttribute : ValidationFilterAttribute
    {
        public ValidatePostCheckItemAttribute(AppDbContext context, ILogger<ValidatePostCheckItemAttribute> logger, ProblemDetailsFactory problemDetailsFactory)
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

            if (requestBodyObj is not PostCheckItemDto postCheckItemDto)
            {
                AddModelError(context, "", "Invalid request body format");
                SetContextResult(context, StatusCodes.Status400BadRequest);
                return false;
            }

            if (await _context.Check.FindAsync(postCheckItemDto.CheckId) == null)
            {
                AddModelError(context, "CheckId", "Check not found");
                SetContextResult(context, StatusCodes.Status400BadRequest);
                return false;
            }

            context.HttpContext.Items["ValidatedPostCheckItemDto"] = postCheckItemDto;
            return true;
        }
    }
}
