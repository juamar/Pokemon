# Implementation Plan

> Roadmap derived from `docs/requirements.md` (spec) and `docs/architecture.md` (structure). Order must follow the exercise's required sequence: Part 1 → Part 2 → Part 3, but Part 2's domain model is scaffolded first since Part 1 depends on it (see requirements Meta table).

## Phase 0 — Scaffolding
- [ ] Create `Pokemons.Domain`, `Pokemons.Infra` class library projects; add to solution.
- [ ] Create `Battles.API` ASP.NET Core Web API project (separate from `Pokedex.API`), own Swagger doc.
- [ ] Create `Pokemons.Tests` xUnit project (unit tests only).
- [ ] Create `Pokemons.IntegrationTests` xUnit project (integration tests only).
- [ ] Wire project references per `architecture.md`.
- [ ] Add EF Core + SQLite packages to `Pokemons.Infra`.

## Phase 1 — Domain Model & Persistence (supports Part 2, prerequisite for Part 1)
- [ ] Define entities in `Pokemons.Domain`: `BasePokemon`, `MyPokemon`, `Move`, `Battle`, `BattleAction`.
- [ ] Define `TypeEffectiveness` lookup and seed data from the matrix in `requirements.md` §1.
- [ ] Create `PokemonsDbContext` in `Pokemons.Infra` with EF configurations + SQLite.
- [ ] Add initial migration + seed data (base Pokemon, moves, type chart).

## Phase 2 — Damage Calculation (Part 1)
- [ ] Define `IRandomProvider` in `Pokemons.Domain`; real impl (System.Random-based) + fixed/fake impl for tests.
- [ ] Implement damage formula service per `requirements.md` §1.
- [ ] Unit tests (`Pokemons.Tests`): formula correctness, effectiveness multiplier application, random bounds (85–100), deterministic override in tests.

## Phase 3 — Pokemon API (Part 2) — `Pokedex.API`
- [ ] CRUD endpoints: Base Pokemon, Moves, My Pokemon (+ up to 4 moves assignment).
- [ ] Query endpoints: moves of a Pokemon, possible moves for a Pokemon, Pokemon sharing a move.
- [ ] Validation: My Pokemon references valid base Pokemon; max 4 moves.
- [ ] Integration tests (`Pokemons.IntegrationTests`) for all endpoints.

## Phase 4 — Battle State API (Part 3) — `Battles.API`
- [ ] Battle creation: initializes two My Pokemon + selected moves, status `Started`.
- [ ] Turn/action execution endpoint: validate turn order, apply damage via Phase 2 service, persist `BattleAction`, update HP, check finish condition.
- [ ] Finish transition: set `Finished`, `WinnerPokemonId`, `FinishedAt`; reject further actions.
- [ ] Query endpoints: battle state, battle history.
- [ ] Integration tests (`Pokemons.IntegrationTests`): full battle flow, out-of-turn rejection, post-finish rejection, history queryable after finish.

## Phase 5 — Hardening / Polish (optional, only if time allows)
- [ ] Error handling / consistent API error responses.
- [ ] Basic request validation messages.
- [ ] README/architecture doc updates if structure changed during implementation.

## Notes

- The type effectiveness matrix is now transcribed in `requirements.md` §1. A few source cells had unexplained footnotes (see note above the matrix) — double-check those during Phase 1 seeding if odd behavior is observed.
