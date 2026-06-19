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
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        DirtyClasses();
        LabelClassBuilder.Dirty();
        CloseClassBuilder.Dirty();
    }

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
        => Layout?.BeginPaneTabDrag( PaneName, eventArgs ) ?? Task.CompletedTask;

    private Task ActivateTab()
        => Layout?.ActivateTab( Node, PaneName ) ?? Task.CompletedTask;

    private Task ClosePane()
        => Layout?.ClosePane( PaneName ) ?? Task.CompletedTask;

    #endregion

    #region Properties

    private bool Active => Layout?.GetActiveTabPaneName( Node ) == PaneName;

    private string Caption => Layout?.GetPaneCaption( PaneName ) ?? PaneName;

    private string LabelClassNames => LabelClassBuilder.Class;

    private string CloseClassNames => CloseClassBuilder.Class;

    private bool CloseButtonVisible => Layout?.IsPaneTabCloseButtonVisible( PaneName, GroupPosition ) == true;

    protected ClassBuilder LabelClassBuilder { get; private set; }

    protected ClassBuilder CloseClassBuilder { get; private set; }

    /// <summary>
    /// Gets or sets the owner dock layout.
    /// </summary>
    [Parameter] public DockLayout Layout { get; set; }

    /// <summary>
    /// Gets or sets the tab node that owns the pane.
    /// </summary>
    [Parameter] public DockNodeState Node { get; set; }

    /// <summary>
    /// Gets or sets the pane name.
    /// </summary>
    [Parameter] public string PaneName { get; set; }

    /// <summary>
    /// Gets or sets the rendered tab group position.
    /// </summary>
    [Parameter] public DockPanePosition GroupPosition { get; set; }

    #endregion
}