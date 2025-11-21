#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using Blazorise.Generator.Features;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Holds the options for the theme.
    /// </summary>
    [GenerateEquality]
    public partial record Theme
    {
        /// <summary>
        /// Globaly enable or disable the theme.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Event raised after the theme options has changed.
        /// </summary>
        [GenerateIgnoreEquality]
        public event EventHandler<EventArgs> Changed;

        /// <summary>
        /// Must be called to rebuild the theme.
        /// </summary>
        public void ThemeHasChanged()
        {
            Changed?.Invoke( this, EventArgs.Empty );
        }

        /// <summary>
        /// Defines the base theme light color.
        /// </summary>
        public string White { get; set; } = "#ffffff";

        /// <summary>
        /// Defines the base theme dark color.
        /// </summary>
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
        /// Defined the body options.
        /// </summary>
        public ThemeBodyOptions BodyOptions { get; set; }

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
        /// Used to override default border styles.
        /// </summary>
        public ThemeBorderOptions BorderOptions { get; set; }

        /// <summary>
        /// Used to override default background colors.
        /// </summary>
        public ThemeTextColorOptions TextColorOptions { get; set; }

        /// <summary>
        /// Set a specific jump point for requesting color jumps
        /// </summary>
        public float ThemeColorInterval { get; set; } = 8f;

        /// <summary>
        /// Theme options to override the <see cref="Button"/> component styles.
        /// </summary>
        public ThemeButtonOptions ButtonOptions { get; set; }

        /// <summary>
        /// Theme options to override the <see cref="Dropdown"/> styles.
        /// </summary>
        public ThemeDropdownOptions DropdownOptions { get; set; }

        /// <summary>
        /// Theme options to override the <see cref="BaseInputComponent{TValue}">Input</see> component(s) styles.
        /// </summary>
        public ThemeInputOptions InputOptions { get; set; }

        /// <summary>
        /// Theme options to override the <see cref="Card"/> styles.
        /// </summary>
        public ThemeCardOptions CardOptions { get; set; }

        /// <summary>
        /// Theme options to override the <see cref="Modal">Modal dialog</see> styles.
        /// </summary>
        public ThemeModalOptions ModalOptions { get; set; }

        /// <summary>
        /// Theme options to override the <see cref="Tabs"/> component styles.
        /// </summary>
        public ThemeTabsOptions TabsOptions { get; set; }

        /// <summary>
        /// Theme options to override the <see cref="Progress{T}">Progress</see> component styles.
        /// </summary>
        public ThemeProgressOptions ProgressOptions { get; set; }

        /// <summary>
        /// Theme options to override the <see cref="Rating"/> component styles.
        /// </summary>
        public ThemeRatingOptions RatingOptions { get; set; }

        /// <summary>
        /// Theme options to override the <see cref="Alert"/> component styles.
        /// </summary>
        public ThemeAlertOptions AlertOptions { get; set; }

        /// <summary>
        /// Theme options to override the <see cref="Table"/> component styles.
        /// </summary>
        public ThemeTableOptions TableOptions { get; set; }

        /// <summary>
        /// Theme options to override the <see cref="Breadcrumb"/> component styles.
        /// </summary>
        public ThemeBreadcrumbOptions BreadcrumbOptions { get; set; }

        /// <summary>
        /// Theme options to override the <see cref="Badge"/> component styles.
        /// </summary>
        public ThemeBadgeOptions BadgeOptions { get; set; }

        /// <summary>
        /// Theme options to override the <see cref="Switch{TValue}">Switch</see> component styles.
        /// </summary>
        public ThemeSwitchOptions SwitchOptions { get; set; }

        /// <summary>
        /// Theme options to override the <see cref="Pagination"/> component styles.
        /// </summary>
        public ThemePaginationOptions PaginationOptions { get; set; }

        /// <summary>
        /// Theme options to override the Sidebar component styles.
        /// </summary>
        public ThemeSidebarOptions SidebarOptions { get; set; }

        /// <summary>
        /// Theme options to override the Snackbar component styles.
        /// </summary>
        public ThemeSnackbarOptions SnackbarOptions { get; set; }

        /// <summary>
        /// Theme options to override the <see cref="Steps"/> component styles.
        /// </summary>
        public ThemeStepsOptions StepsOptions { get; set; }

        /// <summary>
        /// Theme options to override the <see cref="Bar"/> component styles.
        /// </summary>
        public ThemeBarOptions BarOptions { get; set; }

        /// <summary>
        /// Theme options to override the <see cref="Divider"/> component styles.
        /// </summary>
        public ThemeDividerOptions DividerOptions { get; set; }

        /// <summary>
        /// Theme options to override the <see cref="Tooltip"/> component styles.
        /// </summary>
        public ThemeTooltipOptions TooltipOptions { get; set; }

        /// <summary>
        /// Theme options to override the SpinKit component styles.
        /// </summary>
        public ThemeSpinKitOptions SpinKitOptions { get; set; }

        /// <summary>
        /// Theme options to override the <see cref="ListGroupItem"/> component styles.
        /// </summary>
        public ThemeListGroupItemOptions ListGroupItemOptions { get; set; }

        /// <summary>
        /// Theme options to override the spacing utilities.
        /// </summary>
        public ThemeSpacingOptions SpacingOptions { get; set; }
    }
}