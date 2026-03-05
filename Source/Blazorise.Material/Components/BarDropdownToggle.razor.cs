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

        // Material-specific UX: allow toggle-area click to close an already open
        // non-horizontal dropdown when the configured trigger is icon-click.
        // All other click/keyboard accessibility behavior is handled by the base implementation.
        if ( ParentBarDropdown is not null
             && ShouldToggleVisibleOnToggleClickMaterial
             && !IsToggleClickTriggerEnabled )
        {
            await ParentBarDropdown.Toggle( ElementId );
            await Clicked.InvokeAsync( eventArgs );
            return;
        }

        await base.ClickHandler( eventArgs );
    }

    #endregion

    #region Properties

    protected bool ShouldToggleVisibleOnToggleClickMaterial
        => ParentBarDropdown is not null
            && ParentBarDropdown.IsVisible
            && ParentBarDropdownState?.Mode != BarMode.Horizontal;

    protected IconName HorizontalToggleIconName
        => ParentBarDropdown?.IsBarDropdownSubmenu == true
            ? ParentBarDropdown.IsVisible ? IconName.AngleDown : IconName.AngleRight
            : ParentBarDropdown?.IsVisible == true ? ExpandedToggleIconName : CollapsedToggleIconName;

    protected override bool ShouldPreventDefaultOnToggleClick
        => HasNavigationTarget && ( IsToggleClickTriggerEnabled || ShouldToggleVisibleOnToggleClickMaterial );

    protected override IconName ExpandedToggleIconName => Theme?.BarOptions?.DropdownOptions?.ToggleExpandIconName ?? IconName.AngleUp;

    protected override IconName CollapsedToggleIconName => Theme?.BarOptions?.DropdownOptions?.ToggleCollapseIconName ?? IconName.AngleDown;

    protected override IconSize ToggleIconSize => Theme?.BarOptions?.DropdownOptions?.ToggleIconSIze ?? IconSize.Default;

    #endregion
}