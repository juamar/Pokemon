# Implementation Plan

> Roadmap derived from `docs/requirements.md` (spec) and `docs/architecture.md` (structure). Order must follow the exercise's required sequence: Part 1 → Part 2 → Part 3, but Part 2's domain model is scaffolded first since Part 1 depends on it (see requirements Meta table).

## Phase 0 — Scaffolding
- [x] Create `Pokemons.Domain`, `Pokemons.Infra` class library projects; add to solution.
- [x] Create `Pokedex.API` ASP.NET Core Web API project, own Swagger doc.
- [x] Create `Battles.API` ASP.NET Core Web API project (separate from `Pokedex.API`), own Swagger doc.
- [x] Create `Pokemons.Tests` xUnit project (unit tests only).
- [x] Create `Pokemons.IntegrationTests` xUnit project (integration tests only).
- [x] Wire project references per `architecture.md`.
- [x] Add EF Core + SQLite packages to `Pokemons.Infra`.
- [x] **Remove template boilerplate** from each generated API project (e.g., `WeatherForecast.cs`, `WeatherForecastController.cs`, sample `.http` requests) — no leftover `dotnet new` sample code. This is part of the Acceptance Criteria for Phase 0, not optional cleanup.
- [x] Build the solution; confirm zero errors/warnings before moving to Phase 1 (no TDD here — this phase is pure project scaffolding with no domain logic yet to test).

## Phase 1 — Domain Model & Persistence (supports Part 2, prerequisite for Part 1)
- [x] Define entities in `Pokemons.Domain`: `BasePokemon`, `MyPokemon`, `Move`, `Battle`, `BattleAction` (plus `MyPokemonMove` join entity for the up-to-4-moves assignment).
- [x] Define `TypeEffectiveness` lookup and seed data from the matrix in `requirements.md` §1.
- [x] Create `PokemonsDbContext` in `Pokemons.Infra` with EF configurations + SQLite.
- [x] Add initial migration + seed data: 6 `BasePokemon` sourced from pokemondb.net (Charmander, Squirtle, Clefairy, Rattata, Ekans, Pikachu — National Dex #0004/#0007/#0035/#0019/#0023/#0025, per `requirements.md` §5 assumption 11), their level-up attacking `Move`s, and the full type chart.
- [x] **Build an expected-damage verification table** once seed data is finalized (6 Pokemon + moves): picked Charmander (Fuego) vs Bulbasaur (Planta) with Ember — a weakness matchup — computed expected damage per the §1 formula using the actual seeded Level/Attack/Defense/MovePower/type values, and recorded it in `docs/smoke-test-plan.md`'s Smoke Test section.

## Phase 2
- [x] Define `IRandomProvider` in `Pokemons.Domain`; real impl (System.Random-based) + fixed/fake impl for tests.
- [x] Implement damage formula service per `requirements.md` §1.
- [x] Unit tests (`Pokemons.Tests`): formula correctness, effectiveness multiplier application (representative cases only — one weakness/resistance/neutral/immunity, not all 18x18 combinations, per confirmed scope), random bounds (85–100), deterministic override in tests. Use the expected-damage verification table from Phase 1 as the source of truth for at least one test case, so it's checked against real seed data, not just hand-picked numbers.

## Phase 3 — Pokemon API (Part 2) — `Pokedex.API`
- [ ] CRUD endpoints: Base Pokemon, Moves, My Pokemon (+ up to 4 moves assignment).
- [ ] Query endpoints: moves of a My Pokemon, possible moves for a Pokemon, **Base Pokemon** sharing a move (PDF: "Consulta para obtener una lista con los Pokémons que comparten un mismo movimiento" — targets `BasePokemon`, not `MyPokemon`).
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

## Definition of Done for V1

Once all phases/units of work above are complete:

1. **Full solution build** — zero errors/warnings across all projects.
2. **Full test run** — unit (`Pokemons.Tests`) + integration (`Pokemons.IntegrationTests`) all green, not just the last touched area.
3. **Docs reconciliation (final sweep)** — per-unit-of-work reconciliation should already keep docs current (see `.github/copilot-instructions.md`); this is the safety-net pass to catch anything missed:
   - `docs/plan.md` — mark all phases done; remove/adjust stale notes.
   - `docs/architecture.md` — confirm it still matches actual code (interfaces, project names, DI wiring); update if implementation diverged from design.
   - `README.md` — add a "How to run" section (restore, migrations, run each API, run tests).
4. **⚠️ Update `README.md` "Tech Stack" section — explicitly requested by the recruiter.** Replace the `TBD` placeholder under "Libraries / Tools" with the actual packages/versions used (EF Core provider, xUnit, etc.) and confirm "Current / Planned" reflects what was actually built. Do not skip this — do it before leaving/finishing V1.
5. **Smoke test** — execute the "Smoke Test (V1)" section of [docs/smoke-test-plan.md](smoke-test-plan.md) and record the outcome in its result log. Full E2E QA (the rest of that document) is out of scope for V1 — deferred/backlog only. (Note: it's really an *enhanced* smoke test — it covers a full happy-path flow, not just a build-doesn't-fall-over check — see the naming note in that doc.)
6. **📍 Localize the core requested method/function from the exercise PDF for the recruiter.** The exercise's central deliverable (the damage calculation formula, `requirements.md` §1) must be easy for the recruiter to find without exploring the whole solution — add a direct file path reference (e.g., `Pokemons.Domain/DamageCalculator.cs`) in `README.md`'s Notes/Tech Stack section once implemented, pointing straight at the method that implements the formula.
7. **Final commit + push** — one last reviewed commit (e.g. "V1 complete: all phases implemented, tests passing, docs reconciled").
