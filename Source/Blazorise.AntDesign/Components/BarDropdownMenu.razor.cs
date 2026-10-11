#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.AntDesign.Components;

public partial class BarDropdownMenu
{
    #region Methods

    protected override void BuildContainerClasses( ClassBuilder builder )
    {
        base.BuildContainerClasses( builder );

        if ( !IsPopup )
        {
            return;
        }

        builder.Append( "ant-menu-submenu ant-menu-submenu-popup" );

        if ( IsHorizontalPopup )
        {
            builder.Append( ParentDropdownState.RightAligned ? "ant-menu-submenu-placement-bottomRight" : "ant-menu-submenu-placement-bottomLeft" );
        }
        else
        {
            builder.Append( ParentDropdownState.RightAligned ? "ant-menu-submenu-placement-leftTop" : "ant-menu-submenu-placement-rightTop" );
        }

        builder.Append( "ant-menu-submenu-hidden", !ParentDropdownState.Visible );
    }

    #endregion

    #region Properties

    protected bool IsPopup => ParentDropdownState is not null && !ParentDropdownState.IsInlineDisplay;

    protected bool IsHorizontalPopup => ParentDropdownState?.Mode == BarMode.Horizontal && ParentDropdownState?.NestedIndex <= 1;

    [CascadingParameter( Name = "SubMenuId" )] protected string SubMenuId { get; set; }

    #endregion
}