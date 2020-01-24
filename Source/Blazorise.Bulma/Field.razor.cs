#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Bulma
{
    public partial class Field : Blazorise.Field
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            if ( !IsFields && ColumnSize != null )
            {
                builder.Append( ClassProvider.FieldColumn() );
                builder.Append( ColumnSize.Class( ClassProvider ) );
            }

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        #endregion
    }
}
