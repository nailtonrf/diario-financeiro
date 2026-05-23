using Fluxo.Lancamentos.Service.Core;
using Fluxo.Lancamentos.Service.Core.Estornar;
using Fluxo.Lancamentos.Service.Shell.Stores;

namespace Fluxo.Lancamentos.Service.Shell.Resources.Entities;

public sealed class LancamentoStore(
    LancamentosDbContext dbContext) : ILancamentoStore
{
    public async Task<Result<T>> AppendAsync<T>(T @event, CancellationToken cancellationToken) where T : Lancamento
    {
        await dbContext.Lancamentos.AddAsync(@event, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(@event);
    }

    public async Task<Option<Lancamento>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var jaEstornado = await dbContext.Lancamentos
            .AsNoTracking()
            .OfType<EstornoEfetuadoEvent>()
            .SingleOrDefaultAsync(p => p.IdEstornado.Id == id, cancellationToken);

        if (jaEstornado is not null)
            return Some<Lancamento>(jaEstornado);

        var estorno = await dbContext.Lancamentos
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.IdLancamento.Id == id, cancellationToken);

        return estorno is null
            ? None<Lancamento>()
            : Some(estorno);
    }
}