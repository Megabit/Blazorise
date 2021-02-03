namespace Blazorise.States
{
    /// <summary>
    /// Holds the information about the current state of the <see cref="Bar"/> component.
    /// </summary>
    public record BarState
    {
        /// <summary>
        /// Controlls the state of toggler and the menu.
        /// </summary>
        public bool Visible { get; init; }

        /// <summary>
        /// Defines the orientation for the bar. Vertical is required when using as a Sidebar.
        /// </summary>
        public BarMode Mode { get; init; }

        /// <summary>
        /// Defines how the bar will be collapsed.
        /// </summary>
        public BarCollapseMode CollapseMode { get; init; }

        /// <summary>
        /// Used for responsive collapsing.
        /// </summary>
        public Breakpoint Breakpoint { get; init; }

        /// <summary>
        /// Used for responsive collapsing after Navigation.
        /// </summary>
        public Breakpoint NavigationBreakpoint { get; init; }

        /// <summary>
        /// Defines the prefered theme contrast for the <see cref="Bar"/> component.
        /// </summary>
        public ThemeContrast ThemeContrast { get; init; }

        /// <summary>
        /// Defines the alignment within bar.
        /// </summary>
        public Alignment Alignment { get; init; }

        /// <summary>
        /// Gets or sets the bar background color.
        /// </summary>
        public Background Background { get; init; }
    }
}