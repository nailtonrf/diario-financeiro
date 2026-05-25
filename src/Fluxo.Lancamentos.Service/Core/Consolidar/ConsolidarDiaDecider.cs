namespace Fluxo.Lancamentos.Service.Core.Consolidar;

using static GuidModule;

public static class ConsolidarDiaDecider
{
    public static Result<DiaConsolidadoEvent> Decide(
        DateTime dataAtual,
        Competencia competencia)
        => Ok(
            new DiaConsolidadoEvent
            {
                IdLancamento = new LancamentoId(Sequential()),
                DataCompetencia = competencia.DataCompetencia,
                Data = dataAtual,
                Valor = 0,
                Descricao = $"Fechamento do Diário da competência {competencia.DataCompetencia}"
            }
        );
}