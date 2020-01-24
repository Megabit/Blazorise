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

        private bool isActive;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.BreadcrumbItem() );
            builder.Append( ClassProvider.BreadcrumbItemActive(), IsActive );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        [Parameter]
        public bool IsActive
        {
            get => isActive;
            set
            {
                isActive = value;

                DirtyClasses();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
