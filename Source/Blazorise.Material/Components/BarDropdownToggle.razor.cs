#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
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

    /// <inheritdoc/>
    protected override void BuildToggleIconContainerClasses( ClassBuilder builder )
    {
        builder.Append( "mui-bar-dropdown-toggle-icon-container" );
    }

    /// <inheritdoc/>
    protected override void BuildToggleIconLayerClasses( ClassBuilder builder, bool expandedStateLayer )
    {
        var isExpanded = ParentBarDropdown?.IsVisible == true;

        builder.Append( "mui-bar-dropdown-toggle-icon-layer" );

        if ( expandedStateLayer )
        {
            builder.Append( "mui-bar-dropdown-toggle-icon-layer-visible", isExpanded );
            builder.Append( "mui-bar-dropdown-toggle-icon-layer-hidden-expand", !isExpanded );
        }
        else
        {
            builder.Append( "mui-bar-dropdown-toggle-icon-layer-hidden-collapse", isExpanded );
            builder.Append( "mui-bar-dropdown-toggle-icon-layer-visible", !isExpanded );
        }
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