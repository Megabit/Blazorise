#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseCardTitle : BaseTextComponent
    {
        #region Members

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.CardTitle() );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Number from 1 to 6 that defines the title size where the smaller number means larger text.
        /// </summary>
        /// <remarks>
        /// TODO: change to enum
        /// </remarks>
        [Parameter] public int? Size { get; set; }

        #endregion
    }
}
