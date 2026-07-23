#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// Renders a single tab for a dock pane tab group.
/// </summary>
public partial class _DockTabRenderer : BaseComponent
{
    #region Members

    private bool active;

    #endregion

    #region Constructors

    /// <summary>
    /// Default <see cref="_DockTabRenderer"/> constructor.
    /// </summary>
    public _DockTabRenderer()
    {
        LabelClassBuilder = new( BuildLabelClasses );
        CloseClassBuilder = new( BuildCloseClasses );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.DockPaneTab( Active ) );

        base.BuildClasses( builder );
    }

    private void BuildLabelClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.DockPaneTabLabel() );
    }

    private void BuildCloseClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.DockPaneTabClose() );
    }

    private Task BeginPaneTabDrag( PointerEventArgs eventArgs )
        => Context?.BeginPaneTabDrag( PaneName, eventArgs ) ?? Task.CompletedTask;

    private Task ActivateTab()
        => Context?.ActivateTab( NodeId, PaneName ) ?? Task.CompletedTask;

    private Task ClosePane()
        => Context?.ClosePane( PaneName ) ?? Task.CompletedTask;

    #endregion

    #region Properties

    private string Caption => Context?.GetPaneCaption( PaneName ) ?? PaneName;

    private string LabelClassNames => LabelClassBuilder.Class;

    private string CloseClassNames => CloseClassBuilder.Class;

    private bool CloseButtonVisible => Context?.IsPaneTabCloseButtonVisible( PaneName, GroupPosition ) == true;

    private ClassBuilder LabelClassBuilder { get; set; }

    private ClassBuilder CloseClassBuilder { get; set; }

    [CascadingParameter] internal DockLayoutContext Context { get; set; }

    /// <summary>
    /// Gets or sets the tab node id that owns the pane.
    /// </summary>
    [Parameter] public string NodeId { get; set; }

    /// <summary>
    /// Gets or sets the pane name.
    /// </summary>
    [Parameter] public string PaneName { get; set; }

    /// <summary>
    /// Gets or sets the rendered tab group position.
    /// </summary>
    [Parameter] public DockPanePosition GroupPosition { get; set; }

    /// <summary>
    /// Gets or sets whether the tab is active.
    /// </summary>
    [Parameter]
    public bool Active
    {
        get => active;
        set
        {
            if ( active == value )
                return;

            active = value;
            DirtyClasses();
        }
    }

    #endregion
}