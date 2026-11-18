# Repository Instructions for SpaceRockIT

## Sources of record

- **Policies:** always consult the repository wiki for policies covering what you are implementing — PII handling, retention, and anything similar — and follow them. The GitHub MCP servers do not expose wiki pages, so fetch the wiki URL with the `web` tool instead. Cite the page you used. The wiki is the source of record even when the ticket does not link it.
- Resolve `owner` and `repo` for every MCP call from the `origin` remote of this repository, not from the folder name or from memory. Never read from any other repository, and in particular never fall back to the upstream repository this one was forked from.
- Take the issue number from the request. If that issue cannot be fetched from that repository, stop and say so. Do not substitute a similar issue from somewhere else.
- State the `owner/repo` and the issue title you actually fetched in your first reply, so a wrong repository is visible immediately rather than discovered three steps later.

## 1. Implementation Boundaries & Documentation Exception
- Implementation code and tests may ONLY be modified under `src/SpaceRockIT.Reviews.Api/` and `tests/SpaceRockIT.Reviews.Api.Tests/`.
- Documentation exception: `@documenter` may create or update files under `docs/`, including `docs/adr/`, when recording decisions or documenting a change. This exception does not permit code or test changes outside the paths above.
- Never modify solution structure, CI/CD pipelines, or authentication middleware.
- Keep all data persistence in-memory (no EF Core, SQLite, or external databases).

## 2. Planning & Execution Pattern
- Always output a concise implementation plan before making edits.
- State which files will be modified and which tests will be added.

## 3. Testing Obligation
- Every business logic modification requires at least one automated xUnit test in `tests/SpaceRockIT.Reviews.Api.Tests/`.
- Always use synthetic test fixtures (e.g. `alex.dev@enterprise.org`, never real user data).

## 4. Privacy & Data Guardrails
- Sanitize free-text user inputs for email addresses before writing to logs or public response payloads.
- Use the `/pii-sanitizer` skill to apply the standard redaction logic.

## 5. Destructive operations
- Never run a destructive or irreversible shell command. This includes `rm -rf`,
  `git clean -fd`, `git push --force`, dropping tables, and piping a download into a shell.
- If a task appears to need one, stop and explain what you would run and why. The human decides.
- Prefer a reversible alternative when one exists.