namespace Fluxo.Lancamentos.Service.Core;

public static class CompetenciaModule
{
    public static Result<DateTime> ValidarCompetencia(this Competencia competencia, DateTime dataLancamento)
        => competencia.DataCorrente.Equals(dataLancamento.Date)
            ? Ok(dataLancamento)
            : ErrorResult.Validation(
                $"Data de Lançamento - {dataLancamento} - fora da Competência atual - {competencia.DataCorrente}.");
}