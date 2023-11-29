#region Using directives
using Bunit;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class FieldLabelComponentTest : TestContext
{
    public FieldLabelComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProvidersTests().AddTestData();
        JSInterop.AddBlazoriseUtilities();
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



}