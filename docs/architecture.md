# Architecture

> Companion to `docs/requirements.md` (authoritative spec). This file describes **how** the system is structured, not **what** it must do.

## Guiding Principle

Keep it simple. No Clean Architecture ceremony, no CQRS/MediatR, no premature interfaces "just in case." Split only where it gives real decoupling for this exercise (testability of damage/battle logic, swappable persistence).

## Solution Structure

```
Pokemon.slnx
├── Pokedex.API/                 # Part 2: Pokemon resources (BasePokemon, Moves, MyPokemon), own Swagger doc
├── Battles.API/                 # Part 3: Battle state (start/execute/finish/history), own Swagger doc
├── Pokemons.Domain/             # Part 1 + shared: entities, damage calculation, battle rules engine, type effectiveness. No EF/DB references.
├── Pokemons.Infra/              # Part 2 support: EF Core DbContext, SQLite provider, repositories, seed data. Shared by both API projects.
├── Pokemons.Tests/              # Part 1 verification: unit tests only (damage calc, battle rules) — no DB, no HTTP, fast/isolated
└── Pokemons.IntegrationTests/   # Part 2 + 3 verification: integration tests for both APIs — real SQLite + WebApplicationFactory per API
```

> All projects sit flat at the solution root (no `src/`/`tests/` subfolders) — matches the existing `Pokedex.API` layout and keeps the solution structure simple for this exercise's scope.


> Both API projects point at the same SQLite database via `Pokemons.Infra` — no HTTP calls between them are needed (e.g., Battle API reads `MyPokemon`/`Move` rows directly through the shared `PokemonsDbContext`). This keeps them decoupled at the API/deployment level without the overhead of service-to-service calls for this exercise's scope.

## Exercise Scope Mapping

> Purpose: make it immediately obvious which project satisfies which part of the exercise (see `requirements.md` §2 for the required part order: 1. Damage calculation, 2. Pokemon API, 3. Battle state API).

| Project | Exercise Part(s) | What it proves |
|---|---|---|
| `Pokemons.Domain` | **Part 1** (core) + shared model for Parts 2/3 | Damage formula, type effectiveness, battle rules engine — all pure, unit-testable logic |
| `Pokemons.Tests` | **Part 1** (verification) | Unit tests proving the damage formula and battle rules are correct in isolation |
| `Pokemons.Infra` | **Part 2** (support) | Persistence for the domain model (EF Core + SQLite) |
| `Pokedex.API` | **Part 2** | The Pokemon API itself: CRUD for base Pokemon, moves, My Pokemon, plus the required query endpoints |
| `Battles.API` | **Part 3** | The Battle State API: battle lifecycle, turn enforcement, action execution, history |
| `Pokemons.IntegrationTests` | **Part 2 + Part 3** (verification) | End-to-end proof both APIs satisfy their acceptance criteria |

### Why this split
| Project | Responsibility | Depends on |
|---|---|---|
| `Pokemons.Domain` | Entities, `DamageCalculator` (concrete class — no interface, only one implementation ever exists), `IRandomProvider` (for deterministic tests), battle state machine, type chart lookup, **repository interfaces** (`IBasePokemonRepository`, `IMoveRepository`, `IMyPokemonRepository`, `IBattleRepository`, `ISaveChanges`, etc.) | Nothing (pure C#) |
| `Pokemons.Infra` | EF Core `PokemonsDbContext`, entity configurations, repository **implementations** of the `Pokemons.Domain` interfaces, seed data, provider-specific setup (SQLite today, SQL Server swappable) isolated behind a single `AddPokemonsPersistence(...)` DI extension | `Pokemons.Domain` |
| `Pokedex.API` | Pokemon resources: `BasePokemonController`, `MovesController`, `MyPokemonController` — DTOs, validation, DI composition, own Swagger doc | `Pokemons.Domain`, `Pokemons.Infra` |
| `Battles.API` | Battle state: `BattlesController` — create/execute/finish/history endpoints, own Swagger doc | `Pokemons.Domain`, `Pokemons.Infra` |
| `Pokemons.Tests` | Unit tests (xUnit) — damage calc, battle rules engine, type chart lookup. No EF/SQLite/HTTP. | `Pokemons.Domain` only |
| `Pokemons.IntegrationTests` | Integration tests (xUnit) — spins up each API (`WebApplicationFactory`) against a real/temp SQLite database; exercises CRUD endpoints (via `Pokedex.API`) and battle endpoints (via `Battles.API`) end-to-end | `Pokedex.API`, `Battles.API`, `Pokemons.Infra` |

This gives just enough decoupling to unit-test damage/battle logic without a database, per requirements' testability goal — without introducing extra layers (Application/CQRS) the exercise doesn't need.

## Key Design Decisions

- **Randomness is injected** (`IRandomProvider` in `Pokemons.Domain`, real implementation wired in each API, fake/fixed implementation in tests) to satisfy the deterministic-random-in-tests assumption.
- **Damage calculation is a concrete class, not an interface** in `Pokemons.Domain`: `DamageCalculator.Calculate(attacker, move, defender) -> int`, a pure function with no side effects, no DB access. There is only ever one implementation (the formula from `requirements.md` §1), so an interface would add indirection with no real substitutability benefit — `Pokemons.Tests` instantiates the concrete class directly. This is the Part 1 deliverable, physically isolated so it can be reviewed/tested independently of the APIs.
- **Battle rules engine** lives in `Pokemons.Domain` as well: turn validation, HP application, finish-state transition. `Battles.API` only orchestrates (load battle → call domain method → persist → return DTO).
- **EF Core + SQLite** in `Pokemons.Infra` only; `Pokemons.Domain` and `Pokemons.Tests` never reference EF directly (entities are POCOs).
- **Type effectiveness matrix** stored as seed data in `Pokemons.Infra` (table or JSON seed), read through a small Domain-facing lookup service/interface.
- **No auth/trainers** — `OwnerId` is a plain string field, no identity integration.
- **Dependency Inversion for persistence** — `Pokemons.Domain` defines repository interfaces only (`IBasePokemonRepository`, `IMoveRepository`, `IMyPokemonRepository`, `IBattleRepository`, `ITypeEffectivenessLookup`, `ISaveChanges`; no EF, no SQL). `Pokemons.Infra` provides the only **implementation**, using EF Core. Both API projects (and tests) depend on the interfaces from `Pokemons.Domain`, never on `Pokemons.Infra`'s EF Core types directly outside of DI registration in `Program.cs`. This makes swapping SQLite for SQL Server (or any other EF Core provider) a change confined to `Pokemons.Infra` + one line of DI configuration — see "Persistence & Provider Portability" below.
- **No service layer for plain CRUD** — `Pokedex.API` controllers (`BasePokemonController`, `MovesController`, `MyPokemonController`, Part 2) call the relevant repository directly (e.g., `IMyPokemonRepository` + `ISaveChanges`), with no intermediate service class. There is no orchestration, no multi-aggregate coordination, and no business rule beyond simple validation (e.g., max 4 moves) for these endpoints — wrapping them in a service would just forward calls 1:1 t
- **Two separate API projects** — `Pokedex.API` (Pokemon resources) and `Battles.API` (battle state) are independently runnable/deployable ASP.NET Core apps with their own Swagger docs, reflecting that Part 2 (CRUD-style resource API) and Part 3 (stateful battle/action API) are conceptually different API shapes — and making the Part 2 vs. Part 3 boundary visible at the project level, not just in code. Both share `Pokemons.Domain` + `Pokemons.Infra` and the same SQLite database — no duplication of entities/DbContext, no inter-service HTTP calls.

## Persistence & Provider Portability (DIP)

Goal: switching from SQLite to SQL Server (or another EF Core provider) later should require touching only `Pokemons.Infra`, not `Pokemons.Domain`, the APIs, or the tests.

- **Interfaces live in `Pokemons.Domain`**: `IBasePokemonRepository`, `IMoveRepository`, `IMyPokemonRepository`, `IBattleRepository`, `ITypeEffectivenessLookup`, and a minimal `ISaveChanges` (`Task<int> SaveChangesAsync(CancellationToken ct = default)`). These are plain C# interfaces operating on domain entities — no `DbContext`, no `DbSet<T>`, no EF attributes.
- **`Battle` and `BattleAction` share one repository (`IBattleRepository`), not two** — a `BattleAction` never exists independently of its parent `Battle` (it's the turn-by-turn history of that battle), so they are one aggregate, not two. `IBattleRepository` exposes both battle-level operations (`GetByIdAsync` loading the battle with its actions, `UpdateStatusAsync`) and action-level operations (`AddActionAsync`) on the same interface. A separate `IBattleActionRepository` would only add an extra interface for something that is always read/written together with its `Battle`.
- **`ISaveChanges` is justified by one specific, real problem: cross-aggregate atomicity in a battle turn** — executing a move touches two separate aggregates in one logical operation: `MyPokemon` (HP update, via `IMyPokemonRepository`) and `Battle`/`BattleAction` (new history row + status/winner, via `IBattleRepository`). Both must commit together or not at all. Rather than a generic "repositories always need a Unit of Work" assumption, this is the one concrete scenario in this exercise that requires it. Repository methods only **stage** changes (`Add`/`Update`/`Remove` on tracked entities); the **Battle Turn Orchestrator** (see below) composes the two repositories + `DamageCalculator` + `IRandomProvider`, then calls `ISaveChanges.SaveChangesAsync()` exactly once at the end. For simple single-entity CRUD in `Pokedex.API` (create/update one `BasePokemon`, `Move`, or `MyPokemon`), the same rule applies for consistency — the controller calls the repository method, then `ISaveChanges.SaveChangesAsync()` once, with no service in between (see "No service layer for plain CRUD" above). `ISaveChanges` is a direct pass-through to EF Core's own `PokemonsDbContext.SaveChangesAsync()` — no `Begin`/`Commit`/`Rollback`, no custom transaction ceremony, just the smallest seam needed to keep `Pokemons.Domain`/APIs free of EF Core references.
- **Battle Turn Orchestrator — purpose**: a plain class in `Pokemons.Domain` (not a repository, not exposed as an interface — one implementation, same reasoning as `DamageCalculator`) whose single job is to coordinate the multi-aggregate work of executing one battle turn: (1) validate it's the acting Pokemon's turn and the battle isn't already finished, (2) call `DamageCalculator` + `ITypeEffectivenessLookup` + `IRandomProvider` to compute damage, (3) apply the HP change via `IMyPokemonRepository`, (4) record the `BattleAction` and update `Battle` status/winner via `IBattleRepository`, (5) call `ISaveChanges.SaveChangesAsync()` once so all of the above commits atomically. `Battles.API`'s controller stays thin: it only maps HTTP ↔ orchestrator call ↔ DTO. This is the one workflow in the exercise complex/stateful enough to justify a dedicated coordinating class, unlike the Part 2 CRUD endpoints.
- **Implementations live in `Pokemons.Infra`**: EF Core-backed repository classes + `PokemonsDbContext`. The `DbContext` is intentionally `internal` to `Pokemons.Infra`, so API projects cannot inject or reference it directly; they can only consume persistence through `Pokemons.Domain` repository abstractions + `ISaveChanges`.
- **Provider selection is a single seam**: a `AddPokemonsPersistence(IServiceCollection, IConfiguration)` extension method in `Pokemons.Infra` registers `PokemonsDbContext` with whichever provider the connection string/config indicates (e.g., `options.UseSqlite(...)` today, `options.UseSqlServer(...)` later) and registers the repository implementations against their `Pokemons.Domain` interfaces. Each API's `Program.cs` calls this one method — it has zero knowledge of SQLite vs. SQL Server.
- **No provider-specific types/attributes leak into `Pokemons.Domain`** (e.g., no `[Column]`/SQLite-only types on entities); EF configuration (via `IEntityTypeConfiguration<T>`) lives entirely in `Pokemons.Infra`.
- **Fluent API only, no Data Annotations on `Pokemons.Domain` entities** — even basic constraints like required/max-length (e.g. `builder.Property(p => p.Type).IsRequired().HasMaxLength(50)` in `MyPokemonConfiguration`) are set via `IEntityTypeConfiguration<T>` in `Pokemons.Infra`, not `[Required]`/`[MaxLength]` attributes on the entity. This keeps `Pokemons.Domain` entities as persistence-agnostic POCOs with zero reference to EF Core or `System.ComponentModel.DataAnnotations`, keeps all mapping decisions (including trivial ones) centralized in one place alongside FK/seed/relationship config that has no attribute equivalent anyway, and avoids `Pokemons.Tests` (which fakes domain interfaces directly, no DB) ever depending on persistence metadata.
- **Tests reflect the boundary**: `Pokemons.Tests` (unit) never touches `Pokemons.Infra` or a real database — it fakes the `Pokemons.Domain` interfaces directly. `Pokemons.IntegrationTests` exercises the real `Pokemons.Infra` implementation (against SQLite for now), but does not resolve `PokemonsDbContext` directly; test bootstrap uses Infra's `ApplyPokemonsMigrationsAsync(IServiceProvider)` helper so `DbContext` encapsulation remains intact.
- **EF Core Code-First**: `Pokemons.Domain` entities are plain C# classes; `Pokemons.Infra` maps them via `IEntityTypeConfiguration<T>` (Fluent API), and the schema is generated from code through EF Core migrations (`dotnet ef migrations add ...`) — no database-first scaffolding, no hand-written SQL schema. This is the more idiomatic EF workflow and keeps the DB schema versioned alongside the code, for either SQLite or SQL Server.
- **Type effectiveness data is DB-backed, not hard-coded**: a `TypeEffectiveness` entity/table (`AttackingType`, `DefendingType`, `Multiplier`) is defined Code-First in `Pokemons.Infra` and seeded via EF Core's `HasData` (migration seed) from the matrix in `requirements.md` §1 — consistent with how `BasePokemon`/`Move` catalog data is seeded, and portable across providers via the same migration mechanism. The `ITypeEffectivenessLookup` implementation reads this table and caches it in memory on first use (the dataset is small — 18×18 — and effectively static), avoiding a DB round-trip per damage calculation without hard-coding the values in `Pokemons.Infra` C# code.
- **`MyPokemon` snapshots its stats from `BasePokemon` at creation time** (`Type`, `TotalHP`, `BaseAttack`, `BaseDefense` are copied fields, not computed/joined live from `BasePokemon`) rather than `MyPokemon` inheriting from `BasePokemon` or always reading its stats through the `BasePokemonId` reference. This is a deliberate V1 tradeoff, not an oversight: if `MyPokemon` always read stats live from `BasePokemon`, editing a catalog entry (e.g. correcting Charmander's `BaseDefense` via `Pokedex.API`) would retroactively change every already-owned `MyPokemon`'s stats — including ones mid-battle — which isn't how a Pokémon-like game should behave (an owned/caught Pokémon's stats shouldn't shift because the species catalog was edited later). The cost is duplicated data that can drift from `BasePokemon`; see "Deferred to V2 (Backlog)" below for the V2 alternative under consideration.
- **`MyPokemonMove` applies the same snapshot pattern to moves**: `Name`/`Type`/`Power` are copied from `Move` onto `MyPokemonMove` when a move is assigned to an owned Pokemon, rather than always being read live through `MoveId`. Same rationale as the `MyPokemon`/`BasePokemon` snapshot above — editing a `Move` in the catalog (e.g. rebalancing Ember's `Power`) must not retroactively change the moves an owned Pokémon already has assigned, including ones mid-battle. Same V2 backlog note applies (drift vs. explicit resync).

This is the one place in the solution where an abstraction is introduced deliberately ahead of need (SQL Server isn't required for V1) — justified because provider portability was explicitly requested, not as general-purpose future-proofing.

## SOLID Principles Applied

> Explicit mapping so the design intent is visible without having to infer it from code.

| Principle | Where it shows up |
|---|---|
| **S** — Single Responsibility | Each controller owns exactly one resource (`BasePokemonController`, `MovesController`, `MyPokemonController`, `BattlesController`). Each domain service has one job: `DamageCalculator` only calculates damage, `IRandomProvider` only supplies a random value, the battle rules engine only enforces turn/HP/finish rules — persistence, HTTP concerns, and game rules never mix in the same class. |
| **O** — Open/Closed | The type-effectiveness lookup (`ITypeEffectivenessLookup`) and repository interfaces let new implementations be added (e.g., a caching decorator, a different data source) without modifying `Pokemons.Domain` or the API controllers that consume them. |
| **L** — Liskov Substitution | Any implementation of a `Pokemons.Domain` interface must be fully substitutable for another: the EF Core repository implementation (`Pokemons.Infra`) and a test fake/in-memory repository must honor the same contract (e.g., `AddAsync` returns the entity with `Id` populated, `GetByIdAsync` returns `null` — not throws — when not found, no implementation-specific exceptions leak through). Same for `IRandomProvider`: a fixed-value test implementation must still return a value that satisfies "integer between 85 and 100," it just doesn't vary — callers never need to know or care which implementation they got. |
| **I** — Interface Segregation | Repository interfaces are split per aggregate (`IBasePokemonRepository`, `IMoveRepository`, `IMyPokemonRepository`, `IBattleRepository`) rather than one generic `IRepository<T>`
| **D** — Dependency Inversion | Covered in detail above: `Pokemons.Domain` defines persistence/randomness abstractions, `Pokemons.Infra` (and test fakes) implement them, both API projects depend only on the abstractions. |

> **Note on avoiding SOLID-for-its-own-sake**: not everything gets an interface. `DamageCalculator` is a concrete class because there is only ever one implementation and no real substitutability need — adding `IDamageCalculator` would be indirection without benefit. Interfaces in this solution exist only where there's a genuine reason: real substitutable implementations (`IRandomProvider`), a required DIP boundary (repositories, `ISaveChanges`), or test isolation from external systems (`ITypeEffectivenessLookup`).

## API Surface (high level)

### `Pokedex.API` (Pokemon resources — Part 2)
- `/basepokemon` — CRUD
- `/moves` — CRUD
- `/mypokemon` — CRUD + moves sub-resource, plus query endpoints (moves of a Pokemon, possible moves, Pokemon sharing a move)

### `Battles.API` (Battle state — Part 3)
- `/battles` — create (starts immediately), get state/history, post action (execute move)

Detailed request/response contracts to be defined when Part 2/3 implementation starts (kept out of this file to avoid drift — derive from `requirements.md` domain model at implementation time).

## Testing Approach

- **`Pokemons.Tests` (unit)**: pure, fast tests against `Pokemons.Domain` only — damage formula (table-driven, fixed `IRandomProvider`), effectiveness lookup, battle rules (turn order, HP depletion, finished-state rejection). No database, no HTTP, no `WebApplicationFactory`.
- **`Pokemons.IntegrationTests` (integration)**: exercises the real stack against each API separately — `WebApplicationFactory<Program>` for `Pokedex.API` (CRUD endpoints) and a separate one for `Battles.API` (battle flow), both against a SQLite database (temp file or `:memory:` connection kept open for the test's lifetime). Covers CRUD endpoints and full battle flow (create → execute actions → finish → query history), including out-of-turn and post-finish rejection.
- Keeping them in separate projects lets unit tests run on every save/build (fast feedback) while integration tests run less frequently (e.g., CI, pre-commit) without slowing down the inner dev loop.

### TDD vs. test-after — where each applies

> Not a blanket TDD mandate. Apply it where it earns its keep (real logic/branching), skip it where it's just plumbing.

- **TDD (test-first) for `Pokemons.Domain` logic** — `DamageCalculator`, `ITypeEffectivenessLookup`, the battle rules engine, and the Battle Turn Orchestrator. These have a documented contract (`requirements.md` formula, effectiveness matrix, turn/finish rules) and edge cases that are easy to get subtly wrong (effectiveness multipliers, 85–100% random range, out-of-turn rejection, post-finish rejection). Writing the test first pins down the contract before implementation and gives immediate regression coverage on the exercise's core deliverable (Part 1).
- **Test-after for `Pokedex.API` CRUD endpoints** — plain create/read/update against EF Core has little branching logic; write the endpoint, then add an integration test proving the contract (status codes, validation errors, the required query endpoints). TDD here is mostly busywork.
- **General rule**: if a test doesn't force a new abstraction or expose a real edge case, don't add one — this keeps testing aligned with the "no interfaces/layers just in case" principle above.

## Deferred to V2 (Backlog)

> Ideas considered during V1 but intentionally not implemented now, kept here so they aren't lost/re-litigated later.

- **`MyPokemon` stat snapshot vs. live/computed from `BasePokemon`**: V1 copies `TotalHP`/`BaseAttack`/`BaseDefense`/`Type` onto `MyPokemon` at creation time (see "Key Design Decisions" above) to keep an owned Pokémon's stats stable even if its `BasePokemon` catalog entry is edited later. The tradeoff is duplicated data that can silently drift from `BasePokemon` if that's ever undesirable (e.g. a deliberate "rebalance all owned Pokémon" feature). For V2, consider either: (a) keeping the snapshot but adding an explicit "resync stats from BasePokemon" operation, or (b) making the fields computed/read-only proxies onto `BasePokemon` (accepting that catalog edits then affect owned instances immediately) — decide based on whichever behavior the product actually wants once there's a real answer to "should editing a species retroactively change owned Pokémon."
- **`MyPokemonMove` snapshot vs. live/computed from `Move`**: same tradeoff as above, applied to `Name`/`Type`/`Power` on `MyPokemonMove` vs. `Move`. Same V2 options apply: explicit "resync move data" operation, or make the fields live proxies onto `Move` if catalog rebalances should propagate to already-assigned moves.
