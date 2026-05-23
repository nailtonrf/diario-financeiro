using Fluxo.Lancamentos.Service.Core;
using Fluxo.Lancamentos.Service.Shell.Stores;

namespace Fluxo.Lancamentos.Service.Infra.EntityFramework;

public sealed class CompetenciaStore(
    LancamentosDbContext dbContext) : ICompetenciaStore
{
    public async Task<Option<Competencia>> GetAsync(CancellationToken cancellationToken)
    {
        var competencia = await dbContext.Competencias
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);

        return competencia is null
            ? None<Competencia>()
            : Some(competencia);
    }
}