using Fluxo.Lancamentos.Service.Core;

namespace Fluxo.Lancamentos.Service.Infra.EntityFramework;

public sealed class LancamentosDbContext(DbContextOptions<LancamentosDbContext> options) : DbContext(options)
{
    public DbSet<Lancamento> Lancamentos => Set<Lancamento>();

    public DbSet<Competencia> Competencias => Set<Competencia>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(LancamentosDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}