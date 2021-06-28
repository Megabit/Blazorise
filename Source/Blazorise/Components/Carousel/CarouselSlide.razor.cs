﻿#region Using directives
using System.Threading.Tasks;
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

        private bool active;
        private bool left;
        private bool right;
        private bool prev;
        private bool next;

        #endregion

        #region Constructors

        /// <summary>
        /// A default carousel slide constructor.
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
                ParentCarousel.AddSlide( this );

                Active = Name == ParentCarousel.SelectedSlide;
            }

            base.OnInitialized();
        }

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.CarouselSlide() );
            builder.Append( ClassProvider.CarouselSlideActive( Active ) );
            builder.Append( ClassProvider.CarouselSlideSlidingLeft( Left ) );
            builder.Append( ClassProvider.CarouselSlideSlisingRight( Right ) );
            builder.Append( ClassProvider.CarouselSlideSlidingPrev( Prev ) );
            builder.Append( ClassProvider.CarouselSlideSlisingNext( Next ) );

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
            builder.Append( ClassProvider.CarouselIndicatorActive( IndicatorActive ) );
        }

        /// <summary>
        /// Makes this slide active.
        /// </summary>
        public Task Activate()
        {
            DirtyClasses();

            return ParentCarousel.Select( Name );
        }

        internal void Clean()
        {
            Active = false;
            Left = false;
            Right = false;
            Prev = false;
            Next = false;

            DirtyClasses();
        }

        #endregion

        #region Properties

        bool IndicatorActive => ParentCarousel.SelectedSlideIndex < ParentCarousel.NumberOfSlides
            ? ( ParentCarousel.carouselSlides[ParentCarousel.SelectedSlideIndex] == this )
            : false;

        internal bool Active
        {
            get => active;
            set
            {
                if ( active == value )
                    return;

                active = value;

                DirtyClasses();
            }
        }

        internal bool Left
        {
            get => left; set
            {
                if ( left == value )
                    return;

                left = value;

                DirtyClasses();
            }
        }

        internal bool Right
        {
            get => right; set
            {
                if ( right == value )
                    return;

                right = value;

                DirtyClasses();
            }
        }

        internal bool Prev
        {
            get => prev; set
            {
                if ( prev == value )
                    return;

                prev = value;

                DirtyClasses();
            }
        }

        internal bool Next
        {
            get => next; set
            {
                if ( next == value )
                    return;

                next = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Defines the interval(in milliseconds) after which this item will automatically slide.
        /// </summary>
        [Parameter] public int? Interval { get; set; }

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
