#region Using directives
using System;
using System.Globalization;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Modules;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// Owns color-surface pointer interaction and marker rendering.
/// </summary>
public partial class _ColorPickerSurface : BaseComponent, IAsyncDisposable
{
    #region Members

    /// <summary>
    /// The starting and latest pointer events for the current surface interaction.
    /// </summary>
    private (PointerEventArgs Start, PointerEventArgs Current)? pointerInteraction;

    /// <summary>
    /// The measured surface bounds used to map pointer positions to color channels.
    /// </summary>
    private DomRectangle surfaceRectangle;

    /// <summary>
    /// The supplied color and its parameter change metadata.
    /// </summary>
    private ComponentParameterInfo<ColorPickerColor> paramColor;

    /// <summary>
    /// The pending or active subscription for pointer interaction outside the menu.
    /// </summary>
    private Task<IAsyncDisposable> pointerSubscriptionTask;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes the surface marker builders.
    /// </summary>
    public _ColorPickerSurface()
    {
        MarkerClassBuilder = new( BuildMarkerClasses );
        MarkerStyleBuilder = new( BuildMarkerStyles );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override Task SetParametersAsync( ParameterView parameters )
    {
        parameters.TryGetParameter( Color, out paramColor );

        InvalidateColorStyles();

        return base.SetParametersAsync( parameters );
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing )
        {
            pointerInteraction = null;
            await DisposePointerSubscriptionAsync();
        }

        await base.DisposeAsync( disposing );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.ColorPickerSurface() );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void BuildStyles( StyleBuilder builder )
    {
        builder.Append( $"background-color: {CssColor.Hsl( Color.Hue, 100, 50 )};" );

        base.BuildStyles( builder );
    }

    /// <summary>
    /// Builds the classes for the surface marker.
    /// </summary>
    private void BuildMarkerClasses( ClassBuilder builder ) => builder.Append( ClassProvider.ColorPickerMarker() );

    /// <summary>
    /// Builds the position and background color styles for the surface marker.
    /// </summary>
    private void BuildMarkerStyles( StyleBuilder builder )
    {
        var color = Color;

        builder.Append( $"left: {color.Saturation.ToString( "0.###", CultureInfo.InvariantCulture )}%;" );
        builder.Append( $"top: {( 100 - color.Brightness ).ToString( "0.###", CultureInfo.InvariantCulture )}%;" );
        builder.Append( $"background-color: {color.ToHexString()};" );
    }

    /// <inheritdoc/>
    protected internal override void DirtyClasses()
    {
        MarkerClassBuilder.Dirty();

        base.DirtyClasses();
    }

    /// <summary>
    /// Invalidates the surface and marker styles only when their color inputs change.
    /// </summary>
    private void InvalidateColorStyles()
    {
        if ( !paramColor.Changed )
        {
            return;
        }

        var color = paramColor.Value;

        if ( color.Hue != Color.Hue )
        {
            StyleBuilder.Dirty();
        }

        if ( color.Saturation != Color.Saturation
            || color.Brightness != Color.Brightness
            || !color.ToHexString().IsEqual( Color.ToHexString() ) )
        {
            MarkerStyleBuilder.Dirty();
        }
    }

    /// <summary>
    /// Starts a surface interaction and measures the bounds for the initial color selection.
    /// </summary>
    private async Task OnPointerDownHandler( PointerEventArgs eventArgs )
    {
        if ( !CanSelectColor || eventArgs.Button != 0 || pointerSubscriptionTask is not null )
        {
            return;
        }

        pointerInteraction = (eventArgs, eventArgs);
        surfaceRectangle = default;

        pointerSubscriptionTask = DocumentObserver.Subscribe( new()
        {
            OwnerId = ElementId,
            EventTypes = DocumentEventTypes.PointerDrag,
            ExcludeSelector = CssSelectorUtilities.BuildElementIdSelector( Parent.MenuElementId ),
            Handler = OnDocumentPointerHandler,
        } ).AsTask();

        await pointerSubscriptionTask;

        if ( pointerInteraction is not { } activeInteraction
            || !ReferenceEquals( activeInteraction.Start, eventArgs )
            || !CanSelectColor )
        {
            return;
        }

        var rectangle = ( await JSUtilitiesModule.GetElementInfo( ElementRef, ElementId ) ).BoundingClientRect;

        if ( pointerInteraction is { } interaction
            && ReferenceEquals( interaction.Start, eventArgs )
            && CanSelectColor )
        {
            surfaceRectangle = rectangle;
            await UpdateSurfaceColor( interaction.Current, false );
        }
    }

    /// <summary>
    /// Updates the surface selection while the active pointer's primary button is pressed.
    /// </summary>
    internal async Task OnPointerMoveHandler( PointerEventArgs eventArgs )
    {
        if ( pointerInteraction is not { } interaction
            || interaction.Start.PointerId != eventArgs.PointerId
            || ( interaction.Current.Buttons & 1 ) == 0 )
        {
            return;
        }

        if ( ( eventArgs.Buttons & 1 ) == 0 )
        {
            pointerInteraction = null;
            await DisposePointerSubscriptionAsync();
            return;
        }

        pointerInteraction = (interaction.Start, eventArgs);

        await UpdateSurfaceColor( eventArgs );
    }

    /// <summary>
    /// Applies the final pointer position and ends the document pointer subscription.
    /// </summary>
    internal async Task OnPointerUpHandler( PointerEventArgs eventArgs )
    {
        if ( pointerInteraction is not { } interaction
            || interaction.Start.PointerId != eventArgs.PointerId
            || ( interaction.Current.Buttons & 1 ) == 0 )
        {
            return;
        }

        pointerInteraction = (interaction.Start, eventArgs);

        await UpdateSurfaceColor( eventArgs );
        await DisposePointerSubscriptionAsync();
    }

    /// <summary>
    /// Cancels the active pointer interaction and releases its document subscription.
    /// </summary>
    internal async Task OnPointerCancelHandler( PointerEventArgs eventArgs )
    {
        if ( pointerInteraction?.Start.PointerId == eventArgs.PointerId )
        {
            pointerInteraction = null;
            await DisposePointerSubscriptionAsync();
        }
    }

    /// <summary>
    /// Converts document pointer events and forwards them to the surface handlers.
    /// </summary>
    private Task OnDocumentPointerHandler( DocumentEventArgs eventArgs )
    {
        var pointerEventArgs = new PointerEventArgs
        {
            PointerId = eventArgs.PointerId,
            ClientX = eventArgs.ClientX,
            ClientY = eventArgs.ClientY,
            Buttons = eventArgs.Buttons,
        };

        return eventArgs.Type switch
        {
            DocumentEventType.PointerMove => OnPointerMoveHandler( pointerEventArgs ),
            DocumentEventType.PointerUp => OnPointerUpHandler( pointerEventArgs ),
            DocumentEventType.PointerCancel => OnPointerCancelHandler( pointerEventArgs ),
            _ => Task.CompletedTask,
        };
    }

    /// <summary>
    /// Handles keyboard color selection and focus navigation from the surface.
    /// </summary>
    private async Task OnKeyDownHandler( KeyboardEventArgs eventArgs )
    {
        if ( !CanSelectColor )
        {
            return;
        }

        if ( eventArgs.Key == "Tab" )
        {
            if ( eventArgs.ShiftKey )
            {
                await Parent.Focus( false );
            }
            else
            {
                await JSUtilitiesModule.Focus( default, Parent.FirstControlElementId, false );
            }

            return;
        }

        var color = Parent.SelectedColor;
        var saturation = color.Saturation;
        var brightness = color.Brightness;
        var increment = eventArgs.ShiftKey ? 10 : 1;

        switch ( eventArgs.Key )
        {
            case "ArrowLeft":
                saturation = Math.Max( 0, saturation - increment );
                break;
            case "ArrowRight":
                saturation = Math.Min( 100, saturation + increment );
                break;
            case "ArrowUp":
                brightness = Math.Min( 100, brightness + increment );
                break;
            case "ArrowDown":
                brightness = Math.Max( 0, brightness - increment );
                break;
            case "Home":
                saturation = 0;
                brightness = 100;
                break;
            case "End":
                saturation = 100;
                brightness = 0;
                break;
            default:
                return;
        }

        await SelectColor( saturation, brightness );
    }

    /// <summary>
    /// Releases the subscription for pointer interaction outside the menu.
    /// </summary>
    private async Task DisposePointerSubscriptionAsync()
    {
        var subscriptionTask = pointerSubscriptionTask;
        pointerSubscriptionTask = null;

        if ( subscriptionTask is not null )
        {
            await ( await subscriptionTask ).DisposeAsync();
        }
    }

    /// <summary>
    /// Maps the active pointer position to saturation and brightness and updates the selection.
    /// </summary>
    /// <param name="eventArgs">The pointer position to apply.</param>
    /// <param name="measureBounds">Whether to refresh the surface bounds before calculating the color.</param>
    private async Task UpdateSurfaceColor( PointerEventArgs eventArgs, bool measureBounds = true )
    {
        if ( !CanSelectColor || surfaceRectangle.Width <= 0 || surfaceRectangle.Height <= 0 )
        {
            return;
        }

        if ( measureBounds )
        {
            var interactionStart = pointerInteraction?.Start;
            var rectangle = ( await JSUtilitiesModule.GetElementInfo( ElementRef, ElementId ) ).BoundingClientRect;

            if ( !CanSelectColor
                || pointerInteraction is not { } interaction
                || !ReferenceEquals( interaction.Start, interactionStart ) )
            {
                return;
            }

            surfaceRectangle = rectangle;
            eventArgs = interaction.Current;

            if ( surfaceRectangle.Width <= 0 || surfaceRectangle.Height <= 0 )
            {
                return;
            }
        }

        var saturation = Math.Clamp( ( eventArgs.ClientX - surfaceRectangle.Left ) / surfaceRectangle.Width * 100, 0, 100 );
        var brightness = 100 - Math.Clamp( ( eventArgs.ClientY - surfaceRectangle.Top ) / surfaceRectangle.Height * 100, 0, 100 );

        await SelectColor( saturation, brightness );
    }

    /// <summary>
    /// Updates the parent color and redraws only the surface when the bound value is unchanged.
    /// </summary>
    private async Task SelectColor( double saturation, double brightness )
    {
        var previousValue = Parent.Value;

        if ( !await Parent.SelectSurfaceColor( saturation, brightness ) || !CanSelectColor )
        {
            return;
        }

        // Value changes refresh the surface through the parent cascade.
        if ( !previousValue.IsEqual( Parent.Value ) )
        {
            return;
        }

        // Marker-only changes refresh the color snapshot without rendering the menu.
        paramColor = new( Parent.SelectedColor, false, Parent.SelectedColor != Color );
        InvalidateColorStyles();
        Color = paramColor.Value;

        await InvokeAsync( StateHasChanged );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets whether the surface is active and its parent allows color selection.
    /// </summary>
    private bool CanSelectColor => !Disposed && !AsyncDisposed && Parent.IsPickerVisible && !Parent.IsInteractionDisabled;

    /// <summary>
    /// Gets the class builder for the surface marker.
    /// </summary>
    private ClassBuilder MarkerClassBuilder { get; }

    /// <summary>
    /// Gets the style builder for the surface marker.
    /// </summary>
    private StyleBuilder MarkerStyleBuilder { get; }

    /// <summary>
    /// Gets the classes for the surface marker.
    /// </summary>
    private string MarkerClassNames => MarkerClassBuilder.Class;

    /// <summary>
    /// Gets the styles for the surface marker.
    /// </summary>
    private string MarkerStyleNames => MarkerStyleBuilder.Styles;

    /// <summary>
    /// Gets the pointer press handler that suppresses automatic surface rendering.
    /// </summary>
    private Func<PointerEventArgs, Task> NonRenderingPointerDownHandler
        => EventUtil.AsNonRenderingEventHandler<PointerEventArgs>( OnPointerDownHandler );

    /// <summary>
    /// Gets the pointer movement handler that suppresses automatic surface rendering.
    /// </summary>
    private Func<PointerEventArgs, Task> NonRenderingPointerMoveHandler
        => EventUtil.AsNonRenderingEventHandler<PointerEventArgs>( OnPointerMoveHandler );

    /// <summary>
    /// Gets the pointer release handler that suppresses automatic surface rendering.
    /// </summary>
    private Func<PointerEventArgs, Task> NonRenderingPointerUpHandler
        => EventUtil.AsNonRenderingEventHandler<PointerEventArgs>( OnPointerUpHandler );

    /// <summary>
    /// Gets the pointer cancellation handler that suppresses automatic surface rendering.
    /// </summary>
    private Func<PointerEventArgs, Task> NonRenderingPointerCancelHandler
        => EventUtil.AsNonRenderingEventHandler<PointerEventArgs>( OnPointerCancelHandler );

    /// <summary>
    /// Gets the keyboard handler that suppresses automatic surface rendering.
    /// </summary>
    private Func<KeyboardEventArgs, Task> NonRenderingKeyDownHandler
        => EventUtil.AsNonRenderingEventHandler<KeyboardEventArgs>( OnKeyDownHandler );

    /// <summary>
    /// Specifies the <see cref="IJSUtilitiesModule"/> instance.
    /// </summary>
    [Inject] protected IJSUtilitiesModule JSUtilitiesModule { get; set; }

    /// <summary>
    /// Specifies the document observer used while dragging outside the surface.
    /// </summary>
    [Inject] protected IDocumentObserver DocumentObserver { get; set; }

    /// <summary>
    /// Gets or sets the editable color supplied by the parent picker.
    /// </summary>
    [CascadingParameter] internal ColorPickerColor Color { get; set; }

    /// <summary>
    /// Gets or sets the color picker that owns this surface.
    /// </summary>
    [CascadingParameter] public ColorPicker Parent { get; set; }

    #endregion
}