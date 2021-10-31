namespace Blazorise
{
    /// <summary>
    /// Defines the Tabs Mode
    /// </summary>
    public enum TabsMode
    {
        /// <summary>
        /// Always renders tabs as hidden html in the page.
        /// </summary>
        Default,

        /// <summary>
        /// Lazy loads the tab, meaning only the active tab will have it's html rendered on the page.
        /// </summary>
        LazyLoad,
    }
}
