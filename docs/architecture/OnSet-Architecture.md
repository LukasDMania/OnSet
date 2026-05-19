# OnSet architecture (diagrams)

Companion to the PDFs in this folder. Mermaid renders on GitHub and in many Markdown preview tools.

## 1. Big picture: one deployable web app

`mermaid
flowchart TB
  subgraph Client["Clients"]
    B[Browser]
  end

  subgraph Host["ASP.NET Core host"]
    RP[Razor Pages UI]
    MW["HTTP pipeline: security, localization, routing, logging"]
    APP[Application composition: DI and configuration]
  end

  subgraph Core["In-process building blocks"]
    FEAT[Feature modules: commands and queries]
    MED[MediatR dispatcher]
    DOM[Domain model and rules]
    INF["Infrastructure: EF Core, files, audit, identity stores"]
  end

  subgraph External["External systems"]
    DB[(SQL Server)]
    FS[Local filesystem under web root]
  end

  B --> MW --> RP
  RP --> MED
  MED --> FEAT
  FEAT --> DOM
  FEAT --> INF
  INF --> DB
  INF --> FS
  APP --> MW
  APP --> MED
  APP --> INF
`

## 2. Logical layers

`mermaid
flowchart TB
  subgraph Presentation["Presentation layer"]
    P1[Razor Pages and layouts]
    P2[Page models: bind input, call application, redirect or render]
    P3[Localized strings and culture switching]
  end

  subgraph Application["Application / use-case layer"]
    A1[Feature folders: vertical slices]
    A2[Commands and queries with handlers]
    A3[FluentValidation validators]
    A4[DTOs and AutoMapper profiles]
    A5[Domain notifications where needed]
  end

  subgraph Domain["Domain layer"]
    D1[Entities: user, project, membership, documents]
    D2[Value objects: names, address, file metadata]
    D3[Enums: roles, status, languages]
    D4[Domain exceptions and rule violations]
  end

  subgraph Infrastructure["Infrastructure layer"]
    I1[EF Core DbContext and migrations]
    I2[SaveChanges interceptors and auditable entities]
    I3[Command audit logging service]
    I4[ASP.NET Core Identity on same database]
    I5[Local file storage for uploads]
    I6[Filters mapping domain errors to HTTP responses]
  end

  subgraph CrossCut["Shared / cross-cutting"]
    C1[Serilog and request logging]
    C2[Health checks]
    C3[Authorization policies]
    C4[Current user abstraction from HTTP context]
  end

  P2 --> A2
  A2 --> D1
  A2 --> I1
  I1 --> D1
  P2 --> C4
  A2 --> C1
  I1 --> C1
`

## 3. HTTP request pipeline

`mermaid
flowchart LR
  subgraph Pipeline["Typical request order"]
    direction TB
    H1[HTTPS and HSTS]
    S1[Static files]
    L1["Request localization: cookie, query, Accept-Language"]
    R1[Routing]
    A1[Authentication]
    Z1[Authorization]
    E1[Endpoint: Razor Page executes]
  end

  IN[Incoming HTTP] --> H1 --> S1 --> L1 --> R1 --> A1 --> Z1 --> E1
  E1 --> OUT[HTML, redirect, or status code]
`

## 4. Page request: UI to data

`mermaid
sequenceDiagram
  participant Browser
  participant Razor as Razor Page and PageModel
  participant Bus as MediatR
  participant Pipe as Pipeline behaviors
  participant Handler as Feature handler
  participant Dom as Domain entities and rules
  participant Db as EF Core and SQL Server
  participant Files as File storage

  Browser->>Razor: HTTP GET or POST
  Razor->>Bus: Send query or command
  Bus->>Pipe: Outer behaviors first
  Pipe->>Handler: After validation and audit wrappers
  Handler->>Dom: Apply rules and build aggregates
  Handler->>Db: Read or write through DbContext
  Handler->>Files: Optional upload or download
  Handler-->>Bus: Result or DTO
  Bus-->>Razor: Data for page model
  Razor-->>Browser: Rendered HTML or redirect
`

## 5. MediatR cross-cutting pipeline

`mermaid
flowchart TB
  subgraph Outer["Outer to inner order"]
    direction TB
    B1[Command audit behavior]
    B2[Performance telemetry behavior]
    B3[Validation behavior: FluentValidation]
    B4[Handler: load entities, persist, return result]
  end

  REQ[Send command or query] --> B1 --> B2 --> B3 --> B4 --> RES[Return to caller]
`

## 6. Feature slices

`mermaid
flowchart TB
  subgraph Example["Feature slice: one capability"]
    direction TB
    Q[Query and QueryHandler]
    C[Command and CommandHandler]
    V[Validators]
    M[Mapping profile]
  end

  subgraph Domains["Product areas"]
    U[Users: auth and profiles]
    PR[Projects: CRUD, join, documents]
  end

  U --> Q
  PR --> C
  V --> C
  M --> Q
`

## 7. Data, identity, and auditing

`mermaid
flowchart TB
  subgraph DataModel["Relational model"]
    T1[Users and Identity tables]
    T2[Projects and memberships]
    T3[Documents and metadata]
    T4[Command audit log]
    T5[Entity audit and auditable columns]
  end

  subgraph Runtime["Runtime mechanisms"]
    E1[DbContext and migrations]
    E2[SaveChanges interceptor]
    E3[Command audit service]
  end

  T1 --- E1
  T2 --- E1
  T3 --- E1
  T4 --- E3
  T5 --- E2
`

## 8. Authorization and user context

`mermaid
flowchart LR
  subgraph Auth["Security model"]
    I1[ASP.NET Core Identity]
    P1[Authorization policies]
    S1[Current user service for handlers]
  end

  Razor[Razor Pages] --> I1
  Razor --> P1
  Handler[Feature handlers] --> S1
`

## 9. Localization and static content

`mermaid
flowchart TB
  subgraph L10n["Localization"]
    R1[Default neutral resource strings]
    R2[Satellite resources per culture]
    R3[Culture cookie and language switch page]
    L2[String localizer in Razor]
  end

  subgraph Web["Front-end assets"]
    W1[Static CSS and JS under wwwroot]
    W2[User uploads under configured upload root]
  end

  R3 --> R2
  L2 --> R1
  L2 --> R2
`

## 10. Observability and operations

`mermaid
flowchart LR
  subgraph Ops["Operations"]
    H1[Health endpoint for database]
    L1[Structured logging via Serilog]
    L2[HTTP request logging]
  end

  Host[Web host] --> Ops
`
