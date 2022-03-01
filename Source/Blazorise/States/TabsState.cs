namespace Blazorise.States
{
    /// <summary>
    /// Holds the information about the current state of the <see cref="Tabs"/> component.
    /// </summary>
    public record TabsState
    {
        /// <summary>
        /// Makes the tab items to appear as pills.
        /// </summary>
        public bool Pills { get; init; }

        /// <summary>
        /// Makes the tab items to extend the full available width.
        /// </summary>
        public bool FullWidth { get; init; }

        /// <summary>
        /// Makes the tab items to extend the full available width, but every item will be the same width.
        /// </summary>
        public bool Justified { get; init; }

        /// <summary>
        /// Position of tab items.
        /// </summary>
        public TabPosition TabPosition { get; init; }

        /// <summary>
        /// Gets or sets the tabs rendering mode.
        /// </summary>
        public TabsRenderMode RenderMode { get; init; }

        /// <summary>
        /// Gets or sets currently selected tab name.
        /// </summary>
        public string SelectedTab { get; init; }
    }
}
