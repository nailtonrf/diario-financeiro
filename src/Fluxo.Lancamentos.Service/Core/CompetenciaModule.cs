namespace Fluxo.Lancamentos.Service.Core;

public static class CompetenciaModule
{
    public static Result<Competencia> GerarCompetenciaNova(
        this Competencia competenciaAtual,
        DateOnly dataCompetenciaNova)
        => competenciaAtual.DataCompetencia >= dataCompetenciaNova
            ? ErrorResult.Validation(
                $"Próxima Competência deve ser superior a Atual - {competenciaAtual.DataCompetencia}")
            : Ok(competenciaAtual with { DataCompetencia = dataCompetenciaNova });
}