# E2E tests with Playwright

This test project uses Playwright with NUnit.

The NUnit is chosen since there is already a helpful playwright helper package for NUnit, and parallelism is better supported.

The API Docs are good; recommended pages for implementing tests:

- https://playwright.dev/dotnet/docs/input
- https://playwright.dev/dotnet/docs/test-assertions

Below you will find instructions on how to use Playwright. This assumes you will be using the command line and that you will be positioned in the root of the E2E tests project.

## Install

1. Build the project, either by using the `dotnet build` command or `Ctrl+Shift+B` in Visual Studio.
2. Execute the playwright powershell script to install necessary dependencies (browser dependencies, etc...), i.e:

```bash
powershell .\bin\Debug\net10.0\playwright.ps1 install --with-deps
```

> The script must be run in the folder `./Tests/Blazorise.E2E.Tests/`

## Record / Implement new tests

To start a record session to generate c# test code (a browser session & Playwright Inspector should be opened automatically):

```bash
powershell .\bin\Debug\net10.0\playwright.ps1 codegen http://localhost:14696
```

In the **Playwright Inspector**, please select **.NET C# NUnit** as the target library to generate the appropriate code.

Please note that:

- the testing demo is **BasicTestApp.Client** and you should run it in order to interact with the test application and generate tests.
- you should make it so your new PageTest inherits from **BlazorisePageTest** as that setups the test application, and provides helpers.
- in your test, you should navigate by using the provided **RootUri**, `await Page.GotoAsync( RootUri.AbsoluteUri );`
  - In most tests you can just use the `SelectTestComponent` helper, which will automatically navigate to the strongly typed test page.


## Debugging

You can just debug the test as you normally would by using the debugger in Visual Studio. 
You can also disable headless mode so you can visually see the steps the test is taking on the test application.

You can also use the Playwright Inspector to debug the test it will come up if you set `await Page.PauseAsync();` and are running in headed mode.

https://playwright.dev/dotnet/docs/debug#headed-mode

### Ways to disable headless mode

To remove the headless mode, you can run any of the following:

- `dotnet test -- Playwright.LaunchOptions.Headless=false`
- `dotnet test --filter "MyTest" -- Playwright.LaunchOptions.Headless=false` (run a single test)
- Set Headless to false in the .runsettings file that's located in the solution root folder
- `set HEADED=1
dotnet test`

- `set PWDEBUG=1
dotnet test`

### Tracing will gather screenshots and other useful information about your test run.

This will work even in headless mode.

Insert at the beggining of the test:

```cs
// Start tracing before creating / navigating a page.
await Context.Tracing.StartAsync( new()
{
    Screenshots = true,
    Snapshots = true,
    Sources = true
} );
```

Insert at the end of the test: (You might want to wrap the test in a try/catch if the test is failing/throwing)

```cs
// Stop tracing and export it into a zip archive.
await Context.Tracing.StopAsync( new()
{
    Path = "trace.zip"
} );
```

You can then upload the zip file to https://trace.playwright.dev/ and see the screenshots and other relevant information.