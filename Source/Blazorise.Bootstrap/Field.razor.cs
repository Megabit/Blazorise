#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise.Bootstrap;

public partial class Field : Blazorise.Field
{
    #region Members

    #endregion

    #region Methods

    protected override void BuildClasses( ClassBuilder builder )
    {
        if ( IsFields && ColumnSize == null )
            builder.Append( ClassProvider.FieldColumn() );

        base.BuildClasses( builder );
    }

    #endregion
}