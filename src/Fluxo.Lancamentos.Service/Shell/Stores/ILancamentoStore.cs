namespace Fluxo.Lancamentos.Service.Shell.Stores;

using Core;

public interface ILancamentoStore
{
    Task<Result<T>> AppendAsync<T>(T @event, CancellationToken cancellationToken) where T : Lancamento;
    Task<Option<Lancamento>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Option<Lancamento[]>> GetByDataCompetenciaAsync(DateOnly dataCompetencia, CancellationToken cancellationToken);
}