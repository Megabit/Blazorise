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

        public ThemeRatingOptions RatingOptions { get; set; }

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

        public override bool Equals( object obj )
        {
            return obj is Theme theme &&
                     Enabled == theme.Enabled &&
                     White == theme.White &&
                     Black == theme.Black &&
                     LuminanceThreshold == theme.LuminanceThreshold &&
                     IsGradient == theme.IsGradient &&
                     IsRounded == theme.IsRounded &&
                    EqualityComparer<ThemeBreakpointOptions>.Default.Equals( BreakpointOptions, theme.BreakpointOptions ) &&
                    EqualityComparer<ThemeContainerMaxWidthOptions>.Default.Equals( ContainerMaxWidthOptions, theme.ContainerMaxWidthOptions ) &&
                    EqualityComparer<ThemeColorOptions>.Default.Equals( ColorOptions, theme.ColorOptions ) &&
                    EqualityComparer<ThemeBackgroundOptions>.Default.Equals( BackgroundOptions, theme.BackgroundOptions ) &&
                    EqualityComparer<ThemeTextColorOptions>.Default.Equals( TextColorOptions, theme.TextColorOptions ) &&
                     ThemeColorInterval == theme.ThemeColorInterval &&
                    EqualityComparer<ThemeButtonOptions>.Default.Equals( ButtonOptions, theme.ButtonOptions ) &&
                    EqualityComparer<ThemeDropdownOptions>.Default.Equals( DropdownOptions, theme.DropdownOptions ) &&
                    EqualityComparer<ThemeInputOptions>.Default.Equals( InputOptions, theme.InputOptions ) &&
                    EqualityComparer<ThemeCardOptions>.Default.Equals( CardOptions, theme.CardOptions ) &&
                    EqualityComparer<ThemeModalOptions>.Default.Equals( ModalOptions, theme.ModalOptions ) &&
                    EqualityComparer<ThemeTabsOptions>.Default.Equals( TabsOptions, theme.TabsOptions ) &&
                    EqualityComparer<ThemeProgressOptions>.Default.Equals( ProgressOptions, theme.ProgressOptions ) &&
                    EqualityComparer<ThemeRatingOptions>.Default.Equals( RatingOptions, theme.RatingOptions ) &&
                    EqualityComparer<ThemeAlertOptions>.Default.Equals( AlertOptions, theme.AlertOptions ) &&
                    EqualityComparer<ThemeTableOptions>.Default.Equals( TableOptions, theme.TableOptions ) &&
                    EqualityComparer<ThemeBreadcrumbOptions>.Default.Equals( BreadcrumbOptions, theme.BreadcrumbOptions ) &&
                    EqualityComparer<ThemeBadgeOptions>.Default.Equals( BadgeOptions, theme.BadgeOptions ) &&
                    EqualityComparer<ThemeSwitchOptions>.Default.Equals( SwitchOptions, theme.SwitchOptions ) &&
                    EqualityComparer<ThemePaginationOptions>.Default.Equals( PaginationOptions, theme.PaginationOptions ) &&
                    EqualityComparer<ThemeSidebarOptions>.Default.Equals( SidebarOptions, theme.SidebarOptions ) &&
                    EqualityComparer<ThemeSnackbarOptions>.Default.Equals( SnackbarOptions, theme.SnackbarOptions ) &&
                    EqualityComparer<ThemeStepsOptions>.Default.Equals( StepsOptions, theme.StepsOptions ) &&
                    EqualityComparer<ThemeBarOptions>.Default.Equals( BarOptions, theme.BarOptions ) &&
                    EqualityComparer<ThemeDividerOptions>.Default.Equals( DividerOptions, theme.DividerOptions ) &&
                    EqualityComparer<ThemeTooltipOptions>.Default.Equals( TooltipOptions, theme.TooltipOptions );
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add( Enabled );
            hash.Add( White );
            hash.Add( Black );
            hash.Add( LuminanceThreshold );
            hash.Add( IsGradient );
            hash.Add( IsRounded );
            hash.Add( BreakpointOptions );
            hash.Add( ContainerMaxWidthOptions );
            hash.Add( ColorOptions );
            hash.Add( BackgroundOptions );
            hash.Add( TextColorOptions );
            hash.Add( ThemeColorInterval );
            hash.Add( ButtonOptions );
            hash.Add( DropdownOptions );
            hash.Add( InputOptions );
            hash.Add( CardOptions );
            hash.Add( ModalOptions );
            hash.Add( TabsOptions );
            hash.Add( ProgressOptions );
            hash.Add( RatingOptions );
            hash.Add( AlertOptions );
            hash.Add( TableOptions );
            hash.Add( BreadcrumbOptions );
            hash.Add( BadgeOptions );
            hash.Add( SwitchOptions );
            hash.Add( PaginationOptions );
            hash.Add( SidebarOptions );
            hash.Add( SnackbarOptions );
            hash.Add( StepsOptions );
            hash.Add( BarOptions );
            hash.Add( DividerOptions );
            hash.Add( TooltipOptions );
            return hash.ToHashCode();
        }
    }
}
