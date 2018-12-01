#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Base;
using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Components;
#endregion

namespace Blazorise.Sidebar.Base
{
    public abstract class BaseSidebar : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( "sidebar" );

            base.RegisterClasses();
        }

        #endregion

        #region Properties

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
