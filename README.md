# earth-to-mars

## Process

Planned and built with [Matt Pocock's skills](https://github.com/mattpocock/skills) for Claude Code. The skills are installed locally and not committed. Each step runs in its own session:

1. **`/wayfinder`** charts a map of the open decisions as a GitHub issue, with one child ticket per decision. Each session resolves one ticket. Claude recommends an answer and the human decides.
2. **`/to-spec`** turns the closed map into a single spec issue.
3. **`/to-tickets`** splits the spec into build tickets. No code is scaffolded before this step.
4. **`/implement`** builds one ticket per session, test-first, in small commits.
5. **README**: Claude drafts it from the map and the commit history, and the human edits it.
6. **`/retro`** looks back on what worked.
