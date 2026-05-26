namespace Fluxo.Lancamentos.Service.Infra.EntityFramework;

using Core;
using Shell.Stores;

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

    public Result<Competencia> Save(Competencia competencia)
    {
        dbContext.Update(competencia);

        return Ok(competencia);
    }
}