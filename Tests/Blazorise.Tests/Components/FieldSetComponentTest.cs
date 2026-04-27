using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Blazorise.Tests.Components;

public class FieldSetComponentTest : BunitContext
{
    public FieldSetComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        Services.AddSingleton( serviceProvider => new BlazoriseOptions( serviceProvider, options =>
        {
            options.AccessibilityOptions.UseAriaLabelledByAttribute = true;
        } ) );
        JSInterop.AddBlazoriseUtilities();
    }

    [Fact]
    public void FieldSet_Should_RenderSemanticElements()
    {
        var cut = Render<FieldSet>( parameters => parameters
            .AddChildContent( builder =>
            {
                builder.OpenComponent<Legend>( 0 );
                builder.AddAttribute( 1, nameof( Legend.ChildContent ), (RenderFragment)( childBuilder => childBuilder.AddContent( 0, "Contact preferences" ) ) );
                builder.CloseComponent();
            } ) );

        var fieldSet = cut.Find( "fieldset" );
        var legend = cut.Find( "legend" );

        Assert.NotNull( fieldSet );
        Assert.Equal( "Contact preferences", legend.TextContent.Trim() );
    }

    [Fact]
    public void FieldSet_Horizontal_Should_RenderProviderClass()
    {
        var cut = Render<FieldSet>( parameters => parameters
            .Add( x => x.Horizontal, true ) );

        var fieldSet = cut.Find( "fieldset" );

        Assert.Contains( "row", fieldSet.ClassList );
    }

    [Fact]
    public void Legend_RequiredIndicator_Should_RenderProviderClass()
    {
        var cut = Render<Legend>( parameters => parameters
            .Add( x => x.RequiredIndicator, true )
            .AddChildContent( "Contact preferences" ) );

        var legend = cut.Find( "legend" );

        Assert.Contains( "form-label-required", legend.ClassList );
    }

    [Fact]
    public void Legend_Screenreader_Should_RenderProviderClass()
    {
        var cut = Render<Legend>( parameters => parameters
            .Add( x => x.Screenreader, Screenreader.Only )
            .AddChildContent( "Contact preferences" ) );

        var legend = cut.Find( "legend" );

        Assert.Contains( "sr-only", legend.ClassList );
    }

    [Fact]
    public void Legend_Should_Not_Automatically_LabelRadioGroup()
    {
        var cut = Render<FieldSet>( parameters => parameters
            .AddChildContent( builder =>
            {
                builder.OpenComponent<Legend>( 0 );
                builder.AddAttribute( 1, nameof( Legend.ChildContent ), (RenderFragment)( childBuilder => childBuilder.AddContent( 0, "Options" ) ) );
                builder.CloseComponent();

                builder.OpenComponent<RadioGroup<string>>( 2 );
                builder.AddAttribute( 3, nameof( RadioGroup<string>.ElementId ), "options-group" );
                builder.CloseComponent();
            } ) );

        cut.WaitForAssertion( () =>
        {
            var legend = cut.Find( "legend" );
            var radioGroup = cut.Find( "#options-group" );

            Assert.Null( legend.GetAttribute( "id" ) );
            Assert.Null( radioGroup.GetAttribute( "aria-labelledby" ) );
        } );
    }

    [Fact]
    public void Legend_Should_Not_Interfere_With_ExplicitAriaLabelledBy()
    {
        var cut = Render<FieldSet>( parameters => parameters
            .AddChildContent( builder =>
            {
                builder.OpenComponent<Legend>( 0 );
                builder.AddAttribute( 1, nameof( Legend.ChildContent ), (RenderFragment)( childBuilder => childBuilder.AddContent( 0, "Options" ) ) );
                builder.CloseComponent();

                builder.OpenComponent<RadioGroup<string>>( 2 );
                builder.AddAttribute( 3, nameof( RadioGroup<string>.ElementId ), "options-group" );
                builder.AddAttribute( 4, nameof( RadioGroup<string>.AriaLabelledBy ), "custom-legend-id" );
                builder.CloseComponent();
            } ) );

        cut.WaitForAssertion( () =>
        {
            var radioGroup = cut.Find( "#options-group" );

            Assert.Equal( "custom-legend-id", radioGroup.GetAttribute( "aria-labelledby" ) );
        } );
    }
}