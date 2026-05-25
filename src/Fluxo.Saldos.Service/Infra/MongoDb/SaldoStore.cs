namespace Fluxo.Saldos.Service.Infra.MongoDb;

using Core;
using Shell.Stores;
using MongoDB.Driver;

public sealed class SaldoStore(
    IMongoDatabase database) : ISaldoStore
{
    private const string CollectionName = "saldos";

    private readonly IMongoCollection<Saldo>
        _collection = database.GetCollection<Saldo>(CollectionName);

    public async Task InsertAsync(Saldo saldo, CancellationToken cancellationToken)
        => await _collection.InsertOneAsync(
            saldo,
            cancellationToken: cancellationToken);

    public async Task<Saldo[]> GetAllAsync(CancellationToken cancellationToken)
    {
        var saldos = await _collection
            .Find(FilterDefinition<Saldo>.Empty)
            .ToListAsync(cancellationToken);

        return saldos.ToArray();
    }
}