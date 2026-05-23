namespace Fluxo.Lancamentos.Service.Core;

public sealed record Competencia
{
    public int Id { get; init; } = 1;
    public required DateOnly DataCompetencia { get; init; }
}