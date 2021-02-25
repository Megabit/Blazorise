#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Blazorise.Localization;
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// A slideshow component for cycling through elements - images or slides of text.
    /// </summary>
    public partial class Carousel : BaseContainerComponent
    {
        #region Members

        /// <summary>
        /// Holds the state of this carousel.
        /// </summary>
        private CarouselState state = new CarouselState
        {
            Autoplay = true,
            Crossfade = false,
        };

        /// <summary>
        /// A times used to activate the slide animation.
        /// </summary>
        private Timer autoplayTimer;

        /// <summary>
        /// A list of slides placed inside of this carousel.
        /// </summary>
        protected List<CarouselSlide> carouselSlides = new List<CarouselSlide>();

        #endregion

        #region Constructors

        public Carousel()
        {
            IndicatorsClassBuilder = new ClassBuilder( BuildIndicatorsClasses );
            SlidesClassBuilder = new ClassBuilder( BuildSlidesClasses );
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            LocalizerService.LocalizationChanged += OnLocalizationChanged;

            base.OnInitialized();
        }

        /// <inheritdoc/>
        protected override void OnAfterRender( bool firstRender )
        {
            if ( firstRender )
            {
                if ( autoplayTimer == null )
                {
                    autoplayTimer = new Timer
                    {
                        Interval = AutoplayInterval
                    };

                    autoplayTimer.Elapsed += OnAutoplayTimerElapsed;
                    autoplayTimer.AutoReset = true;

                    if ( Autoplay )
                    {
                        autoplayTimer.Start();
                    }
                }

                InvokeAsync( StateHasChanged );
            }

            base.OnAfterRender( firstRender );
        }

        /// <inheritdoc/>
        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if ( autoplayTimer != null )
                {
                    autoplayTimer.Elapsed -= OnAutoplayTimerElapsed;
                    autoplayTimer.Dispose();
                    autoplayTimer = null;
                }

                LocalizerService.LocalizationChanged -= OnLocalizationChanged;
            }

            base.Dispose( disposing );
        }

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Carousel() );

            base.BuildClasses( builder );
        }

        /// <summary>
        /// Builds a list of classnames for the indicators container element.
        /// </summary>
        /// <param name="builder">Class builder used to append the classnames.</param>
        private void BuildIndicatorsClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.CarouselIndicators() );
        }

        /// <summary>
        /// Builds a list of classnames for the slides element.
        /// </summary>
        /// <param name="builder">Class builder used to append the classnames.</param>
        private void BuildSlidesClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.CarouselSlides() );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="slide"></param>
        internal void NotifyCarouselSlideInitialized( CarouselSlide slide )
        {
            carouselSlides.Add( slide );
        }

        /// <summary>
        /// Sets the active item by the name.
        /// </summary>
        /// <param name="slideName"></param>
        public virtual void Select( string slideName )
        {
            if ( Autoplay )
            {
                autoplayTimer.Stop();
                autoplayTimer.Start();
            }

            SelectedSlide = slideName;

            InvokeAsync( StateHasChanged );
        }

        private CarouselSlide FindNextSlide( string slideName )
        {
            var slideIndex = carouselSlides.IndexOf( carouselSlides.First( x => x.Name == slideName ) ) + 1;

            if ( slideIndex >= carouselSlides.Count )
                slideIndex = 0;

            return carouselSlides[slideIndex];
        }

        private CarouselSlide FindPreviousSlide( string slideName )
        {
            var slideIndex = carouselSlides.IndexOf( carouselSlides.First( x => x.Name == SelectedSlide ) ) - 1;

            if ( slideIndex < 0 )
                slideIndex = carouselSlides.Count - 1;

            return carouselSlides[slideIndex];
        }

        /// <summary>
        /// Selects the next slide in a sequence, relative to the current slide.
        /// </summary>
        public void SelectNext()
        {
            if ( carouselSlides.Count == 0 )
                return;

            Select( FindNextSlide( SelectedSlide ).Name );
        }

        /// <summary>
        /// Selects the previous slide in a sequence, relative to the current slide.
        /// </summary>
        public void SelectPrevious()
        {
            if ( carouselSlides.Count == 0 )
                return;

            Select( FindPreviousSlide( SelectedSlide ).Name );
        }

        /// <summary>
        /// Handles the indicator clicked event.
        /// </summary>
        /// <param name="slideName">Slide name for which the indicator was clicked.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected Task OnIndicatorClicked( string slideName )
        {
            Select( slideName );

            return Task.CompletedTask;
        }

        private async void OnAutoplayTimerElapsed( object sender, ElapsedEventArgs e )
        {
            await InvokeAsync( () => SelectNext() );
        }

        private async void OnLocalizationChanged( object sender, EventArgs e )
        {
            await InvokeAsync( StateHasChanged );
        }

        #endregion

        #region Properties

        protected CarouselState State => state;

        protected ClassBuilder IndicatorsClassBuilder { get; private set; }

        protected ClassBuilder SlidesClassBuilder { get; private set; }

        protected string IndicatorsClassNames => IndicatorsClassBuilder.Class;

        protected string SlidesClassNames => SlidesClassBuilder.Class;

        [Inject] protected ITextLocalizerService LocalizerService { get; set; }

        [Inject] protected ITextLocalizer<Carousel> Localizer { get; set; }

        /// <summary>
        /// Gets the localized previous button text.
        /// </summary>
        protected string PreviousButtonString
        {
            get
            {
                var localizationString = "Previous";

                if ( PreviousButtonLocalizer != null )
                    return PreviousButtonLocalizer.Invoke( localizationString );

                return Localizer[localizationString];
            }
        }

        /// <summary>
        /// Gets the localized next button text.
        /// </summary>
        protected string NextButtonString
        {
            get
            {
                var localizationString = "Next";

                if ( PreviousButtonLocalizer != null )
                    return PreviousButtonLocalizer.Invoke( localizationString );

                return Localizer[localizationString];
            }
        }

        /// <summary>
        /// Autoplays the carousel slides from left to right.
        /// </summary>
        [Parameter]
        public bool Autoplay
        {
            get => state.Autoplay;
            set
            {
                state = state with { Autoplay = value };

                DirtyClasses();
            }
        }

        ///// <summary>
        ///// Animate slides with a fade transition instead of a slide.
        ///// </summary>
        //[Parameter]
        //public bool Crossfade
        //{
        //    get => state.Crossfade;
        //    set
        //    {
        //        state = state with { Crossfade = value };

        //        DirtyClasses();
        //    }
        //}

        /// <summary>
        /// Defines the interval(in milliseconds) after which the item will be automatically slide.
        /// </summary>
        [Parameter] public double AutoplayInterval { get; set; } = 5000;

        /// <summary>
        /// Specifies whether to show an indicator for each slide.
        /// </summary>
        [Parameter] public bool ShowIndicators { get; set; } = true;

        /// <summary>
        /// Specifies whether to show the controls that allows the user to navigate to the next or previous slide.
        /// </summary>
        [Parameter] public bool ShowControls { get; set; } = true;

        /// <summary>
        /// Gets or sets currently selected slide name.
        /// </summary>
        [Parameter]
        public string SelectedSlide
        {
            get => state.CurrentSlide;
            set
            {
                if ( value == state.CurrentSlide )
                    return;

                state = state with { CurrentSlide = value };

                SelectedSlideChanged.InvokeAsync( state.CurrentSlide );

                DirtyClasses();
            }
        }

        /// <summary>
        /// Occurs after the selected slide has changed.
        /// </summary>
        [Parameter] public EventCallback<string> SelectedSlideChanged { get; set; }

        /// <summary>
        /// Function used to handle custom localization for previous button that will override a default <see cref="ITextLocalizer"/>.
        /// </summary>
        [Parameter] public TextLocalizerHandler PreviousButtonLocalizer { get; set; }

        /// <summary>
        /// Function used to handle custom localization for next button that will override a default <see cref="ITextLocalizer"/>.
        /// </summary>
        [Parameter] public TextLocalizerHandler NextButtonLocalizer { get; set; }

        #endregion
    }
}
