namespace Blazorise.AntDesign
{
    public partial class DropdownMenu : Blazorise.DropdownMenu
    {
        #region Methods

        protected override void OnVisibleChanged( object sender, bool e )
        {
            ExecuteAfterRender( async () =>
            {
                if ( ParentDropdown != null && ParentDropdown is AntDesign.Dropdown dropdown )
                {
                    var dropdownMenuElementInfo = await JSRunner.GetElementInfo( ElementRef, ElementId );

                    MenuStyleNames = GetMenuStyleNames( dropdown.ElementInfo, dropdownMenuElementInfo, dropdown.Direction );

                    await InvokeAsync( StateHasChanged );
                }
            } );

            base.OnVisibleChanged( sender, e );
        }

        private string GetMenuStyleNames( DomElement dropdownElementInfo, DomElement dropdownMenuElementInfo, Direction direction )
        {
            var dropdownBoundingClientRect = dropdownElementInfo.BoundingClientRect;
            var dropdownMenuBoundingClientRect = dropdownMenuElementInfo.BoundingClientRect;

            if ( direction == Direction.Up )
            {
                return $"{StyleNames} min-width: {(int)dropdownBoundingClientRect.Width}px; left: {(int)dropdownElementInfo.OffsetLeft}px; top: {(int)( dropdownElementInfo.OffsetTop - dropdownMenuBoundingClientRect.Height )}px;";
            }
            else if ( direction == Direction.Left )
            {
                return $"{StyleNames} min-width: {(int)dropdownBoundingClientRect.Width}px; left: {(int)( dropdownElementInfo.OffsetLeft - dropdownMenuBoundingClientRect.Width )}px; top: {(int)dropdownElementInfo.OffsetTop}px;";
            }
            else if ( direction == Direction.Right )
            {
                return $"{StyleNames} min-width: {(int)dropdownBoundingClientRect.Width}px; left: {(int)( dropdownElementInfo.OffsetLeft + dropdownMenuBoundingClientRect.Width )}px; top: {(int)dropdownElementInfo.OffsetTop}px;";
            }

            return $"{StyleNames} min-width: {(int)dropdownBoundingClientRect.Width}px; left: {(int)dropdownElementInfo.OffsetLeft}px; top: {(int)( dropdownElementInfo.OffsetTop + dropdownBoundingClientRect.Height )}px;";
        }

        #endregion

        #region Properties

        protected string MenuStyleNames { get; set; }

        #endregion
    }
}
