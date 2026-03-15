#region Using directives
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Modules;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.AntDesign.Components;

public partial class Carousel : Blazorise.Carousel
{
    #region Members

    private const int SlickTransitionDuration = 300;

    private ElementReference slickListElementRef;

    private string slickListElementId;

    private int slickWidth = -1;

    private int totalWidth = -1;

    private bool suppressSlickTrackTransition;

    private long slickListLastRenderTimeStamp;
    private readonly TimeSpan slickListThrottleTimeSpan = TimeSpan.FromMilliseconds( 500 );

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        slickListElementId = IdGenerator.Generate;
        base.OnInitialized();
    }

    protected override void BuildClasses( ClassBuilder builder )
    {
        base.BuildClasses( builder );
    }

    protected override void BuildIndicatorsStyles( StyleBuilder builder )
    {
        builder.Append( "display: block" );
    }

    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( Stopwatch.GetElapsedTime( slickListLastRenderTimeStamp ) > slickListThrottleTimeSpan )
        {
            slickListLastRenderTimeStamp = Stopwatch.GetTimestamp();
            var listRect = await JSUtilitiesModule.GetElementInfo( slickListElementRef, slickListElementId );

            if ( slickWidth != (int)listRect.BoundingClientRect.Width )
            {
                slickWidth = (int)listRect.BoundingClientRect.Width;
                totalWidth = slickWidth * ( carouselSlides.Count * 2 + 1 );

                await InvokeAsync( StateHasChanged );
            }
        }
        await base.OnAfterRenderAsync( firstRender );
    }

    protected override async Task AnimationEnd( Blazorise.CarouselSlide slide )
    {
        if ( ShouldUseSlickWraparoundTransition )
        {
            await Task.Delay( SlickTransitionDuration );

            suppressSlickTrackTransition = true;

            await InvokeAsync( StateHasChanged );
            await Task.Yield();
        }

        await base.AnimationEnd( slide );

        if ( suppressSlickTrackTransition )
        {
            suppressSlickTrackTransition = false;
        }
    }

    #endregion

    #region Properties

    protected string SlickTrackStyle
    {
        get
        {
            var slideIndex = carouselSlides.IndexOf( carouselSlides.FirstOrDefault( x => x.Name == SelectedSlide ) );
            var targetSlidePosition = slideIndex + 1;

            if ( !suppressSlickTrackTransition && ShouldUseSlickWrapToFirstPosition )
            {
                targetSlidePosition = carouselSlides.Count + 1;
            }
            else if ( !suppressSlickTrackTransition && ShouldUseSlickWrapToLastPosition )
            {
                targetSlidePosition = 0;
            }

            return $"width: {totalWidth}px; opacity: 1; transform: translate3d(-{slickWidth * targetSlidePosition}px, 0px, 0px);transition: {( suppressSlickTrackTransition ? "none" : $"-webkit-transform {SlickTransitionDuration}ms ease 0s" )};";
        }
    }

    protected string SlickClonedStyle
        => $"width: {slickWidth}px;";

    protected string SlickStyle
        => $"outline: none; width: {slickWidth}px;";

    /// <summary>
    /// Gets or sets the <see cref="IJSUtilitiesModule"/> instance.
    /// </summary>
    [Inject] public IJSUtilitiesModule JSUtilitiesModule { get; set; }

    private bool ShouldUseSlickWraparoundTransition
        => carouselSlides.Count > 1
           && !Crossfade
           && ( ShouldUseSlickWrapToFirstPosition || ShouldUseSlickWrapToLastPosition );

    private bool ShouldUseSlickWrapToFirstPosition
        => AnimationRunning
           && PreviouslySelectedSlideIndex == carouselSlides.Count - 1
           && SelectedSlideIndex == 0;

    private bool ShouldUseSlickWrapToLastPosition
        => AnimationRunning
           && PreviouslySelectedSlideIndex == 0
           && SelectedSlideIndex == carouselSlides.Count - 1;

    #endregion
}