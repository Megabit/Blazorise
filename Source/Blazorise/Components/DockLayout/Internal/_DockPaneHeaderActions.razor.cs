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
        return Context?.TogglePaneAutoHide( Pane )
            ?? Pane?.ParentDockLayout?.TogglePaneAutoHide( Pane )
            ?? Task.CompletedTask;
    }

    private Task Close()
    {
        return Context?.ClosePane( Pane )
            ?? Pane?.ParentDockLayout?.ClosePane( Pane )
            ?? Task.CompletedTask;
    }

    #endregion

    #region Properties

    private bool AutoHideActionVisible => Pane?.AutoHideable == true;

    private bool CloseActionVisible => Pane?.Closable == true;

    private bool Visible => AutoHideActionVisible || CloseActionVisible;

    private string AutoHideTitle => Pane?.EffectiveAutoHide == true ? "Pin" : "Unpin";

    private IconName AutoHideIconName => Pane?.EffectiveAutoHide == true ? IconName.Pin : IconName.Unpin;

    private string ActionClassNames => ActionClassBuilder.Class;

    private ClassBuilder ActionClassBuilder { get; set; }

    [CascadingParameter] internal DockPane Pane { get; set; }

    [CascadingParameter] internal DockLayoutContext Context { get; set; }

    #endregion
}