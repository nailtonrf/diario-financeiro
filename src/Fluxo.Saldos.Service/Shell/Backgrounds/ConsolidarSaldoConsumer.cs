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
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await consumerProvider.ConsumeAsync<ConsolidarSaldoCommand>(
            "lancamentos",
            HandleAsync,
            stoppingToken);
    }

    private async ValueTask HandleAsync(
        ConsolidarSaldoCommand command,
        CancellationToken cancellationToken)
    {
        var saldo = new Saldo(
            Id: Sequential().ToString(),
            command.DataCompetencia,
            Valor:
            command.SaldoAnterior +
            command.TotalCreditos -
            command.TotalDebitos +
            command.TotalEstornos);

        await using var scope = scopeFactory.CreateAsyncScope();
        var saldoStore = scope.ServiceProvider.GetRequiredService<ISaldoStore>();
        await saldoStore.InsertAsync(saldo, cancellationToken);
    }
}