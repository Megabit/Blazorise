#region Using directives
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Ribbon;

/// <summary>
/// Coordinates a ribbon and backstage while preserving the document content beneath the backstage overlay.
/// </summary>
public partial class RibbonWorkspace : BaseComponent, IDisposable
{
    #region Members

    private Div containerRef;

    private RibbonBackstage backstage;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes the container that the backstage overlay covers.
    /// </summary>
    public RibbonWorkspace()
    {
        Position = Blazorise.Position.Relative;
        Width = Blazorise.Width.Px().Min( 0 );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        SynchronizeState();

        base.OnParametersSet();
    }

    /// <inheritdoc/>
    protected override Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender )
        {
            ElementRef = containerRef.ElementRef;
        }

        return base.OnAfterRenderAsync( firstRender );
    }

    internal void RegisterBackstage( RibbonBackstage backstage )
    {
        if ( this.backstage is not null && !ReferenceEquals( this.backstage, backstage ) )
        {
            throw new InvalidOperationException( "RibbonWorkspace supports only one RibbonBackstage." );
        }

        this.backstage = backstage;
        Refresh();
    }

    internal void UnregisterBackstage( RibbonBackstage backstage )
    {
        if ( ReferenceEquals( this.backstage, backstage ) )
        {
            this.backstage = null;
            Refresh();
        }
    }

    internal void Refresh()
    {
        if ( !Disposed && !AsyncDisposed )
        {
            SynchronizeState();

            _ = InvokeAsync( StateHasChanged );
        }
    }

    internal async Task SetBackstageVisible( bool visible )
    {
        if ( BackstageVisible == visible )
        {
            return;
        }

        BackstageVisible = visible;
        SynchronizeState();

        await BackstageVisibleChanged.InvokeAsync( visible );
        await InvokeAsync( StateHasChanged );
    }

    private void SynchronizeState()
    {
        var nextState = new RibbonWorkspaceState
        {
            BackstageVisible = BackstageVisible,
            BackstageElementId = backstage?.ElementId,
        };

        if ( State != nextState )
        {
            State = nextState;
        }
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Shares the workspace's backstage association and visibility with its descendants.
    /// </summary>
    protected RibbonWorkspaceState State { get; private set; } = new();

    /// <summary>
    /// Controls whether the workspace's backstage is shown. Defaults to <c>false</c>.
    /// </summary>
    [Parameter] public bool BackstageVisible { get; set; }

    /// <summary>
    /// Occurs when a workspace command or backstage requests a visibility change.
    /// </summary>
    [Parameter] public EventCallback<bool> BackstageVisibleChanged { get; set; }

    /// <summary>
    /// Defines the ribbon, document content, and optional backstage in their natural layout.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}