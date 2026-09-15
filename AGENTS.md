# Repository Guidelines

## Project Structure & Module Organization

- `Source/`: main Blazorise libraries (core in `Source/Blazorise`, providers in `Source/Blazorise.*`, extensions in `Source/Extensions`).
- `Build/`: shared MSBuild props imported by projects.
- `Tests/`: unit tests (`Blazorise.Tests`, `Blazorise.Analyzers.Tests`) and E2E tests (`Blazorise.E2E.Tests`) plus test apps (`BasicTestApp.*`).
- `Demos/`: runnable sample apps for each supported UI provider.
- `Documentation/`: docs site source, generator, and server (`Documentation/Blazorise.Docs.Server`).
- `NuGet/`: local packaging helpers/scripts (not the source of truth for versions).
- Docs snippets (`Documentation/Blazorise.Docs/Models/Snippets*.cs`) are generated artifacts: do not touch any snippet files during AI work (do not create new ones like `Snippets.*.cs`), and avoid incidental diffs from running the docs build.
- `Documentation/Blazorise.Docs/ApiDocs/Blazorise.ApiDocs.cs` is generated: do not edit or touch this file with AI/Codex.
- `Documentation/Blazorise.Docs/Models/Snippets.generated.cs`, `Documentation/Blazorise.Docs/Resources/docs-index.json`, and `Documentation/Blazorise.Docs/Resources/docs-api-index.json` are generated automatically by docs tooling; incidental changes to these files are expected and should be ignored by AI agents unless explicitly requested otherwise.

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

- Follow `.editorconfig` for repository formatting such as 4-space indentation, CRLF endings, spacing, and brace placement.
- Do not split handwritten classes across partial files (for example, `{Type}.Properties.cs` or `{Type}.Selection.cs`). Keep a class in one file and extract distinct responsibilities into separate types when needed. Partial declarations required by Razor or source generation are the only exception.
- Prefer `var` whenever the type can be inferred; use an explicit type only when it cannot. This rule governs new and edited code even where older files or silent IDE preferences still use explicit locals.
- Always preserve consistent `CRLF` line endings per file; never introduce mixed `LF`/`CRLF` endings in the same file.
- Do not add a trailing newline at EOF, except in `*.css` and `*.scss` files; keep all other file endings without an extra line.
- Keep a declaration or expression on one line when it represents one short, direct operation and remains easy to scan. Typical examples are auto-properties, expression-bodied properties and accessors, simple forwarding methods, compact record `with` expressions, and short initializers.
- Use a block body when a member validates input, changes state, performs multiple operations, or benefits from named intermediate values. Do not compress stateful behavior merely to reduce line count.
- There is no fixed maximum line length. Wrap code when its structure becomes easier to understand, not solely because it crosses an arbitrary column. Keep short signatures on one line; for long signatures, inheritance lists, argument lists, initializers, and fluent chains, place one structural item or operation on each continuation line.
- When a Boolean condition spans multiple lines, keep the first operand on the opening line and begin continuation lines with `&&` or `||`. Indent nested condition groups one additional level.
- Always use braces for `if`, `else`, `for`, `foreach`, `while`, and `using` bodies in new or substantially edited code, including single-statement bodies. Put `else`, `catch`, and `finally` on a new line after the preceding closing brace.
- Prefer guard clauses and early returns to reduce nesting. Do not add an `else` after a branch that unconditionally returns, throws, continues, or breaks.
- Use blank lines to separate semantic phases within a method, such as validation, calculation, state mutation, invalidation/rendering, and the final return. Keep tightly related statements together and do not insert blank lines between every statement.
- Keep homogeneous operation sequences together, such as consecutive `builder.Append` calls or related event subscriptions. Add a blank line before a distinct operation such as a `base` call, return, or a new logical phase.
- Separate fields and documented members with blank lines instead of creating dense declaration blocks. Do not use multiple declarations on one line.
- Wrap explicit C# `using` directives in a `Using directives` region. Sort `System` directives first and do not create blank-line subgroups inside that region.
- Use regions to organize substantial component and service types in this order: `Events` when applicable, `Members`, `Constructors`, `Methods`, and `Properties`. Small data-only types do not need empty or artificial regions.
- Place simple parameter and injected auto-properties on one line with their attribute, for example `[Parameter] public bool Disabled { get; set; }`. Put the attribute on its own line when the property has a custom accessor body.
- Use expression-bodied members for properties, accessors, and methods that perform one direct expression. If the declaration is long, place `=>` on the following indented line. Do not use expression bodies for multi-step logic.
- Prefer `is null`, `is not null`, property patterns, null propagation, and null coalescing over verbose null checks or casts followed by null checks.
- Keep object and collection initializers multiline when they contain several meaningful entries, and include a trailing comma in multiline initializers. A short initializer that represents one compact value may remain on one line.
- Keep `switch` labels indented one level inside the switch, with statements indented one further level. Do not insert blank lines between short, structurally identical cases; use blank lines only to separate materially different case groups.
- Wrap LINQ and render-tree fluent chains with one operation per line once the chain no longer reads clearly as a single expression.
- When editing `*.scss` files, do not manually edit generated `*.css` files; CSS will be generated manually by the team.
- In `*.scss` files, use native SCSS syntax to reduce repetition: group selectors that share declarations, nest related descendants, modifiers, states, and pseudo-selectors under their common parent with `&`, and use loops, maps, or mixins for repeated rule families.
- Keep SCSS nesting logical and reasonably shallow, and preserve the compiled selector specificity, cascade order, and behavior; do not nest unrelated selectors merely because they appear in the same file.
- Define reusable SCSS maps and lists in the provider's `_variables.scss` file instead of declaring them inline in component or utility partials.
- When styling provider components, always use the CSS provider's native CSS variables or design tokens whenever suitable tokens exist.
- Provider-owned CSS classes must follow the current provider's native naming convention and prefix (for example, `ant-*` in AntDesign); do not introduce shared `b-*` class names for provider-specific selectors or runtime hooks.
- Use provider-prefixed classes for provider styling hooks. Reserve data attributes for semantic state, and serialize their values as lowercase kebab-case.
- Introduce new CSS variables only when the provider does not expose suitable native variables.
- For providers without native CSS variables but with SCSS variables, prefer the provider's SCSS variables for compiled defaults and override the relevant selectors and values in the provider theme generator for runtime Blazorise theming.
- Any new CSS variables must follow the CSS provider's established variable naming convention; do not invent a provider prefix or use a shared cross-provider naming convention.
- In Razor markup, prefer Blazorise components (for example `Div`, `Span`) and Blazorise utility parameters (for example `Flex`, `Gap`, `Margin`, `Padding`) instead of raw HTML layout tags and inline styles whenever possible.
- Naming: PascalCase for types/members; interfaces start with `I`.
- Use `Effective{Name}` for the final semantic value after resolving parameters, parent state, global options, theme settings, and defaults. Do not use `Effective` for string formatting or serialization.
- Prefix computed boolean members according to intent: `Is{Name}` for current state, `Has{Name}` for presence or ownership, `Can{Verb}` for capability, and `Should{Verb}` for an operation decision. `Should` must be followed by a verb such as `ShouldApply`, `ShouldRender`, or `ShouldSubscribe`.
- Use `{Name}String` for values converted specifically for Razor markup, CSS, JavaScript, or serialization. Use `To{Name}String` for the corresponding conversion method, `Resolve{Name}` for semantic value resolution, and `Format{Name}` for display formatting.
- Name captured parameter metadata `param{Name}` (for example, `paramInline`). Reserve `Defined` for whether a parameter was explicitly supplied instead of using it for non-empty content or ordinary state.
- Use `{Part}ElementId`, `{Part}ClassNames`, and `{Part}StyleNames` for rendered element identifiers, class strings, and style strings; use matching `{Part}ClassBuilder`, `{Part}StyleBuilder`, `Build{Part}Classes`, and `Build{Part}Styles` names for their builders and callbacks.
- Reserve `Value` for an actual component or domain value, `Name` for an actual name or identifier, and `State` for aggregate state. Do not use these suffixes for serialized attribute strings or individual boolean conditions.
- Keep private names context-aware and avoid repeating the component type unless needed to remove ambiguity.
- Dependency versions are centrally managed in `Directory.Packages.props` (don’t hardcode `Version=` in `PackageReference`).

## Documentation Guidelines

- In documentation page introductions, including `DocsPageLead` and opening paragraphs, focus on why the feature is useful, the problems it solves, and representative use cases. Keep implementation steps, configuration details, exhaustive capability lists, and API explanations in the examples and API sections instead of front-loading them in the introduction.

## Component Development Conventions

- Follow the API naming, lifecycle, state-management, and rendering patterns of the closest existing Blazorise components before introducing a new pattern.
- Keep component declarations in the established Blazorise order used by nearby components: events when applicable, members, constructors, methods, and properties. Within methods, place lifecycle overrides before class/style builders, invalidation overrides, event handlers, and private helpers; within properties, place overrides, internal or derived state, and builder-backed names before injected, parameter, and cascading properties.
- Resolve provider-generated class names and styles through `ClassBuilder` and `StyleBuilder` build callbacks. Do not call `ClassProvider` or `StyleProvider` directly from rendered class-name or style-name getters; initialize builders for secondary elements in the constructor, expose their `.Class` or `.Styles` result, and invalidate them from `DirtyClasses` or `DirtyStyles` as appropriate.
- When parameters synchronize derived state, prefer `SetParametersAsync` with `ParameterViewExtensions` for coordinated change detection, especially across several parameters. Use change-detecting parameter setters only for simple, independent local transitions where they are clearer, and resynchronize supplied mutable complex parameters unless value equality is guaranteed.
- When renaming a component parameter while retaining the old name as an obsolete compatibility alias, proxy the alias directly to the canonical parameter (for example, `get => Text; set => Text = value;`). Do not introduce a separate backing field or an `Effective{Name}` member solely for the alias. Supplying both parameter names in the same render is unsupported; do not define precedence between them.
- Maintain a single owner for component state. Descendants should consume parent state through cascading state rather than expose parameters that can create conflicting states.
- Route user interaction, public methods, parameter updates, and two-way binding through the same component lifecycle and event semantics.
- Always bind Razor events to named `On{Name}Handler` entry methods. Use `Handle{Name}` for overridable behavior invoked by those handlers. When argument conversion is needed, perform it in the entry method before calling the behavior method. Do not bind events directly to `Handle{Name}`, overload an entry-method name with different argument types, or resolve method-group ambiguity with lambdas or new forwarding members.
- Provider-specific component overrides must inherit the closest applicable shared implementation so common behavior is implemented once. Keep provider overrides focused on provider-specific markup, styling, and behavior. 
- Keep shared component options and service contracts focused on component behavior rather than browser implementation details. Translate them into JavaScript option models, CSS selectors, class names, and DOM properties within the browser interop layer. Do not expose those details through shared component contracts merely because the existing JavaScript implementation requires them.
- Add state fields only when their values cannot be derived from existing parameters, lifecycle, or collections. Keep one source of truth, and avoid parallel collections, cached values, and pending-render flags when simple checks or Blazor render coalescing are sufficient.
- For fixed cascading parents that own the child subtree, register children in `OnInitialized` and unregister during disposal; do not track previous parents unless the child can demonstrably survive a parent change.
- Keep backing fields only when the raw parameter value and its effective rendered value genuinely differ.
- Prefer Razor for component-local markup. Use shared protected `RenderFragment` builders only when common rendering logic requires provider-specific placement.
- Avoid additional abstractions and public APIs unless they are required to represent genuinely distinct state or behavior.
- Keep static provider styling in SCSS. Use `StyleProvider` only for styles derived from runtime component state.
- Keep each CSS provider's SCSS self-contained. Do not import mixins, functions, or styles from the core Blazorise project or another provider. When multiple providers need the same styling helper, copy it into each provider and maintain the local copies independently.
- Prefer provider-native tokens and variables. Introduce new CSS variables only when necessary and name them according to the provider's established convention.

## Testing Guidelines

- Unit tests: xUnit + bUnit (`Tests/Blazorise.Tests`). Match existing naming like `*ComponentTest.cs`.
- E2E: Playwright + NUnit (`Tests/Blazorise.E2E.Tests`). See `Tests/Blazorise.E2E.Tests/ReadMe.md` for codegen/debug tips and `.runsettings` for headless settings.
- Add only the minimum focused tests needed to cover the behavior; avoid redundant tests of the same rendering path.

## Commit & Pull Request Guidelines

- Branching: target `master` for new work; target `rel-X.Y` for maintenance. Use `dev-*` (features) and `rel-*` (release fixes) branch prefixes.
- Commit subjects typically follow `Area: short summary` and often reference PR/issue numbers (e.g., `DataGrid: sync selected rows (#6309)`).
- PRs: follow `.github/pull_request_template.md`, include “How Has This Been Tested?”, link issues (e.g., `Closes #123`), and add screenshots for UI/visual changes.

## Security & Configuration

- Report vulnerabilities via `SECURITY.md`; do not commit secrets or private keys.
- If working on platform-specific demos (e.g., MAUI/Tizen), check `workload-install.ps1` for workload setup.