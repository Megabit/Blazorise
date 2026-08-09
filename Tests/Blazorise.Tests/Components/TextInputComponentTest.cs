using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Bunit;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Blazorise.Tests.Components;

public class TextInputComponentTest : BunitContext
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
        var comp = Render<TextInputComponent>();
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
        var comp = Render<TextInputComponent>();
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
        var comp = Render<TextInputComponent>();
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
        var comp = Render<TextInput>( parameters => parameters
            .Add( p => p.AriaInvalid, "true" )
            .Add( p => p.AriaRequired, true )
            .Add( p => p.AriaDescribedBy, "text-help" )
            .Add( p => p.AriaLabelledBy, "text-label" ) );

        // test
        var input = comp.Find( "input" );

        // validate
        Assert.Equal( "true", input.GetAttribute( "aria-invalid" ) );
        Assert.Equal( "true", input.GetAttribute( "aria-required" ) );
        Assert.Equal( "text-help", input.GetAttribute( "aria-describedby" ) );
        Assert.Equal( "text-label", input.GetAttribute( "aria-labelledby" ) );
    }

    [Fact]
    public void AutoAriaRequired_IsRendered_ForRequiredFields()
    {
        var model = new RequiredTextModel();
        var comp = RenderRequiredTextInput( model );

        var input = comp.Find( "input" );

        Assert.Equal( "true", input.GetAttribute( "aria-required" ) );
    }

    [Fact]
    public void ExplicitAriaRequired_False_OverridesAutomaticRequiredState()
    {
        var model = new RequiredTextModel();
        var comp = Render<Validations>( parameters => parameters
            .Add( p => p.Model, model )
            .Add( p => p.ChildContent, (RenderFragment)( builder =>
            {
                builder.OpenComponent<Validation>( 0 );
                builder.AddAttribute( 1, nameof( Validation.ChildContent ), (RenderFragment)( childBuilder =>
                {
                    childBuilder.OpenComponent<TextInput>( 0 );
                    childBuilder.AddAttribute( 1, nameof( TextInput.Value ), model.Name );
                    childBuilder.AddAttribute( 2, nameof( TextInput.ValueChanged ), EventCallback.Factory.Create<string>( this, value => model.Name = value ) );
                    childBuilder.AddAttribute( 3, nameof( TextInput.ValueExpression ), (Expression<Func<string>>)( () => model.Name ) );
                    childBuilder.AddAttribute( 4, nameof( TextInput.AriaRequired ), false );
                    childBuilder.CloseComponent();
                } ) );
                builder.CloseComponent();
            } ) ) );

        var input = comp.Find( "input" );

        Assert.Equal( "false", input.GetAttribute( "aria-required" ) );
    }

    [Fact]
    public void ValidationError_RemainsMounted_BeforeErrorState()
    {
        var comp = Render<Validation>( parameters => parameters
            .Add( p => p.Validator, ValidationRule.IsNotEmpty )
            .Add( p => p.Status, ValidationStatus.None )
            .Add( p => p.ChildContent, (RenderFragment)( builder =>
            {
                builder.OpenComponent<Field>( 0 );
                builder.AddAttribute( 1, nameof( Field.ChildContent ), (RenderFragment)( fieldBuilder =>
                {
                    fieldBuilder.OpenComponent<TextInput>( 0 );
                    fieldBuilder.CloseComponent();

                    fieldBuilder.OpenComponent<ValidationError>( 1 );
                    fieldBuilder.CloseComponent();
                } ) );
                builder.CloseComponent();
            } ) ) );

        var validationError = comp.Find( ".invalid-feedback" );

        Assert.Equal( string.Empty, validationError.TextContent.Trim() );
    }

    [Theory]
    [InlineData( ValidationMode.Auto )]
    [InlineData( ValidationMode.Manual )]
    public async Task ValidationState_IsCleared_WhenInputBecomesDisabled( ValidationMode mode )
    {
        var disabled = false;
        var validationCount = 0;

        Action<ValidatorEventArgs> validator = eventArgs =>
        {
            validationCount++;
            ValidationRule.IsNotEmpty( eventArgs );
        };

        RenderFragment childContent = builder =>
        {
            builder.OpenComponent<Validation>( 0 );
            builder.AddAttribute( 1, nameof( Validation.Validator ), validator );
            builder.AddAttribute( 2, nameof( Validation.ChildContent ), (RenderFragment)( childBuilder =>
            {
                childBuilder.OpenComponent<TextInput>( 0 );
                childBuilder.AddAttribute( 1, nameof( TextInput.Value ), string.Empty );
                childBuilder.AddAttribute( 2, nameof( TextInput.Disabled ), disabled );
                childBuilder.CloseComponent();
            } ) );
            builder.CloseComponent();
        };

        var comp = Render<Validations>( parameters => parameters
            .Add( p => p.Mode, mode )
            .Add( p => p.ChildContent, childContent ) );

        Validation validation = comp.FindComponent<Validation>().Instance;

        if ( mode == ValidationMode.Manual )
            Assert.False( await comp.Instance.ValidateAll() );

        Assert.Equal( ValidationStatus.Error, validation.Status );
        Assert.Equal( 1, validationCount );

        disabled = true;
        comp.Render( parameters => parameters
            .Add( p => p.Mode, mode )
            .Add( p => p.ChildContent, childContent ) );

        comp.WaitForAssertion( () =>
        {
            Assert.Equal( ValidationStatus.None, validation.Status );
            Assert.DoesNotContain( "is-invalid", comp.Find( "input" ).ClassList );
            Assert.Equal( 1, validationCount );
        } );

        Assert.True( await comp.Instance.ValidateAll() );
        Assert.Equal( ValidationStatus.None, validation.Status );
        Assert.Equal( 1, validationCount );

        disabled = false;
        comp.Render( parameters => parameters
            .Add( p => p.Mode, mode )
            .Add( p => p.ChildContent, childContent ) );

        if ( mode == ValidationMode.Manual )
        {
            Assert.Equal( ValidationStatus.None, validation.Status );
            Assert.Equal( 1, validationCount );
            Assert.False( await comp.Instance.ValidateAll() );
        }

        comp.WaitForAssertion( () =>
        {
            Assert.Equal( ValidationStatus.Error, validation.Status );
            Assert.Contains( "is-invalid", comp.Find( "input" ).ClassList );
            Assert.Equal( 2, validationCount );
        } );
    }

    private IRenderedComponent<Validations> RenderRequiredTextInput( RequiredTextModel model )
    {
        return Render<Validations>( parameters => parameters
            .Add( p => p.Model, model )
            .Add( p => p.ChildContent, (RenderFragment)( builder =>
            {
                builder.OpenComponent<Validation>( 0 );
                builder.AddAttribute( 1, nameof( Validation.ChildContent ), (RenderFragment)( childBuilder =>
                {
                    childBuilder.OpenComponent<TextInput>( 0 );
                    childBuilder.AddAttribute( 1, nameof( TextInput.Value ), model.Name );
                    childBuilder.AddAttribute( 2, nameof( TextInput.ValueChanged ), EventCallback.Factory.Create<string>( this, value => model.Name = value ) );
                    childBuilder.AddAttribute( 3, nameof( TextInput.ValueExpression ), (Expression<Func<string>>)( () => model.Name ) );
                    childBuilder.CloseComponent();
                } ) );
                builder.CloseComponent();
            } ) ) );
    }
}

public class TextInputAccessibilityOptionsComponentTest : BunitContext
{
    public TextInputAccessibilityOptionsComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        Services.AddSingleton( serviceProvider => new BlazoriseOptions( serviceProvider, options =>
        {
            options.AccessibilityOptions.UseAutoAriaInvalidAttribute = false;
            options.AccessibilityOptions.UseAutoAriaRequiredAttribute = false;
            options.AccessibilityOptions.UseAutoAriaDescribedByAttribute = false;
        } ) );
        JSInterop.AddBlazoriseTextInput();
    }

    [Fact]
    public void ExplicitAriaAttributes_AreRendered_WhenAutoOptionsAreDisabled()
    {
        var comp = Render<TextInput>( parameters => parameters
            .Add( p => p.AriaInvalid, "true" )
            .Add( p => p.AriaRequired, true )
            .Add( p => p.AriaDescribedBy, "text-help" )
            .Add( p => p.AriaLabelledBy, "text-label" ) );

        var input = comp.Find( "input" );

        Assert.Equal( "true", input.GetAttribute( "aria-invalid" ) );
        Assert.Equal( "true", input.GetAttribute( "aria-required" ) );
        Assert.Equal( "text-help", input.GetAttribute( "aria-describedby" ) );
        Assert.Equal( "text-label", input.GetAttribute( "aria-labelledby" ) );
    }

    [Fact]
    public void AutoAriaAttributes_AreNotRendered_WhenDisabled()
    {
        var comp = Render<Validation>( parameters => parameters
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
            Assert.Null( input.GetAttribute( "aria-required" ) );
            Assert.Null( input.GetAttribute( "aria-describedby" ) );
        } );
    }

    [Fact]
    public void AutoAriaRequired_IsNotRendered_WhenDisabled()
    {
        var model = new RequiredTextModel();
        var comp = Render<Validations>( parameters => parameters
            .Add( p => p.Model, model )
            .Add( p => p.ChildContent, (RenderFragment)( builder =>
            {
                builder.OpenComponent<Validation>( 0 );
                builder.AddAttribute( 1, nameof( Validation.ChildContent ), (RenderFragment)( childBuilder =>
                {
                    childBuilder.OpenComponent<TextInput>( 0 );
                    childBuilder.AddAttribute( 1, nameof( TextInput.Value ), model.Name );
                    childBuilder.AddAttribute( 2, nameof( TextInput.ValueChanged ), EventCallback.Factory.Create<string>( this, value => model.Name = value ) );
                    childBuilder.AddAttribute( 3, nameof( TextInput.ValueExpression ), (Expression<Func<string>>)( () => model.Name ) );
                    childBuilder.CloseComponent();
                } ) );
                builder.CloseComponent();
            } ) ) );

        var input = comp.Find( "input" );

        Assert.Null( input.GetAttribute( "aria-required" ) );
    }
}

public class RequiredTextModel
{
    [Required]
    public string Name { get; set; }
}