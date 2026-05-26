namespace Fluxo.Lancamentos.Service.Infra.Rabbit;

using System.Text;
using System.Text.Json;
using Abstractions.Messaging;
using RabbitMQ.Client;

public sealed class Producer(
    IConnectionFactory connectionFactory) : IProducer
{
    private const string Fila = "consolidarsaldo.command";

    public async ValueTask ProduceAsync<T>(T message, CancellationToken cancellationToken) where T : IMessage
    {
        await using var connection =
            await connectionFactory.CreateConnectionAsync(cancellationToken);

        await using var channel =
            await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
            Fila,
            true,
            false,
            false,
            cancellationToken: cancellationToken);

        var body = Encoding.UTF8.GetBytes(
            JsonSerializer.Serialize(message));

        await channel.BasicPublishAsync(
            "",
            Fila,
            body,
            cancellationToken);
    }
}