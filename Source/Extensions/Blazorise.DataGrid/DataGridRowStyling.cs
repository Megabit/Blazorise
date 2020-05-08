#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace Blazorise.DataGrid
{
    /// <summary>
    /// Helpers class to override default row styling.
    /// </summary>
    public class DataGridRowStyling
    {
        /// <summary>
        /// Row custom classnames.
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// Row custom styles.
        /// </summary>
        public string Style { get; set; }

        /// <summary>
        /// Row custom color.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Row custom background color.
        /// </summary>
        public Background Background { get; set; }
    }
}
