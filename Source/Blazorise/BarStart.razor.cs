#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class BarStart : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.BarStart( Mode ) );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        [CascadingParameter( Name = "Mode" )] protected BarMode Mode { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
