namespace Blazorise.E2E.Tests;


[Parallelizable( ParallelScope.Self )]
[TestFixture]
public class DefaultExampleTests : PageTest
{
    /// <summary>
    /// This test serves as simple example of how to use Playwright. This was taken off playwright .NET docs.
    /// </summary>
    /// <returns></returns>
    //[Test]
    public async Task HomepageHasPlaywrightInTitleAndGetStartedLinkLinkingtoTheIntroPage()
    {
        // Pause on the following line.
        await Page.PauseAsync();

        await Page.GotoAsync( "https://playwright.dev" );

        // Expect a title "to contain" a substring.
        await Expect( Page ).ToHaveTitleAsync( new Regex( "Playwright" ) );

        // create a locator
        var getStarted = Page.GetByRole( AriaRole.Link, new() { Name = "Get started" } );

        // Expect an attribute "to be strictly equal" to the value.
        await Expect( getStarted ).ToHaveAttributeAsync( "href", "/docs/intro" );

        // Click the get started link.
        await getStarted.ClickAsync();

        // Expects the URL to contain intro.
        await Expect( Page ).ToHaveURLAsync( new Regex( ".*intro" ) );
    }
}

[Parallelizable( ParallelScope.Self )]
[TestFixture]
public class CopyMeTests : BlazorisePageTest
{
    [Test]
    public async Task CopyMe()
    {
        await SelectTestComponent<ButtonComponent>();
    }
}