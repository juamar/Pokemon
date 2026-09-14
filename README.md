# Pokemon

## Project Overview

This repository contains the implementation for a Pokemon recruitment exercise.

The solution is being developed with a focus on:
- clear domain boundaries,
- simple and maintainable design,
- testability,
- and enough structure to support future evolution without overengineering V1.

The formal exercise requirements are documented in [docs/requirements.md](docs/requirements.md), which is the source of truth for the exercise specification. See also [docs/architecture.md](docs/architecture.md) for the solution structure and [docs/plan.md](docs/plan.md) for the phased implementation roadmap.

---

## Tech Stack

### Current / Planned
**Current (implemented in V1):**
- .NET 10 (`net10.0`)
- ASP.NET Core Web API (separate `Pokedex.API` and `Battles.API`)
- EF Core + SQLite persistence
- xUnit unit and integration testing

**Planned (future/backlog):**
- Auth/users/trainers
- Advanced battle mechanics (abilities, items, weather, etc.)
- Potential architecture evolution for larger scale (if needed)

### Libraries / Tools
- `Microsoft.EntityFrameworkCore.Sqlite` `10.0.12`
- `Microsoft.EntityFrameworkCore.Design` `10.0.12`
- `Swashbuckle.AspNetCore` `10.2.3`
- `Scalar.AspNetCore` `2.17.3`
- `xunit` `2.9.3`
- `xunit.runner.visualstudio` `3.1.4`
- `Microsoft.NET.Test.Sdk` `17.14.1`
- `Microsoft.AspNetCore.Mvc.Testing` `10.0.12`
- `coverlet.collector` `6.0.4`

---

## How to run

### 1) Restore
From solution root:

```bash
dotnet restore Pokemon.slnx
```

### 2) Run each API
Run each API in a separate terminal:

```bash
dotnet run --project Pokedex.API/Pokedex.API.csproj
dotnet run --project Battles.API/Battles.API.csproj
```

### 3) API URLs
- **Pokedex API base URL**: `http://localhost:5156`
- **Battles API base URL**: `http://localhost:5169`

### 4) OpenAPI/Scalar URLs
- **Pokedex OpenAPI JSON**: `http://localhost:5156/swagger/v1/swagger.json`
- **Pokedex Scalar UI**: `http://localhost:5156/scalar`
- **Battles OpenAPI JSON**: `http://localhost:5169/swagger/v1/swagger.json`
- **Battles Scalar UI**: `http://localhost:5169/scalar`

### 5) Run tests

```bash
dotnet test Pokemon.slnx
```

Or run by project:

```bash
dotnet test Pokemons.Tests/Pokemons.Tests.csproj
dotnet test Pokemons.IntegrationTests/Pokemons.IntegrationTests.csproj
```

### Troubleshooting: database/migrations
Migrations are applied automatically at API startup. If your local DB is out of sync, you can run EF manually:

```bash
dotnet ef database update --project Pokemons.Infra/Pokemons.Infra.csproj --startup-project Pokedex.API/Pokedex.API.csproj
dotnet ef database update --project Pokemons.Infra/Pokemons.Infra.csproj --startup-project Battles.API/Battles.API.csproj
```

---

## V1 Assumptions

The following assumptions are currently taken for V1:

1. No special distinction is required between physical and special attack.
2. No special distinction is required between physical and special defense.
3. Pokemon are single-type only.
4. Battle phases are simple turn-based action updates.
5. Battle starts immediately when created.
6. Finished battles are read-only except for history/state queries.
7. SQLite is sufficient for persistence in V1.
8. The random factor can be controlled in tests for deterministic validation.
9. `ownerId` is a string.
10. Trainers, users, and authentication are out of scope for V1.
11. Seed data for V1 includes only 6 real Pokemon sourced from pokemondb.net (Charmander, Squirtle, Clefairy, Rattata, Ekans, Pikachu — National Dex #0004/#0007/#0035/#0019/#0023/#0025, confirmed with recruiter) — not all 18 types/species, with their level-up attacking moves (one high-level move per Pokemon intentionally left unseeded for manual smoke-test addition).

---

## Out of Scope for V1

- Authentication
- User management
- Trainers
- External PokemonDB integration
- Multiplayer battles
- Advanced battle mechanics not described in the exercise
- UI/front-end
- Distributed-system architecture work beyond keeping the design decoupled

---

## V2 Proposals / Future Improvements

The following ideas are intentionally left for a future version:

- Trainers and authenticated ownership
- Real user accounts and authorization
- Battle pre-start / pending state
- Dual-type Pokemon
- Physical/special attack separation
- Physical/special defense separation
- More advanced battle mechanics
- Switching Pokemon mid-battle
- Items, abilities, status conditions, critical hits, weather
- Richer battle analytics and replay features
- Moving from a monolith to a distributed architecture if needed later
- **Query ambiguity clarification**: The PDF requirement "Pokémons que comparten un mismo movimiento" is currently interpreted in V1 as BasePokemons (catalog) that share a move type, useful for team-building discovery. V2 may also extend this to owned MyPokemons (e.g., "which of my Pokemon know this move?") for in-battle move management. Both queries could coexist depending on use case.
- **Open pending topic: in-game (runtime) updates to `BasePokemon`/`Move` catalog data.** V1 has `MyPokemon` and `MyPokemonMove` snapshot their stats/move data from `BasePokemon`/`Move` at creation/assignment time, so editing a catalog entry later does not retroactively change already-owned Pokemon or already-assigned moves (see `docs/architecture.md` "Deferred to V2 (Backlog)" for the full rationale). For V2 we still need to decide the actual product behavior once catalog editing is a real, live-game feature: should species/move rebalances ever propagate to existing instances (explicit resync operation vs. always-live computed stats), and if so, how (opt-in per player, automatic, versioned)? This is currently unresolved and needs a design decision before it's implemented.

---

## Notes

- [docs/requirements.md](docs/requirements.md) is the source of truth for the exercise requirements.
- This README is a human-friendly overview of the project, current assumptions, V1 boundaries, and future ideas.
- See also: [docs/architecture.md](docs/architecture.md), [docs/plan.md](docs/plan.md), [docs/smoke-test-plan.md](docs/smoke-test-plan.md).
- **Damage calculation entry point (Part 1 core deliverable)**: `Pokemons.Domain/DamageCalculator.cs` (`DamageCalculator.CalculateAsync(...)`).

### Questions for next interview session

- Are you using AI tools (e.g., Copilot, ChatGPT, Codex, Claude) to assist with coding? Do you have any workflow or best practices for using them effectively?
- What is your approach for requirements gathering and documentation? How do you ensure that the requirements are clear, complete, and testable?