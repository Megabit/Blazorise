#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Base class for all text-based components.
    /// </summary>
    public abstract class BaseTypographyComponent : BaseComponent
    {
        #region Members

        private TextColor color = TextColor.None;

        private TextAlignment alignment = TextAlignment.Left;

        private TextTransform textTransform = TextTransform.None;

        private TextWeight textWeight = TextWeight.None;

        private bool italic = false;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.TextColor( Color ), Color != TextColor.None );
            builder.Append( ClassProvider.TextAlignment( Alignment ), Alignment != TextAlignment.None );
            builder.Append( ClassProvider.TextTransform( Transform ), Transform != TextTransform.None );
            builder.Append( ClassProvider.TextWeight( Weight ), Weight != TextWeight.None );
            builder.Append( ClassProvider.TextItalic(), Italic );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the text color.
        /// </summary>
        [Parameter]
        public TextColor Color
        {
            get => color;
            set
            {
                color = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Gets or sets the text alignment.
        /// </summary>
        [Parameter]
        public TextAlignment Alignment
        {
            get => alignment;
            set
            {
                alignment = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Gets or sets the text transformation.
        /// </summary>
        [Parameter]
        public TextTransform Transform
        {
            get => textTransform;
            set
            {
                textTransform = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Gets or sets the text weight.
        /// </summary>
        [Parameter]
        public TextWeight Weight
        {
            get => textWeight;
            set
            {
                textWeight = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Italicize text if set to true.
        /// </summary>
        [Parameter]
        public bool Italic
        {
            get => italic;
            set
            {
                italic = value;

                DirtyClasses();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
