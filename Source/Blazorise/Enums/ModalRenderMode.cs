namespace Blazorise
{
    /// <summary>
    /// Defines the Modal Render Mode.
    /// </summary>
    public enum ModalRenderMode
    {
        /// <summary>
        /// Always renders the modal html content to the DOM.
        /// </summary>
        Default,

        /// <summary>
        /// Lazy loads modal, meaning the modal html content will only be rendered when the modal shows.
        /// </summary>
        LazyLoad,

    }
}
