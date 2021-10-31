namespace Blazorise
{
    /// <summary>
    /// Defines the Tabs Mode
    /// </summary>
    public enum TabsMode
    {
        /// <summary>
        /// Always renders the tabs html content to the DOM.
        /// </summary>
        Default,

        /// <summary>
        /// Lazy loads tabs, meaning only the active tab will have it's html rendered to the DOM.
        /// </summary>
        LazyLoad,
    }
}
