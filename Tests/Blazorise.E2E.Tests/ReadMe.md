# Playwright E2E Test Project

This test project uses Playwright with NUnit.
NUnit was chosen since there is already an helpful playwright helper package for NUnit.

# Install
- Build the project
- Execute the playwright powershell script to install necessary dependencies (browser dependencies), i.e: `powershell .\bin\Debug\net7.0\playwright.ps1 install`

# Record / Implement new tests
- To start a record session to generate c# test code (A browser session & Playwright Inspector should be opened) : `powershell .\bin\Debug\net7.0\playwright.ps1 install`

# Debugging

## Ways to disable headless mode

https://playwright.dev/dotnet/docs/debug#headed-mode

Pause the execution with `await page.PauseAsync();` and then use the Playwright Inspector to debug the test.
- dotnet test -- Playwright.LaunchOptions.Headless=false
- Set Headless to false in the csproject
