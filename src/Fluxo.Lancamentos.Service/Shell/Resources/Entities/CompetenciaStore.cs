using Fluxo.Lancamentos.Service.Core;
using Fluxo.Lancamentos.Service.Shell.Stores;
using Microsoft.EntityFrameworkCore.Design;

namespace Fluxo.Lancamentos.Service.Shell.Resources.Entities;

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

public sealed class LancamentosDbContextFactory
    : IDesignTimeDbContextFactory<LancamentosDbContext>
{
    public LancamentosDbContext CreateDbContext(
        string[] args)
    {
        var options =
            new DbContextOptionsBuilder<
                LancamentosDbContext>();

        options.UseNpgsql(
            "Host=localhost;Port=5432;Database=fluxodb;Username=postgres;Password=abc123+");

        return new LancamentosDbContext(
            options.Options);
    }
}