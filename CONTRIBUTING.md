## Open Development

All work on Blazorise happens directly on GitHub. All external contributors must send pull requests which will go through the review process.

## Setup Development Environment

To get the development and test environment set up on your local machine, you need to do the following:

1. Ensure recent build of Chrome browser installed: https://www.google.com/chrome/.
2. For E2E tests, install Playwright browser dependencies as described in the **Testing** section below.

You are now ready to build and test Blazorise:

## How to Write a Community Blog?

To write a community article, go to the dedicated repository: https://github.com/Blazorise/Blazorise.Articles.

All article authoring details, structure, and submission instructions are maintained there.

## Running the Documentation

1. Start a command prompt and navigate to the `\Documentation\Blazorise.Docs.Server` folder under Blazorise root.
2. Run command: `dotnet watch run`.
4. Wait for build to finish and your browser will automatically be opened

## Testing

Blazorise uses:

- Unit tests (xUnit + bUnit) in `Tests/Blazorise.Tests`
- E2E tests (Playwright + NUnit) in `Tests/Blazorise.E2E.Tests`

For Playwright E2E tests:

1. Build `Tests/Blazorise.E2E.Tests` so the Playwright script is generated.
2. From `Tests/Blazorise.E2E.Tests`, install Playwright dependencies:

```powershell
powershell .\bin\Debug\net10.0\playwright.ps1 install --with-deps
```

3. To record and generate test code with Playwright Inspector (target: **.NET C# NUnit**):

```powershell
powershell .\bin\Debug\net10.0\playwright.ps1 codegen http://localhost:14696
```

4. Run the `BasicTestApp.Client` demo app when implementing E2E tests, inherit new page tests from `BlazorisePageTest`, and navigate with `RootUri` (or use `SelectTestComponent` when applicable).
5. For debugging UI behavior, run tests in headed mode (for example `dotnet test -- Playwright.LaunchOptions.Headless=false`), or use `PWDEBUG=1` and `await Page.PauseAsync();` to inspect step-by-step.

For full and up-to-date Playwright guidance, see `Tests\Blazorise.E2E.Tests\ReadMe.md`.

## Branch Organization

We use `master` branch for all development and for all new features and bug fixes that are going into the ongoing milestone.

Once we finish the work for the current milestone, we will create a new **support** branch named `rel-X.Y`, where all the bug fixes will go. Also, we regularly publish our web from the latest `rel-X.Y` branch, so if you plan to write a blog, this is the recommended way and place to create it.

Branch naming must follow this guidelines:

- `dev-` prefix for new features based on `master` branch
- `rel-` prefix for fixed on existing release branch

## Pull requests

When submitting a pull request:

1. Fork the repository.
2. Create a branch from `master` or `rel-X.Y` and give it a meaningful name (e.g. `rel-{version-num}-my-awesome-new-feature`) and describe the feature or fix.  
4. Open a pull request on GitHub.

> :warning: **Don't make any changes on master branch**: You must always create a feature branch by following our guidelines or we will close the PR until the changes are properly organized.