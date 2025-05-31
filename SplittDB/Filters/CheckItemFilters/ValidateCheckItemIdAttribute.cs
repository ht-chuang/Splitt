using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using SplittLib.Data;

namespace SplittDB.Filters.CheckItemFilters
{
    public class ValidateCheckItemIdAttribute : ValidationFilterAttribute
    {
        public ValidateCheckItemIdAttribute(AppDbContext context, ILogger<ValidateCheckItemIdAttribute> logger, ProblemDetailsFactory problemDetailsFactory)
            : base(context, logger, problemDetailsFactory)
        {
        }

        protected override bool Validate(ActionExecutingContext context)
        {
            if (!context.ActionArguments.TryGetValue("id", out var idObj))
            {
                AddModelError(context, "Id", "Check Item ID is required");
                SetContextResult(context, StatusCodes.Status400BadRequest);
                return false;
            }

            if (idObj is not int id)
            {
                AddModelError(context, "Id", "Invalid Check Item ID format");
                SetContextResult(context, StatusCodes.Status400BadRequest);
                return false;
            }

            if (id <= 0)
            {
                AddModelError(context, "Id", "Invalid Check Item ID value");
                SetContextResult(context, StatusCodes.Status400BadRequest);
                return false;
            }

            return true;
        }
    }
}