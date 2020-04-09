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

        protected override async Task OnAfterRenderAsync( bool firstRender )
        {
            if ( firstRender )
            {
                if ( ParentLayout == null )
                {
                    await JSRunner.AddClassToBody( ClassProvider.LayoutBody() );

                    bodyClassApplied = true;
                }
            }

            await base.OnAfterRenderAsync( firstRender );
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if ( bodyClassApplied )
                {
                    _ = JSRunner.RemoveClassFromBody( ClassProvider.LayoutBody() );

                    bodyClassApplied = false;
                }
            }

            base.Dispose( disposing );
        }

        #endregion

        #region Properties

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
