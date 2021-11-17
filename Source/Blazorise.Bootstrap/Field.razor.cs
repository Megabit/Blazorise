#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise.Bootstrap
{
    public partial class Field : Blazorise.Field
    {
        #region Members

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            if ( IsFields && ColumnSize == null )
                builder.Append( ClassProvider.FieldColumn() );

            if ( ColumnSize != null )
                builder.Append( ColumnSize.Class( ClassProvider ) );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        #endregion
    }
}
