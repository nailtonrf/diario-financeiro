using Fluxo.Lancamentos.Service.Core.Creditar;
using Fluxo.Lancamentos.Service.Core.Debitar;

namespace Fluxo.Lancamentos.Service.Infra;

public static class Endpoints
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
            .WithSummary("Efetua um crédito.")
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
            .WithSummary("Efetua um débito.")
            .Produces<CreditoEfetuadoEvent>(201)
            .Produces<ErrorResult>(400);

        return app;
    }
}