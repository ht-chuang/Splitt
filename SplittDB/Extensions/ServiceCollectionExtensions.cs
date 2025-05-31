using System.Reflection;
using SplittDB.Filters;

namespace SplittDB.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddValidationFilters(this IServiceCollection services)
        {
            var filterTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t =>
                    !t.IsAbstract &&
                    t.IsSubclassOf(typeof(ValidationFilterAttribute)));

            foreach (var filterType in filterTypes)
            {
                services.AddScoped(filterType);
            }

            return services;
        }
    }
}