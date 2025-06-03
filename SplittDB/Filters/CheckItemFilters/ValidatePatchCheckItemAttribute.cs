using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using SplittDB.DTOs.CheckItem;
using SplittLib.Data;

namespace SplittDB.Filters.CheckItemFilters
{
    public class ValidatePatchCheckItemAttribute : ValidationFilterAttribute
    {
        public ValidatePatchCheckItemAttribute(AppDbContext context, ILogger<ValidatePatchCheckItemAttribute> logger, ProblemDetailsFactory problemDetailsFactory)
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

            if (requestBodyObj is not PatchCheckItemDto patchCheckItemDto)
            {
                AddModelError(context, "", "Invalid request body format");
                SetContextResult(context, StatusCodes.Status400BadRequest);
                return false;
            }

            if (patchCheckItemDto.Name == null &&
                patchCheckItemDto.Description == null &&
                patchCheckItemDto.Quantity == null &&
                patchCheckItemDto.UnitPrice == null)
            {
                AddModelError(context, "requestBody", "At least one field must be provided for update");
                SetContextResult(context, StatusCodes.Status400BadRequest);
                return false;
            }

            context.HttpContext.Items["ValidatedPatchCheckItemDto"] = patchCheckItemDto;
            return true;
        }
    }
}
