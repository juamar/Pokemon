# Implementation Plan

> Roadmap derived from `docs/requirements.md` (spec) and `docs/architecture.md` (structure). Order must follow the exercise's required sequence: Part 1 → Part 2 → Part 3, but Part 2's domain model is scaffolded first since Part 1 depends on it (see requirements Meta table).

## Phase 0 — Scaffolding
- [ ] Create `Pokedex.Domain`, `Pokedex.Infrastructure` class library projects; add to solution.
- [ ] Create `Pokedex.Tests` xUnit project.
- [ ] Wire project references per `architecture.md`.
- [ ] Add EF Core + SQLite packages to `Pokedex.Infrastructure`.

## Phase 1 — Domain Model & Persistence (supports Part 2, prerequisite for Part 1)
- [ ] Define entities in `Pokedex.Domain`: `BasePokemon`, `MyPokemon`, `Move`, `Battle`, `BattleAction`.
- [ ] Define `TypeEffectiveness` lookup (need actual matrix data — currently a placeholder in requirements.md).
- [ ] Create `PokedexDbContext` in `Pokedex.Infrastructure` with EF configurations + SQLite.
- [ ] Add initial migration + seed data (base Pokemon, moves, type chart).

## Phase 2 — Damage Calculation (Part 1)
- [ ] Define `IRandomProvider` in Domain; real impl (System.Random-based) + fixed/fake impl for tests.
- [ ] Implement damage formula service per `requirements.md` §1.
- [ ] Unit tests: formula correctness, effectiveness multiplier application, random bounds (85–100), deterministic override in tests.

## Phase 3 — Pokemon API (Part 2)
- [ ] CRUD endpoints: Base Pokemon, Moves, My Pokemon (+ up to 4 moves assignment).
- [ ] Query endpoints: moves of a Pokemon, possible moves for a Pokemon, Pokemon sharing a move.
- [ ] Validation: My Pokemon references valid base Pokemon; max 4 moves.
- [ ] Integration tests for all endpoints.

## Phase 4 — Battle State API (Part 3)
- [ ] Battle creation: initializes two My Pokemon + selected moves, status `Started`.
- [ ] Turn/action execution endpoint: validate turn order, apply damage via Phase 2 service, persist `BattleAction`, update HP, check finish condition.
- [ ] Finish transition: set `Finished`, `WinnerPokemonId`, `FinishedAt`; reject further actions.
- [ ] Query endpoints: battle state, battle history.
- [ ] Integration tests: full battle flow, out-of-turn rejection, post-finish rejection, history queryable after finish.

## Phase 5 — Hardening / Polish (optional, only if time allows)
- [ ] Error handling / consistent API error responses.
- [ ] Basic request validation messages.
- [ ] README/architecture doc updates if structure changed during implementation.

## Blocking Item

- **Type effectiveness matrix data** (from `TypeEffectivenessMatrix.PNG`) is not yet transcribed. Needed before Phase 1 seed data / Phase 2 tests can be finalized. Can stub with a placeholder chart in the meantime if needed to keep moving.
