namespace Fluxo.Lancamentos.Service.Core.Estornar;

using Creditar;
using Debitar;
using static GuidModule;

public static class EstornarDecider
{
    public static Result<EstornoEfetuadoEvent> Decide(
        DateTime dataAtual,
        Competencia competencia,
        Lancamento lancamentoEstornar)
        => lancamentoEstornar switch
        {
            EstornoEfetuadoEvent => ErrorResult.Validation("Lançamento já foi estornado."),
            CreditoEfetuadoEvent c => Ok(new EstornoEfetuadoEvent
            {
                IdLancamento = new LancamentoId(Sequential()),
                Descricao = $"[Estorno a Débito] -  {c.Descricao}",
                Data = dataAtual,
                DataCompetencia = competencia.DataCompetencia,
                Valor = c.Valor * -1,
                IdEstornado = c.IdLancamento
            }),
            DebitoEfetuadoEvent d => Ok(new EstornoEfetuadoEvent
            {
                IdLancamento = new LancamentoId(Sequential()),
                Descricao = $"[Estorno a Crédito] -  {d.Descricao}",
                Data = dataAtual,
                DataCompetencia = competencia.DataCompetencia,
                Valor = d.Valor,
                IdEstornado = d.IdLancamento
            }),
            _ => throw new ArgumentOutOfRangeException(nameof(lancamentoEstornar), lancamentoEstornar, null)
        };
}