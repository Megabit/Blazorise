#region Using directives
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// A container for placing content in a carousel slide.
    /// </summary>
    public partial class CarouselSlide : BaseComponent
    {
        #region Members

        /// <summary>
        /// Holds the reference to the parent carousel state object.
        /// </summary>
        private CarouselState parentCarouselState;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public CarouselSlide()
        {
            IndicatorClassBuilder = new ClassBuilder( BuildIndicatorClasses );
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            if ( ParentCarousel != null )
            {
                ParentCarousel.NotifyCarouselSlideInitialized( this );
            }

            base.OnInitialized();
        }

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.CarouselSlide() );
            builder.Append( ClassProvider.CarouselSlideActive( Active ) );

            base.BuildClasses( builder );
        }

        /// <inheritdoc/>
        protected internal override void DirtyClasses()
        {
            IndicatorClassBuilder.Dirty();

            base.DirtyClasses();
        }

        /// <summary>
        /// Builds a list of classnames for the indicator element.
        /// </summary>
        /// <param name="builder">Class builder used to append the classnames.</param>
        private void BuildIndicatorClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.CarouselIndicator() );
            builder.Append( ClassProvider.CarouselIndicatorActive( Active ) );
        }

        /// <summary>
        /// Makes this slide active.
        /// </summary>
        public void Activate()
        {
            DirtyClasses();

            ParentCarousel.Select( Name );
        }

        #endregion

        #region Properties

        /// <summary>
        /// True if this slide is currently active.
        /// </summary>
        public bool Active => ParentCarouselState?.CurrentSlide == Name;

        /// <summary>
        /// Gets or sets the class builder for the indicator element.
        /// </summary>
        protected ClassBuilder IndicatorClassBuilder { get; private set; }

        /// <summary>
        /// Gets the indicator element classnames.
        /// </summary>
        public string IndicatorClassNames => IndicatorClassBuilder.Class;

        /// <summary>
        /// Gets the indicator element styles.
        /// </summary>
        public string IndicatorStyleNames => null;

        /// <summary>
        /// Defines the slide name.
        /// </summary>
        [Parameter] public string Name { get; set; }

        /// <summary>
        /// Cascaded <see cref="Carousel"/> component state object.
        /// </summary>
        [CascadingParameter]
        protected CarouselState ParentCarouselState
        {
            get => parentCarouselState;
            set
            {
                if ( parentCarouselState == value )
                    return;

                parentCarouselState = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Cascaded <see cref="Carousel"/> component.
        /// </summary>
        [CascadingParameter] protected Carousel ParentCarousel { get; set; }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="CarouselSlide"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
