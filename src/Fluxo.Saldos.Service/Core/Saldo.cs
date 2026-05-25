namespace Fluxo.Saldos.Service.Core;

public record Saldo(
    string Id,
    DateOnly DataCompetencia,
    decimal Valor);