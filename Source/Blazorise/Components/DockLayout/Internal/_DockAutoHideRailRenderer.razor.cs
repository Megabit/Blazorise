#region Using directives
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Renders auto-hidden pane tabs for one dock side.
/// </summary>
public partial class _DockAutoHideRailRenderer : BaseComponent
{
    #region Members

    private IReadOnlyList<DockRailItemState> items = [];

    private bool bordered;

    private DockPanePosition railPosition;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        if ( Visible )
        {
            builder.Append( ClassProvider.DockPane( RailPosition, false, true ) );
            builder.Append( ClassProvider.DockPanePosition( RailPosition ) );
            builder.Append( ClassProvider.DockPaneCollapsed( true ) );
            builder.Append( ClassProvider.DockPaneAutoHide( true ) );
            builder.Append( ClassProvider.DockPaneBordered(), Bordered );
            builder.Append( ClassProvider.DockPaneAutoHideRail( RailPosition ) );
        }

        base.BuildClasses( builder );
    }

    private Task ExpandPaneAutoHide( DockPane pane )
        => Context?.ExpandPaneAutoHide( pane ) ?? Task.CompletedTask;

    #endregion

    #region Properties

    private bool Visible => Items.Any();

    private string AutoHideTabClass => ClassProvider.DockPaneAutoHideTab( RailPosition );

    [CascadingParameter] internal DockLayoutContext Context { get; set; }

    /// <summary>
    /// Gets or sets the auto-hidden rail items.
    /// </summary>
    [Parameter]
    public IReadOnlyList<DockRailItemState> Items
    {
        get => items;
        set
        {
            items = value;
            DirtyClasses();
        }
    }

    /// <summary>
    /// Gets or sets the rail position.
    /// </summary>
    [Parameter]
    public DockPanePosition RailPosition
    {
        get => railPosition;
        set
        {
            if ( railPosition == value )
                return;

            railPosition = value;
            DirtyClasses();
        }
    }

    /// <summary>
    /// Gets or sets whether the rail is bordered.
    /// </summary>
    [Parameter]
    public bool Bordered
    {
        get => bordered;
        set
        {
            if ( bordered == value )
                return;

            bordered = value;
            DirtyClasses();
        }
    }

    #endregion
}