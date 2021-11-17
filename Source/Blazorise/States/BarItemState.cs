namespace Blazorise.States
{
    /// <summary>
    /// Holds the information about the current state of the <see cref="BarItem"/> component.
    /// </summary>
    public record BarItemState
    {
        /// <summary>
        /// Gets or sets the flag to indicate if <see cref="BarItem"/> is active, or focused.
        /// </summary>
        public bool Active { get; init; }

        /// <summary>
        /// Gets or sets the disabled state to make <see cref="BarItem"/> inactive.
        /// </summary>
        public bool Disabled { get; init; }

        /// <summary>
        /// Gets or sets the bar mode in which the item is placed.
        /// </summary>
        public BarMode Mode { get; init; }

        /// <summary>
        /// Gets or sets the visibility of the bar component.
        /// </summary>
        public bool BarVisible { get; init; }
    }
}