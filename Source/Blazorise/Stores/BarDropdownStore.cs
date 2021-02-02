namespace Blazorise.Stores
{
    public record BarDropdownStore
    {
        public bool Visible { get; init; }

        public BarMode Mode { get; init; }

        public bool BarVisible { get; init; }

        public int NestedIndex { get; init; }

        public bool IsInlineDisplay => Mode == BarMode.VerticalInline && BarVisible;
    }
}