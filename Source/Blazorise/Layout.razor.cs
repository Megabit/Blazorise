#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class Layout : BaseComponent
    {
        #region Members

        private bool sider;

        private bool bodyClassApplied;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Layout() );
            builder.Append( ClassProvider.LayoutHasSider(), Sider );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Indicates that layout will contain sider.
        /// </summary>
        [Parameter]
        public bool Sider
        {
            get => sider;
            set
            {
                sider = value;

                DirtyClasses();
            }
        }

        [CascadingParameter] protected Layout ParentLayout { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
