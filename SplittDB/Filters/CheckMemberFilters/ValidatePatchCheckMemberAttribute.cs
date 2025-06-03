using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using SplittDB.DTOs.CheckMember;
using SplittLib.Data;

namespace SplittDB.Filters.CheckMemberFilters
{
    public class ValidatePatchCheckMemberAttribute : ValidationFilterAttribute
    {
        public ValidatePatchCheckMemberAttribute(AppDbContext context, ILogger<ValidatePatchCheckMemberAttribute> logger, ProblemDetailsFactory problemDetailsFactory)
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

            if (requestBodyObj is not PatchCheckMemberDto patchCheckMemberDto)
            {
                AddModelError(context, "", "Invalid request body format");
                SetContextResult(context, StatusCodes.Status400BadRequest);
                return false;
            }

            if (patchCheckMemberDto.Name == null &&
                patchCheckMemberDto.UserId == null &&
                patchCheckMemberDto.AmountOwed == null)
            {
                AddModelError(context, "requestBody", "At least one field must be provided for update");
                SetContextResult(context, StatusCodes.Status400BadRequest);
                return false;
            }

            if (patchCheckMemberDto.UserId.HasValue && await _context.User.FindAsync(patchCheckMemberDto.UserId.Value) == null)
            {
                AddModelError(context, "UserId", "User not found");
                SetContextResult(context, StatusCodes.Status400BadRequest);
                return false;
            }

            context.HttpContext.Items["ValidatedPatchCheckMemberDto"] = patchCheckMemberDto;
            return true;
        }
    }
}
