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

    private bool showGutter;

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

        await DocumentObserver.EnsureInitializedAsync();
        await JSModule.Initialize( dotNetObjectRef, ElementRef, ElementId, CreateOptions() );

        await base.OnFirstAfterRenderAsync();
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.ResizeHandle() );
        builder.Append( ClassProvider.ResizeHandleOrientation( Orientation ) );
        builder.Append( ClassProvider.ResizeHandlePlacement( ResolvedPlacement ) );
        builder.Append( ClassProvider.ResizeHandleGutter( ShowGutter ) );
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
    /// Receives notification that a pointer or keyboard resize has started.
    /// </summary>
    /// <param name="eventArgs">Information about the resize operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public virtual Task OnResizeStarted( ResizeHandleEventArgs eventArgs )
        => ResizeStarted.InvokeAsync( eventArgs );

    /// <summary>
    /// Forwards a throttled size update from the active interaction.
    /// </summary>
    /// <param name="eventArgs">Information about the resize operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public virtual Task OnResizing( ResizeHandleEventArgs eventArgs )
        => Resizing.InvokeAsync( eventArgs );

    /// <summary>
    /// Commits the final size and reports completion of the interaction.
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
    /// Chooses a placement that is valid for the current orientation.
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
    /// Resolves the configured CSS property, falling back to width or height for the active axis.
    /// </summary>
    protected string ResolvedResizeProperty
        => !string.IsNullOrWhiteSpace( ResizeProperty )
            ? ResizeProperty
            : Orientation == Blazorise.Orientation.Vertical ? "width" : "height";

    /// <summary>
    /// Maps the component orientation to its ARIA representation.
    /// </summary>
    protected string AriaOrientation
        => Orientation == Blazorise.Orientation.Vertical ? "vertical" : "horizontal";

    /// <summary>
    /// Formats the minimum size for the rendered separator semantics.
    /// </summary>
    protected string AriaValueMin
        => FormatAriaValue( MinSize );

    /// <summary>
    /// Formats the optional maximum size for assistive technologies.
    /// </summary>
    protected string AriaValueMax
        => FormatAriaValue( MaxSize );

    /// <summary>
    /// Supplies the initial accessible size; JavaScript keeps the value synchronized during resizing.
    /// </summary>
    protected string AriaValueNow
        => FormatAriaValue( Size );

    /// <summary>
    /// Converts the disabled state to the lowercase ARIA boolean format.
    /// </summary>
    protected string AriaDisabled
        => Disabled.ToString().ToLowerInvariant();

    /// <summary>
    /// Removes disabled handles from the tab order and otherwise uses <see cref="TabIndex"/>.
    /// </summary>
    protected int ResolvedTabIndex
        => Disabled ? -1 : TabIndex;

    /// <summary>
    /// Provides the shared document-event infrastructure used by the JavaScript interaction.
    /// </summary>
    [Inject] protected IDocumentObserver DocumentObserver { get; set; }

    /// <summary>
    /// Performs pointer, keyboard, focus, and target-sizing operations in the browser.
    /// </summary>
    [Inject] public IJSResizeHandleModule JSModule { get; set; }

    /// <summary>
    /// Identifies the element that receives size updates. When omitted, the handle resizes its parent.
    /// </summary>
    [Parameter] public string TargetElementId { get; set; }

    /// <summary>
    /// Controls the resize axis. Vertical handles change width, while horizontal handles change height.
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
    /// Positions the handle on the target edge. Vertical handles default to end and horizontal handles default to bottom.
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
    /// Names the CSS property updated during resizing, including custom properties such as <c>--panel-width</c>.
    /// </summary>
    [Parameter] public string ResizeProperty { get; set; }

    /// <summary>
    /// Supplies the controlled size in pixels. Without a value, the browser measures the target's initial size.
    /// </summary>
    [Parameter] public double? Size { get; set; }

    /// <summary>
    /// Occurs when a resize interaction commits a new size.
    /// </summary>
    [Parameter] public EventCallback<double> SizeChanged { get; set; }

    /// <summary>
    /// Prevents the target from shrinking below this pixel value.
    /// </summary>
    [Parameter] public double MinSize { get; set; }

    /// <summary>
    /// Limits the target's size in pixels; a null value leaves the upper bound unrestricted.
    /// </summary>
    [Parameter] public double? MaxSize { get; set; }

    /// <summary>
    /// Determines how many pixels each applicable arrow-key press adds or removes.
    /// </summary>
    [Parameter] public double KeyboardStep { get; set; } = 10;

    /// <summary>
    /// Throttles <see cref="Resizing"/> notifications to at least this many milliseconds apart.
    /// </summary>
    [Parameter] public int ResizeEventInterval { get; set; } = 100;

    /// <summary>
    /// Displays the provider-styled gutter, borders, and grip when enabled. The resize area is transparent by default.
    /// </summary>
    [Parameter]
    public bool ShowGutter
    {
        get => showGutter;
        set
        {
            if ( showGutter == value )
                return;

            showGutter = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Prevents both pointer and keyboard resizing when enabled.
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
    /// Describes the separator's purpose to assistive technologies.
    /// </summary>
    [Parameter] public string AriaLabel { get; set; } = "Resize";

    /// <summary>
    /// Controls the handle's position in the keyboard tab order while it is enabled.
    /// </summary>
    [Parameter] public int TabIndex { get; set; }

    /// <summary>
    /// Fires once when pointer or keyboard resizing begins.
    /// </summary>
    [Parameter] public EventCallback<ResizeHandleEventArgs> ResizeStarted { get; set; }

    /// <summary>
    /// Reports intermediate sizes at the cadence configured by <see cref="ResizeEventInterval"/>.
    /// </summary>
    [Parameter] public EventCallback<ResizeHandleEventArgs> Resizing { get; set; }

    /// <summary>
    /// Signals that resizing has finished, including canceled pointer interactions.
    /// </summary>
    [Parameter] public EventCallback<ResizeHandleEventArgs> ResizeEnded { get; set; }

    #endregion
}