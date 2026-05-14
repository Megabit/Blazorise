#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Provides context for rendering a custom carousel navigation button.
/// </summary>
public class CarouselNavigationContext
{
    #region Members

    private readonly EventCallback navigate;

    #endregion

    #region Constructors

    /// <summary>
    /// Default constructor for <see cref="CarouselNavigationContext"/>.
    /// </summary>
    /// <param name="carousel">Carousel instance.</param>
    /// <param name="direction">Navigation direction.</param>
    /// <param name="text">Localized button text.</param>
    /// <param name="navigate">Navigation callback.</param>
    public CarouselNavigationContext( Carousel carousel, CarouselDirection direction, string text, EventCallback navigate )
    {
        Carousel = carousel;
        Direction = direction;
        Text = text;
        this.navigate = navigate;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Navigates the carousel in the requested direction.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task Navigate()
        => navigate.InvokeAsync();

    #endregion

    #region Properties

    /// <summary>
    /// Gets the carousel instance.
    /// </summary>
    public Carousel Carousel { get; }

    /// <summary>
    /// Gets the navigation direction.
    /// </summary>
    public CarouselDirection Direction { get; }

    /// <summary>
    /// Gets the localized button text.
    /// </summary>
    public string Text { get; }

    #endregion
}