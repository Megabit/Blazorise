#region Using directives
using System.Diagnostics;
#endregion

namespace Blazorise.E2E.Tests.Tests.Components.MemoInput;

[Parallelizable( ParallelScope.Self )]
[TestFixture]
public class MemoInputTests : BlazorisePageTest
{
    [Test]
    public async Task CanChangeText()
    {
        await SelectExample();

        var textarea = Page.Locator( "#memo-basic" ).GetByRole( AriaRole.Textbox );
        await Expect( textarea ).ToHaveValueAsync( string.Empty );

        await textarea.FillAsync( "first\nsecond" );
        await Expect( textarea ).ToHaveValueAsync( "first\nsecond" );
    }

    [TestCase( "memo-bind" )]
    [TestCase( "memo-event" )]
    public async Task CanChangeTextUsingBindingOrEvent( string exampleId )
    {
        await SelectExample();

        var sut = Page.Locator( $"#{exampleId}" );
        var textarea = sut.GetByRole( AriaRole.Textbox );
        var result = sut.Locator( $"#{exampleId}-result" );
        await Expect( result ).ToHaveTextAsync( string.Empty );

        await textarea.FillAsync( "first\nsecond" );
        await Expect( textarea ).ToHaveValueAsync( "first\nsecond" );
        await Expect( result ).ToHaveTextAsync( "first\nsecond" );

        await textarea.PressAsync( "Backspace" );
        await Expect( textarea ).ToHaveValueAsync( "first\nsecon" );
        await Expect( result ).ToHaveTextAsync( "first\nsecon" );
    }

    [Test]
    public async Task CanValidateTextWithBind()
    {
        await SelectExample();

        var sut = Page.Locator( "#memo-validation" );
        var textarea = sut.GetByRole( AriaRole.Textbox );
        var feedback = sut.GetByText( "A value is required." );
        var result = sut.Locator( "#memo-validation-result" );

        await Expect( feedback ).ToBeVisibleAsync();

        await textarea.FillAsync( "A valid value" );
        await Expect( result ).ToHaveTextAsync( "A valid value" );
        await Expect( feedback ).ToBeHiddenAsync();

        await textarea.FillAsync( string.Empty );
        await Expect( result ).ToHaveTextAsync( string.Empty );
        await Expect( feedback ).ToBeVisibleAsync();
    }

    [Test]
    public async Task AutoSize_GrowsAndShrinksWithContent()
    {
        await SelectExample();

        var sut = Page.Locator( "#memo-sizing" );
        var textarea = sut.GetByRole( AriaRole.Textbox );
        var result = sut.Locator( "#memo-autosize-result" );
        await Expect( textarea ).ToHaveValueAsync( "Short value" );
        var initialHeight = ( await textarea.BoundingBoxAsync() ).Height;
        var longValue = string.Join( "\n", Enumerable.Repeat( "A line entered by the user.", 8 ) );

        await textarea.FillAsync( longValue );
        await Expect( result ).ToHaveTextAsync( longValue );
        await WaitForHeight( textarea, initialHeight, true );
        var grownHeight = ( await textarea.BoundingBoxAsync() ).Height;
        Assert.That( grownHeight, Is.GreaterThan( initialHeight ) );

        await textarea.FillAsync( "Short value" );
        await Expect( result ).ToHaveTextAsync( "Short value" );
        await WaitForHeight( textarea, grownHeight, false );
        var shrunkHeight = ( await textarea.BoundingBoxAsync() ).Height;
        Assert.That( shrunkHeight, Is.LessThan( grownHeight ) );
        Assert.That( shrunkHeight, Is.GreaterThanOrEqualTo( initialHeight ) );
    }

    [Test]
    public async Task AutoSize_RespondsToProgrammaticValuesAndRuntimeToggling()
    {
        await SelectExample();

        var sut = Page.Locator( "#memo-sizing" );
        var textarea = sut.GetByRole( AriaRole.Textbox );
        await Expect( textarea ).ToHaveValueAsync( "Short value" );
        var initialHeight = ( await textarea.BoundingBoxAsync() ).Height;
        var longValue = string.Join( "\n", Enumerable.Repeat( "A line of programmatically supplied text.", 8 ) );

        await sut.GetByRole( AriaRole.Button, new() { Name = "Set long value" } ).ClickAsync();
        await Expect( textarea ).ToHaveValueAsync( longValue );
        await WaitForHeight( textarea, initialHeight, true );
        var grownHeight = ( await textarea.BoundingBoxAsync() ).Height;

        await sut.Locator( "#memo-toggle-autosize" ).ClickAsync();
        await Expect( sut.Locator( "#memo-toggle-autosize" ) ).ToHaveTextAsync( "Auto size: False" );
        await WaitForHeight( textarea, grownHeight, false );
        var fixedHeight = ( await textarea.BoundingBoxAsync() ).Height;

        await textarea.FillAsync( "Short value" );
        await Expect( sut.Locator( "#memo-autosize-result" ) ).ToHaveTextAsync( "Short value" );
        Assert.That( ( await textarea.BoundingBoxAsync() ).Height, Is.EqualTo( fixedHeight ).Within( 1 ) );
    }

    [Test]
    public async Task ReplaceTab_NotifiesValueChangedWhenIndentingAndUnindentingSelection()
    {
        await SelectExample();

        var sut = Page.Locator( "#memo-editing" );
        var textarea = sut.GetByRole( AriaRole.Textbox );
        var result = sut.Locator( "#memo-editing-result" );
        var changeCount = sut.Locator( "#memo-change-count" );
        var originalValue = "first\nsecond";
        var indentedValue = "    first\n    second";

        await textarea.FillAsync( originalValue );
        await Expect( result ).ToHaveTextAsync( originalValue );
        await Expect( changeCount ).ToHaveTextAsync( "1" );

        await textarea.SelectTextAsync();
        await textarea.PressAsync( "Tab" );
        await Expect( textarea ).ToHaveValueAsync( indentedValue );
        await Expect( result ).ToHaveTextAsync( indentedValue );
        await Expect( changeCount ).ToHaveTextAsync( "2" );

        await textarea.PressAsync( "Shift+Tab" );
        await Expect( textarea ).ToHaveValueAsync( originalValue );
        await Expect( result ).ToHaveTextAsync( originalValue );
        await Expect( changeCount ).ToHaveTextAsync( "3" );
    }

    [TestCase( "(", ")" )]
    [TestCase( "[", "]" )]
    [TestCase( "{", "}" )]
    [TestCase( "\"", "\"" )]
    [TestCase( "'", "'" )]
    public async Task ReplaceTab_PairsCharactersAndHandlesOverwriteAndBackspace( string opening, string closing )
    {
        await SelectExample();

        var sut = Page.Locator( "#memo-editing" );
        var textarea = sut.GetByRole( AriaRole.Textbox );
        var result = sut.Locator( "#memo-editing-result" );
        var changeCount = sut.Locator( "#memo-change-count" );
        var keyPressCount = sut.Locator( "#memo-keypress-count" );
        await Expect( textarea ).ToHaveValueAsync( string.Empty );
        await Expect( result ).ToHaveTextAsync( string.Empty );

        await textarea.ClickAsync();
        await textarea.PressAsync( opening );
        await Expect( textarea ).ToHaveValueAsync( opening + closing );
        await Expect( result ).ToHaveTextAsync( opening + closing );
        await Expect( changeCount ).ToHaveTextAsync( "1" );
        await Expect( keyPressCount ).ToHaveTextAsync( "1" );
        await Expect( textarea ).ToHaveJSPropertyAsync( "selectionStart", 1 );

        await textarea.PressAsync( closing );
        await Expect( textarea ).ToHaveValueAsync( opening + closing );
        await Expect( changeCount ).ToHaveTextAsync( "1" );
        await Expect( keyPressCount ).ToHaveTextAsync( "2" );
        await Expect( textarea ).ToHaveJSPropertyAsync( "selectionStart", 2 );

        await textarea.PressAsync( "ArrowLeft" );
        await textarea.PressAsync( "Backspace" );
        await Expect( textarea ).ToHaveValueAsync( string.Empty );
        await Expect( result ).ToHaveTextAsync( string.Empty );
        await Expect( changeCount ).ToHaveTextAsync( "2" );
    }

    [TestCase( "{}", 1, "{\n    \n}", 6 )]
    [TestCase( "[]", 1, "[\n    \n]", 6 )]
    [TestCase( "()", 1, "(\n)", 2 )]
    [TestCase( "{text", 5, "{text\n    ", 10 )]
    [TestCase( "\"[\"", 3, "\"[\"\n", 4 )]
    public async Task ReplaceTab_IndentsNewLinesAndPreservesCaret( string initialValue, int caret, string expectedValue, int expectedCaret )
    {
        await SelectExample();

        var sut = Page.Locator( "#memo-editing" );
        var textarea = sut.GetByRole( AriaRole.Textbox );
        var result = sut.Locator( "#memo-editing-result" );

        await textarea.FillAsync( initialValue );
        await Expect( result ).ToHaveTextAsync( initialValue );
        await textarea.PressAsync( "Home" );

        for ( var index = 0; index < caret; index++ )
        {
            await textarea.PressAsync( "ArrowRight" );
        }

        await Expect( textarea ).ToHaveJSPropertyAsync( "selectionStart", caret );
        await textarea.PressAsync( "Enter" );
        await Expect( textarea ).ToHaveValueAsync( expectedValue );
        await Expect( result ).ToHaveTextAsync( expectedValue );
        await Expect( textarea ).ToHaveJSPropertyAsync( "selectionStart", expectedCaret );
    }

    [Test]
    public async Task ReplaceTab_RespondsToRuntimeOptionsWithoutDuplicateHandlers()
    {
        await SelectExample();

        var sut = Page.Locator( "#memo-editing" );
        var textarea = sut.GetByRole( AriaRole.Textbox );

        await textarea.ClickAsync();
        await textarea.PressAsync( "Tab" );
        await Expect( textarea ).ToHaveValueAsync( "    " );
        await textarea.PressAsync( "Shift+Tab" );
        await Expect( textarea ).ToHaveValueAsync( string.Empty );

        await sut.Locator( "#memo-change-tab-size" ).ClickAsync();
        await Expect( sut.Locator( "#memo-change-tab-size" ) ).ToHaveTextAsync( "Tab size: 2" );
        await textarea.PressAsync( "Tab" );
        await Expect( textarea ).ToHaveValueAsync( "  " );
        await textarea.PressAsync( "Shift+Tab" );
        await Expect( textarea ).ToHaveValueAsync( string.Empty );

        await sut.Locator( "#memo-toggle-soft-tabs" ).ClickAsync();
        await Expect( sut.Locator( "#memo-toggle-soft-tabs" ) ).ToHaveTextAsync( "Soft tabs: False" );
        await Expect( textarea ).ToHaveCSSAsync( "tab-size", "2" );
        await textarea.PressAsync( "Tab" );
        await Expect( textarea ).ToHaveValueAsync( "\t" );
        await textarea.PressAsync( "Shift+Tab" );
        await Expect( textarea ).ToHaveValueAsync( string.Empty );

        await sut.Locator( "#memo-toggle-replace-tab" ).ClickAsync();
        await Expect( sut.Locator( "#memo-toggle-replace-tab" ) ).ToHaveTextAsync( "Replace tab: False" );
        await textarea.PressAsync( "Tab" );
        await Expect( textarea ).Not.ToBeFocusedAsync();

        await textarea.PressAsync( "(" );
        await Expect( textarea ).ToHaveValueAsync( "(" );

        await sut.Locator( "#memo-toggle-replace-tab" ).ClickAsync();
        await Expect( sut.Locator( "#memo-toggle-replace-tab" ) ).ToHaveTextAsync( "Replace tab: True" );
        await textarea.FillAsync( string.Empty );
        await textarea.PressAsync( "Tab" );
        await Expect( textarea ).ToHaveValueAsync( "\t" );
    }

    [Test]
    public async Task ReplaceTab_UpdatesDeferredValueOnBlur()
    {
        await SelectExample();

        var sut = Page.Locator( "#memo-editing" );
        var textarea = sut.GetByRole( AriaRole.Textbox );
        var result = sut.Locator( "#memo-editing-result" );

        await sut.Locator( "#memo-toggle-immediate" ).ClickAsync();
        await Expect( sut.Locator( "#memo-toggle-immediate" ) ).ToHaveTextAsync( "Immediate: False" );

        await textarea.PressAsync( "(" );
        await Expect( textarea ).ToHaveValueAsync( "()" );
        await Expect( result ).ToHaveTextAsync( string.Empty );

        await Page.Locator( "#memo-basic" ).GetByRole( AriaRole.Textbox ).ClickAsync();
        await Expect( result ).ToHaveTextAsync( "()" );
        await Expect( sut.Locator( "#memo-change-count" ) ).ToHaveTextAsync( "1" );
    }

    [TestCase( "x" )]
    [TestCase( "😀" )]
    public async Task ReplaceTab_PreservesNativeBackspaceAndReadOnlyBehavior( string initialValue )
    {
        await SelectExample();

        var sut = Page.Locator( "#memo-editing" );
        var textarea = sut.GetByRole( AriaRole.Textbox );
        var result = sut.Locator( "#memo-editing-result" );

        await textarea.FillAsync( initialValue );
        await Expect( result ).ToHaveTextAsync( initialValue );
        await textarea.PressAsync( "Home" );
        await textarea.PressAsync( "Backspace" );
        await Expect( textarea ).ToHaveValueAsync( initialValue );

        await textarea.PressAsync( "End" );
        await textarea.PressAsync( "Backspace" );
        await Expect( textarea ).ToHaveValueAsync( string.Empty );
        await Expect( result ).ToHaveTextAsync( string.Empty );

        await sut.Locator( "#memo-toggle-readonly" ).ClickAsync();
        await Expect( textarea ).Not.ToBeEditableAsync();
        await textarea.PressAsync( "(" );
        await textarea.PressAsync( "Enter" );
        await textarea.PressAsync( "Tab" );
        await Expect( textarea ).ToHaveValueAsync( string.Empty );
        await Expect( result ).ToHaveTextAsync( string.Empty );
        await Expect( textarea ).Not.ToBeFocusedAsync();
    }

    private async Task SelectExample()
    {
        await SelectTestComponent<MemoInputComponent>();

        // Autofocus is applied after MemoInput has initialized its editing handlers.
        await Expect( Page.Locator( "#memo-editing" ).GetByRole( AriaRole.Textbox ) ).ToBeFocusedAsync();
    }

    private static async Task WaitForHeight( ILocator textarea, double previousHeight, bool grows )
    {
        var stopwatch = Stopwatch.StartNew();
        float? height = null;

        while ( stopwatch.Elapsed < TimeSpan.FromSeconds( 5 ) )
        {
            height = ( await textarea.BoundingBoxAsync() )?.Height;

            if ( grows ? height > previousHeight : height < previousHeight )
            {
                return;
            }

            await Task.Delay( 50 );
        }

        Assert.Fail( $"Expected textarea height to {( grows ? "grow" : "shrink" )} from {previousHeight}, but last measured {height}." );
    }
}