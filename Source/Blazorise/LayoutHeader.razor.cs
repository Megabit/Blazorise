#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class LayoutHeader : BaseComponent
    {
        #region Members

        private bool @fixed;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.LayoutHeader() );
            builder.Append( ClassProvider.LayoutHeaderFixed(), Fixed );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        [Parameter]
        public bool Fixed
        {
            get => @fixed;
            set
            {
                @fixed = value;

                DirtyClasses();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
