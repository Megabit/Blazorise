namespace Blazorise.E2E.Tests.Tests.Components._;

[Parallelizable( ParallelScope.Self )]
[TestFixture]
public class ComponentRenderingTest : BlazorisePageTest
{
    [Test]
    public async Task BasicTestAppCanBeServed()
    {
        await Page.GotoAsync( RootUri.AbsoluteUri );

        await Expect( Page ).ToHaveTitleAsync( "Blazorise test app" );
    }

    [Test]
    public async Task CanRenderTextOnlyComponent()
    {
        await SelectTestComponent<TextOnlyComponent>();
        await Expect( Page.GetByText( "Hello from TextOnlyComponent" ) ).ToBeVisibleAsync();
    }

    [Test]
    public async Task CanRenderButtonComponent()
    {
        await SelectTestComponent<ButtonOnlyComponent>();
        var button = Page.GetByRole( AriaRole.Button );
        await Expect( button ).ToHaveTextAsync( "hello primary" );
        await Expect( button ).ToHaveClassAsync( "btn btn-primary" );
    }

    [Test]
    public async Task CannotChangeElementId()
    {
        await SelectTestComponent<ElementIdComponent>();
        var date = Page.GetByRole( AriaRole.Textbox );
        var button = Page.GetByRole( AriaRole.Button );

        var idBefore = await date.GetAttributeAsync( "id" );

        Assert.AreNotEqual( string.Empty, idBefore );

        await button.ClickAsync();

        Assert.AreEqual( idBefore, await date.GetAttributeAsync( "id" ) );
    }
}
