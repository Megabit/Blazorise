namespace Blazorise.E2E.Tests.Tests.Components.Tabs;

[Parallelizable( ParallelScope.Self )]
[TestFixture]
public class TabsTests : BlazorisePageTest
{
    [Test]
    public async Task CanSelectTabs()
    {
        await SelectTestComponent<TabsComponent>();


        var sut = Page.Locator( "#basic-tabs" );
        var links = await sut.Locator( "a" ).AllAsync();

        var tabContent = sut.Locator( ".tab-content" );
        var panels = await tabContent.Locator( "div" ).AllAsync();

        Assert.IsNotEmpty( links );
        Assert.IsNotEmpty( panels );

        await DoNotExpectShowClass( links[0] );
        await ExpectShowClass( links[1] );
        await DoNotExpectShowClass( links[2] );


        await DoNotExpectShowClass( panels[0] );
        await ExpectShowClass( panels[1] );
        await DoNotExpectShowClass( panels[2] );

        await links[0].ClickAsync();
        await ExpectShowClass( links[0] );
        await DoNotExpectShowClass( links[1] );
        await DoNotExpectShowClass( links[2] );


        await ExpectShowClass( panels[0] );
        await DoNotExpectShowClass( panels[1] );
        await DoNotExpectShowClass( panels[2] );

        await links[2].ClickAsync();
        await DoNotExpectShowClass( links[0] );
        await DoNotExpectShowClass( links[1] );
        await ExpectShowClass( links[2] );


        await DoNotExpectShowClass( panels[0] );
        await DoNotExpectShowClass( panels[1] );
        await ExpectShowClass( panels[2] );

    }

    private async Task ExpectShowClass( ILocator locator )
    {
        await Expect( locator ).ToHaveClassAsync( expected: new Regex( "show" ) );
    }

    private async Task DoNotExpectShowClass( ILocator locator )
    {
        await Expect( locator ).Not.ToHaveClassAsync( expected: new Regex( "show" ) );
    }

}