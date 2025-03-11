
using Kelist.API.Middlewares;

namespace Kelist.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddOpenApi();
            services.AddTransient<GlobalExceptionHandlingMiddleware>();
            return services;
        }
    }
}
