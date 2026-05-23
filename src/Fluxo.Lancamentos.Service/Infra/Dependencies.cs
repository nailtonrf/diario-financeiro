namespace Fluxo.Lancamentos.Service.Infra;

using Core.Creditar;
using Core.Debitar;
using Core.Estornar;
using EntityFramework;
using Shell.Handlers;
using Shell.Stores;

public static class Dependencies
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