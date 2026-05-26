namespace Fluxo.UnitTesting.Fixtures;

using Fluxo.Lancamentos.Service.Core;

/// <summary>
/// Builder para criar instâncias de teste de Competência.
/// </summary>
public sealed class CompetenciaDataBuilder
{
    private DateOnly _data = DateOnly.FromDateTime(DateTime.UtcNow);

    public CompetenciaDataBuilder WithData(DateOnly data)
    {
        _data = data;

        return this;
    }

    public CompetenciaDataBuilder WithDataFromDateTime(DateTime dateTime)
    {
        _data = DateOnly.FromDateTime(dateTime);

        return this;
    }

    public Competencia Build() => new() { DataCompetencia = DateOnly.FromDateTime(DateTime.UtcNow) };

    public static Competencia Default() => new() { DataCompetencia = DateOnly.FromDateTime(DateTime.UtcNow) };
}