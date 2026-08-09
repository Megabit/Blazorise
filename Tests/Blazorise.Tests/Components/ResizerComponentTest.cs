using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using Blazorise;
using Xunit;

namespace Blazorise.Tests.Components;

public class ResizerComponentTest : BunitContext
{
    public ResizerComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop.AddBlazoriseResizer();
    }

    [Fact]
    public void RendersHorizontalBottomResizerByDefault()
    {
        IRenderedComponent<Resizer> component = Render<Resizer>();
        IElement resizer = component.Find( ".resizer" );

        Assert.Contains( "resizer-horizontal", resizer.ClassList );
        Assert.Contains( "resizer-bottom", resizer.ClassList );
        Assert.Equal( "separator", resizer.GetAttribute( "role" ) );
        Assert.Equal( "horizontal", resizer.GetAttribute( "aria-orientation" ) );
        Assert.Equal( "0", resizer.GetAttribute( "aria-valuemin" ) );
        Assert.Equal( "Resize", resizer.GetAttribute( "aria-label" ) );
        Assert.Equal( "0", resizer.GetAttribute( "tabindex" ) );
    }

    [Fact]
    public void RendersVerticalEndResizerWithAccessibilityValues()
    {
        IRenderedComponent<Resizer> component = Render<Resizer>( parameters => parameters
            .Add( x => x.Orientation, Orientation.Vertical )
            .Add( x => x.Value, 280 )
            .Add( x => x.Min, 180 )
            .Add( x => x.Max, 480 )
            .Add( x => x.AriaLabel, "Resize navigation" ) );
        IElement resizer = component.Find( ".resizer" );

        Assert.Contains( "resizer-vertical", resizer.ClassList );
        Assert.Contains( "resizer-end", resizer.ClassList );
        Assert.Equal( "vertical", resizer.GetAttribute( "aria-orientation" ) );
        Assert.Equal( "180", resizer.GetAttribute( "aria-valuemin" ) );
        Assert.Equal( "480", resizer.GetAttribute( "aria-valuemax" ) );
        Assert.Equal( "280", resizer.GetAttribute( "aria-valuenow" ) );
        Assert.Equal( "Resize navigation", resizer.GetAttribute( "aria-label" ) );
    }

    [Fact]
    public void DisabledResizerIsNotKeyboardFocusable()
    {
        IRenderedComponent<Resizer> component = Render<Resizer>( parameters => parameters
            .Add( x => x.Disabled, true ) );

        Assert.Contains( "resizer-disabled", component.Find( ".resizer" ).ClassList );
        Assert.Equal( "true", component.Find( ".resizer" ).GetAttribute( "aria-disabled" ) );
        Assert.Equal( "-1", component.Find( ".resizer" ).GetAttribute( "tabindex" ) );
    }

    [Fact]
    public void GutterIsOptIn()
    {
        IRenderedComponent<Resizer> component = Render<Resizer>();

        Assert.DoesNotContain( "resizer-gutter", component.Find( ".resizer" ).ClassList );

        component.Render( parameters => parameters
            .Add( x => x.ShowGutter, true ) );

        Assert.Contains( "resizer-gutter", component.Find( ".resizer" ).ClassList );
    }

    [Fact]
    public async Task ResizeEndCommitsValueUnlessCanceled()
    {
        double committedValue = 0;
        int endedCount = 0;

        IRenderedComponent<Resizer> component = Render<Resizer>( parameters => parameters
            .Add( x => x.ValueChanged, ( double value ) => committedValue = value )
            .Add( x => x.ResizeEnded, ( ResizerEventArgs _ ) => endedCount++ ) );

        await component.Instance.OnResizeEnded( new ResizerEventArgs { Size = 320 } );
        await component.Instance.OnResizeEnded( new ResizerEventArgs { Size = 240, Canceled = true } );

        Assert.Equal( 320d, committedValue );
        Assert.Equal( 2, endedCount );
    }
}