namespace Blazorise.Stores
{
    public record BarItemStore
    {
        public bool Active { get; init; }

        public bool Disabled { get; init; }

        public BarMode Mode { get; init; }

        public bool BarVisible { get; init; }
    }
}