#region Using directives
#endregion

namespace Blazorise
{
    /// <summary>
    /// Defines the text transformation.
    /// </summary>
    public enum TextTransform
    {
        /// <summary>
        /// No capitalization. The text renders as it is. This is default.
        /// </summary>
        None,

        /// <summary>
        /// Transforms all characters to lowercase.
        /// </summary>
        Lowercase,

        /// <summary>
        /// Transforms all characters to uppercase.
        /// </summary>
        Uppercase,

        /// <summary>
        /// Transforms the first character of each word to uppercase.
        /// </summary>
        Capitalize,
    }
}
