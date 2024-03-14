namespace Blazorise.E2E.Tests.Tests.Components.TextEdit;


[Parallelizable( ParallelScope.Self )]
[TestFixture]
public class AsyncValidateTextEditTests : BlazorisePageTest
{
    [Test]
    public async Task CanValidateText_InitiallyBlank()
    {
        await SelectTestComponent<AsyncValidateTextEditComponent>();

        var sut = Page.Locator( "#validate-text-initially-blank" );
        var textBox = sut.GetByRole( AriaRole.Textbox );
        var validationFeedback = sut.GetByText( "error" );

        await ExpectTo_StartInvalid_ValidUponFill_InvalidUponClear( textBox, validationFeedback );
    }



    [Test]
    public async Task CanValidateText_InitiallyPopulated()
    {
        await SelectTestComponent<AsyncValidateTextEditComponent>();

        var sut = Page.Locator( "#validate-text-initially-populated" );
        var textBox = sut.GetByRole( AriaRole.Textbox );
        var validationFeedback = sut.GetByText( "error" );

        await ExpectTo_StartValid_InvalidUponClear_ValidUponFill( textBox, validationFeedback );
    }



    [Test]
    public async Task CanValidateTextWithBind_InitiallyBlank()
    {
        await SelectTestComponent<AsyncValidateTextEditComponent>();

        var sut = Page.Locator( "#validate-text-with-bind-initially-blank" );
        var textBox = sut.GetByRole( AriaRole.Textbox );
        var validationFeedback = sut.GetByText( "error" );

        await ExpectTo_StartInvalid_ValidUponFill_InvalidUponClear( textBox, validationFeedback );
    }

    [Test]
    public async Task CanValidateTextWithBind_InitiallyPopulated()
    {

        await SelectTestComponent<AsyncValidateTextEditComponent>();

        var sut = Page.Locator( "#validate-text-with-bind-initially-populated" );
        var textBox = sut.GetByRole( AriaRole.Textbox );
        var validationFeedback = sut.GetByText( "error" );

        await ExpectTo_StartValid_InvalidUponClear_ValidUponFill( textBox, validationFeedback );
    }

    [Test]
    public async Task CanValidateTextWithEvent_InitiallyBlank()
    {
        await SelectTestComponent<AsyncValidateTextEditComponent>();

        var sut = Page.Locator( "#validate-text-with-event-initially-blank" );
        var textBox = sut.GetByRole( AriaRole.Textbox );
        var validationFeedback = sut.GetByText( "error" );

        await ExpectTo_StartInvalid_ValidUponFill_InvalidUponClear( textBox, validationFeedback );
    }

    [Test]
    public async Task CanValidateTextWithEvent_InitiallyPopulated()
    {
        await SelectTestComponent<AsyncValidateTextEditComponent>();

        var sut = Page.Locator( "#validate-text-with-event-initially-populated" );
        var textBox = sut.GetByRole( AriaRole.Textbox );
        var validationFeedback = sut.GetByText( "error" );

        await ExpectTo_StartValid_InvalidUponClear_ValidUponFill( textBox, validationFeedback );
    }

    private async Task ExpectTo_StartInvalid_ValidUponFill_InvalidUponClear( ILocator textBox, ILocator validationFeedback )
    {
        await Expect( validationFeedback ).ToHaveClassAsync( "invalid-feedback" );

        await textBox.FillAsync( "some text" );
        await Expect( validationFeedback ).ToBeHiddenAsync();

        await textBox.FillAsync( "" );
        await Expect( validationFeedback ).ToHaveClassAsync( "invalid-feedback" );
    }

    private async Task ExpectTo_StartValid_InvalidUponClear_ValidUponFill( ILocator textBox, ILocator validationFeedback )
    {
        await Expect( validationFeedback ).ToBeHiddenAsync();

        await textBox.FillAsync( "" );
        await Expect( validationFeedback ).ToHaveClassAsync( "invalid-feedback" );

        await textBox.FillAsync( "some text" );
        await Expect( validationFeedback ).ToBeHiddenAsync();
    }

}
