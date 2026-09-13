# Pokemon

## Project Overview

This repository contains the implementation for a Pokemon recruitment exercise.

The solution is being developed with a focus on:
- clear domain boundaries,
- simple and maintainable design,
- testability,
- and enough structure to support future evolution without overengineering V1.

The formal exercise requirements are documented in `requirements.md`, which is the source of truth for the exercise specification.

---

## Tech Stack

### Current / Planned
- **.NET 10**
- **SQLite** for persistence

### Libraries / Tools
As the project evolves, this section should be updated to reflect the actual libraries and packages used.

- TBD

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
11. Seed data for V1 includes only 6 Pokemon (one per selected type, confirmed with recruiter) — not all 18 types.

---

## Quality & Security Checks

- **Code coverage** for unit tests is checked locally via `coverlet.collector` + `dotnet test --collect:"XPlat Code Coverage"`, summarized with `dotnet-reportgenerator-globaltool` (see `scripts/run-coverage.ps1`). Focused on `Pokemons.Domain` (the pure logic).
- **Nexus IQ / Fortify** (or equivalent SCA/SAST scanning for dependencies, libraries, and security vulnerabilities) are **out of scope for V1** of this exercise.
- **Full end-to-end QA** is **out of scope for V1** — only a happy-path smoke test (see `docs/smoke-test-plan.md`) is run as part of the V1 Definition of Done. The full E2E checklist is kept as backlog for a future hardening pass.

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
- Nexus IQ / Fortify (or equivalent dependency/security scanning)

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

---

## Notes

- `requirements.md` is the source of truth for the exercise requirements.
- This README is a human-friendly overview of the project, current assumptions, V1 boundaries, and future ideas.

### Questions for next interview session

- Are you using AI tools (e.g., Copilot, ChatGPT, Codex, Claude) to assist with coding? Do you have any workflow or best practices for using them effectively?
- What is your approach for requirements gathering and documentation? How do you ensure that the requirements are clear, complete, and testable?