#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Renders the built-in action buttons for a dock pane header.
/// </summary>
public partial class _DockPaneHeaderActions : BaseComponent
{
    #region Constructors

    /// <summary>
    /// Default <see cref="_DockPaneHeaderActions"/> constructor.
    /// </summary>
    public _DockPaneHeaderActions()
    {
        ActionClassBuilder = new( BuildActionClasses );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.DockPaneHeaderActions() );

        base.BuildClasses( builder );
    }

    private void BuildActionClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.DockPaneHeaderAction() );
    }

    private Task ToggleAutoHide()
    {
        if ( Pane?.ParentDockLayout is null )
            return Task.CompletedTask;

        return Pane.ParentDockLayout.TogglePaneAutoHide( Pane );
    }

    private Task Close()
    {
        if ( Pane?.ParentDockLayout is null )
            return Task.CompletedTask;

        return Pane.ParentDockLayout.ClosePane( Pane );
    }

    #endregion

    #region Properties

    private bool AutoHideActionVisible => Pane?.AutoHideable == true;

    private bool CloseActionVisible => Pane?.Closable == true;

    private bool Visible => AutoHideActionVisible || CloseActionVisible;

    private string AutoHideTitle => Pane?.EffectiveAutoHide == true ? "Pin" : "Auto hide";

    private IconName AutoHideIconName => Pane?.EffectiveAutoHide == true ? IconName.Expand : IconName.EyeSlash;

    private string ActionClassNames => ActionClassBuilder.Class;

    protected ClassBuilder ActionClassBuilder { get; private set; }

    /// <summary>
    /// Gets or sets the dock pane that owns the header.
    /// </summary>
    [Parameter] public DockPane Pane { get; set; }

    #endregion
}