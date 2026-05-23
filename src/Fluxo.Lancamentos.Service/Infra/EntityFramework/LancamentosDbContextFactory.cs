using Microsoft.EntityFrameworkCore.Design;

namespace Fluxo.Lancamentos.Service.Infra.EntityFramework;

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
            "Host=localhost;Port=5432;Database=fluxodb;Username=postgres;Password=******");

        return new LancamentosDbContext(
            options.Options);
    }
}