namespace Fluxo.Lancamentos.Service.Shell.Handlers;

using Core.Estornar;
using Stores;

public sealed class EstornarInteractor(
    ILancamentosDataContext dataContext,
    ICompetenciaStore competenciaStore,
    ILancamentoStore lancamentoStore) : IInteractor<EstornarCommand, EstornoEfetuadoEvent>
{
    public async ValueTask<Result<EstornoEfetuadoEvent>>
        InteractAsync(
            EstornarCommand input,
            CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(input.LancamentoId, out var lancamentoId))
            return ErrorResult.Validation($"Lançamento - {lancamentoId} - não encontrado.");

        var lancamentoEstornoOption = await lancamentoStore.GetByIdAsync(lancamentoId, cancellationToken);

        if (lancamentoEstornoOption.IsNone)
            return ErrorResult.Validation($"Lançamento - {lancamentoId} - não encontrado.");

        return await competenciaStore
            .GetAsync(cancellationToken)
            .ToResult(
                ErrorResult.Validation(
                    "Data de Competência não cadastrada."))
            .Bind(competencia =>
                EstornarDecider.Decide(
                    DateTime.UtcNow,
                    competencia,
                    lancamentoEstornoOption.Value!))
            .Bind(evento =>
                lancamentoStore.AppendAsync(
                    evento,
                    cancellationToken))
            .Tee(_ =>
                dataContext.SaveChangesAsync(
                    cancellationToken));
    }
}