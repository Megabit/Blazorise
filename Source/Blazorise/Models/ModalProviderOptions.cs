#region Using directives
#endregion

namespace Blazorise
{
    /// <summary>
    /// Sets the options for Modal Provider
    /// </summary>
    public class ModalProviderOptions
    {
        /// <summary>
        /// Uses the modal standard structure, by setting this to true you are only in charge of providing the custom content.
        /// Defaults to true.
        /// </summary>
        public bool UseModalStructure { get; set; } = true;

        /// <summary>
        /// Gets or Sets the modal's title.
        /// </summary>
        public string Title { get; set; }

    }
}
