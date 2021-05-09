﻿#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// A card is a flexible and extensible content container. It includes options for headers and footers,
    /// a wide variety of content, contextual background colors, and powerful display options.
    /// </summary>
    public partial class Card : BaseContainerComponent
    {
        #region Members

        private bool isWhiteText;

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Card() );
            builder.Append( ClassProvider.CardWhiteText(), WhiteText );

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

        #endregion
    }
}
