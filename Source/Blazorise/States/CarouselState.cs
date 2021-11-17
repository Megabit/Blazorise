namespace Blazorise.States
{
    /// <summary>
    /// Holds the information about the current state of the <see cref="Carousel"/> component.
    /// </summary>
    public record CarouselState
    {
        /// <summary>
        /// Auto-plays the carousel slides from left to right.
        /// </summary>
        public bool Autoplay { get; init; }

        /// <summary>
        /// Auto-repeats the carousel slides from the beginning once it reaches the end slide.
        /// </summary>
        public bool AutoRepeat { get; init; }

        /// <summary>
        /// Animate slides with a fade transition instead of a slide animation.
        /// </summary>
        public bool Crossfade { get; init; }

        /// <summary>
        /// Gets or sets currently selected slide name.
        /// </summary>
        public string SelectedSlide { get; init; }

        /// <summary>
        /// Gets or sets previously selected slide name.
        /// </summary>
        public string PreviouslySelectedSlide { get; init; }
    }
}
