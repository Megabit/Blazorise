using System;
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;

namespace Blazorise.AntDesign.Components;

public partial class DropdownMenu : Blazorise.DropdownMenu
{
    private string overlayStyleNames;

    protected string OverlayClassNames => JoinClasses( "ant-dropdown", GetRootPlacementClassName(), ClassNames );

    protected string OverlayStyleNames => JoinStyles( StyleNames, overlayStyleNames );

    protected string RootMenuClassNames => "ant-dropdown-menu ant-dropdown-menu-root ant-dropdown-menu-vertical ant-dropdown-menu-light";

    protected string SubmenuPopupClassNames => JoinClasses( "ant-dropdown-menu-submenu ant-dropdown-menu-submenu-popup", GetSubmenuPlacementClassName(), ClassNames );

    protected string SubmenuMenuClassNames => "ant-dropdown-menu ant-dropdown-menu-sub ant-dropdown-menu-vertical ant-dropdown-menu-light";

    protected override async Task OnFirstAfterRenderAsync()
    {
        await base.OnFirstAfterRenderAsync();

        if ( ParentDropdown?.IsDropdownSubmenu == true || ParentDropdown is null )
            return;

        var dropdownElementInfo = await JSUtilitiesModule.GetElementInfo( ParentDropdown.ElementRef, ParentDropdown.ElementId );

        overlayStyleNames = $"min-width: {Math.Ceiling( dropdownElementInfo.BoundingClientRect.Width )}px;";

        await InvokeAsync( StateHasChanged );
    }

    private Direction GetEffectiveDirection()
        => ParentDropdown?.IsDropdownSubmenu == true
           && ParentDropdown.Direction == Direction.Default
           && string.IsNullOrEmpty( ParentDropdown.DropdownMenuTargetId )
            ? Direction.End
            : ParentDropdown?.Direction ?? Direction.Default;

    private string GetRootPlacementClassName()
    {
        return GetEffectiveDirection() switch
        {
            Direction.Up => ParentDropdownState?.EndAligned == true ? "ant-dropdown-placement-topRight" : "ant-dropdown-placement-topLeft",
            Direction.Down or Direction.Default => ParentDropdownState?.EndAligned == true ? "ant-dropdown-placement-bottomRight" : "ant-dropdown-placement-bottomLeft",
            _ => null,
        };
    }

    private string GetSubmenuPlacementClassName()
    {
        return GetEffectiveDirection() switch
        {
            Direction.Start => "ant-dropdown-menu-submenu-placement-leftTop",
            _ => "ant-dropdown-menu-submenu-placement-rightTop",
        };
    }

    private static string JoinClasses( params string[] classNames )
    {
        var result = string.Empty;

        foreach ( var className in classNames )
        {
            if ( string.IsNullOrWhiteSpace( className ) )
                continue;

            result = string.IsNullOrEmpty( result )
                ? className
                : $"{result} {className}";
        }

        return result;
    }

    private static string JoinStyles( params string[] styleNames )
    {
        var result = string.Empty;

        foreach ( var styleName in styleNames )
        {
            if ( string.IsNullOrWhiteSpace( styleName ) )
                continue;

            result = string.IsNullOrEmpty( result )
                ? styleName
                : $"{result} {styleName}";
        }

        return result;
    }

    [Inject] public IJSUtilitiesModule JSUtilitiesModule { get; set; }
}