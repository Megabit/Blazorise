#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise.AntDesign
{
    public partial class DropdownMenu : Blazorise.DropdownMenu
    {
        #region Members

        #endregion

        #region Methods

        #endregion

        #region Properties

        protected string MenuStyleNames
        {
            get
            {
                if ( ParentDropdown != null && ParentDropdown is AntDesign.Dropdown dropdown )
                {
                    var dropdowRect = dropdown.ElementInfo.BoundingClientRect;

                    // TODO: add top, left and rigth directions

                    return $"{StyleNames} min-width: {(int)dropdowRect.Width}px; left: {(int)dropdown.ElementInfo.OffsetLeft}px; top: {(int)( dropdown.ElementInfo.OffsetTop + dropdowRect.Height )}px;";
                }

                return StyleNames;
            }
        }

        #endregion
    }
}
