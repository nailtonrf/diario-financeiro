namespace Fluxo.Saldos.Service.Shell.Backgrounds;

using Abstractions.Messaging;
using Contracts;
using Core;
using Stores;
using static Abstractions.Data.GuidModule;

public sealed class ConsolidarSaldoConsumer(
    IConsumer consumerProvider,
    IServiceScopeFactory scopeFactory) : BackgroundService
{
    private const string Fila = "consolidarsaldo.command";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await consumerProvider.ConsumeAsync<ConsolidarSaldoCommand>(
            Fila,
            HandleAsync,
            stoppingToken);
    }

    private async ValueTask HandleAsync(
        ConsolidarSaldoCommand command,
        CancellationToken cancellationToken)
    {
        var saldo = new Saldo(
            Sequential().ToString(),
            command.DataCompetencia,
            command.SaldoAnterior +
            command.TotalCreditos -
            command.TotalDebitos +
            command.TotalEstornos);

        await using var scope = scopeFactory.CreateAsyncScope();
        var saldoStore = scope.ServiceProvider.GetRequiredService<ISaldoStore>();
        await saldoStore.InsertAsync(saldo, cancellationToken);
    }
}