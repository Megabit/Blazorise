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

        var textarea = Page.Locator( "#memo-replace-tab" );
        var result = Page.Locator( "#memo-replace-tab-result" );

        if ( !immediate )
        {
            await Page.Locator( "#memo-toggle-immediate" ).ClickAsync();
        }

        await textarea.PressAsync( key );
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

        var textarea = Page.Locator( "#memo-replace-tab" );
        var result = Page.Locator( "#memo-replace-tab-result" );
        var originalValue = "first\nsecond";
        var indentedValue = "    first\n    second";

        await textarea.FillAsync( originalValue );
        await Expect( result ).ToHaveTextAsync( "1" );

        await textarea.SelectTextAsync();
        await textarea.PressAsync( "Tab" );

        await Expect( textarea ).ToHaveValueAsync( indentedValue );
        await Expect( result ).ToHaveTextAsync( "2" );
        await Expect( result ).ToHaveAttributeAsync( "data-value", indentedValue );

        await textarea.PressAsync( "Shift+Tab" );

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

        var textarea = Page.Locator( "#memo-replace-tab" );
        var result = Page.Locator( "#memo-replace-tab-result" );

        await textarea.PressAsync( opening );

        await Expect( textarea ).ToHaveValueAsync( opening + closing );
        await Expect( result ).ToHaveAttributeAsync( "data-value", opening + closing );
        await Expect( result ).ToHaveTextAsync( "1" );
        await Expect( result ).ToHaveAttributeAsync( "data-keypress-count", "1" );
        Assert.That( await textarea.EvaluateAsync<int>( "element => element.selectionStart" ), Is.EqualTo( 1 ) );

        await textarea.PressAsync( closing );

        await Expect( textarea ).ToHaveValueAsync( opening + closing );
        await Expect( result ).ToHaveTextAsync( "1" );
        await Expect( result ).ToHaveAttributeAsync( "data-keypress-count", "2" );
        Assert.That( await textarea.EvaluateAsync<int>( "element => element.selectionStart" ), Is.EqualTo( 2 ) );

        await textarea.PressAsync( "ArrowLeft" );
        await textarea.PressAsync( "Backspace" );

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

        var textarea = Page.Locator( "#memo-replace-tab" );
        var result = Page.Locator( "#memo-replace-tab-result" );

        await textarea.FillAsync( initialValue );
        await Expect( result ).ToHaveAttributeAsync( "data-value", initialValue );
        await textarea.EvaluateAsync( "(element, caret) => element.setSelectionRange(caret, caret)", caret );
        await textarea.PressAsync( "Enter" );

        await Expect( textarea ).ToHaveValueAsync( expectedValue );
        await Expect( result ).ToHaveAttributeAsync( "data-value", expectedValue );
        Assert.That( await textarea.EvaluateAsync<int>( "element => element.selectionStart" ), Is.EqualTo( expectedCaret ) );
    }

    [Test]
    public async Task ReplaceTab_RespondsToRuntimeOptionsWithoutDuplicateHandlers()
    {
        await SelectTestComponent<MemoInputComponent>();

        var textarea = Page.Locator( "#memo-replace-tab" );

        await textarea.PressAsync( "Tab" );
        await Expect( textarea ).ToHaveValueAsync( "    " );
        await textarea.PressAsync( "Shift+Tab" );
        await Expect( textarea ).ToHaveValueAsync( string.Empty );

        await Page.Locator( "#memo-change-tab-size" ).ClickAsync();
        await textarea.PressAsync( "Tab" );
        await Expect( textarea ).ToHaveValueAsync( "  " );
        await textarea.PressAsync( "Shift+Tab" );
        await Expect( textarea ).ToHaveValueAsync( string.Empty );

        await Page.Locator( "#memo-toggle-soft-tabs" ).ClickAsync();
        await textarea.PressAsync( "Tab" );
        await Expect( textarea ).ToHaveValueAsync( "\t" );
        await textarea.PressAsync( "Shift+Tab" );
        await Expect( textarea ).ToHaveValueAsync( string.Empty );

        await Page.Locator( "#memo-toggle-replace-tab" ).ClickAsync();
        await textarea.PressAsync( "Tab" );
        await Expect( textarea ).Not.ToBeFocusedAsync();
        await textarea.PressAsync( "(" );
        await Expect( textarea ).ToHaveValueAsync( "(" );

        await Page.Locator( "#memo-toggle-replace-tab" ).ClickAsync();
        await textarea.FillAsync( string.Empty );
        await textarea.PressAsync( "Tab" );
        await Expect( textarea ).ToHaveValueAsync( "\t" );
    }

    [TestCase( "x" )]
    [TestCase( "😀" )]
    public async Task ReplaceTab_PreservesNativeBackspaceAndReadOnlyBehavior( string initialValue )
    {
        await SelectTestComponent<MemoInputComponent>();

        var textarea = Page.Locator( "#memo-replace-tab" );
        var result = Page.Locator( "#memo-replace-tab-result" );

        await textarea.FillAsync( initialValue );
        await Expect( result ).ToHaveAttributeAsync( "data-value", initialValue );
        await textarea.PressAsync( "Home" );
        await textarea.PressAsync( "Backspace" );
        await Expect( textarea ).ToHaveValueAsync( initialValue );

        await textarea.PressAsync( "End" );
        await textarea.PressAsync( "Backspace" );
        await Expect( textarea ).ToHaveValueAsync( string.Empty );
        await Expect( result ).ToHaveAttributeAsync( "data-value", string.Empty );

        await Page.Locator( "#memo-toggle-readonly" ).ClickAsync();
        await textarea.PressAsync( "(" );
        await textarea.PressAsync( "Enter" );
        await textarea.PressAsync( "Tab" );

        await Expect( textarea ).ToHaveValueAsync( string.Empty );
        await Expect( textarea ).Not.ToBeFocusedAsync();
    }

    private static Task<float> GetHeight( ILocator locator )
        => locator.EvaluateAsync<float>( "element => element.getBoundingClientRect().height" );
}