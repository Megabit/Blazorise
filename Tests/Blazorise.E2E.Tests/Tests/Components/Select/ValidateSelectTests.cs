namespace Blazorise.E2E.Tests.Tests.Components.Select;


[Parallelizable( ParallelScope.Self )]
[TestFixture]
public class ValidateSelectTests : BlazorisePageTest
{
    [Test]
    public async Task CanValidateString_InitiallyBlank()
    {
        await SelectTestComponent<ValidateSelectComponent>();

        var sut = Page.Locator( "#validate-string-initially-blank" );
        var select = sut.GetByRole( AriaRole.Combobox );
        var validationFeedback = sut.GetByText( "error" );

        await ExpectTo_StartInvalid_ValidUponFill_InvalidUponClear( select, validationFeedback );
    }

    [Test]
    public async Task CanValidateStringWithBind_InitiallyBlank()
    {
        await SelectTestComponent<ValidateSelectComponent>();

        var sut = Page.Locator( "#validate-string-with-bind-initially-blank" );
        var select = sut.GetByRole( AriaRole.Combobox );
        var validationFeedback = sut.GetByText( "error" );

        await ExpectTo_StartInvalid_ValidUponFill_InvalidUponClear( select, validationFeedback );
    }

    [Test]
    public async Task CanValidateStringWithBind_InitiallySelected()
    {
        await SelectTestComponent<ValidateSelectComponent>();

        var sut = Page.Locator( "#validate-string-with-bind-initially-selected" );
        var select = sut.GetByRole( AriaRole.Combobox );
        var validationFeedback = sut.GetByText( "error" );

        await ExpectTo_StartValid_InvalidUponClear_ValidUponFill( select, validationFeedback );
    }

    [Test]
    public async Task CanValidateStringWithEvent_InitiallyBlank()
    {

        await SelectTestComponent<ValidateSelectComponent>();

        var sut = Page.Locator( "#validate-string-with-event-initially-blank" );
        var select = sut.GetByRole( AriaRole.Combobox );
        var validationFeedback = sut.GetByText( "error" );

        await ExpectTo_StartInvalid_ValidUponFill_InvalidUponClear( select, validationFeedback );
    }

    [Test]
    public async Task CanValidateStringWithEvent_InitiallySelected()
    {

        await SelectTestComponent<ValidateSelectComponent>();

        var sut = Page.Locator( "#validate-string-with-event-initially-selected" );
        var select = sut.GetByRole( AriaRole.Combobox );
        var validationFeedback = sut.GetByText( "error" );

        await ExpectTo_StartValid_InvalidUponClear_ValidUponFill( select, validationFeedback );
    }

    [Test]
    public async Task CanValidateInt_InitiallyBlank()
    {
        await SelectTestComponent<ValidateSelectComponent>();

        var sut = Page.Locator( "#validate-int-initially-blank" );
        var select = sut.GetByRole( AriaRole.Combobox );
        var validationFeedback = sut.GetByText( "error" );

        await ExpectTo_StartInvalid_ValidUponFill_InvalidUponClear( select, validationFeedback );
    }

    [Test]
    public async Task CanValidateIntWithBind_InitiallyBlank()
    {
        await SelectTestComponent<ValidateSelectComponent>();

        var sut = Page.Locator( "#validate-int-with-bind-initially-blank" );
        var select = sut.GetByRole( AriaRole.Combobox );
        var validationFeedback = sut.GetByText( "error" );

        await ExpectTo_StartInvalid_ValidUponFill_InvalidUponClear( select, validationFeedback );
    }

    [Test]
    public async Task CanValidateIntWithBind_InitiallySelected()
    {
        await SelectTestComponent<ValidateSelectComponent>();

        var sut = Page.Locator( "#validate-int-with-bind-initially-selected" );
        var select = sut.GetByRole( AriaRole.Combobox );
        var validationFeedback = sut.GetByText( "error" );

        await ExpectTo_StartValid_InvalidUponClear_ValidUponFill( select, validationFeedback );
    }

    [Test]
    public async Task CanValidateEnum_InitiallyBlank()
    {
        await SelectTestComponent<ValidateSelectComponent>();

        var sut = Page.Locator( "#validate-enum-initially-blank" );
        var select = sut.GetByRole( AriaRole.Combobox );
        var validationFeedback = sut.GetByText( "error" );

        await ExpectTo_StartInvalid_ValidUponFill_InvalidUponClear( select, validationFeedback );
    }

    [Test]
    public async Task CanValidateEnumWithBind_InitiallyBlank()
    {
        await SelectTestComponent<ValidateSelectComponent>();

        var sut = Page.Locator( "#validate-enum-with-bind-initially-blank" );
        var select = sut.GetByRole( AriaRole.Combobox );
        var validationFeedback = sut.GetByText( "error" );

        await ExpectTo_StartInvalid_ValidUponFill_InvalidUponClear( select, validationFeedback );
    }

    [Test]
    public async Task CanValidateEnumWithBind_InitiallySelected()
    {
        await SelectTestComponent<ValidateSelectComponent>();

        var sut = Page.Locator( "#validate-enum-with-bind-initially-selected" );
        var select = sut.GetByRole( AriaRole.Combobox );
        var validationFeedback = sut.GetByText( "error" );

        await ExpectTo_StartValid_InvalidUponClear_ValidUponFill( select, validationFeedback );
    }

    [Test]
    public async Task CanValidateMultiString_InitiallyBlank()
    {
        await SelectTestComponent<ValidateSelectComponent>();

        var sut = Page.Locator( "#validate-multi-string-initially-blank" );
        var select = sut.GetByRole( AriaRole.Listbox );
        var validationFeedback = sut.GetByText( "error" );

        var options = await select.Locator( "option" ).AllAsync();


        await Expect( validationFeedback ).ToHaveClassAsync( "invalid-feedback" );
        await select.SelectOptionAsync( new SelectOptionValue[] { new() { Index = 1 }, new() { Index = 2 } } );
        await Expect( validationFeedback ).ToBeHiddenAsync();

        await options[1].ClickAsync( new() { Modifiers = new KeyboardModifier[] { KeyboardModifier.Control } } );
        await Expect( validationFeedback ).ToBeHiddenAsync();

        await options[2].ClickAsync( new() { Modifiers = new KeyboardModifier[] { KeyboardModifier.Control } } );
        await Expect( validationFeedback ).ToHaveClassAsync( "invalid-feedback" );

        await select.SelectOptionAsync( new SelectOptionValue() { Index = 0 } );
        await Expect( validationFeedback ).ToBeHiddenAsync();
    }

    private async Task ExpectTo_StartInvalid_ValidUponFill_InvalidUponClear( ILocator select, ILocator validationFeedback )
    {
        await Expect( validationFeedback ).ToHaveClassAsync( "invalid-feedback" );

        await select.SelectOptionAsync( new SelectOptionValue() { Index = 1 } );
        await Expect( validationFeedback ).ToBeHiddenAsync();

        await select.SelectOptionAsync( new SelectOptionValue() { Index = 0 } );
        await Expect( validationFeedback ).ToHaveClassAsync( "invalid-feedback" );
    }

    private async Task ExpectTo_StartValid_InvalidUponClear_ValidUponFill( ILocator select, ILocator validationFeedback )
    {
        await Expect( validationFeedback ).ToBeHiddenAsync();

        await select.SelectOptionAsync( new SelectOptionValue() { Index = 0 } );
        await Expect( validationFeedback ).ToHaveClassAsync( "invalid-feedback" );

        await select.SelectOptionAsync( new SelectOptionValue() { Index = 1 } );
        await Expect( validationFeedback ).ToBeHiddenAsync();
    }

}
