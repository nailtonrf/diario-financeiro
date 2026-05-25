namespace Fluxo.Saldos.Service.Infra;

using Abstractions.Messaging;
using Shell.Backgrounds;
using Shell.Stores;
using Rabbit;
using MongoDb;
using MongoDB.Driver;

public static class DependenciesInitialization
{
    public static IServiceCollection UseSaldos(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IMongoClient>(_ =>
        {
            var connectionString =
                configuration.GetConnectionString("mongo");

            return new MongoClient(connectionString);
        });

        services.AddSingleton(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();

            return client.GetDatabase("saldodb");
        });

        services.AddTransient<ISaldoStore, SaldoStore>();

        services.AddSingleton<IConsumer, Consumer>();

        services.AddHostedService<ConsolidarSaldoConsumer>();

        return services;
    }
}