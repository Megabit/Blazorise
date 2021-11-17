namespace Blazorise
{
    /// <summary>
    /// Specifies what kind of input mechanism would be most helpful for users entering content.
    /// </summary>
    public enum TextInputMode
    {
        /// <summary>
        /// The user agent should not display a virtual keyboard. This keyword is useful for content that renders its own keyboard control.
        /// </summary>
        None,

        /// <summary>
        /// The user agent should display a virtual keyboard capable of text input in the user's locale.
        /// </summary>
        Text,

        /// <summary>
        /// The user agent should display a virtual keyboard capable of telephone number input. This should including keys for the digits 0 to 9, the "#" character, and the "*" character. In some locales, this can also include alphabetic mnemonic labels (e.g., in the US, the key labeled "2" is historically also labeled with the letters A, B, and C).
        /// </summary>
        Tel,

        /// <summary>
        /// The user agent should display a virtual keyboard capable of text input in the user's locale, with keys for aiding in the input of URLs, such as that for the "/" and "." characters and for quick input of strings commonly found in domain names such as "www." or ".com".
        /// </summary>
        Url,

        /// <summary>
        /// The user agent should display a virtual keyboard capable of text input in the user's locale, with keys for aiding in the input of e-mail addresses, such as that for the "@" character and the "." character.
        /// </summary>
        Email,

        /// <summary>
        /// The user agent should display a virtual keyboard capable of numeric input. This keyword is useful for PIN entry.
        /// </summary>
        Numeric,

        /// <summary>
        /// The user agent should display a virtual keyboard capable of fractional numeric input. Numeric keys and the format separator for the locale should be shown.
        /// </summary>
        Decimal,

        /// <summary>
        /// The user agent should display a virtual keyboard optimized for search.
        /// </summary>
        Search,
    }
}
