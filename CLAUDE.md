# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

CRM para numerólogos cabalísticos. A numeróloga cadastra consulentes (clientes) e gera **Mapas Numerológicos** automaticamente — digitalizando o Gráfico Numerológico que hoje é preenchido à mão. Baseado no livro "Numerologia Cabalística: A Última Fronteira" de Carlos Rosa.

**GitHub repository:** `https://github.com/lordlagon/numerologia` (private)  
**Remote:** `https://github.com/lordlagon/numerologia.git` — authenticate as `lordlagon`  
**Default branch:** `main`

**No code merges to `main` without a Pull Request and passing CI checks.**

## Reference Documents

| Documento | Versionado | Conteúdo |
|-----------|:----------:|----------|
| `docs/stack.md` | ✅ | Stack técnica, estrutura do repo, comandos, testes, CI/CD, Railway, gotchas |
| `docs/planning.md` | ✅ | Roadmap de features, modelo de dados, ordem de desenvolvimento |
| `docs/privado/numerologia.md` | ❌ | Domínio do livro: todas as fórmulas de cálculo do mapa numerológico |
| `docs/privado/` | ❌ | PDF do livro, imagens de referência, segredos OAuth — ficam apenas localmente |

> `docs/privado/` está no `.gitignore`. Os demais documentos em `docs/` são versionados normalmente.

---

## Pre-commit Checklist

**Obrigatório antes de qualquer commit:**

```bash
# 1. Todos os testes devem passar
dotnet test

# 2. Verificar pacotes vulneráveis
dotnet list package --vulnerable
```

Se `dotnet test` falhar, **não commitar**. Corrigir primeiro.

---

## Session Start Convention

**Ao iniciar qualquer sessão de trabalho:** ler `CLAUDE.md` e os documentos de referência em `docs/` antes de propor qualquer código ou modificação.

---

## Core Numerology Logic

Toda a lógica de cálculo fica em `Numerologia.Core/Calculos/`. As regras completas de cálculo estão em `docs/numerologia.md`.

- `CalculoMapa` — converte letras para valores cabalísticos; calcula Motivação (vogais), Impressão (consoantes), Expressão (total), Fig. A, Lições Cármicas, Tendências Ocultas, Resposta Subconsciente, Dívidas Cármicas.
- `CalculoDestino` — data de nascimento → Destino, Missão, Ciclos de Vida, Desafios, Momentos Decisivos.
- `CalculosPessoais` — Ano/Mês/Dia Pessoal (dinâmicos, não persistidos).
- `GeradorMapa` (em `Core/Services/`) — orquestra `CalculoMapa` + `CalculoDestino` + tabelas fixas → cria entidade `MapaNumerologico` (snapshot persistido).
- `EntradaLetra` / `TipoLetra` — grade de letras do nome (Vogal, Consoante, Espaco) com valor cabalístico por posição; incluída em `ResultadoMapa` e persistida no `MapaNumerologico`.
- **Tabelas fixas** — constantes do domínio, hardcoded em C# (não no banco):
  - `TabelaHarmoniaConjugal` — relação entre números (VibraCom, Atrai, EOpostoA, ProfundamenteOpostoA, EPassivoEm); keyed por número 1–9.
  - `TabelaCoresFavoraveis` — cores favoráveis por número de Expressão 1–9.
  - `TabelaNumerosFavoraveis` — números favoráveis por `(dia, mês)` de nascimento; ~370 entradas.
  - `TabelaNumerosHarmonicos` — números que se harmonizam / são neutros por `diaReduzido` 1–9.

O projeto Java em `Numerologia-Java/` é referência histórica — não adicionar features lá. Está no `.gitignore` e não deve ser commitado.

---

## Process — Extreme Programming (XP)

| Prática | Aplicação |
|---------|-----------|
| **TDD** | Teste falho primeiro, depois mínimo de código, depois refactor. Sem código de produção sem teste. |
| **Pair Programming** | Claude Code é o par — revisar toda sugestão antes de aceitar. |
| **Small Releases** | Feature branches curtas (1–2 dias). Cada PR entrega uma fatia vertical completa. |
| **Simple Design** | YAGNI — só o que é necessário agora. Sem abstrações especulativas. |
| **Collective Ownership** | Qualquer área do código pode ser alterada, desde que os testes passem. |

### TDD Cycle

```
Red   → escrever teste que falha descrevendo o comportamento desejado
Green → escrever o mínimo de código para o teste passar
Blue  → refatorar sem quebrar os testes
```

### Branching & PR Rules

- **`main` é sempre deployável.** Push direto bloqueado.
- Todo código vai por **Pull Request**, sem exceção.
- Convenção: `feat/<slug>`, `fix/<slug>`, `refactor/<slug>`, `chore/<slug>`.
- **PRs de branches de trabalho → `staging` (não `main`).**
- `staging` → `main` é feito separadamente após validação em staging.
- CI deve estar verde antes do merge.
- PR deve ter pelo menos 1 aprovação.
- PRs pequenos: uma user story ou um bug fix por PR.

### Ritual pós-merge em `main`

Após qualquer merge de `staging` → `main`, e **antes de iniciar nova feature**:

1. **Atualizar `staging`** com o estado de `main`:
   ```bash
   git checkout staging && git merge main && git push origin staging
   ```
2. **Apagar branches já mergeadas em `main`** (local e remoto):
   ```bash
   # listar candidatas
   git branch --merged main | grep -v "main\|staging"
   # apagar local
   git branch -d <branch>
   # apagar remoto
   git push origin --delete <branch>
   ```
3. **Manter** qualquer branch em que ainda haja trabalho em andamento.

### Vertical Slice Rule

Um PR só vai para `staging` quando estiver completo em todas as camadas:

| Camada | Requisito |
|--------|-----------|
| **DB** | Migration criada e aplicada |
| **Backend (API)** | Endpoints implementados e testados (unit + integration) |
| **Frontend (Blazor)** | Componentes implementados e testados com bUnit |
