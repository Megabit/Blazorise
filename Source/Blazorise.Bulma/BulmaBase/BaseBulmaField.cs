#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Bulma.BulmaBase
{
    public abstract class BaseBulmaField : Blazorise.Base.BaseField
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .If( () => ClassProvider.FieldColumn(), () => !IsFields && ColumnSize != null )
                .If( () => ColumnSize.Class( ClassProvider ), () => !IsFields && ColumnSize != null );

            base.RegisterClasses();
        }

        #endregion

        #region Properties

        #endregion
    }
}
