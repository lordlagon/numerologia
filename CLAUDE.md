# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Kabbalah numerology application based on José Carlos Rosa's book. Single-repository with C# backend (ASP.NET Core Web API) and Blazor frontend, backed by PostgreSQL, deployed on Railway.

Authentication via Google OAuth. No code merges to `main` without passing unit tests (enforced via GitHub branch protection + GitHub Actions).

---

## Stack

| Layer | Technology |
|---|---|
| Backend | ASP.NET Core Web API (.NET 8) |
| Frontend | Blazor WebAssembly |
| Database | PostgreSQL (EF Core + Migrations) |
| Auth | Google OAuth (`Microsoft.AspNetCore.Authentication.Google`) |
| Deploy | Railway |
| ORM | Entity Framework Core + Npgsql |

---

## Repository Structure

```
/
├── src/
│   ├── Numerologia.Api/          # ASP.NET Core Web API
│   ├── Numerologia.Web/          # Blazor WebAssembly
│   ├── Numerologia.Core/         # Domain logic, entities, DTOs (shared)
│   └── Numerologia.Infrastructure/ # EF Core, repositories, external integrations
└── tests/
    ├── Numerologia.UnitTests/    # xUnit unit tests
    └── Numerologia.IntegrationTests/ # WebApplicationFactory tests
```

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

Branch protection on `main`:
- All status checks must pass (build, test, security scan).
- At least one approval required.
- Stale reviews dismissed on new commits.

Key jobs in the workflow:
1. `build` — `dotnet build`
2. `test` — `dotnet test` with coverage collection
3. `security` — `dotnet list package --vulnerable` + GitLeaks

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

The Java implementation in `Numerologia-Java/` is the reference for porting logic; do not add new features there.

---

## Railway Deployment

- Each `push` to `main` triggers an automatic deploy on Railway.
- Environment variables (DB connection string, Google OAuth, JWT secret) are set in Railway's dashboard — not in `appsettings.json`.
- `appsettings.Production.json` should contain no sensitive values.
