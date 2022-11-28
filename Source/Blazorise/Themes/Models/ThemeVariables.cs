namespace Blazorise;

/// <summary>
/// Defines all the built-in CSS variable names.
/// </summary>
public static class ThemeVariables
{
    /// <summary>
    /// Gets the theme white color variable name.
    /// </summary>
    public const string White = "--b-theme-white";

    /// <summary>
    /// Gets the theme black color variable name.
    /// </summary>
    public const string Black = "--b-theme-black";

    /// <summary>
    /// Gets the body background color variable name.
    /// </summary>
    public const string BodyBackgroundColor = "--b-theme-body-background-color";

    /// <summary>
    /// Gets the body text color variable name.
    /// </summary>
    public const string BodyTextColor = "--b-theme-body-text-color";

    /// <summary>
    /// Gets the theme color variable name.
    /// </summary>
    /// <param name="variant">Color variant name.</param>
    /// <returns>Color variant name.</returns>
    public static string Color( string variant ) => $"--b-theme-{variant}";

    /// <summary>
    /// Gets the theme background color variable name.
    /// </summary>
    /// <param name="variant">Color variant name.</param>
    /// <returns>Background variant name.</returns>
    public static string BackgroundColor( string variant ) => $"--b-theme-background-{variant}";

    /// <summary>
    /// Gets the theme background contrast variable name.
    /// </summary>
    /// <param name="variant">Color variant name.</param>
    /// <returns>Contrast variant name.</returns>
    public static string BackgroundYiqColor( string variant ) => $"--b-theme-background-{variant}-yiq";

    /// <summary>
    /// Gets the theme text color variable name.
    /// </summary>
    /// <param name="variant">Text color variant name.</param>
    /// <returns>Text color variant name.</returns>
    public static string TextColor( string variant ) => $"--b-theme-text-{variant}";

    /// <summary>
    /// Gets the breakpoint variable name.
    /// </summary>
    /// <param name="name">Breakpoint name.</param>
    /// <returns>Breakpoint name.</returns>
    public static string Breakpoint( string name ) => $"--b-theme-breakpoint-{name}";

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public const string BorderRadius = "--b-border-radius";
    public const string BorderRadiusLarge = "--b-border-radius-lg";
    public const string BorderRadiusSmall = "--b-border-radius-sm";

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

    public const string LayoutHeaderHeight = "--b-layout-header-height";
    public const string LayoutFooterHeight = "--b-layout-footer-height";

    public const string VerticalBarWidth = "--b-vertical-bar-width";
    public const string VerticalBarSmallWidth = "--b-vertical-bar-small-width";
    public const string VerticalBarBrandHeight = "--b-vertical-bar-brand-height";
    public const string VerticalPopoutMenuWidth = "--b-vertical-bar-popout-menu-width";
    public const string HorizontalBarHeight = "--b-bar-horizontal-height";

    public const string BarDarkBackground = "--b-bar-dark-background";
    public const string BarDarkColor = "--b-bar-dark-color";
    public const string BarItemDarkActiveBackground = "--b-bar-item-dark-active-background";
    public const string BarItemDarkActiveColor = "--b-bar-item-dark-active-color";
    public const string BarItemDarkHoverBackground = "--b-bar-item-dark-hover-background";
    public const string BarItemDarkHoverColor = "--b-bar-item-dark-hover-color";
    public const string BarDropdownDarkBackground = "--b-bar-dropdown-dark-background";
    public const string BarBrandDarkBackground = "--b-bar-brand-dark-background";

    public const string BarLightBackground = "--b-bar-light-background";
    public const string BarLightColor = "--b-bar-light-color";
    public const string BarItemLightActiveBackground = "--b-bar-item-light-active-background";
    public const string BarItemLightActiveColor = "--b-bar-item-light-active-color";
    public const string BarItemLightHoverBackground = "--b-bar-item-light-hover-background";
    public const string BarItemLightHoverColor = "--b-bar-item-light-hover-color";
    public const string BarDropdownLightBackground = "--b-bar-dropdown-light-background";
    public const string BarBrandLightBackground = "--b-bar-brand-light-background";

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


    public static string VariantStepsItemIcon( string variant ) => $"--b-steps-item-{variant}-icon-color";
    public static string VariantStepsItemIconYiq( string variant ) => $"--b-steps-item-{variant}-icon-yiq";
    public static string VariantStepsItemText( string variant ) => $"--b-steps-item-{variant}-text-color";

    public static string StepsItemIcon => "--b-steps-item-icon-color";
    public static string StepsItemIconCompleted => "--b-steps-item-icon-completed";
    public static string StepsItemIconCompletedYiq => "--b-steps-item-icon-completed-yiq";
    public static string StepsItemIconActive => "--b-steps-item-icon-active";
    public static string StepsItemIconActiveYiq => "--b-steps-item-icon-active-yiq";
    public static string StepsItemText => "--b-steps-item-text-color";
    public static string StepsItemTextCompleted => "--b-steps-item-text-completed";
    public static string StepsItemTextActive => "--b-steps-item-text-active";

    public static string SpinKitSize => "--b-spinkit-size";
    public static string SpinKitColor => "--b-spinkit-color";

    public static string VariantPageProgressIndicator( string variant ) => $"--b-page-progress-indicator-{variant}";

    public static string VariantRatingColor( string variant ) => $"--b-rating-{variant}-color";

    public const string BreadcrumbColor = "--b-breadcrumb-color";
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}