namespace Fluxo.Lancamentos.Service.Infra;

using Core;
using EntityFramework;

public static class DataInitialization
{
    public static async Task AplicarMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<LancamentosDbContext>();

        await db.Database.MigrateAsync();
    }

    public static async Task SeedCompetenciaAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<LancamentosDbContext>();

        if (!await db.Competencias.AnyAsync())
        {
            db.Competencias.Add(
                new Competencia
                {
                    Id = 1,
                    DataCompetencia =
                        DateOnly.FromDateTime(
                            DateTime.UtcNow)
                });

            await db.SaveChangesAsync();
        }
    }
}