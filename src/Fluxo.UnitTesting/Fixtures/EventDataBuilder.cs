namespace Fluxo.UnitTesting.Fixtures;

using Fluxo.Lancamentos.Service.Core;
using Fluxo.Lancamentos.Service.Core.Creditar;
using Fluxo.Lancamentos.Service.Core.Debitar;
using Fluxo.Lancamentos.Service.Core.Estornar;

/// <summary>
/// Builder para criar instâncias de teste de Eventos.
/// </summary>
public sealed class EventDataBuilder
{
    private Guid _idLancamento = Guid.NewGuid();
    private Guid _idempotencyKey = Guid.NewGuid();
    private DateOnly _dataCompetencia = DateOnly.FromDateTime(DateTime.UtcNow);
    private decimal _valor = 100m;
    private string _descricao = "Descrição padrão";
    private DateTime _criadoEm = DateTime.UtcNow;
    private int _version = 0;

    public EventDataBuilder WithIdLancamento(Guid idLancamento)
    {
        _idLancamento = idLancamento;
        return this;
    }

    public EventDataBuilder WithIdempotencyKey(Guid idempotencyKey)
    {
        _idempotencyKey = idempotencyKey;
        return this;
    }

    public EventDataBuilder WithDataCompetencia(DateOnly dataCompetencia)
    {
        _dataCompetencia = dataCompetencia;
        return this;
    }

    public EventDataBuilder WithValor(decimal valor)
    {
        _valor = valor;
        return this;
    }

    public EventDataBuilder WithDescricao(string descricao)
    {
        _descricao = descricao;
        return this;
    }

    public EventDataBuilder WithCriadoEm(DateTime criadoEm)
    {
        _criadoEm = criadoEm;
        return this;
    }

    public EventDataBuilder WithVersion(int version)
    {
        _version = version;
        return this;
    }

    public CreditoEfetuadoEvent BuildCreditoEfetuadoEvent() => new(
        IdLancamento: _idLancamento,
        IdempotencyKey: _idempotencyKey,
        DataCompetencia: _dataCompetencia,
        Valor: _valor,
        Descricao: _descricao,
        CriadoEm: _criadoEm,
        EventVersion: _version);

    public DebitoEfetuadoEvent BuildDebitoEfetuadoEvent() => new(
        IdLancamento: _idLancamento,
        IdempotencyKey: _idempotencyKey,
        DataCompetencia: _dataCompetencia,
        Valor: _valor,
        Descricao: _descricao,
        CriadoEm: _criadoEm,
        EventVersion: _version);

    public EstornoEfetuadoEvent BuildEstornoEfetuadoEvent(Guid? idEstornado = null) => new(
        IdLancamento: _idLancamento,
        IdempotencyKey: _idempotencyKey,
        DataCompetencia: _dataCompetencia,
        Valor: _valor,
        Descricao: _descricao,
        CriadoEm: _criadoEm,
        EventVersion: _version,
        IdEstornado: idEstornado ?? Guid.NewGuid());
}
