#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise.FluentUI2.Components;

public partial class Field : Blazorise.Field
{
    #region Methods

    protected override void BuildClasses( ClassBuilder builder )
    {
        if ( IsFields && ColumnSize == null )
            builder.Append( ClassProvider.FieldColumn() );

        base.BuildClasses( builder );
    }

    #endregion
}