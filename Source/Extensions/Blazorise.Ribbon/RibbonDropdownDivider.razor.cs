#region Using directives
using System.Threading.Tasks;
#endregion

namespace Blazorise.Ribbon;

/// <summary>
/// Separates ribbon menu commands using a provider-native dropdown divider.
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