using Blazorise.E2E.Tests.Infrastructure;

namespace Blazorise.E2E.Tests.Tests.Components.Button
{

    [Parallelizable( ParallelScope.Self )]
    [TestFixture]
    public class ButtonTests : BlazorPageTest
    {
        [Test]
        public async Task CanRaiseCallback()
        {
            await Page.GotoAsync( RootUri.AbsoluteUri );

            await Page.GetByRole( AriaRole.Combobox ).SelectOptionAsync( new[] { "BasicTestApp.Client.ButtonComponent" } );

            var buttonResult = Page.Locator( "#basic-button-event-result" );
            await Page.GetByRole( AriaRole.Button, new() { Name = "Count" } ).ClickAsync();
            await Expect( buttonResult ).ToHaveTextAsync( "1" );


            await Page.GetByRole( AriaRole.Button, new() { Name = "Count" } ).ClickAsync();
            await Expect( buttonResult ).ToHaveTextAsync( "2" );


            await Page.GetByRole( AriaRole.Button, new() { Name = "Count" } ).ClickAsync();
            await Expect( buttonResult ).ToHaveTextAsync( "3" );
        }
    }

}
