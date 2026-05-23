using Fluxo.Lancamentos.Service.Core;
using Fluxo.Lancamentos.Service.Core.Creditar;
using Fluxo.Lancamentos.Service.Core.Debitar;
using Fluxo.Lancamentos.Service.Infra.EntityFramework;
using Fluxo.Lancamentos.Service.Shell;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

builder.Services.UseLancamentos(builder.Configuration);

var app = builder.Build();

await AplicarMigrationsAsync(app);

await SeedCompetenciaAsync(app);

app.MapDefaultEndpoints();

app.MapOpenApi();

app.MapScalarApiReference();

app.UseHttpsRedirection();

app.MapPost(
        "/lancamentos/creditar",
        async (
            CreditarCommand command,
            IInteractor<CreditarCommand, CreditoEfetuadoEvent> interactor,
            CancellationToken cancellationToken) =>
        {
            var result =
                await interactor.InteractAsync(
                    command,
                    cancellationToken);

            return result.Match(
                success => Results.Created($"/lancamentos/{success.IdLancamento}", success),
                Results.BadRequest);
        })
    .WithName("Creditar")
    .WithTags("Lancamentos")
    .WithSummary("Efetua um crédito.")
    .Produces<CreditoEfetuadoEvent>(201)
    .Produces<ErrorResult>(400);

app.MapPost(
        "/lancamentos/debitar",
        async (
            DebitarCommand command,
            IInteractor<DebitarCommand, DebitoEfetuadoEvent> interactor,
            CancellationToken cancellationToken) =>
        {
            var result =
                await interactor.InteractAsync(
                    command,
                    cancellationToken);

            return result.Match(
                success => Results.Created($"/lancamentos/{success.IdLancamento}", success),
                Results.BadRequest);
        })
    .WithName("Debitar")
    .WithTags("Lancamentos")
    .WithSummary("Efetua um débito.")
    .Produces<CreditoEfetuadoEvent>(201)
    .Produces<ErrorResult>(400);

await app.RunAsync();

return;

static async Task AplicarMigrationsAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();

    var db = scope.ServiceProvider
        .GetRequiredService<LancamentosDbContext>();

    await db.Database.MigrateAsync();
}

static async Task SeedCompetenciaAsync(WebApplication app)
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