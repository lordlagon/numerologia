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

- [x] **F1.7** — Verificar e testar DELETE CASCADE nas tabelas de Consulente ✅
  - **Contexto:** o `AppDbContext` já configura `OnDelete(DeleteBehavior.Cascade)` nas FKs `Usuario → Consulente` e `Consulente → MapaNumerologico`, mas não há testes que validem o comportamento em cascata
  - Verificar que a migration aplicada no PostgreSQL contém `ON DELETE CASCADE` nas FKs relevantes
  - Criar teste de integração: ao deletar um Consulente, todos os seus MapaNumerologicos devem ser removidos automaticamente
  - Criar teste de integração: ao deletar um Usuario, todos os seus Consulentes (e consequentemente Mapas) devem ser removidos
  - Validar que o endpoint `DELETE /consulentes/{id}` não retorna erro de FK constraint ao excluir consulente com mapas
  - Verificar se o frontend (`ListaConsulentes`) exibe confirmação antes da exclusão informando que os mapas também serão excluídos

- [x] **F1.8** — Ajustar lista de Mapas numerológicos ✅
  - Incluir data de nascimento após o nome
  - Incluir os valores de Motivação e Impressão de cada mapa na listagem

---

### Fase 2 — Experiência
> Objetivo: melhorar usabilidade e adicionar contexto ao mapa.

- [x] **F2.1** — Delete e Editar Mapa ✅
  - `DELETE /consulentes/{id}/mapas/{mapaId}` → 204
  - `PUT /consulentes/{id}/mapas/{mapaId}` → 200 (recalcula todos os números do nome)
  - Botão Excluir com `confirm()` na `ListaMapas`
  - Página `EditarMapa.razor` em `/consulentes/{id}/mapas/{mapaId}/editar`

- [x] **F2.2** — Interpretações por número ✅
  - Para cada campo do mapa, exibir o significado do número calculado
  - Baseado no `docs/numerologia.md`

- [x] **F2.3** — Cores favoráveis (visual) ✅
  - Além do texto das cores, incluir a cor no fundo do badge
  - Ex: "Vermelho" com fundo vermelho, "Azul" com fundo azul
  - Ajustar cor do texto para contraste (fundo escuro → texto branco, fundo claro → texto preto)

- [x] **F2.4** — Dashboard da numeróloga ✅
  - Card com total de consulentes
  - Lista dos últimos 5 mapas criados com link direto para o gráfico
  - Home redireciona para `/dashboard` quando autenticado

- [x] **F2.5** — Busca por nome de consulente ✅
  - Input de busca filtra em tempo real (oninput, case-insensitive)
  - Sem resultado exibe mensagem "Nenhum consulente encontrado"

- [x] **F2.6** — Menu de usuário na barra superior ✅
  - Dropdown com nome, email, link `/perfil` (placeholder) e botão Sair
  - Substituiu o link "About" do template padrão
  - Controlado por Blazor (sem Bootstrap JS / Popper.js)

- [x] **F2.7** — Ajuste no Quadro Fig. F - Ciclos da vida: ✅
  - Incluir nova coluna para conter os anos respectivos
  - Incluir o ano do nascimento em cada ciclo por exemplo no mapa do André:
    ciclo 1 -> 7 -> até 33 anos. deve ficar ciclo 1 -> 7 -> de 1984 até 2017 -> até 33 anos
    ciclo 2 -> 1 -> até 60 anos. deve ficar ciclo 2 -> 1 -> de 2017 até 2044 -> até 60 anos
    ciclo 3 -> 7 -> em diante. deve ficar ciclo 3 -> 22 -> de 2044 em diante -> em diante

- [x] **F2.8** — Ajuste no Quadro Fig. G - Momentos decisivos: ✅
  - Incluir nova coluna para conter os anos respectivos, da mesma forma que o F2.7.
    - Duração: MD1 = igual ao 1º Ciclo de Vida (FimCiclo1Idade); MD2 e MD3 = 9 anos cada; MD4 = resto da vida (livro pág. 196)
    - Exemplo no mapa do André (nascimento 28/07/1984, FimCiclo1=33):
      momento 1 -> 8. deve ficar momento 1 -> 8 -> de 1984 até 2017
      momento 2 -> 5. deve ficar momento 2 -> 5 -> de 2017 até 2026
      momento 3 -> 4. deve ficar momento 3 -> 4 -> de 2026 até 2035
      momento 4 -> 11. deve ficar momento 4 -> 11 -> de 2035 em diante

- [x] **F2.9** — Ajuste no Quadro Fig. 4 - Data nascimento: ✅
  - Incluir a soma dos valores da data de nascimento por exemplo no mapa do André:
      Mês -> 07 => 7
      Dia -> 28 => 1
      Ano -> 1984 => 22

- [x] **F2.10** — Verificação das interpretações: ✅
  - Confirmar no livro se exitem interpretações para os outros quadros do mapa, 
  percebi isso no relações intervalores (pag. 203) que tem interpretação e não foi incluida na secção.     

---

### Fase 3 — PDF
> Objetivo: gerar o Gráfico Numerológico preenchido em PDF para impressão.

- [ ] **F3.1** — Geração de PDF
  - **Biblioteca:** QuestPDF (MIT) — geração server-side no backend
  - **Endpoint API:** `GET /consulentes/{id}/mapas/{mapaId}/pdf` → `application/pdf`
  - **Estrutura do PDF:**
    - **Capa** (pág. 1): título "Mapa Numerológico Cabalístico", nome do Numerólogo (editável — campo no perfil), logo opcional
    - **Cabeçalho** (todas as páginas): nome do consulente + data de nascimento
    - **Gráfico** (pág. 2): layout fiel ao gráfico físico (`docs/privado/Screenshot 2026-04-23 at 23.58.51.png`) — grade de letras, Fig. A–H, ciclos, momentos, harmonia, cores
    - **Interpretações** (pág. 3+): todos os campos com texto interpretativo (mesmo conteúdo da seção Interpretações na tela)
  - **Frontend:** botão "Baixar PDF" na tela do gráfico → download via endpoint
  - **Testes:** unit (gerador produz bytes não-nulos), integração (endpoint retorna 200 + `application/pdf`)

---

### Fase 4 — Pirâmides da Vida
> Objetivo: gerar as piramides da vida.

- [x] **F4.1** — Botão "Pirâmides" na lista de mapas ✅
  - Botão ao lado de "Ver Mapa" em cada linha da `ListaMapas`
  - Inicialmente desabilitado (`disabled`) — telas das Pirâmides da Vida ainda não desenvolvidas
  - Será habilitado quando a funcionalidade de Pirâmides estiver pronta

- [ ] **F4.2** — Inclusão do arquivo de referencia (sheets)
  - Inclusão do arquivo de referencia (sheets)
  - Informar como é a lógica da criação das pirâmides

- [ ] **F4.3** — Geração de PDF da piramides
  - Incluir imagem de referencia (ainda não incluido)
  - Layout idêntico ao gráfico 
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
