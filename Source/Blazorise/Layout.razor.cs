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

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( "b-layout" );
            builder.Append( "b-layout-has-sider", Sider );

            base.BuildClasses( builder );
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

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
