using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using SplittDB.DTOs.CheckMember;
using SplittLib.Data;

namespace SplittDB.Filters.CheckMemberFilters
{
    public class ValidatePostCheckMemberAttribute : ValidationFilterAttribute
    {
        public ValidatePostCheckMemberAttribute(AppDbContext context, ILogger<ValidatePostCheckMemberAttribute> logger, ProblemDetailsFactory problemDetailsFactory)
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

            if (requestBodyObj is not PostCheckMemberDto postCheckMemberDto)
            {
                AddModelError(context, "", "Invalid request body format");
                SetContextResult(context, StatusCodes.Status400BadRequest);
                return false;
            }

            if (await _context.Check.FindAsync(postCheckMemberDto.CheckId) == null)
            {
                AddModelError(context, "CheckId", "Check not found");
                SetContextResult(context, StatusCodes.Status400BadRequest);
                return false;
            }

            if (postCheckMemberDto.UserId.HasValue && await _context.User.FindAsync(postCheckMemberDto.UserId.Value) == null)
            {
                AddModelError(context, "UserId", "User not found");
                SetContextResult(context, StatusCodes.Status400BadRequest);
                return false;
            }

            context.HttpContext.Items["ValidatedPostCheckMemberDto"] = postCheckMemberDto;
            return true;
        }
    }
}
