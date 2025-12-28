# Repository Guidelines

## Project Structure & Module Organization

- `Source/`: main Blazorise libraries (core in `Source/Blazorise`, providers in `Source/Blazorise.*`, extensions in `Source/Extensions`).
- `Build/`: shared MSBuild props imported by projects.
- `Tests/`: unit tests (`Blazorise.Tests`, `Blazorise.Analyzers.Tests`) and E2E tests (`Blazorise.E2E.Tests`) plus test apps (`BasicTestApp.*`).
- `Demos/`: runnable sample apps for each supported UI provider.
- `Documentation/`: docs site source, generator, and server (`Documentation/Blazorise.Docs.Server`).
- `NuGet/`: local packaging helpers/scripts (not the source of truth for versions).
- Docs snippets (`Documentation/Blazorise.Docs/Models/Snippets*.cs`) are generated artifacts: do not touch any snippet files during AI work (do not create new ones like `Snippets.*.cs`), and avoid incidental diffs from running the docs build.

## Build, Test, and Development Commands

⚠️ **AI Agent Execution Policy**

AI agents (including Codex and similar tools) **MUST NOT automatically run build, test, restore, or other shell commands** unless explicitly instructed by a human.

This includes, but is not limited to:
- `dotnet restore`
- `dotnet build`
- `dotnet test`
- `npm install`
- `npm run build`
- Any command that modifies the local file system, caches, or generated artifacts

### Default behavior

- Assume **read-only analysis** by default
  - Perform static code review and reasoning
  - Read-only search/view commands are OK (e.g. `rg`, `Select-String`, `Get-ChildItem`, `Get-Content`)
- Do **not** execute commands to “verify” changes unless explicitly asked

### When execution is allowed

Commands may be executed **only** when the user explicitly says so (e.g. “run a build”, “verify by building”, “execute this command”).

### Rationale

Blazorise is a large multi-project repository. Unsolicited command execution:
- wastes time and resources
- pollutes local or sandboxed caches
- creates unnecessary diffs and artifacts
- is rarely required for documentation, refactoring, or review tasks

---

CI builds with .NET SDK `10.0.x`. From the repo root:

```powershell
dotnet restore
dotnet build -c Release --no-restore
dotnet test .\Tests\Blazorise.Tests\Blazorise.Tests.csproj -c Release --no-build
pwsh .\Tests\Blazorise.E2E.Tests\bin\Release\net10.0\playwright.ps1 install --with-deps
dotnet test .\Tests\Blazorise.E2E.Tests\Blazorise.E2E.Tests.csproj -c Release --no-build
```

Docs server:

```powershell
cd .\Documentation\Blazorise.Docs.Server
dotnet watch run
```

Cleanup: `clean.bat` (removes `bin/`, `obj/`, and generated docs artifacts).

## Coding Style & Naming Conventions

- Follow `.editorconfig`: 4-space indentation, CRLF endings, braces preferred, and explicit types (avoid `var` unless it improves clarity).
- Naming: PascalCase for types/members; interfaces start with `I`.
- Dependency versions are centrally managed in `Directory.Packages.props` (don’t hardcode `Version=` in `PackageReference`).

## Testing Guidelines

- Unit tests: xUnit + bUnit (`Tests/Blazorise.Tests`). Match existing naming like `*ComponentTest.cs`.
- E2E: Playwright + NUnit (`Tests/Blazorise.E2E.Tests`). See `Tests/Blazorise.E2E.Tests/ReadMe.md` for codegen/debug tips and `.runsettings` for headless settings.

## Commit & Pull Request Guidelines

- Branching: target `master` for new work; target `rel-X.Y` for maintenance. Use `dev-*` (features) and `rel-*` (release fixes) branch prefixes.
- Commit subjects typically follow `Area: short summary` and often reference PR/issue numbers (e.g., `DataGrid: sync selected rows (#6309)`).
- PRs: follow `.github/pull_request_template.md`, include “How Has This Been Tested?”, link issues (e.g., `Closes #123`), and add screenshots for UI/visual changes.

## Security & Configuration

- Report vulnerabilities via `SECURITY.md`; do not commit secrets or private keys.
- If working on platform-specific demos (e.g., MAUI/Tizen), check `workload-install.ps1` for workload setup.
