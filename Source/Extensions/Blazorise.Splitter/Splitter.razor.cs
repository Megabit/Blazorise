#region Using directives
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Splitter;

/// <summary>
/// The Split component allows splitting an area into multiple resizable sections.
/// </summary>
public partial class Splitter : BaseComponent, IAsyncDisposable
{
    #region Members

    private readonly List<ElementReference> splitSections = new();

    // Used to ensure we're only ever able to create a single instance despite multi-threaded rendering.
    private readonly SemaphoreSlim createInstanceLock = new( 1, 1 );

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override Task OnInitializedAsync()
    {
        JSModule ??= new JSSplitModule( JSRuntime, VersionProvider );

        return base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        await CreateInstance();
    }

    /// <inheritdoc />
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "split" );

        if ( Direction == SplitDirection.Vertical )
            builder.Append( "split-vertical" );
        else
            builder.Append( "split-horizontal" );

        base.BuildClasses( builder );
    }

    public void RegisterSection( ElementReference section )
    {
        splitSections.Add( section );

        InvokeAsync( StateHasChanged );
    }

    public void UnregisterSection( ElementReference section )
    {
        splitSections.Remove( section );

        InvokeAsync( StateHasChanged );
    }

    private async Task CreateInstance()
    {
        await createInstanceLock.WaitAsync();

        try
        {
            if ( JSSplitInstance is not null )
                await JSSplitInstance.InvokeVoidAsync( "destroy" );

            if ( splitSections.Count > 0 )
            {
                var options = new SplitterOptions
                {
                    Sizes = Sizes,
                    MinSize = MinSize,
                    MaxSize = MaxSize,
                    ExpandToMin = ExpandToMin,
                    GutterSize = GutterSize,
                    GutterAlign = GutterAlign,
                    SnapOffset = SnapOffset,
                    DragInterval = DragInterval,
                    Direction = Direction,
                    Cursor = Cursor,
                };

                JSSplitInstance = await JSModule.InitializeSplit( splitSections, options );
            }
        }
        finally
        {
            createInstanceLock.Release();
        }
    }

    #endregion

    #region Properties

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
    /// Gutter size in pixels. Defaults to 10.
    /// </summary>
    [Parameter] public JavascriptNumber GutterSize { get; init; } = 10;

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
    /// Works particularly well when the <see cref="GutterSize"/> is set to the same size.
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
    /// Specifies the content to be rendered inside this <see cref="Splitter"/>.
    /// </summary>
    [Parameter, EditorRequired] public RenderFragment ChildContent { get; set; }

    #endregion
}