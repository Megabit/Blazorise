# Contributing to Blazorise

Contributions to Blazorise are reviewed through GitHub pull requests. You can help with bug fixes, features, documentation, and examples. Follow our [Code of Conduct](CODE_OF_CONDUCT.md), and report security vulnerabilities privately using the [security policy](SECURITY.md).

## Getting started

1. Fork the repository and clone your fork.
2. Install the .NET SDK used by [CI](workflows/blazorise-ci-basic.yml), currently `11.0.100-rc.2.26504.105`. An editor or IDE must support this SDK to build the solution.
3. Create a working branch from the appropriate target branch:

| Change | PR target | Working branch example |
| --- | --- | --- |
| New features and fixes for ongoing development | `master` | `dev-datagrid-keyboard-navigation` |
| Maintenance fixes for an existing release | The applicable `rel-X.Y` branch | `rel-X.Y-datagrid-selection-fix` |

Make changes on your working branch, then submit the PR to its corresponding target branch.

Run the following commands from the repository root to build the solution and start the Bootstrap 5 demo:

```powershell
dotnet build Blazorise.slnx -c DebugNet11
dotnet run --project Demos/Blazorise.Demo.Bootstrap5/Blazorise.Demo.Bootstrap5.csproj -c DebugNet11
```

`DebugNet11` builds libraries, providers, and extensions for `net11.0` only, with debugging symbols and hot reload support. Generators, analyzers, and weavers retain their existing target frameworks. Standard `Debug` and `Release` configurations build the libraries for .NET 8 through .NET 11. The development framework is defined in [Directory.Build.props](../Directory.Build.props).

Open the local URL shown by the application.

## Repository layout

| Location | Contents |
| --- | --- |
| [Source/Blazorise](../Source/Blazorise) | Core components and shared library infrastructure |
| `Source/Blazorise.*` | CSS providers |
| [Source/Extensions](../Source/Extensions) | Extension components |
| [Source/Shared](../Source/Shared) | Sample data and models shared by demos, documentation, and tests |
| [Demos](../Demos) | Sample applications for the supported providers |
| [Tests](../Tests) | Unit tests, E2E tests, and test applications |
| [Documentation](../Documentation) | Documentation pages, examples, tooling, and the docs server |
| [Build](../Build) | Shared MSBuild configuration |

## Making changes

Keep each PR focused on one problem or feature. For a substantial change, discuss the proposed behavior in an issue before investing in the implementation.

Follow [.editorconfig](../.editorconfig), the coding conventions in [AGENTS.md](../AGENTS.md), and the surrounding code. In particular:

- Add focused tests for changed behavior, and update documentation or examples when the public API or usage changes.
- For changes that affect multiple CSS providers, check the affected providers and describe what you verified.
- Edit provider `.scss` sources; do not manually edit generated `.css` files. The team generates the CSS.
- Edit handwritten documentation `.razor`, `.snippet`, and `.csharp` sources. Generated examples, snippets, API documentation, and search indexes belong under `obj/.../DocsGenerated/`; do not edit them or recreate generated files in the source tree. Blog HTML follows a separate workflow.
- Manage dependency versions in [Directory.Packages.props](../Directory.Packages.props).

### Running the documentation

From the repository root:

```powershell
dotnet watch --project Documentation/Blazorise.Docs.Server/Blazorise.Docs.Server.csproj run --configuration DebugNet11
```

Open the local URL shown by the server and check the pages and examples affected by your change.

## Testing

Run the tests relevant to your change and include the commands and results in your PR. The following commands run from the repository root.

### Unit tests

Component unit tests use xUnit and bUnit:

```powershell
dotnet test Tests/Blazorise.Tests/Blazorise.Tests.csproj -c Release
```

For analyzer changes, run the tests in [Tests/Blazorise.Analyzers.Tests](../Tests/Blazorise.Analyzers.Tests). Add only the focused coverage needed to verify the behavior.

### E2E tests

Browser tests use Playwright and NUnit. Build the test project first so the browser installation script is available, then install its dependencies and run the tests. The installation command requires PowerShell (`pwsh`).

```powershell
dotnet build Tests/Blazorise.E2E.Tests/Blazorise.E2E.Tests.csproj -c Release
pwsh ./Tests/Blazorise.E2E.Tests/bin/Release/net11.0/playwright.ps1 install --with-deps
dotnet test Tests/Blazorise.E2E.Tests/Blazorise.E2E.Tests.csproj -c Release --no-build
```

See the [E2E guide](../Tests/Blazorise.E2E.Tests/ReadMe.md) for recording tests, using the test application, and debugging browser interactions. The [CI workflow](workflows/blazorise-ci-basic.yml) contains the full restore, build, and test sequence.

## Submitting a PR

Push your working branch to your fork and open a pull request against the target branch selected above. Complete the [PR template](pull_request_template.md) and include:

- The problem being solved and the resulting behavior, with a linked issue when applicable, such as `Closes #123`.
- The tests and checks you ran, their results, and any checks you could not run.
- Relevant documentation or example updates.
- Screenshots for visual changes.
- Any public API changes or compatibility considerations reviewers should assess.

Use descriptive commit subjects, such as `DataGrid: fix keyboard selection`.

Contributors must agree to the [Contributor License Agreement](https://gist.github.com/stsrki/abfa5ce0f4a5cf1e6ac67b92f8eb5d63). The CLA Assistant provides signing instructions on your pull request. Respond to review feedback and keep follow-up changes focused on the PR's purpose.

## How to write a community blog

Submit community articles to the dedicated [Blazorise.Articles repository](https://github.com/Blazorise/Blazorise.Articles). It contains the authoring, structure, and submission instructions.