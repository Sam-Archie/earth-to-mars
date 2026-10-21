---
status: accepted
---

# One core project, with a Domain folder and one folder per process

The flat core made it hard to see at a glance which files were domain things and which were processes, so it is grouped by role inside the one `src/MartianRobots` project. `Domain/` holds the things and the rules (planets, positions, commands, scents, claims), and one folder per process sits beside it: `Missions/` runs robots, `Input/` reads Earth's lines, `Map/` draws the mission map. Splitting the core this way also moved the letter → command table out of the line readers into `Domain/Commands/KnownCommands`, and scents and claims out of `Mission` into their own classes.

This supersedes [ADR 0001](0001-commands-folder-ahead-of-need.md): its bet that the core would need folders paid off.

## Considered Options

- **Domain and Application projects (Clean Architecture layers)**: prototyped and rejected. The compiler enforced the boundary, but at this size it cost two extra projects and made the commands and the scent and claim records public just to cross the boundary, for the same separation the folders give.
- **Flat concept folders** (`Planets/`, `Commands/`, `Missions/` …, with no `Domain/`): rejected because `Missions/` would hold domain records such as scents beside the process that runs robots.

## Consequences

- The edge-and-scent rule stays inside `Mission`, though it is a domain rule. Moving it would mean inventing a robot type the glossary doesn't have, to move ten lines.
- The console and the API keep reading a mission separately. They share only the order of the lines; re-asking a bad line versus rejecting the mission is adapter behaviour.
