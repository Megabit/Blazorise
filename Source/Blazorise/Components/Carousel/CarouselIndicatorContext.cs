#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Provides context for rendering a custom carousel indicator.
/// </summary>
public class CarouselIndicatorContext
{
    #region Members

    private readonly EventCallback activate;

    #endregion

    #region Constructors

    /// <summary>
    /// Default constructor for <see cref="CarouselIndicatorContext"/>.
    /// </summary>
    /// <param name="carousel">Carousel instance.</param>
    /// <param name="slide">Carousel slide.</param>
    /// <param name="index">Slide index.</param>
    /// <param name="active">Indicates whether the slide is active.</param>
    /// <param name="activate">Activates the slide.</param>
    public CarouselIndicatorContext( Carousel carousel, CarouselSlide slide, int index, bool active, EventCallback activate )
    {
        Carousel = carousel;
        Slide = slide;
        Index = index;
        Active = active;
        this.activate = activate;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Activates the slide represented by the indicator.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task Activate()
        => activate.InvokeAsync();

    #endregion

    #region Properties

    /// <summary>
    /// Gets the carousel instance.
    /// </summary>
    public Carousel Carousel { get; }

    /// <summary>
    /// Gets the carousel slide.
    /// </summary>
    public CarouselSlide Slide { get; }

    /// <summary>
    /// Gets the zero-based slide index.
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// Gets the one-based slide number.
    /// </summary>
    public int Number => Index + 1;

    /// <summary>
    /// Gets whether the slide is active.
    /// </summary>
    public bool Active { get; }

    #endregion
}