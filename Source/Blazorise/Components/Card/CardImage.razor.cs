#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class CardImage : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.CardImage() );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Image url.
        /// </summary>
        [Parameter] public string Source { get; set; }

        /// <summary>
        /// Alternative image text.
        /// </summary>
        [Parameter] public string Alt { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
