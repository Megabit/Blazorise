#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Material.Components;

public partial class BarDropdownToggle : Blazorise.BarDropdownToggle
{
    #region Methods

    protected async Task ClickHandlerMaterial( MouseEventArgs eventArgs )
    {
        if ( IsDisabled )
            return;

        if ( ParentBarDropdown is not null && ( IsToggleClickTriggerEnabled || ShouldToggleVisibleOnToggleClickMaterial ) )
            await ParentBarDropdown.Toggle( ElementId );

        await Clicked.InvokeAsync( eventArgs );
    }

    protected string GetToggleIconLayerClassMaterial( bool expandedStateLayer )
    {
        var isExpanded = ParentBarDropdown?.IsVisible == true;

        if ( expandedStateLayer )
        {
            return isExpanded
                ? "b-bar-dropdown-toggle-icon-layer mui-bar-dropdown-toggle-icon-layer b-bar-dropdown-toggle-icon-layer-visible mui-bar-dropdown-toggle-icon-layer-visible"
                : "b-bar-dropdown-toggle-icon-layer mui-bar-dropdown-toggle-icon-layer b-bar-dropdown-toggle-icon-layer-hidden-expand mui-bar-dropdown-toggle-icon-layer-hidden-expand";
        }

        return isExpanded
            ? "b-bar-dropdown-toggle-icon-layer mui-bar-dropdown-toggle-icon-layer b-bar-dropdown-toggle-icon-layer-hidden-collapse mui-bar-dropdown-toggle-icon-layer-hidden-collapse"
            : "b-bar-dropdown-toggle-icon-layer mui-bar-dropdown-toggle-icon-layer b-bar-dropdown-toggle-icon-layer-visible mui-bar-dropdown-toggle-icon-layer-visible";
    }

    #endregion

    #region Properties

    protected bool ShouldToggleVisibleOnToggleClickMaterial
        => ParentBarDropdown is not null
            && ParentBarDropdown.IsVisible
            && ParentBarDropdownState?.Mode != BarMode.Horizontal;

    protected bool ShouldPreventDefaultOnToggleClickMaterial
        => HasNavigationTarget && ( IsToggleClickTriggerEnabled || ShouldToggleVisibleOnToggleClickMaterial );

    protected override IconName ExpandedToggleIconName => Theme?.BarOptions?.DropdownOptions?.ToggleExpandIconName ?? IconName.AngleUp;

    protected override IconName CollapsedToggleIconName => Theme?.BarOptions?.DropdownOptions?.ToggleCollapseIconName ?? IconName.AngleDown;

    protected override IconSize ToggleIconSize => Theme?.BarOptions?.DropdownOptions?.ToggleIconSIze ?? IconSize.Default;

    #endregion
}