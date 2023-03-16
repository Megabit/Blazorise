using Blazorise.E2E.Tests.Infrastructure;

namespace Blazorise.E2E.Tests.Tests.Components.TextEdit
{

    [Parallelizable( ParallelScope.Self )]
    [TestFixture]
    public class AsyncValidateTextEditTests : BlazorPageTest
    {
        [Test]
        public async Task CanValidateText_InitiallyBlank()
        {
            await Page.GotoAsync( RootUri.AbsoluteUri );
            await Page.PauseAsync();
            await Page.GetByRole( AriaRole.Combobox ).SelectOptionAsync( new[] { "BasicTestApp.Client.AsyncValidateTextEditComponent" } );

            var sut = Page.Locator( "#validate-text-initially-blank" );
            var textBox = sut.GetByRole( AriaRole.Textbox );
            var validationFeedback = sut.GetByText( "error" );

            await textBox.ClickAsync();
            await Expect( validationFeedback ).ToHaveClassAsync( new Regex( "invalid-feedback" ) );

            await textBox.FillAsync( "a" );
            await Expect( validationFeedback ).ToBeHiddenAsync();

            await textBox.FillAsync( "" );
            await Expect( validationFeedback ).ToHaveClassAsync( "invalid-feedback" );
        }

    }
}
