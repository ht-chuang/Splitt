using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using SplittDB.DTOs.Check;
using SplittLib.Data;

namespace SplittDB.Filters.CheckFilters
{
    public class ValidatePatchCheckAttribute : ValidationFilterAttribute
    {
        public ValidatePatchCheckAttribute(AppDbContext context, ILogger<ValidatePatchCheckAttribute> logger, ProblemDetailsFactory problemDetailsFactory)
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

            if (requestBodyObj is not PatchCheckDto patchCheckDto)
            {
                AddModelError(context, "", "Invalid request body format");
                SetContextResult(context, StatusCodes.Status400BadRequest);
                return false;
            }

            if (patchCheckDto.Title == null &&
                patchCheckDto.Date == null &&
                patchCheckDto.Subtotal == null &&
                patchCheckDto.Tax == null &&
                patchCheckDto.Tip == null &&
                patchCheckDto.Total == null)
            {
                AddModelError(context, "requestBody", "At least one field must be provided for update");
                SetContextResult(context, StatusCodes.Status400BadRequest);
                return false;
            }

            context.HttpContext.Items["ValidatedPatchCheckDto"] = patchCheckDto;
            return true;
        }
    }
}
