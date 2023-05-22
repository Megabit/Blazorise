#region Using directives
using Blazorise.Tests.Helpers;
using Bunit;
using FluentAssertions;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class FieldLabelComponentTest : TestContext
{
    public FieldLabelComponentTest()
    {
        BlazoriseConfig.AddBootstrapProviders( Services );
        BlazoriseConfig.JSInterop.AddUtilities( JSInterop );
    }

    [Fact]
    public void ValidationIndicator_Should_RenderIndicator()
    {
        // setup

        // test
        var cut = RenderComponent<FieldLabel>( parameters =>
        {
            parameters.Add( x => x.ValidationIndicator, true );
            parameters.AddChildContent( "Required Field" );
        } );

        // validate
        var requiredLabel = cut.WaitForElement( ".b-field-label-required" );
        requiredLabel.Should().NotBeNull();
    }



}