using Microsoft.Extensions.DependencyInjection;
using QuickTest.Infrastructure.Services;

namespace QuickTest.Infrastructure;

public static class DependencyInjection {
    public static IServiceCollection AddInfrastructure(this IServiceCollection services) {
        services.AddTransient<WaferDataService>();
        return services;
    }
}