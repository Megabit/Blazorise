#region Using directives
using System;
using System.Collections.Generic;
using Blazorise.Components;
using Blazorise.RichTextEdit;
using Bunit;
using MarkdownEditor = Blazorise.Markdown.Markdown;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using RichTextEditComponent = Blazorise.RichTextEdit.RichTextEdit;
using SignaturePadComponent = Blazorise.SignaturePad.SignaturePad;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class FieldLabelComponentTest : TestContext
{
    public FieldLabelComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        Services.AddSingleton( serviceProvider => new BlazoriseOptions( serviceProvider, options =>
        {
            options.AccessibilityOptions.UseLabelForAttribute = true;
            options.AccessibilityOptions.UseAriaLabelledByAttribute = true;
        } ) );
        Services.AddBlazoriseRichTextEdit();
        JSInterop
            .AddBlazoriseTextInput()
            .AddBlazoriseColorPicker()
            .AddBlazoriseMarkdown()
            .AddBlazoriseRichTextEdit()
            .AddBlazoriseSignaturePad()
            .AddBlazoriseUtilities()
            .AddBlazoriseClosable()
            .AddBlazoriseDropdown();
    }

    [Fact]
    public void RequiredIndicator_Should_RenderIndicator()
    {
        // setup

        // test
        var cut = RenderComponent<FieldLabel>( parameters =>
        {
            parameters.Add( x => x.RequiredIndicator, true );
            parameters.AddChildContent( "Required Field" );
        } );

        // validate
        var requiredLabel = cut.WaitForElement( ".form-label-required" );
        requiredLabel.Should().NotBeNull();
    }

    [Fact]
    public void FieldLabel_Should_LinkToNestedTextInput()
    {
        // setup
        var cut = RenderComponent<Field>( parameters => parameters
            .AddChildContent( builder =>
            {
                builder.OpenComponent<FieldLabel>( 0 );
                builder.AddAttribute( 1, nameof( FieldLabel.ChildContent ), (RenderFragment)( childBuilder => childBuilder.AddContent( 0, "First Name" ) ) );
                builder.CloseComponent();

                builder.OpenComponent<FieldBody>( 2 );
                builder.AddAttribute( 3, nameof( FieldBody.ChildContent ), (RenderFragment)( fieldBodyBuilder =>
                {
                    fieldBodyBuilder.OpenComponent<TextInput>( 0 );
                    fieldBodyBuilder.CloseComponent();
                } ) );
                builder.CloseComponent();
            } ) );

        // validate
        cut.WaitForAssertion( () =>
        {
            var label = cut.Find( "label" );
            var input = cut.Find( "input" );

            Assert.Equal( input.GetAttribute( "id" ), label.GetAttribute( "for" ) );
        } );
    }

    [Fact]
    public void FieldLabel_For_Should_OverrideParentFieldTarget()
    {
        // setup
        var cut = RenderComponent<Field>( parameters => parameters
            .AddChildContent( builder =>
            {
                builder.OpenComponent<FieldLabel>( 0 );
                builder.AddAttribute( 1, nameof( FieldLabel.For ), "custom-input-id" );
                builder.AddAttribute( 2, nameof( FieldLabel.ChildContent ), (RenderFragment)( childBuilder => childBuilder.AddContent( 0, "Email" ) ) );
                builder.CloseComponent();

                builder.OpenComponent<TextInput>( 3 );
                builder.CloseComponent();
            } ) );

        // validate
        cut.WaitForAssertion( () =>
        {
            var label = cut.Find( "label" );
            var input = cut.Find( "input" );

            Assert.Equal( "custom-input-id", label.GetAttribute( "for" ) );
            Assert.NotEqual( input.GetAttribute( "id" ), label.GetAttribute( "for" ) );
        } );
    }

    [Fact]
    public void FieldLabel_Should_LinkToAutocompleteInput()
    {
        // setup
        var cut = RenderComponent<Field>( parameters => parameters
            .AddChildContent( builder =>
            {
                builder.OpenComponent<FieldLabel>( 0 );
                builder.AddAttribute( 1, nameof( FieldLabel.ChildContent ), (RenderFragment)( childBuilder => childBuilder.AddContent( 0, "Country" ) ) );
                builder.CloseComponent();

                builder.OpenComponent<Autocomplete<string, string>>( 2 );
                builder.AddAttribute( 3, "Data", new List<string> { "Croatia", "Portugal" } );
                builder.AddAttribute( 4, "TextField", (Func<string, string>)( item => item ) );
                builder.AddAttribute( 5, "ValueField", (Func<string, string>)( item => item ) );
                builder.CloseComponent();
            } ) );

        // validate
        cut.WaitForAssertion( () =>
        {
            var label = cut.Find( "label" );
            var input = cut.Find( ".b-is-autocomplete input" );

            Assert.Equal( input.GetAttribute( "id" ), label.GetAttribute( "for" ) );
        } );
    }

    [Fact]
    public void FieldLabel_Should_LabelColorPickerWithAriaLabelledBy()
    {
        // setup
        var cut = RenderComponent<Field>( parameters => parameters
            .AddChildContent( builder =>
            {
                builder.OpenComponent<FieldLabel>( 0 );
                builder.AddAttribute( 1, nameof( FieldLabel.ChildContent ), (RenderFragment)( childBuilder => childBuilder.AddContent( 0, "Color" ) ) );
                builder.CloseComponent();

                builder.OpenComponent<ColorPicker>( 2 );
                builder.AddAttribute( 3, nameof( ColorPicker.ElementId ), "favorite-color" );
                builder.CloseComponent();
            } ) );

        // validate
        cut.WaitForAssertion( () =>
        {
            var label = cut.Find( "label" );
            var colorPicker = cut.Find( "#favorite-color" );

            Assert.NotNull( label.GetAttribute( "id" ) );
            Assert.Equal( label.GetAttribute( "id" ), colorPicker.GetAttribute( "aria-labelledby" ) );
            Assert.Null( label.GetAttribute( "for" ) );
        } );
    }

    [Fact]
    public void FieldLabel_Should_UseExplicitAriaLabelledBy_WhenProvided()
    {
        var cut = RenderComponent<Field>( parameters => parameters
            .AddChildContent( builder =>
            {
                builder.OpenComponent<FieldLabel>( 0 );
                builder.AddAttribute( 1, nameof( FieldLabel.ChildContent ), (RenderFragment)( childBuilder => childBuilder.AddContent( 0, "Color" ) ) );
                builder.CloseComponent();

                builder.OpenComponent<ColorPicker>( 2 );
                builder.AddAttribute( 3, nameof( ColorPicker.ElementId ), "favorite-color" );
                builder.AddAttribute( 4, nameof( ColorPicker.AriaLabelledBy ), "custom-label-id" );
                builder.CloseComponent();
            } ) );

        cut.WaitForAssertion( () =>
        {
            var label = cut.Find( "label" );
            var colorPicker = cut.Find( "#favorite-color" );

            Assert.NotNull( label.GetAttribute( "id" ) );
            Assert.Equal( "custom-label-id", colorPicker.GetAttribute( "aria-labelledby" ) );
            Assert.NotEqual( label.GetAttribute( "id" ), colorPicker.GetAttribute( "aria-labelledby" ) );
        } );
    }

    [Fact]
    public void FieldLabel_Should_LabelRadioGroupWithAriaLabelledBy()
    {
        // setup
        var cut = RenderComponent<Field>( parameters => parameters
            .AddChildContent( builder =>
            {
                builder.OpenComponent<FieldLabel>( 0 );
                builder.AddAttribute( 1, nameof( FieldLabel.ChildContent ), (RenderFragment)( childBuilder => childBuilder.AddContent( 0, "Options" ) ) );
                builder.CloseComponent();

                builder.OpenComponent<RadioGroup<string>>( 2 );
                builder.AddAttribute( 3, nameof( RadioGroup<string>.ElementId ), "options-group" );
                builder.CloseComponent();
            } ) );

        // validate
        cut.WaitForAssertion( () =>
        {
            var label = cut.Find( "label" );
            var radioGroup = cut.Find( "#options-group" );

            Assert.NotNull( label.GetAttribute( "id" ) );
            Assert.Equal( label.GetAttribute( "id" ), radioGroup.GetAttribute( "aria-labelledby" ) );
            Assert.Null( label.GetAttribute( "for" ) );
        } );
    }

    [Fact]
    public void FieldLabel_Should_LabelRichTextEditWithAriaLabelledBy()
    {
        // setup
        var cut = RenderComponent<Field>( parameters => parameters
            .AddChildContent( builder =>
            {
                builder.OpenComponent<FieldLabel>( 0 );
                builder.AddAttribute( 1, nameof( FieldLabel.ChildContent ), (RenderFragment)( childBuilder => childBuilder.AddContent( 0, "Message" ) ) );
                builder.CloseComponent();

                builder.OpenComponent<RichTextEditComponent>( 2 );
                builder.AddAttribute( 3, nameof( RichTextEditComponent.ElementId ), "message-editor" );
                builder.CloseComponent();
            } ) );

        // validate
        cut.WaitForAssertion( () =>
        {
            var label = cut.Find( "label" );
            var editor = cut.Find( "#message-editor" );

            Assert.NotNull( label.GetAttribute( "id" ) );
            Assert.Equal( label.GetAttribute( "id" ), editor.GetAttribute( "aria-labelledby" ) );
            Assert.Null( label.GetAttribute( "for" ) );
        } );
    }

    [Fact]
    public void FieldLabel_Should_LabelMarkdownWithAriaLabelledBy()
    {
        // setup
        var cut = RenderComponent<Field>( parameters => parameters
            .AddChildContent( builder =>
            {
                builder.OpenComponent<FieldLabel>( 0 );
                builder.AddAttribute( 1, nameof( FieldLabel.ChildContent ), (RenderFragment)( childBuilder => childBuilder.AddContent( 0, "Description" ) ) );
                builder.CloseComponent();

                builder.OpenComponent<MarkdownEditor>( 2 );
                builder.AddAttribute( 3, nameof( MarkdownEditor.ElementId ), "description-editor" );
                builder.CloseComponent();
            } ) );

        // validate
        cut.WaitForAssertion( () =>
        {
            var label = cut.Find( "label" );
            var editor = cut.Find( "#description-editor" );

            Assert.NotNull( label.GetAttribute( "id" ) );
            Assert.Equal( label.GetAttribute( "id" ), editor.GetAttribute( "aria-labelledby" ) );
            Assert.Equal( editor.GetAttribute( "id" ), label.GetAttribute( "for" ) );
        } );
    }

    [Fact]
    public void FieldLabel_Should_LabelSignaturePadWithAriaLabelledBy()
    {
        // setup
        var cut = RenderComponent<Field>( parameters => parameters
            .AddChildContent( builder =>
            {
                builder.OpenComponent<FieldLabel>( 0 );
                builder.AddAttribute( 1, nameof( FieldLabel.ChildContent ), (RenderFragment)( childBuilder => childBuilder.AddContent( 0, "Signature" ) ) );
                builder.CloseComponent();

                builder.OpenComponent<SignaturePadComponent>( 2 );
                builder.AddAttribute( 3, nameof( SignaturePadComponent.ElementId ), "signature-pad" );
                builder.CloseComponent();
            } ) );

        // validate
        cut.WaitForAssertion( () =>
        {
            var label = cut.Find( "label" );
            var signaturePad = cut.Find( "#signature-pad" );

            Assert.NotNull( label.GetAttribute( "id" ) );
            Assert.Equal( label.GetAttribute( "id" ), signaturePad.GetAttribute( "aria-labelledby" ) );
            Assert.Null( label.GetAttribute( "for" ) );
        } );
    }
}

public class FieldLabelAccessibilityOptionsComponentTest : TestContext
{
    public FieldLabelAccessibilityOptionsComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        Services.AddSingleton( serviceProvider => new BlazoriseOptions( serviceProvider, options =>
        {
            options.AccessibilityOptions.UseLabelForAttribute = false;
            options.AccessibilityOptions.UseAriaLabelledByAttribute = false;
            options.AccessibilityOptions.UseAutoAriaInvalidAttribute = false;
            options.AccessibilityOptions.UseAutoAriaDescribedByAttribute = false;
        } ) );
        JSInterop.AddBlazoriseTextInput();
    }

    [Fact]
    public void FieldLabel_Should_Not_LinkToNestedTextInput_WhenAccessibilityOptionsAreDisabled()
    {
        var cut = RenderComponent<Field>( parameters => parameters
            .AddChildContent( builder =>
            {
                builder.OpenComponent<FieldLabel>( 0 );
                builder.AddAttribute( 1, nameof( FieldLabel.ChildContent ), (RenderFragment)( childBuilder => childBuilder.AddContent( 0, "First Name" ) ) );
                builder.CloseComponent();

                builder.OpenComponent<TextInput>( 2 );
                builder.CloseComponent();
            } ) );

        cut.WaitForAssertion( () =>
        {
            var label = cut.Find( "label" );

            Assert.Null( label.GetAttribute( "for" ) );
            Assert.Null( label.GetAttribute( "id" ) );
        } );
    }
}