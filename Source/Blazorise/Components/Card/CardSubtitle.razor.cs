#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class CardSubtitle : BaseTypographyComponent
    {
        #region Members

        private int size = 6;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.CardSubtitle( InsideHeader ) );
            builder.Append( ClassProvider.CardSubtitleSize( InsideHeader, Size ) );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Indicates if the subtitle is placed inside if card header.
        /// </summary>
        protected bool InsideHeader => ParentCardHeader != null;

        /// <summary>
        /// Number from 1 to 6 that defines the subtitle size where the smaller number means larger text.
        /// </summary>
        /// <remarks>
        /// todo: change to enum
        /// </remarks>
        [Parameter]
        public int Size
        {
            get => size;
            set
            {
                size = value;

                DirtyClasses();
            }
        }

        [CascadingParameter] public CardHeader ParentCardHeader { get; set; }

        #endregion
    }
}
