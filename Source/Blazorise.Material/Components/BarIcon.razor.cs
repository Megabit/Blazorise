#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise.Material.Components;

public partial class BarIcon : Blazorise.BarIcon
{
    #region Methods

    /// <inheritdoc/>
    protected override void BuildBarIconClasses( ClassBuilder builder )
    {
        builder.Append( "mui-bar-icon" );
    }

    #endregion
}