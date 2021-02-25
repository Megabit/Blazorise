namespace Blazorise.States
{
    /// <summary>
    /// Holds the information about the current state of the <see cref="Alert"/> component.
    /// </summary>
    public record AlertState
    {
        /// <summary>
        /// Enables the alert to be closed by placing the padding for close button.
        /// </summary>
        public bool Dismisable { get; init; }

        /// <summary>
        /// Gets or sets the alert visibilty state.
        /// </summary>
        public bool Visible { get; init; }

        /// <summary>
        /// Gets or sets the alert color.
        /// </summary>
        public Color Color { get; init; }
    }
}
