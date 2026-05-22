namespace Fluxo.Lancamentos.Service.Core.Estornar;

public sealed record EstornoEfetuadoEvent : Lancamento
{
    public required LancamentoId IdEstornado { get; init; }
}