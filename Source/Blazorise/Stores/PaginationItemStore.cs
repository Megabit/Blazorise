namespace Blazorise.Stores
{
    /// <summary>
    /// Holds the information about the current state of the <see cref="PaginationItem"/> component.
    /// </summary>
    public record PaginationItemStore
    {
        /// <summary>
        /// Indicate the currently active page.
        /// </summary>
        public bool Active { get; init; }

        /// <summary>
        /// Used for links that appear un-clickable.
        /// </summary>
        public bool Disabled { get; init; }
    }
}
