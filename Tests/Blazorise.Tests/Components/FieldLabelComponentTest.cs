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