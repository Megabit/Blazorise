namespace Blazorise
{
    /// <summary>
    /// Specifies the case of characters in an element.
    /// </summary>
    public enum CharacterCasing
    {
        /// <summary>
        /// The case of characters is left unchanged.
        /// </summary>
        Normal = 0,

        /// <summary>
        /// Converts all characters to uppercase.
        /// </summary>
        Upper = 1,

        /// <summary>
        /// Converts all characters to lowercase.
        /// </summary>
        Lower = 2,

        /// <summary>
        /// Convert first character to uppercase and all other to lowercase.
        /// </summary>
        Title = 3,
    }
}
