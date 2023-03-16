# Playwright E2E Test Project

This test project uses Playwright with NUnit.
NUnit was chosen since there is already an helpful playwright helper package for NUnit.

Below you will find instructions on how to use Playwright. This assumes you will be using the command line and that you will be positioned in the root of the E2E tests project.

## Install
- Build the project, either by using the dotnet build command or Visual Studio.
- Execute the playwright powershell script to install necessary dependencies (browser dependencies, etc...), i.e: `powershell .\bin\Debug\net7.0\playwright.ps1 install`

## Record / Implement new tests
To start a record session to generate c# test code (A browser session & Playwright Inspector should be opened automatically) :
- `powershell .\bin\Debug\net7.0\playwright.ps1 codegen http://localhost:14696`
- In the Playwright Inspector, please select .NET C# NUnit as the target library to generate the appropriate code.


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

To remove the headless mode, you can either:
- dotnet test -- Playwright.LaunchOptions.Headless=false
- Set Headless to false in the .runsettings file that's located in the solution root folder