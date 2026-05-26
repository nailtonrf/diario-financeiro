namespace Fluxo.Lancamentos.Service.Infra.EntityFramework;

using Core;
using Core.Estornar;
using Shell.Stores;

public sealed class LancamentoStore(
    LancamentosDbContext dbContext) : ILancamentoStore
{
    public async Task<Result<T>> AppendAsync<T>(T @event, CancellationToken cancellationToken) where T : Lancamento
    {
        await dbContext.Lancamentos.AddAsync(@event, cancellationToken);

        return Ok(@event);
    }

    public async Task<Option<Lancamento>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var jaEstornado = await dbContext.Lancamentos
            .AsNoTracking()
            .OfType<EstornoEfetuadoEvent>()
            .FirstOrDefaultAsync(p => p.IdEstornado == new LancamentoId(id), cancellationToken);

        if (jaEstornado is not null)
            return Some<Lancamento>(jaEstornado);

        var estorno = await dbContext.Lancamentos
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.IdLancamento == new LancamentoId(id), cancellationToken);

        return estorno is null
            ? None<Lancamento>()
            : Some(estorno);
    }

    public Task<Lancamento[]> GetByDataCompetenciaAsync(
        DateOnly dataCompetencia,
        CancellationToken cancellationToken) =>
        dbContext.Lancamentos
            .AsNoTracking()
            .Where(p => p.DataCompetencia == dataCompetencia)
            .ToArrayAsync(cancellationToken);

    public Task<Lancamento[]> GetAnterioresCompetenciaAsync(
        DateOnly dataCompetencia,
        CancellationToken cancellationToken)
        => dbContext.Lancamentos
            .AsNoTracking()
            .Where(p => p.DataCompetencia < dataCompetencia)
            .ToArrayAsync(cancellationToken);
}