#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise.AntDesign.Components;

public partial class Field : Blazorise.Field
{
    #region Members

    #endregion

    #region Methods

    protected override void BuildClasses( ClassBuilder builder )
    {
        if ( IsFields )
            builder.Append( ClassProvider.FieldColumn() );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    #endregion
}