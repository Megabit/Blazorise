namespace Blazorise
{
    /// <summary>
    /// Defines the Tabs Mode.
    /// </summary>
    public enum TabsRenderMode
    {
        /// <summary>
        /// Always renders the tabs html content to the DOM.
        /// </summary>
        Default,

        /// <summary>
        /// Lazy loads tabs, meaning each tab will only be rendered/loaded the first time it is visited.
        /// </summary>
        LazyLoad,

        /// <summary>
        /// Lazy loads tabs everytime, meaning only the active tab will have it's html rendered to the DOM.
        /// </summary>
        LazyReload,
    }
}
