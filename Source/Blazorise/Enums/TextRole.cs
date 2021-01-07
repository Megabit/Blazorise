#region Using directives
#endregion

namespace Blazorise
{
    /// <summary>
    /// Defines the behaviour of the text input.
    /// </summary>
    public enum TextRole
    {
        /// <summary>
        /// Defines a default text input field.
        /// </summary>
        Text,

        /// <summary>
        /// Used for input fields that should contain an e-mail address.
        /// </summary>
        Email,

        /// <summary>
        /// Defines a password field.
        /// </summary>
        Password,

        /// <summary>
        /// Used for input fields that should contain a URL address.
        /// </summary>
        Url,

        /// <summary>
        /// Define a search field (like a site search, or Google search).
        /// </summary>
        Search,
    }
}
