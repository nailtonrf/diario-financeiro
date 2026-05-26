namespace Fluxo.Lancamentos.Service.Core;

public sealed record Saldo(
    DateOnly Data,
    decimal Valor);