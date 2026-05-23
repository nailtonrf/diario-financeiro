namespace Fluxo.Lancamentos.Service.Core.Debitar;

using static GuidModule;
using static ValorPositivoModule;

public static class DebitarDecider
{
    public static Result<DebitoEfetuadoEvent> Decide(
        DateTime dataAtual,
        Competencia competencia,
        DebitarCommand debitarCommand)
        => debitarCommand.Valor.ValidarValorPositivo()
            .Map(valorPositivo =>
                new DebitoEfetuadoEvent
                {
                    IdLancamento = new LancamentoId(Sequential()),
                    Descricao = debitarCommand.Descricao,
                    Data = dataAtual,
                    DataCompetencia = competencia.DataCompetencia,
                    Valor = valorPositivo.Valor
                });
}