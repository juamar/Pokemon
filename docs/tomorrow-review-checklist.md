# Tomorrow's Review Checklist

> One-time checklist for reviewing tonight's doc changes before starting Phase 0 implementation. Once reviewed, this file can be deleted or archived — it's a session artifact, not a long-lived doc.

## Review items (target: ~20–30 min total)

- [ ] **`docs/plan.md`** (10–15 min) — confirm phase order, scope trims (6 Pokemon seed, representative-only damage-calc tests), and the "Definition of Done for V1" checklist (including the smoke-test-only note) read correctly.
- [ ] **`docs/e2e-test-plan.md`** (10–15 min) — confirm the "Smoke Test (V1)" section covers the right happy path (create 2 Pokemon → battle → execute turns → finish → history), and that the full checklist is clearly marked as deferred/backlog, not required for V1.
- [ ] **`docs/requirements.md`** (5 min) — spot-check §5 assumption 11 (6 seeded Pokemon, one per type, confirmed with recruiter) and the note above the Type Effectiveness Matrix in §1 (full matrix still seeded, but only relevant type pairs need test coverage).
- [ ] **`docs/architecture.md`** (0–10 min, only if something above looks off) — likely no changes needed; already reconciled with the `Battle`/`BattleAction` merge, CRUD-vs-orchestrator split, and TDD-vs-test-after guidance from tonight's session.

## What changed tonight (for context, no need to re-derive)

1. Merged `IBattleActionRepository` into `IBattleRepository` (`Battle`/`BattleAction` are one aggregate) — `docs/architecture.md`.
2. Documented "no service layer for plain CRUD" vs. the Battle Turn Orchestrator's explicit purpose — `docs/architecture.md`.
3. Linked `docs/requirements.md` §4 `TypeEffectiveness` to §1's Type Effectiveness Matrix.
4. Added `.github/copilot-instructions.md`: reviewable-units-of-work rules, TDD-vs-test-after testing requirement, docs-reconciliation-per-unit-of-work rule, coverage tooling reference.
5. Added `scripts/run-coverage.ps1` (coverlet + ReportGenerator, no Sonar needed) and `.gitignore` entries for `TestResults/`/`coveragereport/`.
6. Added `docs/e2e-test-plan.md`, then trimmed it to a **Smoke Test (V1)** section (required) + full checklist demoted to backlog/deferred (not required for V1).
7. Added "Definition of Done for V1" section to `docs/plan.md`.
8. Confirmed with recruiter: V1 seed data is only **6 `BasePokemon`, one per type** — not all 18 types, and exhaustive type-matchup testing is not required. Reflected in `requirements.md` (assumption 11), `plan.md` (Phase 1/2 scope), `e2e-test-plan.md`, and `README.md` (assumption 11 + Quality section).
9. `README.md`: added Quality & Security Checks section (code coverage locally, Nexus IQ/Fortify + full E2E QA explicitly out of scope for V1).

## Time budget for tomorrow (4 hours total)

- Review (above): ~20–30 min
- Phase 0 (scaffolding): ~30–40 min
- Phase 1 (domain + persistence, 6-Pokemon seed): ~45–60 min
- Phase 2 (damage calc, TDD, representative tests): ~30–40 min
- Phase 3 (Pokemon API): ~45–60 min
- Phase 4 (Battle API): ~45–60 min
- Phase 5 (polish): skip
- DoD (build/tests/coverage + smoke test + light doc touch-up): ~20–30 min
