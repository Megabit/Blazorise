﻿#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class DisplayHeading : BaseTypographyComponent
    {
        #region Members

        private DisplayHeadingSize displayHeadingSize = DisplayHeadingSize.Is2;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.DisplayHeadingSize( Size ) );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the display heading size.
        /// </summary>
        [Parameter]
        public DisplayHeadingSize Size
        {
            get => displayHeadingSize;
            set
            {
                displayHeadingSize = value;

                DirtyClasses();
            }
        }

        #endregion
    }
}
