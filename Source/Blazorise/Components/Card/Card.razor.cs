#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class Card : BaseContainerComponent
    {
        #region Members

        private bool isWhiteText;

        private Background background = Background.None;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Card() );
            builder.Append( ClassProvider.CardWhiteText(), WhiteText );
            builder.Append( ClassProvider.CardBackground( Background ), Background != Background.None );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Sets the white text when using the darker background.
        /// </summary>
        [Parameter]
        public bool WhiteText
        {
            get => isWhiteText;
            set
            {
                isWhiteText = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Gets or sets the bar background color.
        /// </summary>
        [Parameter]
        public Background Background
        {
            get => background;
            set
            {
                background = value;

                DirtyClasses();
            }
        }

        #endregion
    }
}
