#region Using directives
namespace Blazorise;
#endregion

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
    /// Gets the body body font family variable name.
    /// </summary>
    public const string BodyFontFamily = "--b-theme-body-font-family";

    /// <summary>
    /// Gets the body body font style variable name.
    /// </summary>
    public const string BodyFontStyle = "--b-theme-body-font-style";

    /// <summary>
    /// Gets the body body font size variable name.
    /// </summary>
    public const string BodyFontSize = "--b-theme-body-font-size";

    /// <summary>
    /// Gets the body body font size variable name.
    /// </summary>
    public const string BodyFontWeight = "--b-theme-body-font-weight";

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
    /// Gets the theme background subtle color variable name.
    /// </summary>
    /// <param name="variant">Color variant name.</param>
    /// <returns>Subtle variant name.</returns>
    public static string BackgroundSubtleColor( string variant ) => $"--b-theme-background-{variant}-subtle";

    /// <summary>
    /// Gets the theme text color variable name.
    /// </summary>
    /// <param name="variant">Text color variant name.</param>
    /// <returns>Text color variant name.</returns>
    public static string TextColor( string variant ) => $"--b-theme-text-{variant}";

    /// <summary>
    /// Gets the theme text emphasis color variable name.
    /// </summary>
    /// <param name="variant">Text color variant name.</param>
    /// <returns>Text color emphasis name.</returns>
    public static string TextEmphasisColor( string variant ) => $"--b-theme-text-{variant}-emphasis";

    /// <summary>
    /// Gets the theme border color variable name.
    /// </summary>
    /// <param name="variant">Color variant name.</param>
    /// <returns>Border variant name.</returns>
    public static string BorderColor( string variant ) => $"--b-theme-border-{variant}";

    /// <summary>
    /// Gets the theme border subtle color variable name.
    /// </summary>
    /// <param name="variant">Color variant name.</param>
    /// <returns>Subtle variant name.</returns>
    public static string BorderSubtleColor( string variant ) => $"--b-theme-border-{variant}-subtle";

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

    /// <summary>
    /// Generates the CSS variable for the background color of a button with the specified variant.
    /// </summary>
    /// <param name="variant">The button variant (e.g., "primary", "secondary").</param>
    /// <returns>The CSS variable name for the button background color.</returns>
    public static string ButtonBackground( string variant ) => $"--b-button-{variant}-background";

    /// <summary>
    /// Generates the CSS variable for the border color of a button with the specified variant.
    /// </summary>
    /// <param name="variant">The button variant.</param>
    /// <returns>The CSS variable name for the button border color.</returns>
    public static string ButtonBorder( string variant ) => $"--b-button-{variant}-border";

    /// <summary>
    /// Generates the CSS variable for the background color of a button when hovered.
    /// </summary>
    /// <param name="variant">The button variant.</param>
    /// <returns>The CSS variable name for the button hover background color.</returns>
    public static string ButtonHoverBackground( string variant ) => $"--b-button-{variant}-hover-background";

    /// <summary>
    /// Generates the CSS variable for the border color of a button when hovered.
    /// </summary>
    /// <param name="variant">The button variant.</param>
    /// <returns>The CSS variable name for the button hover border color.</returns>
    public static string ButtonHoverBorder( string variant ) => $"--b-button-{variant}-hover-border";

    /// <summary>
    /// Generates the CSS variable for the background color of a button when active (clicked).
    /// </summary>
    /// <param name="variant">The button variant.</param>
    /// <returns>The CSS variable name for the button active background color.</returns>
    public static string ButtonActiveBackground( string variant ) => $"--b-button-{variant}-active-background";

    /// <summary>
    /// Generates the CSS variable for the border color of a button when active (clicked).
    /// </summary>
    /// <param name="variant">The button variant.</param>
    /// <returns>The CSS variable name for the button active border color.</returns>
    public static string ButtonActiveBorder( string variant ) => $"--b-button-{variant}-active-border";

    /// <summary>
    /// Generates the CSS variable for the YIQ contrast background color of a button.
    /// YIQ is used to determine text visibility on different backgrounds.
    /// </summary>
    /// <param name="variant">The button variant.</param>
    /// <returns>The CSS variable name for the YIQ background color.</returns>
    public static string ButtonYiqBackground( string variant ) => $"--b-button-{variant}-yiq-background";

    /// <summary>
    /// Generates the CSS variable for the YIQ contrast background color of a button when hovered.
    /// </summary>
    /// <param name="variant">The button variant.</param>
    /// <returns>The CSS variable name for the YIQ hover background color.</returns>
    public static string ButtonYiqHoverBackground( string variant ) => $"--b-button-{variant}-yiq-hover-background";

    /// <summary>
    /// Generates the CSS variable for the YIQ contrast background color of a button when active.
    /// </summary>
    /// <param name="variant">The button variant.</param>
    /// <returns>The CSS variable name for the YIQ active background color.</returns>
    public static string ButtonYiqActiveBackground( string variant ) => $"--b-button-{variant}-yiq-active-background";

    /// <summary>
    /// Generates the CSS variable for the box shadow of a button.
    /// </summary>
    /// <param name="variant">The button variant.</param>
    /// <returns>The CSS variable name for the button box shadow.</returns>
    public static string ButtonBoxShadow( string variant ) => $"--b-button-{variant}-box-shadow";

    /// <summary>
    /// Generates the CSS variable for the text color of an outlined button.
    /// </summary>
    /// <param name="variant">The button variant.</param>
    /// <returns>The CSS variable name for the outline button text color.</returns>
    public static string OutlineButtonColor( string variant ) => $"--b-outline-button-{variant}-color";

    /// <summary>
    /// Generates the CSS variable for the YIQ contrast color of an outlined button.
    /// YIQ is used to ensure text visibility on different background colors.
    /// </summary>
    /// <param name="variant">The button variant.</param>
    /// <returns>The CSS variable name for the outline button YIQ contrast color.</returns>
    public static string OutlineButtonYiqColor( string variant ) => $"--b-outline-button-{variant}-yiq-shadow";

    /// <summary>
    /// Generates the CSS variable for the box shadow color of an outlined button.
    /// </summary>
    /// <param name="variant">The button variant.</param>
    /// <returns>The CSS variable name for the outline button box shadow color.</returns>
    public static string OutlineButtonBoxShadowColor( string variant ) => $"--b-outline-button-{variant}-box-shadow";

    /// <summary>
    /// Generates the CSS variable for the text color of an outlined button when hovered.
    /// </summary>
    /// <param name="variant">The button variant.</param>
    /// <returns>The CSS variable name for the outline button hover text color.</returns>
    public static string OutlineButtonHoverColor( string variant ) => $"--b-outline-button-{variant}-hover-color";

    /// <summary>
    /// Generates the CSS variable for the text color of an outlined button when active (clicked).
    /// </summary>
    /// <param name="variant">The button variant.</param>
    /// <returns>The CSS variable name for the outline button active text color.</returns>
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


    /// <summary>
    /// Generates the CSS variable for the icon color of a <c>StepsItem</c> component with the specified variant.
    /// </summary>
    /// <param name="variant">The steps item variant (e.g., "completed", "active").</param>
    /// <returns>The CSS variable name for the steps item icon color.</returns>
    public static string VariantStepsItemIcon( string variant ) => $"--b-steps-item-{variant}-icon-color";

    /// <summary>
    /// Generates the CSS variable for the YIQ contrast color of a <c>StepsItem</c> icon with the specified variant.
    /// YIQ is used to adjust contrast for better visibility.
    /// </summary>
    /// <param name="variant">The steps item variant.</param>
    /// <returns>The CSS variable name for the steps item icon YIQ contrast color.</returns>
    public static string VariantStepsItemIconYiq( string variant ) => $"--b-steps-item-{variant}-icon-yiq";

    /// <summary>
    /// Generates the CSS variable for the text color of a <c>StepsItem</c> component with the specified variant.
    /// </summary>
    /// <param name="variant">The steps item variant.</param>
    /// <returns>The CSS variable name for the steps item text color.</returns>
    public static string VariantStepsItemText( string variant ) => $"--b-steps-item-{variant}-text-color";

    /// <summary>
    /// CSS variable for defining the icon color of a <c>StepsItem</c> component.
    /// </summary>
    public static string StepsItemIcon => "--b-steps-item-icon-color";

    /// <summary>
    /// CSS variable for defining the icon color of a completed <c>StepsItem</c>.
    /// </summary>
    public static string StepsItemIconCompleted => "--b-steps-item-icon-completed";

    /// <summary>
    /// CSS variable for defining the YIQ contrast color of a completed <c>StepsItem</c> icon.
    /// </summary>
    public static string StepsItemIconCompletedYiq => "--b-steps-item-icon-completed-yiq";

    /// <summary>
    /// CSS variable for defining the icon color of an active <c>StepsItem</c>.
    /// </summary>
    public static string StepsItemIconActive => "--b-steps-item-icon-active";

    /// <summary>
    /// CSS variable for defining the YIQ contrast color of an active <c>StepsItem</c> icon.
    /// </summary>
    public static string StepsItemIconActiveYiq => "--b-steps-item-icon-active-yiq";

    /// <summary>
    /// CSS variable for defining the text color of a <c>StepsItem</c>.
    /// </summary>
    public static string StepsItemText => "--b-steps-item-text-color";

    /// <summary>
    /// CSS variable for defining the text color of a completed <c>StepsItem</c>.
    /// </summary>
    public static string StepsItemTextCompleted => "--b-steps-item-text-completed";

    /// <summary>
    /// CSS variable for defining the text color of an active <c>StepsItem</c>.
    /// </summary>
    public static string StepsItemTextActive => "--b-steps-item-text-active";

    /// <summary>
    /// CSS variable for defining the size of the <c>SpinKit</c> loader.
    /// </summary>
    public static string SpinKitSize => "--b-spinkit-size";

    /// <summary>
    /// CSS variable for defining the color of the <c>SpinKit</c> loader.
    /// </summary>
    public static string SpinKitColor => "--b-spinkit-color";


    /// <summary>
    /// Generates the CSS variable for the color of the page progress indicator with the specified variant.
    /// </summary>
    /// <param name="variant">The progress indicator variant (e.g., "primary", "secondary").</param>
    /// <returns>The CSS variable name for the page progress indicator color.</returns>
    public static string VariantPageProgressIndicator( string variant ) => $"--b-page-progress-indicator-{variant}";

    /// <summary>
    /// Generates the CSS variable for the color of the rating component with the specified variant.
    /// </summary>
    /// <param name="variant">The rating variant (e.g., "primary", "warning", "success").</param>
    /// <returns>The CSS variable name for the rating color.</returns>
    public static string VariantRatingColor( string variant ) => $"--b-rating-{variant}-color";


    public const string BreadcrumbColor = "--b-breadcrumb-color";
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}