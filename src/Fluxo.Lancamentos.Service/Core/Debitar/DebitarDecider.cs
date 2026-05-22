namespace Fluxo.Lancamentos.Service.Core.Debitar;

using static GuidModule;
using static ValorPositivoModule;
using static CompetenciaModule;

public static class DebitarDecider
{
    public static Result<DebitoEfetuadoEvent> Decide(Competencia competencia, DebitarCommand debitarCommand)
        => competencia
            .ValidarCompetencia(debitarCommand.Data)
            .Bind(_ => debitarCommand.Valor.ValidarValorPositivo())
            .Map(valorPositivo =>
                new DebitoEfetuadoEvent
                {
                    Id = new LancamentoId(Sequential()),
                    Descricao = debitarCommand.Descricao,
                    Data = debitarCommand.Data,
                    Valor = valorPositivo.Valor
                });
}