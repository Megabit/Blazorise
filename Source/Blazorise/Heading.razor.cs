#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Heading component is used for titles or subtitles that you want to display on a webpage.
    /// </summary>
    public partial class Heading : BaseComponent
    {
        #region Members

        private HeadingSize headingSize = HeadingSize.Is3;

        private TextColor textColor = TextColor.None;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.HeadingSize( headingSize ) );
            builder.Append( ClassProvider.HeadingTextColor( Color ), Color != TextColor.None );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the heading size.
        /// </summary>
        [Parameter]
        public HeadingSize Size
        {
            get => headingSize;
            set
            {
                headingSize = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Gets or sets the heading text color.
        /// </summary>
        [Parameter]
        public TextColor Color
        {
            get => textColor;
            set
            {
                textColor = value;

                DirtyClasses();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
