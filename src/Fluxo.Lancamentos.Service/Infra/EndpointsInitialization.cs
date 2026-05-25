namespace Fluxo.Lancamentos.Service.Infra;

using Core.Consolidar;
using Core.Creditar;
using Core.Debitar;
using Core.Estornar;

public static class EndpointsInitialization
{
    public static WebApplication UseLancamentosEndpoints(this WebApplication app)
    {
        app.MapGet("/", () => Results.Redirect("/scalar"));

        app.MapPost(
                "/lancamentos/creditar",
                async (
                    CreditarCommand command,
                    IInteractor<CreditarCommand, CreditoEfetuadoEvent> interactor,
                    CancellationToken cancellationToken) =>
                {
                    var result =
                        await interactor.InteractAsync(
                            command,
                            cancellationToken);

                    return result.Match(
                        success => Results.Created($"/lancamentos/{success.IdLancamento}", success),
                        Results.BadRequest);
                })
            .WithName("Creditar")
            .WithTags("Lancamentos")
            .WithSummary("Efetua Crédito.")
            .Produces<CreditoEfetuadoEvent>(201)
            .Produces<ErrorResult>(400);

        app.MapPost(
                "/lancamentos/debitar",
                async (
                    DebitarCommand command,
                    IInteractor<DebitarCommand, DebitoEfetuadoEvent> interactor,
                    CancellationToken cancellationToken) =>
                {
                    var result =
                        await interactor.InteractAsync(
                            command,
                            cancellationToken);

                    return result.Match(
                        success => Results.Created($"/lancamentos/{success.IdLancamento}", success),
                        Results.BadRequest);
                })
            .WithName("Debitar")
            .WithTags("Lancamentos")
            .WithSummary("Efetua Débito.")
            .Produces<DebitoEfetuadoEvent>(201)
            .Produces<ErrorResult>(400);

        app.MapPost(
                "/lancamentos/estornar",
                async (
                    EstornarCommand command,
                    IInteractor<EstornarCommand, EstornoEfetuadoEvent> interactor,
                    CancellationToken cancellationToken) =>
                {
                    var result =
                        await interactor.InteractAsync(
                            command,
                            cancellationToken);

                    return result.Match(
                        success => Results.Created($"/lancamentos/{success.IdLancamento}", success),
                        Results.BadRequest);
                })
            .WithName("Estornar")
            .WithTags("Lancamentos")
            .WithSummary("Efetua Estorno de Lançamento.")
            .Produces<EstornoEfetuadoEvent>(201)
            .Produces<ErrorResult>(400);

        app.MapPost(
                "/lancamentos/consolidar",
                async (
                    ConsolidarDiaCommand command,
                    IInteractor<ConsolidarDiaCommand, DiaConsolidadoEvent> interactor,
                    CancellationToken cancellationToken) =>
                {
                    var result =
                        await interactor.InteractAsync(
                            command,
                            cancellationToken);

                    return result.Match(
                        success => Results.Created($"/lancamentos/{success.IdLancamento}", success),
                        Results.BadRequest);
                })
            .WithName("Consolidar")
            .WithTags("Lancamentos")
            .WithSummary("Efetua Consolidação de Lançamentos.")
            .Produces<DiaConsolidadoEvent>(201)
            .Produces<ErrorResult>(400);

        return app;
    }
}