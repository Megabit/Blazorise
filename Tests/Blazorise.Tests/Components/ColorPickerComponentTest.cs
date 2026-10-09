#region Using directives
using System.Threading.Tasks;
using Blazorise.Modules;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
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
    public void Preview_ShouldRenderColorAndKeepMenuClosed()
    {
        var comp = Render<ColorPicker>( parameters => parameters
            .Add( x => x.Value, "#6200ea" ) );

        Assert.Contains( "background-color: #6200EA", comp.Find( ".form-control-color-swatch" ).GetAttribute( "style" ) );
        Assert.Empty( comp.FindAll( "[role='dialog']" ) );
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

    [Fact]
    public async Task OutsideInteraction_ShouldClosePickerWithoutPreventingBrowserActions()
    {
        var comp = Render<ColorPicker>();

        await comp.InvokeAsync( () => comp.Instance.Show() );

        var menuElementId = comp.WaitForElement( "[role='dialog']" ).Id;
        var subscription = Assert.IsType<DocumentObserverJsSubscription>( Assert.Single( JSInterop.Invocations["addSubscription"] ).Arguments[0] );

        Assert.Equal( new[] { "pointerdown", "focusin" }, subscription.EventNames );
        Assert.Equal( $"[id=\"{comp.Instance.ElementId}\"], [id=\"{menuElementId}\"]", subscription.ExcludeSelector );
        Assert.False( subscription.PreventDefault );
        Assert.False( subscription.StopPropagation );

        // Updating the color keeps the same outside subscription.
        await comp.InvokeAsync( () => comp.Instance.SetValue( "#008000" ) );
        Assert.Single( JSInterop.Invocations["addSubscription"] );

        var observer = Assert.IsType<DocumentObserver>( Services.GetRequiredService<IDocumentObserver>() );
        await comp.InvokeAsync( () => observer.NotifyDocumentEvent( subscription.Id, new DocumentEventArgs { Type = DocumentEventType.PointerDown } ) );

        Assert.Empty( comp.FindAll( "[role='dialog']" ) );
        Assert.Equal( "#008000", comp.Instance.Value );
        Assert.Equal( subscription.Id, Assert.Single( JSInterop.Invocations["removeSubscription"] ).Arguments[0] );
    }

    [Theory]
    [InlineData( false )]
    [InlineData( true )]
    public async Task SurfaceDraggingOutsideMenu_ShouldTrackAndReleasePointer( bool cancelled )
    {
        var utilities = JSInterop.TryGetModuleJSInterop( "import", Services.GetRequiredService<IJSUtilitiesModule>().ModuleFileName );
        utilities.Setup<DomElement>( "getElementInfo", _ => true ).SetResult( new DomElement
        {
            BoundingClientRect = new DomRectangle { Width = 100, Height = 100 },
        } );

        var comp = Render<ColorPicker>( parameters => parameters.Add( x => x.Value, "#FF0000" ) );
        await comp.InvokeAsync( () => comp.Instance.Show() );

        var menuElementId = comp.WaitForElement( "[role='dialog']" ).Id;

        Assert.Single( JSInterop.Invocations["addSubscription"] );

        await comp.Find( ".form-control-color-picker-surface" ).PointerDownAsync( new PointerEventArgs
        {
            PointerId = 1,
            Button = 0,
            Buttons = 1,
            ClientX = 100,
        } );

        Assert.Equal( 2, JSInterop.Invocations["addSubscription"].Count );

        var subscription = Assert.IsType<DocumentObserverJsSubscription>( JSInterop.Invocations["addSubscription"][1].Arguments[0] );
        Assert.Equal( new[] { "pointermove", "pointerup", "pointercancel" }, subscription.EventNames );
        Assert.Equal( $"[id=\"{menuElementId}\"]", subscription.ExcludeSelector );
        Assert.False( subscription.PreventDefault );

        var observer = Assert.IsType<DocumentObserver>( Services.GetRequiredService<IDocumentObserver>() );
        await comp.InvokeAsync( () => observer.NotifyDocumentEvent( subscription.Id, new DocumentEventArgs
        {
            Type = DocumentEventType.PointerMove,
            PointerId = 1,
            ClientX = 150,
            ClientY = 50,
        } ) );

        Assert.Equal( "#800000", comp.Instance.Value );

        await comp.InvokeAsync( () => observer.NotifyDocumentEvent( subscription.Id, new DocumentEventArgs
        {
            Type = cancelled ? DocumentEventType.PointerCancel : DocumentEventType.PointerUp,
            PointerId = 1,
            ClientX = 150,
            ClientY = 150,
        } ) );

        Assert.Equal( cancelled ? "#800000" : "#000000", comp.Instance.Value );
        Assert.Single( comp.FindAll( "[role='dialog']" ) );
        Assert.Equal( subscription.Id, Assert.Single( JSInterop.Invocations["removeSubscription"] ).Arguments[0] );

        // A move after release cannot continue changing the color.
        await comp.Find( ".form-control-color-picker-surface" ).PointerMoveAsync( new PointerEventArgs
        {
            PointerId = 1,
            Buttons = 1,
            ClientX = 100,
        } );

        Assert.Equal( cancelled ? "#800000" : "#000000", comp.Instance.Value );
    }

    [Fact]
    public async Task TriggerAndEscape_ShouldOpenAndCloseNativePicker()
    {
        var comp = Render<ColorPicker>();

        await comp.Find( "button.form-control-color-picker" ).ClickAsync();

        comp.WaitForElement( ".form-control-color-picker-surface" );

        Assert.Single( comp.FindAll( "[role='dialog']" ) );
        Assert.Equal( "true", comp.Find( "button.form-control-color-picker" ).GetAttribute( "aria-expanded" ) );

        await comp.Find( "[role='dialog']" ).KeyDownAsync( new KeyboardEventArgs { Key = "Escape" } );

        Assert.Empty( comp.FindAll( "[role='dialog']" ) );
        Assert.Equal( "false", comp.Find( "button.form-control-color-picker" ).GetAttribute( "aria-expanded" ) );
    }

    [Theory]
    [InlineData( true, false )]
    [InlineData( false, true )]
    public async Task DisabledOrReadOnly_ShouldPreventOpeningAndSelection( bool disabled, bool readOnly )
    {
        var valueChangedCount = 0;
        var comp = Render<ColorPicker>( parameters => parameters
            .Add( x => x.Value, "#FF0000" )
            .Add( x => x.Disabled, disabled )
            .Add( x => x.ReadOnly, readOnly )
            .Add( x => x.ValueChanged, _ => valueChangedCount++ ) );

        await comp.InvokeAsync( () => comp.Instance.Show() );
        await comp.InvokeAsync( () => comp.Instance.SetValue( "#00FF00" ) );

        Assert.Empty( comp.FindAll( "[role='dialog']" ) );
        Assert.Equal( "#FF0000", comp.Instance.Value );
        Assert.Equal( 0, valueChangedCount );
    }

    [Fact]
    public async Task Sliders_ShouldUpdateColorAndOpacityThroughBinding()
    {
        var valueChangedCount = 0;
        var comp = Render<ColorPicker>( parameters => parameters
            .Add( x => x.Value, "#FF0000" )
            .Add( x => x.ValueChanged, _ => valueChangedCount++ ) );

        await comp.Find( "button.form-control-color-picker" ).ClickAsync();

        comp.WaitForElement( ".form-control-color-picker-surface" );

        Assert.Contains( "color: #FF0000", comp.Find( "[data-color-channel='opacity']" ).GetAttribute( "style" ) );

        await comp.Find( "input[type='range'][max='360']" ).InputAsync( "120" );
        await comp.Find( "input[type='range'][max='1']" ).InputAsync( "0.5" );

        Assert.Equal( "#00FF0080", comp.Instance.Value );
        Assert.Equal( 2, valueChangedCount );
        Assert.Contains( "#00FF0080", comp.Find( ".form-control-color-swatch" ).GetAttribute( "style" ) );
        Assert.Contains( "color: #00FF00", comp.Find( "[data-color-channel='opacity']" ).GetAttribute( "style" ) );
    }

    [Theory]
    [InlineData( null )]
    [InlineData( "#FF0000" )]
    public async Task Cancel_ShouldRestoreColorBeforeOpening( string originalColor )
    {
        var comp = Render<ColorPicker>( parameters => parameters
            .Add( x => x.Value, originalColor )
            .Add( x => x.HideAfterPaletteSelect, false )
            .Add( x => x.Palette, new[] { "#00FF00" } ) );

        await comp.Find( "button.form-control-color-picker" ).ClickAsync();

        comp.WaitForElement( ".form-control-color-picker-surface" );

        await comp.Find( "button[id*='-palette-']" ).ClickAsync();

        Assert.Equal( "#00FF00", comp.Instance.Value );

        await comp.Find( "button[aria-label='cancel and close']" ).ClickAsync();

        Assert.Equal( originalColor, comp.Instance.Value );
        Assert.Empty( comp.FindAll( "[role='dialog']" ) );
    }

    [Theory]
    [InlineData( null, "#00FF00", 1 )]
    [InlineData( "#FF0000", "#FF0000", 0 )]
    [InlineData( "rgba(255,0,0,0.5)", "rgba(255,0,0,0.5)", 0 )]
    public async Task Save_ShouldConfirmDisplayedColorAndClosePicker( string initialColor, string expectedColor, int expectedChanges )
    {
        var valueChangedCount = 0;
        var comp = Render<ColorPicker>( parameters => parameters
            .Add( x => x.Value, initialColor )
            .Add( x => x.ValueChanged, _ => valueChangedCount++ )
            .Add( x => x.Palette, new[] { "#00FF00" } ) );

        await comp.Find( "button.form-control-color-picker" ).ClickAsync();

        comp.WaitForElement( ".form-control-color-picker-surface" );

        await comp.Find( "button[aria-label='save and close']" ).ClickAsync();

        Assert.Equal( expectedColor, comp.Instance.Value );
        Assert.Equal( expectedChanges, valueChangedCount );
        Assert.Empty( comp.FindAll( "[role='dialog']" ) );
    }

    [Fact]
    public async Task Save_ShouldPreserveLiveUpdatesAndUseSavedColorForNextCancel()
    {
        var valueChangedCount = 0;
        var comp = Render<ColorPicker>( parameters => parameters
            .Add( x => x.Value, "#FF0000" )
            .Add( x => x.ValueChanged, _ => valueChangedCount++ ) );

        await comp.Find( "button.form-control-color-picker" ).ClickAsync();

        comp.WaitForElement( ".form-control-color-picker-surface" );

        await comp.Find( "input[type='range'][max='360']" ).InputAsync( "120" );

        Assert.Equal( "#00FF00", comp.Instance.Value );
        Assert.Equal( 1, valueChangedCount );

        await comp.Find( "button[aria-label='save and close']" ).ClickAsync();

        Assert.Equal( "#00FF00", comp.Instance.Value );
        Assert.Equal( 1, valueChangedCount );
        Assert.Empty( comp.FindAll( "[role='dialog']" ) );

        await comp.Find( "button.form-control-color-picker" ).ClickAsync();

        comp.WaitForElement( ".form-control-color-picker-surface" );

        await comp.Find( "input[type='range'][max='360']" ).InputAsync( "240" );
        await comp.Find( "button[aria-label='cancel and close']" ).ClickAsync();

        Assert.Equal( "#00FF00", comp.Instance.Value );
        Assert.Equal( 3, valueChangedCount );
    }

    [Theory]
    [InlineData( true )]
    [InlineData( false )]
    public async Task PaletteSelection_ShouldRespectVisibilityOption( bool hideAfterSelect )
    {
        var comp = Render<ColorPicker>( parameters => parameters
            .Add( x => x.Palette, new[] { "rgba(255, 0, 0, 0.5)" } )
            .Add( x => x.HideAfterPaletteSelect, hideAfterSelect ) );

        await comp.Find( "button.form-control-color-picker" ).ClickAsync();

        comp.WaitForElement( ".form-control-color-picker-surface" );

        await comp.WaitForElement( "button[id*='-palette-']" ).ClickAsync();

        Assert.Equal( "#FF000080", comp.Instance.Value );
        comp.WaitForAssertion( () => Assert.Equal( hideAfterSelect ? 0 : 1, comp.FindAll( "[role='dialog']" ).Count ) );
    }

    [Fact]
    public async Task TextInput_ShouldRejectInvalidColorAndAcceptCssColor()
    {
        var comp = Render<ColorPicker>( parameters => parameters
            .Add( x => x.Value, "#FF0000" ) );

        await comp.Find( "button.form-control-color-picker" ).ClickAsync();

        comp.WaitForElement( ".form-control-color-picker-surface" );

        await comp.Find( "input[type='text']" ).ChangeAsync( "invalid-color" );

        Assert.Equal( "#FF0000", comp.Instance.Value );

        await comp.Find( "input[type='text']" ).ChangeAsync( "hsl(120 100% 50%)" );

        Assert.Equal( "hsl(120 100% 50%)", comp.Instance.Value );
        Assert.Contains( "#00FF00", comp.Find( ".form-control-color-swatch" ).GetAttribute( "style" ) );
    }

    [Fact]
    public async Task Clear_ShouldNotifyBindingAndClosePicker()
    {
        var valueChangedCount = 0;
        var comp = Render<ColorPicker>( parameters => parameters
            .Add( x => x.Value, "#FF0000" )
            .Add( x => x.ValueChanged, _ => valueChangedCount++ ) );

        await comp.Find( "button.form-control-color-picker" ).ClickAsync();

        comp.WaitForElement( ".form-control-color-picker-surface" );

        await comp.Find( "button[aria-label='clear and close']" ).ClickAsync();

        Assert.Null( comp.Instance.Value );
        Assert.Equal( 1, valueChangedCount );
        Assert.Empty( comp.FindAll( "[role='dialog']" ) );
    }

    [Fact]
    public async Task FormatSelect_ShouldChangeDisplayWithoutNotifyingBinding()
    {
        var valueChangedCount = 0;
        var comp = Render<ColorPicker>( parameters => parameters
            .Add( x => x.Value, "rgba(255, 0, 0, 0.5)" )
            .Add( x => x.ValueChanged, _ => valueChangedCount++ ) );

        await comp.Find( "button.form-control-color-picker" ).ClickAsync();

        comp.WaitForElement( ".form-control-color-picker-surface" );

        Assert.Equal( "#FF000080", comp.Find( "input[type='text']" ).GetAttribute( "value" ) );

        await comp.Find( "select" ).ChangeAsync( "True" );

        Assert.Equal( "rgba(255,0,0,0.5)", comp.Find( "input[type='text']" ).GetAttribute( "value" ) );
        Assert.Equal( "rgba(255, 0, 0, 0.5)", comp.Instance.Value );
        Assert.Equal( 0, valueChangedCount );

        await comp.Find( "select" ).ChangeAsync( "False" );

        Assert.Equal( "#FF000080", comp.Find( "input[type='text']" ).GetAttribute( "value" ) );
        Assert.Equal( 0, valueChangedCount );
    }

    [Theory]
    [InlineData( false )]
    [InlineData( true )]
    public async Task CopyButton_ShouldCopyDisplayedInputWithoutNotifyingBinding( bool rgba )
    {
        var utilities = JSInterop.TryGetModuleJSInterop( "import", Services.GetRequiredService<IJSUtilitiesModule>().ModuleFileName );
        var copy = utilities.SetupVoid( "copyToClipboard", _ => true );
        copy.SetVoidResult();

        var valueChangedCount = 0;
        var comp = Render<ColorPicker>( parameters => parameters
            .Add( x => x.Value, "#FF000080" )
            .Add( x => x.ValueChanged, _ => valueChangedCount++ ) );

        await comp.Find( "button.form-control-color-picker" ).ClickAsync();

        comp.WaitForElement( ".form-control-color-picker-surface" );

        if ( rgba )
        {
            await comp.Find( "select" ).ChangeAsync( "True" );
        }

        await comp.Find( "button[aria-label='copy color']" ).ClickAsync( new MouseEventArgs() );

        var invocation = Assert.Single( copy.Invocations );

        Assert.Equal( comp.Find( "input[type='text']" ).Id, invocation.Arguments[1] );
        Assert.Equal( rgba ? "rgba(255,0,0,0.502)" : "#FF000080", comp.Find( "input[type='text']" ).GetAttribute( "value" ) );
        Assert.Equal( 0, valueChangedCount );
        Assert.Single( comp.FindAll( "[role='dialog']" ) );
    }

    [Fact]
    public async Task ParameterChanges_ShouldSynchronizeOpenPickerWithoutNotifyingBinding()
    {
        var valueChangedCount = 0;
        var comp = Render<ColorPicker>( parameters => parameters
            .Add( x => x.Value, "#FF0000" )
            .Add( x => x.ValueChanged, _ => valueChangedCount++ ) );

        await comp.Find( "button.form-control-color-picker" ).ClickAsync();

        comp.WaitForElement( ".form-control-color-picker-surface" );

        comp.Render( parameters => parameters
            .Add( x => x.Value, "#00FF0080" )
            .Add( x => x.ShowHueSlider, false )
            .Add( x => x.ShowPalette, false ) );

        Assert.Empty( comp.FindAll( "input[type='range'][max='360']" ) );
        Assert.Empty( comp.FindAll( "button[id*='-palette-']" ) );
        Assert.Equal( "#00FF0080", comp.Find( "input[type='text']" ).GetAttribute( "value" ) );
        Assert.Equal( 0, valueChangedCount );

        comp.Render( parameters => parameters.Add( x => x.ReadOnly, true ) );

        Assert.Empty( comp.FindAll( "[role='dialog']" ) );
    }

    [Fact]
    public async Task KeyboardSelection_ShouldAdjustSaturationAndBrightness()
    {
        var comp = Render<ColorPicker>( parameters => parameters
            .Add( x => x.Value, "#FF0000" ) );

        await comp.Find( "button.form-control-color-picker" ).ClickAsync();

        comp.WaitForElement( ".form-control-color-picker-surface" );

        await comp.Find( ".form-control-color-picker-surface" ).KeyDownAsync( new KeyboardEventArgs { Key = "Home" } );

        Assert.Equal( "#FFFFFF", comp.Instance.Value );

        await comp.Find( ".form-control-color-picker-surface" ).KeyDownAsync( new KeyboardEventArgs { Key = "End" } );

        Assert.Equal( "#000000", comp.Instance.Value );
    }

    [Fact]
    public async Task PickerLocalizer_ShouldLocalizeNativeControls()
    {
        var comp = Render<ColorPicker>( parameters => parameters
            .Add( x => x.PickerLocalizer, ( key, arguments ) => $"localized {key}" ) );

        await comp.Find( "button.form-control-color-picker" ).ClickAsync();

        comp.WaitForElement( ".form-control-color-picker-surface" );

        Assert.Equal( "localized ui:dialog", comp.Find( "[role='dialog']" ).GetAttribute( "aria-label" ) );
        Assert.Equal( "localized btn:clear", comp.Find( "button[aria-label='localized aria:btn:clear']" ).TextContent );
    }

    [Theory]
    [InlineData( false )]
    [InlineData( true )]
    public async Task PointerSelection_ShouldUseFinalPositionAndIgnoreCancellationBeforeMeasurement( bool cancelled )
    {
        var utilities = JSInterop.TryGetModuleJSInterop( "import", Services.GetRequiredService<IJSUtilitiesModule>().ModuleFileName );
        var measurement = utilities.Setup<DomElement>( "getElementInfo", _ => true );
        var comp = Render<ColorPicker>( parameters => parameters.Add( x => x.Value, "#008000" ) );

        await comp.Find( "button.form-control-color-picker" ).ClickAsync();

        comp.WaitForElement( ".form-control-color-picker-surface" );

        var pointerDownTask = comp.Find( ".form-control-color-picker-surface" ).PointerDownAsync( new PointerEventArgs
        {
            PointerId = 1,
            Button = 0,
            Buttons = 1,
            ClientX = 10,
            ClientY = 20,
        } );

        comp.WaitForAssertion( () => Assert.Single( measurement.Invocations ) );

        if ( cancelled )
        {
            await comp.Find( "[role='dialog']" ).PointerCancelAsync( new PointerEventArgs { PointerId = 1 } );
        }
        else
        {
            await comp.Find( "[role='dialog']" ).PointerUpAsync( new PointerEventArgs
            {
                PointerId = 1,
                Button = 0,
                Buttons = 0,
                ClientX = 110,
                ClientY = 20,
            } );
        }

        await comp.InvokeAsync( () => measurement.SetResult( new DomElement
        {
            BoundingClientRect = new DomRectangle
            {
                Left = 10,
                Top = 20,
                Width = 100,
                Height = 100,
            },
        } ) );

        await pointerDownTask;

        Assert.Equal( cancelled ? "#008000" : "#00FF00", comp.Instance.Value );
    }

    [Fact]
    public async Task PopupControls_ShouldPreserveOuterColorValidation()
    {
        var comp = Render<Validation>( parameters => parameters
            .Add( x => x.Validator, ValidationRule.IsNotEmpty )
            .AddChildContent<ColorPicker>( picker => picker
                .Add( x => x.Palette, new[] { "#FF0000" } )
                .Add( x => x.HideAfterPaletteSelect, false ) ) );

        await comp.Find( "button.form-control-color-picker" ).ClickAsync();

        comp.WaitForElement( ".form-control-color-picker-surface" );

        await comp.Find( "button[id*='-palette-']" ).ClickAsync();
        await comp.Find( "input[type='range'][max='360']" ).InputAsync( "120" );

        comp.WaitForAssertion( () => Assert.Equal( ValidationStatus.Success, comp.Instance.Status ) );

        await comp.Find( "button[aria-label='clear and close']" ).ClickAsync();

        comp.WaitForAssertion( () => Assert.Equal( ValidationStatus.Error, comp.Instance.Status ) );
    }

    [Fact]
    public async Task IdleSurfaceEvents_ShouldNotRenderOrChangePicker()
    {
        var valueChangedCount = 0;
        var comp = Render<ColorPicker>( parameters => parameters
            .Add( x => x.Value, "#FF0000" )
            .Add( x => x.ValueChanged, _ => valueChangedCount++ ) );

        await comp.InvokeAsync( () => comp.Instance.Show() );

        comp.WaitForElement( ".form-control-color-picker-surface" );

        var renderCount = comp.RenderCount;
        var markup = comp.Markup;

        await comp.Find( ".form-control-color-picker-surface" ).PointerMoveAsync( new PointerEventArgs { PointerId = 1 } );
        await comp.Find( "[role='dialog']" ).PointerMoveAsync( new PointerEventArgs { PointerId = 1 } );
        await comp.Find( ".form-control-color-picker-surface" ).KeyDownAsync( new KeyboardEventArgs { Key = "a" } );
        await comp.Find( "[role='dialog']" ).KeyDownAsync( new KeyboardEventArgs { Key = "a" } );

        Assert.Equal( renderCount, comp.RenderCount );
        Assert.Equal( markup, comp.Markup );
        Assert.Equal( "#FF0000", comp.Instance.Value );
        Assert.Equal( 0, valueChangedCount );
    }

    [Fact]
    public async Task ColorStyles_ShouldReflectHueOpacityAndSurfaceSelection()
    {
        var comp = Render<ColorPicker>( parameters => parameters.Add( x => x.Value, "#FF0000" ) );

        await comp.InvokeAsync( () => comp.Instance.Show() );

        comp.WaitForElement( ".form-control-color-picker-surface" );

        var surfaceStyleString = comp.Find( ".form-control-color-picker-surface" ).GetAttribute( "style" );
        var hueStyleString = comp.Find( "[data-color-channel='hue']" ).GetAttribute( "style" );
        var opacityStyleString = comp.Find( "[data-color-channel='opacity']" ).GetAttribute( "style" );

        comp.Render( parameters => parameters.Add( x => x.ShowCancelButton, false ) );

        Assert.Equal( surfaceStyleString, comp.Find( ".form-control-color-picker-surface" ).GetAttribute( "style" ) );
        Assert.Equal( hueStyleString, comp.Find( "[data-color-channel='hue']" ).GetAttribute( "style" ) );
        Assert.Equal( opacityStyleString, comp.Find( "[data-color-channel='opacity']" ).GetAttribute( "style" ) );

        await comp.Find( "input[type='range'][max='1']" ).InputAsync( "0.5" );

        comp.WaitForAssertion( () => Assert.Contains( "background-color: #FF000080", comp.Find( ".form-control-color-picker-marker" ).GetAttribute( "style" ) ) );

        Assert.Equal( surfaceStyleString, comp.Find( ".form-control-color-picker-surface" ).GetAttribute( "style" ) );
        Assert.Equal( hueStyleString, comp.Find( "[data-color-channel='hue']" ).GetAttribute( "style" ) );
        Assert.Equal( opacityStyleString, comp.Find( "[data-color-channel='opacity']" ).GetAttribute( "style" ) );

        await comp.Find( "input[type='range'][max='360']" ).InputAsync( "120" );

        comp.WaitForAssertion( () => Assert.Contains( "color: #00FF00", comp.Find( "[data-color-channel='opacity']" ).GetAttribute( "style" ) ) );

        Assert.NotEqual( surfaceStyleString, comp.Find( ".form-control-color-picker-surface" ).GetAttribute( "style" ) );
        Assert.NotEqual( hueStyleString, comp.Find( "[data-color-channel='hue']" ).GetAttribute( "style" ) );
        Assert.NotEqual( opacityStyleString, comp.Find( "[data-color-channel='opacity']" ).GetAttribute( "style" ) );

        surfaceStyleString = comp.Find( ".form-control-color-picker-surface" ).GetAttribute( "style" );
        hueStyleString = comp.Find( "[data-color-channel='hue']" ).GetAttribute( "style" );

        await comp.Find( ".form-control-color-picker-surface" ).KeyDownAsync( new KeyboardEventArgs { Key = "Home" } );

        comp.WaitForAssertion( () => Assert.Contains( "color: #FFFFFF", comp.Find( "[data-color-channel='opacity']" ).GetAttribute( "style" ) ) );

        Assert.Equal( surfaceStyleString, comp.Find( ".form-control-color-picker-surface" ).GetAttribute( "style" ) );
        Assert.Equal( hueStyleString, comp.Find( "[data-color-channel='hue']" ).GetAttribute( "style" ) );
        Assert.Contains( "left: 0%", comp.Find( ".form-control-color-picker-marker" ).GetAttribute( "style" ) );
    }

    [Fact]
    public async Task SurfaceMovement_ShouldOnlyRefreshPreviewWhenColorChanges()
    {
        var utilities = JSInterop.TryGetModuleJSInterop( "import", Services.GetRequiredService<IJSUtilitiesModule>().ModuleFileName );
        utilities.Setup<DomElement>( "getElementInfo", _ => true ).SetResult( new DomElement
        {
            BoundingClientRect = new DomRectangle { Width = 100, Height = 100 },
        } );

        var previewRenderCount = 0;
        RenderFragment previewContent = builder =>
        {
            previewRenderCount++;
            builder.AddContent( 0, "Color preview" );
        };

        var valueChangedCount = 0;
        var comp = Render<ColorPicker>( parameters => parameters
            .Add( x => x.Value, "#FF0000" )
            .Add( x => x.ValueChanged, _ => valueChangedCount++ )
            .Add( x => x.PreviewContent, previewContent ) );

        await comp.InvokeAsync( () => comp.Instance.Show() );

        comp.WaitForElement( ".form-control-color-picker-surface" );

        var initialPreviewRenderCount = previewRenderCount;

        await comp.Find( ".form-control-color-picker-surface" ).PointerDownAsync( new PointerEventArgs
        {
            PointerId = 1,
            Button = 0,
            Buttons = 1,
            ClientX = 100,
        } );

        await comp.Find( ".form-control-color-picker-surface" ).PointerMoveAsync( new PointerEventArgs
        {
            PointerId = 1,
            Buttons = 1,
            ClientX = 99.99,
        } );

        comp.WaitForAssertion( () => Assert.Contains( "left: 99.99%", comp.Find( ".form-control-color-picker-marker" ).GetAttribute( "style" ) ) );

        Assert.Equal( "#FF0000", comp.Instance.Value );
        Assert.Equal( 0, valueChangedCount );
        Assert.Equal( initialPreviewRenderCount, previewRenderCount );

        comp.Render( parameters => parameters.Add( x => x.ShowCancelButton, false ) );

        Assert.Contains( "left: 99.99%", comp.Find( ".form-control-color-picker-marker" ).GetAttribute( "style" ) );

        initialPreviewRenderCount = previewRenderCount;

        var nextPosition = new PointerEventArgs
        {
            PointerId = 1,
            Buttons = 1,
            ClientX = 50,
        };

        await comp.Find( ".form-control-color-picker-surface" ).PointerMoveAsync( nextPosition );

        comp.WaitForAssertion( () => Assert.Equal( "#FF8080", comp.Find( "input[type='text']" ).GetAttribute( "value" ) ) );

        Assert.Equal( "#FF8080", comp.Instance.Value );
        Assert.Equal( 1, valueChangedCount );
        Assert.True( previewRenderCount > initialPreviewRenderCount );
        Assert.Contains( "left: 50%", comp.Find( ".form-control-color-picker-marker" ).GetAttribute( "style" ) );
        Assert.Contains( "color: #FF8080", comp.Find( "[data-color-channel='opacity']" ).GetAttribute( "style" ) );

        initialPreviewRenderCount = previewRenderCount;
        var markerStyleString = comp.Find( ".form-control-color-picker-marker" ).GetAttribute( "style" );

        await comp.Find( ".form-control-color-picker-surface" ).PointerMoveAsync( nextPosition );

        Assert.Equal( 1, valueChangedCount );
        Assert.Equal( initialPreviewRenderCount, previewRenderCount );
        Assert.Equal( markerStyleString, comp.Find( ".form-control-color-picker-marker" ).GetAttribute( "style" ) );
    }
}