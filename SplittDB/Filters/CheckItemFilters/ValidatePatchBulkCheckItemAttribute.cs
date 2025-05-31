using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using SplittDB.DTOs.CheckItem;
using SplittLib.Data;

namespace SplittDB.Filters.CheckFilters
{
    public class ValidatePatchBulkCheckItemAttribute : ValidationFilterAttribute
    {
        public ValidatePatchBulkCheckItemAttribute(AppDbContext context, ILogger<ValidatePatchBulkCheckItemAttribute> logger, ProblemDetailsFactory problemDetailsFactory)
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

            if (requestBodyObj is not IEnumerable<PatchBulkCheckItemDto> patchBulkCheckItemDtoList)
            {
                AddModelError(context, "", "Invalid request body format");
                SetContextResult(context, StatusCodes.Status400BadRequest);
                return false;
            }

            if (!patchBulkCheckItemDtoList.Any())
            {
                AddModelError(context, "", "Request body cannot be empty");
                SetContextResult(context, StatusCodes.Status400BadRequest);
                return false;
            }

            var uniqueIds = patchBulkCheckItemDtoList.Select(item => (int)item.Id!).Distinct().ToList();
            if (uniqueIds.Count != patchBulkCheckItemDtoList.Count())
            {
                AddModelError(context, "Id", "Duplicate check item IDs found");
                SetContextResult(context, StatusCodes.Status400BadRequest);
                return false;
            }

            var existingCheckItemIds = _context.CheckItem
                .Where(ci => uniqueIds.Contains(ci.Id))
                .Select(ci => ci.Id)
                .ToList();
            var invalidCheckItemIds = uniqueIds.Except(existingCheckItemIds).ToList();
            if (invalidCheckItemIds.Any())
            {
                AddModelError(context, "Id", "Check item(s) not found: " + string.Join(", ", invalidCheckItemIds));
                SetContextResult(context, StatusCodes.Status404NotFound);
                return false;
            }

            for (int i = 0; i < patchBulkCheckItemDtoList.Count(); i++)
            {
                var patchBulkCheckItemDto = patchBulkCheckItemDtoList.ElementAt(i);
                if (patchBulkCheckItemDto.Name == null &&
                    patchBulkCheckItemDto.Description == null &&
                    patchBulkCheckItemDto.Quantity == null &&
                    patchBulkCheckItemDto.UnitPrice == null)
                {
                    AddModelError(context, $"requestBody[{i}]", "At least one field must be provided for update");
                    SetContextResult(context, StatusCodes.Status400BadRequest);
                    return false;
                }
            }

            context.HttpContext.Items["ValidatedPatchCheckItemDtoList"] = patchBulkCheckItemDtoList;
            return true;
        }
    }
}
