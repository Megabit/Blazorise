#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise.Material.Components;

public partial class BarIcon : Blazorise.BarIcon
{
    #region Methods

    protected override void BuildClasses( ClassBuilder builder )
    {
        base.BuildClasses( builder );

        builder.Append( "mui-bar-icon" );
    }

    #endregion
}