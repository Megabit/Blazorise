#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Breadcrumbs allow users to make selections from a range of values.
    /// </summary>
    public partial class Breadcrumb : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Breadcrumb() );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Defines the breadcrumb activation mode.
        /// </summary>
        [Parameter] public BreadcrumbMode Mode { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
