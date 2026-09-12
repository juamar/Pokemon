# Copilot Instructions — Pokemon Exercise

> This file is the authoritative, AI-facing entry point for Copilot when working in this repository. `README.md` is for humans only — do not treat it as a spec source; if it ever conflicts with the files below, the files below win.

## Read first

- [docs/requirements.md](../docs/requirements.md) — authoritative spec: exercise rules, domain model, damage formula, type effectiveness matrix, acceptance criteria, assumptions. Source of truth for **what** the system must do.
- [docs/architecture.md](../docs/architecture.md) — solution structure, project responsibilities, SOLID rationale, persistence/DIP design. Source of truth for **how** the system is structured.
- [docs/plan.md](../docs/plan.md) — phased implementation roadmap. Follow phase order when scaffolding or implementing new work.

## Guiding principle

Keep it simple. No Clean Architecture ceremony, no CQRS/MediatR, no interfaces "just in case." Only introduce an abstraction when there's a real, concrete need (testability, provider portability, cross-aggregate atomicity) — see `docs/architecture.md` for the specific decisions already made and their justifications.

## Conventions to follow

- **Solution layout**: `Pokedex.API` (Part 2, Pokemon resources), `Battles.API` (Part 3, battle state), `Pokemons.Domain` (entities + pure logic, no EF references), `Pokemons.Infra` (EF Core + SQLite, repository implementations), `Pokemons.Tests` (unit), `Pokemons.IntegrationTests` (integration). See `docs/architecture.md` for the full tree and rationale.
- **Repositories are per aggregate root**, not per table. `Battle` and `BattleAction` share `IBattleRepository` (an action never exists without its parent battle). Lookup/reference data (e.g. type effectiveness) gets a narrow read-only lookup, not a full repository.
- **No service layer for plain CRUD** — `Pokedex.API` controllers call repositories (+ `ISaveChanges`) directly. Only introduce a service/orchestrator class when there's real multi-step or multi-aggregate coordination (e.g. the Battle Turn Orchestrator in `Battles.API`).
- **Prefer concrete classes over interfaces** when there is, and will only ever be, one implementation (e.g. `DamageCalculator`). Add an interface when there's a genuine substitutability need (e.g. `IRandomProvider` for deterministic tests, repository interfaces for provider portability).
- **Persistence is EF Core Code-First**, isolated entirely in `Pokemons.Infra`. `Pokemons.Domain` entities are plain C# POCOs with no EF attributes/types. Provider selection (SQLite today, SQL Server later) is a single DI seam (`AddPokemonsPersistence(...)`).
- Before proposing new abstractions or layers, check `docs/architecture.md`'s "Key Design Decisions" and "SOLID Principles Applied" sections — many of these tradeoffs have already been discussed and decided.
