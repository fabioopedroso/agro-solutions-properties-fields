using Application.Contracts;
using Core.Interfaces.Repositories;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repository;
using Infrastructure.Services.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("ConnectionString")));

        // Repositories
        services.AddScoped<IPropertyRepository, PropertyRepository>();
        services.AddScoped<IFieldRepository, FieldRepository>();

        // Services
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}
