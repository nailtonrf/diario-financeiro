namespace Fluxo.Lancamentos.Service.Infra;

using Abstractions.Messaging;
using Core.Consolidar;
using Core.Creditar;
using Core.Debitar;
using Core.Estornar;
using Shell.Handlers;
using Shell.Stores;
using EntityFramework;
using Rabbit;
using RabbitMQ.Client;

public static class DependenciesInitialization
{
    public static IServiceCollection UseLancamentos(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddTransient<IInteractor<CreditarCommand, CreditoEfetuadoEvent>, CreditarInteractor>()
            .AddTransient<IInteractor<DebitarCommand, DebitoEfetuadoEvent>, DebitarInteractor>()
            .AddTransient<IInteractor<EstornarCommand, EstornoEfetuadoEvent>, EstornarInteractor>()
            .AddTransient<IInteractor<ConsolidarDiaCommand, DiaConsolidadoEvent>, ConsolidarDiaInteractor>()
            .AddTransient<ICompetenciaStore, CompetenciaStore>()
            .AddTransient<ILancamentoStore, LancamentoStore>();

        services.AddDbContext<ILancamentosDataContext, LancamentosDbContext>(options
            => options.UseNpgsql(configuration.GetConnectionString("fluxodb")));

        services.AddSingleton<IConnectionFactory>(_
            => new ConnectionFactory
            {
                Uri = new Uri(configuration.GetConnectionString("rabbitmq")!)
            });

        services.AddTransient<IProducer, Producer>();

        return services;
    }
}