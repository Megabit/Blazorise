#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Blazorise.Stores;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class Carousel : BaseContainerComponent
    {
        #region Members

        private CarouselStore store = new CarouselStore
        {
            Autoplay = true,
            Crossfade = false,
        };

        private Timer autoplayTimer;

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

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Carousel() );
            //builder.Append( ClassProvider.CarouselFade( Crossfade ) );

            base.BuildClasses( builder );
        }

        private void BuildIndicatorsClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.CarouselIndicators() );
        }

        private void BuildSlidesClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.CarouselSlides() );
        }

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

                    autoplayTimer.Elapsed += OnAutoplayTimer_Elapsed;
                    autoplayTimer.AutoReset = true;

                    if ( Autoplay )
                    {
                        autoplayTimer.Start();
                    }
                }

                StateHasChanged();
            }

            base.OnAfterRender( firstRender );
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if ( autoplayTimer != null )
                {
                    autoplayTimer.Elapsed -= OnAutoplayTimer_Elapsed;
                    autoplayTimer.Dispose();
                    autoplayTimer = null;
                }
            }

            base.Dispose( disposing );
        }

        internal void AddSlide( CarouselSlide slide )
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

            StateHasChanged();
        }

        private CarouselSlide FindNext( string slideName )
        {
            var slideIndex = carouselSlides.IndexOf( carouselSlides.First( x => x.Name == slideName ) ) + 1;

            if ( slideIndex >= carouselSlides.Count )
                slideIndex = 0;

            return carouselSlides[slideIndex];
        }

        private CarouselSlide FindPrevious( string slideName )
        {
            var slideIndex = carouselSlides.IndexOf( carouselSlides.First( x => x.Name == SelectedSlide ) ) - 1;

            if ( slideIndex < 0 )
                slideIndex = carouselSlides.Count - 1;

            return carouselSlides[slideIndex];
        }

        public void SelectNext()
        {
            if ( carouselSlides.Count == 0 )
                return;

            Select( FindNext( SelectedSlide ).Name );
        }

        public void SelectPrevious()
        {
            if ( carouselSlides.Count == 0 )
                return;

            Select( FindPrevious( SelectedSlide ).Name );
        }

        private async void OnAutoplayTimer_Elapsed( object sender, ElapsedEventArgs e )
        {
            await InvokeAsync( () => SelectNext() );
        }

        protected Task OnIndicatorClicked( string slideName )
        {
            Select( slideName );

            return Task.CompletedTask;
        }

        #endregion

        #region Properties

        protected CarouselStore Store => store;

        protected ClassBuilder IndicatorsClassBuilder { get; private set; }

        protected ClassBuilder SlidesClassBuilder { get; private set; }

        protected string IndicatorsClassNames => IndicatorsClassBuilder.Class;

        protected string SlidesClassNames => SlidesClassBuilder.Class;

        /// <summary>
        /// Autoplays the carousel slides from left to right.
        /// </summary>
        [Parameter]
        public bool Autoplay
        {
            get => store.Autoplay;
            set
            {
                store.Autoplay = value;

                DirtyClasses();
            }
        }

        ///// <summary>
        ///// Animate slides with a fade transition instead of a slide.
        ///// </summary>
        //[Parameter]
        //public bool Crossfade
        //{
        //    get => store.Crossfade;
        //    set
        //    {
        //        store.Crossfade = value;

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
            get => store.CurrentSlide;
            set
            {
                if ( value == store.CurrentSlide )
                    return;

                store.CurrentSlide = value;

                SelectedSlideChanged.InvokeAsync( store.CurrentSlide );

                DirtyClasses();
            }
        }

        /// <summary>
        /// Occurs after the selected slide has changed.
        /// </summary>
        [Parameter] public EventCallback<string> SelectedSlideChanged { get; set; }

        #endregion
    }
}
