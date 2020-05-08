#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Holds the options for the theme.
    /// </summary>
    public class Theme
    {
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
        public IEnumerable<(string name, string color)> ValidBreakpoints
            => BreakpointOptions?.Where( x => !string.IsNullOrEmpty( x.Value() ) ).Select( x => (x.Key, x.Value()) ) ?? Enumerable.Empty<(string, string)>();

        /// <summary>
        /// Gets the valid sizes for container.
        /// </summary>
        public IEnumerable<(string name, string color)> ValidContainerMaxWidths
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
        public ThemeContanerMaxWidthOptions ContainerMaxWidthOptions { get; set; }

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

        public ThemePaginationOptions PaginationOptions { get; set; }

        public ThemeSidebarOptions SidebarOptions { get; set; }

        public ThemeSnackbarOptions SnackbarOptions { get; set; }

        public ThemeBarOptions BarOptions { get; set; }

        public ThemeDividerOptions DividerOptions { get; set; }

        public ThemeTooltipOptions TooltipOptions { get; set; }
    }

    public class BasicOptions
    {
        public string BorderRadius { get; set; } = ".25rem";

        public virtual bool HasOptions()
        {
            return !string.IsNullOrEmpty( BorderRadius );
        }
    }

    public class ThemeBreakpointOptions : IEnumerable<KeyValuePair<string, Func<string>>>
    {
        private Dictionary<string, Func<string>> breakpointMap => new Dictionary<string, Func<string>> {
            { "mobile", () => Mobile },
            { "tablet", () => Tablet },
            { "desktop", () => Desktop },
            { "widescreen", () => Widescreen },
            { "fullhd", () => FullHD },
        };

        public IEnumerator<KeyValuePair<string, Func<string>>> GetEnumerator()
        {
            return breakpointMap.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return breakpointMap.GetEnumerator();
        }

        public Func<string> this[string key] => breakpointMap[key];

        public string Mobile { get; set; } = "576px";

        public string Tablet { get; set; } = "768px";

        public string Desktop { get; set; } = "992px";

        public string Widescreen { get; set; } = "1200px";

        public string FullHD { get; set; } = "1400px";
    }

    public class ThemeContanerMaxWidthOptions : IEnumerable<KeyValuePair<string, Func<string>>>
    {
        private Dictionary<string, Func<string>> breakpointMap => new Dictionary<string, Func<string>> {
            { "mobile", () => Mobile },
            { "tablet", () => Tablet },
            { "desktop", () => Desktop },
            { "widescreen", () => Widescreen },
            { "fullhd", () => FullHD },
        };

        public IEnumerator<KeyValuePair<string, Func<string>>> GetEnumerator()
        {
            return breakpointMap.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return breakpointMap.GetEnumerator();
        }

        public Func<string> this[string key] => breakpointMap[key];

        public string Mobile { get; set; } = "540px";

        public string Tablet { get; set; } = "720px";

        public string Desktop { get; set; } = "960px";

        public string Widescreen { get; set; } = "1140px";

        public string FullHD { get; set; } = "1320px";
    }

    public class ThemeButtonOptions : BasicOptions
    {
        public string Padding { get; set; }

        public string Margin { get; set; }

        public string BoxShadowSize { get; set; }

        public byte BoxShadowTransparency { get; set; } = 127;

        public float HoverDarkenColor { get; set; } = 15f;

        public float HoverLightenColor { get; set; } = 20f;

        public float ActiveDarkenColor { get; set; } = 20f;

        public float ActiveLightenColor { get; set; } = 25f;

        public string LargeBorderRadius { get; set; } = ".3rem";

        public string SmallBorderRadius { get; set; } = ".2rem";

        public float GradientBlendPercentage { get; set; } = 15f;
    }

    public class ThemeDropdownOptions : BasicOptions
    {
        public float GradientBlendPercentage { get; set; } = 15f;
    }

    public class ThemeInputOptions : BasicOptions
    {
        public string Color { get; set; }

        public string CheckColor { get; set; }

        public string SliderColor { get; set; }

        public override bool HasOptions()
        {
            return !string.IsNullOrEmpty( Color )
                || !string.IsNullOrEmpty( CheckColor )
                || !string.IsNullOrEmpty( SliderColor )
                || base.HasOptions();
        }
    }

    public class ThemeCardOptions : BasicOptions
    {
        public string ImageTopRadius { get; set; } = "calc(.25rem - 1px)";
    }

    public class ThemeJumbotronOptions : BasicOptions
    {
    }

    public class ThemeModalOptions : BasicOptions
    {
    }

    public class ThemeTabsOptions : BasicOptions
    {
    }

    public class ThemeProgressOptions : BasicOptions
    {
    }

    public class ThemeAlertOptions : BasicOptions
    {
        public int BackgroundLevel { get; set; } = -10;

        public int BorderLevel { get; set; } = -7;

        public int ColorLevel { get; set; } = 6;

        public float GradientBlendPercentage { get; set; } = 15f;
    }

    public class ThemeTableOptions : BasicOptions
    {
        public int BackgroundLevel { get; set; } = -9;

        public int BorderLevel { get; set; } = -6;
    }

    public class ThemeBreadcrumbOptions : BasicOptions
    {
        public string Color { get; set; } = ThemeColors.Blue.Shades["400"].Value;
    }

    public class ThemeBadgeOptions : BasicOptions
    {
    }

    public class ThemePaginationOptions : BasicOptions
    {
        public string LargeBorderRadius { get; set; } = ".3rem";
    }

    public class ThemeBarOptions
    {
    }

    public class ThemeDividerOptions
    {
        public string Color { get; set; } = "#999999";

        public string Thickness { get; set; } = "2px";

        public string TextSize { get; set; } = ".85rem";
    }

    public class ThemeParagraphOptions
    {
    }

    public class ThemeTooltipOptions : BasicOptions
    {
        public string BackgroundColor { get; set; } = "#808080";

        public float? BackgroundOpacity { get; set; } = 90f;

        public string Color { get; set; } = "#ffffff";

        public string FontSize { get; set; } = ".875rem";

        public string FadeTime { get; set; } = "0.3s";

        public string MaxWidth { get; set; } = "15rem";

        public string Padding { get; set; } = ".5rem 1rem";

        public string ZIndex { get; set; } = "1020";
    }

    public class ThemeColorOptions : IEnumerable<KeyValuePair<string, Func<string>>>
    {
        private Dictionary<string, Func<string>> colorMap => new Dictionary<string, Func<string>> {
            { "primary", () => Primary },
            { "secondary", () => Secondary },
            { "success", () => Success },
            { "info", () => Info },
            { "warning", () => Warning },
            { "danger", () => Danger },
            { "light", () => Light },
            { "dark", () => Dark }
        };

        public IEnumerator<KeyValuePair<string, Func<string>>> GetEnumerator()
        {
            return colorMap.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return colorMap.GetEnumerator();
        }

        /// <summary>
        /// Gets the color handler associated with the specified color key.
        /// </summary>
        /// <param name="key">Color key</param>
        /// <returns>Return the color getter.</returns>
        public Func<string> this[string key] => colorMap[key];

        public string Primary { get; set; } = ThemeColors.Blue.Shades["400"].Value;

        public string Secondary { get; set; } = ThemeColors.Gray.Shades["600"].Value;

        public string Success { get; set; } = ThemeColors.Green.Shades["500"].Value;

        public string Info { get; set; } = ThemeColors.Cyan.Shades["500"].Value;

        public string Warning { get; set; } = ThemeColors.Yellow.Shades["500"].Value;

        public string Danger { get; set; } = ThemeColors.Red.Shades["500"].Value;

        public string Light { get; set; } = ThemeColors.Gray.Shades["100"].Value;

        public string Dark { get; set; } = ThemeColors.Gray.Shades["800"].Value;
    }

    public class ThemeBackgroundOptions : IEnumerable<KeyValuePair<string, Func<string>>>
    {
        private Dictionary<string, Func<string>> colorMap => new Dictionary<string, Func<string>> {
            { "primary", () => Primary },
            { "secondary", () => Secondary },
            { "success", () => Success },
            { "info", () => Info },
            { "warning", () => Warning },
            { "danger", () => Danger },
            { "light", () => Light },
            { "dark", () => Dark },
            { "body", () => Body },
            { "muted", () => Muted }
        };

        public IEnumerator<KeyValuePair<string, Func<string>>> GetEnumerator()
        {
            return colorMap.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return colorMap.GetEnumerator();
        }

        /// <summary>
        /// Gets the color handler associated with the specified color key.
        /// </summary>
        /// <param name="key">Color key</param>
        /// <returns>Return the color getter.</returns>
        public Func<string> this[string key] => colorMap[key];

        public string Primary { get; set; } = ThemeColors.Blue.Shades["400"].Value;

        public string Secondary { get; set; } = ThemeColors.Gray.Shades["600"].Value;

        public string Success { get; set; } = ThemeColors.Green.Shades["500"].Value;

        public string Info { get; set; } = ThemeColors.Cyan.Shades["500"].Value;

        public string Warning { get; set; } = ThemeColors.Yellow.Shades["500"].Value;

        public string Danger { get; set; } = ThemeColors.Red.Shades["500"].Value;

        public string Light { get; set; } = ThemeColors.Gray.Shades["100"].Value;

        public string Dark { get; set; } = ThemeColors.Gray.Shades["800"].Value;

        public string Body { get; set; }

        public string Muted { get; set; }
    }

    public class ThemeTextColorOptions : IEnumerable<KeyValuePair<string, Func<string>>>
    {
        private Dictionary<string, Func<string>> colorMap => new Dictionary<string, Func<string>> {
            { "primary", () => Primary },
            { "secondary", () => Secondary },
            { "success", () => Success },
            { "info", () => Info },
            { "warning", () => Warning },
            { "danger", () => Danger },
            { "light", () => Light },
            { "dark", () => Dark },
            { "body", () => Body },
            { "muted", () => Muted },
            { "white", () => White },
            { "black50", () => Black50 },
            { "white50", () => White50 },
        };

        public IEnumerator<KeyValuePair<string, Func<string>>> GetEnumerator()
        {
            return colorMap.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return colorMap.GetEnumerator();
        }

        /// <summary>
        /// Gets the color handler associated with the specified color key.
        /// </summary>
        /// <param name="key">Color key</param>
        /// <returns>Return the color getter.</returns>
        public Func<string> this[string key] => colorMap[key];

        public string Primary { get; set; } = ThemeColors.Blue.Shades["400"].Value;

        public string Secondary { get; set; } = ThemeColors.Gray.Shades["600"].Value;

        public string Success { get; set; } = ThemeColors.Green.Shades["500"].Value;

        public string Info { get; set; } = ThemeColors.Cyan.Shades["500"].Value;

        public string Warning { get; set; } = ThemeColors.Yellow.Shades["500"].Value;

        public string Danger { get; set; } = ThemeColors.Red.Shades["500"].Value;

        public string Light { get; set; } = ThemeColors.Gray.Shades["100"].Value;

        public string Dark { get; set; } = ThemeColors.Gray.Shades["800"].Value;

        public string Body { get; set; } = ThemeColors.Gray.Shades["900"].Value;

        public string Muted { get; set; } = ThemeColors.Gray.Shades["600"].Value;

        public string White { get; set; } = ThemeColors.Gray.Shades["50"].Value;

        public string Black50 { get; set; } = "#000000";

        public string White50 { get; set; } = "#FFFFFF";
    }

    public class ThemeSidebarOptions
    {
        public string Width { get; set; } = "220px";

        public string BackgroundColor { get; set; } = "#343a40";

        public string Color { get; set; } = "#ced4da";
    }

    public class ThemeSnackbarOptions
    {
        /// <summary>
        /// Default snackbar color.
        /// </summary>
        public string BackgroundColor { get; set; }

        /// <summary>
        /// Default text color.
        /// </summary>
        public string TextColor { get; set; }

        /// <summary>
        /// Default button color.
        /// </summary>
        public string ButtonColor { get; set; }

        /// <summary>
        /// Default button hover color.
        /// </summary>
        public string ButtonHoverColor { get; set; }

        public int VariantBackgroundColorLevel { get; set; } = -3;

        //public int VariantTextColorLevel { get; set; } = 6;

        //public int VariantButtonColorLevel { get; set; } = 8;

        //public int VariantButtonHoverColorLevel { get; set; } = 4;
    }

    public static class ThemeVariables
    {
        public const string White = "--b-theme-white";
        public const string Black = "--b-theme-black";

        public const string BorderRadius = "--b-border-radius";
        public const string BorderRadiusLarge = "--b-border-radius-lg";
        public const string BorderRadiusSmall = "--b-border-radius-sm";

        /// <summary>
        /// Gets the theme color variable name.
        /// </summary>
        /// <param name="variant">Color variant name.</param>
        /// <returns></returns>
        public static string Color( string variant ) => $"--b-theme-{variant}";

        /// <summary>
        /// Gets the breakpoint variable name.
        /// </summary>
        /// <param name="name">Breakpoint name.</param>
        /// <returns></returns>
        public static string Breakpoint( string name ) => $"--b-theme-breakpoint-{name}";

        /// <summary>
        /// Gets the theme background color variable name.
        /// </summary>
        /// <param name="variant">Color variant name.</param>
        /// <returns></returns>
        public static string BackgroundColor( string variant ) => $"--b-theme-background-{variant}";
        public static string BackgroundYiqColor( string variant ) => $"--b-theme-background-{variant}-yiq";

        public static string TextColor( string variant ) => $"--b-theme-text-{variant}";

        public static string ButtonBackground( string variant ) => $"--b-button-{variant}-background";
        public static string ButtonBorder( string variant ) => $"--b-button-{variant}-border";
        public static string ButtonHoverBackground( string variant ) => $"--b-button-{variant}-hover-background";
        public static string ButtonHoverBorder( string variant ) => $"--b-button-{variant}-hover-border";
        public static string ButtonActiveBackground( string variant ) => $"--b-button-{variant}-active-background";
        public static string ButtonActiveBorder( string variant ) => $"--b-button-{variant}-active-border";
        public static string ButtonYiqBackground( string variant ) => $"--b-button-{variant}-yiq-background";
        public static string ButtonYiqHoverBackground( string variant ) => $"--b-button-{variant}-yiq-hover-background";
        public static string ButtonYiqActiveBackground( string variant ) => $"--b-button-{variant}-yiq-active-background";
        public static string ButtonBoxShadow( string variant ) => $"--b-button-{variant}-box-shadow";

        public static string OutlineButtonColor( string variant ) => $"--b-outline-button-{variant}-color";
        public static string OutlineButtonYiqColor( string variant ) => $"--b-outline-button-{variant}-yiq-shadow";
        public static string OutlineButtonBoxShadowColor( string variant ) => $"--b-outline-button-{variant}-box-shadow";
        public static string OutlineButtonHoverColor( string variant ) => $"--b-outline-button-{variant}-hover-color";
        public static string OutlineButtonActiveColor( string variant ) => $"--b-outline-button-{variant}-active-color";

        public const string SidebarWidth = "--b-sidebar-width";
        public const string SidebarBackground = "--b-sidebar-background";
        public const string SidebarColor = "--b-sidebar-color";

        public const string SnackbarBackground = "--b-snackbar-background";
        public const string SnackbarTextColor = "--b-snackbar-text";
        public const string SnackbarButtonColor = "--b-snackbar-button";
        public const string SnackbarButtonHoverColor = "--b-snackbar-button-hover";

        public const string DividerColor = "--b-divider-color";
        public const string DividerThickness = "--b-divider-thickness";
        public const string DividerTextSize = "--b-divider-font-size";

        public const string TooltipBackgroundColorR = "--b-tooltip-background-color-r";
        public const string TooltipBackgroundColorG = "--b-tooltip-background-color-g";
        public const string TooltipBackgroundColorB = "--b-tooltip-background-color-b";
        public const string TooltipBackgroundOpacity = "--b-tooltip-background-opacity";
        public const string TooltipColor = "--b-tooltip-color";
        public const string TooltipFontSize = "--b-tooltip-font-size";
        public const string TooltipBorderRadius = "--b-tooltip-border-radius";
        public const string TooltipFadeTime = "--b-tooltip-fade-time";
        public const string TooltipMaxWidth = "--b-tooltip-maxwidth";
        public const string TooltipPadding = "--b-tooltip-padding";
        public const string TooltipZIndex = "--b-tooltip-z-index";

        public const string BreadcrumbColor = "--b-breadcrumb-color";
    }

    /// <summary>
    /// Default theme colors. 
    /// </summary>
    /// <remarks>
    /// Source: https://htmlcolorcodes.com/color-chart/material-design-color-chart/
    /// </remarks>
    public static class ThemeColors
    {
        public static ThemeColorRed Red { get; } = new ThemeColorRed();
        public static ThemeColorPink Pink { get; } = new ThemeColorPink();
        public static ThemeColorPurple Purple { get; } = new ThemeColorPurple();
        public static ThemeColorDeepPurple DeepPurple { get; } = new ThemeColorDeepPurple();
        public static ThemeColorIndigo Indigo { get; } = new ThemeColorIndigo();
        public static ThemeColorBlue Blue { get; } = new ThemeColorBlue();
        public static ThemeColorLightBlue LightBlue { get; } = new ThemeColorLightBlue();
        public static ThemeColorCyan Cyan { get; } = new ThemeColorCyan();
        public static ThemeColorTeal Teal { get; } = new ThemeColorTeal();
        public static ThemeColorGreen Green { get; } = new ThemeColorGreen();
        public static ThemeColorLightGreen LightGreen { get; } = new ThemeColorLightGreen();
        public static ThemeColorLime Lime { get; } = new ThemeColorLime();
        public static ThemeColorYellow Yellow { get; } = new ThemeColorYellow();
        public static ThemeColorAmber Amber { get; } = new ThemeColorAmber();
        public static ThemeColorOrange Orange { get; } = new ThemeColorOrange();
        public static ThemeColorDeepOrange DeepOrange { get; } = new ThemeColorDeepOrange();
        public static ThemeColorBrown Brown { get; } = new ThemeColorBrown();
        public static ThemeColorGray Gray { get; } = new ThemeColorGray();
        public static ThemeColorBlueGray BlueGray { get; } = new ThemeColorBlueGray();

        static ThemeColors()
        {
            Items = new Dictionary<string, ThemeColor>
            {
                {Red.Key, Red},
                {Pink.Key, Pink},
                {Purple.Key, Purple},
                {DeepPurple.Key, DeepPurple},
                {Indigo.Key, Indigo},
                {Blue.Key, Blue},
                {LightBlue.Key, LightBlue},
                {Cyan.Key, Cyan},
                {Teal.Key, Teal},
                {Green.Key, Green},
                {LightGreen.Key, LightGreen},
                {Lime.Key, Lime},
                {Yellow.Key, Yellow},
                {Amber.Key, Amber},
                {Orange.Key, Orange},
                {DeepOrange.Key, DeepOrange},
                {Brown.Key, Brown},
                {Gray.Key, Gray},
                {BlueGray.Key, BlueGray},
            };
        }

        public static Dictionary<string, ThemeColor> Items { get; set; }
    }

    public class ThemeColorRed : ThemeColor
    {
        public ThemeColorShade _50 { get; } = new ThemeColorShade( "50", "_50", "#ffebee" );
        public ThemeColorShade _100 { get; } = new ThemeColorShade( "100", "_100", "#ffcdd2" );
        public ThemeColorShade _200 { get; } = new ThemeColorShade( "200", "_200", "#ef9a9a" );
        public ThemeColorShade _300 { get; } = new ThemeColorShade( "300", "_300", "#e57373" );
        public ThemeColorShade _400 { get; } = new ThemeColorShade( "400", "_400", "#ef5350" );
        public ThemeColorShade _500 { get; } = new ThemeColorShade( "500", "_500", "#f44336" );
        public ThemeColorShade _600 { get; } = new ThemeColorShade( "600", "_600", "#e53935" );
        public ThemeColorShade _700 { get; } = new ThemeColorShade( "700", "_700", "#d32f2f" );
        public ThemeColorShade _800 { get; } = new ThemeColorShade( "800", "_800", "#c62828" );
        public ThemeColorShade _900 { get; } = new ThemeColorShade( "900", "_900", "#b71c1c" );
        public ThemeColorShade A100 { get; } = new ThemeColorShade( "A100", "A100", "#ff8a80" );
        public ThemeColorShade A200 { get; } = new ThemeColorShade( "A200", "A200", "#ff5252" );
        public ThemeColorShade A400 { get; } = new ThemeColorShade( "A400", "A400", "#ff1744" );
        public ThemeColorShade A700 { get; } = new ThemeColorShade( "A700", "A700", "#d50000" );

        public ThemeColorRed()
            : base( "red", "Red" )
        {
            Shades = new Dictionary<string, ThemeColorShade>
            {
                {_50.Key, _50},
                {_100.Key, _100},
                {_200.Key, _200},
                {_300.Key, _300},
                {_400.Key, _400},
                {_500.Key, _500},
                {_600.Key, _600},
                {_700.Key, _700},
                {_800.Key, _800},
                {_900.Key, _900},
                {A100.Key, A100},
                {A200.Key, A200},
                {A400.Key, A400},
                {A700.Key, A700},
            };
        }
    }

    public class ThemeColorPink : ThemeColor
    {
        public ThemeColorShade _50 { get; } = new ThemeColorShade( "50", "_50", "#fce4ec" );
        public ThemeColorShade _100 { get; } = new ThemeColorShade( "100", "_100", "#f8bbd0" );
        public ThemeColorShade _200 { get; } = new ThemeColorShade( "200", "_200", "#f48fb1" );
        public ThemeColorShade _300 { get; } = new ThemeColorShade( "300", "_300", "#f06292" );
        public ThemeColorShade _400 { get; } = new ThemeColorShade( "400", "_400", "#ec407a" );
        public ThemeColorShade _500 { get; } = new ThemeColorShade( "500", "_500", "#e91e63" );
        public ThemeColorShade _600 { get; } = new ThemeColorShade( "600", "_600", "#d81b60" );
        public ThemeColorShade _700 { get; } = new ThemeColorShade( "700", "_700", "#c2185b" );
        public ThemeColorShade _800 { get; } = new ThemeColorShade( "800", "_800", "#ad1457" );
        public ThemeColorShade _900 { get; } = new ThemeColorShade( "900", "_900", "#880e4f" );
        public ThemeColorShade A100 { get; } = new ThemeColorShade( "A100", "A100", "#ff80ab" );
        public ThemeColorShade A200 { get; } = new ThemeColorShade( "A200", "A200", "#ff4081" );
        public ThemeColorShade A400 { get; } = new ThemeColorShade( "A400", "A400", "#f50057" );
        public ThemeColorShade A700 { get; } = new ThemeColorShade( "A700", "A700", "#c51162" );

        public ThemeColorPink() : base( "pink", "Pink" )
        {
            Shades = new Dictionary<string, ThemeColorShade>()
            {
                {_50.Key, _50},
                {_100.Key, _100},
                {_200.Key, _200},
                {_300.Key, _300},
                {_400.Key, _400},
                {_500.Key, _500},
                {_600.Key, _600},
                {_700.Key, _700},
                {_800.Key, _800},
                {_900.Key, _900},
                {A100.Key, A100},
                {A200.Key, A200},
                {A400.Key, A400},
                {A700.Key, A700},
            };
        }
    }

    public class ThemeColorPurple : ThemeColor
    {
        public ThemeColorShade _50 { get; } = new ThemeColorShade( "50", "_50", "#f3e5f5" );
        public ThemeColorShade _100 { get; } = new ThemeColorShade( "100", "_100", "#e1bee7" );
        public ThemeColorShade _200 { get; } = new ThemeColorShade( "200", "_200", "#ce93d8" );
        public ThemeColorShade _300 { get; } = new ThemeColorShade( "300", "_300", "#ba68c8" );
        public ThemeColorShade _400 { get; } = new ThemeColorShade( "400", "_400", "#ab47bc" );
        public ThemeColorShade _500 { get; } = new ThemeColorShade( "500", "_500", "#9c27b0" );
        public ThemeColorShade _600 { get; } = new ThemeColorShade( "600", "_600", "#8e24aa" );
        public ThemeColorShade _700 { get; } = new ThemeColorShade( "700", "_700", "#7b1fa2" );
        public ThemeColorShade _800 { get; } = new ThemeColorShade( "800", "_800", "#6a1b9a" );
        public ThemeColorShade _900 { get; } = new ThemeColorShade( "900", "_900", "#4a148c" );
        public ThemeColorShade A100 { get; } = new ThemeColorShade( "A100", "A100", "#ea80fc" );
        public ThemeColorShade A200 { get; } = new ThemeColorShade( "A200", "A200", "#e040fb" );
        public ThemeColorShade A400 { get; } = new ThemeColorShade( "A400", "A400", "#d500f9" );
        public ThemeColorShade A700 { get; } = new ThemeColorShade( "A700", "A700", "#a0f" );

        public ThemeColorPurple() : base( "purple", "Purple" )
        {
            Shades = new Dictionary<string, ThemeColorShade>()
            {
                {_50.Key, _50},
                {_100.Key, _100},
                {_200.Key, _200},
                {_300.Key, _300},
                {_400.Key, _400},
                {_500.Key, _500},
                {_600.Key, _600},
                {_700.Key, _700},
                {_800.Key, _800},
                {_900.Key, _900},
                {A100.Key, A100},
                {A200.Key, A200},
                {A400.Key, A400},
                {A700.Key, A700},
            };
        }
    }

    public class ThemeColorDeepPurple : ThemeColor
    {
        public ThemeColorShade _50 { get; } = new ThemeColorShade( "50", "_50", "#ede7f6" );
        public ThemeColorShade _100 { get; } = new ThemeColorShade( "100", "_100", "#d1c4e9" );
        public ThemeColorShade _200 { get; } = new ThemeColorShade( "200", "_200", "#b39ddb" );
        public ThemeColorShade _300 { get; } = new ThemeColorShade( "300", "_300", "#9575cd" );
        public ThemeColorShade _400 { get; } = new ThemeColorShade( "400", "_400", "#7e57c2" );
        public ThemeColorShade _500 { get; } = new ThemeColorShade( "500", "_500", "#673ab7" );
        public ThemeColorShade _600 { get; } = new ThemeColorShade( "600", "_600", "#5e35b1" );
        public ThemeColorShade _700 { get; } = new ThemeColorShade( "700", "_700", "#512da8" );
        public ThemeColorShade _800 { get; } = new ThemeColorShade( "800", "_800", "#4527a0" );
        public ThemeColorShade _900 { get; } = new ThemeColorShade( "900", "_900", "#311b92" );
        public ThemeColorShade A100 { get; } = new ThemeColorShade( "A100", "A100", "#b388ff" );
        public ThemeColorShade A200 { get; } = new ThemeColorShade( "A200", "A200", "#7c4dff" );
        public ThemeColorShade A400 { get; } = new ThemeColorShade( "A400", "A400", "#651fff" );
        public ThemeColorShade A700 { get; } = new ThemeColorShade( "A700", "A700", "#6200ea" );

        public ThemeColorDeepPurple() : base( "deep-purple", "DeepPurple" )
        {
            Shades = new Dictionary<string, ThemeColorShade>()
            {
                {_50.Key, _50},
                {_100.Key, _100},
                {_200.Key, _200},
                {_300.Key, _300},
                {_400.Key, _400},
                {_500.Key, _500},
                {_600.Key, _600},
                {_700.Key, _700},
                {_800.Key, _800},
                {_900.Key, _900},
                {A100.Key, A100},
                {A200.Key, A200},
                {A400.Key, A400},
                {A700.Key, A700},
            };
        }
    }

    public class ThemeColorIndigo : ThemeColor
    {
        public ThemeColorShade _50 { get; } = new ThemeColorShade( "50", "_50", "#e8eaf6" );
        public ThemeColorShade _100 { get; } = new ThemeColorShade( "100", "_100", "#c5cae9" );
        public ThemeColorShade _200 { get; } = new ThemeColorShade( "200", "_200", "#9fa8da" );
        public ThemeColorShade _300 { get; } = new ThemeColorShade( "300", "_300", "#7986cb" );
        public ThemeColorShade _400 { get; } = new ThemeColorShade( "400", "_400", "#5c6bc0" );
        public ThemeColorShade _500 { get; } = new ThemeColorShade( "500", "_500", "#3f51b5" );
        public ThemeColorShade _600 { get; } = new ThemeColorShade( "600", "_600", "#3949ab" );
        public ThemeColorShade _700 { get; } = new ThemeColorShade( "700", "_700", "#303f9f" );
        public ThemeColorShade _800 { get; } = new ThemeColorShade( "800", "_800", "#283593" );
        public ThemeColorShade _900 { get; } = new ThemeColorShade( "900", "_900", "#1a237e" );
        public ThemeColorShade A100 { get; } = new ThemeColorShade( "A100", "A100", "#8c9eff" );
        public ThemeColorShade A200 { get; } = new ThemeColorShade( "A200", "A200", "#536dfe" );
        public ThemeColorShade A400 { get; } = new ThemeColorShade( "A400", "A400", "#3d5afe" );
        public ThemeColorShade A700 { get; } = new ThemeColorShade( "A700", "A700", "#304ffe" );

        public ThemeColorIndigo() : base( "indigo", "Indigo" )
        {
            Shades = new Dictionary<string, ThemeColorShade>()
            {
                {_50.Key, _50},
                {_100.Key, _100},
                {_200.Key, _200},
                {_300.Key, _300},
                {_400.Key, _400},
                {_500.Key, _500},
                {_600.Key, _600},
                {_700.Key, _700},
                {_800.Key, _800},
                {_900.Key, _900},
                {A100.Key, A100},
                {A200.Key, A200},
                {A400.Key, A400},
                {A700.Key, A700},
            };
        }
    }

    public class ThemeColorBlue : ThemeColor
    {
        public ThemeColorShade _50 { get; } = new ThemeColorShade( "50", "_50", "#e3f2fd" );
        public ThemeColorShade _100 { get; } = new ThemeColorShade( "100", "_100", "#bbdefb" );
        public ThemeColorShade _200 { get; } = new ThemeColorShade( "200", "_200", "#90caf9" );
        public ThemeColorShade _300 { get; } = new ThemeColorShade( "300", "_300", "#64b5f6" );
        public ThemeColorShade _400 { get; } = new ThemeColorShade( "400", "_400", "#42a5f5" );
        public ThemeColorShade _500 { get; } = new ThemeColorShade( "500", "_500", "#2196f3" );
        public ThemeColorShade _600 { get; } = new ThemeColorShade( "600", "_600", "#1e88e5" );
        public ThemeColorShade _700 { get; } = new ThemeColorShade( "700", "_700", "#1976d2" );
        public ThemeColorShade _800 { get; } = new ThemeColorShade( "800", "_800", "#1565c0" );
        public ThemeColorShade _900 { get; } = new ThemeColorShade( "900", "_900", "#0d47a1" );
        public ThemeColorShade A100 { get; } = new ThemeColorShade( "A100", "A100", "#82b1ff" );
        public ThemeColorShade A200 { get; } = new ThemeColorShade( "A200", "A200", "#448aff" );
        public ThemeColorShade A400 { get; } = new ThemeColorShade( "A400", "A400", "#2979ff" );
        public ThemeColorShade A700 { get; } = new ThemeColorShade( "A700", "A700", "#2962ff" );

        public ThemeColorBlue() : base( "blue", "Blue" )
        {
            Shades = new Dictionary<string, ThemeColorShade>()
            {
                {_50.Key, _50},
                {_100.Key, _100},
                {_200.Key, _200},
                {_300.Key, _300},
                {_400.Key, _400},
                {_500.Key, _500},
                {_600.Key, _600},
                {_700.Key, _700},
                {_800.Key, _800},
                {_900.Key, _900},
                {A100.Key, A100},
                {A200.Key, A200},
                {A400.Key, A400},
                {A700.Key, A700},
            };
        }
    }

    public class ThemeColorLightBlue : ThemeColor
    {
        public ThemeColorShade _50 { get; } = new ThemeColorShade( "50", "_50", "#e1f5fe" );
        public ThemeColorShade _100 { get; } = new ThemeColorShade( "100", "_100", "#b3e5fc" );
        public ThemeColorShade _200 { get; } = new ThemeColorShade( "200", "_200", "#81d4fa" );
        public ThemeColorShade _300 { get; } = new ThemeColorShade( "300", "_300", "#4fc3f7" );
        public ThemeColorShade _400 { get; } = new ThemeColorShade( "400", "_400", "#29b6f6" );
        public ThemeColorShade _500 { get; } = new ThemeColorShade( "500", "_500", "#03a9f4" );
        public ThemeColorShade _600 { get; } = new ThemeColorShade( "600", "_600", "#039be5" );
        public ThemeColorShade _700 { get; } = new ThemeColorShade( "700", "_700", "#0288d1" );
        public ThemeColorShade _800 { get; } = new ThemeColorShade( "800", "_800", "#0277bd" );
        public ThemeColorShade _900 { get; } = new ThemeColorShade( "900", "_900", "#01579b" );
        public ThemeColorShade A100 { get; } = new ThemeColorShade( "A100", "A100", "#80d8ff" );
        public ThemeColorShade A200 { get; } = new ThemeColorShade( "A200", "A200", "#40c4ff" );
        public ThemeColorShade A400 { get; } = new ThemeColorShade( "A400", "A400", "#00b0ff" );
        public ThemeColorShade A700 { get; } = new ThemeColorShade( "A700", "A700", "#0091ea" );

        public ThemeColorLightBlue() : base( "light-blue", "LightBlue" )
        {
            Shades = new Dictionary<string, ThemeColorShade>()
            {
                {_50.Key, _50},
                {_100.Key, _100},
                {_200.Key, _200},
                {_300.Key, _300},
                {_400.Key, _400},
                {_500.Key, _500},
                {_600.Key, _600},
                {_700.Key, _700},
                {_800.Key, _800},
                {_900.Key, _900},
                {A100.Key, A100},
                {A200.Key, A200},
                {A400.Key, A400},
                {A700.Key, A700},
            };
        }
    }

    public class ThemeColorCyan : ThemeColor
    {
        public ThemeColorShade _50 { get; } = new ThemeColorShade( "50", "_50", "#e0f7fa" );
        public ThemeColorShade _100 { get; } = new ThemeColorShade( "100", "_100", "#b2ebf2" );
        public ThemeColorShade _200 { get; } = new ThemeColorShade( "200", "_200", "#80deea" );
        public ThemeColorShade _300 { get; } = new ThemeColorShade( "300", "_300", "#4dd0e1" );
        public ThemeColorShade _400 { get; } = new ThemeColorShade( "400", "_400", "#26c6da" );
        public ThemeColorShade _500 { get; } = new ThemeColorShade( "500", "_500", "#00bcd4" );
        public ThemeColorShade _600 { get; } = new ThemeColorShade( "600", "_600", "#00acc1" );
        public ThemeColorShade _700 { get; } = new ThemeColorShade( "700", "_700", "#0097a7" );
        public ThemeColorShade _800 { get; } = new ThemeColorShade( "800", "_800", "#00838f" );
        public ThemeColorShade _900 { get; } = new ThemeColorShade( "900", "_900", "#006064" );
        public ThemeColorShade A100 { get; } = new ThemeColorShade( "A100", "A100", "#84ffff" );
        public ThemeColorShade A200 { get; } = new ThemeColorShade( "A200", "A200", "#18ffff" );
        public ThemeColorShade A400 { get; } = new ThemeColorShade( "A400", "A400", "#00e5ff" );
        public ThemeColorShade A700 { get; } = new ThemeColorShade( "A700", "A700", "#00b8d4" );

        public ThemeColorCyan() : base( "cyan", "Cyan" )
        {
            Shades = new Dictionary<string, ThemeColorShade>()
            {
                {_50.Key, _50},
                {_100.Key, _100},
                {_200.Key, _200},
                {_300.Key, _300},
                {_400.Key, _400},
                {_500.Key, _500},
                {_600.Key, _600},
                {_700.Key, _700},
                {_800.Key, _800},
                {_900.Key, _900},
                {A100.Key, A100},
                {A200.Key, A200},
                {A400.Key, A400},
                {A700.Key, A700},
            };
        }
    }

    public class ThemeColorTeal : ThemeColor
    {
        public ThemeColorShade _50 { get; } = new ThemeColorShade( "50", "_50", "#e0f2f1" );
        public ThemeColorShade _100 { get; } = new ThemeColorShade( "100", "_100", "#b2dfdb" );
        public ThemeColorShade _200 { get; } = new ThemeColorShade( "200", "_200", "#80cbc4" );
        public ThemeColorShade _300 { get; } = new ThemeColorShade( "300", "_300", "#4db6ac" );
        public ThemeColorShade _400 { get; } = new ThemeColorShade( "400", "_400", "#26a69a" );
        public ThemeColorShade _500 { get; } = new ThemeColorShade( "500", "_500", "#009688" );
        public ThemeColorShade _600 { get; } = new ThemeColorShade( "600", "_600", "#00897b" );
        public ThemeColorShade _700 { get; } = new ThemeColorShade( "700", "_700", "#00796b" );
        public ThemeColorShade _800 { get; } = new ThemeColorShade( "800", "_800", "#00695c" );
        public ThemeColorShade _900 { get; } = new ThemeColorShade( "900", "_900", "#004d40" );
        public ThemeColorShade A100 { get; } = new ThemeColorShade( "A100", "A100", "#a7ffeb" );
        public ThemeColorShade A200 { get; } = new ThemeColorShade( "A200", "A200", "#64ffda" );
        public ThemeColorShade A400 { get; } = new ThemeColorShade( "A400", "A400", "#1de9b6" );
        public ThemeColorShade A700 { get; } = new ThemeColorShade( "A700", "A700", "#00bfa5" );

        public ThemeColorTeal() : base( "teal", "Teal" )
        {
            Shades = new Dictionary<string, ThemeColorShade>()
            {
                {_50.Key, _50},
                {_100.Key, _100},
                {_200.Key, _200},
                {_300.Key, _300},
                {_400.Key, _400},
                {_500.Key, _500},
                {_600.Key, _600},
                {_700.Key, _700},
                {_800.Key, _800},
                {_900.Key, _900},
                {A100.Key, A100},
                {A200.Key, A200},
                {A400.Key, A400},
                {A700.Key, A700},
            };
        }
    }

    public class ThemeColorGreen : ThemeColor
    {
        public ThemeColorShade _50 { get; } = new ThemeColorShade( "50", "_50", "#e8f5e9" );
        public ThemeColorShade _100 { get; } = new ThemeColorShade( "100", "_100", "#c8e6c9" );
        public ThemeColorShade _200 { get; } = new ThemeColorShade( "200", "_200", "#a5d6a7" );
        public ThemeColorShade _300 { get; } = new ThemeColorShade( "300", "_300", "#81c784" );
        public ThemeColorShade _400 { get; } = new ThemeColorShade( "400", "_400", "#66bb6a" );
        public ThemeColorShade _500 { get; } = new ThemeColorShade( "500", "_500", "#4caf50" );
        public ThemeColorShade _600 { get; } = new ThemeColorShade( "600", "_600", "#43a047" );
        public ThemeColorShade _700 { get; } = new ThemeColorShade( "700", "_700", "#388e3c" );
        public ThemeColorShade _800 { get; } = new ThemeColorShade( "800", "_800", "#2e7d32" );
        public ThemeColorShade _900 { get; } = new ThemeColorShade( "900", "_900", "#1b5e20" );
        public ThemeColorShade A100 { get; } = new ThemeColorShade( "A100", "A100", "#b9f6ca" );
        public ThemeColorShade A200 { get; } = new ThemeColorShade( "A200", "A200", "#69f0ae" );
        public ThemeColorShade A400 { get; } = new ThemeColorShade( "A400", "A400", "#00e676" );
        public ThemeColorShade A700 { get; } = new ThemeColorShade( "A700", "A700", "#00c853" );

        public ThemeColorGreen() : base( "green", "Green" )
        {
            Shades = new Dictionary<string, ThemeColorShade>()
            {
                {_50.Key, _50},
                {_100.Key, _100},
                {_200.Key, _200},
                {_300.Key, _300},
                {_400.Key, _400},
                {_500.Key, _500},
                {_600.Key, _600},
                {_700.Key, _700},
                {_800.Key, _800},
                {_900.Key, _900},
                {A100.Key, A100},
                {A200.Key, A200},
                {A400.Key, A400},
                {A700.Key, A700},
            };
        }
    }

    public class ThemeColorLightGreen : ThemeColor
    {
        public ThemeColorShade _50 { get; } = new ThemeColorShade( "50", "_50", "#f1f8e9" );
        public ThemeColorShade _100 { get; } = new ThemeColorShade( "100", "_100", "#dcedc8" );
        public ThemeColorShade _200 { get; } = new ThemeColorShade( "200", "_200", "#c5e1a5" );
        public ThemeColorShade _300 { get; } = new ThemeColorShade( "300", "_300", "#aed581" );
        public ThemeColorShade _400 { get; } = new ThemeColorShade( "400", "_400", "#9ccc65" );
        public ThemeColorShade _500 { get; } = new ThemeColorShade( "500", "_500", "#8bc34a" );
        public ThemeColorShade _600 { get; } = new ThemeColorShade( "600", "_600", "#7cb342" );
        public ThemeColorShade _700 { get; } = new ThemeColorShade( "700", "_700", "#689f38" );
        public ThemeColorShade _800 { get; } = new ThemeColorShade( "800", "_800", "#558b2f" );
        public ThemeColorShade _900 { get; } = new ThemeColorShade( "900", "_900", "#33691e" );
        public ThemeColorShade A100 { get; } = new ThemeColorShade( "A100", "A100", "#ccff90" );
        public ThemeColorShade A200 { get; } = new ThemeColorShade( "A200", "A200", "#b2ff59" );
        public ThemeColorShade A400 { get; } = new ThemeColorShade( "A400", "A400", "#76ff03" );
        public ThemeColorShade A700 { get; } = new ThemeColorShade( "A700", "A700", "#64dd17" );

        public ThemeColorLightGreen() : base( "light-green", "LightGreen" )
        {
            Shades = new Dictionary<string, ThemeColorShade>()
            {
                {_50.Key, _50},
                {_100.Key, _100},
                {_200.Key, _200},
                {_300.Key, _300},
                {_400.Key, _400},
                {_500.Key, _500},
                {_600.Key, _600},
                {_700.Key, _700},
                {_800.Key, _800},
                {_900.Key, _900},
                {A100.Key, A100},
                {A200.Key, A200},
                {A400.Key, A400},
                {A700.Key, A700},
            };
        }
    }

    public class ThemeColorLime : ThemeColor
    {
        public ThemeColorShade _50 { get; } = new ThemeColorShade( "50", "_50", "#f9fbe7" );
        public ThemeColorShade _100 { get; } = new ThemeColorShade( "100", "_100", "#f0f4c3" );
        public ThemeColorShade _200 { get; } = new ThemeColorShade( "200", "_200", "#e6ee9c" );
        public ThemeColorShade _300 { get; } = new ThemeColorShade( "300", "_300", "#dce775" );
        public ThemeColorShade _400 { get; } = new ThemeColorShade( "400", "_400", "#d4e157" );
        public ThemeColorShade _500 { get; } = new ThemeColorShade( "500", "_500", "#cddc39" );
        public ThemeColorShade _600 { get; } = new ThemeColorShade( "600", "_600", "#c0ca33" );
        public ThemeColorShade _700 { get; } = new ThemeColorShade( "700", "_700", "#afb42b" );
        public ThemeColorShade _800 { get; } = new ThemeColorShade( "800", "_800", "#9e9d24" );
        public ThemeColorShade _900 { get; } = new ThemeColorShade( "900", "_900", "#827717" );
        public ThemeColorShade A100 { get; } = new ThemeColorShade( "A100", "A100", "#f4ff81" );
        public ThemeColorShade A200 { get; } = new ThemeColorShade( "A200", "A200", "#eeff41" );
        public ThemeColorShade A400 { get; } = new ThemeColorShade( "A400", "A400", "#c6ff00" );
        public ThemeColorShade A700 { get; } = new ThemeColorShade( "A700", "A700", "#aeea00" );

        public ThemeColorLime() : base( "lime", "Lime" )
        {
            Shades = new Dictionary<string, ThemeColorShade>()
            {
                {_50.Key, _50},
                {_100.Key, _100},
                {_200.Key, _200},
                {_300.Key, _300},
                {_400.Key, _400},
                {_500.Key, _500},
                {_600.Key, _600},
                {_700.Key, _700},
                {_800.Key, _800},
                {_900.Key, _900},
                {A100.Key, A100},
                {A200.Key, A200},
                {A400.Key, A400},
                {A700.Key, A700},
            };
        }
    }

    public class ThemeColorYellow : ThemeColor
    {
        public ThemeColorShade _50 { get; } = new ThemeColorShade( "50", "_50", "#fffde7" );
        public ThemeColorShade _100 { get; } = new ThemeColorShade( "100", "_100", "#fff9c4" );
        public ThemeColorShade _200 { get; } = new ThemeColorShade( "200", "_200", "#fff59d" );
        public ThemeColorShade _300 { get; } = new ThemeColorShade( "300", "_300", "#fff176" );
        public ThemeColorShade _400 { get; } = new ThemeColorShade( "400", "_400", "#ffee58" );
        public ThemeColorShade _500 { get; } = new ThemeColorShade( "500", "_500", "#ffeb3b" );
        public ThemeColorShade _600 { get; } = new ThemeColorShade( "600", "_600", "#fdd835" );
        public ThemeColorShade _700 { get; } = new ThemeColorShade( "700", "_700", "#fbc02d" );
        public ThemeColorShade _800 { get; } = new ThemeColorShade( "800", "_800", "#f9a825" );
        public ThemeColorShade _900 { get; } = new ThemeColorShade( "900", "_900", "#f57f17" );
        public ThemeColorShade A100 { get; } = new ThemeColorShade( "A100", "A100", "#ffff8d" );
        public ThemeColorShade A200 { get; } = new ThemeColorShade( "A200", "A200", "#ff0" );
        public ThemeColorShade A400 { get; } = new ThemeColorShade( "A400", "A400", "#ffea00" );
        public ThemeColorShade A700 { get; } = new ThemeColorShade( "A700", "A700", "#ffd600" );

        public ThemeColorYellow() : base( "yellow", "Yellow" )
        {
            Shades = new Dictionary<string, ThemeColorShade>()
            {
                {_50.Key, _50},
                {_100.Key, _100},
                {_200.Key, _200},
                {_300.Key, _300},
                {_400.Key, _400},
                {_500.Key, _500},
                {_600.Key, _600},
                {_700.Key, _700},
                {_800.Key, _800},
                {_900.Key, _900},
                {A100.Key, A100},
                {A200.Key, A200},
                {A400.Key, A400},
                {A700.Key, A700},
            };
        }
    }

    public class ThemeColorAmber : ThemeColor
    {
        public ThemeColorShade _50 { get; } = new ThemeColorShade( "50", "_50", "#fff8e1" );
        public ThemeColorShade _100 { get; } = new ThemeColorShade( "100", "_100", "#ffecb3" );
        public ThemeColorShade _200 { get; } = new ThemeColorShade( "200", "_200", "#ffe082" );
        public ThemeColorShade _300 { get; } = new ThemeColorShade( "300", "_300", "#ffd54f" );
        public ThemeColorShade _400 { get; } = new ThemeColorShade( "400", "_400", "#ffca28" );
        public ThemeColorShade _500 { get; } = new ThemeColorShade( "500", "_500", "#ffc107" );
        public ThemeColorShade _600 { get; } = new ThemeColorShade( "600", "_600", "#ffb300" );
        public ThemeColorShade _700 { get; } = new ThemeColorShade( "700", "_700", "#ffa000" );
        public ThemeColorShade _800 { get; } = new ThemeColorShade( "800", "_800", "#ff8f00" );
        public ThemeColorShade _900 { get; } = new ThemeColorShade( "900", "_900", "#ff6f00" );
        public ThemeColorShade A100 { get; } = new ThemeColorShade( "A100", "A100", "#ffe57f" );
        public ThemeColorShade A200 { get; } = new ThemeColorShade( "A200", "A200", "#ffd740" );
        public ThemeColorShade A400 { get; } = new ThemeColorShade( "A400", "A400", "#ffc400" );
        public ThemeColorShade A700 { get; } = new ThemeColorShade( "A700", "A700", "#ffab00" );

        public ThemeColorAmber() : base( "amber", "Amber" )
        {
            Shades = new Dictionary<string, ThemeColorShade>()
            {
                {_50.Key, _50},
                {_100.Key, _100},
                {_200.Key, _200},
                {_300.Key, _300},
                {_400.Key, _400},
                {_500.Key, _500},
                {_600.Key, _600},
                {_700.Key, _700},
                {_800.Key, _800},
                {_900.Key, _900},
                {A100.Key, A100},
                {A200.Key, A200},
                {A400.Key, A400},
                {A700.Key, A700},
            };
        }
    }

    public class ThemeColorOrange : ThemeColor
    {
        public ThemeColorShade _50 { get; } = new ThemeColorShade( "50", "_50", "#fff3e0" );
        public ThemeColorShade _100 { get; } = new ThemeColorShade( "100", "_100", "#ffe0b2" );
        public ThemeColorShade _200 { get; } = new ThemeColorShade( "200", "_200", "#ffcc80" );
        public ThemeColorShade _300 { get; } = new ThemeColorShade( "300", "_300", "#ffb74d" );
        public ThemeColorShade _400 { get; } = new ThemeColorShade( "400", "_400", "#ffa726" );
        public ThemeColorShade _500 { get; } = new ThemeColorShade( "500", "_500", "#ff9800" );
        public ThemeColorShade _600 { get; } = new ThemeColorShade( "600", "_600", "#fb8c00" );
        public ThemeColorShade _700 { get; } = new ThemeColorShade( "700", "_700", "#f57c00" );
        public ThemeColorShade _800 { get; } = new ThemeColorShade( "800", "_800", "#ef6c00" );
        public ThemeColorShade _900 { get; } = new ThemeColorShade( "900", "_900", "#e65100" );
        public ThemeColorShade A100 { get; } = new ThemeColorShade( "A100", "A100", "#ffd180" );
        public ThemeColorShade A200 { get; } = new ThemeColorShade( "A200", "A200", "#ffab40" );
        public ThemeColorShade A400 { get; } = new ThemeColorShade( "A400", "A400", "#ff9100" );
        public ThemeColorShade A700 { get; } = new ThemeColorShade( "A700", "A700", "#ff6d00" );

        public ThemeColorOrange() : base( "orange", "Orange" )
        {
            Shades = new Dictionary<string, ThemeColorShade>()
            {
                {_50.Key, _50},
                {_100.Key, _100},
                {_200.Key, _200},
                {_300.Key, _300},
                {_400.Key, _400},
                {_500.Key, _500},
                {_600.Key, _600},
                {_700.Key, _700},
                {_800.Key, _800},
                {_900.Key, _900},
                {A100.Key, A100},
                {A200.Key, A200},
                {A400.Key, A400},
                {A700.Key, A700},
            };
        }
    }

    public class ThemeColorDeepOrange : ThemeColor
    {
        public ThemeColorShade _50 { get; } = new ThemeColorShade( "50", "_50", "#fbe9e7" );
        public ThemeColorShade _100 { get; } = new ThemeColorShade( "100", "_100", "#ffccbc" );
        public ThemeColorShade _200 { get; } = new ThemeColorShade( "200", "_200", "#ffab91" );
        public ThemeColorShade _300 { get; } = new ThemeColorShade( "300", "_300", "#ff8a65" );
        public ThemeColorShade _400 { get; } = new ThemeColorShade( "400", "_400", "#ff7043" );
        public ThemeColorShade _500 { get; } = new ThemeColorShade( "500", "_500", "#ff5722" );
        public ThemeColorShade _600 { get; } = new ThemeColorShade( "600", "_600", "#f4511e" );
        public ThemeColorShade _700 { get; } = new ThemeColorShade( "700", "_700", "#e64a19" );
        public ThemeColorShade _800 { get; } = new ThemeColorShade( "800", "_800", "#d84315" );
        public ThemeColorShade _900 { get; } = new ThemeColorShade( "900", "_900", "#bf360c" );
        public ThemeColorShade A100 { get; } = new ThemeColorShade( "A100", "A100", "#ff9e80" );
        public ThemeColorShade A200 { get; } = new ThemeColorShade( "A200", "A200", "#ff6e40" );
        public ThemeColorShade A400 { get; } = new ThemeColorShade( "A400", "A400", "#ff3d00" );
        public ThemeColorShade A700 { get; } = new ThemeColorShade( "A700", "A700", "#dd2c00" );

        public ThemeColorDeepOrange() : base( "deep-orange", "DeepOrange" )
        {
            Shades = new Dictionary<string, ThemeColorShade>()
            {
                {_50.Key, _50},
                {_100.Key, _100},
                {_200.Key, _200},
                {_300.Key, _300},
                {_400.Key, _400},
                {_500.Key, _500},
                {_600.Key, _600},
                {_700.Key, _700},
                {_800.Key, _800},
                {_900.Key, _900},
                {A100.Key, A100},
                {A200.Key, A200},
                {A400.Key, A400},
                {A700.Key, A700},
            };
        }
    }

    public class ThemeColorBrown : ThemeColor
    {
        public ThemeColorShade _50 { get; } = new ThemeColorShade( "50", "_50", "#efebe9" );
        public ThemeColorShade _100 { get; } = new ThemeColorShade( "100", "_100", "#d7ccc8" );
        public ThemeColorShade _200 { get; } = new ThemeColorShade( "200", "_200", "#bcaaa4" );
        public ThemeColorShade _300 { get; } = new ThemeColorShade( "300", "_300", "#a1887f" );
        public ThemeColorShade _400 { get; } = new ThemeColorShade( "400", "_400", "#8d6e63" );
        public ThemeColorShade _500 { get; } = new ThemeColorShade( "500", "_500", "#795548" );
        public ThemeColorShade _600 { get; } = new ThemeColorShade( "600", "_600", "#6d4c41" );
        public ThemeColorShade _700 { get; } = new ThemeColorShade( "700", "_700", "#5d4037" );
        public ThemeColorShade _800 { get; } = new ThemeColorShade( "800", "_800", "#4e342e" );
        public ThemeColorShade _900 { get; } = new ThemeColorShade( "900", "_900", "#3e2723" );

        public ThemeColorBrown() : base( "brown", "Brown" )
        {
            Shades = new Dictionary<string, ThemeColorShade>()
            {
                {_50.Key, _50},
                {_100.Key, _100},
                {_200.Key, _200},
                {_300.Key, _300},
                {_400.Key, _400},
                {_500.Key, _500},
                {_600.Key, _600},
                {_700.Key, _700},
                {_800.Key, _800},
                {_900.Key, _900},
            };
        }
    }

    public class ThemeColorGray : ThemeColor
    {
        public ThemeColorShade _50 { get; } = new ThemeColorShade( "50", "_50", "#fafafa" );
        public ThemeColorShade _100 { get; } = new ThemeColorShade( "100", "_100", "#f5f5f5" );
        public ThemeColorShade _200 { get; } = new ThemeColorShade( "200", "_200", "#eee" );
        public ThemeColorShade _300 { get; } = new ThemeColorShade( "300", "_300", "#e0e0e0" );
        public ThemeColorShade _400 { get; } = new ThemeColorShade( "400", "_400", "#bdbdbd" );
        public ThemeColorShade _500 { get; } = new ThemeColorShade( "500", "_500", "#9e9e9e" );
        public ThemeColorShade _600 { get; } = new ThemeColorShade( "600", "_600", "#757575" );
        public ThemeColorShade _700 { get; } = new ThemeColorShade( "700", "_700", "#616161" );
        public ThemeColorShade _800 { get; } = new ThemeColorShade( "800", "_800", "#424242" );
        public ThemeColorShade _900 { get; } = new ThemeColorShade( "900", "_900", "#212121" );

        public ThemeColorGray() : base( "gray", "Gray" )
        {
            Shades = new Dictionary<string, ThemeColorShade>()
            {
                {_50.Key, _50},
                {_100.Key, _100},
                {_200.Key, _200},
                {_300.Key, _300},
                {_400.Key, _400},
                {_500.Key, _500},
                {_600.Key, _600},
                {_700.Key, _700},
                {_800.Key, _800},
                {_900.Key, _900},
            };
        }
    }

    public class ThemeColorBlueGray : ThemeColor
    {
        public ThemeColorShade _50 { get; } = new ThemeColorShade( "50", "_50", "#eceff1" );
        public ThemeColorShade _100 { get; } = new ThemeColorShade( "100", "_100", "#cfd8dc" );
        public ThemeColorShade _200 { get; } = new ThemeColorShade( "200", "_200", "#b0bec5" );
        public ThemeColorShade _300 { get; } = new ThemeColorShade( "300", "_300", "#90a4ae" );
        public ThemeColorShade _400 { get; } = new ThemeColorShade( "400", "_400", "#78909c" );
        public ThemeColorShade _500 { get; } = new ThemeColorShade( "500", "_500", "#607d8b" );
        public ThemeColorShade _600 { get; } = new ThemeColorShade( "600", "_600", "#546e7a" );
        public ThemeColorShade _700 { get; } = new ThemeColorShade( "700", "_700", "#455a64" );
        public ThemeColorShade _800 { get; } = new ThemeColorShade( "800", "_800", "#37474f" );
        public ThemeColorShade _900 { get; } = new ThemeColorShade( "900", "_900", "#263238" );

        public ThemeColorBlueGray() : base( "blue-gray", "BlueGray" )
        {
            Shades = new Dictionary<string, ThemeColorShade>()
            {
                {_50.Key, _50},
                {_100.Key, _100},
                {_200.Key, _200},
                {_300.Key, _300},
                {_400.Key, _400},
                {_500.Key, _500},
                {_600.Key, _600},
                {_700.Key, _700},
                {_800.Key, _800},
                {_900.Key, _900},
            };
        }
    }

    public class ThemeColor
    {
        #region Constructors

        public ThemeColor( string key, string name )
        {
            Key = key;
            Name = name;
        }

        #endregion

        #region Methods

        #endregion

        #region Properties

        public string Key { get; }

        public string Name { get; }

        public Dictionary<string, ThemeColorShade> Shades { get; protected set; }

        #endregion
    }

    public class ThemeColorShade
    {
        #region Constructors

        public ThemeColorShade( string key, string name, string value )
        {
            Key = key;
            Name = name;
            Value = value;
        }

        #endregion

        #region Methods

        #endregion

        #region Properties

        public string Key { get; }

        public string Name { get; }

        public string Value { get; }

        #endregion
    }
}
