#region Using directives
using System;
using System.Globalization;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Modules;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise;

/// <summary>
/// A draggable separator that resizes its parent element or another element identified by ID.
/// </summary>
public partial class ResizeHandle : BaseComponent, IAsyncDisposable
{
    #region Members

    private DotNetObjectReference<ResizeHandle> dotNetObjectRef;

    private Orientation orientation = Blazorise.Orientation.Horizontal;

    private Placement? placement;

    private bool disabled;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        parameters.TryGetParameter( ResizeStarted, out ComponentParameterInfo<EventCallback<ResizeHandleEventArgs>> paramResizeStarted );
        parameters.TryGetParameter( Resizing, out ComponentParameterInfo<EventCallback<ResizeHandleEventArgs>> paramResizing );
        parameters.TryGetParameter( ResizeEnded, out ComponentParameterInfo<EventCallback<ResizeHandleEventArgs>> paramResizeEnded );
        parameters.TryGetParameter( SizeChanged, out ComponentParameterInfo<EventCallback<double>> paramSizeChanged );

        EventCallback<ResizeHandleEventArgs> nextResizeStarted = paramResizeStarted.GetValueOrDefault( ResizeStarted );
        EventCallback<ResizeHandleEventArgs> nextResizing = paramResizing.GetValueOrDefault( Resizing );
        EventCallback<ResizeHandleEventArgs> nextResizeEnded = paramResizeEnded.GetValueOrDefault( ResizeEnded );
        EventCallback<double> nextSizeChanged = paramSizeChanged.GetValueOrDefault( SizeChanged );

        bool updateOptions = Rendered
            && ( parameters.IsParameterChanged( TargetElementId )
                || parameters.IsParameterChanged( Orientation )
                || parameters.IsParameterChanged( Placement )
                || parameters.IsParameterChanged( ResizeProperty )
                || parameters.IsParameterChanged( Size )
                || parameters.IsParameterChanged( MinSize )
                || parameters.IsParameterChanged( MaxSize )
                || parameters.IsParameterChanged( KeyboardStep )
                || parameters.IsParameterChanged( ResizeEventInterval )
                || parameters.IsParameterChanged( Disabled )
                || nextResizeStarted.HasDelegate != ResizeStarted.HasDelegate
                || nextResizing.HasDelegate != Resizing.HasDelegate
                || ( nextResizeEnded.HasDelegate || nextSizeChanged.HasDelegate ) != ( ResizeEnded.HasDelegate || SizeChanged.HasDelegate ) );

        await base.SetParametersAsync( parameters );

        if ( updateOptions )
            ExecuteAfterRender( async () => await JSModule.UpdateOptions( ElementRef, ElementId, CreateOptions() ) );
    }

    /// <inheritdoc/>
    protected override async Task OnFirstAfterRenderAsync()
    {
        dotNetObjectRef = CreateDotNetObjectRef( this );

        await JSModule.Initialize( dotNetObjectRef, ElementRef, ElementId, CreateOptions() );

        await base.OnFirstAfterRenderAsync();
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.ResizeHandle() );
        builder.Append( ClassProvider.ResizeHandleOrientation( Orientation ) );
        builder.Append( ClassProvider.ResizeHandlePlacement( ResolvedPlacement ) );
        builder.Append( ClassProvider.ResizeHandleDisabled( Disabled ) );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing )
        {
            if ( Rendered )
                await JSModule.SafeDestroy( ElementRef, ElementId );

            DisposeDotNetObjectRef( dotNetObjectRef );
            dotNetObjectRef = null;
        }

        await base.DisposeAsync( disposing );
    }

    /// <summary>
    /// Handles the start of a resize interaction.
    /// </summary>
    /// <param name="eventArgs">Information about the resize operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public virtual Task OnResizeStarted( ResizeHandleEventArgs eventArgs )
        => ResizeStarted.InvokeAsync( eventArgs );

    /// <summary>
    /// Handles a throttled resize interaction update.
    /// </summary>
    /// <param name="eventArgs">Information about the resize operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public virtual Task OnResizing( ResizeHandleEventArgs eventArgs )
        => Resizing.InvokeAsync( eventArgs );

    /// <summary>
    /// Handles the end of a resize interaction.
    /// </summary>
    /// <param name="eventArgs">Information about the resize operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public virtual async Task OnResizeEnded( ResizeHandleEventArgs eventArgs )
    {
        if ( !eventArgs.Canceled )
            await SizeChanged.InvokeAsync( eventArgs.Size );

        await ResizeEnded.InvokeAsync( eventArgs );
    }

    private ResizeHandleJSOptions CreateOptions()
        => new()
        {
            TargetElementId = TargetElementId,
            Vertical = Orientation == Blazorise.Orientation.Vertical,
            ResizeFromStart = ResolvedPlacement is Blazorise.Placement.Start or Blazorise.Placement.Top,
            ResizeProperty = ResolvedResizeProperty,
            Size = Size,
            MinSize = MinSize,
            MaxSize = MaxSize,
            KeyboardStep = KeyboardStep,
            ResizeEventInterval = ResizeEventInterval,
            Disabled = Disabled,
            ResizingClassNames = ClassProvider.ResizeHandleResizing( true ),
            TargetResizingClassNames = ClassProvider.ResizeHandleTargetResizing( true ),
            NotifyResizeStarted = ResizeStarted.HasDelegate,
            NotifyResizing = Resizing.HasDelegate,
            NotifyResizeEnded = ResizeEnded.HasDelegate || SizeChanged.HasDelegate,
        };

    private static string FormatAriaValue( double? value )
        => value?.ToString( "0.####", CultureInfo.InvariantCulture );

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Gets the resolved placement of the handle.
    /// </summary>
    protected Placement ResolvedPlacement
    {
        get
        {
            if ( Orientation == Blazorise.Orientation.Vertical )
            {
                return Placement switch
                {
                    Blazorise.Placement.Start => Blazorise.Placement.Start,
                    Blazorise.Placement.End => Blazorise.Placement.End,
                    _ => Blazorise.Placement.End,
                };
            }

            return Placement switch
            {
                Blazorise.Placement.Top => Blazorise.Placement.Top,
                Blazorise.Placement.Bottom => Blazorise.Placement.Bottom,
                _ => Blazorise.Placement.Bottom,
            };
        }
    }

    /// <summary>
    /// Gets the CSS property changed while resizing.
    /// </summary>
    protected string ResolvedResizeProperty
        => !string.IsNullOrWhiteSpace( ResizeProperty )
            ? ResizeProperty
            : Orientation == Blazorise.Orientation.Vertical ? "width" : "height";

    /// <summary>
    /// Gets the ARIA orientation value.
    /// </summary>
    protected string AriaOrientation
        => Orientation == Blazorise.Orientation.Vertical ? "vertical" : "horizontal";

    /// <summary>
    /// Gets the ARIA minimum value.
    /// </summary>
    protected string AriaValueMin
        => FormatAriaValue( MinSize );

    /// <summary>
    /// Gets the ARIA maximum value.
    /// </summary>
    protected string AriaValueMax
        => FormatAriaValue( MaxSize );

    /// <summary>
    /// Gets the initial ARIA current value. JavaScript keeps it synchronized while resizing.
    /// </summary>
    protected string AriaValueNow
        => FormatAriaValue( Size );

    /// <summary>
    /// Gets the ARIA disabled value.
    /// </summary>
    protected string AriaDisabled
        => Disabled.ToString().ToLowerInvariant();

    /// <summary>
    /// Gets the effective tab index.
    /// </summary>
    protected int ResolvedTabIndex
        => Disabled ? -1 : TabIndex;

    /// <summary>
    /// Specifies the JavaScript module used by the component.
    /// </summary>
    [Inject] public IJSResizeHandleModule JSModule { get; set; }

    /// <summary>
    /// Gets or sets the ID of the element to resize. When omitted, the handle's parent element is resized.
    /// </summary>
    [Parameter] public string TargetElementId { get; set; }

    /// <summary>
    /// Gets or sets the orientation of the visible separator. A vertical handle changes width and a horizontal handle changes height.
    /// </summary>
    [Parameter]
    public Orientation Orientation
    {
        get => orientation;
        set
        {
            if ( orientation == value )
                return;

            orientation = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Gets or sets the edge on which the handle is positioned. Defaults to end for vertical handles and bottom for horizontal handles.
    /// </summary>
    [Parameter]
    public Placement? Placement
    {
        get => placement;
        set
        {
            if ( placement == value )
                return;

            placement = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Gets or sets the CSS property changed while resizing. CSS custom properties are supported.
    /// </summary>
    [Parameter] public string ResizeProperty { get; set; }

    /// <summary>
    /// Gets or sets the size in pixels. When omitted, the initial size is measured from the target element.
    /// </summary>
    [Parameter] public double? Size { get; set; }

    /// <summary>
    /// Occurs when a resize interaction commits a new size.
    /// </summary>
    [Parameter] public EventCallback<double> SizeChanged { get; set; }

    /// <summary>
    /// Gets or sets the minimum size in pixels.
    /// </summary>
    [Parameter] public double MinSize { get; set; }

    /// <summary>
    /// Gets or sets the maximum size in pixels. When omitted, no maximum is applied.
    /// </summary>
    [Parameter] public double? MaxSize { get; set; }

    /// <summary>
    /// Gets or sets the number of pixels applied by an arrow-key press.
    /// </summary>
    [Parameter] public double KeyboardStep { get; set; } = 10;

    /// <summary>
    /// Gets or sets the minimum interval in milliseconds between <see cref="Resizing"/> callbacks.
    /// </summary>
    [Parameter] public int ResizeEventInterval { get; set; } = 100;

    /// <summary>
    /// Gets or sets whether resizing is disabled.
    /// </summary>
    [Parameter]
    public bool Disabled
    {
        get => disabled;
        set
        {
            if ( disabled == value )
                return;

            disabled = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Gets or sets the accessible label for the separator.
    /// </summary>
    [Parameter] public string AriaLabel { get; set; } = "Resize";

    /// <summary>
    /// Gets or sets the tab index used when the handle is enabled.
    /// </summary>
    [Parameter] public int TabIndex { get; set; }

    /// <summary>
    /// Occurs when a resize interaction starts.
    /// </summary>
    [Parameter] public EventCallback<ResizeHandleEventArgs> ResizeStarted { get; set; }

    /// <summary>
    /// Occurs while resizing. Notifications are throttled by <see cref="ResizeEventInterval"/>.
    /// </summary>
    [Parameter] public EventCallback<ResizeHandleEventArgs> Resizing { get; set; }

    /// <summary>
    /// Occurs when a resize interaction ends or is canceled.
    /// </summary>
    [Parameter] public EventCallback<ResizeHandleEventArgs> ResizeEnded { get; set; }

    #endregion
}