using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderProcessingSystem.Application.Common.Interfaces;
using OrderProcessingSystem.Infrastructure.Persistence;
using OrderProcessingSystem.Infrastructure.Repositories;
using OrderProcessingSystem.Infrastructure.Repositories.Implementations;
using OrderProcessingSystem.Infrastructure.UnitOfWork;
using UnitOfWorkObject = OrderProcessingSystem.Infrastructure.UnitOfWork.UnitOfWork;

namespace OrderProcessingSystem.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection"),
                    sql => sql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWorkObject>();

            return services;
        }
    }
}
