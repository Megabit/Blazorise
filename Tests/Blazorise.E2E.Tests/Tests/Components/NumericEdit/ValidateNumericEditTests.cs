namespace Blazorise.E2E.Tests.Tests.Components.NumericEdit;


[Parallelizable( ParallelScope.Self )]
[TestFixture]
public class ValidateNumericEditTests : BlazorisePageTest
{
    [Test]
    public async Task CanValidateNumeric_InitiallyBlank()
    {
        await SelectTestComponent<ValidateNumericEditComponent>();

        var sut = Page.Locator( "#validate-numeric-initially-blank" );
        var textBox = sut.Locator( "input[type=number]" );
        var validationFeedback = sut.GetByText( "error" );

        await ExpectTo_StartInvalid_ValidUponFill_InvalidUponClear( textBox, validationFeedback );
    }

    [Test]
    public async Task CanValidateNumeric_InitiallyPopulated()
    {
        await SelectTestComponent<ValidateNumericEditComponent>();

        var sut = Page.Locator( "#validate-numeric-initially-populated" );
        var textBox = sut.Locator( "input[type=number]" );
        var validationFeedback = sut.GetByText( "error" );

        await ExpectTo_StartValid_InvalidUponClear_ValidUponFill( textBox, validationFeedback );
    }

    [Test]
    public async Task CanValidateNumericWithBind_InitiallyBlank()
    {
        await SelectTestComponent<ValidateNumericEditComponent>();

        var sut = Page.Locator( "#validate-numeric-with-bind-initially-blank" );
        var textBox = sut.Locator( "input[type=number]" );
        var validationFeedback = sut.GetByText( "error" );

        await ExpectTo_StartInvalid_ValidUponFill_InvalidUponClear( textBox, validationFeedback );
    }

    [Test]
    public async Task CanValidateNumericWithBind_InitiallyPopulated()
    {

        await SelectTestComponent<ValidateNumericEditComponent>();

        var sut = Page.Locator( "#validate-numeric-with-bind-initially-populated" );
        var textBox = sut.Locator( "input[type=number]" );
        var validationFeedback = sut.GetByText( "error" );

        await ExpectTo_StartValid_InvalidUponClear_ValidUponFill( textBox, validationFeedback );
    }

    [Test]
    public async Task CanValidateNumericWithEvent_InitiallyBlank()
    {

        await SelectTestComponent<ValidateNumericEditComponent>();

        var sut = Page.Locator( "#validate-numeric-with-event-initially-blank" );
        var textBox = sut.Locator( "input[type=number]" );
        var validationFeedback = sut.GetByText( "error" );

        await ExpectTo_StartInvalid_ValidUponFill_InvalidUponClear( textBox, validationFeedback );
    }

    [Test]
    public async Task CanValidateNumericWithEvent_InitiallyPopulated()
    {
        await SelectTestComponent<ValidateNumericEditComponent>();

        var sut = Page.Locator( "#validate-numeric-with-event-initially-populated" );
        var textBox = sut.Locator( "input[type=number]" );
        var validationFeedback = sut.GetByText( "error" );

        await ExpectTo_StartValid_InvalidUponClear_ValidUponFill( textBox, validationFeedback );
    }

    private async Task ExpectTo_StartInvalid_ValidUponFill_InvalidUponClear( ILocator textBox, ILocator validationFeedback )
    {
        await Expect( validationFeedback ).ToHaveClassAsync( "invalid-feedback" );

        await textBox.FillAsync( "1" );
        await Expect( validationFeedback ).ToBeHiddenAsync();

        await textBox.FillAsync( "" );
        await Expect( validationFeedback ).ToHaveClassAsync( "invalid-feedback" );
    }

    private async Task ExpectTo_StartValid_InvalidUponClear_ValidUponFill( ILocator textBox, ILocator validationFeedback )
    {
        await Expect( validationFeedback ).ToBeHiddenAsync();

        await textBox.FillAsync( "" );
        await Expect( validationFeedback ).ToHaveClassAsync( "invalid-feedback" );

        await textBox.FillAsync( "1" );
        await Expect( validationFeedback ).ToBeHiddenAsync();
    }

}
