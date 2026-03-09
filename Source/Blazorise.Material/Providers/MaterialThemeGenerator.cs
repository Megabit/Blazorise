#region Using directives
using System;
using System.Globalization;
using System.Text;
#endregion

namespace Blazorise.Material.Providers;

public class MaterialThemeGenerator : ThemeGenerator
{
    #region Constructors

    public MaterialThemeGenerator( IThemeCache themeCache )
        : base( themeCache )
    {
    }

    #endregion

    #region Methods

    public override string GenerateVariables( Theme theme )
    {
        string background = ResolveThemeColor(
            "#FFFBFF",
            theme?.BodyOptions?.BackgroundColor,
            theme?.BackgroundOptions?.Body );

        System.Drawing.Color backgroundColor = ParseColor( background );
        bool isDarkScheme = LuminanceFromColor( backgroundColor ) < 45d;

        string onBackground = ResolveThemeColor(
            ToHex( Contrast( theme, backgroundColor ) ),
            theme?.BodyOptions?.TextColor,
            theme?.TextColorOptions?.Body );

        System.Drawing.Color onBackgroundColor = ParseColor( onBackground );
        System.Drawing.Color surfaceColor = backgroundColor;
        System.Drawing.Color onSurfaceColor = onBackgroundColor;

        System.Drawing.Color surfaceVariantColor = Mix( onBackgroundColor, surfaceColor, isDarkScheme ? 16d : 12d );
        System.Drawing.Color onSurfaceVariantColor = Mix( onBackgroundColor, surfaceColor, isDarkScheme ? 66d : 70d );

        System.Drawing.Color surfaceDimColor = isDarkScheme
            ? ShadeColor( surfaceColor, 8d )
            : ShadeColor( surfaceColor, 12d );

        System.Drawing.Color surfaceBrightColor = isDarkScheme
            ? TintColor( surfaceColor, 8d )
            : TintColor( surfaceColor, 3d );

        System.Drawing.Color surfaceContainerLowestColor = isDarkScheme
            ? ShadeColor( surfaceColor, 6d )
            : TintColor( surfaceColor, 10d );

        System.Drawing.Color surfaceContainerLowColor = isDarkScheme
            ? TintColor( surfaceColor, 4d )
            : ShadeColor( surfaceColor, 3d );

        System.Drawing.Color surfaceContainerColor = isDarkScheme
            ? TintColor( surfaceColor, 8d )
            : ShadeColor( surfaceColor, 6d );

        System.Drawing.Color surfaceContainerHighColor = isDarkScheme
            ? TintColor( surfaceColor, 12d )
            : ShadeColor( surfaceColor, 9d );

        System.Drawing.Color surfaceContainerHighestColor = isDarkScheme
            ? TintColor( surfaceColor, 16d )
            : ShadeColor( surfaceColor, 12d );

        System.Drawing.Color outlineColor = Mix( onSurfaceVariantColor, surfaceColor, isDarkScheme ? 72d : 82d );
        System.Drawing.Color outlineVariantColor = Mix( onSurfaceVariantColor, surfaceColor, isDarkScheme ? 42d : 34d );

        SetVar( "--mui-background", ToHex( backgroundColor ) );
        SetVar( "--mui-on-background", ToHex( onBackgroundColor ) );
        SetVar( "--mui-surface", ToHex( surfaceColor ) );
        SetVar( "--mui-on-surface", ToHex( onSurfaceColor ) );
        SetVar( "--mui-surface-variant", ToHex( surfaceVariantColor ) );
        SetVar( "--mui-on-surface-variant", ToHex( onSurfaceVariantColor ) );
        SetVar( "--mui-surface-dim", ToHex( surfaceDimColor ) );
        SetVar( "--mui-surface-bright", ToHex( surfaceBrightColor ) );
        SetVar( "--mui-surface-container-lowest", ToHex( surfaceContainerLowestColor ) );
        SetVar( "--mui-surface-container-low", ToHex( surfaceContainerLowColor ) );
        SetVar( "--mui-surface-container", ToHex( surfaceContainerColor ) );
        SetVar( "--mui-surface-container-high", ToHex( surfaceContainerHighColor ) );
        SetVar( "--mui-surface-container-highest", ToHex( surfaceContainerHighestColor ) );
        SetVar( "--mui-outline", ToHex( outlineColor ) );
        SetVar( "--mui-outline-variant", ToHex( outlineVariantColor ) );

        string primary = ResolveThemeColor( "#4355B9", theme?.ColorOptions?.Primary );
        string secondary = ResolveThemeColor( "#5B5D72", theme?.ColorOptions?.Secondary );
        string tertiary = ResolveThemeColor( "#7A5260", ResolveTertiaryColor( primary, secondary, theme ) );
        string error = ResolveThemeColor( "#BA1A1A", theme?.ColorOptions?.Danger );
        string success = ResolveThemeColor( "#006D3D", theme?.ColorOptions?.Success );
        string warning = ResolveThemeColor( "#7A5800", theme?.ColorOptions?.Warning );
        string info = ResolveThemeColor( "#00629B", theme?.ColorOptions?.Info );
        string light = ResolveThemeColor( "#FDF8FD", theme?.ColorOptions?.Light, theme?.BackgroundOptions?.Light );
        string dark = ResolveThemeColor( "#1C1B1E", theme?.ColorOptions?.Dark, theme?.BackgroundOptions?.Dark );

        SetMaterialColorRole( theme, "primary", primary, isDarkScheme, outlineColor, outlineVariantColor );
        SetMaterialColorRole( theme, "secondary", secondary, isDarkScheme, outlineColor, outlineVariantColor );
        SetMaterialColorRole( theme, "tertiary", tertiary, isDarkScheme, outlineColor, outlineVariantColor );
        SetMaterialColorRole( theme, "error", error, isDarkScheme, outlineColor, outlineVariantColor );
        SetMaterialColorRole( theme, "success", success, isDarkScheme, outlineColor, outlineVariantColor );
        SetMaterialColorRole( theme, "warning", warning, isDarkScheme, outlineColor, outlineVariantColor );
        SetMaterialColorRole( theme, "info", info, isDarkScheme, outlineColor, outlineVariantColor );
        SetMaterialColorRole( theme, "light", light, isDarkScheme, outlineColor, outlineVariantColor );
        SetMaterialColorRole( theme, "dark", dark, isDarkScheme, outlineColor, outlineVariantColor );

        string shadow = ResolveThemeColor( "#000000", theme?.Black );
        SetVar( "--mui-shadow", shadow );
        SetVar( "--mui-scrim", shadow );

        System.Drawing.Color activeColor = ParseColor( ToHex( onSurfaceVariantColor ) );
        SetVar(
            "--mui-active",
            string.Format(
                CultureInfo.InvariantCulture,
                "rgb({0} {1} {2} / {3})",
                activeColor.R,
                activeColor.G,
                activeColor.B,
                isDarkScheme ? "0.24" : "0.16" ) );

        SetVar(
            "--mui-overlay",
            string.Format(
                CultureInfo.InvariantCulture,
                "rgb({0} {1} {2} / {3})",
                ParseColor( shadow ).R,
                ParseColor( shadow ).G,
                ParseColor( shadow ).B,
                isDarkScheme ? "0.64" : "0.50" ) );

        if ( !string.IsNullOrEmpty( theme?.BodyOptions?.FontOptions?.Family ) )
            SetVar( "--mui-font-family", theme.BodyOptions.FontOptions.Family );

        if ( !string.IsNullOrEmpty( theme?.BodyOptions?.FontOptions?.Size ) )
        {
            SetVar( "--mui-font-size", theme.BodyOptions.FontOptions.Size );

            if ( TryParseLength( theme.BodyOptions.FontOptions.Size, out decimal fontSize, out string unit ) )
            {
                SetVar( "--mui-font-size-sm", string.Format( CultureInfo.InvariantCulture, "{0:0.###}{1}", fontSize * 0.875m, unit ) );
                SetVar( "--mui-font-size-lg", string.Format( CultureInfo.InvariantCulture, "{0:0.###}{1}", fontSize * 1.25m, unit ) );
            }
        }

        if ( !string.IsNullOrEmpty( theme?.SpacingOptions?.Is3 ) )
            SetVar( "--mui-spacing", theme.SpacingOptions.Is3 );

        return base.GenerateVariables( theme );
    }

    protected override void GenerateBackgroundVariantStyles( StringBuilder sb, Theme theme, string variant )
    {
    }

    protected override void GenerateBorderVariantStyles( StringBuilder sb, Theme theme, string variant )
    {
    }

    protected override void GenerateButtonVariantStyles( StringBuilder sb, Theme theme, string variant, ThemeButtonOptions options )
    {
    }

    protected override void GenerateButtonOutlineVariantStyles( StringBuilder sb, Theme theme, string variant, ThemeButtonOptions options )
    {
    }

    protected override void GenerateButtonStyles( StringBuilder sb, Theme theme, ThemeButtonOptions options )
    {
    }

    protected override void GenerateDropdownStyles( StringBuilder sb, Theme theme, ThemeDropdownOptions options )
    {
    }

    protected override void GenerateInputStyles( StringBuilder sb, Theme theme, ThemeInputOptions options )
    {
        _ = sb;
        _ = theme;
        _ = options;
    }

    protected void GenerateInputCheckEditStyles( StringBuilder sb, Theme theme, ThemeInputOptions options )
    {
        _ = sb;
        _ = theme;
        _ = options;
    }

    public string GenerateSvgDataUrl( string colorHex, float sizeInRem, int basePixelSize = 24 )
    {
        if ( string.IsNullOrWhiteSpace( colorHex ) )
            colorHex = "#000000";

        var color = colorHex.TrimStart( '#' );

        int pixelSize = (int)Math.Round( sizeInRem * basePixelSize );

        string rawSvg = $@"<svg xmlns='http://www.w3.org/2000/svg' height='{pixelSize}' width='{pixelSize}' fill='%23{color}'><path d='M0 0h24v24H0z' fill='none'/><path d='M19 3H5a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2V5a2 2 0 0 0-2-2zm-9 14-5-5 1.41-1.41L10 14.17l7.59-7.59L19 8l-9 9z'/></svg>";

        string encodedSvg = rawSvg
            .Replace( "<", "%3C" )
            .Replace( ">", "%3E" )
            .Replace( "\"", "'" )
            .Replace( "#", "%23" )
            .Replace( " ", "%20" )
            .Replace( "\n", "" )
            .Replace( "\r", "" );

        return encodedSvg;
    }

    protected override void GenerateBadgeVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor )
    {
    }

    protected override void GenerateSwitchVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, ThemeSwitchOptions options )
    {
        _ = sb;
        _ = theme;
        _ = variant;
        _ = inBackgroundColor;
        _ = options;
    }

    protected override void GenerateStepsVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, ThemeStepsOptions stepsOptions )
    {
        _ = sb;
        _ = theme;
        _ = variant;
        _ = inBackgroundColor;
        _ = stepsOptions;
    }

    protected override void GenerateRatingVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, ThemeRatingOptions ratingOptions )
    {
    }

    protected override void GenerateAlertVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, string inBorderColor, string inColor, ThemeAlertOptions options )
    {
    }

    protected override void GenerateTableVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, string inBorderColor )
    {
    }

    protected override void GenerateCardStyles( StringBuilder sb, Theme theme, ThemeCardOptions options )
    {
    }

    protected override void GenerateModalStyles( StringBuilder sb, Theme theme, ThemeModalOptions options )
    {
    }

    protected override void GenerateTabsStyles( StringBuilder sb, Theme theme, ThemeTabsOptions options )
    {
    }

    protected override void GenerateProgressStyles( StringBuilder sb, Theme theme, ThemeProgressOptions options )
    {
        if ( !string.IsNullOrEmpty( options?.BorderRadius ) )
        {
            sb.Append( ".mui-progress" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );
        }

        sb.Append( ".mui-progress" ).Append( "{" )
            .Append( "--mui-progress-color: var(--mui-primary);" )
            .Append( "--mui-progress-text-color: var(--mui-on-primary);" )
            .AppendLine( "}" );

        base.GenerateProgressStyles( sb, theme, options );
    }

    protected override void GenerateAlertStyles( StringBuilder sb, Theme theme, ThemeAlertOptions options )
    {
    }

    protected override void GenerateBreadcrumbStyles( StringBuilder sb, Theme theme, ThemeBreadcrumbOptions options )
    {
    }

    protected override void GenerateBadgeStyles( StringBuilder sb, Theme theme, ThemeBadgeOptions options )
    {
    }

    protected override void GeneratePaginationStyles( StringBuilder sb, Theme theme, ThemePaginationOptions options )
    {
    }

    protected override void GenerateBarStyles( StringBuilder sb, Theme theme, ThemeBarOptions options )
    {
    }

    protected override void GenerateStepsStyles( StringBuilder sb, Theme theme, ThemeStepsOptions stepsOptions )
    {
        _ = sb;
        _ = theme;
        _ = stepsOptions;
    }

    protected override void GenerateRatingStyles( StringBuilder sb, Theme theme, ThemeRatingOptions ratingOptions )
    {
    }

    protected override void GenerateParagraphVariantStyles( StringBuilder sb, Theme theme, string variant, string inTextColor )
    {
    }

    protected override void GenerateInputVariantStyles( StringBuilder sb, Theme theme, string variant, string inColor )
    {
    }

    protected override void GenerateListGroupItemStyles( StringBuilder sb, Theme theme, ThemeListGroupItemOptions options )
    {
    }

    protected override void GenerateListGroupItemVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, string inColor, ThemeListGroupItemOptions options )
    {
    }

    protected override void GenerateSpacingStyles( StringBuilder sb, Theme theme, ThemeSpacingOptions options )
    {
    }

    private void SetMaterialColorRole( Theme theme, string role, string roleHex, bool isDarkScheme, System.Drawing.Color globalOutline, System.Drawing.Color globalOutlineVariant )
    {
        System.Drawing.Color roleColor = ParseColor( roleHex );

        if ( roleColor.IsEmpty )
            return;

        bool roleIsLight = LuminanceFromColor( roleColor ) > 65d;
        bool isLightRole = role.Equals( "light", StringComparison.Ordinal );
        bool isDarkRole = role.Equals( "dark", StringComparison.Ordinal );

        System.Drawing.Color hoverColor = isLightRole
            ? isDarkScheme ? TintColor( roleColor, 6d ) : ShadeColor( roleColor, 3d )
            : isDarkRole ? TintColor( roleColor, 8d ) : GetHoverColor( roleColor, isDarkScheme );

        System.Drawing.Color containerColor = isLightRole
            ? isDarkScheme ? TintColor( roleColor, 10d ) : ShadeColor( roleColor, 3d )
            : isDarkRole ? isDarkScheme ? ShadeColor( roleColor, 12d ) : TintColor( roleColor, 18d )
            : isDarkScheme ? ShadeColor( roleColor, roleIsLight ? 35d : 50d ) : roleIsLight ? ShadeColor( roleColor, 8d ) : TintColor( roleColor, 82d );

        System.Drawing.Color onContainerColor = isDarkRole
            ? TintColor( roleColor, 82d )
            : isLightRole ? ShadeColor( roleColor, 80d )
            : isDarkScheme ? TintColor( roleColor, 80d ) : ShadeColor( roleColor, 65d );

        System.Drawing.Color outlineRoleColor = Mix( containerColor, globalOutline, 45d );
        System.Drawing.Color outlineRoleVariantColor = Mix( containerColor, globalOutlineVariant, 45d );

        SetVar( $"--mui-{role}", ToHex( roleColor ) );
        SetVar( $"--mui-{role}-hover", ToHex( hoverColor ) );
        SetVar( $"--mui-on-{role}", ToHex( Contrast( theme, roleColor ) ) );
        SetVar( $"--mui-{role}-container", ToHex( containerColor ) );
        SetVar( $"--mui-on-{role}-container", ToHex( onContainerColor ) );
        SetVar( $"--mui-outline-{role}", ToHex( outlineRoleColor ) );
        SetVar( $"--mui-outline-{role}-variant", ToHex( outlineRoleVariantColor ) );
    }

    private static System.Drawing.Color GetHoverColor( System.Drawing.Color color, bool isDarkScheme )
    {
        double luminance = LuminanceFromColor( color );

        if ( isDarkScheme && luminance < 70d )
            return Lighten( color, 8f );

        return Darken( color, 8f );
    }

    private static bool TryParseLength( string value, out decimal length, out string unit )
    {
        length = 0m;
        unit = null;

        if ( string.IsNullOrWhiteSpace( value ) )
            return false;

        string normalized = value.Trim().ToLowerInvariant();

        if ( normalized.EndsWith( "rem", StringComparison.Ordinal ) )
            unit = "rem";
        else if ( normalized.EndsWith( "em", StringComparison.Ordinal ) )
            unit = "em";
        else if ( normalized.EndsWith( "px", StringComparison.Ordinal ) )
            unit = "px";
        else
            return false;

        string numericPart = normalized.Substring( 0, normalized.Length - unit.Length ).Trim();

        if ( !decimal.TryParse( numericPart, NumberStyles.Float, CultureInfo.InvariantCulture, out length ) )
            return false;

        return true;
    }

    private static string ResolveTertiaryColor( string primaryHex, string secondaryHex, Theme theme )
    {
        if ( !string.IsNullOrEmpty( theme?.ColorOptions?.Link ) )
        {
            System.Drawing.Color linkColor = ParseColor( theme.ColorOptions.Link );
            System.Drawing.Color primaryColor = ParseColor( primaryHex );

            if ( !linkColor.IsEmpty && ToHex( linkColor ) != ToHex( primaryColor ) )
                return ToHex( linkColor );
        }

        System.Drawing.Color primary = ParseColor( primaryHex );
        System.Drawing.Color secondary = ParseColor( secondaryHex );

        if ( primary.IsEmpty || secondary.IsEmpty )
            return null;

        return ToHex( Mix( secondary, primary, 52d ) );
    }

    private static string ResolveThemeColor( string fallback, params string[] candidates )
    {
        if ( candidates is not null )
        {
            for ( int i = 0; i < candidates.Length; i++ )
            {
                string candidate = candidates[i];

                if ( string.IsNullOrWhiteSpace( candidate ) )
                    continue;

                System.Drawing.Color parsedColor = ParseColor( candidate );

                if ( !parsedColor.IsEmpty )
                    return ToHex( parsedColor );
            }
        }

        System.Drawing.Color fallbackColor = ParseColor( fallback );

        if ( !fallbackColor.IsEmpty )
            return ToHex( fallbackColor );

        return fallback;
    }

    #endregion
}