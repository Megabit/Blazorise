namespace Blazorise.States
{
    /// <summary>
    /// Holds the information about the current state of the <see cref="Steps"/> component.
    /// </summary>
    public record StepsState
    {
        /// <summary>
        /// Gets the name of the selected step item.
        /// </summary>
        public string SelectedStep { get; init; }
    }
}
