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
- [ ] Add initial migration + seed data: 6 `BasePokemon` (one per selected type, per `requirements.md` §5 assumption 11), a handful of `Move`s, and the full type chart.
- [ ] **Build an expected-damage verification table** once seed data is finalized (6 Pokemon + moves): for 2–3 representative matchups (one weakness, one neutral, using the actual seeded Level/Attack/Defense/MovePower/type values), manually compute expected damage per the §1 formula and record it (e.g., in `docs/smoke-test-plan.md` or a small table in this file). This table is the source of truth used by both Phase 2 unit tests and the Phase 4 battle smoke test to confirm the damage calculation behaves as expected against real seed data, not just arbitrary numbers.

## Phase 2 — Damage Calculation (Part 1)
- [ ] Define `IRandomProvider` in `Pokemons.Domain`; real impl (System.Random-based) + fixed/fake impl for tests.
- [ ] Implement damage formula service per `requirements.md` §1.
- [ ] Unit tests (`Pokemons.Tests`): formula correctness, effectiveness multiplier application (representative cases only — one weakness/resistance/neutral/immunity, not all 18x18 combinations, per confirmed scope), random bounds (85–100), deterministic override in tests. Use the expected-damage verification table from Phase 1 as the source of truth for at least one test case, so it's checked against real seed data, not just hand-picked numbers.

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
3. **Full coverage report** — run `./scripts/run-coverage.ps1 -OpenReport`; sanity-check `Pokemons.Domain` coverage as a whole.
4. **Docs reconciliation (final sweep)** — per-unit-of-work reconciliation should already keep docs current (see `.github/copilot-instructions.md`); this is the safety-net pass to catch anything missed:
   - `docs/plan.md` — mark all phases done; remove/adjust stale notes.
   - `docs/architecture.md` — confirm it still matches actual code (interfaces, project names, DI wiring); update if implementation diverged from design.
   - `README.md` — add a "How to run" section (restore, migrations, run each API, run tests, run coverage).
5. **⚠️ Update `README.md` "Tech Stack" section — explicitly requested by the recruiter.** Replace the `TBD` placeholder under "Libraries / Tools" with the actual packages/versions used (EF Core provider, coverlet, xUnit, etc.) and confirm "Current / Planned" reflects what was actually built. Do not skip this — do it before leaving/finishing V1.
6. **Smoke test** — execute the "Smoke Test (V1)" section of [docs/smoke-test-plan.md](smoke-test-plan.md) and record the outcome in its result log. Full E2E QA (the rest of that document) is out of scope for V1 — deferred/backlog only.
7. **📍 Localize the core requested method/function from the exercise PDF for the recruiter.** The exercise's central deliverable (the damage calculation formula, `requirements.md` §1) must be easy for the recruiter to find without exploring the whole solution — add a direct file path reference (e.g., `Pokemons.Domain/DamageCalculator.cs`) in `README.md`'s Notes/Tech Stack section once implemented, pointing straight at the method that implements the formula.
8. **Final commit + push** — one last reviewed commit (e.g. "V1 complete: all phases implemented, tests passing, docs reconciled").
