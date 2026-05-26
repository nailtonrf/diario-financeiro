namespace Fluxo.UnitTesting.Fixtures;

using Bogus;
using Fluxo.Lancamentos.Service.Core.Creditar;
using Fluxo.Lancamentos.Service.Core.Debitar;
using Fluxo.Lancamentos.Service.Core.Estornar;
using Fluxo.Lancamentos.Service.Core.Consolidar;

/// <summary>
/// Builder para criar instâncias de teste de Commands.
/// </summary>
public sealed class CommandDataBuilder
{
    private readonly Faker _faker = new("pt_BR");
    private decimal _valor = 100m;
    private string _descricao = "Descrição padrão";

    public CommandDataBuilder WithValor(decimal valor)
    {
        _valor = valor;

        return this;
    }

    public CommandDataBuilder WithDescricao(string descricao)
    {
        _descricao = descricao;

        return this;
    }

    public CreditarCommand BuildCreditarCommand() => new(
        Valor: _valor,
        Descricao: _descricao);

    public DebitarCommand BuildDebitarCommand() => new(
        Valor: _valor,
        Descricao: _descricao);

    public EstornarCommand BuildEstornarCommand(Guid? lancamentoId = null) => new(
        (lancamentoId ?? Guid.NewGuid()).ToString());

    public ConsolidarDiaCommand BuildConsolidarDiaCommand(DateOnly? data = null)
        => new(data ?? DateOnly.FromDateTime(DateTime.UtcNow));
}