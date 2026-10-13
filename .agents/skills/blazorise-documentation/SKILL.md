---
name: blazorise-documentation
description: Write, update, or review Blazorise documentation pages, component examples, handwritten snippets, and API explanations under Documentation. Use for documentation source work, rather than unrelated prose or generated documentation output.
---

# Blazorise documentation

Follow the repository's [AGENTS.md](../../../AGENTS.md), including its execution policy and generated-documentation protections. Skill invocation does not authorize documentation generation, builds, or other commands.

## Find the handwritten sources

Read the closest existing documentation page and its example sources. Follow its page composition, example presentation, API section, and snippet-helper usage.

Edit the relevant handwritten `.razor`, `.snippet`, or `.csharp` sources. When a snippet-helper change is needed, inspect `Documentation/Blazorise.Docs/Models/Snippets.cs`. The authoritative generated-output locations and the separate blog HTML workflow are documented in `AGENTS.md`.

## Write the page and examples

- In documentation page introductions, including `DocsPageLead` and opening paragraphs, focus on why the feature is useful, the problems it solves, and representative use cases. Keep implementation steps, configuration details, exhaustive capability lists, and API explanations in the examples and API sections instead of front-loading them in the introduction.

- Follow the repository's Razor component and utility-parameter conventions in examples.
- Read the shared [C# formatting examples](../blazorise-components/references/csharp-formatting.md) when editing C# samples or handwritten helpers.

For related library component implementation changes, also use [blazorise-components](../blazorise-components/SKILL.md); for provider implementation styling changes, use [blazorise-provider-styling](../blazorise-provider-styling/SKILL.md).

## Review the source changes

Check the examples and explanations against the component's current source API. Check that page references, snippet identifiers, and handwritten example paths resolve and agree with nearby documentation patterns. Apply the repository's generated-file and blog protections when reviewing the final diff; run documentation tooling only when explicitly authorized.