# Architectural Boundaries & Scoping Policy

## Allowed File Modifications
When implementing API endpoints and tests:
- Source Code: `src/SpaceRockIT.Reviews.Api/**`
- Test Suites: `tests/SpaceRockIT.Reviews.Api.Tests/**`

## Forbidden Modifications (Out-of-Scope)
- Never modify solution structure or global project files (`*.sln`, `*.csproj` package references).
- Never introduce external database engines, ORMs, or distributed caches.
- Never modify authentication middleware or infrastructure scripts.