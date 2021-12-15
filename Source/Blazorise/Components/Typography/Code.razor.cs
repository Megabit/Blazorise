#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Wrapper for code components.
    /// </summary>
    public partial class Code : BaseTypographyComponent
    {
        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Code() );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// If true, the content will be wrapped with the <c>&lt;</c> and <c>&gt;</c> tags, eg. <c>&lt;button&gt;</c>;.
        /// </summary>
        [Parameter] public bool Tag { get; set; }

        #endregion
    }
}
