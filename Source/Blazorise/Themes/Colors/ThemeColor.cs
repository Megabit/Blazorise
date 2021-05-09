#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Defines the color and its shade options.
    /// </summary>
    public record ThemeColor
    {
        #region Constructors

        /// <summary>
        /// A default <see cref="ThemeColor"/> constructor.
        /// </summary>
        /// <param name="key">Color key.</param>
        /// <param name="name">Color display name.</param>
        public ThemeColor( string key, string name )
        {
            Key = key;
            Name = name;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the color key.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets the color display name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the list of color shades.
        /// </summary>
        public Dictionary<string, ThemeColorShade> Shades { get; protected set; }

        #endregion
    }
}
