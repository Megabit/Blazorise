namespace Blazorise.E2E.Tests.Tests.Components.MemoInput;

[Parallelizable( ParallelScope.Self )]
[TestFixture]
public class MemoInputTests : BlazorisePageTest
{
    [Test]
    public async Task AutoSize_GrowsAndShrinksWithContent()
    {
        await SelectTestComponent<MemoInputComponent>();

        ILocator textarea = Page.Locator( "#memo-autosize" );
        float initialHeight = await GetHeight( textarea );

        await textarea.FillAsync( string.Join( "\n", Enumerable.Repeat( "A line entered by the user.", 8 ) ) );
        float grownHeight = await GetHeight( textarea );

        Assert.That( grownHeight, Is.GreaterThan( initialHeight ) );

        await textarea.FillAsync( "Short value" );
        float shrunkHeight = await GetHeight( textarea );

        Assert.That( shrunkHeight, Is.LessThan( grownHeight ) );
        Assert.That( shrunkHeight, Is.GreaterThanOrEqualTo( initialHeight ) );
    }

    [Test]
    public async Task AutoSize_RespondsToProgrammaticValuesAndRuntimeToggling()
    {
        await SelectTestComponent<MemoInputComponent>();

        ILocator textarea = Page.Locator( "#memo-autosize" );
        float initialHeight = await GetHeight( textarea );

        await Page.Locator( "#memo-set-long-value" ).ClickAsync();
        float programmaticHeight = await GetHeight( textarea );

        Assert.That( programmaticHeight, Is.GreaterThan( initialHeight ) );

        await Page.Locator( "#memo-toggle-autosize" ).ClickAsync();

        await Expect( textarea ).Not.ToHaveAttributeAsync( "data-autosize", "true" );
    }

    [Test]
    public async Task AutoSize_UsesJavaScriptFallbackWhenFieldSizingIsUnsupported()
    {
        await Page.AddInitScriptAsync(
            """
            const nativeSupports = CSS.supports.bind(CSS);
            CSS.supports = (property, value) => property === 'field-sizing'
                ? false
                : nativeSupports(property, value);
            """ );

        await SelectTestComponent<MemoInputComponent>();

        ILocator textarea = Page.Locator( "#memo-autosize" );

        await Expect( textarea ).ToHaveAttributeAsync( "data-blazorise-memo-auto-sized", "true" );

        float initialHeight = await GetHeight( textarea );

        await textarea.FillAsync( string.Join( "\n", Enumerable.Repeat( "Fallback content.", 8 ) ) );
        float grownHeight = await GetHeight( textarea );

        Assert.That( grownHeight, Is.GreaterThan( initialHeight ) );

        await Page.Locator( "#memo-toggle-autosize" ).ClickAsync();
        await Expect( textarea ).Not.ToHaveAttributeAsync( "data-blazorise-memo-auto-sized", "true" );
    }

    [TestCase( "Enter", "\n", true )]
    [TestCase( "Tab", "    ", true )]
    [TestCase( "Enter", "\n", false )]
    [TestCase( "Tab", "    ", false )]
    public async Task ReplaceTab_NotifiesValueChangedAccordingToImmediate( string key, string expectedValue, bool immediate )
    {
        await SelectTestComponent<MemoInputComponent>();

        var sut = Page.Locator( "#memo-editing" );
        var textarea = sut.GetByRole( AriaRole.Textbox );
        var result = sut.Locator( "#memo-replace-tab-result" );

        await Expect( textarea ).ToBeEditableAsync();

        if ( !immediate )
        {
            await sut.Locator( "#memo-toggle-immediate" ).ClickAsync();
            await Expect( sut.Locator( "#memo-toggle-immediate" ) ).ToHaveTextAsync( "Immediate: False" );
        }

        await textarea.ClickAsync();
        await Expect( textarea ).ToBeFocusedAsync();
        await Page.Keyboard.PressAsync( key );
        await Expect( textarea ).ToHaveValueAsync( expectedValue );

        if ( immediate )
        {
            await Expect( result ).ToHaveTextAsync( "1" );
            await Expect( result ).ToHaveAttributeAsync( "data-value", expectedValue );
        }
        else
        {
            await Expect( result ).ToHaveTextAsync( "0" );
        }

        await textarea.BlurAsync();

        await Expect( result ).ToHaveTextAsync( "1" );
        await Expect( result ).ToHaveAttributeAsync( "data-value", expectedValue );
    }

    [Test]
    public async Task ReplaceTab_NotifiesValueChangedWhenIndentingAndUnindentingSelection()
    {
        await SelectTestComponent<MemoInputComponent>();

        var sut = Page.Locator( "#memo-editing" );
        var textarea = sut.GetByRole( AriaRole.Textbox );
        var result = sut.Locator( "#memo-replace-tab-result" );
        var originalValue = "first\nsecond";
        var indentedValue = "    first\n    second";

        await Expect( textarea ).ToBeEditableAsync();

        await textarea.ClickAsync();
        await textarea.FillAsync( originalValue );
        await Expect( result ).ToHaveTextAsync( "1" );

        await textarea.SelectTextAsync();
        await Page.Keyboard.PressAsync( "Tab" );

        await Expect( textarea ).ToHaveValueAsync( indentedValue );
        await Expect( result ).ToHaveTextAsync( "2" );
        await Expect( result ).ToHaveAttributeAsync( "data-value", indentedValue );

        await Page.Keyboard.PressAsync( "Shift+Tab" );

        await Expect( textarea ).ToHaveValueAsync( originalValue );
        await Expect( result ).ToHaveTextAsync( "3" );
        await Expect( result ).ToHaveAttributeAsync( "data-value", originalValue );
    }

    [TestCase( "(", ")" )]
    [TestCase( "[", "]" )]
    [TestCase( "{", "}" )]
    [TestCase( "\"", "\"" )]
    [TestCase( "'", "'" )]
    public async Task ReplaceTab_PairsCharactersAndHandlesOverwriteAndBackspace( string opening, string closing )
    {
        await SelectTestComponent<MemoInputComponent>();

        var sut = Page.Locator( "#memo-editing" );
        var textarea = sut.GetByRole( AriaRole.Textbox );
        var result = sut.Locator( "#memo-replace-tab-result" );

        await Expect( textarea ).ToBeEditableAsync();
        await textarea.ClickAsync();
        await Expect( textarea ).ToBeFocusedAsync();
        await Page.Keyboard.PressAsync( opening );

        await Expect( textarea ).ToHaveValueAsync( opening + closing );
        await Expect( result ).ToHaveAttributeAsync( "data-value", opening + closing );
        await Expect( result ).ToHaveTextAsync( "1" );
        await Expect( result ).ToHaveAttributeAsync( "data-keypress-count", "1" );
        await Expect( textarea ).ToHaveJSPropertyAsync( "selectionStart", 1 );

        await Page.Keyboard.PressAsync( closing );

        await Expect( textarea ).ToHaveValueAsync( opening + closing );
        await Expect( result ).ToHaveTextAsync( "1" );
        await Expect( result ).ToHaveAttributeAsync( "data-keypress-count", "2" );
        await Expect( textarea ).ToHaveJSPropertyAsync( "selectionStart", 2 );

        await Page.Keyboard.PressAsync( "ArrowLeft" );
        await Page.Keyboard.PressAsync( "Backspace" );

        await Expect( textarea ).ToHaveValueAsync( string.Empty );
        await Expect( result ).ToHaveTextAsync( "2" );
        await Expect( result ).ToHaveAttributeAsync( "data-value", string.Empty );
    }

    [TestCase( "{}", 1, "{\n    \n}", 6 )]
    [TestCase( "[]", 1, "[\n    \n]", 6 )]
    [TestCase( "()", 1, "(\n)", 2 )]
    [TestCase( "{text", 5, "{text\n    ", 10 )]
    [TestCase( "\"[\"", 3, "\"[\"\n", 4 )]
    public async Task ReplaceTab_IndentsNewLinesAndPreservesCaret( string initialValue, int caret, string expectedValue, int expectedCaret )
    {
        await SelectTestComponent<MemoInputComponent>();

        var sut = Page.Locator( "#memo-editing" );
        var textarea = sut.GetByRole( AriaRole.Textbox );
        var result = sut.Locator( "#memo-replace-tab-result" );

        await Expect( textarea ).ToBeEditableAsync();
        await textarea.ClickAsync();
        await textarea.FillAsync( initialValue );
        await Expect( result ).ToHaveAttributeAsync( "data-value", initialValue );
        await Page.Keyboard.PressAsync( "Home" );

        for ( var index = 0; index < caret; index++ )
        {
            await Page.Keyboard.PressAsync( "ArrowRight" );
        }

        await Expect( textarea ).ToHaveJSPropertyAsync( "selectionStart", caret );
        await Page.Keyboard.PressAsync( "Enter" );

        await Expect( textarea ).ToHaveValueAsync( expectedValue );
        await Expect( result ).ToHaveAttributeAsync( "data-value", expectedValue );
        await Expect( textarea ).ToHaveJSPropertyAsync( "selectionStart", expectedCaret );
    }

    [Test]
    public async Task ReplaceTab_RespondsToRuntimeOptionsWithoutDuplicateHandlers()
    {
        await SelectTestComponent<MemoInputComponent>();

        var sut = Page.Locator( "#memo-editing" );
        var textarea = sut.GetByRole( AriaRole.Textbox );

        await Expect( textarea ).ToBeEditableAsync();
        await textarea.ClickAsync();
        await Expect( textarea ).ToBeFocusedAsync();
        await Page.Keyboard.PressAsync( "Tab" );
        await Expect( textarea ).ToHaveValueAsync( "    " );
        await Page.Keyboard.PressAsync( "Shift+Tab" );
        await Expect( textarea ).ToHaveValueAsync( string.Empty );

        await sut.Locator( "#memo-change-tab-size" ).ClickAsync();
        await Expect( sut.Locator( "#memo-change-tab-size" ) ).ToHaveTextAsync( "Tab size: 2" );
        await textarea.ClickAsync();
        await Page.Keyboard.PressAsync( "Tab" );
        await Expect( textarea ).ToHaveValueAsync( "  " );
        await Page.Keyboard.PressAsync( "Shift+Tab" );
        await Expect( textarea ).ToHaveValueAsync( string.Empty );

        await sut.Locator( "#memo-toggle-soft-tabs" ).ClickAsync();
        await Expect( textarea ).ToHaveCSSAsync( "tab-size", "2" );
        await textarea.ClickAsync();
        await Page.Keyboard.PressAsync( "Tab" );
        await Expect( textarea ).ToHaveValueAsync( "\t" );
        await Page.Keyboard.PressAsync( "Shift+Tab" );
        await Expect( textarea ).ToHaveValueAsync( string.Empty );

        await sut.Locator( "#memo-toggle-replace-tab" ).ClickAsync();
        await Expect( sut.Locator( "#memo-toggle-replace-tab" ) ).ToHaveTextAsync( "Replace tab: False" );
        await textarea.ClickAsync();
        await Page.Keyboard.PressAsync( "Tab" );
        await Expect( textarea ).Not.ToBeFocusedAsync();

        await textarea.ClickAsync();
        await Page.Keyboard.PressAsync( "(" );
        await Expect( textarea ).ToHaveValueAsync( "(" );

        await sut.Locator( "#memo-toggle-replace-tab" ).ClickAsync();
        await Expect( sut.Locator( "#memo-toggle-replace-tab" ) ).ToHaveTextAsync( "Replace tab: True" );
        await Expect( textarea ).ToBeEditableAsync();
        await textarea.FillAsync( string.Empty );
        await Page.Keyboard.PressAsync( "Tab" );
        await Expect( textarea ).ToHaveValueAsync( "\t" );
    }

    [TestCase( "x" )]
    [TestCase( "😀" )]
    public async Task ReplaceTab_PreservesNativeBackspaceAndReadOnlyBehavior( string initialValue )
    {
        await SelectTestComponent<MemoInputComponent>();

        var sut = Page.Locator( "#memo-editing" );
        var textarea = sut.GetByRole( AriaRole.Textbox );
        var result = sut.Locator( "#memo-replace-tab-result" );

        await Expect( textarea ).ToBeEditableAsync();
        await textarea.ClickAsync();
        await textarea.FillAsync( initialValue );
        await Expect( result ).ToHaveAttributeAsync( "data-value", initialValue );
        await Page.Keyboard.PressAsync( "Home" );
        await Page.Keyboard.PressAsync( "Backspace" );
        await Expect( textarea ).ToHaveValueAsync( initialValue );

        await Page.Keyboard.PressAsync( "End" );
        await Page.Keyboard.PressAsync( "Backspace" );
        await Expect( textarea ).ToHaveValueAsync( string.Empty );
        await Expect( result ).ToHaveAttributeAsync( "data-value", string.Empty );

        await sut.Locator( "#memo-toggle-readonly" ).ClickAsync();
        await Expect( textarea ).ToHaveJSPropertyAsync( "readOnly", true );
        await textarea.ClickAsync();
        await Page.Keyboard.PressAsync( "(" );
        await Page.Keyboard.PressAsync( "Enter" );
        await Page.Keyboard.PressAsync( "Tab" );

        await Expect( textarea ).ToHaveValueAsync( string.Empty );
        await Expect( textarea ).Not.ToBeFocusedAsync();
    }

    private static Task<float> GetHeight( ILocator locator )
        => locator.EvaluateAsync<float>( "element => element.getBoundingClientRect().height" );
}