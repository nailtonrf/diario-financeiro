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
        CancellationToken ct)
    {
        competenciaStore.Save(novaCompetencia);

        await lancamentoStore.AppendAsync(
            evento,
            ct);

        await dataContext.SaveChangesAsync(ct);
    }

    private async ValueTask<ConsolidarSaldoCommand>
        CalcularSaldoAsync(
            DateOnly dataCompetencia,
            CancellationToken ct)
    {
        var lancamentosOption =
            await lancamentoStore
                .GetByDataCompetenciaAsync(
                    dataCompetencia,
                    ct);

        var lancamentos =
            lancamentosOption.Value!;

        return new ConsolidarSaldoCommand(
            dataCompetencia,
            0,
            Totalizar<CreditoEfetuadoEvent>(
                lancamentos,
                dataCompetencia),
            Totalizar<DebitoEfetuadoEvent>(
                lancamentos,
                dataCompetencia),
            Totalizar<EstornoEfetuadoEvent>(
                lancamentos,
                dataCompetencia));
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