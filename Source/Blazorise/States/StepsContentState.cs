namespace Blazorise.States
{
    /// <summary>
    /// Holds the information about the current state of the <see cref="Steps"/> component.
    /// </summary>
    public record StepsContentState
    {
        /// <summary>
        /// Gets the name of the selected step panel.
        /// </summary>
        public string SelectedPanel { get; init; }
    }
}
