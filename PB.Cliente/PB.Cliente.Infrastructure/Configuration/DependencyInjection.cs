using PB.Cliente.Domain.Interfaces;
using PB.Cliente.Infrastructure.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace PB.Cliente.Infrastructure.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IClienteRepository, ClienteRepository>();
            
            return services;

        }
    }
}