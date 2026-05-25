namespace Fluxo.Saldos.Service.Infra;

using Shell.Stores;

public static class EndpointsInitialization
{
    public static WebApplication UseSaldosEndpoints(this WebApplication app)
    {
        app.MapGet("/", () => Results.Redirect("/scalar"));

        app.MapGet(
            "/saldos",
            async (
                ISaldoStore store,
                CancellationToken ct) =>
            {
                var saldos =
                    await store.GetAllAsync(ct);

                return saldos
                    .Select(x => new
                    {
                        x.DataCompetencia,
                        x.Valor
                    });
            });

        return app;
    }
}