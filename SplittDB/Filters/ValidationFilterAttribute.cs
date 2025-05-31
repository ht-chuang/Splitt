using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using SplittLib.Data;

namespace SplittDB.Filters
{
    public abstract class ValidationFilterAttribute : ActionFilterAttribute
    {
        protected readonly AppDbContext _context;
        protected readonly ILogger _logger;
        protected readonly ProblemDetailsFactory _problemDetailsFactory;

        protected ValidationFilterAttribute(AppDbContext context, ILogger logger, ProblemDetailsFactory problemDetailsFactory)
        {
            _context = context;
            _logger = logger;
            _problemDetailsFactory = problemDetailsFactory;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                SetContextResult(context, StatusCodes.Status400BadRequest);
                return;
            }

            if (HasSyncValidation())
            {
                if (!Validate(context))
                {
                    return;
                }
            }

            if (HasAsyncValidation())
            {
                if (!await ValidateAsync(context))
                {
                    return;
                }
            }

            await next();
        }

        // Virtual methods with default implementations (so they're optional)
        protected virtual bool Validate(ActionExecutingContext context)
        {
            return true;
        }

        protected virtual Task<bool> ValidateAsync(ActionExecutingContext context)
        {
            return Task.FromResult(true);
        }

        // Helper methods to check if validation methods are overridden
        private bool HasSyncValidation()
        {
            var method = GetType().GetMethod(nameof(Validate),
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            return method?.DeclaringType != typeof(ValidationFilterAttribute);
        }

        private bool HasAsyncValidation()
        {
            var method = GetType().GetMethod(nameof(ValidateAsync),
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            return method?.DeclaringType != typeof(ValidationFilterAttribute);
        }

        protected void AddModelError(ActionExecutingContext context, string key, string message)
        {
            context.ModelState.AddModelError(key, message);
            _logger.LogWarning("Validation error: {Key} - {Message}", key, message);
        }

        protected void SetContextResult(
            ActionExecutingContext context,
            int statusCode,
            string? title = null,
            string? type = null,
            string? detail = null)
        {
            var problemDetails = _problemDetailsFactory.CreateValidationProblemDetails(
                httpContext: context.HttpContext,
                modelStateDictionary: context.ModelState,
                statusCode: statusCode,
                title: title,
                type: type,
                detail: detail,
                instance: context.HttpContext.Request.Path);

            context.Result = new ObjectResult(problemDetails)
            {
                StatusCode = problemDetails.Status,
            };
        }
    }
}