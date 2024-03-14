namespace Blazorise.E2E.Tests.Tests.Components.Button;

[Parallelizable( ParallelScope.Self )]
[TestFixture]
public class ButtonTests : BlazorisePageTest
{
    [Test]
    public async Task CanRaiseCallback()
    {
        await SelectTestComponent<ButtonComponent>();

        var buttonResult = Page.Locator( "#basic-button-event-result" );
        await Page.GetByRole( AriaRole.Button, new() { Name = "Count" } ).ClickAsync();
        await Expect( buttonResult ).ToHaveTextAsync( "1" );


        await Page.GetByRole( AriaRole.Button, new() { Name = "Count" } ).ClickAsync();
        await Expect( buttonResult ).ToHaveTextAsync( "2" );


        await Page.GetByRole( AriaRole.Button, new() { Name = "Count" } ).ClickAsync();
        await Expect( buttonResult ).ToHaveTextAsync( "3" );
    }
}

