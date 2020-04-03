#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise.AntDesign
{
    public partial class Field : Blazorise.Field
    {
        #region Members

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            if ( IsFields )
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
