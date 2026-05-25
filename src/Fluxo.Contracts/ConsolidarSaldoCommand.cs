namespace Fluxo.Contracts;

using Abstractions.Shell;

public record ConsolidarSaldoCommand(
    DateOnly DataCompetencia,
    decimal SaldoAnterior,
    decimal TotalCreditos = 0,
    decimal TotalDebitos = 0,
    decimal TotalEstornos = 0) : IMessage;