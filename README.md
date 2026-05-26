# diario-financeiro

Diário financeiro usando **Functional Core, Imperative Shell** (FCIS) com arquitetura de microsserviços em C# .NET.

## 📋 Padrões Utilizados

1. **Functional Core, Imperative Shell (FCIS)**
   - Separação clara entre lógica de negócio pura (functional core) e operações com efeitos colaterais (imperative shell)

2. **Arquitetura em Camadas**
   - `Fluxo.Abstractions` - Interfaces e abstrações
   - `Fluxo.Contracts` - Modelos de contrato (DTOs)
   - `Fluxo.Lancamentos.Service` - Serviço de transações
   - `Fluxo.Saldos.Service` - Serviço de saldos
   - `Fluxo.ServiceDefaults` - Configurações compartilhadas

3. **Microsserviços**
   - Decomposição em serviços especializados (Lançamentos e Saldos)
   - `Fluxo.AppHost` - Orquestração com .NET Aspire

4. **Separação de Responsabilidades**
   - Domain (regras de negócio)
   - Application (handlers, casos de uso)
   - Infrastructure (persistência, comunicação)
   - Contracts (comunicação entre serviços)

5. **Message-Driven Architecture**
   - Integração via RabbitMQ
   - Comunicação assíncrona entre serviços

---

## 🛠️ Como FCIS Ajuda na Manutenção

### **1. Testabilidade**
- **Core funcional puro** é testável sem dependências externas (sem mocks complexos)
- Testes unitários rápidos e determinísticos
- Testes de integração isolados na "shell imperativa"

### **2. Rastreabilidade de Efeitos**
- Fácil identificar onde ocorrem operações I/O, banco de dados, mensagens
- Código funcional é **previsível** - mesma entrada = mesma saída sempre
- Reduz bugs relacionados a estado compartilhado

### **3. Refatoração com Segurança**
- Pode refatorar a shell sem afetar a lógica de negócio
- Trocar RabbitMQ por outro message broker sem alterar o core
- Mudar Entity Framework por outro ORM com impacto mínimo

### **4. Clareza de Intenção**
- Fácil distinguir o que é "lógica pura" vs "efeitos colaterais"
- Novos desenvolvedores entendem rapidamente a arquitetura
- Reduz pontos de confusão e erros

### **5. Reusabilidade**
- O core funcional pode ser consumido de múltiplas formas (HTTP, Message Queue, gRPC)
- Lógica não fica acoplada a um único tipo de interface
- Facilita evolução para novos tipos de cliente

### **6. Performance e Debugging**
- Core funcional pode ser otimizado sem tocar na shell
- Efeitos colaterais isolados facilitam identificação de gargalos
- Mais fácil adicionar logs, métricas e tracing na shell

### **7. Escalabilidade**
- Com microsserviços (Lançamentos + Saldos), cada serviço pode escalar independentemente
- A separação funcional torna claro quais operações podem ser paralelizadas
- Facilita implementação de cache e otimizações

---

## ✅ Conclusão

A abordagem **FCIS em microsserviços** proporciona um código que é:
- **Testável** e confiável
- **Manutenível** e compreensível
- **Escalável** e resiliente
- **Evolutivo** - pronto para mudanças futuras

Ideal para sistemas financeiros onde a corretude da lógica é crítica! 💰

---

## 🚀 Getting Started

### Requisitos Necessários

**Software**
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download) ou superior
- [Git](https://git-scm.com/) (para clonar o repositório)
- [Docker](https://www.docker.com/) e Docker Compose (para os serviços de infraestrutura)

**Serviços de Infraestrutura (via Docker)**
- **PostgreSQL** - Banco de dados relacional (porta 5432)
- **MongoDB** - Banco de dados NoSQL (porta 27017)
- **RabbitMQ** - Message broker (porta 5672, management 15672)

---

### Passo a Passo para Executar

#### 1️⃣ Clonar o Repositório
```bash
git clone https://github.com/nailtonrf/diario-financeiro.git
cd diario-financeiro
```

#### 2️⃣ Configurar Variáveis de Ambiente
Crie um arquivo `.env` na raiz do projeto:

```env
# PostgreSQL
POSTGRES_USER=admin
POSTGRES_PASSWORD=senha123
POSTGRES_DB=fluxodb
POSTGRES_PORT=5432

# MongoDB
MONGODB_USER=admin
MONGODB_PASSWORD=senha123
MONGO_PORT=27017

# RabbitMQ
RABBITMQ_USER=guest
RABBITMQ_PASSWORD=guest
RABBITMQ_PORT=5672
RABBITMQ_MANAGEMENT_PORT=15672
```

#### 3️⃣ Navegar para o Diretório de Código
```bash
cd src
```

#### 4️⃣ Restaurar Dependências
```bash
dotnet restore
```

#### 5️⃣ Executar a Aplicação via .NET Aspire
```bash
dotnet run --project Fluxo.AppHost/Fluxo.AppHost.csproj
```

Isso irá:
- Inicializar os containers Docker (PostgreSQL, MongoDB, RabbitMQ)
- Aplicar migrations automaticamente
- Seeding de dados iniciais
- Iniciar ambos os microsserviços

#### 6️⃣ Acessar a Aplicação

Após iniciar, você terá acesso a:

| Serviço | URL | Descrição |
|---------|-----|-----------|
| **Fluxo.Lancamentos.Service** | `http://localhost:5000` | API de transações financeiras |
| **Fluxo.Saldos.Service** | `http://localhost:5001` | API de saldos |
| **Aspire Dashboard** | `http://localhost:18888` | Monitoramento em tempo real |
| **RabbitMQ Management** | `http://localhost:15672` | Gerenciamento de filas (guest/guest) |
| **Scalar API Docs** | `http://localhost:5000/scalar/v1` | Documentação das APIs |

---

### 🔍 Verificação Rápida

Para verificar se tudo está funcionando:

```bash
# Verificar .NET SDK
dotnet --version

# Verificar Docker
docker --version

# Listar containers em execução
docker ps
```

---

### 📝 Notas Importantes

1. **First Run**: Na primeira execução, as migrations serão aplicadas automaticamente
2. **Seeding**: Dados iniciais (competências) são criados automaticamente
3. **Docker**: Certifique-se de que o Docker daemon está rodando antes de iniciar
4. **Portas**: As portas padrão (5000, 5001, 5432, 27017, 5672, 15672) precisam estar livres

---

### 🛑 Parar a Aplicação

```bash
# No terminal onde está rodando
Ctrl + C

# Opcionalmente, parar containers Docker
docker compose down
```
