# Smoke Test Plan (V1)

> Companion to `docs/requirements.md` (acceptance criteria source) and `docs/plan.md` (roadmap). This is a manual/scripted walkthrough checklist proving the **whole flow** works across both APIs together, complementing (not replacing) `Pokemons.IntegrationTests`, which verify endpoints individually.

## Scope for V1

**Full E2E QA (the complete checklist below) is out of scope for V1** — only the **Smoke Test** section is required as part of the V1 Definition of Done. The full checklist is kept as a reference/backlog for a future hardening pass (V1.1+) if more time becomes available.

## Smoke Test (V1) — run this, not the full checklist

A single happy-path walkthrough proving the three parts work together, using the 6 seeded Pokemon (one per type):

- [ ] Create a `BasePokemon` (e.g., two different types to exercise effectiveness later) — or confirm seeded data already covers this (6 Pokemon, one per type, per V1 scope).
- [ ] Retrieve the created `BasePokemon` by id.
- [ ] Update a `BasePokemon` and confirm the change persists.
- [ ] Create at least 2 `Move`s (different types/powers).
- [ ] Retrieve a `Move` by id.
- [ ] Create/confirm a `MyPokemon` (from seeded `BasePokemon`) with 1–2 moves assigned.
- [ ] Create a second `MyPokemon` (opponent) the same way.
- [ ] Query: moves of a given My Pokemon — returns the assigned moves.
- [ ] Query: possible moves for a given Pokemon — returns expected candidate moves.
- [ ] Query: **Base Pokemon** sharing a given move — returns all `BasePokemon` with that move available/assigned (PDF: "Consulta para obtener una lista con los Pokémons que comparten un mismo movimiento" — targets `BasePokemon`, not `MyPokemon`).
- [ ] Create a battle with both Pokemon + selected moves; confirm `Status = Started` immediately.
- [ ] Execute one action for the Pokemon whose turn it is; confirm damage applied and HP updated.
- [ ] Continue executing turns until one Pokemon's HP reaches 0.
- [ ] Confirm battle transitions to `Status = Finished` with `WinnerPokemonId` set.
- [ ] Query battle history after finish; confirm it's retrievable.
- [ ] No unhandled exceptions/500s during the walkthrough.

## When to execute

Run the Smoke Test once Phase 4 (`Battles.API`) is complete and both APIs (`Pokedex.API`, `Battles.API`) are runnable end-to-end — see "Definition of Done for V1" in `docs/plan.md`.

## How to execute

Either manually via Swagger UI for each API, or scripted via `.http` files / `Pokemons.IntegrationTests`. Record actual results (pass/fail + notes) inline or in a linked run log.

---

## Full Checklist (deferred / backlog — not required for V1)

> Kept for reference and as a candidate scope for a future hardening pass. Do not block V1 completion on this section.
> Human validation pending

## Part 1 — Damage Calculation (verified via unit tests, sanity-checked here through battle flow)

> Scope note (confirmed with recruiter): only 6 `BasePokemon` (one per selected type) are seeded for V1. Exhaustive coverage of all 18x18 type-effectiveness combinations is **not required** — spot-check representative cases only (one weakness, one resistance/neutral) using the type pairs actually present among the 6 seeded Pokemon.

- [ ] Damage produced during a battle action matches the documented formula for known inputs (spot-check one turn's damage against manual calculation).
- [ ] Effectiveness multiplier is visibly applied for at least one weakness matchup among the seeded Pokemon (produces roughly double the damage of a neutral matchup, all else equal).
- [ ] Random factor stays within 85–100% range across multiple executed turns.

## Part 2 — Pokemon API (`Pokedex.API`)

- [ ] Create a `MyPokemon` referencing a valid `BasePokemonId`.
- [ ] Assign up to 4 moves to a `MyPokemon`; confirm a 5th assignment is rejected.
- [ ] Attempt to create a `MyPokemon` with an invalid `BasePokemonId`; confirm rejection.
- [ ] Create a second `MyPokemon` (opponent) with its own moves, for use in Part 3.

## Part 3 — Battle State API (`Battles.API`)

- [ ] Create a battle with two valid `MyPokemon` ids + selected moves; confirm response has `Status = Started` immediately (no pending/not-started state).
- [ ] Retrieve battle state right after creation; confirm both Pokemon, HP, and turn order are present.
- [ ] Execute an action for the Pokemon whose turn it is; confirm damage applied, HP updated, action recorded in history.
- [ ] Attempt an out-of-turn action (wrong Pokemon acting); confirm rejected.
- [ ] Continue executing valid turns until one Pokemon's HP reaches 0.
- [ ] Confirm battle transitions to `Status = Finished`, `WinnerPokemonId` set, `FinishedAt` populated.
- [ ] Attempt to execute another action after finish; confirm rejected.
- [ ] Query battle state after finish; confirm final state (HP, winner, status) is still retrievable.
- [ ] Query battle history after finish; confirm full turn-by-turn history is still retrievable.

## Cross-cutting checks

- [ ] Both APIs run independently (separate ports/Swagger docs) and share the same SQLite database consistently (e.g., a `MyPokemon` created via `Pokedex.API` is visible/usable in `Battles.API`).
- [ ] No unhandled exceptions/500s during the full happy-path walkthrough above.
- [ ] Basic invalid-input cases (missing fields, non-existent ids) return sensible 4xx responses, not 500s.

---

## Result log

| Date | Run by | Outcome | Notes |
|---|---|---|---|
| | | | |
