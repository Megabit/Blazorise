#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// Visual splitter used by resizable dock panels.
/// </summary>
public partial class DockSplitter : BaseComponent
{
    #region Members

    private DockPanelPosition dock;

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
        if ( ParentDockPanel?.ParentDockLayout is null )
            return Task.CompletedTask;

        return ParentDockPanel.ParentDockLayout.BeginPanelResize( ParentDockPanel, eventArgs );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Defines the panel side that owns this splitter.
    /// </summary>
    [Parameter]
    public DockPanelPosition Dock
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

    [CascadingParameter] internal DockPanel ParentDockPanel { get; set; }

    #endregion
}