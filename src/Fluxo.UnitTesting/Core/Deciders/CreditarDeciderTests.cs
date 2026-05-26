namespace Fluxo.UnitTesting.Core.Deciders;

using Fluxo.Lancamentos.Service.Core.Creditar;
using Fixtures;

/// <summary>
/// Testes unitários para o CreditarDecider.
/// Valida a lógica de decisão para operações de crédito.
/// </summary>
public sealed class CreditarDeciderTests
{
    [Fact]
    public void Decide_ComDadosValidos_RetornaOk()
    {
        // Arrange
        var competencia = CompetenciaDataBuilder.Default();
        var command = new CommandDataBuilder()
            .WithValor(100m)
            .WithDescricao("Crédito válido")
            .BuildCreditarCommand();

        // Act
        var result = CreditarDecider.Decide(
            DateTime.UtcNow,
            competencia,
            command);

        // Assert
        result.IsOk.Should().BeTrue();
        result.Value.Should().BeOfType<CreditoEfetuadoEvent>();
        result.Value?.Valor.Should().Be(100m);
        result.Value?.Descricao.Should().Be("Crédito válido");
    }

    [Fact]
    public void Decide_ComValorZero_RetornaErro()
    {
        // Arrange
        var competencia = CompetenciaDataBuilder.Default();
        var command = new CommandDataBuilder()
            .WithValor(0m)
            .BuildCreditarCommand();

        // Act
        var result = CreditarDecider.Decide(
            DateTime.UtcNow,
            competencia,
            command);

        // Assert
        result.IsOk.Should().BeFalse();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    [InlineData(-0.01)]
    public void Decide_ComValorNegativo_RetornaErro(decimal valor)
    {
        // Arrange
        var competencia = CompetenciaDataBuilder.Default();
        var command = new CommandDataBuilder()
            .WithValor(valor)
            .BuildCreditarCommand();

        // Act
        var result = CreditarDecider.Decide(
            DateTime.UtcNow,
            competencia,
            command);

        // Assert
        result.IsOk.Should().BeFalse();
    }

    [Theory]
    [InlineData(0.01)]
    [InlineData(1)]
    [InlineData(1000)]
    [InlineData(999999.99)]
    public void Decide_ComValoresPositivosValidos_RetornaOk(decimal valor)
    {
        // Arrange
        var competencia = CompetenciaDataBuilder.Default();
        var command = new CommandDataBuilder()
            .WithValor(valor)
            .BuildCreditarCommand();

        // Act
        var result = CreditarDecider.Decide(
            DateTime.UtcNow,
            competencia,
            command);

        // Assert
        result.IsOk.Should().BeTrue();
        result.Value?.Valor.Should().Be(valor);
    }

    [Fact]
    public void Decide_PreencheIdLancamentoGerado()
    {
        // Arrange
        var competencia = CompetenciaDataBuilder.Default();
        var command = new CommandDataBuilder()
            .WithValor(50m)
            .BuildCreditarCommand();

        // Act
        var result = CreditarDecider.Decide(
            DateTime.UtcNow,
            competencia,
            command);

        // Assert
        result.IsOk.Should().BeTrue();
        result.Value?.IdLancamento.Should().NotBe(default);
    }

    [Fact]
    public void Decide_PreencheDataCompetencia()
    {
        // Arrange
        var data = DateOnly.FromDateTime(new DateTime(2026, 5, 26));
        var competencia = new CompetenciaDataBuilder()
            .WithData(data)
            .Build();
        var command = new CommandDataBuilder()
            .WithValor(100m)
            .BuildCreditarCommand();

        // Act
        var result = CreditarDecider.Decide(
            DateTime.UtcNow,
            competencia,
            command);

        // Assert
        result.IsOk.Should().BeTrue();
        result.Value?.DataCompetencia.Should().Be(data);
    }

    [Fact]
    public void Decide_PreencheDataCriacao()
    {
        // Arrange
        var dataCriacao = new DateTime(2026, 5, 26, 15, 30, 45);
        var competencia = CompetenciaDataBuilder.Default();
        var command = new CommandDataBuilder()
            .WithValor(100m)
            .BuildCreditarCommand();

        // Act
        var result = CreditarDecider.Decide(
            dataCriacao,
            competencia,
            command);

        // Assert
        result.IsOk.Should().BeTrue();
        result.Value?.Data.Should().Be(dataCriacao);
    }

    [Fact]
    public void Decide_ComDescricaoVazia_RetornaOk()
    {
        // Arrange
        var competencia = CompetenciaDataBuilder.Default();
        var command = new CommandDataBuilder()
            .WithValor(100m)
            .WithDescricao(string.Empty)
            .BuildCreditarCommand();

        // Act
        var result = CreditarDecider.Decide(
            DateTime.UtcNow,
            competencia,
            command);

        // Assert
        result.IsOk.Should().BeTrue();
        result.Value?.Descricao.Should().Be(string.Empty);
    }

    [Fact]
    public void Decide_ComDescricaoLonga_RetornaOk()
    {
        // Arrange
        var descricao = new string('A', 500);
        var competencia = CompetenciaDataBuilder.Default();
        var command = new CommandDataBuilder()
            .WithValor(100m)
            .WithDescricao(descricao)
            .BuildCreditarCommand();

        // Act
        var result = CreditarDecider.Decide(
            DateTime.UtcNow,
            competencia,
            command);

        // Assert
        result.IsOk.Should().BeTrue();
        result.Value?.Descricao.Should().Be(descricao);
    }
}