namespace Blazorise.Stores
{
    public record AlertStore
    {
        public bool Dismisable { get; init; }

        public bool Visible { get; init; }

        public Color Color { get; init; }
    }
}
