namespace Fluxo.UnitTesting.Core.Deciders;

using Fluxo.Lancamentos.Service.Core.Estornar;
using FluentAssertions;
using Fluxo.UnitTesting.Fixtures;

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

        var command = new CommandDataBuilder()
            .BuildEstornarCommand(idLancamentoOriginal);

        // Act
        var result = EstornarDecider.Decide(
            DateTime.UtcNow,
            competencia,
            lancamentoOriginal,
            command);

        // Assert
        result.IsOk.Should().BeTrue();
        result.Value.Should().BeOfType<EstornoEfetuadoEvent>();
        result.Value.Valor.Should().Be(100m);
    }

    [Fact]
    public void Decide_ReferenciaCorretamenteLancamentoOriginal()
    {
        // Arrange
        var idLancamentoOriginal = Guid.NewGuid();
        var competencia = CompetenciaDataBuilder.Default();
        var lancamentoOriginal = new EventDataBuilder()
            .WithIdLancamento(idLancamentoOriginal)
            .WithValor(50m)
            .BuildDebitoEfetuadoEvent();

        var command = new CommandDataBuilder()
            .BuildEstornarCommand(idLancamentoOriginal);

        // Act
        var result = EstornarDecider.Decide(
            DateTime.UtcNow,
            competencia,
            lancamentoOriginal,
            command);

        // Assert
        result.IsOk.Should().BeTrue();
        result.Value.IdEstornado.Should().Be(idLancamentoOriginal);
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

        var command = new CommandDataBuilder()
            .BuildEstornarCommand(idLancamentoOriginal);

        // Act
        var result = EstornarDecider.Decide(
            DateTime.UtcNow,
            competencia,
            lancamentoOriginal,
            command);

        // Assert
        result.IsOk.Should().BeTrue();
        result.Value.IdLancamento.Should().NotBe(idLancamentoOriginal);
        result.Value.IdLancamento.Should().NotBe(default);
    }

    [Fact]
    public void Decide_PreservaIdempotencyKey()
    {
        // Arrange
        var idempotencyKey = Guid.NewGuid();
        var idLancamentoOriginal = Guid.NewGuid();
        var competencia = CompetenciaDataBuilder.Default();
        var lancamentoOriginal = new EventDataBuilder()
            .WithIdLancamento(idLancamentoOriginal)
            .BuildCreditoEfetuadoEvent();

        var command = new CommandDataBuilder()
            .WithIdempotencyKey(idempotencyKey)
            .BuildEstornarCommand(idLancamentoOriginal);

        // Act
        var result = EstornarDecider.Decide(
            DateTime.UtcNow,
            competencia,
            lancamentoOriginal,
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
        var idLancamentoOriginal = Guid.NewGuid();
        var competencia = new CompetenciaDataBuilder()
            .WithData(data)
            .Build();
        var lancamentoOriginal = new EventDataBuilder()
            .WithIdLancamento(idLancamentoOriginal)
            .BuildCreditoEfetuadoEvent();

        var command = new CommandDataBuilder()
            .BuildEstornarCommand(idLancamentoOriginal);

        // Act
        var result = EstornarDecider.Decide(
            DateTime.UtcNow,
            competencia,
            lancamentoOriginal,
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
        var idLancamentoOriginal = Guid.NewGuid();
        var competencia = CompetenciaDataBuilder.Default();
        var lancamentoOriginal = new EventDataBuilder()
            .WithIdLancamento(idLancamentoOriginal)
            .BuildCreditoEfetuadoEvent();

        var command = new CommandDataBuilder()
            .BuildEstornarCommand(idLancamentoOriginal);

        // Act
        var result = EstornarDecider.Decide(
            dataCriacao,
            competencia,
            lancamentoOriginal,
            command);

        // Assert
        result.IsOk.Should().BeTrue();
        result.Value.CriadoEm.Should().Be(dataCriacao);
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

        var command = new CommandDataBuilder()
            .BuildEstornarCommand(idLancamentoOriginal);

        // Act
        var result = EstornarDecider.Decide(
            DateTime.UtcNow,
            competencia,
            lancamentoOriginal,
            command);

        // Assert
        result.IsOk.Should().BeTrue();
        result.Value.Valor.Should().Be(250m);
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

        var command = new CommandDataBuilder()
            .BuildEstornarCommand(idLancamentoOriginal);

        // Act
        var result = EstornarDecider.Decide(
            DateTime.UtcNow,
            competencia,
            lancamentoOriginal,
            command);

        // Assert
        result.IsOk.Should().BeTrue();
        result.Value.Valor.Should().Be(150m);
    }

    [Fact]
    public void Decide_EventoRetornadoTemVersionZero()
    {
        // Arrange
        var idLancamentoOriginal = Guid.NewGuid();
        var competencia = CompetenciaDataBuilder.Default();
        var lancamentoOriginal = new EventDataBuilder()
            .WithIdLancamento(idLancamentoOriginal)
            .BuildCreditoEfetuadoEvent();

        var command = new CommandDataBuilder()
            .BuildEstornarCommand(idLancamentoOriginal);

        // Act
        var result = EstornarDecider.Decide(
            DateTime.UtcNow,
            competencia,
            lancamentoOriginal,
            command);

        // Assert
        result.IsOk.Should().BeTrue();
        result.Value.EventVersion.Should().Be(0);
    }
}
