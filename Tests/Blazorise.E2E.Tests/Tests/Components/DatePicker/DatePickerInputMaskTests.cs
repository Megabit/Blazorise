namespace Blazorise.E2E.Tests.Tests.Components.DatePicker;

public class DatePickerInputMaskTests : BlazorisePageTest
{
    [TestCase( "masked-date-control", "01.01.2025" )]
    [TestCase( "masked-date-display-control", "2025-01-01" )]
    public async Task TypingYear_ShouldOverwritePrefilledDigits( string elementId, string displayValue )
    {
        await Page.Clock.SetFixedTimeAsync( new DateTime( 2026, 1, 1, 0, 0, 0, DateTimeKind.Utc ) );
        await SelectTestComponent<DatePickerComponent>();

        var input = Page.Locator( $"#{elementId}" );
        var result = Page.Locator( "#masked-date-result" );

        await input.FocusAsync();
        await Page.WaitForFunctionAsync( "elementId => !!document.getElementById(elementId).inputmask", elementId );
        await Page.Keyboard.TypeAsync( "010120" );

        await Expect( input ).ToHaveValueAsync( "01.01.2026" );
        await Expect( result ).ToHaveTextAsync( "2026-01-01" );
        await Expect( input ).ToHaveJSPropertyAsync( "selectionStart", 8 );
        await Expect( input ).ToHaveJSPropertyAsync( "selectionEnd", 8 );

        await Page.Keyboard.TypeAsync( "25" );

        await Expect( input ).ToHaveValueAsync( "01.01.2025" );
        await Expect( result ).ToHaveTextAsync( "2025-01-01" );
        await Expect( input ).ToHaveJSPropertyAsync( "selectionStart", 10 );
        await Expect( input ).ToHaveJSPropertyAsync( "selectionEnd", 10 );

        await input.BlurAsync();

        await Expect( input ).ToHaveValueAsync( displayValue );
        await Expect( result ).ToHaveTextAsync( "2025-01-01" );
    }
}