#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// A wrapper component around <see cref="Icon"/> that is used by the <see cref="Bar"/> component.
    /// </summary>
    public partial class BarIcon : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( "b-bar-icon" );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Icon name that can be either a string or <see cref="IconName"/>.
        /// </summary>
        [Parameter] public object IconName { get; set; }

        /// <summary>
        /// Suggested icon style.
        /// </summary>
        [Parameter] public IconStyle IconStyle { get; set; }

        #endregion
    }
}
