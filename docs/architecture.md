# Architecture

> Companion to `docs/requirements.md` (authoritative spec). This file describes **how** the system is structured, not **what** it must do.

## Guiding Principle

Keep it simple. No Clean Architecture ceremony, no CQRS/MediatR, no premature interfaces "just in case." Split only where it gives real decoupling for this exercise (testability of damage/battle logic, swappable persistence).

## Solution Structure

```
Pokemon.slnx
├── src/
│   ├── Pokedex.API/            # ASP.NET Core Web API — controllers/endpoints, DI wiring, EF migrations host
│   ├── Pokedex.Domain/         # Entities, enums, damage calculation, battle rules engine, type effectiveness. No EF/DB references.
│   └── Pokedex.Infrastructure/ # EF Core DbContext, SQLite provider, repositories, seed data
└── tests/
	└── Pokedex.Tests/          # Unit tests (damage calc, battle rules) + integration tests (API, EF)
```

### Why this split
| Project | Responsibility | Depends on |
|---|---|---|
| `Pokedex.Domain` | Entities, `IDamageCalculator`, `IRandomProvider` (for deterministic tests), battle state machine, type chart lookup | Nothing (pure C#) |
| `Pokedex.Infrastructure` | `PokedexDbContext`, EF configurations, SQLite setup, repository implementations | `Pokedex.Domain` |
| `Pokedex.API` | Minimal API/Controllers, request/response DTOs, validation, DI composition | `Pokedex.Domain`, `Pokedex.Infrastructure` |
| `Pokedex.Tests` | xUnit tests | All of the above |

This gives just enough decoupling to unit-test damage/battle logic without a database, per requirements' testability goal — without introducing extra layers (Application/CQRS) the exercise doesn't need.

## Key Design Decisions

- **Randomness is injected** (`IRandomProvider` in Domain, real implementation in Infrastructure/API, fake/fixed implementation in tests) to satisfy the deterministic-random-in-tests assumption.
- **Damage calculation is a pure function/service** in Domain: `(attacker, move, defender) -> int damage`, no side effects, no DB access — easy to unit test against the formula directly.
- **Battle rules engine** lives in Domain as well: turn validation, HP application, finish-state transition. The API layer only orchestrates (load battle → call domain method → persist → return DTO).
- **EF Core + SQLite** in Infrastructure only; Domain and tests never reference EF directly (entities are POCOs).
- **Type effectiveness matrix** stored as seed data in Infrastructure (table or JSON seed), read through a small Domain-facing lookup service/interface.
- **No auth/trainers** — `OwnerId` is a plain string field, no identity integration.

## API Surface (high level)

- `/basepokemon` — CRUD
- `/moves` — CRUD
- `/mypokemon` — CRUD + moves sub-resource, plus query endpoints (moves of a Pokemon, possible moves, Pokemon sharing a move)
- `/battles` — create (starts immediately), get state/history, post action (execute move)

Detailed request/response contracts to be defined when Part 2/3 implementation starts (kept out of this file to avoid drift — derive from `requirements.md` domain model at implementation time).

## Testing Approach

- Domain: pure unit tests for damage formula (table-driven, fixed random) and battle rules (turn order, HP depletion, finished-state rejection).
- Infrastructure/API: integration tests against SQLite (in-memory or temp file) for CRUD and battle flow end-to-end.
