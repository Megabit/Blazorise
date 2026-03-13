using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Blazorise.Tests.Components;

public class FieldSetComponentTest : TestContext
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
        var cut = RenderComponent<FieldSet>( parameters => parameters
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
    public void Legend_Should_LabelRadioGroupWithAriaLabelledBy()
    {
        var cut = RenderComponent<FieldSet>( parameters => parameters
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

            Assert.NotNull( legend.GetAttribute( "id" ) );
            Assert.Equal( legend.GetAttribute( "id" ), radioGroup.GetAttribute( "aria-labelledby" ) );
        } );
    }

    [Fact]
    public void Legend_Should_NotOverrideExplicitAriaLabelledBy()
    {
        var cut = RenderComponent<FieldSet>( parameters => parameters
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
            var legend = cut.Find( "legend" );
            var radioGroup = cut.Find( "#options-group" );

            Assert.NotNull( legend.GetAttribute( "id" ) );
            Assert.Equal( "custom-legend-id", radioGroup.GetAttribute( "aria-labelledby" ) );
            Assert.NotEqual( legend.GetAttribute( "id" ), radioGroup.GetAttribute( "aria-labelledby" ) );
        } );
    }
}