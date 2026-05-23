namespace Fluxo.Lancamentos.Service.Shell.Handlers;

using Core.Creditar;
using Stores;

public sealed class CreditarInteractor(
    ICompetenciaStore competenciaStore,
    ILancamentoStore lancamentoStore) : IInteractor<CreditarCommand, CreditoEfetuadoEvent>
{
    public async ValueTask<Result<CreditoEfetuadoEvent>> InteractAsync(
        CreditarCommand creditar,
        CancellationToken cancellationToken)
        => await competenciaStore.GetAsync(cancellationToken)
            .ToResult(ErrorResult.Validation("Data de Competência não cadastrada."))
            .Bind(competencia => CreditarDecider.Decide(DateTime.UtcNow, competencia, creditar))
            .Bind(async evento => await lancamentoStore.AppendAsync(evento, cancellationToken));
}