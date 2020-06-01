﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class BarLabel : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.BarLabel() );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
