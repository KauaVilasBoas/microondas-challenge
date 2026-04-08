# Micro-ondas Digital — Challenge

Simulador de micro-ondas digital construído em ASP.NET Core 8, demonstrando **Clean Architecture**, **Domain-Driven Design (DDD)**, **CQRS**, eventos de domínio, **SignalR** para atualizações em tempo real e uma **REST API** com autenticação JWT.

> This is a challenge by [Coodesh](https://coodesh.com/)

---

## Índice

1. [A Ideia: Simulador de Micro-ondas](#1-a-ideia-simulador-de-micro-ondas)
2. [Regras de Negócio](#2-regras-de-negócio)
3. [Pré-requisitos](#3-pré-requisitos)
4. [Como Rodar](#4-como-rodar)
5. [Arquitetura Geral](#5-arquitetura-geral)
6. [Camadas do Projeto](#6-camadas-do-projeto)
7. [Decisões Técnicas Detalhadas](#7-decisões-técnicas-detalhadas)
8. [Por que Apenas Uma Tabela no Banco?](#8-por-que-apenas-uma-tabela-no-banco)
9. [Por que a Sessão Fica em Memória?](#9-por-que-a-sessão-fica-em-memória)
10. [Por que Singleton, Scoped e Transient?](#10-por-que-singleton-scoped-e-transient)
11. [Fluxo Completo de uma Requisição](#11-fluxo-completo-de-uma-requisição)
12. [SignalR e Tempo Real](#12-signalr-e-tempo-real)
13. [API REST e Autenticação JWT](#13-api-rest-e-autenticação-jwt)
14. [Testes](#14-testes)
15. [Estrutura de Pastas](#15-estrutura-de-pastas)
16. [Tecnologias](#16-tecnologias)

---

## 1. A Ideia: Simulador de Micro-ondas

O projeto simula o painel de controle de um **micro-ondas digital**. A ideia central é modelar fielmente o comportamento de um aparelho físico:

- Você define **tempo** (em segundos) e **potência** (1–10)
- Ao iniciar, o micro-ondas começa a **contar regressivamente**, exibindo um **display visual** que se preenche com um caractere representando o aquecimento — por exemplo, `.` para o padrão, ou `p` para pipoca
- A cada segundo, o display acumula segmentos separados por espaços — `... ... ... ...` — como no display LCD de um micro-ondas real
- O aparelho pode ser **pausado**, **retomado** ou **cancelado** (quando pausado)
- Enquanto roda, pressionar iniciar novamente adiciona **+30 segundos** — exatamente como um botão físico de micro-ondas
- Existem **programas pré-definidos** (Pipoca, Leite, Carnes, Frango, Feijão) com tempo, potência e caractere já configurados
- O usuário pode criar **programas customizados** com suas próprias configurações

O objetivo técnico é demonstrar como se modela um domínio com regras claras usando práticas modernas de desenvolvimento de software empresarial.

---

## 2. Regras de Negócio

### Aquecimento Manual

| Regra | Detalhe |
|---|---|
| Tempo válido | Entre 1 e 120 segundos |
| Potência válida | Entre 1 e 10 |
| Tempo padrão | 30 segundos (quando não informado) |
| Potência padrão | 10 (quando não informada) |
| Caractere padrão | `.` (ponto) |
| Display visual | `RenderSegment` = caractere repetido `potência` vezes. Ex: potência 3, char `.` → `...` |
| Acúmulo do display | Cada segundo adiciona um segmento separado por espaço: `... ... ...` |
| Botão iniciar (já rodando) | Adiciona 30 segundos ao tempo restante |
| Botão iniciar (pausado) | Retoma o aquecimento de onde parou |
| Botão pausar/cancelar (rodando) | Pausa |
| Botão pausar/cancelar (pausado) | Cancela e limpa a sessão |

### Máquina de Estados da Sessão

```
         ┌──────────────────────────────────────────────────────┐
         │                                                      │
         ▼                                                      │
      [Idle] ──── Iniciar ────► [Running] ──── Pausar ────► [Paused]
                                    │                           │
                                    │                           └── Cancelar ──► [Cancelled]
                                    │
                                    └── Tempo esgotado ──► [Completed]
```

### Regras de Programas

| Regra | Detalhe |
|---|---|
| Programas pré-definidos | 5 fixos, não podem ser excluídos |
| Programas customizados | Criados pelo usuário, podem ser excluídos |
| Caractere reservado | `.` (padrão) e os caracteres de todos os pré-definidos estão bloqueados |
| Caractere único | Cada programa deve ter um caractere exclusivo; duplicatas são rejeitadas |
| +30 segundos bloqueado | Não é permitido adicionar tempo a sessões iniciadas por programa |
| Nome | Obrigatório, máximo 50 caracteres |
| Alimento | Obrigatório, máximo 100 caracteres |
| Instruções | Opcional, máximo 500 caracteres |

### Programas Pré-Definidos

| Nome | Alimento | Tempo | Potência | Char |
|---|---|---|---|---|
| Pipoca | Pipoca | 3 min (180s) | 7 | `p` |
| Leite | Leite | 5 min (300s) | 5 | `l` |
| Carnes de boi | Carnes | 14 min (840s) | 4 | `c` |
| Frango | Frango | 8 min (480s) | 7 | `f` |
| Feijão | Feijão | 8 min (480s) | 9 | `j` |

---

## 3. Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/pt-br/sql-server/sql-server-downloads) (LocalDB, Express ou Developer)

---

## 4. Como Rodar

### 1. Clone o repositório

```bash
git clone https://github.com/seu-usuario/microondas-challenge.git
cd microondas-challenge
```

### 2. Configure a Connection String

Edite `src/Microondas.Web/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MicroondasDb;Trusted_Connection=True;"
  }
}
```

Para SQL Server Express:
```
Server=.\SQLEXPRESS;Database=MicroondasDb;Trusted_Connection=True;
```

### 3. Aplique as Migrations

```bash
dotnet ef database update \
  --project src/Microondas.Migrations \
  --startup-project src/Microondas.Web
```

Ou via Package Manager Console (Visual Studio):
```
Update-Database -Project Microondas.Migrations -StartupProject Microondas.Web
```

### 4. Rode o projeto Web (interface gráfica)

```bash
cd src/Microondas.Web
dotnet run
```

Acesse `https://localhost:<porta>` conforme exibido no terminal.

### 5. Rode o projeto API (opcional — REST + JWT)

```bash
cd src/Microondas.Api
dotnet run
```

Swagger UI disponível em `https://localhost:<porta>/swagger`

**Credenciais padrão:**

| Campo | Valor |
|---|---|
| Usuário | `admin` |
| Senha | `admin` |

Autentique-se:
```bash
POST /api/auth/login
Content-Type: application/json

{ "username": "admin", "password": "admin" }
```

Use o token retornado: `Authorization: Bearer <token>`

### 6. Rodando os testes

```bash
dotnet test
```

---

## 5. Arquitetura Geral

O projeto segue **Clean Architecture**. As dependências sempre apontam para dentro:

```
┌──────────────────────────────────────────────────────────────────────┐
│  Apresentação  (Microondas.Web / Microondas.Api)                     │
│  ┌─────────────────────────────────────────────────────────────────┐ │
│  │  Aplicação  (CommandHandlers / ReadModels / EventHandlers)      │ │
│  │  ┌──────────────────────────────────────────────────────────┐   │ │
│  │  │  Domínio  (Domain + Domain.Contracts)                    │   │ │
│  │  │  ┌───────────────────────────────────────────────────┐   │   │ │
│  │  │  │  SharedKernel  (Entity, ValueObject, Result, ...)  │   │   │ │
│  │  │  └───────────────────────────────────────────────────┘   │   │ │
│  │  └──────────────────────────────────────────────────────────┘   │ │
│  └─────────────────────────────────────────────────────────────────┘ │
│  Infraestrutura  (EF Core, SQL Server, SignalR, JWT)                  │
└──────────────────────────────────────────────────────────────────────┘
```

### Projetos da Solution

| Projeto | Camada | Responsabilidade |
|---|---|---|
| `Microondas.SharedKernel` | Núcleo | Base classes, Result\<T\>, Error, Guard |
| `Microondas.Domain` | Domínio | Agregados, Value Objects, Eventos, Serviços de Domínio |
| `Microondas.Domain.Contracts` | Contratos | Interfaces de Commands, Queries, Events e Repositórios |
| `Microondas.Application.CommandHandlers` | Aplicação | Handlers de Commands + Pipeline Behaviors |
| `Microondas.Application.ReadModels` | Aplicação | Handlers de Queries + DTOs de leitura |
| `Microondas.Application.EventHandlers` | Aplicação | Handlers de Domain Events (SignalR, logs) |
| `Microondas.Infrastructure` | Infraestrutura | EF Core, repositórios, serviços de sistema |
| `Microondas.Migrations` | Infraestrutura | Migrations EF Core isoladas do restante |
| `Microondas.Workers` | Infraestrutura | `BackgroundService` do timer de aquecimento |
| `Microondas.Web` | Apresentação | ASP.NET Core MVC + Razor Views + SignalR |
| `Microondas.Api` | Apresentação | REST API com autenticação JWT |

---

## 6. Camadas do Projeto

### SharedKernel — A Fundação

Contém os blocos construtivos usados por todas as camadas.

#### `Entity`

Classe base para toda entidade do domínio. Gera um `Guid` como identificador e implementa igualdade baseada em identidade: dois objetos com o mesmo `Id` são o mesmo objeto, independente dos demais campos.

#### `ValueObject`

Classe base para objetos de valor. Value objects **não têm identidade própria** — dois `PowerLevel(5)` são idênticos. A igualdade é baseada nos dados. São sempre imutáveis: campos definidos apenas no construtor.

#### `AggregateRoot`

Estende `Entity`. Adiciona a capacidade de **acumular eventos de domínio** internamente (`RaiseDomainEvent`) e limpá-los após o despacho (`ClearDomainEvents`). Os eventos não são publicados dentro do agregado — isso é responsabilidade da camada de aplicação, garantindo separação de concerns.

#### `Result<T>` e `Error` — Railway-Oriented Programming

Em vez de lançar exceções para erros de negócio, o projeto usa o padrão **Result**:

```csharp
// Em vez de:
throw new Exception("Potência deve ser entre 1 e 10");

// Usamos:
return Result.Failure(Error.Validation("PowerLevel.Invalid", "Potência deve ser entre 1 e 10"));
```

`Result<T>` encapsula ou um valor de sucesso ou um `Error` estruturado. Isso força o chamador a lidar com o erro explicitamente, tornando o fluxo de erros visível no tipo de retorno — sem try/catch espalhados.

`Error` é um `record` com `Code` (identificador da regra) e `Description` (mensagem). Os tipos de erro são: `Validation`, `NotFound`, `Conflict` e `Forbidden`.

#### `Guard`

Métodos estáticos de verificação defensiva (`Guard.AgainstNull`, `Guard.AgainstOutOfRange`, etc.). Lançam exceções técnicas — não de negócio — quando invariantes do sistema são violadas. Diferente do `Result` (que trata erros esperados de negócio), o `Guard` protege contra erros de programação.

---

### Domain — O Coração da Aplicação

#### `HeatingSession` — O Agregado Principal

Representa uma **sessão ativa de aquecimento**. Toda a lógica do micro-ondas vive aqui. Sua criação é feita via factory method `Start(...)`, que valida os parâmetros e inicia o estado como `Running`.

Cada método que muda estado (`Tick`, `Pause`, `Resume`, `Cancel`, `AddThirtySeconds`) verifica uma pré-condição (ex: `Status == Running`) e, se válida, altera o estado e **levanta um evento de domínio**.

O método `Tick` é o mais importante — chamado a cada segundo pelo `HeatingTimerService`:
1. Chama o renderer para gerar o segmento visual (ex: `........` para potência 8)
2. Acumula no `CurrentOutput` separado por espaço
3. Decrementa `RemainingSeconds`
4. Se chegou a zero, chama `Complete()` internamente
5. Levanta `HeatingTickedEvent` com o estado atual (para o front atualizar em tempo real)

#### `HeatingProgram` — Programas de Aquecimento

Representa um programa de aquecimento: `Predefined` (imutável, hardcoded) ou `Custom` (criado pelo usuário, persistido no banco).

A factory `CreatePredefined` não valida — os dados são confiáveis (seed data interno). A factory `CreateCustom` valida todos os campos via value objects e retorna `Result<HeatingProgram>`. A exclusão (`Delete`) só é permitida em programas `Custom` — predefinidos retornam `Result.Failure(Error.Forbidden(...))`.

#### Value Objects — Imutabilidade e Validação Encapsulada

Cada conceito do domínio com regras de validação vira um value object:

| Value Object | Regra encapsulada |
|---|---|
| `HeatingTime` | Manual: 1–120s; Programa: ≥1s (sem limite superior) |
| `PowerLevel` | 1 a 10 |
| `HeatingCharacter` | Não pode ser whitespace |
| `HeatingDisplayTime` | Se 61–99 segundos → "M:SS"; caso contrário → número puro |
| `HeatingParameters` | Agrupa Time + Power + Character num único objeto imutável |
| `ProgramName` | Obrigatório, máximo 50 caracteres |
| `FoodDescription` | Obrigatório, máximo 100 caracteres |
| `InstructionText` | Opcional, máximo 500 caracteres |

O motivo de criar um value object para algo simples como `PowerLevel` (apenas um `int`) é semântica e proteção: você não pode passar um `int` qualquer onde se espera um `PowerLevel` validado. A validação roda uma única vez, na criação, e o objeto resultante é sempre válido.

#### `PredefinedProgramSeed`

Lista estática dos 5 programas pré-definidos. Não é um arquivo de configuração nem uma tabela no banco — é **código**. Isso garante que:
1. Os programas estejam sempre presentes, mesmo em banco limpo
2. Nunca possam ser deletados por acidente via SQL
3. Estejam versionados junto com o código

O `ReservedCharacters` expõe um `HashSet<char>` com os caracteres de todos os pré-definidos + `.` (padrão), usado para validar programas customizados.

#### `IHeatingOutputRenderer` — Inversão para Testabilidade

O domínio precisa gerar o texto do display (`...`), mas **não deve saber como renderizar**. A interface `IHeatingOutputRenderer` inverte essa dependência: o domínio chama `RenderSegment(character, power)` e a implementação concreta faz o trabalho. Isso permite testar o domínio com um renderer fake.

#### `HeatingSessionHolder` — Estado em Memória com Thread Safety

Guarda a referência à sessão ativa atual. É um singleton com um `lock` interno para garantir acesso seguro entre o `HeatingTimerService` (rodando em background) e as requisições HTTP (threads concorrentes):

```csharp
private HeatingSession? _current;
private readonly object _lock = new();

public HeatingSession? Current
{
    get { lock (_lock) { return _current; } }
}
```

---

### Application — Orquestração

#### Command Handlers

Cada comando representa uma **intenção do usuário**. Os handlers orquestram o domínio sem conter lógica de negócio:

```
StartHeatingCommand
  ├─ Sessão rodando?  → AddThirtySeconds()
  ├─ Sessão pausada?  → Resume()
  └─ Sem sessão?      → HeatingSession.Start(...) → HeatingSessionHolder.Set(session)
```

Nenhum handler contém validação de dados (fica no `ValidationBehavior`) nem logging (fica no `LoggingBehavior`). O handler foca **apenas na lógica de orquestração**.

#### Pipeline Behaviors — Cross-Cutting Concerns

Os behaviors do MediatR formam uma cadeia em torno de cada command/query:

```
Requisição
  │
  ▼
LoggingBehavior              ← loga "Handling StartHeatingCommand"
  │
  ▼
ValidationBehavior           ← valida com FluentValidation; retorna erro se inválido
  │
  ▼
CommandHandler               ← lógica de orquestração do domínio
  │
  ▼
DomainEventDispatchBehavior  ← publica os eventos coletados pelo handler
  │
  ▼
LoggingBehavior              ← loga "Handled StartHeatingCommand"
  │
  ▼
Resposta (Result<T>)
```

O `DomainEventCollector` é um serviço **scoped** que o handler usa para extrair eventos dos agregados. O `DomainEventDispatchBehavior`, executado após o handler, publica todos os eventos coletados via `IMediator.Publish()`. O domínio **levanta** eventos, o handler os **coleta**, e o behavior os **publica** — nenhuma camada conhece as outras.

#### Event Handlers

Quando um evento é publicado, os handlers registrados reagem. Para eventos de aquecimento, o handler notifica o SignalR:

```csharp
public async Task Handle(HeatingTickedEvent notification, CancellationToken ct)
    => await _notifier.NotifyHeatingTickedAsync(
           notification.SessionId,
           notification.RemainingSeconds,
           notification.DisplayTime,
           notification.CurrentOutput);
```

O `IHeatingHubNotifier` abstrai a comunicação com o SignalR. No projeto Web, a implementação real usa `IHubContext<HeatingHub>`. No projeto API, usa `NoOpHeatingHubNotifier` — porque a API não possui SignalR.

#### Read Models (Queries CQRS)

Queries retornam DTOs diretamente, sem passar por agregados complexos. `GetAllProgramsQueryHandler`, por exemplo, combina os programas do seed (in-memory) com os programas customizados do banco e retorna uma lista de `ProgramReadModel`. Não há lógica de negócio — apenas leitura e mapeamento.

---

### Infrastructure

#### EF Core e `MicroondasDbContext`

O contexto tem apenas um `DbSet<HeatingProgram>`. O mapeamento usa **Owned Entities** do EF Core para value objects:

```csharp
builder.OwnsOne(x => x.Name, name =>
    name.Property(n => n.Value).HasColumnName("Name").HasMaxLength(50).IsRequired()
);
```

Isso mapeia o value object `ProgramName` para a coluna `Name` da tabela, de forma transparente para o domínio.

#### `HeatingTimerService` — O Coração do Tempo Real

```csharp
protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
    var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
    while (await timer.WaitForNextTickAsync(stoppingToken))
    {
        var session = _sessionHolder.Current;
        if (session is null || !session.IsRunning) continue;

        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Send(new TickHeatingCommand());
    }
}
```

`PeriodicTimer` (introduzido no .NET 6) não acumula ticks perdidos e é mais eficiente que `System.Threading.Timer`. O worker cria um novo **scope a cada tick** porque é singleton mas precisa de serviços scoped (`IMediator`, `DbContext`). Criar um scope garante que cada tick tenha suas próprias dependências, sem vazar estado.

---

## 7. Decisões Técnicas Detalhadas

### Por que DDD?

O micro-ondas tem **regras de negócio ricas e interdependentes**: uma sessão não pode ser cancelada enquanto roda; programas pré-definidos não podem ser deletados; o caractere de aquecimento deve ser único; +30s não funciona em sessões de programa. DDD resolve isso colocando as regras **dentro dos próprios objetos de domínio**, que se auto-protegem.

### Por que CQRS?

Leitura e escrita têm requisitos diferentes. A leitura de "todos os programas" combina dados in-memory (seed) com dados do banco — não deve passar pela lógica de domínio. A escrita de "criar programa" valida, levanta eventos e persiste — exige o domínio completo. Separar em Commands e Queries torna cada fluxo mais simples e testável independentemente.

### Por que MediatR?

MediatR atua como um **mediador** entre quem envia uma intenção (controller) e quem a executa (handler), sem que um conheça o outro. Isso permite behaviors reutilizáveis (logging, validação, eventos) aplicados uniformemente a todos os commands, e facilita a adição de novos handlers sem modificar quem chama.

### Por que FluentValidation com ValidationBehavior?

A validação de entrada (ex: "tempo entre 1 e 120") é separada da validação de negócio (ex: "sessão não pode pausar se estiver Idle"). FluentValidation cuida da primeira, os agregados cuidam da segunda. O `ValidationBehavior` intercepta qualquer command antes do handler e rejeita com erro padronizado se houver falha.

### Por que eventos de domínio em vez de chamar SignalR diretamente?

Se o handler chamasse o SignalR diretamente, ele dependeria de infraestrutura e seria difícil de testar. Ao levantar um evento, o handler fica agnóstico ao que acontece depois. Isso permite testar o handler sem mock de SignalR e adicionar novos comportamentos (ex: webhook, log de auditoria) sem modificar o handler.

### Por que `Result<T>` em vez de exceções?

Exceções são para situações excepcionais — erros de sistema, bugs. Regras de negócio quebradas são situações esperadas e devem ser parte do fluxo normal. Com `Result<T>`, o tipo de retorno de um método anuncia explicitamente que pode falhar, forçando o chamador a lidar com o erro. Isso elimina try/catch espalhados e torna o fluxo de erros visível na assinatura dos métodos.

---

## 8. Por que Apenas Uma Tabela no Banco?

O banco tem **somente** a tabela `HeatingPrograms`.

**Por que não persistir `HeatingSession`?**

A sessão de aquecimento é **efêmera por natureza** — dura segundos ou minutos. Se o servidor reiniciar durante um aquecimento, o comportamento correto é: o usuário recomeça. Não há dado valioso a recuperar. Persistir a sessão adicionaria complexidade significativa: queries a cada segundo, sincronização entre banco e memória, lógica de recuperação no boot — tudo para nenhum benefício prático.

**Por que não persistir os programas pré-definidos?**

Eles são **imutáveis e versionados com o código**. Colocá-los no banco criaria um problema de sincronização: mudar o seed no código sem uma migration de dados deixaria o banco inconsistente. Em código, a consistência é garantida pelo compilador. A separação é clara:
- Programas pré-definidos → código (`PredefinedProgramSeed.cs`)
- Programas customizados → banco (`HeatingPrograms` onde `Type = 'Custom'`)

**Por que não há foreign keys ou tabelas separadas?**

`HeatingProgram` é um único agregado com um único conjunto de campos. Não há relacionamentos entre programas, nem subdivisões que justifiquem normalização. Uma tabela com uma coluna discriminadora (`Type`) é o modelo mais simples e correto.

---

## 9. Por que a Sessão Fica em Memória?

O `HeatingSessionHolder` é um **singleton** que guarda a sessão ativa em RAM, protegido por `lock` para thread safety.

**Razões:**

1. **Performance**: O `HeatingTimerService` chama `Tick` a cada segundo. Buscar a sessão no banco a cada segundo seria custoso e introduziria latência perceptível.

2. **Semântica correta**: Um micro-ondas físico tem uma sessão ativa por vez. O singleton reflete exatamente isso — há um único estado global do aparelho.

3. **Simplicidade**: Não há necessidade de query, serialização/deserialização, nem concorrência de banco. O lock garante acesso seguro.

**Trade-off conhecido**: Em produção com múltiplas instâncias (ex: Docker com 3 réplicas), cada instância teria seu próprio `HeatingSessionHolder`. Para esse cenário, a solução seria um cache distribuído (ex: Redis). Para um micro-ondas — que por definição é um único aparelho — múltiplas instâncias não fazem sentido.

---

## 10. Por que Singleton, Scoped e Transient?

| Serviço | Lifetime | Justificativa |
|---|---|---|
| `HeatingSessionHolder` | **Singleton** | Estado global único do aparelho; precisa sobreviver entre requisições e ser acessado pelo worker |
| `IHeatingOutputRenderer` | **Singleton** | Completamente stateless; não faz sentido instanciar a cada requisição |
| `IClock` (`SystemClock`) | **Singleton** | Stateless; retorna `DateTimeOffset.UtcNow` |
| `FileExceptionLogger` | **Singleton** | Mantém o path do arquivo de log; compartilhado por toda a aplicação |
| `HeatingTimerService` | **Singleton** (HostedService) | Loop que roda continuamente; deve existir uma única instância |
| `DbContext` | **Scoped** | Padrão do EF Core: uma instância por request HTTP; garante Unit of Work por transação |
| `IHeatingProgramRepository` | **Scoped** | Depende do DbContext; deve ter o mesmo lifetime |
| `DomainEventCollector` | **Scoped** | Acumula eventos por request; deve ser limpo após cada requisição |
| `IHeatingHubNotifier` | **Scoped** | Depende de `IHubContext`, que é scoped no ASP.NET Core |
| Validators (FluentValidation) | **Scoped** | Padrão do FluentValidation |

**Por que o worker cria um novo scope a cada tick?**

O `HeatingTimerService` é singleton, mas precisa resolver serviços scoped (`IMediator`, `DbContext`). A forma correta no .NET é usar `IServiceScopeFactory.CreateScope()` a cada operação, criando um escopo efêmero que é descartado ao final. Isso evita capturar um serviço scoped em um singleton — anti-pattern que causa bugs de estado compartilhado entre requisições.

---

## 11. Fluxo Completo de uma Requisição

Exemplo: usuário clica em **"▶ Iniciar"** no navegador.

```
1. Browser → POST /Heating/Start { TimeInSeconds: 45, PowerLevel: 8 }

2. HeatingController.Start(viewModel)
   └─ _mediator.Send(new StartHeatingCommand(45, 8, null))

3. MediatR Pipeline:
   ├─ LoggingBehavior:     log "Handling StartHeatingCommand"
   ├─ ValidationBehavior:  FluentValidation (45 ∈ [1,120] ✓, 8 ∈ [1,10] ✓)
   ├─ StartHeatingCommandHandler:
   │  ├─ Cria HeatingTime.CreateManual(45)
   │  ├─ Cria PowerLevel.Create(8)
   │  ├─ HeatingSession.Start(parameters, renderer)
   │  │  └─ Levanta HeatingStartedEvent (internamente)
   │  ├─ _sessionHolder.Set(session)
   │  └─ _collector.Collect(session)  ← extrai HeatingStartedEvent
   ├─ DomainEventDispatchBehavior:
   │  └─ _mediator.Publish(HeatingStartedEvent)
   │     └─ HeatingStartedEventHandler:
   │        └─ SignalR → Browser: "HeatingStarted" { totalSeconds: 45, powerLevel: 8 }
   └─ LoggingBehavior:     log "Handled StartHeatingCommand"

4. Controller → Redirect para /Heating

5. HeatingTimerService (background, 1s depois):
   ├─ Cria scope → resolve IMediator
   └─ _mediator.Send(new TickHeatingCommand())
      └─ TickHeatingCommandHandler:
         ├─ session.Tick(renderer)
         │  ├─ Renderiza "........" (8 pontos, potência 8)
         │  ├─ CurrentOutput = "........"
         │  ├─ RemainingSeconds = 44
         │  └─ Levanta HeatingTickedEvent
         └─ DomainEventDispatchBehavior → HeatingTickedEventHandler
            └─ SignalR → Browser: display "44", output "........"

6. (Repete a cada segundo até RemainingSeconds = 0)

7. Tick final (RemainingSeconds = 0):
   ├─ session.Complete(renderer)
   │  └─ Levanta HeatingCompletedEvent
   ├─ Handler limpa HeatingSessionHolder
   └─ SignalR → Browser: "HeatingCompleted" → display "✓ CONCLUÍDO"
```

---

## 12. SignalR e Tempo Real

O SignalR permite que o servidor **empurre atualizações** para o browser sem que ele precise fazer polling.

### Fluxo de notificação

1. O browser estabelece uma conexão WebSocket com `/heatingHub` ao carregar a página (`heating-signalr.js`)
2. O servidor usa `IHubContext<HeatingHub>` para enviar mensagens a **todos os clientes conectados** (`Clients.All.SendAsync(...)`)
3. Cada evento de domínio gera uma mensagem SignalR:

| Evento de Domínio | Mensagem SignalR | Dados enviados |
|---|---|---|
| `HeatingStartedEvent` | `HeatingStarted` | totalSeconds, powerLevel |
| `HeatingTickedEvent` | `HeatingTicked` | remainingSeconds, displayTime, currentOutput |
| `HeatingPausedEvent` | `HeatingPaused` | remainingSeconds |
| `HeatingResumedEvent` | `HeatingResumed` | remainingSeconds |
| `HeatingCancelledEvent` | `HeatingCancelled` | — |
| `HeatingCompletedEvent` | `HeatingCompleted` | finalOutput |
| `HeatingTimeAddedEvent` | `HeatingTimeAdded` | newRemainingSeconds |

4. O JavaScript escuta e atualiza o DOM:

```javascript
connection.on("HeatingTicked", function (data) {
    updateTimeDisplay(data.remainingSeconds, data.displayTime);
    updateOutput(data.currentOutput);
});
```

O `HeatingHub` é propositalmente mínimo — apenas registra conexões. Toda a lógica de broadcast está no `HeatingHubNotifier`. O `IHeatingHubNotifier` desacopla os event handlers da implementação concreta: a API usa `NoOpHeatingHubNotifier` (noop), permitindo que os mesmos handlers funcionem nos dois projetos.

### Por que `Clients.All`?

O projeto não tem autenticação por sessão de usuário na interface web — qualquer browser aberto vê o mesmo micro-ondas. Em um cenário com múltiplos usuários independentes, usaríamos `Clients.User(userId)` ou groups.

---

## 13. API REST e Autenticação JWT

O projeto `Microondas.Api` expõe a mesma funcionalidade via REST, protegida por **JWT Bearer Token**.

### Por que JWT?

JWT é stateless: o token contém todas as informações necessárias para validar a requisição. O servidor **não armazena sessão** — apenas valida a assinatura do token com a chave secreta. Vantagens:
- Funciona com qualquer cliente (mobile, CLI, outro serviço)
- Não requer cookies ou sessão no servidor
- Escalável (qualquer instância valida o token com a mesma chave)
- Expira automaticamente (o token tem `exp`)

### Configuração do JWT

```json
{
  "Jwt": {
    "Secret": "MicroondasDefaultSecretKey2024!!",
    "Issuer": "Microondas.Api",
    "Audience": "Microondas.Web",
    "ExpirationMinutes": 60
  }
}
```

O token gerado contém: `iss`, `aud`, `exp`, `name` (username) e `jti` (ID único do token).

### Por que SHA256 para a senha?

O projeto tem um único usuário admin. SHA256 é usado para não armazenar a senha em texto puro. Em produção com múltiplos usuários, use `Microsoft.AspNetCore.Identity` com bcrypt ou Argon2.

### Endpoints da API

| Método | Rota | Descrição |
|---|---|---|
| `POST` | `/api/auth/login` | Autentica e retorna JWT |
| `GET` | `/api/heating/status` | Status atual do aquecimento |
| `POST` | `/api/heating/start` | Inicia aquecimento manual |
| `POST` | `/api/heating/quick-start` | Inicia com padrões (30s, pot. 10) |
| `POST` | `/api/heating/pause-cancel` | Pausa ou cancela |
| `POST` | `/api/heating/start-program/{id}` | Inicia por programa |
| `GET` | `/api/programs` | Lista todos os programas |
| `GET` | `/api/programs/{id}` | Busca programa por ID |
| `POST` | `/api/programs` | Cria programa customizado |
| `DELETE` | `/api/programs/{id}` | Remove programa customizado |

---

## 14. Testes

### Domain Tests

Testam os agregados e value objects **diretamente**, sem mocks de infraestrutura:

- **`HeatingSessionTests`**: fluxos de start/tick/pause/resume/cancel/complete/addTime; validação de pré-condições; acúmulo correto do output visual
- **`HeatingProgramTests`**: criação de programas custom e predefinidos; tentativa de delete em predefinido (deve falhar com Forbidden)
- **`HeatingTimeTests`**, **`PowerLevelTests`**, **`HeatingCharacterTests`**: validação dos value objects
- **`HeatingDisplayTimeTests`**: formatação do tempo (MM:SS vs segundos puros)
- **`HeatingOutputRendererTests`**: geração correta dos segmentos (`char * power`)

### Application Tests

Testam os command handlers com mocks das dependências:

- **`StartHeatingCommandHandlerTests`**: verifica os 3 casos — nova sessão, sessão rodando (+30s), sessão pausada (resume)
- **`TickHeatingCommandHandlerTests`**: tick sem sessão é no-op; sessão concluída é limpa do holder
- **`PauseOrCancelHeatingCommandHandlerTests`**: verifica a máquina de estados pelo handler
- **`CreateCustomProgramCommandHandlerTests`**: validação de unicidade de caractere; criação bem-sucedida
- **`GetAllProgramsQueryHandlerTests`**: retorna programas do seed + customizados do repositório mock

### Integration Tests

Testam o pipeline completo com banco real, verificando que behaviors (validation, logging, event dispatch) se encadeiam corretamente de ponta a ponta.

---

## 15. Estrutura de Pastas

```
microondas-challenge/
│
├── src/
│   ├── Microondas.SharedKernel/
│   │   ├── Entity.cs
│   │   ├── ValueObject.cs
│   │   ├── AggregateRoot.cs
│   │   ├── Result.cs
│   │   ├── Error.cs
│   │   ├── BusinessException.cs
│   │   ├── Guard.cs
│   │   └── IClock.cs
│   │
│   ├── Microondas.Domain/
│   │   ├── Heating/
│   │   │   ├── HeatingSession.cs              ← Agregado principal
│   │   │   ├── HeatingStatus.cs               ← Enum de estados
│   │   │   ├── Events/                        ← 7 eventos de domínio
│   │   │   ├── ValueObjects/                  ← HeatingTime, PowerLevel, etc.
│   │   │   └── Services/
│   │   │       ├── IHeatingOutputRenderer.cs
│   │   │       ├── HeatingOutputRenderer.cs
│   │   │       └── HeatingSessionHolder.cs    ← Estado em memória
│   │   └── Programs/
│   │       ├── HeatingProgram.cs              ← Agregado de programas
│   │       ├── ProgramType.cs                 ← Enum Predefined/Custom
│   │       ├── PredefinedProgramSeed.cs       ← Seed data hardcoded
│   │       ├── Events/                        ← 2 eventos de domínio
│   │       └── ValueObjects/                  ← ProgramName, FoodDescription, etc.
│   │
│   ├── Microondas.Domain.Contracts/
│   │   ├── Commands/   ICommand, ICommandHandler
│   │   ├── Queries/    IQuery, IQueryHandler
│   │   ├── Events/     IDomainEventHandler
│   │   └── Repositories/  IHeatingProgramRepository
│   │
│   ├── Microondas.Application.CommandHandlers/
│   │   ├── Behaviors/
│   │   │   ├── LoggingBehavior.cs
│   │   │   ├── ValidationBehavior.cs
│   │   │   ├── DomainEventCollector.cs
│   │   │   └── DomainEventDispatchBehavior.cs
│   │   ├── Heating/
│   │   │   ├── StartHeating/
│   │   │   ├── QuickStartHeating/
│   │   │   ├── TickHeating/
│   │   │   ├── PauseOrCancelHeating/
│   │   │   └── StartProgramHeating/
│   │   └── Programs/
│   │       ├── CreateCustomProgram/
│   │       └── DeleteCustomProgram/
│   │
│   ├── Microondas.Application.ReadModels/
│   │   ├── Heating/
│   │   │   ├── GetHeatingStatusQuery.cs
│   │   │   ├── GetHeatingStatusQueryHandler.cs
│   │   │   └── HeatingStatusReadModel.cs
│   │   └── Programs/
│   │       ├── GetAllProgramsQuery.cs
│   │       ├── GetProgramByIdQuery.cs
│   │       └── ProgramReadModel.cs
│   │
│   ├── Microondas.Application.EventHandlers/
│   │   ├── IHeatingHubNotifier.cs             ← Contrato do notificador SignalR
│   │   ├── Heating/                           ← 7 handlers → SignalR
│   │   └── Programs/                          ← 2 handlers → log de auditoria
│   │
│   ├── Microondas.Infrastructure/
│   │   ├── Persistence/
│   │   │   ├── MicroondasDbContext.cs
│   │   │   ├── Configurations/HeatingProgramConfiguration.cs
│   │   │   └── Repositories/HeatingProgramRepository.cs
│   │   ├── Services/  SystemClock, PasswordHasher
│   │   ├── Logging/   FileExceptionLogger
│   │   └── DependencyInjection.cs
│   │
│   ├── Microondas.Migrations/
│   │   └── Migrations/[timestamp]_InitialMigration.cs
│   │
│   ├── Microondas.Workers/
│   │   └── HeatingTimerService.cs             ← BackgroundService (tick a cada 1s)
│   │
│   ├── Microondas.Web/
│   │   ├── Controllers/  HeatingController, ProgramsController
│   │   ├── Hubs/         HeatingHub, HeatingHubNotifier
│   │   ├── Middleware/   GlobalExceptionMiddleware
│   │   ├── ViewComponents/  DigitalKeyboard, HeatingDisplay, ProgramSelector
│   │   ├── Views/        Heating/Index, Programs/Index, Programs/Create
│   │   ├── wwwroot/      microwave.css, heating-signalr.js, digital-keyboard.js
│   │   └── Program.cs
│   │
│   └── Microondas.Api/
│       ├── Controllers/  AuthController, HeatingApiController, ProgramsApiController
│       ├── Authentication/  JwtSettings, JwtTokenGenerator, AuthCredentials
│       ├── Middleware/   ApiExceptionMiddleware
│       ├── Notifications/ NoOpHeatingHubNotifier
│       └── Program.cs
│
└── tests/
    ├── Microondas.Domain.Tests/
    ├── Microondas.Application.Tests/
    └── Microondas.Integration.Tests/
```

---

## 16. Tecnologias

| Tecnologia | Versão | Uso |
|---|---|---|
| .NET / C# | 8.0 / 12 | Plataforma e linguagem |
| ASP.NET Core MVC | 8.0 | Interface web com Razor Views |
| ASP.NET Core Web API | 8.0 | REST API |
| SignalR | 8.0 | Atualizações em tempo real (WebSocket) |
| Entity Framework Core | 8.0 | ORM e persistência |
| SQL Server | qualquer | Banco de dados relacional |
| MediatR | 12.x | CQRS / mediator pattern / pipeline behaviors |
| FluentValidation | 11.x | Validação declarativa de commands |
| xUnit | 2.x | Framework de testes |
| FluentAssertions | 6.x | Assertions expressivas |
| NSubstitute | 5.x | Mocks para testes de aplicação |
| Swagger / Swashbuckle | 6.x | Documentação interativa da API |

---

Desenvolvido por **Kauã Vilas Boas** como desafio técnico.
