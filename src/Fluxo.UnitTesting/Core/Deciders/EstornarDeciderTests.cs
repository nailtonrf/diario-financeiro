namespace Fluxo.UnitTesting.Core.Deciders;

using Fluxo.Lancamentos.Service.Core.Estornar;
using Fixtures;

/// <summary>
/// Testes unitários para o EstornarDecider.
/// Valida a lógica de decisão para operações de estorno.
/// </summary>
public sealed class EstornarDeciderTests
{
    [Fact]
    public void Decide_ComDadosValidos_RetornaOk()
    {
        // Arrange
        var idLancamentoOriginal = Guid.NewGuid();
        var competencia = CompetenciaDataBuilder.Default();
        var lancamentoOriginal = new EventDataBuilder()
            .WithIdLancamento(idLancamentoOriginal)
            .WithValor(100m)
            .WithDescricao("Crédito original")
            .BuildCreditoEfetuadoEvent();

        // Act
        var result = EstornarDecider.Decide(
            DateTime.UtcNow,
            competencia,
            lancamentoOriginal);

        // Assert
        result.IsOk.Should().BeTrue();
        result.Value.Should().BeOfType<EstornoEfetuadoEvent>();
        result.Value?.Valor.Should().Be(-100m);
    }

    [Fact]
    public void Decide_GeraNovoIdLancamento()
    {
        // Arrange
        var idLancamentoOriginal = Guid.NewGuid();
        var competencia = CompetenciaDataBuilder.Default();
        var lancamentoOriginal = new EventDataBuilder()
            .WithIdLancamento(idLancamentoOriginal)
            .BuildCreditoEfetuadoEvent();

        // Act
        var result = EstornarDecider.Decide(
            DateTime.UtcNow,
            competencia,
            lancamentoOriginal);

        // Assert
        result.IsOk.Should().BeTrue();
        result.Value?.IdLancamento.Should().NotBe(idLancamentoOriginal);
        result.Value?.IdLancamento.Should().NotBe(null);
    }

    [Fact]
    public void Decide_PreencheDataCompetencia()
    {
        // Arrange
        var data = DateOnly.FromDateTime(new DateTime(2026, 5, 26));
        var idLancamentoOriginal = Guid.NewGuid();
        var competencia = new CompetenciaDataBuilder()
            .WithData(data)
            .Build();
        var lancamentoOriginal = new EventDataBuilder()
            .WithIdLancamento(idLancamentoOriginal)
            .BuildCreditoEfetuadoEvent();

        // Act
        var result = EstornarDecider.Decide(
            DateTime.UtcNow,
            competencia,
            lancamentoOriginal);

        // Assert
        result.IsOk.Should().BeTrue();
        result.Value?.DataCompetencia.Should().Be(data);
    }

    [Fact]
    public void Decide_PreencheDataCriacao()
    {
        // Arrange
        var dataCriacao = new DateTime(2026, 5, 26, 15, 30, 45);
        var idLancamentoOriginal = Guid.NewGuid();
        var competencia = CompetenciaDataBuilder.Default();
        var lancamentoOriginal = new EventDataBuilder()
            .WithIdLancamento(idLancamentoOriginal)
            .BuildCreditoEfetuadoEvent();

        // Act
        var result = EstornarDecider.Decide(
            dataCriacao,
            competencia,
            lancamentoOriginal);

        // Assert
        result.IsOk.Should().BeTrue();
        result.Value?.Data.Should().Be(dataCriacao);
    }

    [Fact]
    public void Decide_ComValorDeCredito_RetornaOk()
    {
        // Arrange
        var idLancamentoOriginal = Guid.NewGuid();
        var competencia = CompetenciaDataBuilder.Default();
        var lancamentoOriginal = new EventDataBuilder()
            .WithIdLancamento(idLancamentoOriginal)
            .WithValor(250m)
            .BuildCreditoEfetuadoEvent();

        // Act
        var result = EstornarDecider.Decide(
            DateTime.UtcNow,
            competencia,
            lancamentoOriginal);

        // Assert
        result.IsOk.Should().BeTrue();
        result.Value?.Valor.Should().Be(-250m);
    }

    [Fact]
    public void Decide_ComValorDeDebito_RetornaOk()
    {
        // Arrange
        var idLancamentoOriginal = Guid.NewGuid();
        var competencia = CompetenciaDataBuilder.Default();
        var lancamentoOriginal = new EventDataBuilder()
            .WithIdLancamento(idLancamentoOriginal)
            .WithValor(150m)
            .BuildDebitoEfetuadoEvent();

        // Act
        var result = EstornarDecider.Decide(
            DateTime.UtcNow,
            competencia,
            lancamentoOriginal);

        // Assert
        result.IsOk.Should().BeTrue();
        result.Value?.Valor.Should().Be(150m);
    }
}