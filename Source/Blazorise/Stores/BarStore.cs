namespace Blazorise.Stores
{
    public record BarStore
    {
        public bool Visible { get; init; }

        public BarMode Mode { get; init; }

        public BarCollapseMode CollapseMode { get; init; }

        public Breakpoint Breakpoint { get; init; }

        public Breakpoint NavigationBreakpoint { get; init; }

        public ThemeContrast ThemeContrast { get; init; }

        public Alignment Alignment { get; init; }

        public Background Background { get; init; }
    }
}