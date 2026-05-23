using Fluxo.Lancamentos.Service.Shell.Resources.Entities;
using Fluxo.Lancamentos.Service.Shell.Stores;

namespace Fluxo.Lancamentos.Service.Shell;

using Core.Creditar;
using Core.Debitar;
using Core.Estornar;
using Handlers;

public static class Configuration
{
    public static IServiceCollection UseLancamentos(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddTransient<IInteractor<CreditarCommand, CreditoEfetuadoEvent>, CreditarInteractor>()
            .AddTransient<IInteractor<DebitarCommand, DebitoEfetuadoEvent>, DebitarInteractor>()
            .AddTransient<IInteractor<EstornarCommand, EstornoEfetuadoEvent>, EstornarInteractor>()
            .AddTransient<ICompetenciaStore, CompetenciaStore>()
            .AddTransient<ILancamentoStore, LancamentoStore>();

        services.AddDbContext<LancamentosDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("fluxodb"));
        });

        return services;
    }
}