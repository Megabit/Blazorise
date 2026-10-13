---
name: blazorise-provider-styling
description: Create, change, or review Blazorise CSS provider SCSS, native tokens, provider class names, styling hooks, or runtime theme generation. Use for provider implementation work, rather than general application UI styling.
---

# Blazorise provider styling

Follow the repository's [AGENTS.md](../../../AGENTS.md), including its execution policy and generated-CSS protection. Skill invocation does not authorize CSS generation or other commands.

## Locate the provider conventions

Read the affected provider's SCSS partial, `_variables.scss`, nearby selectors, and relevant class provider or theme generator. Resolve available native tokens and naming conventions from that provider before adding styling.

## SCSS structure

- In `*.scss` files, use native SCSS syntax to reduce repetition: group selectors that share declarations, nest related descendants, modifiers, states, and pseudo-selectors under their common parent with `&`, and use loops, maps, or mixins for repeated rule families.
- Prefer provider-native logical utilities and CSS logical properties for layouts that adapt to LTR and RTL reading direction, using `Start` and `End` terminology for the corresponding APIs. Preserve physical positioning when it is the intended behavior.
- Keep SCSS nesting logical and reasonably shallow, and preserve the compiled selector specificity, cascade order, and behavior; do not nest unrelated selectors merely because they appear in the same file.
- Define reusable SCSS maps and lists in the provider's `_variables.scss` file instead of declaring them inline in component or utility partials.
- Keep each CSS provider's SCSS self-contained. Do not import mixins, functions, or styles from the core Blazorise project or another provider. When multiple providers need the same styling helper, copy it into each provider and maintain the local copies independently.

## Tokens and runtime themes

- When styling provider components, always use the CSS provider's native CSS variables or design tokens whenever suitable tokens exist.
- Introduce new CSS variables only when the provider does not expose suitable native variables.
- For providers without native CSS variables but with SCSS variables, prefer the provider's SCSS variables for compiled defaults and override the relevant selectors and values in the provider theme generator for runtime Blazorise theming.
- Any new CSS variables must follow the CSS provider's established variable naming convention; do not invent a provider prefix or use a shared cross-provider naming convention.
- Keep static provider styling in SCSS. Use `StyleProvider` only for styles derived from runtime component state.

## Classes and semantic state

- Shared core and extension styling can use `b-*` classes, such as `b-tree-view`. Keep provider-owned selectors and runtime hooks in the provider's native naming convention and prefix.
- Provider-owned CSS classes must follow the current provider's native naming convention and prefix (for example, `ant-*` in AntDesign); do not introduce shared `b-*` class names for provider-specific selectors or runtime hooks.
- Use provider-prefixed classes for provider styling hooks. Reserve data attributes for semantic state, and serialize their values as lowercase kebab-case.

For component markup, class/style builders, lifecycle changes, or browser interop contracts, also use [blazorise-components](../blazorise-components/SKILL.md). For C# edits to class providers or theme generators, read the shared [C# formatting examples](../blazorise-components/references/csharp-formatting.md).

Review selector specificity, cascade order, compiled defaults, and runtime theme overrides against the existing implementation. Keep generated CSS untouched; the team generates it manually.