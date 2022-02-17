namespace Blazorise
{
    /// <summary>
    /// Defines how the modal content will be rendered.
    /// </summary>
    public enum ModalRenderMode
    {
        /// <summary>
        /// Always renders the modal html content to the DOM.
        /// </summary>
        Default,

        /// <summary>
        /// Lazy loads modal, meaning the modal html content will only be rendered/loaded the first time it is visited.
        /// </summary>
        LazyLoad,

        /// <summary>
        /// Lazy loads modal everytime, meaning the modal html content will have it's html re-rendered to the DOM everytime.
        /// </summary>
        LazyReload,
    }
}
