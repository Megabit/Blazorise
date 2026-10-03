# Documentation generation

Building `Blazorise.Docs` or `Blazorise.Docs.Server` runs the docs compiler before compiling the docs assembly. Newly generated example HTML is included as embedded resources in that same build, along with the generated copyable snippets.

Generated HTML, copyable snippet constants, API documentation, and search indexes are written beneath the consuming project's `obj/<configuration>/<framework>/DocsGenerated/` directory (including the runtime identifier when applicable). The docs assembly embeds examples from there. The MCP project sets `GenerateDocsSearchDataOnly=true` to generate just its two JSON indexes in its own intermediate output, skipping example HTML, snippet constants, and API documentation C# files. These generated files should not be committed.

Use `__BLAZORISE_VERSION__` for four-part CSS/JS cache query versions and `__BLAZORISE_PACKAGE_VERSION__` for the exact NuGet version in package references. Both come from `Build/Blazorise.Version.props` and are resolved for displayed examples, copied snippets, and search indexes.

IDE design-time builds do not run the compiler. Editing or opening a file does not regenerate documentation through those builds. Starting the server with a build refreshes the generated documentation; starting without a build uses the existing assembly.

Normal builds refresh example HTML only when its source has changed since the last successful markup generation or its generated HTML is missing. Versioned examples also refresh when the release version changes. The last run and current example list are tracked by `ExampleCodeFiles.txt` in the generated output directory.

To build and refresh changed examples from the repository root:

```powershell
dotnet build Documentation/Blazorise.Docs.Server/Blazorise.Docs.Server.csproj
```

To build using the existing generated documentation:

```powershell
dotnet build Documentation/Blazorise.Docs.Server/Blazorise.Docs.Server.csproj -p:GenerateDocs=false
```

Skipping generation requires the generated files to exist already. `GenerateDocs=false` also takes precedence over requests to regenerate all examples.

Changes to shared source or example-processing code do not refresh otherwise unchanged example HTML during normal builds. To explicitly regenerate all example HTML after those changes:

```powershell
dotnet build Documentation/Blazorise.Docs.Server/Blazorise.Docs.Server.csproj -p:RegenerateDocsExamples=true
```

When invoking the docs compiler directly, the equivalent option is `--regenerate-examples true`. Use `--output-path <directory>` to choose the output directory; otherwise, outputs go to `Documentation/Blazorise.Docs/obj/DocsGenerated/`. Pass `--search-data-only true` to generate only the search indexes. This mode still analyzes component sources for the API index, but does not render example HTML or generate C# files.

Processed HTML is still compared by content before writing. Unchanged HTML, snippet files, and API documentation C# files are not rewritten, preserving timestamps so they do not trigger unnecessary compilation. After successful API generation, obsolete API documentation C# files are removed. The JSON writers also preserve unchanged indexes, ignoring generation timestamps. The timestamp shortcut applies only to example HTML; snippet and index generation keep their existing behavior.