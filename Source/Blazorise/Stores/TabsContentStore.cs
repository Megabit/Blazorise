namespace Blazorise.Stores
{
    /// <summary>
    /// Holds the information about the current state of the <see cref="TabsContent"/> component.
    /// </summary>
    public record TabsContentStore
    {
        /// <summary>
        /// Gets or sets currently selected panel name.
        /// </summary>
        public string SelectedPanel { get; init; }
    }
}
