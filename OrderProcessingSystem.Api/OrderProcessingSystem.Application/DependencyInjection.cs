using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OrderProcessingSystem.Application.Common.Behaviors;
using OrderProcessingSystem.Application.Common.Interfaces;
using System.Reflection;

namespace OrderProcessingSystem.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            services.AddScoped<IDateTime, DateTimeService>();
            return services;
        }
    }
}
