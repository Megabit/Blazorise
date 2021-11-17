#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Wrapper for longer text inside of <see cref="Alert"/> component.
    /// </summary>
    public partial class AlertDescription : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.AlertDescription() );

            base.BuildClasses( builder );
        }

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            ParentAlert?.NotifyHasDescription();

            base.OnInitialized();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the reference to the parent <see cref="Alert"/> component.
        /// </summary>
        [CascadingParameter] protected Alert ParentAlert { get; set; }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="AlertDescription"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
