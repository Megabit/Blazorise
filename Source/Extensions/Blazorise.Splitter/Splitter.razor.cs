using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazorise.Splitter;

/// <summary>
/// The Split component allows splitting an area into multiple resizable sections
/// </summary>
public partial class Splitter : BaseComponent, IAsyncDisposable
{
    /// <summary>
    /// <see cref="SplitterSection"/>s
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; } = null!;

    /// <summary>
    /// Initial sizes of each element in percents or CSS values.
    /// </summary>
    [Parameter] public IEnumerable<JavascriptNumber> Sizes { get; init; }

    /// <summary>
    /// Minimum size of each element.
    /// </summary>
    [Parameter] public JavascriptNumberOrArray MinSize { get; init; }

    /// <summary>
    /// Maximum size of each element.
    /// </summary>
    [Parameter] public JavascriptNumberOrArray MaxSize { get; init; }

    /// <summary>
    /// When the split is created, if ExpandToMin is true, the minSize for each element overrides the percentage value from the sizes option.
    /// </summary>
    [Parameter] public bool? ExpandToMin { get; init; }

    /// <summary>
    /// Gutter size in pixels.
    /// </summary>
    [Parameter] public JavascriptNumber GutterSize { get; init; }

    /// <summary>
    /// Determines how the gutter aligns between the two elements.
    /// </summary>
    [Parameter] public SplitGutterAlignment GutterAlign { get; init; } = SplitGutterAlignment.Center;

    /// <summary>
    /// Snap to minimum size offset in pixels.
    /// </summary>
    [Parameter] public JavascriptNumberOrArray SnapOffset { get; init; }

    /// <summary>
    /// Drag this number of pixels at a time. Defaults to 1 for smooth dragging, but can be set to a pixel value to give more control over the resulting sizes.
    /// Works particularly well when the gutterSize is set to the same size.
    /// </summary>
    [Parameter] public JavascriptNumber DragInterval { get; init; }

    /// <summary>
    /// Direction to split.
    /// </summary>
    [Parameter] public SplitDirection Direction { get; init; } = SplitDirection.Horizontal;

    /// <summary>
    /// Cursor to display while dragging.
    /// </summary>
    [Parameter] public string Cursor { get; init; }

    /// <summary>
    /// Gets or sets the <see cref="JSSplitModule"/> instance.
    /// </summary>
    protected JSSplitModule JSModule { get; private set; }

    /// <summary>
    /// Gets or sets the reference to the JS split instance
    /// </summary>
    protected IJSObjectReference JSSplitInstance { get; private set; }

    /// <summary>
    /// Gets or sets the JS runtime.
    /// </summary>
    [Inject] private IJSRuntime JSRuntime { get; set; }

    /// <summary>
    /// Gets or sets the version provider.
    /// </summary>
    [Inject] private IVersionProvider VersionProvider { get; set; }

    private readonly List<ElementReference> _splitSections = new();

    // Used to ensure we're only ever able to create a single instance despite multi-threaded rendering
    private readonly SemaphoreSlim _createInstanceLock = new( 1, 1 );

    /// <inheritdoc/>
    protected override Task OnInitializedAsync()
    {
        JSModule ??= new JSSplitModule(JSRuntime, VersionProvider);
        return base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        await CreateInstance();
    }

    public void RegisterSection( ElementReference section )
    {
        _splitSections.Add( section );
        StateHasChanged();
    }

    public void UnregisterSection( ElementReference section )
    {
        _splitSections.Remove( section );
        StateHasChanged();
    }

    private async Task CreateInstance()
    {
        await _createInstanceLock.WaitAsync();

        try
        {
            if ( JSSplitInstance != null )
                await JSSplitInstance.InvokeVoidAsync( "destroy" );

            if ( _splitSections.Count > 0 )
            {
                var options = new SplitterConfiguration
                {
                    Sizes        = Sizes,
                    MinSize      = MinSize,
                    MaxSize      = MaxSize,
                    ExpandToMin  = ExpandToMin,
                    GutterSize   = GutterSize,
                    GutterAlign  = GutterAlign,
                    SnapOffset   = SnapOffset,
                    DragInterval = DragInterval,
                    Direction    = Direction,
                    Cursor       = Cursor
                };
                JSSplitInstance = await JSModule.InitializeSplit( _splitSections, options );
            }
        }
        finally
        {
            _createInstanceLock.Release();
        }
    }
}