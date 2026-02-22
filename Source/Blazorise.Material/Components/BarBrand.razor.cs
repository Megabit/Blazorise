#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise.Material.Components;

public partial class BarBrand : Blazorise.BarBrand
{
    #region Methods

    /// <inheritdoc/>
    protected override void BuildBarMobileToggleClasses( ClassBuilder builder )
    {
        builder.Append( "mui-bar-mobile-toggle" );
    }

    #endregion
}