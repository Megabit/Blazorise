#region Using directives
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Modules;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.AntDesign;

public partial class Carousel : Blazorise.Carousel
{
    #region Members

    private ElementReference slickListElementRef;

    private string slickListElementId;

    private int slickWidth = -1;

    private int totalWidth = -1;

    private long slickListLastRenderTimeStamp;
    private TimeSpan slickListThrottleTimeSpan = TimeSpan.FromMilliseconds( 500 );

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

    [Obsolete( "Replace this with Stopwatch.GetElapsedTime() when DOTNET6 is abandoned." )]
    private static TimeSpan GetElapsedtime( long startingTimestamp )
    {
        var ticksPerSecond = (double)10000000;
        var s_tickFrequency = ticksPerSecond / Stopwatch.Frequency;
        var currentTimestamp = Stopwatch.GetTimestamp();
        return new TimeSpan( (long)( ( currentTimestamp - startingTimestamp ) * s_tickFrequency ) );
    }

    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( GetElapsedtime( slickListLastRenderTimeStamp ) > slickListThrottleTimeSpan )
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

    #endregion

    #region Properties

    protected string SlickTrackStyle
    {
        get
        {
            var slideIndex = carouselSlides.IndexOf( carouselSlides.FirstOrDefault( x => x.Name == SelectedSlide ) );

            return $"width: {totalWidth}px; opacity: 1; transform: translate3d(-{slickWidth * ( slideIndex + 1 )}px, 0px, 0px);transition: -webkit-transform 500ms ease 0s;";
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

    #endregion
}