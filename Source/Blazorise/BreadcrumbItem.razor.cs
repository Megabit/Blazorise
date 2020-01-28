#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class BreadcrumbItem : BaseComponent
    {
        #region Members

        private bool active;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.BreadcrumbItem() );
            builder.Append( ClassProvider.BreadcrumbItemActive(), Active );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        [Parameter]
        public bool Active
        {
            get => active;
            set
            {
                active = value;

                DirtyClasses();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
