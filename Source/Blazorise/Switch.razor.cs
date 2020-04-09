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
    /// Switches toggle the state of a single setting on or off.
    /// </summary>
    /// <typeparam name="TValue">Checked value type.</typeparam>
    public partial class Switch<TValue> : BaseCheckComponent<TValue>
    {
        #region Members

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Switch() );
            builder.Append( ClassProvider.SwitchChecked( Checked?.ToString() == bool.TrueString ) );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        protected override string TrueValueName => "true";

        #endregion
    }
}
