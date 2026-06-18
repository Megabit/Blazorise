#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// Header content for a <see cref="DockPanel"/>.
/// </summary>
public partial class DockPanelHeader : BaseComponent
{
    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.DockPanelHeader() );

        base.BuildClasses( builder );
    }

    private Task BeginDrag( PointerEventArgs eventArgs )
    {
        if ( ParentDockPanel?.ParentDockLayout is null )
            return Task.CompletedTask;

        return ParentDockPanel.ParentDockLayout.BeginPanelDrag( ParentDockPanel, eventArgs, true );
    }

    private Task ToggleAutoHide()
    {
        if ( ParentDockPanel?.ParentDockLayout is null )
            return Task.CompletedTask;

        return ParentDockPanel.ParentDockLayout.TogglePanelAutoHide( ParentDockPanel );
    }

    private Task Close()
    {
        if ( ParentDockPanel?.ParentDockLayout is null )
            return Task.CompletedTask;

        return ParentDockPanel.ParentDockLayout.ClosePanel( ParentDockPanel );
    }

    #endregion

    #region Properties

    [CascadingParameter] internal DockPanel ParentDockPanel { get; set; }

    private bool AutoHideActionVisible => ParentDockPanel?.AutoHideable == true;

    private bool CloseActionVisible => ParentDockPanel?.Closable == true;

    private bool HeaderActionsVisible => AutoHideActionVisible || CloseActionVisible;

    private string AutoHideTitle => ParentDockPanel?.EffectiveAutoHide == true ? "Pin" : "Auto hide";

    private IconName AutoHideIconName => ParentDockPanel?.EffectiveAutoHide == true ? IconName.Expand : IconName.EyeSlash;

    /// <summary>
    /// Specifies the header content to be rendered inside this <see cref="DockPanelHeader"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}