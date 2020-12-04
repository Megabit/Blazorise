#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Heading component is used for titles or subtitles that you want to display on a webpage.
    /// </summary>
    public partial class Heading : BaseTypographyComponent
    {
        #region Members

        private HeadingSize headingSize = HeadingSize.Is3;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.HeadingSize( headingSize ) );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        protected string TagName => $"h{SizeNumber}";

        protected string SizeNumber => Size switch
        {
            HeadingSize.Is1 => "1",
            HeadingSize.Is2 => "2",
            HeadingSize.Is3 => "3",
            HeadingSize.Is4 => "4",
            HeadingSize.Is5 => "5",
            HeadingSize.Is6 => "6",
            _ => "3",
        };

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

        #endregion
    }
}
