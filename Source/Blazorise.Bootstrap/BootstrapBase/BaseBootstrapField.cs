#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Bootstrap.BootstrapBase
{
    public abstract class BaseBootstrapField : Blazorise.Base.BaseField
    {
        #region Members

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .If( () => ClassProvider.FieldColumn(), () => IsFields )
                .If( () => ColumnSize.Class( ClassProvider ), () => ColumnSize != null );

            base.RegisterClasses();
        }

        #endregion

        #region Properties

        #endregion
    }
}
