namespace Blazorise.E2E.Tests.Tests.Components.NumericInput;

[Parallelizable( ParallelScope.Self )]
[TestFixture]
public class NumericInputTests : BlazorisePageTest
{
    [Test]
    public async Task CanChangeUndefinedIntegerUsingEvent()
    {
        await SelectTestComponent<NumericInputComponent>();

        var sut = Page.Locator( "#int-event-initially-undefined" );
        var input = sut.Locator( "input[type=number]" );
        var result = sut.Locator( "#int-event-initially-undefined-result" );

        await Expect( result ).ToHaveTextAsync( "0" );

        await input.FillAsync( "100" );
        await Expect( result ).ToHaveTextAsync( "100" );

        await Page.Keyboard.PressAsync( "Backspace" );
        await Expect( result ).ToHaveTextAsync( "10" );
    }

    [Test]
    public async Task CanChangeNullableIntegerUsingEvent()
    {
        await SelectTestComponent<NumericInputComponent>();

        var sut = Page.Locator( "#nullable-int-event-initially-null" );
        var input = sut.Locator( "input[type=number]" );
        var result = sut.Locator( "#nullable-int-event-initially-null-result" );

        await Expect( result ).ToHaveTextAsync( string.Empty );

        await input.FillAsync( "100" );
        await Expect( result ).ToHaveTextAsync( "100" );

        await Page.Keyboard.PressAsync( "Backspace" );
        await Expect( result ).ToHaveTextAsync( "10" );
    }

    [Test]
    public async Task CanChangeUndefinedDecimalUsingEvent()
    {
        await SelectTestComponent<NumericInputComponent>();

        var sut = Page.Locator( "#decimal-event-initially-undefined" );
        var input = sut.Locator( "input[type=number]" );
        var result = sut.Locator( "#decimal-event-initially-undefined-result" );

        await Expect( result ).ToHaveTextAsync( "0" );

        await input.FillAsync( "100" );
        await Expect( result ).ToHaveTextAsync( "100" );

        await Page.Keyboard.PressAsync( "Backspace" );
        await Expect( result ).ToHaveTextAsync( "10" );
    }

    [Test]
    public async Task CanChangeNullableDecimalUsingEvent()
    {
        await SelectTestComponent<NumericInputComponent>();

        var sut = Page.Locator( "#nullable-decimal-event-initially-null" );
        var input = sut.Locator( "input[type=number]" );
        var result = sut.Locator( "#nullable-decimal-event-initially-null-result" );

        await Expect( result ).ToHaveTextAsync( string.Empty );

        await input.FillAsync( "100" );
        await Expect( result ).ToHaveTextAsync( "100" );

        await Page.Keyboard.PressAsync( "Backspace" );
        await Expect( result ).ToHaveTextAsync( "10" );
    }

    [Test]
    public async Task CanChangeValueWithStepDefault()
    {
        await SelectTestComponent<NumericInputComponent>();

        var sut = Page.Locator( "#step-change-default" );
        var input = sut.Locator( "input[type=number]" );
        var result = sut.Locator( "#step-change-default-result" );

        await Expect( result ).ToHaveTextAsync( "1" );

        await input.FocusAsync();
        await Page.Keyboard.PressAsync( "ArrowUp" );
        await Page.Keyboard.PressAsync( "ArrowUp" );
        await Expect( result ).ToHaveTextAsync( "3" );

        await Page.Keyboard.PressAsync( "ArrowDown" );
        await Expect( result ).ToHaveTextAsync( "2" );
    }

    [Test]
    public async Task CanChangeValueWithStepBy2()
    {
        await SelectTestComponent<NumericInputComponent>();

        var sut = Page.Locator( "#step-change-by-2" );
        var input = sut.Locator( "input[type=number]" );
        var result = sut.Locator( "#step-change-by-2-result" );

        await Expect( result ).ToHaveTextAsync( "2" );

        await input.FocusAsync();
        await Page.Keyboard.PressAsync( "ArrowUp" );
        await Page.Keyboard.PressAsync( "ArrowUp" );
        await Expect( result ).ToHaveTextAsync( "6" );

        await Page.Keyboard.PressAsync( "ArrowDown" );
        await Expect( result ).ToHaveTextAsync( "4" );
    }

    // [Test]
    // Removed temporarly as execution fails on a colleague's machine for some unknown reason...
    // Have to figure out if it's something with playwright configuration... or something else...
    public async Task CanTypeNumberWithDotDecimalSeparator()
    {
        await SelectTestComponent<NumericInputComponent>();

        var sut = Page.Locator( "#decimal-separator-with-dot" );
        var input = sut.Locator( "input[type=number]" );
        var result = sut.Locator( "#decimal-separator-with-dot-result" );

        await Expect( result ).ToHaveTextAsync( "42.5" );

        await input.FocusAsync();
        await Page.Keyboard.PressAsync( "6" );
        await Expect( result ).ToHaveTextAsync( "42.56" );

        await Page.Keyboard.PressAsync( "Backspace" );
        await Page.Keyboard.PressAsync( "Backspace" );

        await Expect( result ).ToHaveTextAsync( "42" );

        await Page.Keyboard.PressAsync( ".", new KeyboardPressOptions() { Delay = 100 } );
        await Page.Keyboard.PressAsync( "3" );
        await Expect( result ).ToHaveTextAsync( "42.3" );
    }

    [Test]
    public async Task CanTypeMinMax()
    {
        await SelectTestComponent<NumericInputComponent>();

        var sut = Page.Locator( "#decimal-min-max-non-nullable" );
        var input = sut.Locator( "input[type=number]" );
        var result = sut.Locator( "#decimal-min-max-non-nullable-result" );

        await Expect( result ).ToHaveTextAsync( "0" );

        await input.FillAsync( "2" );
        await Expect( result ).ToHaveTextAsync( "2" );

        await input.BlurAsync();
        await Expect( result ).ToHaveTextAsync( "10" );

        await input.BlurAsync();
        await input.FillAsync( "15" );
        await Expect( result ).ToHaveTextAsync( "15" );
        await input.BlurAsync();
        await Expect( result ).ToHaveTextAsync( expected: "15" );

        await input.BlurAsync();
        await input.FillAsync( "21" );
        await Expect( result ).ToHaveTextAsync( "21" );
        await input.BlurAsync();
        await Expect( result ).ToHaveTextAsync( expected: "20" );

        await input.BlurAsync();
        await input.FillAsync( "0" );
        await Expect( result ).ToHaveTextAsync( "0" );
        await input.BlurAsync();
        await Expect( result ).ToHaveTextAsync( expected: "10" );
    }
}
