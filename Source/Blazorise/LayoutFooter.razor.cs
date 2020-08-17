#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class LayoutFooter : BaseComponent
    {
        #region Members

        private bool @fixed;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.LayoutFooter() );
            builder.Append( ClassProvider.LayoutFooterFixed(), Fixed );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// If true footer will be fixed to the bottom of the page.
        /// </summary>
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
