namespace Blazorise.States
{
    /// <summary>
    /// Holds the information about the current state of the <see cref="Carousel"/> component.
    /// </summary>
    public record CarouselState
    {
        /// <summary>
        /// Autoplays the carousel slides from left to right.
        /// </summary>
        public bool Autoplay { get; init; }

        /// <summary>
        /// Animate slides with a fade transition instead of a slide.
        /// </summary>
        public bool Crossfade { get; init; }

        /// <summary>
        /// Gets or sets currently selected slide name.
        /// </summary>
        public string CurrentSlide { get; init; }
    }
}
