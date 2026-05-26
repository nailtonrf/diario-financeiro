namespace Fluxo.UnitTesting.Core.Deciders;

using Fluxo.Lancamentos.Service.Core.Debitar;
using FluentAssertions;
using Fluxo.UnitTesting.Fixtures;

/// <summary>
/// Testes unitários para o DebitarDecider.
/// Valida a lógica de decisão para operações de débito.
/// </summary>
public sealed class DebitarDeciderTests
{
    [Fact]
    public void Decide_ComDadosValidos_RetornaOk()
    {
        // Arrange
        var competencia = CompetenciaDataBuilder.Default();
        var command = new CommandDataBuilder()
            .WithValor(100m)
            .WithDescricao("Débito válido")
            .BuildDebitarCommand();

        // Act
        var result = DebitarDecider.Decide(
            DateTime.UtcNow,
            competencia,
            command);

        // Assert
        result.IsOk.Should().BeTrue();
        result.Value.Should().BeOfType<DebitoEfetuadoEvent>();
        result.Value.Valor.Should().Be(100m);
        result.Value.Descricao.Should().Be("Débito válido");
    }

    [Fact]
    public void Decide_ComValorZero_RetornaErro()
    {
        // Arrange
        var competencia = CompetenciaDataBuilder.Default();
        var command = new CommandDataBuilder()
            .WithValor(0m)
            .BuildDebitarCommand();

        // Act
        var result = DebitarDecider.Decide(
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
            .BuildDebitarCommand();

        // Act
        var result = DebitarDecider.Decide(
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
            .BuildDebitarCommand();

        // Act
        var result = DebitarDecider.Decide(
            DateTime.UtcNow,
            competencia,
            command);

        // Assert
        result.IsOk.Should().BeTrue();
        result.Value.Valor.Should().Be(valor);
    }

    [Fact]
    public void Decide_PreencheIdLancamentoGerado()
    {
        // Arrange
        var competencia = CompetenciaDataBuilder.Default();
        var command = new CommandDataBuilder()
            .WithValor(50m)
            .BuildDebitarCommand();

        // Act
        var result = DebitarDecider.Decide(
            DateTime.UtcNow,
            competencia,
            command);

        // Assert
        result.IsOk.Should().BeTrue();
        result.Value.IdLancamento.Should().NotBe(default);
    }

    [Fact]
    public void Decide_PreservaIdempotencyKey()
    {
        // Arrange
        var idempotencyKey = Guid.NewGuid();
        var competencia = CompetenciaDataBuilder.Default();
        var command = new CommandDataBuilder()
            .WithValor(75m)
            .WithIdempotencyKey(idempotencyKey)
            .BuildDebitarCommand();

        // Act
        var result = DebitarDecider.Decide(
            DateTime.UtcNow,
            competencia,
            command);

        // Assert
        result.IsOk.Should().BeTrue();
        result.Value.IdempotencyKey.Should().Be(idempotencyKey);
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
            .BuildDebitarCommand();

        // Act
        var result = DebitarDecider.Decide(
            DateTime.UtcNow,
            competencia,
            command);

        // Assert
        result.IsOk.Should().BeTrue();
        result.Value.DataCompetencia.Should().Be(data);
    }

    [Fact]
    public void Decide_PreencheDataCriacao()
    {
        // Arrange
        var dataCriacao = new DateTime(2026, 5, 26, 15, 30, 45);
        var competencia = CompetenciaDataBuilder.Default();
        var command = new CommandDataBuilder()
            .WithValor(100m)
            .BuildDebitarCommand();

        // Act
        var result = DebitarDecider.Decide(
            dataCriacao,
            competencia,
            command);

        // Assert
        result.IsOk.Should().BeTrue();
        result.Value.CriadoEm.Should().Be(dataCriacao);
    }

    [Fact]
    public void Decide_ComDescricaoVazia_RetornaOk()
    {
        // Arrange
        var competencia = CompetenciaDataBuilder.Default();
        var command = new CommandDataBuilder()
            .WithValor(100m)
            .WithDescricao(string.Empty)
            .BuildDebitarCommand();

        // Act
        var result = DebitarDecider.Decide(
            DateTime.UtcNow,
            competencia,
            command);

        // Assert
        result.IsOk.Should().BeTrue();
        result.Value.Descricao.Should().Be(string.Empty);
    }

    [Fact]
    public void Decide_ComDescricaoLonga_RetornaOk()
    {
        // Arrange
        var descricao = new string('B', 500);
        var competencia = CompetenciaDataBuilder.Default();
        var command = new CommandDataBuilder()
            .WithValor(100m)
            .WithDescricao(descricao)
            .BuildDebitarCommand();

        // Act
        var result = DebitarDecider.Decide(
            DateTime.UtcNow,
            competencia,
            command);

        // Assert
        result.IsOk.Should().BeTrue();
        result.Value.Descricao.Should().Be(descricao);
    }

    [Fact]
    public void Decide_EventoRetornadoTemVersionZero()
    {
        // Arrange
        var competencia = CompetenciaDataBuilder.Default();
        var command = new CommandDataBuilder()
            .WithValor(100m)
            .BuildDebitarCommand();

        // Act
        var result = DebitarDecider.Decide(
            DateTime.UtcNow,
            competencia,
            command);

        // Assert
        result.IsOk.Should().BeTrue();
        result.Value.EventVersion.Should().Be(0);
    }
}
