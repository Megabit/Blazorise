using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace Blazorise.Tests.Components;

public class FocusTrapComponentTest : BunitContext
{
    public FocusTrapComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider();
        JSInterop
            .AddBlazoriseButton()
            .AddBlazoriseFocusTrap();
    }

    [Fact]
    public void ActiveFocusTrapDoesNotRenderFocusableSentinels()
    {
        var component = Render( builder =>
        {
            builder.OpenComponent<FocusTrap>( 0 );
            builder.AddAttribute( 1, nameof( FocusTrap.Active ), true );
            builder.AddAttribute( 2, nameof( FocusTrap.ChildContent ), (RenderFragment)( childBuilder =>
            {
                childBuilder.OpenComponent<Button>( 0 );
                childBuilder.AddAttribute( 1, nameof( Button.ChildContent ), (RenderFragment)( buttonBuilder => buttonBuilder.AddContent( 0, "OK" ) ) );
                childBuilder.CloseComponent();
            } ) );
            builder.CloseComponent();
        } );

        Assert.Empty( component.FindAll( "div[tabindex='0']" ) );
    }
}