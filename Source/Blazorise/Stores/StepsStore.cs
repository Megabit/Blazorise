namespace Blazorise.Stores
{
    /// <summary>
    /// Holds the information about the current state of the <see cref="Steps"/> component.
    /// </summary>
    public record StepsStore
    {
        /// <summary>
        /// Gets the name of the selected step item.
        /// </summary>
        public string SelectedStep { get; init; }
    }
}
