#region Using directives
using System.Threading.Tasks;
#endregion

namespace Blazorise.Ribbon;

/// <summary>
/// Visually separates related application commands inside a menu or submenu.
/// </summary>
public partial class RibbonApplicationMenuDivider : BaseComponent
{
    #region Members

    private DropdownDivider dividerRef;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender )
        {
            ElementRef = dividerRef.ElementRef;
        }

        return base.OnAfterRenderAsync( firstRender );
    }

    #endregion
}