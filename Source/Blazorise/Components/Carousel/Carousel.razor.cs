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

namespace Blazorise;

/// <summary>
/// A slideshow component for cycling through elements - images or slides of text.
/// </summary>
public partial class Carousel : BaseComponent<CarouselClasses, CarouselStyles>, IDisposable
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
    public System.Timers.Timer Timer { get; set; }

    /// <summary>
    /// A times used to animate the slide transition.
    /// </summary>
    public System.Timers.Timer TransitionTimer { get; set; }

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
        IndicatorsClassBuilder = new( BuildIndicatorsClasses, builder => builder.Append( Classes?.Indicators ) );
        SlidesClassBuilder = new( BuildSlidesClasses, builder => builder.Append( Classes?.Slides ) );
        IndicatorsStyleBuilder = new( BuildIndicatorsStyles, builder => builder.Append( Styles?.Indicators ) );
        SlidesStyleBuilder = new( BuildSlidesStyles, builder => builder.Append( Styles?.Slides ) );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        SetTimer();

        base.OnParametersSet();
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        SetTimer();

        if ( TransitionTimer is null )
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
            if ( Timer is not null )
            {
                Timer.Stop();
                Timer.Elapsed -= OnTimerEvent;
                Timer.Dispose();
            }

            if ( TransitionTimer is not null )
            {
                TransitionTimer.Stop();
                TransitionTimer.Elapsed -= OnTransitionTimerEvent;
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
    /// Builds a list of styles for the indicators container element.
    /// </summary>
    /// <param name="builder">Style builder used to append the styles.</param>
    protected virtual void BuildIndicatorsStyles( StyleBuilder builder )
    {
    }

    /// <summary>
    /// Builds a list of styles for the slides element.
    /// </summary>
    /// <param name="builder">Style builder used to append the styles.</param>
    protected virtual void BuildSlidesStyles( StyleBuilder builder )
    {
    }

    /// <inheritdoc/>
    protected internal override void DirtyClasses()
    {
        IndicatorsClassBuilder.Dirty();
        SlidesClassBuilder.Dirty();

        base.DirtyClasses();
    }

    /// <inheritdoc/>
    protected override void DirtyStyles()
    {
        IndicatorsStyleBuilder.Dirty();
        SlidesStyleBuilder.Dirty();

        base.DirtyStyles();
    }

    /// <summary>
    /// Gets the next slide in sequence.
    /// </summary>
    /// <param name="slideName">Slide name from where the search will start.</param>
    /// <returns>Next slide in a sequence or first if not found.</returns>
    private CarouselSlide FindNextSlide( string slideName )
    {
        var slideIndex = FindCarouselSlideIdxByName( slideName ) + 1;

        if ( slideIndex >= carouselSlides.Count )
            slideIndex = 0;

        return GetCarouselSlide( slideIndex );
    }

    /// <summary>
    /// Gets the previous slide in sequence.
    /// </summary>
    /// <param name="slideName">Slide name from where the search will start.</param>
    /// <returns>Previous slide in a sequence or last if not found.</returns>
    private CarouselSlide FindPreviousSlide( string slideName )
    {
        var slideIndex = FindCarouselSlideIdxByName( slideName ) - 1;

        if ( slideIndex < 0 )
            slideIndex = carouselSlides.Count - 1;

        return GetCarouselSlide( slideIndex );
    }

    /// <summary>
    /// Gets the slide by name. Returns -1 if the slide does not exist. 
    /// </summary>
    /// <param name="slideName"></param>
    private int FindCarouselSlideIdxByName( string slideName ) => carouselSlides.IndexOf( carouselSlides.FirstOrDefault( x => string.Compare( x.Name, slideName ) == 0 ) );

    /// <summary>
    /// Gets the slide by index. Returns null if no slide has been found.
    /// </summary>
    /// <param name="slideIndex"></param>
    private CarouselSlide GetCarouselSlide( int slideIndex ) => carouselSlides.ElementAtOrDefault( slideIndex );

    /// <summary>
    /// Gets the currently selected slide by index.
    /// </summary>
    private CarouselSlide GetSelectedCarouselSlide() => GetCarouselSlide( SelectedSlideIndex );

    /// <summary>
    /// Gets the slide by index.
    /// </summary>
    private CarouselSlide GetPreviouslySelectedCarouselSlide() => GetCarouselSlide( PreviouslySelectedSlideIndex );

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
    /// Removes the slide from the list of running slides.
    /// </summary>
    /// <param name="slide">Slide to remove.</param>
    internal void RemoveSlide( CarouselSlide slide )
    {
        carouselSlides.Remove( slide );

        Reset();
    }

    private void Reset()
    {
        AnimationRunning = false;
        ResetTimer();
        ResetTransitionTimer();
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

    private void SetTimer()
    {
        TimerEnabled = ( Interval > 0 );

        if ( Timer is null && TimerEnabled )
        {
            InitializeTimer();
        }

        if ( AutoPlayEnabled )
        {
            Timer.Start();
        }
    }

    private void ResetTimer()
    {
        if ( Timer is not null )
        {
            Timer.Stop();

            if ( AutoPlayEnabled )
            {
                Timer.Interval = GetSelectedCarouselSlide()?.Interval ?? Interval;
                // Avoid an System.ObjectDisposedException due to the timer being disposed. This occurs when the Enabled property of the timer is set to false by the call to Stop() above.
                InitializeTimer();
                Timer.Start();
            }
        }
    }

    private void ResetTransitionTimer()
    {
        if ( TransitionTimer is not null )
        {
            TransitionTimer.Stop();
            // Avoid an System.ObjectDisposedException due to the timer being disposed. This occurs when the Enabled property of the timer is set to false by the call to Stop() above.
            InitializeTransitionTimer();
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
        var selectedSlide = GetSelectedCarouselSlide();
        if ( !selectedSlide?.Active ?? false )
        {
            await InvokeAsync( async () =>
            {
                await AnimationEnd( selectedSlide );
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
        var selectedSlide = GetSelectedCarouselSlide();

        if ( selectedSlide is null )
            return;

        if ( slide.Name == selectedSlide.Name )
        {
            AnimationRunning = false;

            GetPreviouslySelectedCarouselSlide()?.Clean();

            if ( selectedSlide is not null )
            {
                selectedSlide.Clean();
                selectedSlide.Active = true;
            }

            await InvokeAsync( StateHasChanged );

            if ( TimerEnabled )
            {
                ResetTimer();
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

        var selectedSlide = GetSelectedCarouselSlide();
        var previouslySelectedSlide = GetPreviouslySelectedCarouselSlide();

        if ( TimerEnabled )
        {
            Timer.Stop();
            Timer.Interval = selectedSlide?.Interval ?? Interval;
        }

        AnimationRunning = true;
        Direction = DetermineDirection();


        //Add new item to DOM on appropriate side
        if ( selectedSlide is not null )
        {
            selectedSlide.Clean();
            selectedSlide.Next = Direction == CarouselDirection.Previous;
            selectedSlide.Prev = Direction == CarouselDirection.Next;
        }

        await InvokeAsync( StateHasChanged );

        await Task.Delay( 300 ); //Ensure new item is rendered on DOM before continuing

        //Trigger Animation
        SetSlideDirection( selectedSlide );
        SetSlideDirection( previouslySelectedSlide );

        await InvokeAsync( StateHasChanged );
        await Task.Delay( selectedSlide.AnimationTime );

        await AnimationEnd( selectedSlide );
        if ( AnimationRunning ) //Animation is still running for some reason, let's go ahead and setup a timer to reset it
        {
            ResetTransitionTimer();
        }
    }

    private void SetSlideDirection( CarouselSlide slide )
    {
        if ( slide is not null )
        {
            slide.Left = Direction == CarouselDirection.Previous;
            slide.Right = Direction == CarouselDirection.Next;
        }
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

    /// <summary>
    /// Gets the index of the slide with the specified name.
    /// </summary>
    /// <param name="slideName">Slide name.</param>
    /// <returns>An index of slide.</returns>
    public int SlideIndex( string slideName )
        => carouselSlides.IndexOf( carouselSlides.FirstOrDefault( x => x.Name == slideName ) );

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
    /// Gets or sets the flag that indicates if the timer is running and AutoPlay is enabled.
    /// </summary>
    private bool AutoPlayEnabled
        => ( Autoplay && TimerEnabled );

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
    /// Gets or sets the carousel indicator style-builder.
    /// </summary>
    protected StyleBuilder IndicatorsStyleBuilder { get; private set; }

    /// <summary>
    /// Gets or sets the carousel slides style-builder.
    /// </summary>
    protected StyleBuilder SlidesStyleBuilder { get; private set; }

    /// <summary>
    /// Gets the class-names for a carousel indicator.
    /// </summary>
    protected string IndicatorsClassNames => IndicatorsClassBuilder.Class;

    /// <summary>
    /// Gets the class-names for a carousel slides.
    /// </summary>
    protected string SlidesClassNames => SlidesClassBuilder.Class;

    /// <summary>
    /// Gets the styles for a carousel indicator container.
    /// </summary>
    protected string IndicatorsStyleNames => IndicatorsStyleBuilder.Styles;

    /// <summary>
    /// Gets the styles for carousel slides container.
    /// </summary>
    protected string SlidesStyleNames => SlidesStyleBuilder.Styles;

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

            if ( PreviousButtonLocalizer is not null )
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

            if ( PreviousButtonLocalizer is not null )
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
    /// Defines the interval (in milliseconds) after which the item will automatically slide.
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
                PreviouslySelectedSlide = state.SelectedSlide,
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

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="Carousel"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}