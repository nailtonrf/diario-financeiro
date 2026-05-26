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
