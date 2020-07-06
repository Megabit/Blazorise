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

        private bool loading;

        private bool bodyClassApplied;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Layout() );
            builder.Append( ClassProvider.LayoutHasSider(), Sider );
            builder.Append( ClassProvider.LayoutLoading(), Loading );
            builder.Append( ClassProvider.LayoutRoot(), ParentLayout == null );

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

        [Parameter]
        public bool Loading
        {
            get => loading;
            set
            {
                loading = value;

                DirtyClasses();
            }
        }

        [Parameter] public EventCallback<bool> LoadingChanged { get; set; }

        [CascadingParameter] protected Layout ParentLayout { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
