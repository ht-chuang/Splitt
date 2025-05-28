using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using SplittLib.Data;

namespace SplittDB.Filters.CheckFilters
{
    public class ValidateCheckIdAttribute : ValidationFilterAttribute
    {
        public ValidateCheckIdAttribute(AppDbContext context, ILogger<ValidateCheckIdAttribute> logger, ProblemDetailsFactory problemDetailsFactory)
            : base(context, logger, problemDetailsFactory)
        {
        }

        protected override bool Validate(ActionExecutingContext context)
        {
            if (!context.ActionArguments.TryGetValue("id", out var idObj))
            {
                AddModelError(context, "Id", "Check ID is required");
                SetContextResult(context, StatusCodes.Status400BadRequest);
                return false;
            }

            if (idObj is not int id)
            {
                AddModelError(context, "Id", "Invalid Check ID format");
                SetContextResult(context, StatusCodes.Status400BadRequest);
                return false;
            }

            if (id <= 0)
            {
                AddModelError(context, "Id", "Invalid Check ID value");
                SetContextResult(context, StatusCodes.Status400BadRequest);
                return false;
            }

            return true;
        }
    }
}