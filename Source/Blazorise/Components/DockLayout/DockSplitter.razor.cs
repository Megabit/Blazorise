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
    #region Members

    private DockPanePosition dock;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.DockSplitter( Dock ) );

        base.BuildClasses( builder );
    }

    private Task BeginResize( PointerEventArgs eventArgs )
    {
        if ( ParentDockPane?.ParentDockLayout is null )
            return Task.CompletedTask;

        return ParentDockPane.ParentDockLayout.BeginPaneResize( ParentDockPane, eventArgs );
    }

    #endregion

    #region Properties

    [CascadingParameter] internal DockPane ParentDockPane { get; set; }

    /// <summary>
    /// Defines the pane side that owns this splitter.
    /// </summary>
    [Parameter]
    public DockPanePosition Dock
    {
        get => dock;
        set
        {
            if ( dock == value )
                return;

            dock = value;

            DirtyClasses();
        }
    }

    #endregion
}