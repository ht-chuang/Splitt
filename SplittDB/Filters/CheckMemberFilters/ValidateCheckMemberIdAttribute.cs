using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using SplittLib.Data;

namespace SplittDB.Filters.CheckMemberFilters
{
    public class ValidateCheckMemberIdAttribute : ValidationFilterAttribute
    {
        public ValidateCheckMemberIdAttribute(AppDbContext context, ILogger<ValidateCheckMemberIdAttribute> logger, ProblemDetailsFactory problemDetailsFactory)
            : base(context, logger, problemDetailsFactory)
        {
        }

        protected override bool Validate(ActionExecutingContext context)
        {
            if (!context.ActionArguments.TryGetValue("id", out var idObj))
            {
                AddModelError(context, "Id", "Check Member ID is required");
                SetContextResult(context, StatusCodes.Status400BadRequest);
                return false;
            }

            if (idObj is not int id)
            {
                AddModelError(context, "Id", "Invalid Check Member ID format");
                SetContextResult(context, StatusCodes.Status400BadRequest);
                return false;
            }

            if (id <= 0)
            {
                AddModelError(context, "Id", "Invalid Check Member ID value");
                SetContextResult(context, StatusCodes.Status400BadRequest);
                return false;
            }

            return true;
        }
    }
}