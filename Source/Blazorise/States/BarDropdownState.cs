namespace Blazorise.States
{
    /// <summary>
    /// Holds the information about the current state of the <see cref="BarDropdown"/> component.
    /// </summary>
    public record BarDropdownState
    {
        /// <summary>
        /// Gets or sets the dropdown menu visibility state.
        /// </summary>
        public bool Visible { get; init; }

        /// <summary>
        /// If true, a dropdown menu will be right aligned.
        /// </summary>
        public bool RightAligned { get; init; }

        /// <summary>
        /// Gets or sets the bar mode in which the dropdown is placed.
        /// </summary>
        public BarMode Mode { get; init; }

        /// <summary>
        /// Gets or sets the visibility of the bar component.
        /// </summary>
        public bool BarVisible { get; init; }

        /// <summary>
        /// Gets or sets the hierarchy index of the dropdown in the bar component.
        /// </summary>
        public int NestedIndex { get; init; }

        /// <summary>
        /// True if dropdown is in vertical inline mode and the bar is visible.
        /// </summary>
        public bool IsInlineDisplay => Mode == BarMode.VerticalInline && BarVisible;
    }
}