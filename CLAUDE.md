# Martian Robots — TDL developer challenge

The brief is `docs/brief.md` (source of truth for the problem). `README.md` is the reviewer-facing deliverable: how to run it, technical choices, approach, assumptions, what would be built around it, and how AI was used and where it wasn't.

## Standing decisions

- Stack: .NET 10, pinned in `global.json` (`rollForward: latestFeature`), xunit. A reviewer should need nothing beyond the SDK: `dotnet test` from the root runs everything.
- Layout: one `.sln` at the root; `src/MartianRobots` (core), `src/MartianRobots.Cli` (console adapter), `src/MartianRobots.Api` (HTTP adapter), `tests/MartianRobots.Tests`.
- Shape: the core is a class library with no `Console` and no ASP.NET reference, exposing parse → simulate → format. The console app is a separate, thin adapter (read stdin, call the core, write stdout). The API is a second thin adapter: one `POST /missions` route, each request its own mission, no CORS, auth or stubs. Tests target the core directly, plus one end-to-end test that runs the brief's sample through the CLI and one through the API.
- Scope: the program, its minimal API and their tests. No UI, database or pipeline is built; the README describes what would come next.
- Simplicity: this is a two-to-three-hour piece of work and the brief asks for "as simple as possible". No layered architecture. One deliberate seam for future command types; everything else plain.
- Every call made where the brief leaves something open is recorded as an assumption in the README.

## Code

- `Nullable` enabled in every project.
- Value types are records (e.g. a position).
- File-scoped namespaces.
- No analyzer or style packages beyond the SDK.

## Tests

- Written test-first: each new test is seen failing before the code that passes it.
- No test that only re-asserts the implementation. For example, none of these:
  - asserting a value the test itself just set (constructor sets a property, getter returns it);
  - computing the expected value with the same logic as the code under test;
  - testing the language or framework (an enum has four members, records compare by value).
- Expected values are worked out by hand from the brief (e.g. the sample's `3 3 N LOST`).
- Test names describe behaviour in domain words.

## Commits

The commit history is reviewed.

- Small commits: one behaviour or one decision each.
- Messages are clear and easy to read, in the imperative ("Ignore forward move off a scented edge").
- `dotnet test` passes on every commit.
- AI-assisted commits keep the `Co-Authored-By: Claude` trailer.

## Process

Planned with Matt Pocock's skills (installed in `.claude/skills/`, gitignored). The order is `/wayfinder` → `/to-spec` → `/to-tickets` → `/implement` → README → `/retro`. Once the map exists, its Notes section is the standing context.

- Wayfinder plans; nothing is scaffolded before `/to-tickets`.
- One ticket per session.
- Grilling tickets are human-in-the-loop: ask the user, never answer for them.
- The map's Notes stay plan-only.

## Agent skills

### Issue tracker

Issues live in GitHub Issues on `Sam-Archie/earth-to-mars`, managed with `gh`. See `docs/agents/issue-tracker.md`.

### Triage labels

The five default triage roles, each label named after its role. See `docs/agents/triage-labels.md`.

### Domain docs

Single-context: one `GLOSSARY.md` and `docs/adr/` at the repo root. See `docs/agents/domain.md`.
