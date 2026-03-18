using System.Threading.Tasks;
using Bunit;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Blazorise.Tests.Components;

public class TextInputComponentTest : TestContext
{
    public TextInputComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop.AddBlazoriseTextInput();
    }

    [Fact]
    public async Task CanChangeText()
    {
        // setup
        var comp = RenderComponent<TextInputComponent>();
        var paragraph = comp.Find( "#text-basic" );
        var text = comp.Find( "input" );

        Assert.Null( text.GetAttribute( "value" ) );

        // test
        await text.InputAsync( new Microsoft.AspNetCore.Components.ChangeEventArgs() { Value = "abc" } );

        // validate
        Assert.Contains( "abc", text.GetAttribute( "value" ) );
    }

    [Fact]
    public async Task CanChangeTextUsingEvent()
    {
        // setup
        var comp = RenderComponent<TextInputComponent>();
        var paragraph = comp.Find( "#text-event-initially-blank" );
        var text = comp.Find( "#text-with-event" );
        var result = comp.Find( "#text-event-initially-blank-result" );

        Assert.Equal( string.Empty, result.InnerHtml );

        // test initial
        await text.InputAsync( new Microsoft.AspNetCore.Components.ChangeEventArgs() { Value = "abcde" } );
        Assert.Equal( "abcde", result.InnerHtml );

        // test additional text
        await text.InputAsync( new Microsoft.AspNetCore.Components.ChangeEventArgs() { Value = "abcdefghijklmnopqrstuvwxyz" } );
        Assert.Equal( "abcdefghijklmnopqrstuvwxyz", result.InnerHtml );

        // text backspace.
        // todo: figure out how to set special keys.
        // text.KeyPress( "Keys.Backspace" );
        // Assert.Equal( "abcdefghijklmnopqrstuvwxy", result.InnerHtml );
    }

    [Fact]
    public async Task CanChangeTextUsingBind()
    {
        // setup
        var comp = RenderComponent<TextInputComponent>();
        var paragraph = comp.Find( "#text-bind-initially-blank" );
        var text = comp.Find( "#text-binding" );
        var result = comp.Find( "#text-bind-initially-blank-result" );

        Assert.Equal( string.Empty, result.InnerHtml );

        // test additional text
        await text.InputAsync( new Microsoft.AspNetCore.Components.ChangeEventArgs() { Value = "abcdefghijklmnopqrstuvwxyz" } );
        Assert.Equal( "abcdefghijklmnopqrstuvwxyz", result.InnerHtml );

        // text backspace.
        // todo: figure out how to set special keys.
        // text.KeyPress( "Keys.Backspace" );
        // Assert.Equal( "abcdefghijklmnopqrstuvwxy", result.InnerHtml );
    }

    [Fact]
    public void AriaAttributes_AreRenderedFromParameters()
    {
        // setup
        var comp = RenderComponent<TextInput>( parameters => parameters
            .Add( p => p.AriaInvalid, "true" )
            .Add( p => p.AriaDescribedBy, "text-help" )
            .Add( p => p.AriaLabelledBy, "text-label" ) );

        // test
        var input = comp.Find( "input" );

        // validate
        Assert.Equal( "true", input.GetAttribute( "aria-invalid" ) );
        Assert.Equal( "text-help", input.GetAttribute( "aria-describedby" ) );
        Assert.Equal( "text-label", input.GetAttribute( "aria-labelledby" ) );
    }
}

public class TextInputAccessibilityOptionsComponentTest : TestContext
{
    public TextInputAccessibilityOptionsComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        Services.AddSingleton( serviceProvider => new BlazoriseOptions( serviceProvider, options =>
        {
            options.AccessibilityOptions.UseAutoAriaInvalidAttribute = false;
            options.AccessibilityOptions.UseAutoAriaDescribedByAttribute = false;
        } ) );
        JSInterop.AddBlazoriseTextInput();
    }

    [Fact]
    public void ExplicitAriaAttributes_AreRendered_WhenAutoOptionsAreDisabled()
    {
        var comp = RenderComponent<TextInput>( parameters => parameters
            .Add( p => p.AriaInvalid, "true" )
            .Add( p => p.AriaDescribedBy, "text-help" )
            .Add( p => p.AriaLabelledBy, "text-label" ) );

        var input = comp.Find( "input" );

        Assert.Equal( "true", input.GetAttribute( "aria-invalid" ) );
        Assert.Equal( "text-help", input.GetAttribute( "aria-describedby" ) );
        Assert.Equal( "text-label", input.GetAttribute( "aria-labelledby" ) );
    }

    [Fact]
    public void AutoAriaAttributes_AreNotRendered_WhenDisabled()
    {
        var comp = RenderComponent<Validation>( parameters => parameters
            .Add( p => p.Validator, ValidationRule.IsNotEmpty )
            .Add( p => p.Status, ValidationStatus.Error )
            .Add( p => p.ChildContent, (RenderFragment)( builder =>
            {
                builder.OpenComponent<Field>( 0 );
                builder.AddAttribute( 1, nameof( Field.ChildContent ), (RenderFragment)( fieldBuilder =>
                {
                    fieldBuilder.OpenComponent<TextInput>( 0 );
                    fieldBuilder.CloseComponent();

                    fieldBuilder.OpenComponent<FieldHelp>( 1 );
                    fieldBuilder.AddAttribute( 2, nameof( FieldHelp.ChildContent ), (RenderFragment)( helpBuilder => helpBuilder.AddContent( 0, "Help" ) ) );
                    fieldBuilder.CloseComponent();

                    fieldBuilder.OpenComponent<ValidationError>( 3 );
                    fieldBuilder.CloseComponent();
                } ) );
                builder.CloseComponent();
            } ) ) );

        comp.WaitForAssertion( () =>
        {
            var input = comp.Find( "input" );

            Assert.Null( input.GetAttribute( "aria-invalid" ) );
            Assert.Null( input.GetAttribute( "aria-describedby" ) );
        } );
    }
}