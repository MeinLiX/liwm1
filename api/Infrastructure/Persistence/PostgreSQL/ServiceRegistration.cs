using Microsoft.Extensions.DependencyInjection;

namespace PostgreSQL;

public static class ServiceRegistration
{
    public static IServiceCollection AddPersistencePostgreSQLInfrastructureLayer(this IServiceCollection services)
    => services; // TODO
}

