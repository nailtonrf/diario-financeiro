namespace Fluxo.Abstractions.Messaging;

using Shell;

public interface IConsumer
{
    ValueTask ConsumeAsync<TMessage>(
        string queue,
        Func<TMessage, CancellationToken, ValueTask> handler,
        CancellationToken cancellationToken) where TMessage : IMessage;
}