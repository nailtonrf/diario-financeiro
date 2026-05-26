namespace Fluxo.Saldos.Service.Infra;

using Abstractions.Messaging;
using Shell.Backgrounds;
using Shell.Stores;
using Rabbit;
using RabbitMQ.Client;
using MongoDb;
using MongoDB.Driver;

public static class DependenciesInitialization
{
    public static IServiceCollection UseSaldos(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IMongoClient>(_ =>
        {
            var connectionString =
                configuration.GetConnectionString("saldodb");

            return new MongoClient(connectionString);
        });

        services.AddSingleton(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();

            return client.GetDatabase("saldodb");
        });

        services.AddTransient<ISaldoStore, SaldoStore>();

        services.AddSingleton<IConnectionFactory>(_
            => new ConnectionFactory
            {
                Uri = new Uri(configuration.GetConnectionString("rabbitmq")!)
            });

        services.AddSingleton<IConsumer, Consumer>();

        services.AddHostedService<ConsolidarSaldoConsumer>();

        return services;
    }
}