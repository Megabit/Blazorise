namespace Blazorise.E2E.Tests.Tests.Extensions.CodeEditor;

[Parallelizable( ParallelScope.Self )]
[TestFixture]
public class CodeEditorTests : BlazorisePageTest
{
    [SetUp]
    public async Task Init()
    {
        await SelectTestComponent<CodeEditorComponent>();
        await WaitForEditor( "#code-editor-immediate" );
    }

    [Test]
    public async Task UpdatesBoundValueWhileTyping()
    {
        await ReplaceEditorValue( "#code-editor-immediate", "immediate value" );

        await Expect( Page.Locator( "#code-editor-immediate-result" ) ).ToHaveTextAsync( "immediate value" );
    }

    [Test]
    public async Task UpdatesBoundValueOnBlur()
    {
        await ReplaceEditorValue( "#code-editor-blur", "blur value" );
        await Expect( Page.Locator( "#code-editor-blur-result" ) ).ToHaveTextAsync( string.Empty );

        await Page.Locator( "#code-editor-blur-target" ).ClickAsync();

        await Expect( Page.Locator( "#code-editor-blur-result" ) ).ToHaveTextAsync( "blur value" );
    }

    [Test]
    public async Task DebouncesBoundValueUpdates()
    {
        await ReplaceEditorValue( "#code-editor-debounce", "debounced value" );
        await Page.WaitForTimeoutAsync( 250 );
        await Expect( Page.Locator( "#code-editor-debounce-result" ) ).ToHaveTextAsync( string.Empty );

        await Expect( Page.Locator( "#code-editor-debounce-result" ) ).ToHaveTextAsync(
            "debounced value",
            new LocatorAssertionsToHaveTextOptions { Timeout = 2000 } );
    }

    [Test]
    public async Task FormatsDocumentWithDotNetProvider()
    {
        await WaitForEditor( "#code-editor-formatting" );
        await Page.Locator( "#code-editor-format" ).ClickAsync();

        await Expect( Page.Locator( "#code-editor-format-result" ) ).ToContainTextAsync( "\"name\": \"Blazorise\"" );
    }

    [Test]
    public async Task ShowsCompletionItems()
    {
        await WaitForEditor( "#code-editor-completion" );
        await Page.Locator( "#code-editor-completion" ).ClickAsync();
        await Page.Keyboard.PressAsync( "Control+Space" );

        ILocator suggestions = Page.Locator( ".suggest-widget" );
        await Expect( suggestions ).ToBeVisibleAsync();
        await Expect( suggestions.GetByText( "Customer.Name", new() { Exact = true } ) ).ToBeVisibleAsync();
    }

    [Test]
    public async Task ReadsDiagnostics()
    {
        await WaitForEditor( "#code-editor-diagnostics" );
        await Page.Locator( "#code-editor-read-diagnostics" ).ClickAsync();

        await Expect( Page.Locator( "#code-editor-diagnostics-result" ) ).ToHaveTextAsync( "1" );
    }

    [Test]
    public async Task FlushesPendingValueWhenDisposed()
    {
        await WaitForEditor( "#code-editor-disposable" );
        await ReplaceEditorValue( "#code-editor-disposable", "pending value" );
        await Expect( Page.Locator( "#code-editor-disposable-result" ) ).ToHaveTextAsync( string.Empty );

        await Page.Locator( "#code-editor-remove-disposable" ).EvaluateAsync( "button => button.click()" );

        await Expect( Page.Locator( "#code-editor-disposable" ) ).ToHaveCountAsync( 0 );
        await Expect( Page.Locator( "#code-editor-disposable-result" ) ).ToHaveTextAsync( "pending value" );
    }

    [Test]
    public async Task MultipleEditorsRemainIndependent()
    {
        await WaitForEditor( "#code-editor-first" );
        await WaitForEditor( "#code-editor-second" );
        await ReplaceEditorValue( "#code-editor-first", "first value" );

        await Page.Locator( "#code-editor-remove-first" ).ClickAsync();
        await Expect( Page.Locator( "#code-editor-first" ) ).ToHaveCountAsync( 0 );

        await ReplaceEditorValue( "#code-editor-second", "second value" );
        await Expect( Page.Locator( "#code-editor-second-result" ) ).ToHaveTextAsync( "second value" );
    }

    private async Task WaitForEditor( string selector )
    {
        await Page.Locator( selector ).Locator( ".monaco-editor" ).WaitForAsync(
            new LocatorWaitForOptions { State = WaitForSelectorState.Visible } );
    }

    private async Task ReplaceEditorValue( string selector, string value )
    {
        ILocator editor = Page.Locator( selector );

        await WaitForEditor( selector );
        await editor.ClickAsync();
        await Page.Keyboard.PressAsync( "Control+A" );
        await Page.Keyboard.InsertTextAsync( value );
    }
}