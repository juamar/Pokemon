# Copilot Instructions — Pokemon Exercise

> This file is the authoritative, AI-facing entry point for Copilot when working in this repository. `README.md` is for humans only — do not treat it as a spec source; if it ever conflicts with the files below, the files below win.

## Read first

- [docs/requirements.md](../docs/requirements.md) — authoritative spec: exercise rules, domain model, damage formula, type effectiveness matrix, acceptance criteria, assumptions. Source of truth for **what** the system must do.
- [docs/architecture.md](../docs/architecture.md) — solution structure, project responsibilities, SOLID rationale, persistence/DIP design. Source of truth for **how** the system is structured.
- [docs/plan.md](../docs/plan.md) — phased implementation roadmap. Follow phase order when scaffolding or implementing new work.
- [docs/smoke-test-plan.md](../docs/smoke-test-plan.md) — manual/scripted end-to-end checklist covering both APIs together, executed as part of the V1 Definition of Done.

## Guiding principle

Keep it simple. No Clean Architecture ceremony, no CQRS/MediatR, no interfaces "just in case." Only introduce an abstraction when there's a real, concrete need (testability, provider portability, cross-aggregate atomicity) — see `docs/architecture.md` for the specific decisions already made and their justifications.

## Conventions to follow

- **Solution layout**: `Pokedex.API` (Part 2, Pokemon resources), `Battles.API` (Part 3, battle state), `Pokemons.Domain` (entities + pure logic, no EF references), `Pokemons.Infra` (EF Core + SQLite, repository implementations), `Pokemons.Tests` (unit), `Pokemons.IntegrationTests` (integration). See `docs/architecture.md` for the full tree and rationale.
- **Repositories are per aggregate root**, not per table. `Battle` and `BattleAction` share `IBattleRepository` (an action never exists without its parent battle). Lookup/reference data (e.g. type effectiveness) gets a narrow read-only lookup, not a full repository.
- **No service layer for plain CRUD** — `Pokedex.API` controllers call repositories (+ `ISaveChanges`) directly. Only introduce a service/orchestrator class when there's real multi-step or multi-aggregate coordination (e.g. the Battle Turn Orchestrator in `Battles.API`).
- **Prefer concrete classes over interfaces** when there is, and will only ever be, one implementation (e.g. `DamageCalculator`). Add an interface when there's a genuine substitutability need (e.g. `IRandomProvider` for deterministic tests, repository interfaces for provider portability).
- **Persistence is EF Core Code-First**, isolated entirely in `Pokemons.Infra`. `Pokemons.Domain` entities are plain C# POCOs with no EF attributes/types. Provider selection (SQLite today, SQL Server later) is a single DI seam (`AddPokemonsPersistence(...)`).
- Before proposing new abstractions or layers, check `docs/architecture.md`'s "Key Design Decisions" and "SOLID Principles Applied" sections — many of these tradeoffs have already been discussed and decided.

## Working in reviewable units of work

The human must stay in control of every change — never batch multiple phases or unrelated concerns into one uninterrupted run.

- **One unit of work = one `docs/plan.md` step (or a single, clearly-scoped sub-task)**, not a whole phase and not the whole plan. Stop after completing a unit and let the human review before continuing to the next.
- **CRITICAL: Do NOT stage, commit, or push changes without explicit user approval.** Always present your changes and wait for the human to review and say "commit" or "push" before executing any git operations. This is non-negotiable.
- **Pause for review before committing or moving to the next unit.** Do not chain "implement → commit → next step" without an explicit go-ahead. Committing/pushing to git only happens when the human asks for it.
- **Call out new interfaces, services, repositories, or other abstractions explicitly** in the response the moment they're introduced — don't let them appear silently inside a larger multi-file diff.
- **Prefer several small diffs over one large one.** If a task naturally spans multiple files/projects, sequence the edits and summarize each before moving on, rather than editing everything and presenting it all at once.
- **Keep the working tree easy to audit**: after a unit of work, a `git status`/diff should show only the files relevant to that unit — no incidental or drive-by changes.

## Testing requirement per unit of work

- **Every unit of work that touches `Pokemons.Domain` logic (damage calculation, type effectiveness, battle rules, Battle Turn Orchestrator) must include its tests as part of the same unit** — write the test alongside (ideally before, per the TDD-vs-test-after split in `docs/architecture.md`) the implementation, not as a separate deferred step. A unit of work is not "done" until its relevant tests exist and pass.
- **CRUD endpoints in `Pokedex.API`** get an integration test in the same unit that adds the endpoint (test-after is fine here, per `docs/architecture.md`), rather than shipping the endpoint untested.

## Docs reconciliation per unit of work

- **Docs reconciliation is incremental, not just a final step.** At the end of each unit of work, check whether `docs/requirements.md`, `docs/architecture.md`, or `docs/plan.md` need updating because the implementation revealed a divergence from what was written (e.g. a decision changed, a new interface was justified, a phase's scope shifted) — reconcile them in the same unit, not deferred to a final pass.
- **Call out doc drift explicitly** in the response when it's found (e.g. "implementation required X, which differs from architecture.md's Y — updating architecture.md accordingly") rather than silently letting code and docs disagree.
- **The end-of-project "Definition of Done" reconciliation pass (`docs/plan.md`) is a final sweep/safety net**, not the first time docs get reconciled — most drift should already be caught and fixed unit by unit.
