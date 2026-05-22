namespace Fluxo.Lancamentos.Service.Core;

public abstract record Lancamento : IEvent
{
    public required LancamentoId Id { get; init; }
    public required string Descricao { get; init; }
    public required DateTime Data { get; init; }
    public required decimal Valor { get; init; }
}