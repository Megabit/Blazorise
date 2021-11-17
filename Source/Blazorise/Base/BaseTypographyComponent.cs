#region Using directives
using System;
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

        private bool italic = false;

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.TextItalic(), Italic );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the text color.
        /// </summary>
        [Obsolete( "The Color parameter will be removed. Please use TextColor instead!", true )]
        [Parameter]
        public TextColor Color
        {
            get => TextColor;
            set => TextColor = value;
        }

        /// <summary>
        /// Gets or sets the text alignment.
        /// </summary>
        [Obsolete( "The Alignment parameter will be removed. Please use TextAlignment instead!", true )]
        [Parameter]
        public TextAlignment Alignment
        {
            get => TextAlignment;
            set => TextAlignment = value;
        }

        /// <summary>
        /// Gets or sets the text transformation.
        /// </summary>
        [Obsolete( "The Transform parameter will be removed. Please use TextTransform instead!", true )]
        [Parameter]
        public TextTransform Transform
        {
            get => TextTransform;
            set => TextTransform = value;
        }

        /// <summary>
        /// Gets or sets the text weight.
        /// </summary>
        [Obsolete( "The Weight parameter will be removed. Please use TextWeight instead!", true )]
        [Parameter]
        public TextWeight Weight
        {
            get => TextWeight;
            set => TextWeight = value;
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

        /// <summary>
        /// Specifies the content to be rendered inside this component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
