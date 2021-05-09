﻿namespace Blazorise
{
    /// <summary>
    /// Defines the theme options for the <see cref="Progress"/> component.
    /// </summary>
    public record ThemeProgressOptions : ThemeBasicOptions
    {
        /// <summary>
        /// Default color of the progress bar.
        /// </summary>
        public string PageProgressDefaultColor { get; set; } = "#ffffff";
    }
}
