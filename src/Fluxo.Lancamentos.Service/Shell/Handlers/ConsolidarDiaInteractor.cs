namespace Fluxo.Lancamentos.Service.Shell.Handlers;

using Abstractions.Messaging;
using Contracts;
using Core;
using Core.Consolidar;
using Core.Creditar;
using Core.Debitar;
using Core.Estornar;
using Stores;

public sealed class ConsolidarDiaInteractor(
    ILancamentosDataContext dataContext,
    ICompetenciaStore competenciaStore,
    ILancamentoStore lancamentoStore,
    IProducer messageProducer) : IInteractor<ConsolidarDiaCommand, DiaConsolidadoEvent>
{
    public async ValueTask<Result<DiaConsolidadoEvent>> InteractAsync(
        ConsolidarDiaCommand consolidar,
        CancellationToken cancellationToken)
    {
        var competenciaOption =
            await competenciaStore.GetAsync(cancellationToken);

        if (competenciaOption.IsNone)
            return ErrorResult.Validation(
                "Data de Competência não cadastrada.");

        var competencia = competenciaOption.Value!;
        var dataCompetencia = competencia.DataCompetencia;

        var consolidacaoResult =
            ConsolidarDiaDecider
                .Decide(DateTime.UtcNow, competencia);

        if (consolidacaoResult.IsError)
            return consolidacaoResult.Errors;

        var diaConsolidadoEvent =
            consolidacaoResult.Unwrap();

        var novaCompetenciaResult =
            competencia.GerarCompetenciaNova(
                consolidar.ProximaData);

        if (novaCompetenciaResult.IsError)
            return novaCompetenciaResult.Errors;

        var novaCompetencia =
            novaCompetenciaResult.Unwrap();

        await PersistirAsync(
            novaCompetencia,
            diaConsolidadoEvent,
            cancellationToken);

        var consolidarSaldoCommand =
            await CalcularSaldoAsync(
                dataCompetencia,
                cancellationToken);

        await messageProducer.ProduceAsync(
            consolidarSaldoCommand,
            cancellationToken);

        return Ok(diaConsolidadoEvent);
    }

    private async ValueTask PersistirAsync(
        Competencia novaCompetencia,
        DiaConsolidadoEvent evento,
        CancellationToken cancellationToken)
    {
        competenciaStore.Save(novaCompetencia);

        await lancamentoStore.AppendAsync(
            evento,
            cancellationToken);

        await dataContext.SaveChangesAsync(cancellationToken);
    }

    private async ValueTask<ConsolidarSaldoCommand>
        CalcularSaldoAsync(
            DateOnly dataCompetencia,
            CancellationToken cancellationToken)
    {
        var lancamentosAnteriores =
            await lancamentoStore
                .GetAnterioresCompetenciaAsync(
                    dataCompetencia,
                    cancellationToken);

        var lancamentosCompetencia =
            await lancamentoStore
                .GetByDataCompetenciaAsync(
                    dataCompetencia,
                    cancellationToken);

        return new ConsolidarSaldoCommand
        {
            DataCompetencia = dataCompetencia,
            SaldoAnterior = SaldoEvolver.Evolve(new Saldo(DateOnly.MinValue, 0), lancamentosAnteriores)
                .Valor,
            TotalCreditos =
                Totalizar<CreditoEfetuadoEvent>(
                    lancamentosCompetencia,
                    dataCompetencia),
            TotalDebitos =
                Totalizar<DebitoEfetuadoEvent>(
                    lancamentosCompetencia,
                    dataCompetencia),
            TotalEstornos =
                Totalizar<EstornoEfetuadoEvent>(
                    lancamentosCompetencia,
                    dataCompetencia)
        };
    }

    static private decimal Totalizar<TEvento>(
        IEnumerable<Lancamento> lancamentos,
        DateOnly dataCompetencia)
        where TEvento : Lancamento
        => lancamentos
            .OfType<TEvento>()
            .Where(x => x.DataCompetencia == dataCompetencia)
            .Sum(x => x.Valor);
}