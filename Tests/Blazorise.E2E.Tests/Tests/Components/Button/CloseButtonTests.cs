using Blazorise.E2E.Tests.Infrastructure;

namespace Blazorise.E2E.Tests.Tests.Components.Button
{

    [Parallelizable( ParallelScope.Self )]
    [TestFixture]
    public class CloseButtonTests : BlazorPageTest
    {
        [Test]
        public async Task CanRaiseCallback()
        {
            await Page.GotoAsync( RootUri.AbsoluteUri );

            await Page.GetByRole( AriaRole.Combobox ).SelectOptionAsync( new[] { "BasicTestApp.Client.CloseButtonComponent" } );

            var closeButtonResult = Page.Locator( "#close-button-event-result" );
            await Page.GetByText( "× Count" ).ClickAsync();
            await Expect( closeButtonResult ).ToHaveTextAsync( "1" );

            await Page.GetByText( "× Count" ).ClickAsync();
            await Expect( closeButtonResult ).ToHaveTextAsync( "2" );

            await Page.GetByText( "× Count" ).ClickAsync();
            await Expect( closeButtonResult ).ToHaveTextAsync( "3" );
        }

        [Test]
        public async Task CanAutoClose()
        {
            await Page.GotoAsync( RootUri.AbsoluteUri );

            await Page.GetByRole( AriaRole.Combobox ).SelectOptionAsync( new[] { "BasicTestApp.Client.CloseButtonComponent" } );

            var autoCloseButtonResult = Page.Locator( "#autoclose-button-event-result" );
            await Page.Locator( "#autoclose-button" ).ClickAsync();
            await Expect( autoCloseButtonResult ).ToHaveTextAsync( "1" );
        }
    }

}
