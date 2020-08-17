#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.Utils;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Checkboxes allow the user to select one or more items from a set.
    /// </summary>
    /// <typeparam name="TValue">Checked value type.</typeparam>
    public partial class Check<TValue> : BaseCheckComponent<TValue>
    {
        #region Members

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Check() );
            builder.Append( ClassProvider.CheckSize( Size ), Size != Size.None );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        protected override string TrueValueName => "true";

        #endregion
    }
}
