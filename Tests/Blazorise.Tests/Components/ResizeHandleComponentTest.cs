using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using Blazorise;
using Xunit;

namespace Blazorise.Tests.Components;

public class ResizeHandleComponentTest : BunitContext
{
    public ResizeHandleComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop.AddBlazoriseResizeHandle();
    }

    [Fact]
    public void RendersHorizontalBottomHandleByDefault()
    {
        IRenderedComponent<ResizeHandle> component = Render<ResizeHandle>();
        IElement handle = component.Find( ".resize-handle" );

        Assert.Contains( "resize-handle-horizontal", handle.ClassList );
        Assert.Contains( "resize-handle-bottom", handle.ClassList );
        Assert.Equal( "separator", handle.GetAttribute( "role" ) );
        Assert.Equal( "horizontal", handle.GetAttribute( "aria-orientation" ) );
        Assert.Equal( "0", handle.GetAttribute( "aria-valuemin" ) );
        Assert.Equal( "Resize", handle.GetAttribute( "aria-label" ) );
        Assert.Equal( "0", handle.GetAttribute( "tabindex" ) );
    }

    [Fact]
    public void RendersVerticalEndHandleWithAccessibilityValues()
    {
        IRenderedComponent<ResizeHandle> component = Render<ResizeHandle>( parameters => parameters
            .Add( x => x.Orientation, Orientation.Vertical )
            .Add( x => x.Value, 280 )
            .Add( x => x.Min, 180 )
            .Add( x => x.Max, 480 )
            .Add( x => x.AriaLabel, "Resize navigation" ) );
        IElement handle = component.Find( ".resize-handle" );

        Assert.Contains( "resize-handle-vertical", handle.ClassList );
        Assert.Contains( "resize-handle-end", handle.ClassList );
        Assert.Equal( "vertical", handle.GetAttribute( "aria-orientation" ) );
        Assert.Equal( "180", handle.GetAttribute( "aria-valuemin" ) );
        Assert.Equal( "480", handle.GetAttribute( "aria-valuemax" ) );
        Assert.Equal( "280", handle.GetAttribute( "aria-valuenow" ) );
        Assert.Equal( "Resize navigation", handle.GetAttribute( "aria-label" ) );
    }

    [Fact]
    public void DisabledHandleIsNotKeyboardFocusable()
    {
        IRenderedComponent<ResizeHandle> component = Render<ResizeHandle>( parameters => parameters
            .Add( x => x.Disabled, true ) );

        Assert.Contains( "resize-handle-disabled", component.Find( ".resize-handle" ).ClassList );
        Assert.Equal( "true", component.Find( ".resize-handle" ).GetAttribute( "aria-disabled" ) );
        Assert.Equal( "-1", component.Find( ".resize-handle" ).GetAttribute( "tabindex" ) );
    }

    [Fact]
    public void GutterIsOptIn()
    {
        IRenderedComponent<ResizeHandle> component = Render<ResizeHandle>();

        Assert.DoesNotContain( "resize-handle-gutter", component.Find( ".resize-handle" ).ClassList );

        component.Render( parameters => parameters
            .Add( x => x.ShowGutter, true ) );

        Assert.Contains( "resize-handle-gutter", component.Find( ".resize-handle" ).ClassList );
    }

    [Fact]
    public async Task ResizeEndCommitsValueUnlessCanceled()
    {
        double committedValue = 0;
        int endedCount = 0;

        IRenderedComponent<ResizeHandle> component = Render<ResizeHandle>( parameters => parameters
            .Add( x => x.ValueChanged, ( double value ) => committedValue = value )
            .Add( x => x.ResizeEnded, ( ResizeHandleEventArgs _ ) => endedCount++ ) );

        await component.Instance.OnResizeEnded( new ResizeHandleEventArgs { Size = 320 } );
        await component.Instance.OnResizeEnded( new ResizeHandleEventArgs { Size = 240, Canceled = true } );

        Assert.Equal( 320d, committedValue );
        Assert.Equal( 2, endedCount );
    }
}