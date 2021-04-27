namespace Blazorise
{
    /// <summary>
    /// Defines the color options for the <see cref="Bar"/> component.
    /// </summary>
    public record ThemeBarColorOptions
    {
        /// <summary>
        /// Gets or sets the bar background color.
        /// </summary>
        public string BackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the bar text color.
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="BarDropdownItem"/> color options.
        /// </summary>
        public ThemeBarItemColorOptions ItemColorOptions { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="BarDropdown"/> color options.
        /// </summary>
        public ThemeBarDropdownColorOptions DropdownColorOptions { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="BarBrand"/> color options.
        /// </summary>
        public ThemeBarBrandColorOptions BrandColorOptions { get; set; }
    }
}
