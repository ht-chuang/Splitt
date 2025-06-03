using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using SplittDB.DTOs.CheckMember;
using SplittLib.Data;

namespace SplittDB.Filters.CheckMemberFilters
{
    public class ValidatePatchBulkCheckMemberAttribute : ValidationFilterAttribute
    {
        public ValidatePatchBulkCheckMemberAttribute(AppDbContext context, ILogger<ValidatePatchBulkCheckMemberAttribute> logger, ProblemDetailsFactory problemDetailsFactory)
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

            if (requestBodyObj is not IEnumerable<PatchBulkCheckMemberDto> patchBulkCheckMemberDtoList)
            {
                AddModelError(context, "", "Invalid request body format");
                SetContextResult(context, StatusCodes.Status400BadRequest);
                return false;
            }

            if (!patchBulkCheckMemberDtoList.Any())
            {
                AddModelError(context, "", "Request body cannot be empty");
                SetContextResult(context, StatusCodes.Status400BadRequest);
                return false;
            }

            var uniqueIds = patchBulkCheckMemberDtoList.Select(item => (int)item.Id!).Distinct().ToList();
            if (uniqueIds.Count != patchBulkCheckMemberDtoList.Count())
            {
                AddModelError(context, "Id", "Duplicate check member IDs found");
                SetContextResult(context, StatusCodes.Status400BadRequest);
                return false;
            }

            var existingCheckMemberIds = _context.CheckMember
                .Where(ci => uniqueIds.Contains(ci.Id))
                .Select(ci => ci.Id)
                .ToList();
            var invalidCheckMemberIds = uniqueIds.Except(existingCheckMemberIds).ToList();
            if (invalidCheckMemberIds.Any())
            {
                AddModelError(context, "Id", "Check member(s) not found: " + string.Join(", ", invalidCheckMemberIds));
                SetContextResult(context, StatusCodes.Status404NotFound);
                return false;
            }

            for (int i = 0; i < patchBulkCheckMemberDtoList.Count(); i++)
            {
                var patchBulkCheckMemberDto = patchBulkCheckMemberDtoList.ElementAt(i);
                if (patchBulkCheckMemberDto.Name == null &&
                    patchBulkCheckMemberDto.UserId == null &&
                    patchBulkCheckMemberDto.AmountOwed == null)
                {
                    AddModelError(context, $"requestBody[{i}]", "At least one field must be provided for update");
                    SetContextResult(context, StatusCodes.Status400BadRequest);
                    return false;
                }
            }

            var userIds = patchBulkCheckMemberDtoList
                .Where(dto => dto.UserId.HasValue)
                .Select(dto => dto.UserId!.Value)
                .Distinct()
                .ToList();
            if (userIds.Any())
            {
                var existingUserIds = await _context.User
                    .Where(u => userIds.Contains(u.Id))
                    .Select(u => u.Id)
                    .ToListAsync();
                var invalidUserIds = userIds.Except(existingUserIds).ToList();
                if (invalidUserIds.Any())
                {
                    AddModelError(context, "UserId", "User(s) not found: " + string.Join(", ", invalidUserIds));
                    SetContextResult(context, StatusCodes.Status404NotFound);
                    return false;
                }
            }

            context.HttpContext.Items["ValidatedPatchCheckMemberDtoList"] = patchBulkCheckMemberDtoList;
            return true;
        }
    }
}
