namespace Fluxo.Contracts;

using Abstractions.Shell;

public sealed record ConsolidarSaldoCommand : IMessage
{
    public DateOnly DataCompetencia { get; set; }
    public decimal SaldoAnterior { get; set; } = 0;
    public decimal TotalCreditos { get; set; } = 0;
    public decimal TotalDebitos { get; set; } = 0;
    public decimal TotalEstornos { get; set; } = 0;
}