# Repository Guidelines

## Project Structure & Module Organization

- `Source/`: main Blazorise libraries (core in `Source/Blazorise`, providers in `Source/Blazorise.*`, extensions in `Source/Extensions`).
- `Build/`: shared MSBuild props imported by projects.
- `Tests/`: unit tests (`Blazorise.Tests`, `Blazorise.Analyzers.Tests`) and E2E tests (`Blazorise.E2E.Tests`) plus test apps (`BasicTestApp.*`).
- `Demos/`: runnable sample apps for each supported UI provider.
- `Documentation/`: docs site source, generator, and server (`Documentation/Blazorise.Docs.Server`).
- `NuGet/`: local packaging helpers/scripts (not the source of truth for versions).
- Generated documentation (example HTML, `Snippets.generated.cs`, API documentation, and search indexes) belongs under each project's `obj/<configuration>/<framework>/DocsGenerated/` directory, with a runtime identifier when applicable. Do not edit generated outputs or recreate the former source-tree artifacts.
- Maintain documentation examples in their handwritten `.razor`, `.snippet`, and `.csharp` sources. `Documentation/Blazorise.Docs/Models/Snippets.cs` is the handwritten helper, not a generated artifact.
- Blog HTML uses a separate workflow; do not remove it as part of compiler-generated example cleanup.

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

CI builds with .NET SDK `11.0.100-rc.2.26504.105`. From the repo root:

```powershell
dotnet restore
dotnet build -c Release --no-restore
dotnet test .\Tests\Blazorise.Tests\Blazorise.Tests.csproj -c Release --no-build
pwsh .\Tests\Blazorise.E2E.Tests\bin\Release\net11.0\playwright.ps1 install --with-deps
dotnet test .\Tests\Blazorise.E2E.Tests\Blazorise.E2E.Tests.csproj -c Release --no-build
```

Docs server:

```powershell
cd .\Documentation\Blazorise.Docs.Server
dotnet watch run
```

Cleanup: `clean.bat` (removes `bin/`, `obj/`, and generated docs artifacts).

## Repository Skills

Use the relevant repository skills before implementing or reviewing work in these areas. Load only the skills relevant to the task; changes spanning multiple areas may need more than one.

| Work | Required skill |
| --- | --- |
| Blazorise library component behavior, state, parameters, events, rendering, provider overrides, or interop contracts | [blazorise-components](.agents/skills/blazorise-components/SKILL.md) |
| CSS provider SCSS, native tokens, class names, styling hooks, or runtime theming | [blazorise-provider-styling](.agents/skills/blazorise-provider-styling/SKILL.md) |
| Documentation pages, handwritten examples, snippets, or API explanations | [blazorise-documentation](.agents/skills/blazorise-documentation/SKILL.md) |

For C# changes, read the shared [formatting examples](.agents/skills/blazorise-components/references/csharp-formatting.md) alongside the coding rules below. Skill invocation does not authorize command execution; the execution policy above applies to all skills.

## Coding Style & Naming Conventions

- Follow `.editorconfig` for repository formatting such as 4-space indentation, CRLF endings, spacing, and brace placement.
- Do not split handwritten classes across partial files (for example, `{Type}.Properties.cs` or `{Type}.Selection.cs`). Keep a class in one file and extract distinct responsibilities into separate types when needed. Partial declarations required by Razor or source generation are the only exception.
- Prefer `var` whenever the type can be inferred; use an explicit type only when it cannot. This rule governs new and edited code even where older files or silent IDE preferences still use explicit locals.
- Always preserve consistent `CRLF` line endings per file; never introduce mixed `LF`/`CRLF` endings in the same file.
- Do not add a trailing newline at EOF, except in `*.css` and `*.scss` files; keep all other file endings without an extra line.
- Keep a declaration or expression on one line when it represents one short, direct operation and remains easy to scan. Typical examples are auto-properties, expression-bodied properties and accessors, simple forwarding methods, compact record `with` expressions, and short initializers.
- Use a block body when a member validates input, changes state, performs multiple operations, or benefits from named intermediate values. Do not compress stateful behavior merely to reduce line count.
- There is no fixed maximum line length. Wrap code when its structure becomes easier to understand, not solely because it crosses an arbitrary column. Keep short signatures on one line; for long signatures, inheritance lists, argument lists, initializers, and fluent chains, place one structural item or operation on each continuation line.
- Keep short Boolean expressions on one line when they remain easy to scan, including a method call followed by a simple comparison, for example `var valueChanged = parameters.TryGetValue<int>( nameof( Value ), out var paramValue ) && paramValue != Value;`. Do not wrap solely because an expression contains `&&` or `||`.
- Prefer multiline Boolean declarations when they combine distinct method-call checks whose purpose or arguments are easier to scan separately, such as a presence check followed by a string comparison. The short-expression preference does not require flattening every two-operand declaration.
- Multiline Boolean conditions remain appropriate when their operands represent distinct checks or validation steps, including two related method calls such as an exact string comparison followed by a suffix check. Existing multiline `if` and `while` conditions, Boolean `return` expressions, and expression-bodied Boolean members may keep one operand per line even when short; the compact-expression preference does not require flattening them. Preserve readable grouping and do not mechanically collapse conditions based on length or operand count.
- When a Boolean condition spans multiple lines, keep the first operand on the opening line and begin continuation lines with `&&` or `||`. Indent nested condition groups one additional level.
- Always use braces for `if`, `else`, `for`, `foreach`, `while`, and `using` bodies in new or substantially edited code, including single-statement bodies. Put `else`, `catch`, and `finally` on a new line after the preceding closing brace.
- Prefer guard clauses and early returns to reduce nesting. Do not add an `else` after a branch that unconditionally returns, throws, continues, or breaks.
- Use blank lines to separate semantic phases within a method, such as validation, calculation, state mutation, invalidation/rendering, and the final return. Keep tightly related statements together and do not insert blank lines between every statement.
- Separate a multiline local declaration or assignment from adjacent statements with a blank line, including between consecutive multiline Boolean expressions. Keep related short, single-line declarations together.
- Keep homogeneous operation sequences together, such as consecutive `builder.Append` calls or related event subscriptions. Add a blank line before a distinct operation such as a `base` call, return, or a new logical phase.
- Separate fields and documented members with blank lines instead of creating dense declaration blocks. Do not use multiple declarations on one line.
- Format XML documentation blocks such as `<summary>` and `<remarks>` with the opening tag, content, and closing tag on separate `///` lines, even for a single sentence. Self-closing tags such as `<inheritdoc />` and inline references such as `<see cref="..."/>` may remain compact.
- Wrap explicit C# `using` directives in a `Using directives` region. Sort `System` directives first and do not create blank-line subgroups inside that region.
- Use regions to organize substantial component and service types in this order: `Events` when applicable, `Members`, `Constructors`, `Methods`, and `Properties`. Small data-only types do not need empty or artificial regions.
- Place simple parameter and injected auto-properties on one line with their attribute, for example `[Parameter] public bool Disabled { get; set; }`. Put the attribute on its own line when the property has a custom accessor body.
- Use expression-bodied members for properties, accessors, and methods that perform one direct expression. If the declaration is long, place `=>` on the following indented line. Do not use expression bodies for multi-step logic.
- Prefer `is null`, `is not null`, property patterns, null propagation, and null coalescing over verbose null checks or casts followed by null checks.
- Keep object and collection initializers multiline when they contain several meaningful entries, and include a trailing comma in multiline initializers. A short initializer that represents one compact value may remain on one line.
- Keep `switch` labels indented one level inside the switch, with statements indented one further level. Do not insert blank lines between short, structurally identical cases; use blank lines only to separate materially different case groups.
- Wrap LINQ and render-tree fluent chains with one operation per line once the chain no longer reads clearly as a single expression.
- When editing `*.scss` files, do not manually edit generated `*.css` files; CSS will be generated manually by the team.
- In Razor markup, prefer Blazorise components (for example `Div`, `Span`) and Blazorise utility parameters (for example `Flex`, `Gap`, `Margin`, `Padding`) instead of raw HTML layout tags and inline styles whenever possible.
- Avoid adding custom CSS when Blazorise components and utilities can provide the required appearance. Especially avoid scoped CSS (`*.razor.css`); add styling only where existing components and utilities cannot express the requirement, following the provider styling skill when applicable.
- Naming: PascalCase for types, methods, and properties; camelCase for private fields and local variables; interfaces start with `I`.
- Use `Effective{Name}` for the final semantic value after resolving parameters, parent state, global options, theme settings, and defaults. Do not use `Effective` for string formatting or serialization.
- Prefix computed boolean members according to intent: `Is{Name}` for current state, `Has{Name}` for presence or ownership, `Can{Verb}` for capability, and `Should{Verb}` for an operation decision. `Should` must be followed by a verb such as `ShouldApply`, `ShouldRender`, or `ShouldSubscribe`.
- Use `{Name}String` for values converted specifically for Razor markup, CSS, JavaScript, or serialization. Use `To{Name}String` for the corresponding conversion method, `Resolve{Name}` for semantic value resolution, and `Format{Name}` for display formatting.
- Name captured parameter metadata `param{Name}` (for example, `paramInline`). Reserve `Defined` for whether a parameter was explicitly supplied instead of using it for non-empty content or ordinary state.
- Use `{Part}ElementId`, `{Part}ClassNames`, and `{Part}StyleNames` for rendered element identifiers, class strings, and style strings; use matching `{Part}ClassBuilder`, `{Part}StyleBuilder`, `Build{Part}Classes`, and `Build{Part}Styles` names for their builders and callbacks.
- Reserve `Value` for an actual component or domain value, `Name` for an actual name or identifier, and `State` for aggregate state. Do not use these suffixes for serialized attribute strings or individual boolean conditions.
- Keep private names context-aware and avoid repeating the component type unless needed to remove ambiguity.
- Dependency versions are centrally managed in `Directory.Packages.props` (don’t hardcode `Version=` in `PackageReference`).

## Component Development

- Never fetch, refresh, or reload data from `OnParametersSet` or `OnParametersSetAsync`. Do not rebuild scenes or recalculate geometry unconditionally in these hooks. Detect relevant changes in `SetParametersAsync` using `ParameterViewExtensions`, and perform updates only when their inputs change or an explicit operation requests them. Moving unconditional work to another lifecycle hook does not satisfy this rule.
- Prefer native C# and Blazor for component logic, state, layout calculations, and rendering wherever practical. Keep JavaScript interop limited to browser capabilities that require it, and reuse existing interop modules before adding new JavaScript.
- Use the existing document observer services where document or window events are needed, including pointer tracking, outside interactions, and focus handling. Reuse the shared subscription and disposal patterns instead of adding component-specific global JavaScript listeners.
- When adding a new component, include simple examples in the appropriate demo app. When introducing its demo page, add the sidebar entry and mark it as "New" using the existing sidebar convention.

## Testing Guidelines

- Unit tests: xUnit + bUnit (`Tests/Blazorise.Tests`). Match existing naming like `*ComponentTest.cs`.
- E2E: Playwright + NUnit (`Tests/Blazorise.E2E.Tests`). See `Tests/Blazorise.E2E.Tests/ReadMe.md` for codegen/debug tips and `.runsettings` for headless settings.
- Add only the minimum focused tests needed to cover the behavior; avoid redundant tests of the same rendering path.

## Commit & Pull Request Guidelines

- Branching: target `master` for new work; target `rel-X.Y` for maintenance. Use `dev-*` (features) and `rel-*` (release fixes) branch prefixes.
- Commit subjects typically follow `Area: short summary` and often reference PR/issue numbers (e.g., `DataGrid: sync selected rows (#6309)`).
- PRs: follow `.github/pull_request_template.md`, include “How Has This Been Tested?”, link issues (e.g., `Closes #123`), and add screenshots for UI/visual changes.

## Security & Configuration

- Report vulnerabilities via `.github/SECURITY.md`; do not commit secrets or private keys.
- If working on platform-specific demos (e.g., MAUI/Tizen), check `workload-install.ps1` for workload setup.