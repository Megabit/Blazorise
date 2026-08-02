#region Using directives
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.CodeEditor;
using Bunit;
using CodeEditorComponent = Blazorise.CodeEditor.CodeEditor;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class CodeEditorComponentTest : BunitContext
{
    public CodeEditorComponentTest()
    {
        Services.AddBlazoriseTests()
            .AddBootstrapProviders()
            .AddEmptyIconProvider()
            .AddTestData()
            .AddBlazoriseCodeEditor();

        JSInterop.AddBlazoriseCodeEditor();
    }

    [Fact]
    public void Initialize_Should_MapOptionsAndAccessibility()
    {
        CodeEditorReadyEventArgs readyEventArgs = null;
        CodeEditorOptions options = new()
        {
            Minimap = false,
            TabSize = 2,
            WordWrap = true
        };

        IRenderedComponent<CodeEditorComponent> cut = Render<CodeEditorComponent>( parameters => parameters
            .Add( component => component.Value, "const answer = 42;" )
            .Add( component => component.Language, CodeEditorLanguage.JavaScript )
            .Add( component => component.EditorOptions, options )
            .Add( component => component.Debounce, true )
            .Add( component => component.DebounceInterval, 125 )
            .Add( component => component.Disabled, true )
            .Add( component => component.Ready, eventArgs => readyEventArgs = eventArgs )
            .Add( component => component.Attributes, new Dictionary<string, object> { ["aria-label"] = "Source code" } ) );

        JSRuntimeInvocation invocation = JSInterop.VerifyInvoke( "initialize" );
        CodeEditorJSOptions jsOptions = Assert.IsType<CodeEditorJSOptions>( invocation.Arguments[3] );

        Assert.Equal( "const answer = 42;", jsOptions.Value );
        Assert.Equal( CodeEditorLanguage.JavaScript, jsOptions.Language );
        Assert.False( jsOptions.Minimap );
        Assert.Equal( 2, jsOptions.TabSize );
        Assert.True( jsOptions.WordWrap );
        Assert.True( jsOptions.Immediate );
        Assert.True( jsOptions.Debounce );
        Assert.Equal( 125, jsOptions.DebounceInterval );
        Assert.True( jsOptions.Disabled );
        Assert.Equal( -1, jsOptions.TabIndex );
        Assert.Same( cut.Instance, readyEventArgs.Editor );
        Assert.Equal( "true", cut.Find( ".b-code-editor" ).GetAttribute( "aria-disabled" ) );
    }

    [Fact]
    public void ParameterChanges_Should_InvokeTargetedJavaScriptUpdates()
    {
        CodeEditorOptions initialOptions = new();
        IRenderedComponent<CodeEditorComponent> cut = Render<CodeEditorComponent>( parameters => parameters
            .Add( component => component.EditorOptions, initialOptions )
            .Add( component => component.Language, CodeEditorLanguage.CSharp ) );

        cut.Render( parameters => parameters
            .Add( component => component.EditorOptions, new CodeEditorOptions { Minimap = false } )
            .Add( component => component.Language, CodeEditorLanguage.Json ) );

        JSInterop.VerifyInvoke( "updateOptions" );
        JSInterop.VerifyInvoke( "setLanguage" );
        JSInterop.VerifyInvoke( "setCompletionProvider" );
    }

    [Fact]
    public async Task GetDiagnostics_Should_InvokeJavaScript()
    {
        IRenderedComponent<CodeEditorComponent> cut = Render<CodeEditorComponent>();

        IReadOnlyList<CodeEditorDiagnostic> diagnostics = await cut.Instance.GetDiagnostics();

        Assert.Empty( diagnostics );
        JSInterop.VerifyInvoke( "getDiagnostics" );
    }

    [Fact]
    public async Task FormattingProvider_Should_MapAndInvokeFormatter()
    {
        CodeEditorDocumentFormattingProvider provider = new()
        {
            Language = CodeEditorLanguage.CSharp,
            Formatter = value => Task.FromResult( value.ToUpperInvariant() )
        };
        IRenderedComponent<CodeEditorComponent> cut = Render<CodeEditorComponent>( parameters => parameters
            .Add( component => component.Language, CodeEditorLanguage.CSharp )
            .Add( component => component.FormattingProvider, provider ) );

        JSRuntimeInvocation invocation = JSInterop.VerifyInvoke( "initialize" );
        CodeEditorJSOptions jsOptions = Assert.IsType<CodeEditorJSOptions>( invocation.Arguments[3] );

        Assert.Equal( CodeEditorLanguage.CSharp, jsOptions.FormattingProvider.Language );
        Assert.True( jsOptions.FormattingProvider.UseFormatter );
        Assert.Equal( "FORMATTED", await cut.Instance.NotifyDocumentFormatting( "formatted" ) );
        Assert.True( await cut.Instance.FormatDocumentAsync() );
    }

    [Fact]
    public async Task CompletionProvider_Should_MapAndInvokeItemsProvider()
    {
        CodeEditorCompletionContext receivedContext = null;
        IReadOnlyList<CodeEditorCompletionItem> expectedItems =
        [
            new()
            {
                Label = "Customer.Name",
                InsertText = "{Customer.Name}",
                Kind = CodeEditorCompletionItemKind.Field,
                Range = new()
                {
                    StartLineNumber = 1,
                    StartColumn = 1,
                    EndLineNumber = 1,
                    EndColumn = 10,
                },
            },
        ];
        CodeEditorCompletionProvider provider = new()
        {
            Language = "formula",
            TriggerCharacters = ["{"],
            ItemsProvider = context =>
            {
                receivedContext = context;

                return Task.FromResult( expectedItems );
            },
        };
        IRenderedComponent<CodeEditorComponent> cut = Render<CodeEditorComponent>( parameters => parameters
            .Add( component => component.Language, "formula" )
            .Add( component => component.CompletionProvider, provider ) );

        JSRuntimeInvocation invocation = JSInterop.VerifyInvoke( "initialize" );
        CodeEditorJSOptions jsOptions = Assert.IsType<CodeEditorJSOptions>( invocation.Arguments[3] );
        CodeEditorCompletionContext context = new()
        {
            Value = "{Customer",
            LineText = "{Customer",
            LineNumber = 1,
            Column = 10,
            Word = "Customer",
            TriggerCharacter = "{",
        };

        IReadOnlyList<CodeEditorCompletionItem> items = await cut.Instance.NotifyCompletion( context );

        Assert.True( jsOptions.CompletionProvider.UseItemsProvider );
        Assert.Same( context, receivedContext );
        Assert.Same( expectedItems, items );
        Assert.Equal( 1, Assert.Single( items ).Range.StartColumn );
    }

    [Fact]
    public void DeclarativeTokenizer_Should_IncludeNamedStates()
    {
        IRenderedComponent<CodeEditorComponent> cut = Render<CodeEditorComponent>( parameters => parameters
            .Add( component => component.Language, "formula" )
            .AddChildContent( BuildLanguageDefinition() ) );

        JSRuntimeInvocation invocation = JSInterop.VerifyInvoke( "initialize" );
        CodeEditorJSOptions jsOptions = Assert.IsType<CodeEditorJSOptions>( invocation.Arguments[3] );
        CodeEditorLanguageDefinition language = Assert.Single( jsOptions.Languages.Where( item => item.Id == "formula" ) );

        Assert.NotNull( language.Tokenizer );
        Assert.Single( language.Tokenizer.Tokens );
        Assert.True( language.Tokenizer.States.ContainsKey( "string" ) );
        Assert.Single( language.Tokenizer.States["string"] );
    }

    [Theory]
    [InlineData( CodeEditorCompletionItemKind.Method, 0 )]
    [InlineData( CodeEditorCompletionItemKind.Text, 18 )]
    [InlineData( CodeEditorCompletionItemKind.Snippet, 27 )]
    public void CompletionItemKind_Should_MatchMonacoValues( CodeEditorCompletionItemKind kind, int expected )
    {
        Assert.Equal( expected, (int)kind );
    }

    private static RenderFragment BuildLanguageDefinition()
    {
        return builder =>
        {
            builder.OpenComponent<CodeEditorCustomLanguage>( 0 );
            builder.AddAttribute( 1, nameof( CodeEditorCustomLanguage.Id ), "formula" );
            builder.AddAttribute( 2, nameof( CodeEditorCustomLanguage.ChildContent ), (RenderFragment)( languageBuilder =>
            {
                languageBuilder.OpenComponent<CodeEditorTokenizer>( 0 );
                languageBuilder.AddAttribute( 1, nameof( CodeEditorTokenizer.ChildContent ), (RenderFragment)( tokenizerBuilder =>
                {
                    tokenizerBuilder.OpenComponent<CodeEditorToken>( 0 );
                    tokenizerBuilder.AddAttribute( 1, nameof( CodeEditorToken.Pattern ), "\"" );
                    tokenizerBuilder.AddAttribute( 2, nameof( CodeEditorToken.Token ), "string.quote" );
                    tokenizerBuilder.AddAttribute( 3, nameof( CodeEditorToken.Next ), "@string" );
                    tokenizerBuilder.CloseComponent();

                    tokenizerBuilder.OpenComponent<CodeEditorTokenizerState>( 4 );
                    tokenizerBuilder.AddAttribute( 5, nameof( CodeEditorTokenizerState.Name ), "string" );
                    tokenizerBuilder.AddAttribute( 6, nameof( CodeEditorTokenizerState.ChildContent ), (RenderFragment)( stateBuilder =>
                    {
                        stateBuilder.OpenComponent<CodeEditorToken>( 0 );
                        stateBuilder.AddAttribute( 1, nameof( CodeEditorToken.Pattern ), "\"" );
                        stateBuilder.AddAttribute( 2, nameof( CodeEditorToken.Token ), "string.quote" );
                        stateBuilder.AddAttribute( 3, nameof( CodeEditorToken.Next ), "@pop" );
                        stateBuilder.CloseComponent();
                    } ) );
                    tokenizerBuilder.CloseComponent();
                } ) );
                languageBuilder.CloseComponent();
            } ) );
            builder.CloseComponent();
        };
    }
}