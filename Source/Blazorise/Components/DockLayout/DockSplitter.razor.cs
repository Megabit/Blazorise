#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// Visual splitter used by resizable dock panes.
/// </summary>
public partial class DockSplitter : BaseComponent
{
    #region Methods

    /// <inheritdoc/>
    public override Task SetParametersAsync( ParameterView parameters )
    {
        if ( parameters.TryGetValue<DockPanePosition>( nameof( Dock ), out DockPanePosition dock ) && Dock != dock )
            DirtyClasses();

        return base.SetParametersAsync( parameters );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.DockSplitter( Dock ) );

        base.BuildClasses( builder );
    }

    private Task BeginResize( PointerEventArgs eventArgs )
    {
        if ( Context is not null && !string.IsNullOrWhiteSpace( PaneName ) )
            return Context.BeginNodeResize( ElementRef, PaneName, NodeId, Dock, eventArgs, MinSize, MaxSize );

        return Context?.BeginPaneResize( ParentDockPane, NodeId, Dock, eventArgs )
            ?? ParentDockPane?.ParentDockLayout?.BeginPaneResize( ParentDockPane, NodeId, Dock, eventArgs )
            ?? Task.CompletedTask;
    }

    #endregion

    #region Properties

    [CascadingParameter] internal DockPane ParentDockPane { get; set; }

    [CascadingParameter] internal DockLayoutContext Context { get; set; }

    /// <summary>
    /// Defines the pane or node name used by resize notifications.
    /// </summary>
    [Parameter] public string PaneName { get; set; }

    /// <summary>
    /// Defines the pane side that owns this splitter.
    /// </summary>
    [Parameter] public DockPanePosition Dock { get; set; }

    /// <summary>
    /// Defines the split node that owns this splitter.
    /// </summary>
    [Parameter] public string NodeId { get; set; }

    /// <summary>
    /// Defines the minimum size allowed for the resized element.
    /// </summary>
    [Parameter] public string MinSize { get; set; }

    /// <summary>
    /// Defines the maximum size allowed for the resized element.
    /// </summary>
    [Parameter] public string MaxSize { get; set; }

    #endregion
}