namespace Fluxo.Saldos.Service.Infra.Rabbit;

using System.Text;
using System.Text.Json;
using Abstractions.Messaging;
using Abstractions.Shell;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public sealed class Consumer(
    IConnectionFactory connectionFactory) : IConsumer
{
    public async ValueTask ConsumeAsync<TMessage>(
        string queue,
        Func<TMessage, CancellationToken, ValueTask> handler,
        CancellationToken cancellationToken) where TMessage : IMessage
    {
        await using var connection =
            await connectionFactory.CreateConnectionAsync(cancellationToken);

        await using var channel =
            await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
            queue,
            true,
            false,
            false,
            cancellationToken: cancellationToken);

        var consumer =
            new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var json =
                    Encoding.UTF8.GetString(ea.Body.ToArray());

                var message =
                    JsonSerializer.Deserialize<TMessage>(json);

                if (message is null)
                    return;

                await handler(
                    message,
                    cancellationToken);

                await channel.BasicAckAsync(
                    ea.DeliveryTag,
                    false,
                    cancellationToken);
            }
            catch
            {
                await channel.BasicNackAsync(
                    ea.DeliveryTag,
                    false,
                    true,
                    cancellationToken);
            }
        };

        await channel.BasicConsumeAsync(
            queue,
            false,
            consumer,
            cancellationToken);
    }
}