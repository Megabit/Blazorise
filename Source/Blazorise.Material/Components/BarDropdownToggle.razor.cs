#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Material.Components;

public partial class BarDropdownToggle : Blazorise.BarDropdownToggle
{
    #region Methods

    protected override async Task ClickHandler( MouseEventArgs eventArgs )
    {
        if ( IsDisabled )
            return;

        if ( ParentBarDropdown is not null && ( IsToggleClickTriggerEnabled || ShouldToggleVisibleOnToggleClickMaterial ) )
            await ParentBarDropdown.Toggle( ElementId );

        await Clicked.InvokeAsync( eventArgs );
    }

    #endregion

    #region Properties

    protected bool ShouldToggleVisibleOnToggleClickMaterial
        => ParentBarDropdown is not null
            && ParentBarDropdown.IsVisible
            && ParentBarDropdownState?.Mode != BarMode.Horizontal;

    protected override bool ShouldPreventDefaultOnToggleClick
        => HasNavigationTarget && ( IsToggleClickTriggerEnabled || ShouldToggleVisibleOnToggleClickMaterial );

    protected override IconName ExpandedToggleIconName => Theme?.BarOptions?.DropdownOptions?.ToggleExpandIconName ?? IconName.AngleUp;

    protected override IconName CollapsedToggleIconName => Theme?.BarOptions?.DropdownOptions?.ToggleCollapseIconName ?? IconName.AngleDown;

    protected override IconSize ToggleIconSize => Theme?.BarOptions?.DropdownOptions?.ToggleIconSIze ?? IconSize.Default;

    #endregion
}