#region Using directives
using Bunit;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class ColorPickerComponentTest : BunitContext
{
    public ColorPickerComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop.AddBlazoriseColorPicker();
    }

    [Fact]
    public void UpdatingValueParameter_ShouldNotRaiseValueChanged()
    {
        // setup
        var valueChangedCount = 0;
        var comp = Render<ColorPicker>( parameters => parameters
            .Add( x => x.Value, "#6200ea" )
            .Add( x => x.ValueChanged, _ => valueChangedCount++ ) );

        // test
        comp.Render( parameters => parameters
            .Add( x => x.Value, "#9B85BB" )
            .Add( x => x.ValueChanged, _ => valueChangedCount++ ) );

        // validate
        Assert.Equal( 0, valueChangedCount );
    }
}