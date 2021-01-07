#region Using directives
using Blazorise.Utilities;
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

        /// <summary>
        /// If true header will be fixed to the top of the page.
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
