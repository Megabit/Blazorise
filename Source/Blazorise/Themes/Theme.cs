#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Holds the options for the theme.
    /// </summary>
    public class Theme
    {
        /// <summary>
        /// Globaly enable or disable the theme.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Event raised after the theme options has changed.
        /// </summary>
        public event EventHandler<EventArgs> Changed;

        /// <summary>
        /// Must be called to rebuild the theme.
        /// </summary>
        public void ThemeHasChanged()
        {
            Changed?.Invoke( this, EventArgs.Empty );
        }

        public string White { get; set; } = "#ffffff";

        public string Black { get; set; } = "#343a40";

        /// <summary>
        /// The yiq lightness value that determines when the lightness of color changes from "dark" to "light". Acceptable values are between 0 and 255.
        /// </summary>
        public byte LuminanceThreshold { get; set; } = 150;

        /// <summary>
        /// Enables the gradient background colors.
        /// </summary>
        public bool IsGradient { get; set; } = false;

        /// <summary>
        /// Globaly enables rounded elements.
        /// </summary>
        public bool IsRounded { get; set; } = true;

        /// <summary>
        /// Gets the valid breakpoints.
        /// </summary>
        public IEnumerable<(string name, string size)> ValidBreakpoints
            => BreakpointOptions?.Where( x => !string.IsNullOrEmpty( x.Value() ) ).Select( x => (x.Key, x.Value()) ) ?? Enumerable.Empty<(string, string)>();

        /// <summary>
        /// Gets the valid sizes for container.
        /// </summary>
        public IEnumerable<(string name, string size)> ValidContainerMaxWidths
            => ContainerMaxWidthOptions?.Where( x => !string.IsNullOrEmpty( x.Value() ) ).Select( x => (x.Key, x.Value()) ) ?? Enumerable.Empty<(string, string)>();

        /// <summary>
        /// Gets the valid variant colors.
        /// </summary>
        public IEnumerable<(string name, string color)> ValidColors
            => ColorOptions?.Where( x => !string.IsNullOrEmpty( x.Value() ) ).Select( x => (x.Key, x.Value()) ) ?? Enumerable.Empty<(string, string)>();

        /// <summary>
        /// Gets the valid background colors.
        /// </summary>
        public IEnumerable<(string name, string color)> ValidBackgroundColors
            => BackgroundOptions?.Where( x => !string.IsNullOrEmpty( x.Value() ) ).Select( x => (x.Key, x.Value()) ) ?? Enumerable.Empty<(string, string)>();

        /// <summary>
        /// Gets the valid text colors.
        /// </summary>
        public IEnumerable<(string name, string color)> ValidTextColors
            => TextColorOptions?.Where( x => !string.IsNullOrEmpty( x.Value() ) ).Select( x => (x.Key, x.Value()) ) ?? Enumerable.Empty<(string, string)>();

        /// <summary>
        /// Global options for media breakpoints.
        /// </summary>
        public ThemeBreakpointOptions BreakpointOptions { get; set; }

        /// <summary>
        /// Define the maximum width of container for different screen sizes.
        /// </summary>
        public ThemeContainerMaxWidthOptions ContainerMaxWidthOptions { get; set; }

        /// <summary>
        /// Used to override default theme colors.
        /// </summary>
        public ThemeColorOptions ColorOptions { get; set; }

        /// <summary>
        /// Used to override default background colors.
        /// </summary>
        public ThemeBackgroundOptions BackgroundOptions { get; set; }

        /// <summary>
        /// Used to override default background colors.
        /// </summary>
        public ThemeTextColorOptions TextColorOptions { get; set; }

        /// <summary>
        /// Set a specific jump point for requesting color jumps
        /// </summary>
        public float ThemeColorInterval { get; set; } = 8f;

        /// <summary>
        /// Overrides the button styles.
        /// </summary>
        public ThemeButtonOptions ButtonOptions { get; set; }

        public ThemeDropdownOptions DropdownOptions { get; set; }

        public ThemeInputOptions InputOptions { get; set; }

        public ThemeCardOptions CardOptions { get; set; }

        public ThemeModalOptions ModalOptions { get; set; }

        public ThemeTabsOptions TabsOptions { get; set; }

        public ThemeProgressOptions ProgressOptions { get; set; }

        public ThemeAlertOptions AlertOptions { get; set; }

        public ThemeTableOptions TableOptions { get; set; }

        public ThemeBreadcrumbOptions BreadcrumbOptions { get; set; }

        public ThemeBadgeOptions BadgeOptions { get; set; }

        public ThemeSwitchOptions SwitchOptions { get; set; }

        public ThemePaginationOptions PaginationOptions { get; set; }

        public ThemeSidebarOptions SidebarOptions { get; set; }

        public ThemeSnackbarOptions SnackbarOptions { get; set; }

        public ThemeStepsOptions StepsOptions { get; set; }

        public ThemeBarOptions BarOptions { get; set; }

        public ThemeDividerOptions DividerOptions { get; set; }

        public ThemeTooltipOptions TooltipOptions { get; set; }
    }
}
