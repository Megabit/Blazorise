# Documentation generation

Building `Blazorise.Docs` or `Blazorise.Docs.Server` runs the docs compiler before compiling the docs assembly. Newly generated example HTML is included as embedded resources in that same build, along with the generated copyable snippets.

The compiler is a build dependency restored with the consuming project and built through MSBuild. Docs and MCP can share that compiler build within a solution build. Generation executes the resulting assembly directly, without starting another restore or CLI build. The compiler copies XML documentation beside the exact ASP.NET Core and System.Runtime assembly references selected by the SDK.

Generated HTML, copyable snippet constants, API documentation, and search indexes are written beneath the consuming project's `obj/<configuration>/<framework>/DocsGenerated/` directory (including the runtime identifier when applicable). The docs assembly embeds examples from there. The MCP project sets `GenerateDocsSearchDataOnly=true` to generate just its two JSON indexes in its own intermediate output, skipping example HTML, snippet constants, and API documentation C# files. These generated files should not be committed.

Use `__BLAZORISE_VERSION__` for four-part CSS/JS cache query versions and `__BLAZORISE_PACKAGE_VERSION__` for the exact NuGet version in package references. Both come from `Build/Blazorise.Version.props` and are resolved for displayed examples, copied snippets, and search indexes.

IDE design-time builds do not run the compiler. Editing or opening a file does not regenerate documentation through those builds. Starting the server with a build refreshes the generated documentation; starting without a build uses the existing assembly.

The compiler also writes `Models/DocsPageChanges.generated.cs` under `DocsGenerated`, and the docs project compiles it alongside the snippets. It contains a simple list of `(Route, Version, Date, IsNew)` entries read from `AddedVersion` and `AddedDate` on page titles and section headers. Page titles produce New entries; section headers produce Updated entries for their page. Route aliases share the same changes. Values must be literal `major.minor` versions and `yyyy-MM-dd` dates.

The docs layout reads this list directly using the configured NuGet package version from `Build/Blazorise.Version.props`. New badges cover the current and preceding minor release within the same major version; Updated badges cover only the current minor release. Version checks compare numeric major/minor values, ignoring patch and revision numbers and prerelease/build suffixes: current versions 2.4.1 and 2.4.1 accept metadata versions 2.3 and 2.4 for New, but only 2.4 for Updated. Page versions determine New badges. Updated badges check both page and section versions, including sections whose page title has no version metadata. For example, Button's 2.4 section qualifies its sidebar link as Updated when the current package version is 2.4.0; its 2.3 section does not. New takes precedence when both badges qualify. Top-level dropdowns have no badges. Nested groups show Updated for matching page or section changes in the current minor release unless their own page is New. The layout selects the existing New and Updated badge components and preserves their original flex-row markup; entries without a matching change render as plain text. Add metadata to the page or section, not the sidebar; the next docs build refreshes the list. Search-data-only generation continues to produce only its two JSON indexes.

Examples, API documentation, and search content have separate content-based caches under `DocsGenerated/.cache/`. An unchanged stage is skipped when its inputs and outputs match the last successful generation. Source additions, removals, compiler or dependency changes, and missing or altered outputs invalidate the relevant cache. Changes to the release version also invalidate caches through the versioned compiler assembly. API scanning excludes icon projects and `bin`, `obj`, and `__SOURCEGENERATED__` directories.

Example HTML also has a cache per example. Its fingerprint includes the composed example source, including shared support types, resolved version placeholders, and the compiler dependencies. A changed timestamp alone does not trigger rendering, and a content change is detected even when its timestamp is unchanged. `ExampleCodeFiles.txt` lists the generated examples to embed; it is not used as a timestamp shortcut.

To build and refresh changed examples from the repository root:

```powershell
dotnet build Documentation/Blazorise.Docs.Server/Blazorise.Docs.Server.csproj
```

To build using the existing generated documentation:

```powershell
dotnet build Documentation/Blazorise.Docs.Server/Blazorise.Docs.Server.csproj -p:GenerateDocs=false
```

Skipping generation requires the generated files to exist already. `GenerateDocs=false` also takes precedence over requests to regenerate all examples.

Shared source and example-processing changes are detected automatically. To explicitly render all example HTML regardless of its cache:

```powershell
dotnet build Documentation/Blazorise.Docs.Server/Blazorise.Docs.Server.csproj -p:RegenerateDocsExamples=true
```

When invoking the docs compiler directly, the equivalent option is `--regenerate-examples true`. Use `--output-path <directory>` to choose the output directory; otherwise, outputs go to `Documentation/Blazorise.Docs/obj/DocsGenerated/`. Pass `--search-data-only true` to generate only the search indexes. This mode still analyzes component sources for the API index, but does not render example HTML or generate C# files.

Processed HTML is still compared by content before writing. Unchanged HTML, snippet files, and API documentation C# files are not rewritten, preserving timestamps so they do not trigger unnecessary compilation. After successful API generation, obsolete API documentation C# files are removed. The JSON writers also preserve unchanged indexes, ignoring generation timestamps. Cache files are intermediate build artifacts and should not be committed.