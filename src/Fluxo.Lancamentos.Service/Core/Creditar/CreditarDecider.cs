namespace Fluxo.Lancamentos.Service.Core.Creditar;

using static GuidModule;
using static ValorPositivoModule;
using static CompetenciaModule;

public static class CreditarDecider
{
    public static Result<CreditoEfetuadoEvent> Decide(Competencia competencia, CreditarCommand creditarCommand)
        => competencia
            .ValidarCompetencia(creditarCommand.Data)
            .Bind(_ => creditarCommand.Valor.ValidarValorPositivo())
            .Map(valorPositivo =>
                new CreditoEfetuadoEvent
                {
                    Id = new LancamentoId(Sequential()),
                    Descricao = creditarCommand.Descricao,
                    Data = creditarCommand.Data,
                    Valor = valorPositivo.Valor
                });
}