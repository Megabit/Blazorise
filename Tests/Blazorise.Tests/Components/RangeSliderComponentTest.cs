using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace Blazorise.Tests.Components;

public class RangeSliderComponentTest : TestContext
{
    public RangeSliderComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop.AddBlazoriseUtilities();
        JSInterop.AddBlazoriseRangeSlider();
    }

    [Fact]
    public async Task SwapsHandlesByDefault()
    {
        // setup
        IRenderedComponent<RangeSlider<int>> comp = RenderComponent<RangeSlider<int>>( parameters => parameters
            .Add( x => x.Value, new RangeSliderValue<int>( 20, 70 ) )
            .Add( x => x.Min, 0 )
            .Add( x => x.Max, 100 )
            .Add( x => x.Step, 1 ) );

        IElement startInput = comp.FindAll( "input" )[0];

        // test
        await startInput.InputAsync( new ChangeEventArgs { Value = "80" } );

        // validate
        Assert.Equal( 70, comp.Instance.Value.Start );
        Assert.Equal( 80, comp.Instance.Value.End );
    }

    [Fact]
    public async Task PreventsStartHandleFromCrossingWhenClampToOtherHandleIsTrue()
    {
        // setup
        IRenderedComponent<RangeSlider<int>> comp = RenderComponent<RangeSlider<int>>( parameters => parameters
            .Add( x => x.Value, new RangeSliderValue<int>( 20, 70 ) )
            .Add( x => x.Min, 0 )
            .Add( x => x.Max, 100 )
            .Add( x => x.Step, 1 )
            .Add( x => x.ClampToOtherHandle, true ) );

        IElement startInput = comp.FindAll( "input" )[0];

        // test
        await startInput.InputAsync( new ChangeEventArgs { Value = "80" } );

        // validate
        Assert.Equal( 70, comp.Instance.Value.Start );
        Assert.Equal( 70, comp.Instance.Value.End );
    }

    [Fact]
    public void KeepsFullInputBoundsWhenClampToOtherHandleIsTrue()
    {
        // setup
        IRenderedComponent<RangeSlider<int>> comp = RenderComponent<RangeSlider<int>>( parameters => parameters
            .Add( x => x.Value, new RangeSliderValue<int>( 20, 70 ) )
            .Add( x => x.Min, 0 )
            .Add( x => x.Max, 100 )
            .Add( x => x.Step, 1 )
            .Add( x => x.ClampToOtherHandle, true ) );

        IElement startInput = comp.FindAll( "input" )[0];
        IElement endInput = comp.FindAll( "input" )[1];

        // validate
        Assert.Equal( "0", startInput.GetAttribute( "min" ) );
        Assert.Equal( "100", startInput.GetAttribute( "max" ) );
        Assert.Equal( "0", endInput.GetAttribute( "min" ) );
        Assert.Equal( "100", endInput.GetAttribute( "max" ) );
    }

    [Fact]
    public async Task PreventsEndHandleFromCrossingWhenClampToOtherHandleIsTrue()
    {
        // setup
        IRenderedComponent<RangeSlider<int>> comp = RenderComponent<RangeSlider<int>>( parameters => parameters
            .Add( x => x.Value, new RangeSliderValue<int>( 20, 70 ) )
            .Add( x => x.Min, 0 )
            .Add( x => x.Max, 100 )
            .Add( x => x.Step, 1 )
            .Add( x => x.ClampToOtherHandle, true ) );

        IElement endInput = comp.FindAll( "input" )[1];

        // test
        await endInput.InputAsync( new ChangeEventArgs { Value = "10" } );

        // validate
        Assert.Equal( 20, comp.Instance.Value.Start );
        Assert.Equal( 20, comp.Instance.Value.End );
    }

    [Fact]
    public async Task PreventsStartHandleFromMatchingEndWhenAllowEqualValuesIsFalse()
    {
        // setup
        IRenderedComponent<RangeSlider<int>> comp = RenderComponent<RangeSlider<int>>( parameters => parameters
            .Add( x => x.Value, new RangeSliderValue<int>( 20, 70 ) )
            .Add( x => x.Min, 0 )
            .Add( x => x.Max, 100 )
            .Add( x => x.Step, 1 )
            .Add( x => x.ClampToOtherHandle, true )
            .Add( x => x.AllowEqualValues, false ) );

        IElement startInput = comp.FindAll( "input" )[0];

        // test
        await startInput.InputAsync( new ChangeEventArgs { Value = "70" } );

        // validate
        Assert.Equal( 69, comp.Instance.Value.Start );
        Assert.Equal( 70, comp.Instance.Value.End );
    }

    [Fact]
    public async Task PreventsEndHandleFromMatchingStartWhenAllowEqualValuesIsFalse()
    {
        // setup
        IRenderedComponent<RangeSlider<int>> comp = RenderComponent<RangeSlider<int>>( parameters => parameters
            .Add( x => x.Value, new RangeSliderValue<int>( 20, 70 ) )
            .Add( x => x.Min, 0 )
            .Add( x => x.Max, 100 )
            .Add( x => x.Step, 5 )
            .Add( x => x.ClampToOtherHandle, true )
            .Add( x => x.AllowEqualValues, false ) );

        IElement endInput = comp.FindAll( "input" )[1];

        // test
        await endInput.InputAsync( new ChangeEventArgs { Value = "20" } );

        // validate
        Assert.Equal( 20, comp.Instance.Value.Start );
        Assert.Equal( 25, comp.Instance.Value.End );
    }

    [Fact]
    public void RangeSliderValue_ImplicitlyConvertsFromTuple()
    {
        RangeSliderValue<decimal> range = (50m, 175m);

        Assert.Equal( 50m, range.Start );
        Assert.Equal( 175m, range.End );
    }

    [Fact]
    public void RangeSliderValue_ImplicitlyConvertsToTuple()
    {
        RangeSliderValue<decimal> range = new( 50m, 175m );

        (decimal Start, decimal End) tuple = range;

        Assert.Equal( 50m, tuple.Start );
        Assert.Equal( 175m, tuple.End );
    }
}