namespace Blazorise
{
    /// <summary>
    /// Shared attributes for <see cref="ModalProvider"/> modal instances.
    /// </summary>
    public interface IModalInstance : IDomComponent
    {
        /// <summary>
        /// Centers the modal vertically.
        /// </summary>
        /// <remarks>
        /// Only considered if <see cref="ModalInstanceOptions.UseModalStructure"/> is set.
        /// </remarks>
        bool Centered { get; set; }

        /// <summary>
        /// Scrolls the modal content independent of the page itself.
        /// </summary>
        /// <remarks>
        /// Only considered if <see cref="ModalInstanceOptions.UseModalStructure"/> is set.
        /// </remarks>
        bool Scrollable { get; set; }

        /// <summary>
        /// Changes the size of the modal.
        /// </summary>
        /// <remarks>
        /// Only considered if <see cref="ModalInstanceOptions.UseModalStructure"/> is set.
        /// </remarks>
        ModalSize Size { get; set; }
    }
}
