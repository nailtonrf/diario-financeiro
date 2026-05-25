namespace Fluxo.Lancamentos.Service.Infra;

using Core;
using EntityFramework;

public static class DataInitialization
{
    extension(WebApplication app)
    {
        public async Task AplicarMigrationsAsync()
        {
            using var scope = app.Services.CreateScope();

            var db = scope.ServiceProvider
                .GetRequiredService<LancamentosDbContext>();

            await db.Database.MigrateAsync();
        }

        public async Task SeedCompetenciaAsync()
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
}