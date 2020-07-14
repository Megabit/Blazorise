#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class CardLink : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.CardLink() );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Link url.
        /// </summary>
        [Parameter] public string Source { get; set; }

        /// <summary>
        /// Alternative link text.
        /// </summary>
        [Parameter] public string Alt { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
