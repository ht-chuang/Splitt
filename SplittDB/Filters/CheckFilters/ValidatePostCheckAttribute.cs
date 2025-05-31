using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using SplittDB.DTOs.Check;
using SplittLib.Data;

namespace SplittDB.Filters.CheckFilters
{
    public class ValidatePostCheckAttribute : ValidationFilterAttribute
    {
        public ValidatePostCheckAttribute(AppDbContext context, ILogger<ValidatePostCheckAttribute> logger, ProblemDetailsFactory problemDetailsFactory)
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

            if (requestBodyObj is not PostCheckDto postCheckDto)
            {
                AddModelError(context, "", "Invalid request body format");
                SetContextResult(context, StatusCodes.Status400BadRequest);
                return false;
            }

            if (await _context.User.FindAsync(postCheckDto!.OwnerId) == null)
            {
                AddModelError(context, "OwnerId", "User not found");
                SetContextResult(context, StatusCodes.Status400BadRequest);
                return false;
            }

            context.HttpContext.Items["ValidatedPostCheckDto"] = postCheckDto;
            return true;
        }
    }
}
