namespace Fluxo.Abstractions.Messaging;

using Shell;

public interface IProducer
{
    ValueTask ProduceAsync<T>(
        T message,
        CancellationToken cancellationToken) where T : IMessage;
}