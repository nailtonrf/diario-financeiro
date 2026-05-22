namespace Fluxo.Lancamentos.Service.Core.Estornar;

using Creditar;
using Debitar;
using static GuidModule;

public static class EstornarDecider
{
    public static Result<EstornoEfetuadoEvent> Decide(Competencia competencia, Lancamento lancamentoEstornar)
        => lancamentoEstornar switch
        {
            EstornoEfetuadoEvent => ErrorResult.Validation("Lançamento já foi estornado."),
            CreditoEfetuadoEvent c => Ok(new EstornoEfetuadoEvent
            {
                Id = new LancamentoId(Sequential()),
                Descricao = $"[Estorno a Débito] -  {c.Descricao}",
                Data = competencia.DataCorrenteComHora,
                Valor = c.Valor * -1,
                IdEstornado = c.Id
            }),
            DebitoEfetuadoEvent d => Ok(new EstornoEfetuadoEvent
            {
                Id = new LancamentoId(Sequential()),
                Descricao = $"[Estorno a Crédito] -  {d.Descricao}",
                Data = competencia.DataCorrenteComHora,
                Valor = d.Valor * -1,
                IdEstornado = d.Id
            }),
            _ => throw new ArgumentOutOfRangeException(nameof(lancamentoEstornar), lancamentoEstornar, null)
        };
}