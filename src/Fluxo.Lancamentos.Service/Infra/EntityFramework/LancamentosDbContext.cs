namespace Fluxo.Lancamentos.Service.Infra.EntityFramework;

using Core;
using Shell.Stores;

public sealed class LancamentosDbContext(DbContextOptions<LancamentosDbContext> options)
    : DbContext(options), ILancamentosDataContext
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