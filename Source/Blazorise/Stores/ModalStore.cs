namespace Blazorise.Stores
{
    /// <summary>
    /// Holds the information about the current state of the <see cref="Modal"/> component.
    /// </summary>
    public record ModalStore
    {
        /// <summary>
        /// Defines the visibility of modal dialog.
        /// </summary>
        public bool Visible { get; init; }
    }
}
