#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.LoadingIndicator;

/// <summary>
/// A wrapper component that adds a loading spinner or shows a loading message.
/// Fully templatable, supports two-way binding, direct use via @ref
/// </summary>
public partial class LoadingIndicator : BaseComponent<LoadingIndicatorClasses, LoadingIndicatorStyles>, IDisposable
{
    #region Members

    private ILoadingIndicatorService service;

    private bool? initializing;
    private bool initializingParameter;

    private bool? visible;
    private bool visibleParameter;

    #endregion

    #region Constructors

    public LoadingIndicator()
    {
        IndicatorClassBuilder = new( BuildIndicatorClasses, builder => builder.Append( Classes?.Indicator ) );
        IndicatorStyleBuilder = new( BuildIndicatorStyles, builder => builder.Append( Styles?.Indicator ) );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override Task SetParametersAsync( ParameterView parameters )
    {
        if ( parameters.TryGetValue( nameof( Initializing ), out bool newLoadedParameter ) && initializingParameter != newLoadedParameter )
        {
            initializing = null; // use parameter instead of local value
        }

        if ( parameters.TryGetValue( nameof( Visible ), out bool newVisibleParameter ) && visibleParameter != newVisibleParameter )
        {
            visible = null; // use parameter instead of local value
        }

        DirtyClasses();
        DirtyStyles();

        return base.SetParametersAsync( parameters );
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            Service = null;
        }

        base.Dispose( disposing );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "b-loading-indicator-wrapper" );
        builder.Append( "b-loading-indicator-wrapper-busy", Visible );
        builder.Append( "b-loading-indicator-wrapper-relative", Visible && !FullScreen );
        builder.Append( "b-loading-indicator-wrapper-inline", Inline );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void DirtyClasses()
    {
        IndicatorClassBuilder.Dirty();

        base.DirtyClasses();
    }

    /// <inheritdoc/>
    protected override void DirtyStyles()
    {
        IndicatorStyleBuilder.Dirty();

        base.DirtyStyles();
    }

    private void BuildIndicatorClasses( ClassBuilder builder )
    {
        builder.Append( "b-loading-indicator-overlay" );
        builder.Append( FullScreen ? "b-loading-indicator-overlay-fixed" : "b-loading-indicator-overlay-relative" );
        builder.Append( IndicatorPadding?.Class( ClassProvider ), IndicatorPadding != null );
        builder.Append( LoadingIndicatorPlacementToFluentFlex().Class( ClassProvider ) );
    }

    private void BuildIndicatorStyles( StyleBuilder builder )
    {
        builder.Append( $"animation:b-loading-indicator-overlay-fadein {FadeInDuration.TotalMilliseconds}ms ease-in;", FadeIn );
        builder.Append( $"background-color:{IndicatorBackground.Name}" );
        builder.Append( $"z-index:{ZIndex}", ZIndex.HasValue );
    }

    /// <summary>
    /// Show loading indicator
    /// </summary>
    public Task Show() => SetVisible( true );

    /// <summary>
    /// Hide loading indicator
    /// </summary>
    public Task Hide() => SetVisible( false );

    /// <summary>
    /// Set component Busy state
    /// </summary>
    /// <param name="value">true or false</param>
    internal async Task SetVisible( bool value )
    {
        if ( Visible != value )
        {
            visible = value;

            DirtyClasses();
            DirtyStyles();

            await VisibleChanged.InvokeAsync( value );
            await InvokeAsync( StateHasChanged );
        }
    }

    /// <summary>
    /// Set component Loaded state
    /// </summary>
    /// <param name="value">true or false</param>
    public async Task SetInitializing( bool value )
    {
        if ( Initializing != value )
        {
            initializing = value;
            await InitializingChanged.InvokeAsync( value );
            await InvokeAsync( StateHasChanged );
        }
    }

    private IFluentFlex LoadingIndicatorPlacementToFluentFlex()
    {
        var flex = IndicatorHorizontalPlacement switch
        {
            LoadingIndicatorPlacement.Start => Blazorise.Flex.JustifyContent.Start,
            LoadingIndicatorPlacement.End => Blazorise.Flex.JustifyContent.End,
            _ => Blazorise.Flex.JustifyContent.Center
        };

        return IndicatorVerticalPlacement switch
        {
            LoadingIndicatorPlacement.Top => flex.AlignItems.Start,
            LoadingIndicatorPlacement.Bottom => flex.AlignItems.End,
            _ => flex.AlignItems.Center
        };
    }

    #endregion

    #region Properties

    /// <summary>
    /// Indicator class builder.
    /// </summary>
    protected ClassBuilder IndicatorClassBuilder { get; private set; }

    /// <summary>
    /// Gets indicator class-names.
    /// </summary>
    protected string IndicatorClassNames => IndicatorClassBuilder.Class;

    /// <summary>
    /// Indicator style builder.
    /// </summary>
    protected StyleBuilder IndicatorStyleBuilder { get; private set; }

    /// <summary>
    /// Gets indicator style-names.
    /// </summary>
    protected string IndicatorStyleNames => IndicatorStyleBuilder.Styles;

    /// <summary>
    /// Workaround for issue https://github.com/dotnet/aspnetcore/issues/15311
    /// Setting svg width or height to null if it had a value before throws an exception
    /// Using string interpolation instead of declaring it in a .razor file
    /// Graphics courtesy of https://icons8.com/preloaders/en/search/spinner#
    /// </summary>
    private RenderFragment Spinner => ( builder ) =>
    {
        builder.OpenRegion( 0 );
        builder.AddMarkupContent( 1, @$"
                <svg viewBox='0 0 128 128'
                    {( !string.IsNullOrEmpty( SpinnerWidth ) ? $"width='{SpinnerWidth}'" : "" )}
                    {( !string.IsNullOrEmpty( SpinnerHeight ) ? $"height='{SpinnerHeight}'" : "" )}>
                      <g>
                          <path d = 'M38.52 33.37L21.36 16.2A63.6 63.6 0 0 1 59.5.16v24.3a39.5 39.5 0 0 0-20.98 8.92z' fill='{SpinnerColor.Name}' />
                          <path d = 'M38.52 33.37L21.36 16.2A63.6 63.6 0 0 1 59.5.16v24.3a39.5 39.5 0 0 0-20.98 8.92z' fill='{SpinnerBackground.Name}' transform='rotate(45 64 64)' />
                          <path d = 'M38.52 33.37L21.36 16.2A63.6 63.6 0 0 1 59.5.16v24.3a39.5 39.5 0 0 0-20.98 8.92z' fill='{SpinnerBackground.Name}' transform='rotate(90 64 64)' />
                          <path d = 'M38.52 33.37L21.36 16.2A63.6 63.6 0 0 1 59.5.16v24.3a39.5 39.5 0 0 0-20.98 8.92z' fill='{SpinnerBackground.Name}' transform='rotate(135 64 64)' />
                          <path d = 'M38.52 33.37L21.36 16.2A63.6 63.6 0 0 1 59.5.16v24.3a39.5 39.5 0 0 0-20.98 8.92z' fill='{SpinnerBackground.Name}' transform='rotate(180 64 64)' />
                          <path d = 'M38.52 33.37L21.36 16.2A63.6 63.6 0 0 1 59.5.16v24.3a39.5 39.5 0 0 0-20.98 8.92z' fill='{SpinnerBackground.Name}' transform='rotate(225 64 64)' />
                          <path d = 'M38.52 33.37L21.36 16.2A63.6 63.6 0 0 1 59.5.16v24.3a39.5 39.5 0 0 0-20.98 8.92z' fill='{SpinnerBackground.Name}' transform='rotate(270 64 64)' />
                          <path d = 'M38.52 33.37L21.36 16.2A63.6 63.6 0 0 1 59.5.16v24.3a39.5 39.5 0 0 0-20.98 8.92z' fill='{SpinnerBackground.Name}' transform='rotate(315 64 64)' />
                          <animateTransform attributeName = 'transform' type='rotate' values='0 64 64;45 64 64;90 64 64;135 64 64;180 64 64;225 64 64;270 64 64;315 64 64' calcMode='discrete' dur='720ms' repeatCount='indefinite' />
                      </g>
                </svg>" );
        builder.CloseRegion();
    };

    /// <summary>
    /// Service used to control this instance.
    /// </summary>
    [Parameter]
    public ILoadingIndicatorService Service
    {
        get => service;
        set
        {
            if ( value != service )
            {
                service?.Unsubscribe( this );

                service = value;

                service?.Subscribe( this );
            }
        }
    }

    /// <summary>
    /// Indicates whether component should display initializing or actual content.
    /// </summary>
    [Parameter]
    public bool Initializing
    {
        get => initializing ?? initializingParameter;
        set => initializingParameter = value;
    }

    /// <summary>
    /// Indicates whether the loading indicator is visible.
    /// </summary>
    [Parameter]
    public bool Visible
    {
        get => visible ?? visibleParameter;
        set => visibleParameter = value;
    }

    /// <summary>
    /// Occurs when Initializing state has changed.
    /// </summary>
    [Parameter] public EventCallback<bool> InitializingChanged { get; set; }

    /// <summary>
    /// Occurs when Visible state has changed.
    /// </summary>
    [Parameter] public EventCallback<bool> VisibleChanged { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="LoadingIndicator"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Busy indicator template.
    /// </summary>
    [Parameter] public RenderFragment IndicatorTemplate { get; set; }

    /// <summary>
    /// Loading state template.
    /// </summary>
    [Parameter] public RenderFragment InitializingTemplate { get; set; }

    /// <summary>
    /// Spinner background color.
    /// </summary>
    [Parameter] public Background SpinnerBackground { get; set; } = "#c0c0c0";

    /// <summary>
    /// Defines the spinner color in a HEX format.
    /// </summary>
    [Parameter] public Color SpinnerColor { get; set; } = "#000000";

    /// <summary>
    /// Defines the spinner HTML width, eg. "64px".
    /// </summary>
    [Parameter] public string SpinnerWidth { get; set; }

    /// <summary>
    /// Defines the spinner HTML height, eg. "64px".
    /// </summary>
    [Parameter] public string SpinnerHeight { get; set; } = "64px";

    /// <summary>
    /// Indicator vertical position.
    /// </summary>
    [Parameter] public LoadingIndicatorPlacement IndicatorVerticalPlacement { get; set; } = LoadingIndicatorPlacement.Middle;

    /// <summary>
    /// Indicator horizontal position.
    /// </summary>
    [Parameter] public LoadingIndicatorPlacement IndicatorHorizontalPlacement { get; set; } = LoadingIndicatorPlacement.Middle;

    /// <summary>
    /// Indicator div padding.
    /// </summary>
    [Parameter] public IFluentSpacing IndicatorPadding { get; set; }

    /// <summary>
    /// Busy screen color.
    /// </summary>
    [Parameter] public Background IndicatorBackground { get; set; } = "rgba(255, 255, 255, 0.7)";

    /// <summary>
    /// Show busy indicator full screen.
    /// </summary>
    [Parameter] public bool FullScreen { get; set; }

    /// <summary>
    /// Fade in indicator into view.
    /// </summary>
    [Parameter] public bool FadeIn { get; set; }

    /// <summary>
    /// Fade in duration in milliseconds. Default is 700 ms.
    /// </summary>
    [Parameter] public TimeSpan FadeInDuration { get; set; } = TimeSpan.FromMilliseconds( 700 );

    /// <summary>
    /// Wrap inline content.
    /// </summary>
    [Parameter] public bool Inline { get; set; }

    /// <summary>
    /// Overlay screen z-index.
    /// </summary>
    [Parameter] public int? ZIndex { get; set; }

    #endregion
}