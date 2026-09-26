namespace Blazorise.E2E.Tests.Tests.Components.NumericPicker;

public class NumericPickerAutofillTests : BlazorisePageTest
{
    [TestCase( "autofillImmediate", "1,234.56", "1234.56", "1,234.56", true )]
    [TestCase( "autofillDeferred", "1,234.56", "1234.56", "1,234.56", false )]
    [TestCase( "autofillComma", "1.234,56", "1234.56", "1.234,56", true )]
    [TestCase( "autofillComma", "1.234", "1234", "1.234,00", true )]
    public async Task NativeInput_ShouldBindValueAndPreserveItOnBlur( string elementId, string text, string value, string formattedValue, bool immediate )
    {
        await SelectTestComponent<NumericPickerAutofillComponent>();
        await WaitForPicker( elementId );

        ILocator input = Page.Locator( $"#{elementId}" );
        ILocator result = Page.Locator( $"#{elementId}Value" );

        // Fill produces browser input without AutoNumeric's keypress handling, like autofill.
        await input.FillAsync( text );
        await Expect( result ).ToHaveTextAsync( immediate ? value : "null" );

        await Page.Locator( "#autofillBlur" ).ClickAsync();
        await Expect( result ).ToHaveTextAsync( value );
        await Expect( input ).ToHaveValueAsync( formattedValue );

        await input.FillAsync( string.Empty );
        await Page.Locator( "#autofillBlur" ).ClickAsync();
        await Expect( result ).ToHaveTextAsync( "null" );
        await Expect( input ).ToHaveValueAsync( string.Empty );
    }

    [TestCase( "invalid", "12", "12.00" )]
    [TestCase( "101", "100", "100.00" )]
    [TestCase( "-1", "0", "0.00" )]
    public async Task NativeInput_ShouldRespectNumericValidationOnBlur( string text, string expectedValue, string expectedFormattedValue )
    {
        await SelectTestComponent<NumericPickerAutofillComponent>();
        await WaitForPicker( "autofillLimited" );

        ILocator input = Page.Locator( "#autofillLimited" );
        await Expect( input ).ToHaveValueAsync( "12.00" );
        await input.FillAsync( text );
        await Page.Locator( "#autofillBlur" ).ClickAsync();

        await Expect( Page.Locator( "#autofillLimitedValue" ) ).ToHaveTextAsync( expectedValue );
        await Expect( input ).ToHaveValueAsync( expectedFormattedValue );
    }

    [Test]
    public async Task Typing_ShouldStillAllowEnteringDecimals()
    {
        await SelectTestComponent<NumericPickerAutofillComponent>();
        await WaitForPicker( "autofillImmediate" );

        ILocator input = Page.Locator( "#autofillImmediate" );
        await input.FocusAsync();
        await Page.Keyboard.TypeAsync( "1234.56" );

        await Expect( Page.Locator( "#autofillImmediateValue" ) ).ToHaveTextAsync( "1234.56" );
        await Expect( input ).ToHaveValueAsync( "1,234.56" );
    }

    private async Task WaitForPicker( string elementId )
    {
        await Page.WaitForFunctionAsync( """
            elementId => typeof AutoNumeric !== 'undefined'
                && document.getElementById(elementId)
                && AutoNumeric.isManagedByAutoNumeric(document.getElementById(elementId))
            """, elementId );
    }
}