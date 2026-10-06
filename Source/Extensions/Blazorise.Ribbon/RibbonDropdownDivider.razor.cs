#region Using directives
using System.Threading.Tasks;
#endregion

namespace Blazorise.Ribbon;

/// <summary>
/// Visually separates groups of commands inside a ribbon dropdown or split button menu.
/// </summary>
public partial class RibbonDropdownDivider : BaseComponent
{
    #region Members

    private DropdownDivider dividerRef;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override Task OnAfterRenderAsync( bool firstRender )
    {
        ElementRef = dividerRef.ElementRef;

        return base.OnAfterRenderAsync( firstRender );
    }

    #endregion
}