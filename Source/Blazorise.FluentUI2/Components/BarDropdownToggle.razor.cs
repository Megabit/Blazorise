#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.FluentUI2.Components;

public partial class BarDropdownToggle : Blazorise.BarDropdownToggle
{
    #region Members

    private DateTime? lastToggleButtonKeyboardToggleTimestampUtc;

    #endregion

    #region Methods

    protected override void BuildStyles( StyleBuilder builder )
    {
        base.BuildStyles( builder );

        bool shouldAlignWithTopLevelItems = ParentBarDropdownState is
        {
            IsInlineDisplay: true,
            NestedIndex: 1,
        };

        builder.Append( "padding-inline-start: var(--spacingHorizontalMNudge)", shouldAlignWithTopLevelItems );
    }

    /// <summary>
    /// Handles navigation separately from submenu expansion for split items.
    /// </summary>
    protected Task NavigationClickHandler( MouseEventArgs eventArgs )
        => IsDisabled
            ? Task.CompletedTask
            : IsToggleClickTriggerEnabled
                ? ClickHandler( eventArgs )
                : OnClickHandler( eventArgs );

    /// <summary>
    /// Handles the dedicated Fluent expansion button without invoking navigation.
    /// </summary>
    protected Task ToggleButtonClickHandler( MouseEventArgs eventArgs )
    {
        if ( IsDisabled
             || ParentBarDropdown is null
             || !( IsIconClickTriggerEnabled || IsToggleClickTriggerEnabled ) )
            return Task.CompletedTask;

        if ( eventArgs.Detail == 0 && lastToggleButtonKeyboardToggleTimestampUtc.HasValue )
        {
            bool wasHandledByKeyDown = DateTime.UtcNow.Subtract( lastToggleButtonKeyboardToggleTimestampUtc.Value ).TotalMilliseconds < 500;

            lastToggleButtonKeyboardToggleTimestampUtc = null;

            if ( wasHandledByKeyDown )
                return Task.CompletedTask;
        }

        return ParentBarDropdown.Toggle( ElementId );
    }

    /// <summary>
    /// Routes split-button keyboard input through the existing BarDropdownToggle behavior.
    /// </summary>
    protected Task ToggleButtonKeyDownHandler( KeyboardEventArgs eventArgs )
    {
        if ( IsDisabled )
            return Task.CompletedTask;

        if ( eventArgs.Key == "Enter" || eventArgs.Key == "NumpadEnter" )
        {
            if ( !IsToggleClickTriggerEnabled )
                return Task.CompletedTask;

            lastToggleButtonKeyboardToggleTimestampUtc = DateTime.UtcNow;
        }

        return KeyDownHandler( eventArgs );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets an accessible label for the dedicated submenu expansion button.
    /// </summary>
    protected string ToggleAriaLabel => string.IsNullOrWhiteSpace( Title )
        ? "Toggle submenu"
        : $"Toggle {Title} submenu";

    /// <summary>
    /// Gets the role used by triggers that participate in a floating menu.
    /// </summary>
    protected string ToggleRole => IsNestedFloatingTrigger ? "menuitem" : null;

    /// <summary>
    /// Gets the role used by the navigation half of a split floating-menu item.
    /// </summary>
    protected string SplitNavigationRole => IsNestedFloatingTrigger ? "menuitem" : null;

    private bool IsNestedFloatingTrigger => ParentBarDropdown?.IsBarDropdownSubmenu == true
                                            && ParentBarDropdownState?.IsInlineDisplay != true;

    #endregion
}