# Stack Técnica — Numerologia

## Tecnologias

| Camada | Tecnologia |
|--------|------------|
| Backend | ASP.NET Core Web API (.NET 10) |
| Frontend | Blazor WebAssembly (.NET 10) |
| UI Components | MudBlazor 7.16.0 — tema Deep Purple (`#7b1fa2`) |
| Banco de dados | PostgreSQL |
| ORM | Entity Framework Core 10 + Npgsql 10 |
| Auth | Google OAuth (`Microsoft.AspNetCore.Authentication.Google` v10.*) |
| Geração de PDF | QuestPDF (MIT) — server-side no backend |
| Deploy | Railway |
| Containerização | Docker (multi-stage build) |

> **Atenção:** todos os projetos devem usar a mesma major version do EF Core e Npgsql (`10.*`). Mismatch causa `MissingMethodException` em runtime nos testes de integração.

---

## Estrutura do Repositório

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

> `docs/` está no `.gitignore` — arquivos de referência ficam apenas localmente.

---

## Comandos

Execute a partir da raiz da solução (onde está o `.sln`):

```bash
# Restaurar dependências
dotnet restore

# Build
dotnet build

# Rodar a API
dotnet run --project src/Numerologia.Api

# Rodar o frontend Blazor
dotnet run --project src/Numerologia.Web

# Rodar todos os testes
dotnet test

# Rodar uma classe de teste específica
dotnet test --filter "FullyQualifiedName~CalculoMapaTests"

# Rodar testes com cobertura
dotnet test --collect:"XPlat Code Coverage"

# Aplicar migrations do EF Core
dotnet ef database update --project src/Numerologia.Infrastructure --startup-project src/Numerologia.Api

# Criar nova migration
dotnet ef migrations add <NomeDaMigration> --project src/Numerologia.Infrastructure --startup-project src/Numerologia.Api

# Verificar pacotes vulneráveis
dotnet list package --vulnerable
```

---

## Ambiente de Desenvolvimento Local

```bash
docker compose up
```

Sobe a API em `http://localhost:8080` e o PostgreSQL em `localhost:5432`.

---

## Testes

### Pacotes

| Finalidade | Pacote | Versão |
|------------|--------|--------|
| Framework | `xunit` | v2.x |
| Mocking | `NSubstitute` | v5.x |
| Componentes Blazor | `bunit` | v1.x |
| Assertions | `FluentAssertions` | **`~7.*` only** |
| Dados de teste | `Bogus` | v35.x |
| Cobertura | `coverlet.collector` | v6.x |
| Integração | `Microsoft.AspNetCore.Mvc.Testing` | built-in |

> **FluentAssertions v8+ passou para licença comercial em Jan/2025. Fixar em v7.x:**
> ```xml
> <PackageReference Include="FluentAssertions" Version="7.*" />
> ```

### Convenções

- Testes unitários em `Numerologia.UnitTests`, espelhando a estrutura dos projetos fonte.
- Testes de integração usam `WebApplicationFactory<Program>` com SQLite in-memory (substitui PostgreSQL via `ConfigureTestServices`). A factory chama `EnsureCreated()` — **não chamar** `Database.Migrate()` em testes.
- Testes de componentes Blazor usam `bUnit`; mockar `IJSRuntime` — bUnit não executa JavaScript.
- Usar `Bogus` para dados realistas com constraints explícitas; evitar magic strings.

### bUnit + MudBlazor — setup obrigatório

Todo `TestContext` que renderize componentes MudBlazor precisa:

```csharp
public MinhaClasseTests()
{
    JSInterop.Mode = JSRuntimeMode.Loose;
    Services.AddMudServices();
}
```

Sem isso: `InvalidOperationException: Cannot provide a value for 'Localizer' on type 'MudInput'`.

**`MudMenu` / `MudPopover`:** itens abertos aparecem no `MudPopoverProvider`, não no `cut`:
```csharp
private readonly IRenderedComponent<MudPopoverProvider> _popoverProvider;
// no construtor — antes de qualquer RenderComponent:
_popoverProvider = RenderComponent<MudPopoverProvider>();
```

**`MudTextField` com `Immediate="true"`:** usar `element.Input("valor")` (não `.Change()`) nos testes.

**`data-testid` em `MudTd`:** o atributo fica no `<td>`, não no `<tr>`. Seletor: `td[data-testid^='...']`.

**Href com parâmetros int:** `Href="@($"/path/{id}")"` — nunca `Href="/path/@id"` (erro de compilação).

---

## Segurança

| Ferramenta | Tipo | Observação |
|-----------|------|------------|
| `SecurityCodeScan` NuGet | SAST | Roda em build via Roslyn analyzer (SonarCloud free não cobre repos privados) |
| GitHub Dependabot | Dependency scanning | Abre PRs automáticos para pacotes vulneráveis |
| `dotnet list package --vulnerable` | Dependency scanning | Rodar localmente antes de commitar e no CI |
| GitLeaks | Secret scanning | Hook pre-commit + GitHub Actions |
| `NetEscapades.AspNetCore.SecurityHeaders` | HTTP headers | Middleware na API |
| OWASP ZAP + Playwright | DAST | Roda contra staging no CI antes do deploy para produção |

---

## CI/CD — GitHub Actions

### Fluxo de branches

```
feature/* ──PR──► staging ──PR──► main
              CD→ Railway Staging    CD→ Railway Production
```

| Branch | Workflow | O que faz |
|--------|----------|-----------|
| `feature/*` | `ci.yml` | build + test + security |
| `staging` | `cd-staging.yml` | CI completo + deploy Railway Staging |
| `main` | `cd-production.yml` | CI completo + deploy Railway Production |

- `cd-production.yml` tem concorrência bloqueada (`cancel-in-progress: false`) — nunca abortar deploy de produção.

### Branch protection

**`staging`:** PR obrigatório + CI verde.  
**`main`:** PR obrigatório + CI verde + 1 aprovação + stale reviews descartados.

### Segredos e variáveis no GitHub

| Nome | Tipo | Onde configurar |
|------|------|----------------|
| `RAILWAY_TOKEN` | Secret | Settings → Secrets → Actions (repository-level) |
| `RAILWAY_SERVICE` | Variable | Settings → Environments → `staging`/`production` → Variables |
| `RAILWAY_ENVIRONMENT` | Variable | Settings → Environments → `staging`/`production` → Variables |
| `RAILWAY_STAGING_URL` | Variable | Settings → Environments → `staging` → Variables |
| `RAILWAY_PRODUCTION_URL` | Variable | Settings → Environments → `production` → Variables |

> `RAILWAY_SERVICE` e `RAILWAY_ENVIRONMENT` devem estar dentro do **GitHub Environment**, não como variáveis de repositório.  
> O `RAILWAY_TOKEN` é gerado em: Railway Dashboard → Account Settings → Tokens.

---

## Google OAuth

- Client ID e Secret ficam como variáveis de ambiente no Railway (`GOOGLE_CLIENT_ID`, `GOOGLE_CLIENT_SECRET`). Nunca no código.
- Fluxo: Blazor navega para `/auth/login` → API emite challenge → Google OAuth → callback em `/signin-google` → cookie de sessão → redirect para `/`.
- Blazor chama `/auth/me` via `HttpClient` → retorna `{ nome, email }` ou 401.
- `UseForwardedHeaders` é obrigatório **antes** de `UseAuthentication` (proxy HTTPS do Railway).
- Redirect URIs devem ser cadastradas no Google Cloud Console:
  - Staging: `https://<slug>.up.railway.app/signin-google`
  - Production: `https://<dominio>/signin-google`

---

## Railway — Deploy e Configuração

- Deploy acionado pelo GitHub Actions CD (não pelo auto-deploy nativo do Railway).
- **Desabilitar auto-deploy nativo** em todos os environments (Railway Dashboard → serviço → Settings → Source).
- `DATABASE_URL` é injetada pelo plugin PostgreSQL do Railway no formato URI. A função `ToNpgsqlConnectionString()` em `Program.cs` converte para key-value (Npgsql).
- Migrations aplicadas automaticamente no startup via `Database.Migrate()` (com guard `IsNpgsql()`).
- Não usar `UseHttpsRedirection` — Railway termina HTTPS externamente.

### Gotchas conhecidos

| Problema | Causa | Solução |
|----------|-------|---------|
| Blazor travado em 100% / 404 em `_framework/*.dat` | `UseStaticFiles` não serve extensões desconhecidas | `ServeUnknownFileTypes = true` + mapear `.dat`, `.blat`, `.wasm` |
| Redirect loop | `UseHttpsRedirection` conflita com proxy TLS do Railway | Remover `UseHttpsRedirection` |
| Docker build falha "unable to find python in PATH" | `wasm-tools` requer Python + Emscripten | Não instalar `wasm-tools` — publish sem ele roda em modo interpretado |
| GitLeaks 403 em PRs | `GITHUB_TOKEN` sem permissão de leitura de PRs | `permissions: pull-requests: read` no workflow |
| GitLeaks "ambiguous argument" | Checkout shallow sem histórico | `fetch-depth: 0` no checkout |
| Variáveis Railway não encontradas | Configuradas como Repository Variables | Mover para dentro do **GitHub Environment** |
| `Format of the initialization string` | `DATABASE_URL` em formato URI, Npgsql espera key-value | `ToNpgsqlConnectionString()` em `Program.cs` |
| OAuth não redireciona | `ForwardedHeaders` ausente | `UseForwardedHeaders` antes de `UseAuthentication` |
| Migrations não aplicadas no 1º deploy | Railway não roda `dotnet ef database update` | `Database.Migrate()` no startup com guard `IsNpgsql()` |
| "Deployment cancelled" bloqueia PR merge | Auto-deploy Railway + CD GitHub Actions simultaneamente | Desabilitar auto-deploy no Railway |
