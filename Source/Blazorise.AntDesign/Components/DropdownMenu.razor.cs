using Blazorise.Modules;
using Microsoft.AspNetCore.Components;

namespace Blazorise.AntDesign.Components;

public partial class DropdownMenu : Blazorise.DropdownMenu
{
    #region Methods

    internal protected override void OnVisibleChanged( bool e )
    {
        ExecuteAfterRender( async () =>
        {
            if ( ParentDropdown != null && ParentDropdown is AntDesign.Components.Dropdown dropdown )
            {
                var dropdownMenuElementInfo = await JSUtilitiesModule.GetElementInfo( ElementRef, ElementId );

                MenuStyleNames = GetMenuStyleNames( dropdown.ElementInfo, dropdownMenuElementInfo, dropdown.Direction );

                await InvokeAsync( StateHasChanged );
            }
        } );

        base.OnVisibleChanged( e );
    }

    private string GetMenuStyleNames( DomElement dropdownElementInfo, DomElement dropdownMenuElementInfo, Direction direction )
    {
        var dropdownBoundingClientRect = dropdownElementInfo.BoundingClientRect;
        var dropdownMenuBoundingClientRect = dropdownMenuElementInfo.BoundingClientRect;

        if ( direction == Direction.Up )
        {
            return $"{StyleNames} min-width: {(int)dropdownBoundingClientRect.Width}px; left: {(int)dropdownElementInfo.OffsetLeft}px; top: {(int)( dropdownElementInfo.OffsetTop - dropdownMenuBoundingClientRect.Height )}px;";
        }
        else if ( direction == Direction.Start )
        {
            return $"{StyleNames} min-width: {(int)dropdownBoundingClientRect.Width}px; left: {(int)( dropdownElementInfo.OffsetLeft - dropdownMenuBoundingClientRect.Width )}px; top: {(int)dropdownElementInfo.OffsetTop}px;";
        }
        else if ( direction == Direction.End )
        {
            return $"{StyleNames} min-width: {(int)dropdownBoundingClientRect.Width}px; left: {(int)( dropdownElementInfo.OffsetLeft + dropdownMenuBoundingClientRect.Width )}px; top: {(int)dropdownElementInfo.OffsetTop}px;";
        }

        return $"{StyleNames} min-width: {(int)dropdownBoundingClientRect.Width}px; left: {(int)dropdownElementInfo.OffsetLeft}px; top: {(int)( dropdownElementInfo.OffsetTop + dropdownBoundingClientRect.Height )}px;";
    }

    #endregion

    #region Properties

    protected string MenuStyleNames { get; set; }

    [Inject] public IJSUtilitiesModule JSUtilitiesModule { get; set; }

    #endregion
}