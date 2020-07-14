﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class AlertMessage : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.AlertMessage() );

            base.BuildClasses( builder );
        }

        protected override void OnInitialized()
        {
            ParentAlert?.NotifyHasMessage();

            base.OnInitialized();
        }

        #endregion

        #region Properties

        [CascadingParameter] protected Alert ParentAlert { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
