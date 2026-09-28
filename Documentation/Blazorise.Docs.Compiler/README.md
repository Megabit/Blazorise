# Documentation generation

Building `Blazorise.Docs` or `Blazorise.Docs.Server` runs the docs compiler before compiling the docs assembly. Newly generated example HTML is included as embedded resources in that same build, along with the generated copyable snippets.

IDE design-time builds do not run the compiler. Editing or opening a file does not regenerate documentation through those builds. Starting the server with a build refreshes the generated documentation; starting without a build uses the existing assembly.

Normal builds refresh example HTML only when its source has changed since the last successful markup generation or its generated HTML is missing. The last run is tracked by `NewFilesToBuild.txt`.

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

When invoking the docs compiler directly, the equivalent option is `--regenerate-examples true`.

Processed HTML is still compared by content before writing. Unchanged HTML and snippet files are not rewritten. The JSON writers also preserve unchanged indexes, ignoring generation timestamps. The timestamp shortcut applies only to example HTML; snippet and index generation keep their existing behavior.