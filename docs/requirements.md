# Pokemon Exercise — Requirements (Authoritative / AI Source of Truth)

> This file is the single authoritative specification for implementation. It is optimized for machine parsing (tables, key-value lists, explicit ids). For a human-friendly narrative overview, see `README.md`.

## Meta

| Key | Value |
|---|---|
| Target framework | .NET 10 |
| Persistence | SQLite |
| Design goal | Decoupled, modular, testable. No overengineering for future distributed systems. |
| Exercise order (must keep) | 1. Damage calculation, 2. Pokemon API, 3. Battle state API |
| Dependency: Part 1 requires | Part 2 (Pokemon API must exist first, since damage calc needs the domain model) |
| Dependency: Part 3 requires | Parts 1 and 2 |

---

## 1. Damage Calculation (Part 1)

**Input:** attacking Pokemon, selected move, opponent Pokemon.
**Output:** damage value (number), consumable by the battle system.

Formula:
```
Damage = { [ (2 * Level / 5 + 2) * Attack * MovePower / Defense ] / 50 } * Effectiveness * (Random / 100)
```

| Symbol | Source |
|---|---|
| Level | Acting Pokemon |
| Attack | Acting Pokemon base attack |
| MovePower | Selected move's Power |
| Defense | Opponent Pokemon base defense |
| Effectiveness | Type chart lookup (attacking move type vs defending Pokemon type) |
| Random | Random integer 85–100 inclusive (must be injectable/deterministic for tests) |

### Effectiveness multipliers

| Symbol | Meaning | Multiplier |
|---|---|---|
| `-` | Normal | 1 |
| `x2` | Weakness | 2 |
| `1/2` | Resistance | 0.5 |
| `x0` | Immunity | 0 |

> Full type-vs-type matrix below (transcribed from TypeEffectivenessMatrix.PNG). Row = attacking move type, Column = defending Pokemon type. The full matrix is seeded regardless of scope, but V1 seed data only uses 6 real Pokemon sourced from pokemondb.net (see §5 assumption 11) — tests only need to exercise the type pairs present among those 6, not the full matrix.

#### Type Effectiveness Matrix

| Attacker \ Defender | Acero | Agua | Bicho | Dragon | Electrico | Fantasma | Fuego | Hada | Hielo | Lucha | Normal | Planta | Psiquico | Roca | Siniestro | Tierra | Veneno | Volador |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Acero | 1/2 | 1/2 | - | - | 1/2 | - | 1/2 | x2 | x2 | - | - | - | - | x2 | - | - | - | - |
| Agua | - | 1/2 | - | 1/2 | - | - | x2 | - | - | - | - | 1/2 | - | x2 | - | x2 | - | - |
| Bicho | 1/2 | - | - | - | - | 1/2 | 1/2 | 1/2 | - | 1/2 | - | x2 | x2 | - | x2 | - | 1/2 | 1/2 |
| Dragon | 1/2 | - | - | x2 | - | - | - | x0 | - | - | - | - | - | - | - | - | - | - |
| Electrico | - | x2 | - | 1/2 | 1/2 | - | - | - | - | - | - | 1/2 | - | - | - | x0 | - | x2 |
| Fantasma | - | - | - | - | - | x2 | - | - | - | - | x0 | - | x2 | - | 1/2 | - | - | - |
| Fuego | x2 | 1/2 | x2 | 1/2 | - | - | 1/2 | - | x2 | - | - | x2 | - | 1/2 | - | - | - | - |
| Hada | 1/2 | - | - | x2 | - | - | 1/2 | - | - | x2 | - | - | - | - | x2 | - | 1/2 | - |
| Hielo | 1/2 | 1/2 | - | x2 | - | - | 1/2 | - | 1/2 | - | - | x2 | - | - | - | x2 | - | x2 |
| Lucha | x2 | - | 1/2 | - | - | x0 | - | 1/2 | x2 | - | x2 | - | 1/2 | x2 | x2 | - | 1/2 | 1/2 |
| Normal | 1/2 | - | - | - | - | x0 | - | - | - | - | - | - | - | 1/2 | - | - | - | - |
| Planta | 1/2 | x2 | 1/2 | 1/2 | - | - | 1/2 | - | - | - | - | 1/2 | - | x2 | - | x2 | 1/2 | 1/2 |
| Psiquico | 1/2 | - | - | - | - | - | - | - | - | x2 | - | - | 1/2 | - | x0 | - | x2 | - |
| Roca | 1/2 | - | x2 | - | - | - | x2 | - | x2 | 1/2 | - | - | - | - | - | 1/2 | - | x2 |
| Siniestro | - | - | - | - | - | x2 | - | 1/2 | - | 1/2 | - | - | x2 | - | 1/2 | - | - | - |
| Tierra | x2 | - | 1/2 | - | x2 | - | x2 | - | - | - | - | 1/2 | - | x2 | - | - | x2 | x0 |
| Veneno | x0 | - | - | - | - | 1/2 | - | x2 | - | - | - | x2 | - | 1/2 | - | 1/2 | 1/2 | - |
| Volador | 1/2 | - | x2 | - | 1/2 | - | - | - | - | x2 | - | x2 | - | 1/2 | - | - | - | - |

### Acceptance criteria — Part 1
- Damage calculation method exists and follows the formula exactly.
- Effectiveness multiplier applied correctly per type chart.
- Random factor constrained to 85–100 and overridable in tests.
- Output usable directly by the battle system.

---

## 2. Pokemon API (Part 2)

### Required capabilities
- CRUD: base Pokemon
- CRUD: moves
- CRUD: My Pokemon (owned Pokemon instances)
- CRUD: up to 4 moves assigned to an owned Pokemon
- Query: moves of a given My Pokemon (its assigned moves)
- Query: possible moves for a given Pokemon (candidate moves by type match)
- Query: **Base Pokemon** that share a given move (PDF: "Consulta para obtener una lista con los Pokémons que comparten un mismo movimiento" — V1 interprets this as moves shared by BasePokemons in the catalog, for team-building discovery; future versions may also add a similar query on owned MyPokemons for in-battle move management)

### My Pokemon (V1 ownership rules)
- References a base Pokemon (`BasePokemonId`).
- Up to 4 moves.
- Retrievable as its own resource.
- `OwnerId` is a `string`.
- No trainers/users/auth in V1.

### Acceptance criteria — Part 2
- Base Pokemon CRUD exists.
- Moves CRUD exists.
- My Pokemon CRUD exists; references a base Pokemon; up to 4 moves.
- Queries exist for: a My Pokemon's assigned moves, possible moves for a Pokemon, BasePokemons sharing a move (V1 searches the catalog; V2 may extend to owned MyPokemons).

---

## 3. Battle State API (Part 3)

### Lifecycle
Two states only: `Started`, `Finished`. No "not started" state — battle starts immediately on creation with both Pokemon and required fight data; each turn uses moves already assigned to each `MyPokemon`.

### Rules
| Rule | Behavior |
|---|---|
| Turn order | Enforced |
| Out-of-turn action | Rejected |
| Actions per turn | One move/action |
| Damage | Applied via Part 1 damage calculation |
| Persistence | Battle state + full history (executed moves, turn data) persisted |
| End condition | Battle marked `Finished` when a Pokemon reaches 0 HP |
| Post-finish actions | Rejected (read-only except history/state queries) |
| Post-finish queries | History and final state remain queryable |

### Explicitly out of scope for battle logic (V1)
Switching Pokemon, multiplayer, items, abilities, status effects, accuracy, critical hits, weather effects.

### Acceptance criteria — Part 3
- Battle created already initialized with two Pokemon, starts in `Started`.
- Battle can transition to `Finished`.
- Turn order enforced; out-of-turn execution rejected.
- Battle actions/history persisted.
- Battle ends when a Pokemon reaches 0 HP.
- Finished battles reject further actions but allow history/state queries.

---

## 4. Domain Model

### BasePokemon
| Field | Type |
|---|---|
| Id | int/guid |
| Name | string |
| Type | string (single type only, V1) |
| Level | int |
| TotalHP | int |
| BaseAttack | int |
| BaseDefense | int |
| BaseSpecialAttack | int |
| BaseSpecialDefense | int |
| BaseSpeed | int |

> **Implementation note (Phase 1):** `BaseDefense` was added during implementation — the §1 damage formula requires "Opponent Pokemon base defense" as an input, which was missing from this table. Same addition applies to `MyPokemon` below.
> **Implementation note (correction):** `BaseSpecialAttack`, `BaseSpecialDefense`, and `BaseSpeed` were also missing from this table despite being listed in the exercise PDF's Pokemon field list (Puntos Ataque/Defensa Especial base, Puntos Velocidad base). They are now included on both `BasePokemon` and `MyPokemon` for spec completeness, even though the §1 damage formula only consumes `BaseAttack`/`BaseDefense` (V1 has no special-move-category distinction and no turn-order-by-speed rule — see §3 out-of-scope list).

### MyPokemon (owned instance)
| Field | Type |
|---|---|
| Id | int/guid |
| OwnerId | string |
| BasePokemonId | FK -> BasePokemon |
| Name | string |
| Type | string |
| Level | int |
| CurrentHP | int |
| TotalHP | int |
| BaseAttack | int |
| BaseDefense | int |
| BaseSpecialAttack | int |
| BaseSpecialDefense | int |
| BaseSpeed | int |
| Moves | up to 4, FK -> Move |

### Move
| Field | Type |
|---|---|
| Id | int/guid |
| Name | string |
| Type | string |
| Power | int |

### MyPokemonMove (join: a move assigned to an owned Pokemon)
| Field | Type |
|---|---|
| Id | int/guid |
| MyPokemonId | FK -> MyPokemon |
| MoveId | FK -> Move |
| Name | string |
| Type | string |
| Power | int |

> **Implementation note:** `Name`/`Type`/`Power` are snapshotted from `Move` at assignment time, for the same reason `MyPokemon` snapshots its stats from `BasePokemon` (see "Deferred to V2 (Backlog)" in `architecture.md`) — editing the `Move` catalog later must not retroactively change the behavior of moves already assigned to an owned Pokemon, including ones mid-battle.

### Battle
| Field | Type |
|---|---|
| Id | int/guid |
| Status | enum: Started, Finished |
| PokemonOneId | FK -> MyPokemon |
| PokemonTwoId | FK -> MyPokemon |
| CurrentTurn | reference to acting Pokemon |
| WinnerPokemonId | FK -> MyPokemon (nullable until finished) |
| FinishedAt | datetime? |

### BattleAction
| Field | Type |
|---|---|
| Id | int/guid |
| BattleId | FK -> Battle |
| TurnNumber | int |
| ActingPokemonId | FK -> MyPokemon |
| UsedMoveId | FK -> Move |
| DamageDealt | number |
| CreatedAt | datetime |

### TypeEffectiveness
Type-vs-type effectiveness matrix (attacking type x defending type -> multiplier). Must be seeded/implemented as part of the domain. See §1 [Type Effectiveness Matrix](#type-effectiveness-matrix) for the full transcribed data used to seed this table.

---

## 5. Assumptions (V1)

1. No physical/special move distinction.
2. No physical/special defense distinction.
3. Pokemon are single-type only.
4. Battle phases are simple turn-based action updates.
5. Battle starts immediately when created.
6. Finished battles are read-only except for history/state queries.
7. SQLite is sufficient for persistence.
8. Random factor is controllable/injectable in tests for deterministic validation.
9. `OwnerId` is a string.
10. Trainers, users, and authentication are out of scope.
11. **Seed data scope (confirmed with recruiter)**: V1 seed data includes only 6 `BasePokemon`, sourced from https://pokemondb.net/pokedex/all — National Dex #0004 Charmander, #0007 Squirtle, #0035 Clefairy, #0019 Rattata, #0023 Ekans, #0025 Pikachu (not all 18 types/species; Caterpie #0010 was swapped for Clefairy since it has too few attacking moves, and a second Veneno Pokemon was avoided since Ekans already covers that type). Moves are each Pokemon's "Moves learnt by level up" attacking moves (per-Pokemon page, e.g. https://pokemondb.net/pokedex/charmander#dex-evolution); one high-level move per Pokemon is intentionally left unseeded for manual addition during the smoke test. The full type-effectiveness matrix is still seeded/persisted as-is (cheap, already transcribed), but only the type pairs actually represented among the 6 seeded Pokemon need to be exercised by tests/E2E — exhaustive coverage of all 18x18 type combinations is explicitly not required.

## 6. Out of Scope (V1)

Authentication, user management, trainers, external PokemonDB integration, multiplayer battles, advanced battle mechanics beyond this spec, UI/front-end, distributed-system architecture work (beyond keeping the design decoupled).

## 7. V2 Proposals (Future, not implemented now)

Trainers + authenticated ownership, real user accounts/authorization, battle pre-start/pending state, dual-type Pokemon, physical/special attack separation, physical/special defense separation, more advanced battle mechanics, switching Pokemon mid-battle, items/abilities/status conditions/critical hits/weather, richer battle analytics/replay, monolith -> distributed architecture migration if needed.

## 8. Implementation Notes

- Keep codebase modular and low-coupled; prefer simple boundaries over unnecessary abstraction.
- Avoid overengineering for future distributed-system needs.
- Build with testability in mind, especially damage and battle logic (inject randomness, isolate rules engine).