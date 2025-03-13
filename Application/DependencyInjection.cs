using Application.Common;
using Application.Common.Behaviors;
using Application.Users.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssemblyContaining<ApplicationAssemblyReference>();
            });

            services.AddScoped(
                typeof(IPipelineBehavior<,>),
                typeof(ValidationBehavior<,>));

            services.AddValidatorsFromAssemblyContaining<ApplicationAssemblyReference>();
            services.AddScoped<IDomainEventPublisher, DomainEventPublisher>();
            services.AddScoped<UserService>();

            return services;
        }
    }
}
