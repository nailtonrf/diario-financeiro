namespace Fluxo.Abstractions.Messaging;

using Shell;

public interface IProducer
{
    ValueTask ProduceAsync(IMessage message, CancellationToken cancellationToken);
}