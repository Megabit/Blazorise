#region Using directives
using System.Threading.Tasks;
using Blazorise.Modules;
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
    public void ShowValue_ShouldDefaultToTrue()
    {
        var comp = Render<ColorPicker>( parameters => parameters
            .Add( x => x.Value, "#6200ea" ) );

        Assert.True( comp.Instance.ShowValue );
        Assert.Equal( "true", comp.Find( ".form-control-color-picker" ).GetAttribute( "data-show-value" ) );
        Assert.Equal( "#6200ea", comp.Find( ".form-control-color-picker" ).GetAttribute( "data-color" ) );
    }

    [Fact]
    public void TogglingShowValue_ShouldPreserveUpdatedValueAndNotRaiseValueChanged()
    {
        var valueChangedCount = 0;
        var comp = Render<ColorPicker>( parameters => parameters
            .Add( x => x.Value, "#6200ea" )
            .Add( x => x.ValueChanged, _ => valueChangedCount++ )
            .Add( x => x.ShowValue, false ) );

        Assert.Equal( "false", comp.Find( ".form-control-color-picker" ).GetAttribute( "data-show-value" ) );

        comp.Render( parameters => parameters
            .Add( x => x.Value, "#9B85BB" ) );

        comp.Render( parameters => parameters
            .Add( x => x.ShowValue, true ) );

        Assert.Equal( "true", comp.Find( ".form-control-color-picker" ).GetAttribute( "data-show-value" ) );
        Assert.Equal( "#9B85BB", comp.Instance.Value );
        Assert.Equal( "#9B85BB", comp.Find( ".form-control-color-picker" ).GetAttribute( "data-color" ) );
        Assert.Equal( 0, valueChangedCount );
    }

    [Fact]
    public void Initialization_ShouldUseRenderedProviderSwatchSelector()
    {
        var comp = Render<ColorPicker>( parameters => parameters
            .Add( x => x.Value, "#6200ea" ) );

        var initialization = Assert.Single(
            JSInterop.Invocations["initialize"],
            invocation => invocation.Arguments.Count > 3 && invocation.Arguments[3] is ColorPickerJSOptions );

        var options = Assert.IsType<ColorPickerJSOptions>( initialization.Arguments[3] );

        Assert.NotNull( comp.Find( ".form-control-color-picker > .form-control-color-preview > .form-control-color-swatch" ) );
        Assert.Equal( ":scope > .form-control-color-preview > .form-control-color-swatch", options.ColorPreviewElementSelector );
        Assert.Null( options.ColorValueElementSelector );
    }

    [Theory]
    [InlineData( "#9B85BB" )]
    [InlineData( null )]
    public async Task PickerCallback_ShouldRefreshRenderedValueWithoutParentBinding( string value )
    {
        var valueChangedCount = 0;
        var comp = Render<ColorPicker>( parameters => parameters
            .Add( x => x.Value, "#6200ea" )
            .Add( x => x.ValueChanged, _ => valueChangedCount++ ) );

        await comp.InvokeAsync( () => comp.Instance.SetValue( value ) );

        Assert.Equal( value, comp.Instance.Value );
        Assert.Equal( value, comp.Find( ".form-control-color-picker" ).GetAttribute( "data-color" ) );
        Assert.Equal( 1, valueChangedCount );
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