#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class Image : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// The absolute or relative URL of the image.
        /// </summary>
        [Parameter] public string Source { get; set; }

        /// <summary>
        /// Alternate text for an image.
        /// </summary>
        [Parameter] public string Text { get; set; }

        #endregion
    }
}
