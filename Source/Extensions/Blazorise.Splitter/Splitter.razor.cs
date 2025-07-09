#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Splitter;

/// <summary>
/// The splitter component allows splitting an area into multiple resizable sections.
/// </summary>
public partial class Splitter : BaseComponent, IAsyncDisposable
{
    #region Members

    private readonly List<SplitterSection> splitterSections = new();

    // Used to ensure we're only ever able to create a single instance despite multi-threaded rendering.
    private readonly SemaphoreSlim createInstanceLock = new( 1, 1 );

    private bool recreateInstance = true;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        if ( Rendered )
        {
            var expandToMinChanged = parameters.TryGetValue<bool?>( nameof( ExpandToMin ), out var paramExpandToMin ) && !ExpandToMin.IsEqual( paramExpandToMin );
            var gutterSizeChanged = parameters.TryGetValue<double>( nameof( GutterSize ), out var paramGutterSize ) && !GutterSize.IsEqual( paramGutterSize );
            var gutterGutterBackgroundImage = parameters.TryGetValue<string>( nameof( GutterBackgroundImage ), out var paramGutterBackgroundImage ) && !GutterBackgroundImage.IsEqual( paramGutterBackgroundImage );
            var gutterAlignChanged = parameters.TryGetValue<SplitterGutterAlignment>( nameof( GutterAlign ), out var paramGutterAlign ) && !GutterAlign.IsEqual( paramGutterAlign );
            var dragIntervalChanged = parameters.TryGetValue<double>( nameof( DragInterval ), out var paramDragInterval ) && !DragInterval.IsEqual( paramDragInterval );
            var directionChanged = parameters.TryGetValue<SplitterDirection>( nameof( Direction ), out var paramDirection ) && !Direction.IsEqual( paramDirection );
            var cursorChanged = parameters.TryGetValue<string>( nameof( Cursor ), out var paramCursor ) && !Cursor.IsEqual( paramCursor );

            if ( expandToMinChanged
                || gutterSizeChanged
                || gutterGutterBackgroundImage
                || gutterAlignChanged
                || dragIntervalChanged
                || directionChanged
                || cursorChanged )
            {
                ExecuteAfterRender( CreateInstance );
            }
        }

        await base.SetParametersAsync( parameters );
    }

    /// <inheritdoc />
    protected override Task OnInitializedAsync()
    {
        JSModule ??= new JSSplitModule( JSRuntime, VersionProvider, BlazoriseOptions );

        return base.OnInitializedAsync();
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender || recreateInstance )
        {
            await CreateInstance();
        }

        await base.OnAfterRenderAsync( firstRender );
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && Rendered )
        {
            if ( JSModule is not null )
            {
                await JSModule.SafeDestroy( ElementRef, ElementId );

                await JSModule.SafeDisposeAsync();
            }
        }

        await base.DisposeAsync( disposing );
    }

    /// <inheritdoc />
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "splitter" );

        if ( Direction == SplitterDirection.Vertical )
            builder.Append( "splitter-vertical" );
        else
            builder.Append( "splitter-horizontal" );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Registers a new splitter section. This method is intended for internal framework use only and should not be called directly by user code.
    /// </summary>
    /// <param name="splitterSection">The splitter section to register.</param>
    public void RegisterSection( SplitterSection splitterSection )
    {
        splitterSections.Add( splitterSection );

        recreateInstance = true;

        if ( Rendered )
            InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Unregisters a splitter section. This method is intended for internal framework use only and should not be called directly by user code.
    /// </summary>
    /// <param name="splitterSection">The splitter section to unregister.</param>
    public void UnregisterSection( SplitterSection splitterSection )
    {
        splitterSections.Remove( splitterSection );

        recreateInstance = true;

        if ( Rendered )
            InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Updates the splitter section. This method is intended for internal framework use only and should not be called directly by user code.
    /// </summary>
    /// <param name="splitterSection">The splitter section to update.</param>
    public void UpdateSection( SplitterSection splitterSection )
    {
        recreateInstance = true;

        if ( Rendered )
            InvokeAsync( StateHasChanged );
    }

    private async Task CreateInstance()
    {
        await createInstanceLock.WaitAsync();

        try
        {
            await JSModule.Destroy( ElementRef, ElementId );

            if ( splitterSections.Count > 0 )
            {
                await JSModule.InitializeSplitter( ElementRef, ElementId, splitterSections.Select( x => x.ElementRef ), new SplitterOptions
                {
                    Sizes = splitterSections.Any( x => x.Size != null )
                        ? splitterSections.Select( x => new JavascriptNumber( x.Size ?? 0 ) ).ToArray()
                        : null,
                    MinSize = splitterSections.Select( x => new JavascriptNumber( x.MinSize ) ).ToArray(),
                    MaxSize = splitterSections.Select( x => new JavascriptNumber( x.MaxSize ) ).ToArray(),
                    ExpandToMin = ExpandToMin,
                    GutterSize = GutterSize,
                    GutterAlign = GutterAlign,
                    SnapOffset = splitterSections.Any( x => x.SnapOffset > 0 )
                        ? splitterSections.Select( x => new JavascriptNumber( x.SnapOffset ) ).ToArray()
                        : 0,
                    DragInterval = DragInterval,
                    Direction = Direction,
                    Cursor = Cursor,
                }, new SplitterGutterOptions
                {
                    BackgroundImage = GutterBackgroundImage,
                } );

                recreateInstance = false;
            }
        }
        finally
        {
            createInstanceLock.Release();
        }
    }

    #endregion

    #region Properties

    /// <inheritdoc />
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Gets or sets the <see cref="JSSplitModule"/> instance.
    /// </summary>
    protected JSSplitModule JSModule { get; private set; }

    /// <summary>
    /// Gets or sets the JS runtime.
    /// </summary>
    [Inject] private IJSRuntime JSRuntime { get; set; }

    /// <summary>
    /// Gets or sets the version provider.
    /// </summary>
    [Inject] private IVersionProvider VersionProvider { get; set; }

    /// <summary>
    /// Gets or sets the blazorise options.
    /// </summary>
    [Inject] protected BlazoriseOptions BlazoriseOptions { get; set; }

    /// <summary>
    /// When the splitter is created, if ExpandToMin is true, the minSize for each element overrides the percentage value from the sizes option.
    /// </summary>
    [Parameter] public bool? ExpandToMin { get; init; }

    /// <summary>
    /// Gutter size in pixels. Defaults to 10.
    /// </summary>
    [Parameter] public double GutterSize { get; init; } = 10;

    /// <summary>
    /// <para>
    /// Defines the custom background image for the gutter element.
    /// </para>
    /// <para>
    /// This parameter accepts either a Base64 encoded string that represents an image, or a URL that points to an image resource.
    /// </para>
    /// </summary>
    [Parameter] public string GutterBackgroundImage { get; init; }

    /// <summary>
    /// Determines how the gutter aligns between the two elements.
    /// </summary>
    [Parameter] public SplitterGutterAlignment GutterAlign { get; init; } = SplitterGutterAlignment.Center;

    /// <summary>
    /// Drag this number of pixels at a time. Defaults to 1 for smooth dragging, but can be set to a pixel value to give more control over the resulting sizes.
    /// Works particularly well when the <see cref="GutterSize"/> is set to the same size.
    /// </summary>
    [Parameter] public double DragInterval { get; init; } = 1;

    /// <summary>
    /// Direction to split.
    /// </summary>
    [Parameter] public SplitterDirection Direction { get; init; } = SplitterDirection.Horizontal;

    /// <summary>
    /// Cursor to display while dragging.
    /// </summary>
    [Parameter] public string Cursor { get; init; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="Splitter"/>.
    /// </summary>
    [Parameter, EditorRequired] public RenderFragment ChildContent { get; set; }

    #endregion
}