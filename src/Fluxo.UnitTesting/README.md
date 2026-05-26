# Fluxo.UnitTesting 🧪

Projeto de testes unitários para a aplicação Fluxo Diário Financeiro utilizando **xUnit** e **C# 14** com **.NET 10**.

## 📁 Estrutura

```
Fluxo.UnitTesting/
├── Fixtures/
│   ├── CompetenciaDataBuilder.cs    # Builder para Competência
│   ├── CommandDataBuilder.cs         # Builder para Commands
│   └── EventDataBuilder.cs           # Builder para Eventos
├── Core/
│   └── Deciders/
│       ├── CreditarDeciderTests.cs
│       ├── DebitarDeciderTests.cs
│       └── EstornarDeciderTests.cs
└── README.md
```

## 🚀 Como Executar

### Todos os testes

```bash
cd src/Fluxo.UnitTesting
dotnet test
```

### Testes específicos

```bash
# Apenas testes de Crédito
dotnet test --filter "FullyQualifiedName~CreditarDeciderTests"

# Apenas testes de Débito
dotnet test --filter "FullyQualifiedName~DebitarDeciderTests"

# Apenas testes de Estorno
dotnet test --filter "FullyQualifiedName~EstornarDeciderTests"
```

### Com Cobertura de Código

```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:Exclude="[*]*.Program"
```

## 📦 Dependências

- **xUnit** 2.7.0 - Framework de testes
- **Moq** 4.20.70 - Mocking
- **FluentAssertions** 6.12.0 - Assertions fluentes
- **Bogus** 35.6.0 - Geração de dados de teste

## ✅ Cobertura de Testes

### CreditarDecider

- ✅ Crédito com dados válidos
- ✅ Valor zero (erro)
- ✅ Valor negativo (erro)
- ✅ Valores positivos válidos (Theory)
- ✅ ID lançamento gerado
- ✅ Preservação de Idempotency Key
- ✅ Preenchimento de data de competência
- ✅ Preenchimento de data de criação
- ✅ Descrição vazia
- ✅ Descrição longa
- ✅ Versão do evento

### DebitarDecider

- ✅ Débito com dados válidos
- ✅ Valor zero (erro)
- ✅ Valor negativo (erro)
- ✅ Valores positivos válidos (Theory)
- ✅ ID lançamento gerado
- ✅ Preservação de Idempotency Key
- ✅ Preenchimento de data de competência
- ✅ Preenchimento de data de criação
- ✅ Descrição vazia
- ✅ Descrição longa
- ✅ Versão do evento

### EstornarDecider

- ✅ Estorno com dados válidos
- ✅ Referenciamento correto de lançamento original
- ✅ Geração de novo ID de lançamento
- ✅ Preservação de Idempotency Key
- ✅ Preenchimento de data de competência
- ✅ Preenchimento de data de criação
- ✅ Estorno de crédito
- ✅ Estorno de débito
- ✅ Versão do evento

## 🔨 Builders Disponíveis

### CompetenciaDataBuilder

```csharp
var competencia = new CompetenciaDataBuilder()
    .WithData(DateOnly.FromDateTime(DateTime.UtcNow))
    .Build();

// Atalho para padrão
var competenciaDefault = CompetenciaDataBuilder.Default();
```

### CommandDataBuilder

```csharp
var creditCommand = new CommandDataBuilder()
    .WithValor(100m)
    .WithDescricao("Crédito de teste")
    .WithIdempotencyKey(Guid.NewGuid())
    .BuildCreditarCommand();

var debitCommand = new CommandDataBuilder()
    .WithValor(50m)
    .BuildDebitarCommand();

var revertCommand = new CommandDataBuilder()
    .BuildEstornarCommand(lancamentoId);
```

### EventDataBuilder

```csharp
var creditEvent = new EventDataBuilder()
    .WithValor(100m)
    .WithDescricao("Evento de teste")
    .BuildCreditoEfetuadoEvent();

var debitEvent = new EventDataBuilder()
    .WithValor(75m)
    .BuildDebitoEfetuadoEvent();

var revertEvent = new EventDataBuilder()
    .WithIdEstornado(originalId)
    .BuildEstornoEfetuadoEvent();
```

## 📊 Padrões de Teste Utilizados

### Arrange-Act-Assert (AAA)

Todos os testes seguem o padrão AAA para máxima clareza:

```csharp
[Fact]
public void Decide_ComDadosValidos_RetornaOk()
{
    // Arrange - Setup dos dados de teste
    var competencia = CompetenciaDataBuilder.Default();
    var command = new CommandDataBuilder().WithValor(100m).BuildCreditarCommand();

    // Act - Executa a ação
    var result = CreditarDecider.Decide(DateTime.UtcNow, competencia, command);

    // Assert - Valida o resultado
    result.IsOk.Should().BeTrue();
    result.Value.Valor.Should().Be(100m);
}
```

### Theory Tests

Para validar múltiplos valores com mesmo comportamento esperado:

```csharp
[Theory]
[InlineData(0.01)]
[InlineData(1)]
[InlineData(1000)]
public void Decide_ComValoresPositivosValidos_RetornaOk(decimal valor)
{
    // ...
}
```

## 🎯 Próximas Melhorias

- [ ] Testes para ConsolidarDiaDecider
- [ ] Testes de Interactors
- [ ] Testes de Stores
- [ ] Testes de Integração
- [ ] Testes de Arquitetura

## 📝 Referências

- [xUnit Documentation](https://xunit.net/)
- [FluentAssertions](https://fluentassertions.com/)
- [Moq Documentation](https://github.com/moq/moq4/wiki/Quickstart)
- [Bogus - Fake Data Generator](https://github.com/bchavez/Bogus)
