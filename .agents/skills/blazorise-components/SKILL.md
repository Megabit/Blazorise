---
name: blazorise-components
description: Create, change, or review Blazorise library components and their state, parameters, events, rendering, provider overrides, or browser interop contracts. Use for library implementation work in Source, rather than ordinary application markup that consumes Blazorise.
---

# Blazorise components

Follow the repository's [AGENTS.md](../../../AGENTS.md), including its execution policy. Skill invocation does not authorize builds, tests, restore, or other commands.

## Find the existing pattern

Identify the closest existing shared component and, when relevant, its provider overrides. Read their Razor markup, lifecycle methods, class/style builders, and related state or interop types before choosing the implementation. Follow these patterns unless the task requires a distinct behavior.

## Structure and rendering

- Follow the API naming, lifecycle, state-management, and rendering patterns of the closest existing Blazorise components before introducing a new pattern.
- Keep component declarations in the established Blazorise order used by nearby components: events when applicable, members, constructors, methods, and properties. Within methods, place lifecycle overrides before class/style builders, invalidation overrides, event handlers, and private helpers; within properties, place overrides, internal or derived state, and builder-backed names before injected, parameter, and cascading properties.
- Resolve provider-generated class names and styles through `ClassBuilder` and `StyleBuilder` build callbacks. Do not call `ClassProvider` or `StyleProvider` directly from rendered class-name or style-name getters; initialize builders for secondary elements in the constructor, expose their `.Class` or `.Styles` result, and invalidate them from `DirtyClasses` or `DirtyStyles` as appropriate.
- Provider-specific component overrides must inherit the closest applicable shared implementation so common behavior is implemented once. Keep provider overrides focused on provider-specific markup, styling, and behavior.
- For components with Razor markup, keep markup in `ComponentName.razor` and parameters, members, and component logic in `ComponentName.razor.cs`. Preserve established `.cs` files for shared base implementations.
- Prefer Razor for component-local markup. Use shared protected `RenderFragment` builders only when common rendering logic requires provider-specific placement.

## Parameters and state

- Prefer prefixless names for simple boolean component parameters, such as `Loading` or `Disabled`. Name computed booleans by intent using the `Is`, `Has`, `Can`, and `Should` conventions in `AGENTS.md`.
- Use the `*Template` suffix for `RenderFragment` parameters that represent customizable slots, such as `ItemTemplate`, `LoadingTemplate`, or `HeaderTemplate`. Use `ChildContent` for ordinary child markup and preserve established names for other fragment roles.
- Prefer `Start` and `End` for layout-related APIs that adapt to LTR and RTL reading direction. Use physical side names when the physical side is part of the API's intended meaning.
- When parameters synchronize derived state, prefer `SetParametersAsync` with `ParameterViewExtensions` for coordinated change detection, especially across several parameters. Use change-detecting parameter setters only for simple, independent local transitions where they are clearer, and resynchronize supplied mutable complex parameters unless value equality is guaranteed.
- Preserve public API compatibility. Describe necessary breaking changes in an issue or discussion, deprecate replaced APIs with `[Obsolete]` and a reason, and plan obsolete API removal for the next major release. Introduce breaking API changes only in major version increments.
- When renaming a component parameter while retaining the old name as an obsolete compatibility alias, proxy the alias directly to the canonical parameter (for example, `get => Text; set => Text = value;`). Do not introduce a separate backing field or an `Effective{Name}` member solely for the alias. Supplying both parameter names in the same render is unsupported; do not define precedence between them.
- Maintain a single owner for component state. Descendants should consume parent state through cascading state rather than expose parameters that can create conflicting states.
- Add state fields only when their values cannot be derived from existing parameters, lifecycle, or collections. Keep one source of truth, and avoid parallel collections, cached values, and pending-render flags when simple checks or Blazor render coalescing are sufficient.
- For fixed cascading parents that own the child subtree, register children in `OnInitialized` and unregister during disposal; do not track previous parents unless the child can demonstrably survive a parent change.
- Keep backing fields only when the raw parameter value and its effective rendered value genuinely differ.
- Avoid additional abstractions and public APIs unless they are required to represent genuinely distinct state or behavior.

## Interaction and interop

- Route user interaction, public methods, parameter updates, and two-way binding through the same component lifecycle and event semantics.
- Always bind Razor events to named `On{Name}Handler` entry methods. Use `Handle{Name}` for overridable behavior invoked by those handlers. When argument conversion is needed, perform it in the entry method before calling the behavior method. Do not bind events directly to `Handle{Name}`, overload an entry-method name with different argument types, or resolve method-group ambiguity with lambdas or new forwarding members.
- Keep shared component options and service contracts focused on component behavior rather than browser implementation details. Translate them into JavaScript option models, CSS selectors, class names, and DOM properties within the browser interop layer. Do not expose those details through shared component contracts merely because the existing JavaScript implementation requires them.

## Apply related guidance

- For C# formatting decisions, read [C# formatting examples](references/csharp-formatting.md). The core coding and naming rules remain in `AGENTS.md`.
- If the change includes provider SCSS, design tokens, styling hooks, or runtime theming, also use `blazorise-provider-styling` at [../blazorise-provider-styling/SKILL.md](../blazorise-provider-styling/SKILL.md).
- If the requested work includes documentation pages or examples, use `blazorise-documentation` at [../blazorise-documentation/SKILL.md](../blazorise-documentation/SKILL.md).

Review the affected parameter, interaction, binding, and disposal paths statically. Add only focused tests when needed by the behavior, following the repository's testing guidance; execute verification commands only when explicitly authorized.