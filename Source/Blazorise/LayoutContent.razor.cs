#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class LayoutContent : BaseComponent
    {
        #region Members

        private bool @fixedHeader;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( "b-layout-content" );
            builder.Append( "b-layout-content-header-fixed", FixedHeader );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        [Parameter]
        public bool FixedHeader
        {
            get => @fixedHeader;
            set
            {
                @fixedHeader = value;

                DirtyClasses();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
