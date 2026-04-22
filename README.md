<p align="center">
  <img src="docs/logo.png" alt="Numerologia Cabalística" width="120" />
</p>

<h1 align="center">Numerologia Cabalística</h1>

<p align="center">
  Aplicação de numerologia cabalística baseada no livro de <strong>José Carlos Rosa</strong><br/>
  Calcule seu Mapa Numerológico com mapa de pirâmide, destino, motivação e impressão
</p>

<p align="center">
  <img alt="Build" src="https://github.com/lordlagon/numerologia/actions/workflows/ci.yml/badge.svg" />
  <img alt=".NET" src="https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet" />
  <img alt="Blazor" src="https://img.shields.io/badge/Blazor-WASM-512BD4?logo=blazor" />
  <img alt="PostgreSQL" src="https://img.shields.io/badge/PostgreSQL-16-336791?logo=postgresql&logoColor=white" />
  <img alt="Railway" src="https://img.shields.io/badge/Deploy-Railway-0B0D0E?logo=railway" />
  <img alt="License" src="https://img.shields.io/badge/license-MIT-green" />
</p>

---

## Visão Geral

**Numerologia Cabalística** é uma aplicação web full-stack que implementa a metodologia do livro de José Carlos Rosa. A partir do nome e data de nascimento do usuário, gera um mapa numerológico completo com:

- **Motivação** — calculado pelas vogais do nome
- **Impressão** — calculado pelas consoantes do nome
- **Destino** — calculado a partir da data de nascimento
- **Pirâmide** — sequências de redução cabalística sobre o mapa de letras

---

## Stack

| Camada | Tecnologia |
|--------|-----------|
| Backend | ASP.NET Core Web API (.NET 8) |
| Frontend | Blazor WebAssembly |
| Banco de Dados | PostgreSQL 16 (EF Core + Migrations) |
| Autenticação | Google OAuth 2.0 |
| Deploy | Railway |
| ORM | Entity Framework Core + Npgsql |
| Testes | xUnit + NSubstitute + bUnit + FluentAssertions |

---

## Estrutura do Projeto

```
/
├── src/
│   ├── Numerologia.Api/            # ASP.NET Core Web API
│   ├── Numerologia.Web/            # Blazor WebAssembly
│   ├── Numerologia.Core/           # Domínio, entidades, DTOs
│   └── Numerologia.Infrastructure/ # EF Core, repositórios
├── tests/
│   ├── Numerologia.UnitTests/      # Testes unitários (xUnit)
│   └── Numerologia.IntegrationTests/ # Testes de integração (WebApplicationFactory)
└── docs/                           # Documentação e imagens de referência
```

---

## Como Rodar Localmente

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL 16](https://www.postgresql.org/)
- Conta Google (para OAuth)

### Configuração

```bash
# 1. Clone o repositório
git clone https://github.com/lordlagon/numerologia.git
cd numerologia

# 2. Restaurar dependências
dotnet restore

# 3. Configurar variáveis de ambiente
#    Crie um appsettings.Development.json em src/Numerologia.Api/
#    com ConnectionStrings, Google:ClientId e Google:ClientSecret

# 4. Aplicar migrations
dotnet ef database update \
  --project src/Numerologia.Infrastructure \
  --startup-project src/Numerologia.Api

# 5. Rodar a API
dotnet run --project src/Numerologia.Api

# 6. Rodar o frontend (outro terminal)
dotnet run --project src/Numerologia.Web
```

---

## Testes

```bash
# Todos os testes
dotnet test

# Filtrar por classe
dotnet test --filter "FullyQualifiedName~CalculoMapaTests"

# Com cobertura
dotnet test --collect:"XPlat Code Coverage"
```

---

## Segurança

| Ferramenta | Finalidade |
|-----------|-----------|
| SecurityCodeScan | SAST via Roslyn analyzer em build |
| GitHub Dependabot | PRs automáticos para pacotes vulneráveis |
| GitLeaks | Detecção de segredos em pre-commit e CI |
| NetEscapades.SecurityHeaders | HTTP Security Headers na API |
| OWASP ZAP + Playwright | DAST contra ambiente de staging |

---

## CI/CD

O GitHub Actions roda automaticamente em todo push e PR:

1. `build` — `dotnet build`
2. `test` — `dotnet test` com coleta de cobertura
3. `security` — `dotnet list package --vulnerable` + GitLeaks

Branch `main` protegida: todos os checks devem passar + aprovação obrigatória.

Todo merge na `main` aciona deploy automático no **Railway**.

---

## Lógica de Numerologia

Toda a lógica de cálculo fica em `Numerologia.Core/Calculos/`:

- **Tabela Cabalística** — mapeamento letra → número (p. 63 do livro de referência)
- **Redução** — soma os dígitos de um número até resultar em um único dígito
- **Pirâmide** — soma de dígitos adjacentes com redução cabalística (subtrai 9 se > 9)
- **Motivação** — vogais do nome reduzidas
- **Impressão** — consoantes do nome reduzidas
- **Destino** — dígitos da data de nascimento reduzidos

---

## Metodologia — Extreme Programming (XP)

Este projeto segue as práticas de **Extreme Programming**. Não usamos Scrum (sem sprints, sem velocity, sem burndowns).

### Práticas adotadas

| Prática | Como se aplica |
|---------|---------------|
| **TDD** | Escreva o teste que falha primeiro, depois o código mínimo para passar, depois refatore |
| **Integração Contínua** | Todo push em feature branch executa build + testes + segurança via GitHub Actions |
| **Small Releases** | Branches vivem no máximo 1–2 dias; cada PR entrega uma fatia vertical funcionando |
| **Design Simples** | YAGNI — sem abstrações especulativas, sem código para o futuro |
| **Refatoração Contínua** | Após cada teste verde, melhore o design; os testes são a rede de segurança |
| **Propriedade Coletiva** | Qualquer área do código pode ser alterada por qualquer pessoa, desde que os testes passem |
| **Ritmo Sustentável** | Não acumule dívida técnica para ir mais rápido a curto prazo |

### Ciclo TDD

```
Vermelho → escreva um teste que falha descrevendo o comportamento desejado
Verde    → escreva o mínimo de código de produção para o teste passar
Azul     → refatore teste e código de produção sem quebrar os testes
```

Nunca pule o passo **Vermelho**. Sem teste falhando, não há feature.

---

## Contribuindo

> **Pushes diretos para `main` estão bloqueados.** Toda mudança — por menor que seja — precisa de um Pull Request.

1. Crie uma branch a partir da `main`:
   ```bash
   git checkout -b feat/minha-feature
   ```
2. Escreva os testes antes do código (TDD)
3. Faça commits pequenos e frequentes:
   ```bash
   git commit -m 'feat: adiciona minha feature'
   ```
4. Push e abra o PR:
   ```bash
   git push origin feat/minha-feature
   ```
5. Aguarde o CI passar e a aprovação do PR
6. Merge para `main` → deploy automático no Railway

### Convenção de branches

| Prefixo | Quando usar |
|---------|-------------|
| `feat/` | Nova funcionalidade |
| `fix/` | Correção de bug |
| `refactor/` | Melhoria de design sem mudar comportamento |
| `chore/` | Configuração, CI, dependências |

---

## Licença

Distribuído sob a licença MIT. Veja [LICENSE](LICENSE) para mais detalhes.

---

<p align="center">
  Feito com ♥ por <a href="https://github.com/lordlagon">André Deka Macedo</a>
</p>
