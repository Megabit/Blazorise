using System;
using Blazorise;
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace Blazorise.Tests.Components;

public class AddonsComponentTest : BunitContext
{
    public AddonsComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop.AddBlazoriseTextInput();
    }

    [Fact]
    public void ValidationFeedback_IsRenderedAfterAddons_WithAggregateStatus()
    {
        var comp = Render<Addons>( parameters => parameters
            .Add( p => p.ChildContent, (RenderFragment)( builder =>
            {
                builder.OpenComponent<Addon>( 0 );
                builder.AddAttribute( 1, nameof( Addon.AddonType ), AddonType.Body );
                builder.AddAttribute( 2, nameof( Addon.ChildContent ), CreateValidatedInput( ValidationStatus.Error, "First error" ) );
                builder.CloseComponent();

                builder.OpenComponent<Addon>( 3 );
                builder.AddAttribute( 4, nameof( Addon.AddonType ), AddonType.Body );
                builder.AddAttribute( 5, nameof( Addon.ChildContent ), CreateValidatedInput( ValidationStatus.Warning, "Second warning" ) );
                builder.CloseComponent();

                builder.OpenComponent<Addon>( 6 );
                builder.AddAttribute( 7, nameof( Addon.AddonType ), AddonType.Body );
                builder.AddAttribute( 8, nameof( Addon.ChildContent ), CreateValidatedInput( ValidationStatus.Success, "Third success" ) );
                builder.CloseComponent();

                builder.OpenComponent<Addon>( 9 );
                builder.AddAttribute( 10, nameof( Addon.AddonType ), AddonType.End );
                builder.AddAttribute( 11, nameof( Addon.ChildContent ), (RenderFragment)( addonBuilder => addonBuilder.AddContent( 0, "End" ) ) );
                builder.CloseComponent();
            } ) ) );

        comp.WaitForAssertion( () =>
        {
            var addons = comp.Find( ".input-group" );
            var error = comp.Find( ".input-group + .input-group-validation-feedback > [data-validation-status='error'] > .invalid-feedback" );
            var warning = comp.Find( ".input-group + .input-group-validation-feedback > [data-validation-status='warning'] > .warning-feedback" );
            var success = comp.Find( ".input-group + .input-group-validation-feedback > [data-validation-status='success'] > .valid-feedback" );

            Assert.Contains( "is-invalid", addons.ClassList );
            Assert.Contains( "input-group-append", addons.LastElementChild.ClassList );
            Assert.Empty( addons.QuerySelectorAll( ".invalid-feedback" ) );
            Assert.Equal( "First error", error.TextContent.Trim() );
            Assert.Equal( "Second warning", warning.TextContent.Trim() );
            Assert.Equal( "Third success", success.TextContent.Trim() );
        } );
    }

    private static RenderFragment CreateValidatedInput( ValidationStatus status, string message ) => builder =>
    {
        builder.OpenComponent<Validation>( 0 );
        builder.AddAttribute( 1, nameof( Validation.Validator ), (Action<ValidatorEventArgs>)( eventArgs => eventArgs.Status = status ) );
        builder.AddAttribute( 2, nameof( Validation.ChildContent ), (RenderFragment)( validationBuilder =>
        {
            validationBuilder.OpenComponent<TextInput>( 0 );
            validationBuilder.AddAttribute( 1, nameof( TextInput.Feedback ), (RenderFragment)( feedbackBuilder =>
            {
                if ( status == ValidationStatus.Error )
                {
                    feedbackBuilder.OpenComponent<ValidationError>( 0 );
                }
                else if ( status == ValidationStatus.Warning )
                {
                    feedbackBuilder.OpenComponent<ValidationWarning>( 0 );
                }
                else
                {
                    feedbackBuilder.OpenComponent<ValidationSuccess>( 0 );
                }

                feedbackBuilder.AddAttribute( 1, nameof( ValidationError.ChildContent ), (RenderFragment)( messageBuilder => messageBuilder.AddContent( 0, message ) ) );
                feedbackBuilder.CloseComponent();
            } ) );
            validationBuilder.CloseComponent();
        } ) );
        builder.CloseComponent();
    };
}