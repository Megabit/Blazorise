namespace Blazorise.Stores
{
    public record CarouselStore
    {
        public bool Autoplay { get; init; }

        public bool Crossfade { get; init; }

        public string CurrentSlide { get; init; }
    }
}
