#region Using directives
using System;
using System.Collections.Generic;
using Blazorise.Components;
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class FieldLabelComponentTest : TestContext
{
    public FieldLabelComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop
            .AddBlazoriseTextInput()
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
                builder.AddChildContent( 1, "First Name" );
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
                builder.AddChildContent( 2, "Email" );
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
                builder.AddChildContent( 1, "Country" );
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
}