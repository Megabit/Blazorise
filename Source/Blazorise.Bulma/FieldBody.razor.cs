#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Bulma
{
    public partial class FieldBody : Blazorise.FieldBody
    {
        #region Members

        #endregion

        #region Methods

        #endregion

        #region Properties

        protected override bool ColumnSizeSupported => false; // Bulma does not support column sizes on fields.

        #endregion
    }
}
