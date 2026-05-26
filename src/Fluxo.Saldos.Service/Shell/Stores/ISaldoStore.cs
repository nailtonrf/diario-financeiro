namespace Fluxo.Saldos.Service.Shell.Stores;

using Core;

public interface ISaldoStore
{
    Task InsertAsync(
        Saldo saldo,
        CancellationToken cancellationToken);

    Task<Saldo[]> GetAllAsync(
        CancellationToken cancellationToken);
}