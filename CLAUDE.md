# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Kabbalah numerology application based on José Carlos Rosa's book. Single-repository with C# backend (ASP.NET Core Web API) and Blazor frontend, backed by PostgreSQL, deployed on Railway.

Authentication via Google OAuth. **No code merges to `main` without a Pull Request and passing CI checks** (enforced via GitHub branch protection + GitHub Actions).

**GitHub repository:** `https://github.com/lordlagon/numerologia` (private)
**Remote:** `https://github.com/lordlagon/numerologia.git` — authenticate as `lordlagon`
**Default branch:** `main`

---

## Stack

| Layer | Technology |
|---|---|
| Backend | ASP.NET Core Web API (.NET 10) |
| Frontend | Blazor WebAssembly (.NET 10) |
| Database | PostgreSQL (EF Core + Migrations) |
| Auth | Google OAuth (`Microsoft.AspNetCore.Authentication.Google`) |
| Deploy | Railway |
| ORM | Entity Framework Core + Npgsql |

---

## Repository Structure

```
/
├── src/
│   ├── Numerologia.Api/            # ASP.NET Core Web API
│   ├── Numerologia.Web/            # Blazor WebAssembly (servido pela API em produção)
│   ├── Numerologia.Core/           # Domain logic, entities, DTOs (shared)
│   └── Numerologia.Infrastructure/ # EF Core, repositories, external integrations
├── tests/
│   ├── Numerologia.UnitTests/      # xUnit unit tests
│   └── Numerologia.IntegrationTests/ # WebApplicationFactory tests
├── Dockerfile                      # Multi-stage: web-build → api-build → runtime
├── docker-compose.yml              # Dev local: API + PostgreSQL
└── railway.toml                    # Config de deploy no Railway
```

## Containers

### Produção / Staging (Railway)

```
Railway
├── Serviço: numerologia   ← Dockerfile (API + Blazor WASM no wwwroot)
└── Serviço: PostgreSQL    ← plugin nativo do Railway
```

A variável `DATABASE_URL` é injetada automaticamente pelo Railway ao adicionar o plugin PostgreSQL.

### Desenvolvimento local

```bash
docker compose up
```

Sobe a API em `http://localhost:8080` e o PostgreSQL em `localhost:5432`.

---

## Commands

Run from the solution root (where `.sln` is located):

```bash
# Restore dependencies
dotnet restore

# Build
dotnet build

# Run API
dotnet run --project src/Numerologia.Api

# Run Blazor frontend
dotnet run --project src/Numerologia.Web

# Run all tests
dotnet test

# Run a single test class
dotnet test --filter "FullyQualifiedName~CalculoMapaTests"

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Apply EF Core migrations
dotnet ef database update --project src/Numerologia.Infrastructure --startup-project src/Numerologia.Api

# Add a new migration
dotnet ef migrations add <MigrationName> --project src/Numerologia.Infrastructure --startup-project src/Numerologia.Api

# Check for vulnerable packages
dotnet list package --vulnerable
```

---

## Testing — Required Tools & Rules

**No code may be merged to `main` without passing tests.**

### Packages (tests projects)

| Purpose | Package | Version |
|---|---|---|
| Test framework | `xunit` | v2.x |
| Mocking | `NSubstitute` | v5.x |
| Blazor components | `bunit` | v1.x |
| Assertions | `FluentAssertions` | **`~7.*` only** |
| Test data | `Bogus` | v35.x |
| Coverage | `coverlet.collector` | v6.x |
| Integration tests | `Microsoft.AspNetCore.Mvc.Testing` | built-in |

> **FluentAssertions v8+ changed to a commercial license in Jan 2025. Pin to v7.x:**
> ```xml
> <PackageReference Include="FluentAssertions" Version="7.*" />
> ```

### Test conventions

- Unit tests go in `Numerologia.UnitTests`, mirroring the source project structure.
- Integration tests use `WebApplicationFactory<Program>` with an in-memory or containerized PostgreSQL (TestContainers).
- Blazor component tests use `bUnit`; mock `IJSRuntime` — bUnit does not execute JavaScript.
- Use `Bogus` to generate realistic test data with explicit constraints; avoid magic strings.

---

## Security Tools

### Static Analysis (SAST)
- **Security Code Scan** (`SecurityCodeScan` NuGet) — runs automaticamente em tempo de build via Roslyn analyzer. Escolhido no lugar do SonarCloud pois o repo é **privado** (SonarCloud free só cobre repos públicos).

### Dependency Scanning
- **GitHub Dependabot** — enabled on the repo; opens PRs automatically for vulnerable packages.
- **`dotnet list package --vulnerable`** — run locally before committing and in CI.

### Secret Scanning
- **GitLeaks** — runs as a pre-commit hook and in GitHub Actions. Never commit credentials.

### HTTP Security Headers
- **`NetEscapades.AspNetCore.SecurityHeaders`** — add to API middleware pipeline.

### DAST (Dynamic)
- **OWASP ZAP + Playwright** — run against staging environment in CI before production deploys.

---

## CI/CD — GitHub Actions

### Fluxo de branches

```
feature/* ──PR──► staging ──PR──► main
              CD→ Railway Staging    CD→ Railway Production
```

| Branch | Workflow | O que faz |
|--------|----------|-----------|
| `feature/*` | `ci.yml` | build + test + security (validação de PR) |
| `staging` | `cd-staging.yml` | build + test + security + **deploy Railway Staging** |
| `main` | `cd-production.yml` | build + test + security + **deploy Railway Production** |

### Workflows

- **`ci.yml`** — dispara em push de feature branches e em PRs abertos contra `staging` ou `main`.
- **`cd-staging.yml`** — dispara em push para `staging`; inclui CI completo + deploy. Deploy só ocorre se todos os checks passarem.
- **`cd-production.yml`** — dispara em push para `main`; inclui CI completo + deploy. Concorrência bloqueada (`cancel-in-progress: false`) para nunca abortar um deploy de produção.

### Branch protection

**`staging`:**
- PR obrigatório (direto push bloqueado).
- CI (`ci.yml`) deve passar.

**`main`:**
- PR obrigatório (direto push bloqueado).
- CI (`ci.yml`) deve passar.
- Pelo menos 1 aprovação.
- Stale reviews descartados em novos commits.

### Segredos e variáveis necessários no GitHub

| Nome | Tipo | Onde configurar |
|------|------|----------------|
| `RAILWAY_TOKEN` | Secret | Settings → Secrets → Actions (repository-level) |
| `RAILWAY_SERVICE` | Variable | Settings → Environments → `staging` (e `production`) → Variables |
| `RAILWAY_ENVIRONMENT` | Variable | Settings → Environments → `staging` (e `production`) → Variables |
| `RAILWAY_STAGING_URL` | Variable | Settings → Environments → `staging` → Variables |
| `RAILWAY_PRODUCTION_URL` | Variable | Settings → Environments → `production` → Variables |

> **Importante:** `RAILWAY_SERVICE` e `RAILWAY_ENVIRONMENT` devem ser configuradas dentro do **GitHub Environment** (staging/production), não como variáveis de repositório.
> O Railway cria environments com nomes como "Numerologia / staging" — **ignore-os**. Crie environments próprios chamados `staging` e `production` no GitHub.
> O `RAILWAY_TOKEN` é gerado em: Railway Dashboard → Account Settings → Tokens.

---

## Google Authentication

- Package: `Microsoft.AspNetCore.Authentication.Google`
- Client ID and Secret are stored as Railway environment variables (`GOOGLE_CLIENT_ID`, `GOOGLE_CLIENT_SECRET`), never in source code.
- Blazor WASM auth flow: frontend uses `Microsoft.AspNetCore.Components.WebAssembly.Authentication`; the API handles the OAuth callback and issues a cookie/JWT.

---

## Core Numerology Logic

All calculation logic lives in `Numerologia.Core/Calculos/`:

- **CalculoMapa** — maps letters a–z to numbers using the Kabbalistic table (page 63 of the reference book; see `docs/tabela.png`).
- **SequenciasPiramide** — pyramid reduction: add adjacent digits, subtract 9 if result > 9.
- **CalculoDestino** — birth date → destiny number (sum and reduce digits).
- Vowels (a, e, i, o, u) → `Motivação`; consonants → `Impressão`.
- Reduction rule: sum all digits of a number until a single digit remains.

The Java implementation in `Numerologia-Java/` is the reference for porting logic; do not add new features there. It is excluded from the Git repository via `.gitignore` and must not be committed.

---

## Railway Deployment

- Deploy é acionado pelo CD do GitHub Actions (não pelo auto-deploy nativo do Railway).
- Environment variables (DB connection string, Google OAuth, JWT secret) são definidas no Railway dashboard — nunca em `appsettings.json`.
- `appsettings.Production.json` não deve conter valores sensíveis.
- O plugin PostgreSQL do Railway injeta `DATABASE_URL` automaticamente.

### Gotchas descobertos em produção

| Problema | Causa | Solução |
|----------|-------|---------|
| Blazor travado em 100% com 404 em `_framework/*.dat` | `UseStaticFiles` não serve extensões desconhecidas por padrão | `ServeUnknownFileTypes = true` + mapear `.dat`, `.blat`, `.wasm` no `FileExtensionContentTypeProvider` |
| Redirect loop no Railway | `UseHttpsRedirection` conflita com o proxy TLS do Railway | **Não usar** `UseHttpsRedirection` — Railway termina HTTPS externamente |
| Build Docker falha com "unable to find python in PATH" | `wasm-tools` workload requer Python + Emscripten | **Não instalar** `wasm-tools` na imagem Docker — publish sem ele funciona em modo interpretado |
| GitLeaks falha com 403 em PRs | Token `GITHUB_TOKEN` sem permissão de leitura de PRs | Adicionar `permissions: pull-requests: read` no workflow |
| GitLeaks falha com "ambiguous argument" | Checkout shallow (depth 1) sem histórico completo | Adicionar `fetch-depth: 0` no step de checkout do job security |
| Variáveis Railway não encontradas no CD | Variáveis configuradas como Repository Variables em vez de Environment Variables | Configurar `RAILWAY_SERVICE` e `RAILWAY_ENVIRONMENT` dentro do **GitHub Environment** `staging`/`production` |

---

## Process — Extreme Programming (XP)

This project follows **Extreme Programming** practices. Scrum artifacts (sprints, velocity, burndowns) are not used here.

### Core XP Practices

| Practice | How it applies here |
|---|---|
| **Test-Driven Development (TDD)** | Write a failing test first, then the minimum code to make it pass, then refactor. No production code without a prior failing test. |
| **Pair Programming** | When working with Claude Code, treat it as a pair: review every suggestion before accepting it. |
| **Continuous Integration** | Every push to a feature branch runs the full CI pipeline (build → test → security). |
| **Small Releases** | Feature branches are short-lived (1–2 days max). Each PR delivers one vertical slice of working, tested functionality. |
| **Simple Design** | YAGNI — only build what is needed right now. No speculative abstractions. |
| **Refactoring** | After every green test, improve the design. The test suite is the safety net. |
| **Collective Code Ownership** | Any area of the codebase can be changed by anyone, provided tests remain green. |
| **Coding Standards** | Follow C# conventions (PascalCase for types/methods, camelCase for locals). No deviations without team consensus. |
| **Sustainable Pace** | Do not accumulate technical debt to go faster short-term. |

### Branching & PR Rules

- **`main` is always deployable.** Direct pushes to `main` are blocked.
- Every change — no matter how small — must go through a **Pull Request**.
- PR naming convention: `feat/<slug>`, `fix/<slug>`, `refactor/<slug>`, `chore/<slug>`.
- CI must be green before merging.
- PR must have at least one approval.
- Keep PRs small: one user story or one bug fix per PR.

### Vertical Slice Rule

**A feature branch must not be sent to staging unless it is complete end-to-end:**

| Layer | Requirement |
|---|---|
| **DB** | Migration created and applied; schema reflects the feature |
| **Backend (API)** | Endpoints implemented, tested (unit + integration) |
| **Frontend (Blazor)** | Components implemented, tested with bUnit |

No partial slices. A PR that touches only the API without the corresponding Blazor UI (or vice-versa) must not be opened against `staging`.

### TDD Cycle

```
Red   → write a failing test that describes the desired behaviour
Green → write the minimum production code to make it pass
Blue  → refactor both test and production code without breaking the tests
```

Never skip the Red step. If there is no failing test, there is no feature.
