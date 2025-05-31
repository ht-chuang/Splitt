using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using SplittLib.Data;

namespace SplittDB.Filters.UserFilters
{
    public class ValidateUserIdAttribute : ValidationFilterAttribute
    {
        public ValidateUserIdAttribute(AppDbContext context, ILogger<ValidateUserIdAttribute> logger, ProblemDetailsFactory problemDetailsFactory)
            : base(context, logger, problemDetailsFactory)
        {
        }

        protected override bool Validate(ActionExecutingContext context)
        {
            if (!context.ActionArguments.TryGetValue("id", out var idObj))
            {
                AddModelError(context, "Id", "User ID is required");
                SetContextResult(context, StatusCodes.Status400BadRequest);
                return false;
            }

            if (idObj is not int id)
            {
                AddModelError(context, "Id", "Invalid User ID format");
                SetContextResult(context, StatusCodes.Status400BadRequest);
                return false;
            }

            if (id <= 0)
            {
                AddModelError(context, "Id", "Invalid User ID value");
                SetContextResult(context, StatusCodes.Status400BadRequest);
                return false;
            }

            return true;
        }

    }
}