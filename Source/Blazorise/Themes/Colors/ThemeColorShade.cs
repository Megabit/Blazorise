namespace Blazorise
{
    /// <summary>
    /// Defines the color shade.
    /// </summary>
    public record ThemeColorShade
    {
        #region Constructors

        /// <summary>
        /// A default <see cref="ThemeColorShade"/> constructor.
        /// </summary>
        /// <param name="key">Shade key.</param>
        /// <param name="name">Shade display name.</param>
        /// <param name="value">Shade color.</param>
        public ThemeColorShade( string key, string name, string value )
        {
            Key = key;
            Name = name;
            Value = value;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the shade key.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets the shade display name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the shade color.
        /// </summary>
        public string Value { get; }

        #endregion
    }
}
