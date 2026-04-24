# Planejamento de Desenvolvimento — Numerologia CRM

## Visão do Produto

CRM para numerólogos cabalísticos. A numeróloga cadastra seus consulentes, gera mapas numerológicos completos e acompanha o histórico de cada cliente. O sistema automatiza todos os cálculos que hoje são feitos à mão no Gráfico Numerológico.

**Referências:**
- Domínio: `docs/numerologia.md`
- Stack técnica: `docs/stack.md`
- Livro base: "Numerologia Cabalística: A Última Fronteira" — Carlos Rosa

---

## Modelo de Dados

### Entidades principais

```
Numeróloga (usuária autenticada via Google OAuth)
    └── Consulente (cliente)
            ├── Nome completo
            ├── Data de nascimento
            ├── Contatos (email, telefone, etc.)
            └── MapaNumerologico (1..N por consulente)
                    ├── Nome utilizado (certidão ou nome de casada)
                    ├── Data de nascimento (snapshot)
                    ├── Data de criação do mapa
                    └── Todos os campos calculados (persistidos)
```

### Campos do MapaNumerologico

**Do nome:**
| Campo | Tipo | Descrição |
|-------|------|-----------|
| `GradeLetras` | JSON | Letra, tipo (vogal/consoante), valor cabalístico — para exibição célula a célula |
| `NumeroMotivacao` | int | Soma vogais → reduzido |
| `NumeroImpressao` | int | Soma consoantes → reduzido |
| `NumeroExpressao` | int | Soma total → reduzido |
| `TotalLetrasPorNumero` | JSON | Dicionário int→int (1-9: contagem) — Fig. A |
| `LicoesCarmicas` | int[] | Valores 1-8 ausentes do nome — Fig. B |
| `TendenciasOcultas` | int[] | Valores mais frequentes — Fig. C |
| `RespostaSubconsciente` | int | Quantidade de lições cármicas — Fig. D |
| `DividasCarmicas` | int[] | Somas intermediárias = 13/14/16/19 |

**Da data de nascimento:**
| Campo | Tipo | Descrição |
|-------|------|-----------|
| `MesNascimentoReduzido` | int | Mês → reduzido — Fig. D |
| `DiaNascimentoReduzido` | int | Dia → reduzido — Fig. D |
| `AnoNascimentoReduzido` | int | Ano → reduzido — Fig. D |
| `NumeroDestino` | int | Todos dígitos da data → reduzido — Fig. E |
| `Missao` | int | Destino + Expressão → reduzido — Fig. E |
| `CicloVida1` | int | Mês reduzido — Fig. F |
| `CicloVida2` | int | Dia reduzido — Fig. F |
| `CicloVida3` | int | Ano reduzido — Fig. F |
| `FimCiclo1Idade` | int | 36 − Destino |
| `FimCiclo2Idade` | int | FimCiclo1 + 27 |
| `Desafio1` | int | \|Mês − Dia\| — Fig. H |
| `Desafio2` | int | \|Ano − Dia\| — Fig. H |
| `DesafioPrincipal` | int | \|Desafio2 − Desafio1\| — Fig. H |
| `MomentoDecisivo1` | int | Dia + Mês → reduzido — Fig. G |
| `MomentoDecisivo2` | int | Dia + Ano → reduzido — Fig. G |
| `MomentoDecisivo3` | int | MD1 + MD2 → reduzido — Fig. G |
| `MomentoDecisivo4` | int | Mês + Ano → reduzido — Fig. G |
| `DiasMesFavoraveis` | int[] | Da tabela por dia/mês nasc. |
| `NumerosHarmonicos` | int[] | Da tabela por dia/mês nasc. |

**Calculados dinamicamente (não persistir — dependem do ano/mês/dia atual):**
| Campo | Cálculo |
|-------|---------|
| `AnoPassoal` | Dia nasc + Mês nasc + Ano atual → reduzir |
| `MesPessoal` | Ano Pessoal + mês atual → reduzir |
| `DiaPessoal` | Mês Pessoal + dia atual → reduzir |

**Do Nº de Expressão (tabelas fixas):**
| Campo | Tipo | Descrição |
|-------|------|-----------|
| `RelacaoIntervalores` | int | Impressão − Motivação |
| `HarmoniaConjugal` | JSON | { VibraCom, Atrai, EOpostoA, EPassivo } |
| `CoresFavoraveis` | string[] | Lista de cores por Expressão |

---

## Roadmap de Features

### Fase 1 — Fundação (MVP)
> Objetivo: cadastrar consulente e gerar mapa com todos os cálculos na tela.

- [x] **F1.1** — Cadastro de Consulente ✅
  - Nome completo, data de nascimento, email, telefone (opcional)
  - CRUD completo com listagem
  - Entidade `Consulente` + migration + repositório + 5 endpoints API
  - Blazor: `ListaConsulentes`, `FormConsulente`, `IConsulentesService`
  - Testes: unit (entidade), integração (endpoints + repositório), bUnit (componentes)

- [x] **F1.2** — Motor de Cálculo (core) ✅
  - Tabela cabalística completa (a–z + diacríticos: ´^`..~o)
  - Redução numérica (com suporte a mestres 11 e 22)
  - Motivação, Impressão, Expressão
  - Fig. A (contagem por número)
  - Lições Cármicas, Tendências Ocultas, Resposta Subconsciente
  - Dívidas Cármicas (detecção de 13/14/16/19 intermediários)
  - Destino, Missão
  - Dia/Mês/Ano do nascimento reduzidos
  - Ciclos de Vida (com durações)
  - Desafios (1º, 2º, Principal)
  - Momentos Decisivos (1º ao 4º)
  - Dias do Mês Favoráveis — `TabelaNumerosFavoraveis` (~370 entradas)
  - Números Harmônicos — `TabelaNumerosHarmonicos`
  - Relação Intervalores
  - Harmonia Conjugal — `TabelaHarmoniaConjugal`
  - Cores Favoráveis — `TabelaCoresFavoraveis`
  - Ano/Mês/Dia Pessoal (dinâmico) — `CalculosPessoais`

- [x] **F1.3** — Geração do Mapa ✅
  - Entidade `MapaNumerologico` com snapshot de todos os campos calculados
  - `GeradorMapa` orquestra `CalculoMapa` + `CalculoDestino` + tabelas fixas
  - `IMapasRepository` / `MapasRepository` (multi-tenant: filtra por usuário)
  - Migration `AddMapaNumerologico` (FK cascade para Consulente)
  - 3 endpoints API: POST/GET lista/GET detalhe sob `/consulentes/{id}/mapas`
  - Blazor: `ListaMapas`, `FormMapa`, `IMapasService`, botão "Mapas" na lista de consulentes
  - Testes: unit (entity + service), integração (endpoints), bUnit (componentes)

- [x] **F1.3.1** — Ícone de acesso aos Mapas na lista de Consulentes ✅
  - Substituído botão "Mapas" por ícone `bi-file-text` (Bootstrap Icons) com `aria-label="Mapas"` e `title="Mapas"`

- [x] **F1.4** — Exibição do Gráfico Numerológico ✅
  - Layout fiel ao Gráfico Numerológico (imagem de referência em `docs/privado/`)
  - Grade de letras (Vogais/Nome/Consoante/TOTAL) com valores cabalísticos por célula
  - Todos os campos calculados preenchidos automaticamente
  - Campos dinâmicos (Ano/Mês/Dia Pessoal) recalculados ao abrir via `/api/calculos/pessoal`
  - `GradeLetras` adicionada a `ResultadoMapa`, `MapaNumerologico` e API

- [x] **F1.5** — Persistência da sessão (cookie) ✅
  - Cookie com `IsPersistent=true`, `ExpireTimeSpan=30d`, `SlidingExpiration=true`
  - `DataProtection` persistido no banco via `IDataProtectionKeyContext` — sobrevive a redeploys no Railway
  - Migration `AddDataProtectionKeys` criada

- [x] **F1.6** — Hardening de segurança ✅
  - **Security Headers**: `NetEscapades.AspNetCore.SecurityHeaders` ativado — CSP, HSTS, X-Frame-Options, X-Content-Type-Options, Referrer-Policy
  - **Exception Handler global**: `app.UseExceptionHandler()` — retorna JSON genérico, nunca stack trace em produção
  - **DateOnly.Parse seguro**: `DateOnly.TryParse()` com `400 BadRequest` nos endpoints POST/PUT e range check em `/api/calculos/pessoal`
  - **Rate Limiting**: 60 req/min geral por usuário; 10 req/min no `POST /api/consulentes/{id}/mapas`
  - **Cookie flags**: `HttpOnly=true`, `SameSite=Lax`, `SecurePolicy=SameAsRequest`
  - **Validação de input**: `[MaxLength]` nos records de request

---

### Fase 2 — Experiência
> Objetivo: melhorar usabilidade e adicionar contexto ao mapa.

- [ ] **F2.1** — Interpretações por número
  - Para cada campo do mapa, exibir o significado do número calculado
  - Baseado no `docs/numerologia.md`

- [ ] **F2.2** — Dashboard da numeróloga
  - Últimos mapas criados
  - Total de consulentes

- [ ] **F2.3** — Busca e filtros
  - Buscar consulente por nome
  - Filtrar mapas por data

---

### Fase 3 — PDF
> Objetivo: gerar o Gráfico Numerológico preenchido em PDF para impressão.

- [ ] **F3.1** — Geração de PDF
  - Layout idêntico ao gráfico físico (`docs/Screenshot 2026-04-23 at 23.58.51.png`)
  - Download direto pelo sistema

---

## Ordem de Desenvolvimento (TDD — XP)

Cada item abaixo é uma user story. Seguir o ciclo Red → Green → Blue.  
Cada story só vai para staging quando tiver backend + frontend + testes completos (vertical slice).

```
1. CalculoMapa (motor de cálculo) — testes unitários primeiro
   └── TabelaCabalistica, Reducao, Motivacao, Impressao, Expressao
   └── FiguraA, LicoesCarmicas, TendenciasOcultas, RespostaSubconsciente
   └── DividasCarmicas, Destino, Missao
   └── Ciclos, Desafios, MomentosdDecisivos
   └── Tabelas (DiasFavoraveis, NumerosHarmonicos, HarmoniaConjugal, Cores)

2. Entidade Consulente + API CRUD
   └── POST /consulentes, GET /consulentes, GET /consulentes/{id}
   └── PUT /consulentes/{id}, DELETE /consulentes/{id}

3. Entidade MapaNumerologico + geração via API
   └── POST /consulentes/{id}/mapas → calcula e persiste
   └── GET /consulentes/{id}/mapas
   └── GET /consulentes/{id}/mapas/{mapaId}

4. Frontend — Listagem e cadastro de consulentes (Blazor)

5. Frontend — Criação e exibição do Gráfico Numerológico (Blazor)
```

---

## Decisões Técnicas

| Decisão | Escolha | Motivo |
|---------|---------|--------|
| Cálculos no backend | `Numerologia.Core/Calculos/` | Testável, reutilizável, não expõe lógica no frontend |
| Campos dinâmicos | Não persistir Ano/Mês/Dia Pessoal | Dependem do dia atual; recalcular ao exibir |
| Tabelas fixas | Hardcoded em C# (não no banco) | São constantes do domínio (livro); não mudam |
| PDF | Fase 3 (não agora) | Complexidade alta; tela primeiro |
| Mapa armazenado | Snapshot completo no banco | Mapa não deve mudar se a lógica de cálculo mudar |
| Nomes mestres | 11 e 22 nunca reduzidos | Regra do livro — verificar em todo ponto de redução |
