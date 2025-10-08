# AGENTS.md

## Setup commands

* Install **.NET SDK (LTS)** and a recent **Node.js** (for tooling in demos if needed).
* Restore deps: `dotnet restore`
* Build: `dotnet build -c Debug`
* Start a demo locally (pick any demo project): `dotnet run --project <path-to-demo-project>`
* Hot reload while developing components: `dotnet watch --project <path-to-demo-or-sample> run`

## Code style

* **C# (latest LTS)**.
* Enforce formatting via `.editorconfig` + `dotnet format` (run before committing).
* Public APIs require XML docs (`<summary>`, `<remarks>`, examples when useful).
* Prefer immutability and clear, functional patterns (LINQ) where appropriate.
* Use `EventCallback`/`EventCallback<T>` for Blazor events; avoid `Action` for UI events.
* Use `[Parameter]` (and `[EditorRequired]` when needed). Avoid breaking parameter renames.
* Keep rendering fast: minimize allocations in `BuildRenderTree`/`OnParametersSet`; guard re-renders with `ShouldRender` only when necessary.

## Dev environment tips

* Recommended IDEs: **Visual Studio 2022** or **VS Code** (C# Dev Kit + Razor extension).
* Open the root solution (`.sln`), build all projects once, then work in the target package.
* Use `dotnet test --filter "FullyQualifiedName~<AreaOrComponent>"` to focus tests.
* For docs/samples, run the appropriate demo app and verify examples render as expected.
* When editing shared primitives (core, utilities), run a full solution build before pushing.

## Testing instructions

* Run unit and component tests: `dotnet test -c Release`.
* Lint/format check: `dotnet format --verify-no-changes`.
* If you add a component parameter or change behavior, **add/update tests** and sample snippets.
* CI must be green before merge. Fix type warnings (treat as errors in CI).

## PR instructions

* Title format: `<area>: <short description>` (e.g., `Scheduler: Support BYDAY positions`).
* Before committing: `dotnet format` and `dotnet test` locally.
* Update XML docs and examples when touching public APIs/components.
* Note breaking changes with an `[Obsolete]` path first when possible; include migration notes.
* Link related issues in the description (e.g., `Fixes #123`, or `Closes #123`).
