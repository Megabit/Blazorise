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
public partial class Resizer : BaseComponent, IAsyncDisposable
{
    #region Members

    private DotNetObjectReference<Resizer> dotNetObjectRef;

    private Orientation orientation = Blazorise.Orientation.Horizontal;

    private Placement? placement;

    private bool showGutter;

    private double? thickness;

    private double offset;

    private bool disabled;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        parameters.TryGetParameter( ResizeStarted, out ComponentParameterInfo<EventCallback<ResizerEventArgs>> paramResizeStarted );
        parameters.TryGetParameter( Resizing, out ComponentParameterInfo<EventCallback<ResizerEventArgs>> paramResizing );
        parameters.TryGetParameter( ResizeEnded, out ComponentParameterInfo<EventCallback<ResizerEventArgs>> paramResizeEnded );
        parameters.TryGetParameter( ValueChanged, out ComponentParameterInfo<EventCallback<double>> paramValueChanged );

        EventCallback<ResizerEventArgs> nextResizeStarted = paramResizeStarted.GetValueOrDefault( ResizeStarted );
        EventCallback<ResizerEventArgs> nextResizing = paramResizing.GetValueOrDefault( Resizing );
        EventCallback<ResizerEventArgs> nextResizeEnded = paramResizeEnded.GetValueOrDefault( ResizeEnded );
        EventCallback<double> nextValueChanged = paramValueChanged.GetValueOrDefault( ValueChanged );

        bool updateOptions = Rendered
            && ( parameters.IsParameterChanged( Targets )
                || parameters.IsParameterChanged( TargetId )
                || parameters.IsParameterChanged( Orientation )
                || parameters.IsParameterChanged( Placement )
                || parameters.IsParameterChanged( ResizeProperty )
                || parameters.IsParameterChanged( Value )
                || parameters.IsParameterChanged( Min )
                || parameters.IsParameterChanged( Max )
                || parameters.IsParameterChanged( KeyboardStep )
                || parameters.IsParameterChanged( ResizingInterval )
                || parameters.IsParameterChanged( Disabled )
                || nextResizeStarted.HasDelegate != ResizeStarted.HasDelegate
                || nextResizing.HasDelegate != Resizing.HasDelegate
                || ( nextResizeEnded.HasDelegate || nextValueChanged.HasDelegate ) != ( ResizeEnded.HasDelegate || ValueChanged.HasDelegate ) );

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
        builder.Append( ClassProvider.Resizer() );
        builder.Append( ClassProvider.ResizerOrientation( Orientation ) );
        builder.Append( ClassProvider.ResizerPlacement( ResolvedPlacement ) );
        builder.Append( ClassProvider.ResizerGutter( ShowGutter ) );
        builder.Append( ClassProvider.ResizerDisabled( Disabled ) );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void BuildStyles( StyleBuilder builder )
    {
        if ( Thickness is not null )
        {
            string thicknessValue = Thickness.Value.ToString( "0.####", CultureInfo.InvariantCulture );

            builder.Append( $"width:{thicknessValue}px", Orientation == Blazorise.Orientation.Vertical );
            builder.Append( $"height:{thicknessValue}px", Orientation == Blazorise.Orientation.Horizontal );
        }

        if ( Offset != 0 )
        {
            string offsetValue = ( -Offset ).ToString( "0.####", CultureInfo.InvariantCulture );

            builder.Append( $"top:{offsetValue}px", ResolvedPlacement == Blazorise.Placement.Top );
            builder.Append( $"bottom:{offsetValue}px", ResolvedPlacement == Blazorise.Placement.Bottom );
            builder.Append( $"inset-inline-start:{offsetValue}px", ResolvedPlacement == Blazorise.Placement.Start );
            builder.Append( $"inset-inline-end:{offsetValue}px", ResolvedPlacement == Blazorise.Placement.End );
        }

        base.BuildStyles( builder );
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
    public virtual Task OnResizeStarted( ResizerEventArgs eventArgs )
        => ResizeStarted.InvokeAsync( eventArgs );

    /// <summary>
    /// Forwards a throttled size update from the active interaction.
    /// </summary>
    /// <param name="eventArgs">Information about the resize operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public virtual Task OnResizing( ResizerEventArgs eventArgs )
        => Resizing.InvokeAsync( eventArgs );

    /// <summary>
    /// Commits the final value and reports completion of the interaction.
    /// </summary>
    /// <param name="eventArgs">Information about the resize operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public virtual async Task OnResizeEnded( ResizerEventArgs eventArgs )
    {
        if ( !eventArgs.Canceled )
            await ValueChanged.InvokeAsync( eventArgs.Size );

        await ResizeEnded.InvokeAsync( eventArgs );
    }

    private ResizerJSOptions CreateOptions()
        => new()
        {
            Targets = Targets,
            TargetId = TargetId,
            Vertical = Orientation == Blazorise.Orientation.Vertical,
            ResizeFromStart = ResolvedPlacement is Blazorise.Placement.Start or Blazorise.Placement.Top,
            ResizeProperty = ResolvedResizeProperty,
            Value = Value,
            Min = Min,
            Max = Max,
            KeyboardStep = KeyboardStep,
            ResizingInterval = ResizingInterval,
            Disabled = Disabled,
            FocusedClassNames = ClassProvider.ResizerFocused( true ),
            ResizingClassNames = ClassProvider.ResizerResizing( true ),
            TargetResizingClassNames = ClassProvider.ResizerTargetResizing( true ),
            NotifyResizeStarted = ResizeStarted.HasDelegate,
            NotifyResizing = Resizing.HasDelegate,
            NotifyResizeEnded = ResizeEnded.HasDelegate || ValueChanged.HasDelegate,
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
        => FormatAriaValue( Min );

    /// <summary>
    /// Formats the optional maximum size for assistive technologies.
    /// </summary>
    protected string AriaValueMax
        => FormatAriaValue( Max );

    /// <summary>
    /// Supplies the initial accessible size; JavaScript keeps the value synchronized during resizing.
    /// </summary>
    protected string AriaValueNow
        => FormatAriaValue( Value );

    /// <summary>
    /// Converts the disabled state to the lowercase ARIA boolean format.
    /// </summary>
    protected string AriaDisabled
        => Disabled.ToString().ToLowerInvariant();

    /// <summary>
    /// Removes disabled resizers from the tab order and otherwise uses <see cref="TabIndex"/>.
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
    [Inject] public IJSResizerModule JSModule { get; set; }

    /// <summary>
    /// Coordinates logical start and end targets while preserving their combined size. Use <see cref="TargetId"/> for one-target resizing.
    /// </summary>
    [Parameter] public ResizerTargets Targets { get; set; }

    /// <summary>
    /// Identifies the element that receives size updates. When omitted, the resizer resizes its parent.
    /// </summary>
    [Parameter] public string TargetId { get; set; }

    /// <summary>
    /// Controls the resize axis. Vertical resizers change width, while horizontal resizers change height.
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
            DirtyStyles();
        }
    }

    /// <summary>
    /// Positions the resizer on the target edge. Vertical resizers default to end and horizontal resizers default to bottom.
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
            DirtyStyles();
        }
    }

    /// <summary>
    /// Names the CSS property updated during resizing, including custom properties such as <c>--panel-width</c>.
    /// </summary>
    [Parameter] public string ResizeProperty { get; set; }

    /// <summary>
    /// Supplies the controlled resize value in pixels. Without a value, the browser measures the target's initial size.
    /// </summary>
    [Parameter] public double? Value { get; set; }

    /// <summary>
    /// Occurs when a resize interaction commits a new value.
    /// </summary>
    [Parameter] public EventCallback<double> ValueChanged { get; set; }

    /// <summary>
    /// Prevents the resize value from falling below this pixel value.
    /// </summary>
    [Parameter] public double Min { get; set; }

    /// <summary>
    /// Limits the resize value in pixels; a null value leaves the upper bound unrestricted.
    /// </summary>
    [Parameter] public double? Max { get; set; }

    /// <summary>
    /// Determines how many pixels each applicable arrow-key press adds or removes.
    /// </summary>
    [Parameter] public double KeyboardStep { get; set; } = 10;

    /// <summary>
    /// Throttles <see cref="Resizing"/> notifications to at least this many milliseconds apart.
    /// </summary>
    [Parameter] public int ResizingInterval { get; set; } = 100;

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
    /// Moves the resizer outward from its configured edge by the specified number of pixels. Negative values move it inward.
    /// </summary>
    [Parameter]
    public double Offset
    {
        get => offset;
        set
        {
            if ( offset == value )
                return;

            offset = value;

            DirtyStyles();
        }
    }

    /// <summary>
    /// Overrides the resizer thickness in pixels. Vertical resizers use it as width and horizontal resizers as height.
    /// </summary>
    [Parameter]
    public double? Thickness
    {
        get => thickness;
        set
        {
            if ( thickness == value )
                return;

            thickness = value;

            DirtyStyles();
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
    /// Controls the resizer's position in the keyboard tab order while it is enabled.
    /// </summary>
    [Parameter] public int TabIndex { get; set; }

    /// <summary>
    /// Fires once when pointer or keyboard resizing begins.
    /// </summary>
    [Parameter] public EventCallback<ResizerEventArgs> ResizeStarted { get; set; }

    /// <summary>
    /// Reports intermediate sizes at the cadence configured by <see cref="ResizingInterval"/>.
    /// </summary>
    [Parameter] public EventCallback<ResizerEventArgs> Resizing { get; set; }

    /// <summary>
    /// Signals that resizing has finished, including canceled pointer interactions.
    /// </summary>
    [Parameter] public EventCallback<ResizerEventArgs> ResizeEnded { get; set; }

    #endregion
}