#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// Header content for a <see cref="DockPane"/>.
/// </summary>
public partial class DockPaneHeader : BaseComponent
{
    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.DockPaneHeader() );

        base.BuildClasses( builder );
    }

    private Task BeginDrag( PointerEventArgs eventArgs )
    {
        if ( ParentDockPane?.ParentDockLayout is null )
            return Task.CompletedTask;

        return ParentDockPane.ParentDockLayout.BeginPaneDrag( ParentDockPane, eventArgs, true );
    }

    private Task ToggleAutoHide()
    {
        if ( ParentDockPane?.ParentDockLayout is null )
            return Task.CompletedTask;

        return ParentDockPane.ParentDockLayout.TogglePaneAutoHide( ParentDockPane );
    }

    private Task Close()
    {
        if ( ParentDockPane?.ParentDockLayout is null )
            return Task.CompletedTask;

        return ParentDockPane.ParentDockLayout.ClosePane( ParentDockPane );
    }

    #endregion

    #region Properties

    [CascadingParameter] internal DockPane ParentDockPane { get; set; }

    private bool AutoHideActionVisible => ParentDockPane?.AutoHideable == true;

    private bool CloseActionVisible => ParentDockPane?.Closable == true;

    private bool HeaderActionsVisible => AutoHideActionVisible || CloseActionVisible;

    private string AutoHideTitle => ParentDockPane?.EffectiveAutoHide == true ? "Pin" : "Auto hide";

    private IconName AutoHideIconName => ParentDockPane?.EffectiveAutoHide == true ? IconName.Expand : IconName.EyeSlash;

    /// <summary>
    /// Specifies the header content to be rendered inside this <see cref="DockPaneHeader"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}