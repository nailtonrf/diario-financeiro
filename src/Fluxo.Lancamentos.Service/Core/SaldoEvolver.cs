namespace Fluxo.Lancamentos.Service.Core;

using Creditar;
using Debitar;
using Estornar;

public static class SaldoEvolver
{
    public static Saldo Evolve(
        Saldo saldoInicial,
        IEnumerable<Lancamento> lancamentos)
    {
        var saldoCalculado = saldoInicial.Valor;
        var dataCalculada = saldoInicial.Data;
        foreach (var lancamento in lancamentos)
        {
            dataCalculada = lancamento.DataCompetencia;
            saldoCalculado += lancamento switch
            {
                CreditoEfetuadoEvent c => c.Valor,
                DebitoEfetuadoEvent d => d.Valor * -1,
                EstornoEfetuadoEvent e => e.Valor,
                _ => 0
            };
        }

        return new Saldo(
            dataCalculada,
            saldoCalculado);
    }
}