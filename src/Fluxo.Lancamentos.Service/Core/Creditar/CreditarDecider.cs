namespace Fluxo.Lancamentos.Service.Core.Creditar;

using static GuidModule;
using static ValorPositivoModule;

public static class CreditarDecider
{
    public static Result<CreditoEfetuadoEvent> Decide(
        DateTime dataAtual,
        Competencia competencia,
        CreditarCommand creditarCommand)
        => creditarCommand.Valor
            .ValidarValorPositivo()
            .Map(valorPositivo =>
                new CreditoEfetuadoEvent
                {
                    IdLancamento = new LancamentoId(Sequential()),
                    Descricao = creditarCommand.Descricao,
                    Data = dataAtual,
                    DataCompetencia = competencia.DataCompetencia,
                    Valor = valorPositivo.Valor
                });
}