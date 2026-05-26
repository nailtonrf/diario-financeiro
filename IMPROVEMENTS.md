# 🚀 Plano de Melhorias - Diario Financeiro

Documento com tópicos estratégicos para melhorar a qualidade, confiabilidade e performance da aplicação.

---

## 1. 🔁 Idempotência

### Problema Atual
Operações como crédito, débito e estorno precisam ser idempotentes para evitar duplicação de lançamentos em caso de:
- Requisições duplicadas do cliente
- Retry automático em falhas de rede
- Processamento de mensagens RabbitMQ com falhas

### Soluções Propostas

#### 1.1 Implementar Idempotency Key
```csharp
// Adicionar campo opcional nas Commands
public sealed record CreditarCommand(
    Guid? IdempotencyKey,  // Cliente fornece chave única
    decimal Valor,
    string Descricao);

// No Handler, verificar se já foi processado
public async ValueTask<Result<CreditoEfetuadoEvent>> InteractAsync(
    CreditarCommand creditar,
    CancellationToken cancellationToken)
{
    // Buscar transação existente com mesma chave
    var jaProcessado = await lancamentoStore
        .GetByIdempotencyKeyAsync(creditar.IdempotencyKey, cancellationToken);
    
    if (jaProcessado.IsSome)
        return jaProcessado.Value;
    
    // Continuar com processamento...
}
```

#### 1.2 Event Sourcing com Deduplicação
- Armazenar `IdempotencyKey` junto ao evento
- Consultar antes de processar novos eventos
- Implementar índice único em `IdempotencyKey`

#### 1.3 Versionamento de Eventos
```csharp
public abstract record Lancamento(
    LancamentoId IdLancamento,
    Guid IdempotencyKey,    // Nova propriedade
    int EventVersion,        // Versão do evento
    DateTime CriadoEm);
```

### Benefícios
✅ Evita lançamentos duplicados  
✅ Permite retry seguro  
✅ Melhora confiabilidade em falhas de rede  
✅ Cumpre requisitos de sistemas financeiros

---

## 2. ✅ Testes Unitários

### Problema Atual
O projeto não possui testes unitários para a lógica de negócio (Core).

### Soluções Propostas

#### 2.1 Estrutura de Testes
```bash
src/
├── Fluxo.Lancamentos.Service/
├── Fluxo.Lancamentos.Service.Tests/          # NOVO
│   ├── Core/
│   │   ├── CreditarDeciderTests.cs
│   │   ├── DebitarDeciderTests.cs
│   │   ├── EstornarDeciderTests.cs
│   │   └── ConsolidarDiaDeciderTests.cs
│   ├── Shell/
│   │   ├── Handlers/
│   │   │   ├── CreditarInteractorTests.cs
│   │   │   └── DebitarInteractorTests.cs
│   │   └── Stores/
│   │       └── LancamentoStoreTests.cs
│   └── Fluxo.Lancamentos.Service.Tests.csproj
```

#### 2.2 Exemplo de Teste Unitário - Decider
```csharp
public class CreditarDeciderTests
{
    [Fact]
    public void Decide_ValidaDataCompetencia_RetornaOkQuandoValida()
    {
        // Arrange
        var competencia = new Competencia(DateOnly.FromDateTime(DateTime.UtcNow));
        var command = new CreditarCommand(
            IdempotencyKey: Guid.NewGuid(),
            Valor: 100m,
            Descricao: "Crédito teste");
        
        // Act
        var result = CreditarDecider.Decide(
            DateTime.UtcNow,
            competencia,
            command);
        
        // Assert
        Assert.True(result.IsOk);
        Assert.IsType<CreditoEfetuadoEvent>(result.Value);
        Assert.Equal(100m, result.Value.Valor);
    }

    [Fact]
    public void Decide_ValorNegativo_RetornaErro()
    {
        // Arrange
        var competencia = new Competencia(DateOnly.FromDateTime(DateTime.UtcNow));
        var command = new CreditarCommand(
            IdempotencyKey: Guid.NewGuid(),
            Valor: -100m,  // Inválido
            Descricao: "Crédito inválido");
        
        // Act
        var result = CreditarDecider.Decide(
            DateTime.UtcNow,
            competencia,
            command);
        
        // Assert
        Assert.False(result.IsOk);
        Assert.IsType<ErrorResult>(result.Error);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-50)]
    [InlineData(-1000)]
    public void Decide_ValoresInvalidos_RetornaErro(decimal valor)
    {
        // Arrange
        var competencia = new Competencia(DateOnly.FromDateTime(DateTime.UtcNow));
        var command = new CreditarCommand(Guid.NewGuid(), valor, "Teste");
        
        // Act
        var result = CreditarDecider.Decide(DateTime.UtcNow, competencia, command);
        
        // Assert
        Assert.False(result.IsOk);
    }
}
```

#### 2.3 Exemplo de Teste Unitário - Interactor (com Mock)
```csharp
public class CreditarInteractorTests
{
    [Fact]
    public async Task InteractAsync_ComDadosValidos_RetornaSucesso()
    {
        // Arrange
        var mockDataContext = new Mock<ILancamentosDataContext>();
        var mockCompetenciaStore = new Mock<ICompetenciaStore>();
        var mockLancamentoStore = new Mock<ILancamentoStore>();
        
        var competencia = new Competencia(DateOnly.FromDateTime(DateTime.UtcNow));
        mockCompetenciaStore
            .Setup(x => x.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(competencia);
        
        mockLancamentoStore
            .Setup(x => x.AppendAsync(It.IsAny<Lancamento>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Ok(new CreditoEfetuadoEvent(
                Guid.NewGuid(),
                Guid.NewGuid(),
                DateOnly.FromDateTime(DateTime.UtcNow),
                100m,
                "Teste",
                DateTime.UtcNow,
                0)));
        
        var interactor = new CreditarInteractor(
            mockDataContext.Object,
            mockCompetenciaStore.Object,
            mockLancamentoStore.Object);
        
        var command = new CreditarCommand(Guid.NewGuid(), 100m, "Crédito teste");
        
        // Act
        var result = await interactor.InteractAsync(command, CancellationToken.None);
        
        // Assert
        Assert.True(result.IsOk);
        mockLancamentoStore.Verify(x => 
            x.AppendAsync(It.IsAny<Lancamento>(), It.IsAny<CancellationToken>()), 
            Times.Once);
        mockDataContext.Verify(x => 
            x.SaveChangesAsync(It.IsAny<CancellationToken>()), 
            Times.Once);
    }
}
```

#### 2.4 Framework e Ferramentas Recomendadas
```xml
<ItemGroup>
    <PackageReference Include="xunit" Version="2.7.0" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.5.7" />
    <PackageReference Include="Moq" Version="4.20.70" />
    <PackageReference Include="FluentAssertions" Version="6.12.0" />
    <PackageReference Include="Bogus" Version="35.6.0" />  <!-- Gerador de dados de teste -->
</ItemGroup>
```

#### 2.5 Cobertura de Testes
- **Target:** Mínimo 80% de cobertura no Core
- **Ferramentas:** 
  - `coverlet` para medir cobertura
  - `ReportGenerator` para relatórios HTML

```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

### Benefícios
✅ Confiança na lógica de negócio  
✅ Documentação viva do comportamento esperado  
✅ Facilita refatoração segura  
✅ Reduz bugs em produção

---

## 3. 🧪 Testes de Integração

### Problema Atual
Faltam testes que validam a integração entre camadas (Handlers, Stores, Banco de Dados).

### Soluções Propostas

#### 3.1 Estrutura de Testes de Integração
```bash
src/
├── Fluxo.Lancamentos.Service.IntegrationTests/  # NOVO
│   ├── Fixtures/
│   │   ├── LancamentosDbContextFixture.cs       # Setup BD para testes
│   │   └── RabbitMqFixture.cs                   # Setup RabbitMQ para testes
│   ├── Endpoints/
│   │   ├── CreditarEndpointTests.cs
│   │   └── DebitarEndpointTests.cs
│   ├── Stores/
│   │   ├── LancamentoStoreIntegrationTests.cs
│   │   └── CompetenciaStoreIntegrationTests.cs
│   └── Fluxo.Lancamentos.Service.IntegrationTests.csproj
```

#### 3.2 Testcontainers para Infraestrutura
```csharp
public class LancamentosDbContextFixture : IAsyncLifetime
{
    private readonly PostgresContainer _postgres;
    public LancamentosDbContext DbContext { get; private set; }
    
    public LancamentosDbContextFixture()
    {
        _postgres = new PostgresBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("fluxodb_test")
            .WithUsername("testuser")
            .WithPassword("testpass")
            .Build();
    }
    
    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        
        var connectionString = _postgres.GetConnectionString();
        var options = new DbContextOptionsBuilder<LancamentosDbContext>()
            .UseNpgsql(connectionString)
            .Options;
        
        DbContext = new LancamentosDbContext(options);
        await DbContext.Database.MigrateAsync();
    }
    
    public async Task DisposeAsync()
    {
        await _postgres.StopAsync();
        await _postgres.DisposeAsync();
    }
}
```

#### 3.3 Exemplo de Teste de Integração - Endpoint
```csharp
public class CreditarEndpointTests : IAsyncLifetime
{
    private WebApplication _app;
    private readonly LancamentosDbContextFixture _dbFixture;
    private HttpClient _httpClient;
    
    public CreditarEndpointTests()
    {
        _dbFixture = new LancamentosDbContextFixture();
    }
    
    public async Task InitializeAsync()
    {
        await _dbFixture.InitializeAsync();
        
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddScoped(_ => _dbFixture.DbContext);
        builder.Services.UseLancamentos(builder.Configuration);
        
        _app = builder.Build();
        _app.UseLancamentosEndpoints();
        
        _httpClient = new HttpClient { BaseAddress = new Uri("http://localhost") };
    }
    
    [Fact]
    public async Task Post_CreditarComDadosValidos_Retorna201Created()
    {
        // Arrange
        var command = new CreditarCommand(
            IdempotencyKey: Guid.NewGuid(),
            Valor: 100m,
            Descricao: "Crédito de teste");
        
        var content = new StringContent(
            JsonSerializer.Serialize(command),
            Encoding.UTF8,
            "application/json");
        
        // Act
        var response = await _httpClient.PostAsync("/lancamentos/creditar", content);
        
        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var responseContent = await response.Content.ReadAsAsync<CreditoEfetuadoEvent>();
        Assert.Equal(100m, responseContent.Valor);
    }
    
    [Fact]
    public async Task Post_CreditarComValorNegativo_Retorna400BadRequest()
    {
        // Arrange
        var command = new CreditarCommand(
            IdempotencyKey: Guid.NewGuid(),
            Valor: -100m,
            Descricao: "Crédito inválido");
        
        var content = new StringContent(
            JsonSerializer.Serialize(command),
            Encoding.UTF8,
            "application/json");
        
        // Act
        var response = await _httpClient.PostAsync("/lancamentos/creditar", content);
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    public async Task DisposeAsync()
    {
        _app?.Dispose();
        await _dbFixture.DisposeAsync();
    }
}
```

#### 3.4 Teste de Integração com RabbitMQ
```csharp
public class LancamentoPublishingIntegrationTests : IAsyncLifetime
{
    private readonly RabbitMQContainer _rabbitMq;
    private IProducer _producer;
    
    public LancamentoPublishingIntegrationTests()
    {
        _rabbitMq = new RabbitMQBuilder()
            .WithImage("rabbitmq:3.13-alpine")
            .Build();
    }
    
    public async Task InitializeAsync()
    {
        await _rabbitMq.StartAsync();
        
        var connectionFactory = new ConnectionFactory
        {
            Uri = new Uri(_rabbitMq.GetConnectionString())
        };
        
        _producer = new Producer(connectionFactory);
    }
    
    [Fact]
    public async Task PublishCreditoEvent_ComEventoValido_PublicaComSucesso()
    {
        // Arrange
        var creditoEvent = new CreditoEfetuadoEvent(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.UtcNow),
            100m,
            "Crédito teste",
            DateTime.UtcNow,
            0);
        
        // Act & Assert
        var exception = await Record.ExceptionAsync(async () =>
            await _producer.PublishAsync(creditoEvent, CancellationToken.None));
        
        Assert.Null(exception);
    }
    
    public async Task DisposeAsync()
    {
        await _rabbitMq.StopAsync();
        await _rabbitMq.DisposeAsync();
    }
}
```

#### 3.5 Packages Necessários
```xml
<ItemGroup>
    <PackageReference Include="Testcontainers" Version="3.7.0" />
    <PackageReference Include="Testcontainers.PostgreSQL" Version="3.7.0" />
    <PackageReference Include="Testcontainers.RabbitMQ" Version="3.7.0" />
    <PackageReference Include="xunit" Version="2.7.0" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.5.7" />
</ItemGroup>
```

### Benefícios
✅ Valida integração entre camadas  
✅ Testa fluxo completo da requisição  
✅ Detecta problemas de configuração  
✅ Garante que BD, Cache e Message Queue funcionam

---

## 4. 🏗️ Testes de Arquitetura

### Problema Atual
Não há validação automática de conformidade arquitetural (regras de dependências entre camadas).

### Soluções Propostas

#### 4.1 ArchUnit para Validar Camadas
```csharp
public class ArchitectureTests
{
    [Fact]
    public void Core_ShouldNotDependOnShell()
    {
        var coreTypes = Types.InNamespace("Fluxo.Lancamentos.Service.Core");
        var shellTypes = Types.InNamespace("Fluxo.Lancamentos.Service.Shell");
        
        var rule = coreTypes
            .Should()
            .NotDependOnAny(shellTypes.GetTypes())
            .Because("Core deve ser independente de Shell");
        
        rule.Check();
    }

    [Fact]
    public void Shell_ShouldDependOnCore()
    {
        var shellTypes = Types.InNamespace("Fluxo.Lancamentos.Service.Shell");
        var coreTypes = Types.InNamespace("Fluxo.Lancamentos.Service.Core");
        
        var rule = shellTypes
            .Should()
            .DependOnAny(coreTypes.GetTypes())
            .Because("Shell precisa usar Core");
        
        rule.Check();
    }

    [Fact]
    public void Interactors_ShouldImplementIInteractor()
    {
        var interactorTypes = Types
            .InNamespace("Fluxo.Lancamentos.Service.Shell.Handlers")
            .That()
            .HaveNameEndingWith("Interactor");
        
        var rule = interactorTypes
            .Should()
            .ImplementInterface(typeof(IInteractor<,>))
            .Because("Todo handler deve ser um Interactor");
        
        rule.Check();
    }

    [Fact]
    public void DatabaseClasses_ShouldNotBeUsedInCore()
    {
        var coreTypes = Types.InNamespace("Fluxo.Lancamentos.Service.Core");
        
        var rule = coreTypes
            .Should()
            .NotDependOn("Microsoft.EntityFrameworkCore")
            .Because("Core não deve conhecer detalhes de persistência");
        
        rule.Check();
    }

    [Fact]
    public void PublicClasses_ShouldHaveXmlComments()
    {
        var allPublicTypes = Types
            .InNamespace("Fluxo.Lancamentos.Service")
            .That()
            .ArePublic();
        
        var rule = allPublicTypes
            .Should()
            .HaveXmlDocumentation()
            .Because("Documentação melhora manutenibilidade");
        
        rule.Check();
    }
}
```

#### 4.2 NArchitecture para Verificação de Clean Architecture
```csharp
public class CleanArchitectureTests
{
    private readonly Architecture _architecture = new ArchLoader()
        .LoadAssemblies(typeof(Program).Assembly)
        .Build();
    
    [Fact]
    public void Domain_ShouldNotDependOnOtherLayers()
    {
        var domainClasses = _architecture.Layers
            .Where(l => l.Name.Contains("Core"))
            .SelectMany(l => l.Classes);
        
        var rule = domainClasses
            .Should()
            .NotDependOnAny(
                _architecture.Layers
                    .Where(l => l.Name.Contains("Infra") || l.Name.Contains("Shell"))
                    .SelectMany(l => l.Classes));
        
        rule.Check();
    }

    [Fact]
    public void InfrastructureShouldDependOnDomain()
    {
        var infraClasses = _architecture.Layers
            .Where(l => l.Name.Contains("Infra"))
            .SelectMany(l => l.Classes);
        
        var coreClasses = _architecture.Layers
            .Where(l => l.Name.Contains("Core"))
            .SelectMany(l => l.Classes);
        
        // Verificar se infraestrutura pode depender de Core
        var rule = infraClasses
            .Should()
            .DependOnAny(coreClasses)
            .Because("Infra deve implementar interfaces do Core");
        
        rule.Check();
    }
}
```

#### 4.3 Package References
```xml
<ItemGroup>
    <PackageReference Include="ArchUnitNET" Version="1.3.1" />
    <PackageReference Include="ArchUnitNET.xUnit" Version="1.3.1" />
    <PackageReference Include="xunit" Version="2.7.0" />
</ItemGroup>
```

### Benefícios
✅ Garante independência de camadas  
✅ Previne acoplamento indesejado  
✅ Documenta regras arquiteturais  
✅ Falha automaticamente em violações

---

## 5. 💾 Cache Strategy

### Problema Atual
Dados frequentemente consultados (saldos, competências) não são cacheados.

### Soluções Propostas

#### 5.1 Implementar IMemoryCache para Competência
```csharp
public sealed class CachedCompetenciaStore(
    ICompetenciaStore innerStore,
    IMemoryCache memoryCache) : ICompetenciaStore
{
    private const string CompetenciaKey = "competencia:current";
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromHours(1);
    
    public async Task<Option<Competencia>> GetAsync(CancellationToken cancellationToken)
    {
        if (memoryCache.TryGetValue(CompetenciaKey, out Option<Competencia> competencia))
            return competencia;
        
        var result = await innerStore.GetAsync(cancellationToken);
        
        if (result.IsSome)
            memoryCache.Set(CompetenciaKey, result, CacheExpiration);
        
        return result;
    }
    
    public async Task<Result<Unit>> SetAsync(Competencia competencia, CancellationToken cancellationToken)
    {
        var result = await innerStore.SetAsync(competencia, cancellationToken);
        
        if (result.IsOk)
            memoryCache.Set(CompetenciaKey, Some(competencia), CacheExpiration);
        else
            memoryCache.Remove(CompetenciaKey);
        
        return result;
    }
}
```

Registrar no DI:
```csharp
services
    .AddMemoryCache()
    .AddTransient<ICompetenciaStore>(sp =>
        new CachedCompetenciaStore(
            new CompetenciaStore(sp.GetRequiredService<LancamentosDbContext>()),
            sp.GetRequiredService<IMemoryCache>()));
```

#### 5.2 Cache Distribuído com Redis para Saldos
```csharp
public sealed class CachedSaldoStore(
    ISaldoStore innerStore,
    IDistributedCache distributedCache) : ISaldoStore
{
    private const string SaldoCacheKeyPrefix = "saldo:";
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(5);
    
    public async Task<IEnumerable<Saldo>> GetAllAsync(CancellationToken cancellationToken)
    {
        const string allSaldosKey = "saldo:all";
        
        var cached = await distributedCache.GetStringAsync(allSaldosKey, cancellationToken);
        if (!string.IsNullOrEmpty(cached))
            return JsonSerializer.Deserialize<IEnumerable<Saldo>>(cached) ?? [];
        
        var saldos = await innerStore.GetAllAsync(cancellationToken);
        
        await distributedCache.SetStringAsync(
            allSaldosKey,
            JsonSerializer.Serialize(saldos),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = CacheExpiration },
            cancellationToken);
        
        return saldos;
    }
    
    public async Task<Option<Saldo>> GetByDataAsync(DateOnly data, CancellationToken cancellationToken)
    {
        var cacheKey = $"{SaldoCacheKeyPrefix}{data:yyyy-MM-dd}";
        
        var cached = await distributedCache.GetStringAsync(cacheKey, cancellationToken);
        if (!string.IsNullOrEmpty(cached))
            return JsonSerializer.Deserialize<Saldo>(cached) is { } saldo
                ? Some(saldo)
                : None<Saldo>();
        
        var result = await innerStore.GetByDataAsync(data, cancellationToken);
        
        if (result.IsSome)
            await distributedCache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(result.Value),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = CacheExpiration },
                cancellationToken);
        
        return result;
    }
}
```

Registrar no DI:
```csharp
services
    .AddStackExchangeRedisCache(options =>
        options.Configuration = configuration.GetConnectionString("redis"))
    .AddTransient<ISaldoStore>(sp =>
        new CachedSaldoStore(
            new SaldoStore(sp.GetRequiredService<IMongoClient>()),
            sp.GetRequiredService<IDistributedCache>()));
```

#### 5.3 Invalidação de Cache
```csharp
public sealed class CacheInvalidationHandler(
    IDistributedCache cache) : IConsumer
{
    public async Task ConsumeAsync<T>(T message, CancellationToken cancellationToken) 
        where T : class
    {
        // Invalidar cache de saldos quando lançamento é criado
        if (message is CreditoEfetuadoEvent or DebitoEfetuadoEvent or EstornoEfetuadoEvent)
        {
            await cache.RemoveAsync("saldo:all", cancellationToken);
        }
    }
}
```

#### 5.4 Cache Warming (Pré-carregamento)
```csharp
public class CacheWarmingHostedService(
    ISaldoStore saldoStore,
    IMemoryCache memoryCache) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Warm up cache ao iniciar
        _ = await saldoStore.GetAllAsync(stoppingToken);
        
        // Re-warm cache a cada 1 hora
        var timer = new PeriodicTimer(TimeSpan.FromHours(1));
        
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                memoryCache.Remove("saldo:all");
                _ = await saldoStore.GetAllAsync(stoppingToken);
            }
            catch { }
        }
    }
}
```

Registrar:
```csharp
services.AddHostedService<CacheWarmingHostedService>();
```

#### 5.5 Packages Necessários
```xml
<ItemGroup>
    <PackageReference Include="Microsoft.Extensions.Caching.StackExchangeRedis" Version="8.0.0" />
    <PackageReference Include="StackExchange.Redis" Version="2.7.0" />
</ItemGroup>
```

#### 5.6 Estratégia de Cache por Nível

| Dados | Tipo | TTL | Estratégia |
|-------|------|-----|-----------|
| Competência | Memory | 1 hora | Write-through |
| Saldos | Redis | 5 minutos | Cache-aside |
| Lançamentos | Nenhum | - | Sem cache (dados em tempo real) |

### Benefícios
✅ Reduz carga no BD  
✅ Melhora performance de resposta  
✅ Reduz latência em consultas frequentes  
✅ Escalabilidade melhorada

---

## 6. 📊 Monitoramento e Observabilidade

### Adicionar

- **OpenTelemetry** para rastreamento distribuído
- **Prometheus** para métricas
- **Structured Logging** com Serilog

---

## 7. 🔐 Validações e Segurança

### Adicionar

- **FluentValidation** para DTOs
- **Rate Limiting** nas APIs
- **Encryption** para dados sensíveis
- **SQL Injection** prevention (já coberto com EF Core)

---

## 📋 Checklist de Implementação

- [ ] Implementar Idempotência com Idempotency Keys
- [ ] Criar projeto de Testes Unitários
- [ ] Criar projeto de Testes de Integração
- [ ] Implementar testes de Arquitetura
- [ ] Adicionar IMemoryCache para Competência
- [ ] Implementar Redis para cache distribuído
- [ ] Configurar invalidação de cache
- [ ] Adicionar OpenTelemetry
- [ ] Implementar Serilog estruturado
- [ ] Adicionar FluentValidation
- [ ] Implementar Rate Limiting
- [ ] Atingir 80% de cobertura de testes

---

## 📚 Referências

- [Entity Framework Testing Guide](https://docs.microsoft.com/en-us/ef/core/testing/)
- [xUnit Documentation](https://xunit.net/)
- [Testcontainers](https://www.testcontainers.org/)
- [ArchUnitNET](https://archunitnet.readthedocs.io/)
- [Microsoft Caching Guidance](https://docs.microsoft.com/en-us/dotnet/core/extensions/caching)
- [Redis Best Practices](https://redis.io/topics/client-side-caching)
