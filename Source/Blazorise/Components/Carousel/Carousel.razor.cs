﻿#region Using directives
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
        private CarouselState state = new()
        {
            Autoplay = true,
            AutoRepeat = true,
            Crossfade = false,
        };

        /// <summary>
        /// A times used to activate the slide animation.
        /// </summary>
        public Timer Timer { get; set; }

        /// <summary>
        /// A times used to animate the slide transition.
        /// </summary>
        public Timer TransitionTimer { get; set; }

        /// <summary>
        /// A list of slides placed inside of this carousel.
        /// </summary>
        protected internal readonly List<CarouselSlide> carouselSlides = new();

        #endregion

        #region Constructors

        /// <summary>
        /// A default carousel constructor.
        /// </summary>
        public Carousel()
        {
            IndicatorsClassBuilder = new( BuildIndicatorsClasses );
            SlidesClassBuilder = new( BuildSlidesClasses );
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void OnParametersSet()
        {
            if ( Interval != 0 )
                TimerEnabled = true;

            if ( Autoplay /*&& SelectedSlideIndex == 0*/ )
            {
                if ( TimerEnabled )
                    Timer.Start();
            }

            base.OnParametersSet();
        }

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            if ( Interval == 0 )
                TimerEnabled = false;

            if ( Timer == null && TimerEnabled )
            {
                InitializeTimer();

                Timer.Start();
            }

            if ( TransitionTimer == null )
            {
                InitializeTransitionTimer();
            }

            LocalizerService.LocalizationChanged += OnLocalizationChanged;

            base.OnInitialized();
        }

        /// <inheritdoc/>
        protected override async Task OnAfterRenderAsync( bool firstRender )
        {
            if ( firstRender )
            {
                await InvokeAsync( StateHasChanged );
            }

            await base.OnAfterRenderAsync( firstRender );
        }

        /// <inheritdoc/>
        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if ( Timer != null )
                {
                    Timer.Stop();
                    Timer.Dispose();
                }

                if ( TransitionTimer != null )
                {
                    TransitionTimer.Stop();
                    TransitionTimer.Dispose();
                }

                LocalizerService.LocalizationChanged -= OnLocalizationChanged;
            }

            base.Dispose( disposing );
        }

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Carousel() );
            builder.Append( ClassProvider.CarouselFade( Crossfade ) );

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
        /// Gets the next slide in sequence.
        /// </summary>
        /// <param name="slideName">Slide name from where the search will start.</param>
        /// <returns>Next slide in a sequence or first if not found.</returns>
        private CarouselSlide FindNextSlide( string slideName )
        {
            var slideIndex = carouselSlides.IndexOf( carouselSlides.First( x => x.Name == slideName ) ) + 1;

            if ( slideIndex >= carouselSlides.Count )
                slideIndex = 0;

            return carouselSlides[slideIndex];
        }

        /// <summary>
        /// Gets the previous slide in sequence.
        /// </summary>
        /// <param name="slideName">Slide name from where the search will start.</param>
        /// <returns>Previous slide in a sequence or last if not found.</returns>
        private CarouselSlide FindPreviousSlide( string slideName )
        {
            var slideIndex = carouselSlides.IndexOf( carouselSlides.First( x => x.Name == slideName ) ) - 1;

            if ( slideIndex < 0 )
                slideIndex = carouselSlides.Count - 1;

            return carouselSlides[slideIndex];
        }

        /// <summary>
        /// Adds the slide to the list of running slides.
        /// </summary>
        /// <param name="slide">Slide to add.</param>
        internal void AddSlide( CarouselSlide slide )
        {
            carouselSlides.Add( slide );

            if ( carouselSlides.Count == 1 && string.IsNullOrEmpty( SelectedSlide ) )
            {
                state = state with
                {
                    SelectedSlide = carouselSlides.Single().Name
                };
            }
        }

        /// <summary>
        /// Selects the next slide in a sequence, relative to the current slide.
        /// </summary>
        public Task SelectNext()
        {
            if ( AnimationRunning )
                return Task.CompletedTask;

            ResetTimer();
            SelectedSlide = FindNextSlide( SelectedSlide )?.Name;

            return RunAnimations();
        }

        /// <summary>
        /// Selects the previous slide in a sequence, relative to the current slide.
        /// </summary>
        public Task SelectPrevious()
        {
            if ( AnimationRunning )
                return Task.CompletedTask;

            ResetTimer();
            SelectedSlide = FindPreviousSlide( SelectedSlide )?.Name;

            return RunAnimations();
        }

        /// <summary>
        /// Selects the slide by its name.
        /// </summary>
        /// <param name="name">Name of the slide.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task Select( string name )
        {
            ResetTimer();

            SelectedSlide = name;

            return RunAnimations();
        }

        /// <summary>
        /// Handles the localization changed event.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="eventArgs">Data about the localization event.</param>
        private async void OnLocalizationChanged( object sender, EventArgs eventArgs )
        {
            await InvokeAsync( StateHasChanged );
        }

        private void InitializeTimer()
        {
            Timer = new( Interval );
            Timer.Elapsed += OnTimerEvent;
            Timer.AutoReset = true;
        }

        private void InitializeTransitionTimer()
        {
            TransitionTimer = new( 2000 );
            TransitionTimer.Elapsed += OnTransitionTimerEvent;
            TransitionTimer.AutoReset = false;
        }

        private void ResetTimer()
        {
            if ( Timer != null )
            {
                Timer.Stop();

                if ( TimerEnabled )
                {
                    Timer.Interval = carouselSlides[SelectedSlideIndex].Interval ?? Interval;
                    Timer.Start();
                }
            }
        }

        private void ResetTransitionTimer()
        {
            if ( TransitionTimer != null )
            {
                TransitionTimer.Stop();
                InitializeTransitionTimer(); // Avoid an System.ObjectDisposedException due to the timer being disposed. This occurs when the Enabled property of the timer is set to false by the call to Stop() above.
                TransitionTimer.Start();
            }
        }

        private async void OnTimerEvent( object source, ElapsedEventArgs e )
        {
            if ( AnimationRunning )
                return;

            if ( SelectedSlideIndex == NumberOfSlides - 1 && !AutoRepeat )
            {
                Timer.Stop();
                return;
            }

            SelectedSlide = FindNextSlide( SelectedSlide )?.Name;

            await InvokeAsync( RunAnimations );
        }

        private async void OnTransitionTimerEvent( object source, ElapsedEventArgs e )
        {
            if ( !AnimationRunning )
                return;

            // If the active index is not "active" by the time the timer is elapsed something is very wrong, reset the animation
            if ( !carouselSlides[SelectedSlideIndex].Active )
            {
                await InvokeAsync( async () =>
                {
                    await AnimationEnd( carouselSlides[SelectedSlideIndex] );
                } );
            }
        }

        /// <summary>
        /// Finish the slide animation.
        /// </summary>
        /// <param name="slide">Target slide.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected virtual async Task AnimationEnd( CarouselSlide slide )
        {
            if ( slide.Name == carouselSlides[SelectedSlideIndex].Name )
            {
                AnimationRunning = false;

                carouselSlides[PreviouslySelectedSlideIndex].Clean();
                carouselSlides[SelectedSlideIndex].Clean();
                carouselSlides[SelectedSlideIndex].Active = true;

                await InvokeAsync( StateHasChanged );

                if ( TimerEnabled )
                {
                    InitializeTimer();
                    Timer.Start();
                }

                await SelectedSlideChanged.InvokeAsync( SelectedSlide );
            }
        }

        /// <summary>
        /// Runs the animation for the active slide.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected virtual async Task RunAnimations()
        {
            if ( NumberOfSlides == 0 )
                return;

            if ( TimerEnabled )
            {
                Timer.Stop();
                Timer.Interval = carouselSlides[SelectedSlideIndex].Interval ?? Interval;
            }

            AnimationRunning = true;
            carouselSlides[SelectedSlideIndex].Clean();

            Direction = DetermineDirection();

            //Add new item to DOM on appropriate side
            carouselSlides[SelectedSlideIndex].Next = Direction == CarouselDirection.Previous;
            carouselSlides[SelectedSlideIndex].Prev = Direction == CarouselDirection.Next;

            await InvokeAsync( StateHasChanged );

            await Task.Delay( 300 ); //Ensure new item is rendered on DOM before continuing

            //Trigger Animation
            carouselSlides[SelectedSlideIndex].Left = Direction == CarouselDirection.Previous;
            carouselSlides[PreviouslySelectedSlideIndex].Left = Direction == CarouselDirection.Previous;

            carouselSlides[SelectedSlideIndex].Right = Direction == CarouselDirection.Next;
            carouselSlides[PreviouslySelectedSlideIndex].Right = Direction == CarouselDirection.Next;

            await InvokeAsync( StateHasChanged );

            ResetTransitionTimer();
        }

        private CarouselDirection DetermineDirection()
        {
            if ( PreviouslySelectedSlideIndex == 0 )
            {
                if ( SelectedSlideIndex == NumberOfSlides - 1 )
                {
                    return CarouselDirection.Next;
                }
                else
                {
                    return CarouselDirection.Previous;
                }
            }

            if ( PreviouslySelectedSlideIndex == NumberOfSlides - 1 )
            {
                if ( SelectedSlideIndex == 0 )
                {
                    return CarouselDirection.Previous;
                }
                else
                {
                    return CarouselDirection.Next;
                }
            }

            if ( SelectedSlideIndex > PreviouslySelectedSlideIndex )
            {
                return CarouselDirection.Previous;
            }
            else
            {
                return CarouselDirection.Next;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the direction of slide animation.
        /// </summary>
        internal protected CarouselDirection Direction { get; set; }

        /// <summary>
        /// Gets the total number of slides in a carousel.
        /// </summary>
        internal protected int NumberOfSlides => carouselSlides.Count;

        /// <summary>
        /// Gets or sets the flag that indicates if the timer is running.
        /// </summary>
        private bool TimerEnabled { get; set; } = true;

        /// <summary>
        /// Gets the carousel state.
        /// </summary>
        protected CarouselState State => state;

        /// <summary>
        /// Gets or sets the carousel indicator class-builder.
        /// </summary>
        protected ClassBuilder IndicatorsClassBuilder { get; private set; }

        /// <summary>
        /// Gets or sets the carousel slides class-builder.
        /// </summary>
        protected ClassBuilder SlidesClassBuilder { get; private set; }

        /// <summary>
        /// Gets the class-names for a carousel indicator.
        /// </summary>
        protected string IndicatorsClassNames => IndicatorsClassBuilder.Class;

        /// <summary>
        /// Gets the class-names for a carousel slides.
        /// </summary>
        protected string SlidesClassNames => SlidesClassBuilder.Class;

        /// <summary>
        /// Gets the index of the current active slide.
        /// </summary>
        public int SelectedSlideIndex
            => carouselSlides.IndexOf( carouselSlides.FirstOrDefault( x => x.Name == state.SelectedSlide ) );

        /// <summary>
        /// Gets the index of the previously active slide.
        /// </summary>
        public int PreviouslySelectedSlideIndex
            => carouselSlides.IndexOf( carouselSlides.FirstOrDefault( x => x.Name == state.PreviouslySelectedSlide ) );

        /// <summary>
        /// Indicates if slide animation is currently running.
        /// </summary>
        public bool AnimationRunning { get; private set; } = false;

        /// <summary>
        /// Gets or sets the DI registered <see cref="ITextLocalizerService"/>.
        /// </summary>
        [Inject] protected ITextLocalizerService LocalizerService { get; set; }

        /// <summary>
        /// Gets or sets the DI registered <see cref="ITextLocalizer{Carousel}"/>.
        /// </summary>
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
        /// Autoplays the carousel slides.
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

        /// <summary>
        /// Auto-repeats the carousel slides once they reach the end.
        /// </summary>
        [Parameter]
        public bool AutoRepeat
        {
            get => state.AutoRepeat;
            set
            {
                state = state with { AutoRepeat = value };

                DirtyClasses();
            }
        }

        /// <summary>
        /// Animate slides with a fade transition instead of a slide.
        /// </summary>
        [Parameter]
        public bool Crossfade
        {
            get => state.Crossfade;
            set
            {
                state = state with { Crossfade = value };

                DirtyClasses();
            }
        }

        /// <summary>
        /// Defines the interval(in milliseconds) after which the item will automatically slide.
        /// </summary>
        [Parameter] public double Interval { get; set; } = 2000;

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
            get => state.SelectedSlide;
            set
            {
                if ( value == state.SelectedSlide )
                    return;

                state = state with
                {
                    PreviouslySelectedSlide = SelectedSlide,
                    SelectedSlide = value
                };

                InvokeAsync( () => SelectedSlideChanged.InvokeAsync( state.SelectedSlide ) );

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
