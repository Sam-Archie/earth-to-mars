---
status: superseded by ADR-0002
---

# Commands get their own folder before the core needs one

The core is small enough to sit in one flat folder, but we moved `ICommand`, `TurnLeft`, `TurnRight` and `MoveForward` into `Commands/` (namespace `MartianRobots.Commands`) ahead of need. Commands are the brief's one planned extension point, and later tickets will add more files to the core, so we hope grouping them early keeps the core easy to navigate. The letter → command table deliberately stays a private field in `LineReaders` rather than its own class, to avoid a layer that only wraps a dictionary.

This is a bet, not a proven need. If the build finishes with `Commands/` as the only folder and no other grouping has earned its place, fold the files back into the root: it is a move and a namespace change, and no test depends on where a file lives.

## Considered Options

- **CQRS-style split into commands and queries**: rejected. "Command" already means one letter's action in the glossary, so CQRS commands would reuse the word for something else. The core has no separate read and write models to keep apart, and `CLAUDE.md` rules out layered architecture.
- **A `CommandLookup` class holding the table**: tried and removed. It was a whole class around one dictionary with a single caller.
