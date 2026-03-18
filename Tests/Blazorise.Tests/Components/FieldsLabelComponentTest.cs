using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Blazorise.Tests.Components;

public class FieldsLabelComponentTest : TestContext
{
    public FieldsLabelComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        Services.AddSingleton( serviceProvider => new BlazoriseOptions( serviceProvider, options =>
        {
            options.AccessibilityOptions.UseAriaLabelledByAttribute = true;
        } ) );
        JSInterop.AddBlazoriseUtilities();
    }

    [Fact]
    public void GroupedFields_Should_RenderLegendFromFieldsLabel()
    {
        var cut = RenderComponent<Fields>( parameters => parameters
            .Add( p => p.Group, true )
            .AddChildContent( builder =>
            {
                builder.OpenComponent<FieldsLabel>( 0 );
                builder.AddAttribute( 1, nameof( FieldsLabel.ChildContent ), (RenderFragment)( childBuilder => childBuilder.AddContent( 0, "Shipping address" ) ) );
                builder.CloseComponent();
            } ) );

        var fieldSet = cut.Find( "fieldset" );
        var legend = cut.Find( "legend" );

        Assert.NotNull( fieldSet );
        Assert.Equal( "Shipping address", legend.TextContent.Trim() );
    }

    [Fact]
    public void FieldsLabel_Should_LabelDirectChildRadioGroupWithAriaLabelledBy()
    {
        var cut = RenderComponent<Fields>( parameters => parameters
            .Add( p => p.Group, true )
            .AddChildContent( builder =>
            {
                builder.OpenComponent<FieldsLabel>( 0 );
                builder.AddAttribute( 1, nameof( FieldsLabel.ChildContent ), (RenderFragment)( childBuilder => childBuilder.AddContent( 0, "Delivery options" ) ) );
                builder.CloseComponent();

                builder.OpenComponent<RadioGroup<string>>( 2 );
                builder.AddAttribute( 3, nameof( RadioGroup<string>.ElementId ), "delivery-options" );
                builder.CloseComponent();
            } ) );

        cut.WaitForAssertion( () =>
        {
            var legend = cut.Find( "legend" );
            var radioGroup = cut.Find( "#delivery-options" );

            Assert.NotNull( legend.GetAttribute( "id" ) );
            Assert.Equal( legend.GetAttribute( "id" ), radioGroup.GetAttribute( "aria-labelledby" ) );
        } );
    }
}