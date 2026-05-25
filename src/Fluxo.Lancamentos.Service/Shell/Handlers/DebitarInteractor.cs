namespace Fluxo.Lancamentos.Service.Shell.Handlers;

using Core.Debitar;
using Stores;

public sealed class DebitarInteractor(
    ILancamentosDataContext dataContext,
    ICompetenciaStore competenciaStore,
    ILancamentoStore lancamentoStore) : IInteractor<DebitarCommand, DebitoEfetuadoEvent>
{
    public async ValueTask<Result<DebitoEfetuadoEvent>> 
        InteractAsync(
            DebitarCommand debitar,
            CancellationToken cancellationToken)
        => await competenciaStore
            .GetAsync(cancellationToken)
            .ToResult(
                ErrorResult.Validation(
                    "Data de Competência não cadastrada."))
            .Bind(competencia => 
                DebitarDecider.Decide(
                    DateTime.UtcNow, 
                    competencia, 
                    debitar))
            .Bind(evento => 
                lancamentoStore.AppendAsync(
                    evento, 
                    cancellationToken))
            .Tee(_ =>
                dataContext.SaveChangesAsync(
                    cancellationToken));    
}