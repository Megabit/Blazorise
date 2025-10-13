#region Using directives
using System;
using System.Text;
#endregion

namespace Blazorise.FluentUI2.Providers;

public class FluentUI2ThemeGenerator : ThemeGenerator
{
    #region Constructors

    public FluentUI2ThemeGenerator( IThemeCache themeCache )
        : base( themeCache )
    {
    }

    #endregion

    #region Methods

    public override string GenerateVariables( Theme theme )
    {
        // Polarity-aware state generator: lighten dark colors, darken light colors.
        (string hover, string pressed, string selected) StatesFrom( string hex )
        {
            var c = ParseColor( hex );

            // simple perceptual luminance (0..255), same components used by Contrast()
            var luminance = ( 299 * c.R + 587 * c.G + 114 * c.B ) / 1000d;

            // pick direction based on brightness (threshold ~ 140 is a nice middle)
            bool isDark = luminance < theme.LuminanceThreshold;

            // use smaller deltas to match Fluent’s subtle state ramps
            float h = 6f, p = 12f, s = 9f;

            var hover = ToHex( isDark ? Lighten( c, h ) : Darken( c, h ) );
            var pressed = ToHex( isDark ? Lighten( c, p ) : Darken( c, p ) );
            var selected = ToHex( isDark ? Lighten( c, s ) : Darken( c, s ) );
            return (hover, pressed, selected);
        }

        // Body / typography (override if provided)
        if ( !string.IsNullOrWhiteSpace( theme.BodyOptions?.TextColor ) )
            SetVar( "--colorNeutralForeground1", ToHex( ParseColor( theme.BodyOptions.TextColor ) ) );

        if ( !string.IsNullOrWhiteSpace( theme.BodyOptions?.BackgroundColor ) )
            SetVar( "--colorNeutralBackground1", ToHex( ParseColor( theme.BodyOptions.BackgroundColor ) ) );

        if ( theme.BodyOptions?.FontOptions is not null )
        {
            if ( !string.IsNullOrWhiteSpace( theme.BodyOptions.FontOptions.Family ) )
                SetVar( "--fontFamilyBase", theme.BodyOptions.FontOptions.Family );

            if ( !string.IsNullOrWhiteSpace( theme.BodyOptions.FontOptions.Size ) )
            {
                SetVar( "--fontSizeBase400", theme.BodyOptions.FontOptions.Size );
                ApplyFluentTypeRampFromBase400( theme.BodyOptions.FontOptions.Size );
            }

            if ( !string.IsNullOrWhiteSpace( theme.BodyOptions?.FontOptions.Weight ) )
            {
                var wRegular = ParseCssFontWeight( theme.BodyOptions.FontOptions.Weight, 400 );
                var (reg, med, semi, bold) = DeriveFontWeights( wRegular );

                SetVar( "--fontWeightRegular", reg.ToString() );
                SetVar( "--fontWeightMedium", med.ToString() );
                SetVar( "--fontWeightSemibold", semi.ToString() );
                SetVar( "--fontWeightBold", bold.ToString() );
            }
        }

        // Brand color (Primary) - Fluent tokens (+ states)
        var brand = theme.ColorOptions?.Primary;
        if ( !string.IsNullOrWhiteSpace( brand ) )
        {
            var baseHex = ToHex( ParseColor( brand ) );
            var (hover, pressed, selected) = StatesFrom( baseHex );

            // core brand bg + states
            SetVar( "--colorBrandBackground", baseHex );
            SetVar( "--colorBrandBackgroundHover", hover );
            SetVar( "--colorBrandBackgroundPressed", pressed );
            SetVar( "--colorBrandBackgroundSelected", selected );
            SetVar( "--colorBrandBackgroundStatic", baseHex );

            // compound brand backgrounds
            SetVar( "--colorCompoundBrandBackground", baseHex );
            SetVar( "--colorCompoundBrandBackgroundHover", hover );
            SetVar( "--colorCompoundBrandBackgroundPressed", pressed );

            // brand foregrounds/links
            SetVar( "--colorBrandForeground1", baseHex );
            SetVar( "--colorBrandForeground2", ToHex( Darken( ParseColor( baseHex ), 10f ) ) );
            SetVar( "--colorBrandForeground2Hover", ToHex( Darken( ParseColor( baseHex ), 15f ) ) );
            SetVar( "--colorBrandForeground2Pressed", ToHex( Darken( ParseColor( baseHex ), 35f ) ) );
            SetVar( "--colorBrandForegroundLink", baseHex );
            SetVar( "--colorBrandForegroundLinkHover", ToHex( Darken( ParseColor( baseHex ), 12f ) ) );
            SetVar( "--colorBrandForegroundLinkPressed", ToHex( Darken( ParseColor( baseHex ), 28f ) ) );
            SetVar( "--colorBrandForegroundLinkSelected", baseHex );

            // on-brand text/strokes
            SetVar( "--colorNeutralForegroundOnBrand", "#ffffff" );
            SetVar( "--colorNeutralStrokeOnBrand", "#ffffff" );
            SetVar( "--colorBrandStroke1", baseHex );
            SetVar( "--colorCompoundBrandStroke", baseHex );
            SetVar( "--colorCompoundBrandStrokeHover", hover );
            SetVar( "--colorCompoundBrandStrokePressed", ToHex( Darken( ParseColor( baseHex ), 12f ) ) );

            // subtle brand background (for chips/badges) + states
            var brand2 = ToHex( TintColor( ParseColor( baseHex ), 85f ) );
            SetVar( "--colorBrandBackground2", brand2 );
            SetVar( "--colorBrandBackground2Hover", ToHex( ShadeColor( ParseColor( brand2 ), 10f ) ) );
            SetVar( "--colorBrandBackground2Pressed", ToHex( ShadeColor( ParseColor( brand2 ), 25f ) ) );

            // inverted brand background - defer to neutral backgrounds (keeps system consistent)
            SetVar( "--colorBrandBackgroundInverted", "var(--colorNeutralBackground1)" );
            SetVar( "--colorBrandBackgroundInvertedHover", "var(--colorNeutralBackground1Hover)" );
            SetVar( "--colorBrandBackgroundInvertedPressed", "var(--colorNeutralBackground1Pressed)" );
            SetVar( "--colorBrandBackgroundInvertedSelected", "var(--colorNeutralBackground1Selected)" );

            // brand stroke 2 + contrast
            SetVar( "--colorBrandStroke2", ToHex( ShadeColor( ParseColor( baseHex ), 30f ) ) );
            SetVar( "--colorBrandStroke2Hover", ToHex( ShadeColor( ParseColor( baseHex ), 45f ) ) );
            SetVar( "--colorBrandStroke2Pressed", baseHex );
            SetVar( "--colorBrandStroke2Contrast", ToHex( TintColor( ParseColor( baseHex ), 55f ) ) );

            // foreground on light surfaces
            SetVar( "--colorBrandForegroundOnLight", baseHex );
            SetVar( "--colorBrandForegroundOnLightHover", ToHex( Darken( ParseColor( baseHex ), 8f ) ) );
            SetVar( "--colorBrandForegroundOnLightPressed", ToHex( Darken( ParseColor( baseHex ), 20f ) ) );
            SetVar( "--colorBrandForegroundOnLightSelected", ToHex( Darken( ParseColor( baseHex ), 12f ) ) );

            // compound brand foreground
            SetVar( "--colorCompoundBrandForeground1", baseHex );
            SetVar( "--colorCompoundBrandForeground1Hover", hover );
            SetVar( "--colorCompoundBrandForeground1Pressed", pressed );

            // neutral brand-variants (useful for mixed UIs)
            SetVar( "--colorNeutralForeground2BrandHover", hover );
            SetVar( "--colorNeutralForeground2BrandPressed", ToHex( Darken( ParseColor( baseHex ), 18f ) ) );
            SetVar( "--colorNeutralForeground2BrandSelected", baseHex );
            SetVar( "--colorNeutralForeground3BrandHover", hover );
            SetVar( "--colorNeutralForeground3BrandPressed", ToHex( Darken( ParseColor( baseHex ), 18f ) ) );
            SetVar( "--colorNeutralForeground3BrandSelected", baseHex );

            // neutral "link" mapped to brand (neutral link semantics)
            SetVar( "--colorNeutralForeground2Link", baseHex );
            SetVar( "--colorNeutralForeground2LinkHover", hover );
            SetVar( "--colorNeutralForeground2LinkPressed", pressed );
            SetVar( "--colorNeutralForeground2LinkSelected", baseHex );

            // on-brand2 strokes
            SetVar( "--colorNeutralStrokeOnBrand2", baseHex );
            SetVar( "--colorNeutralStrokeOnBrand2Hover", ToHex( TintColor( ParseColor( baseHex ), 60f ) ) );
            SetVar( "--colorNeutralStrokeOnBrand2Pressed", ToHex( TintColor( ParseColor( baseHex ), 40f ) ) );
            SetVar( "--colorNeutralStrokeOnBrand2Selected", ToHex( TintColor( ParseColor( baseHex ), 60f ) ) );

            // NOTE: we intentionally DO NOT set --colorNeutralForegroundInverted*
            // so controls like Switch keep their correct white checked foreground.
        }

        // Secondary - some neutral accents
        var secondary = theme.ColorOptions?.Secondary;
        if ( !string.IsNullOrWhiteSpace( secondary ) )
        {
            var sec = ToHex( ParseColor( secondary ) );
            SetVar( "--colorNeutralForeground3", sec );
            SetVar( "--colorNeutralForeground2", ToHex( TintColor( ParseColor( sec ), 15f ) ) );
            SetVar( "--colorNeutralBackground3", ToHex( TintColor( ParseColor( sec ), 90f ) ) );
            SetVar( "--colorNeutralStrokeAccessible", ToHex( ShadeColor( ParseColor( sec ), 35f ) ) );
            SetVar( "--colorNeutralStroke1", ToHex( ShadeColor( ParseColor( sec ), 8f ) ) );
            SetVar( "--colorNeutralStroke1Hover", ToHex( ShadeColor( ParseColor( sec ), 12f ) ) );
            SetVar( "--colorNeutralStroke1Pressed", ToHex( ShadeColor( ParseColor( sec ), 16f ) ) );
            SetVar( "--colorNeutralStroke1Selected", ToHex( ShadeColor( ParseColor( sec ), 10f ) ) );
        }

        // Status colors
        var success = theme.ColorOptions?.Success;
        if ( !string.IsNullOrWhiteSpace( success ) )
        {
            var hex = ToHex( ParseColor( success ) );
            var (h, p, _) = StatesFrom( hex );

            SetVar( "--colorStatusSuccessBackground3", hex );
            SetVar( "--colorStatusSuccessBackground2", ToHex( TintColor( ParseColor( hex ), 55f ) ) );
            SetVar( "--colorStatusSuccessBackground1", ToHex( TintColor( ParseColor( hex ), 92f ) ) );

            SetVar( "--colorStatusSuccessForeground3", hex );
            SetVar( "--colorStatusSuccessForeground2", ToHex( ShadeColor( ParseColor( hex ), 35f ) ) );
            SetVar( "--colorStatusSuccessForeground1", ToHex( ShadeColor( ParseColor( hex ), 45f ) ) );

            SetVar( "--colorStatusSuccessBorderActive", hex );
            SetVar( "--colorStatusSuccessBackground3Hover", h );
            SetVar( "--colorStatusSuccessBackground3Pressed", p );

            // inverted foreground for dark surfaces
            SetVar( "--colorStatusSuccessForegroundInverted", ToHex( ShadeColor( ParseColor( hex ), 20f ) ) );
        }

        var warning = theme.ColorOptions?.Warning;
        if ( !string.IsNullOrWhiteSpace( warning ) )
        {
            var hex = ToHex( ParseColor( warning ) );
            var (h, p, _) = StatesFrom( hex );

            SetVar( "--colorStatusWarningBackground3", hex );
            SetVar( "--colorStatusWarningBackground2", ToHex( TintColor( ParseColor( hex ), 55f ) ) );
            SetVar( "--colorStatusWarningBackground1", ToHex( TintColor( ParseColor( hex ), 92f ) ) );

            SetVar( "--colorStatusWarningForeground3", ToHex( ShadeColor( ParseColor( hex ), 25f ) ) );
            SetVar( "--colorStatusWarningForeground2", ToHex( ShadeColor( ParseColor( hex ), 35f ) ) );
            SetVar( "--colorStatusWarningForeground1", ToHex( ShadeColor( ParseColor( hex ), 45f ) ) );

            SetVar( "--colorStatusWarningBorderActive", hex );
            SetVar( "--colorStatusWarningBackground3Hover", h );
            SetVar( "--colorStatusWarningBackground3Pressed", p );

            SetVar( "--colorStatusWarningForegroundInverted", ToHex( ShadeColor( ParseColor( hex ), 20f ) ) );
        }

        var danger = theme.ColorOptions?.Danger;
        if ( !string.IsNullOrWhiteSpace( danger ) )
        {
            var hex = ToHex( ParseColor( danger ) );
            var (h, p, _) = StatesFrom( hex );

            SetVar( "--colorStatusDangerBackground3", hex );
            SetVar( "--colorStatusDangerBackground2", ToHex( TintColor( ParseColor( hex ), 55f ) ) );
            SetVar( "--colorStatusDangerBackground1", ToHex( TintColor( ParseColor( hex ), 92f ) ) );

            SetVar( "--colorStatusDangerForeground3", hex );
            SetVar( "--colorStatusDangerForeground2", ToHex( ShadeColor( ParseColor( hex ), 35f ) ) );
            SetVar( "--colorStatusDangerForeground1", ToHex( ShadeColor( ParseColor( hex ), 45f ) ) );

            SetVar( "--colorStatusDangerBorderActive", hex );
            SetVar( "--colorStatusDangerBackground3Hover", h );
            SetVar( "--colorStatusDangerBackground3Pressed", p );

            SetVar( "--colorStatusDangerForegroundInverted", ToHex( ShadeColor( ParseColor( hex ), 20f ) ) );
        }

        // Link (explicit) + inverted neutral link
        var link = theme.ColorOptions?.Link;
        if ( !string.IsNullOrWhiteSpace( link ) )
        {
            var hex = ToHex( ParseColor( link ) );
            var (h, p, _) = StatesFrom( hex );
            SetVar( "--colorBrandForegroundLink", hex );
            SetVar( "--colorBrandForegroundLinkHover", h );
            SetVar( "--colorBrandForegroundLinkPressed", p );
            SetVar( "--colorBrandForegroundLinkSelected", hex );

            SetVar( "--colorNeutralForegroundInvertedLink", hex );
            SetVar( "--colorNeutralForegroundInvertedLinkHover", h );
            SetVar( "--colorNeutralForegroundInvertedLinkPressed", p );
            SetVar( "--colorNeutralForegroundInvertedLinkSelected", hex );
        }
        else if ( !string.IsNullOrWhiteSpace( brand ) )
        {
            // fallback to brand for inverted link if no explicit link
            var baseHex = ToHex( ParseColor( brand ) );
            var (h, p, _) = StatesFrom( baseHex );
            SetVar( "--colorNeutralForegroundInvertedLink", baseHex );
            SetVar( "--colorNeutralForegroundInvertedLinkHover", h );
            SetVar( "--colorNeutralForegroundInvertedLinkPressed", p );
            SetVar( "--colorNeutralForegroundInvertedLinkSelected", baseHex );
        }

        // Light / Dark - a few neutrals
        if ( !string.IsNullOrWhiteSpace( theme.ColorOptions?.Light ) )
        {
            var hex = ToHex( ParseColor( theme.ColorOptions.Light ) );
            SetVar( "--colorNeutralBackground2", hex );
            SetVar( "--colorNeutralBackground3", ToHex( TintColor( ParseColor( hex ), 10f ) ) );
            SetVar( "--colorNeutralStroke2", ToHex( ShadeColor( ParseColor( hex ), 18f ) ) );
        }
        if ( !string.IsNullOrWhiteSpace( theme.ColorOptions?.Dark ) )
        {
            var hex = ToHex( ParseColor( theme.ColorOptions.Dark ) );
            SetVar( "--colorNeutralForeground1", hex );
            SetVar( "--colorNeutralForeground2", ToHex( TintColor( ParseColor( hex ), 22f ) ) );
            SetVar( "--colorNeutralStrokeAccessible", ToHex( ShadeColor( ParseColor( hex ), 15f ) ) );
        }

        // Disabled & focus helpers (conservative overrides)
        SetVar( "--colorNeutralForegroundDisabled", "rgba(0,0,0,0.30)" );
        SetVar( "--colorNeutralForegroundInvertedDisabled", "rgba(255,255,255,0.40)" );
        if ( !string.IsNullOrWhiteSpace( theme.ColorOptions?.Light ) )
            SetVar( "--colorNeutralBackgroundDisabled", ToHex( ParseColor( theme.ColorOptions.Light ) ) );
        else
            SetVar( "--colorNeutralBackgroundDisabled", "var(--colorNeutralBackground3)" );
        SetVar( "--colorNeutralBackgroundInvertedDisabled", "rgba(255,255,255,0.10)" );

        SetVar( "--colorStrokeFocus1", "#ffffff" );
        SetVar( "--colorStrokeFocus2", "#000000" );

        // Optional radii toggle
        if ( theme.IsRounded == false )
        {
            SetVar( "--borderRadiusSmall", "0" );
            SetVar( "--borderRadiusMedium", "0" );
            SetVar( "--borderRadiusLarge", "0" );
            SetVar( "--borderRadiusXLarge", "0" );
        }
        else
        {
            SetVar( "--borderRadiusSmall", "2px" );
            SetVar( "--borderRadiusMedium", "4px" );
            SetVar( "--borderRadiusLarge", "6px" );
            SetVar( "--borderRadiusXLarge", "8px" );
        }

        return base.GenerateVariables( theme );
    }

    private void ApplyFluentTypeRampFromBase400( string baseSizeLiteral )
    {
        // Parse "<number><unit>" -> (value, unit)
        static (float value, string unit) ParseLength( string s )
        {
            s = s.Trim().ToLowerInvariant();
            // default px if no unit
            var unit = ( s.EndsWith( "rem" ) ? "rem" :
                        s.EndsWith( "em" ) ? "em" :
                        s.EndsWith( "px" ) ? "px" : "px" );

            if ( unit != "px" && unit != "rem" && unit != "em" )
                unit = "px";

            var numeric = s.Replace( "rem", "" ).Replace( "em", "" ).Replace( "px", "" ).Trim();
            if ( !float.TryParse( numeric, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var v ) )
                v = 16f; // safe fallback

            return (v, unit);
        }

        static string Format( float v, string unit )
            => v.ToString( "0.###", System.Globalization.CultureInfo.InvariantCulture ) + unit;

        var (base400, unit) = ParseLength( baseSizeLiteral );

        // Font-size multipliers relative to Base400
        var fs = new (string token, float mult)[]
        {
            ("--fontSizeBase100",   0.625f),
            ("--fontSizeBase200",   0.75f),
            ("--fontSizeBase300",   0.875f),
            ("--fontSizeBase400",   1.0f),
            ("--fontSizeBase500",   1.25f),
            ("--fontSizeBase600",   1.5f),
            ("--fontSizeHero700",   1.75f),
            ("--fontSizeHero800",   2.0f),
            ("--fontSizeHero900",   2.5f),
            ("--fontSizeHero1000",  4.25f),
        };

        // Line-height multipliers per step (derived from Fluent defaults)
        var lh = new (string token, string fsToken, float mult)[]
        {
            ("--lineHeightBase100",   "--fontSizeBase100",   1.4f),
            ("--lineHeightBase200",   "--fontSizeBase200",   1.3333f),
            ("--lineHeightBase300",   "--fontSizeBase300",   1.4286f),
            ("--lineHeightBase400",   "--fontSizeBase400",   1.375f),
            ("--lineHeightBase500",   "--fontSizeBase500",   1.4f),
            ("--lineHeightBase600",   "--fontSizeBase600",   1.3333f),
            ("--lineHeightHero700",   "--fontSizeHero700",   1.2857f),
            ("--lineHeightHero800",   "--fontSizeHero800",   1.25f),
            ("--lineHeightHero900",   "--fontSizeHero900",   1.3f),
            ("--lineHeightHero1000",  "--fontSizeHero1000",  1.3529f),
        };

        // Emit font sizes
        var computedFs = new System.Collections.Generic.Dictionary<string, float>();
        foreach ( var (token, mult) in fs )
        {
            var size = base400 * mult;
            computedFs[token] = size;
            SetVar( token, Format( size, unit ) );
        }

        // Emit line-heights (use same unit as font-size tokens)
        foreach ( var (token, fsToken, mult) in lh )
        {
            if ( computedFs.TryGetValue( fsToken, out var fsz ) )
            {
                var lhVal = fsz * mult;
                SetVar( token, Format( lhVal, unit ) );
            }
        }
    }

    private static int ParseCssFontWeight( string raw, int fallback = 400 )
    {
        if ( string.IsNullOrWhiteSpace( raw ) )
            return fallback;

        var s = raw.Trim().ToLowerInvariant();

        // named aliases -> numeric
        if ( s is "normal" )
            return 400;
        if ( s is "bold" )
            return 700;
        if ( s is "lighter" )
            return 300; // pragmatic
        if ( s is "bolder" )
            return 700; // pragmatic
        if ( s is "thin" )
            return 100;
        if ( s is "extralight" or "ultralight" )
            return 200;
        if ( s is "light" )
            return 300;
        if ( s is "medium" )
            return 500;
        if ( s is "semibold" or "demibold" )
            return 600;
        if ( s is "extrabold" or "ultrabold" or "heavy" )
            return 800;
        if ( s is "black" or "heavyblack" )
            return 900;

        // numeric value as string
        if ( int.TryParse( s, out var n ) )
            return Math.Clamp( n, 100, 900 );

        return fallback;
    }

    private static int Clamp100_900( int v ) => Math.Clamp( v, 100, 900 );

    // Given a "regular", derive others with standard steps (+100 each)
    private static (int regular, int medium, int semibold, int bold) DeriveFontWeights( int regular )
    {
        var medium = Clamp100_900( regular + 100 );   // 400->500
        var semibold = Clamp100_900( regular + 200 );   // 400->600
        var bold = Clamp100_900( regular + 300 );   // 400->700
        return (regular, medium, semibold, bold);
    }

    protected override void GenerateBackgroundVariantStyles( StringBuilder sb, Theme theme, string variant ) { }
    protected override void GenerateBorderVariantStyles( StringBuilder sb, Theme theme, string variant ) { }
    protected override void GenerateButtonVariantStyles( StringBuilder sb, Theme theme, string variant, ThemeButtonOptions options ) { }
    protected override void GenerateButtonOutlineVariantStyles( StringBuilder sb, Theme theme, string variant, ThemeButtonOptions buttonOptions ) { }
    protected override void GenerateButtonStyles( StringBuilder sb, Theme theme, ThemeButtonOptions options ) { }
    protected override void GenerateDropdownStyles( StringBuilder sb, Theme theme, ThemeDropdownOptions options ) { }
    protected override void GenerateInputStyles( StringBuilder sb, Theme theme, ThemeInputOptions options ) { }
    protected override void GenerateInputVariantStyles( StringBuilder sb, Theme theme, string variant, string inColor ) { }
    protected override void GenerateBadgeVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor ) { }
    protected override void GenerateSwitchVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, ThemeSwitchOptions options ) { }
    protected override void GenerateStepsStyles( StringBuilder sb, Theme theme, ThemeStepsOptions stepsOptions ) { }
    protected override void GenerateStepsVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, ThemeStepsOptions stepsOptions ) { }
    protected override void GenerateRatingStyles( StringBuilder sb, Theme theme, ThemeRatingOptions ratingOptions ) { }
    protected override void GenerateRatingVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, ThemeRatingOptions ratingOptions ) { }
    protected override void GenerateAlertVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, string inBorderColor, string inColor, ThemeAlertOptions options ) { }
    protected override void GenerateTableVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, string inBorderColor ) { }
    protected override void GenerateCardStyles( StringBuilder sb, Theme theme, ThemeCardOptions options ) { }
    protected override void GenerateModalStyles( StringBuilder sb, Theme theme, ThemeModalOptions options ) { }
    protected override void GenerateTabsStyles( StringBuilder sb, Theme theme, ThemeTabsOptions options ) { }
    protected override void GenerateProgressStyles( StringBuilder sb, Theme theme, ThemeProgressOptions options ) { }
    protected override void GenerateAlertStyles( StringBuilder sb, Theme theme, ThemeAlertOptions options ) { }
    protected override void GenerateBreadcrumbStyles( StringBuilder sb, Theme theme, ThemeBreadcrumbOptions options ) { }
    protected override void GenerateBadgeStyles( StringBuilder sb, Theme theme, ThemeBadgeOptions options ) { }
    protected override void GeneratePaginationStyles( StringBuilder sb, Theme theme, ThemePaginationOptions options ) { }
    protected override void GenerateBarStyles( StringBuilder sb, Theme theme, ThemeBarOptions options ) { }
    protected override void GenerateParagraphVariantStyles( StringBuilder sb, Theme theme, string variant, string inTextColor ) { }
    protected override void GenerateListGroupItemStyles( StringBuilder sb, Theme theme, ThemeListGroupItemOptions options ) { }
    protected override void GenerateListGroupItemVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, string inColor, ThemeListGroupItemOptions options ) { }
    protected override void GenerateSpacingStyles( StringBuilder sb, Theme theme, ThemeSpacingOptions options ) { }

    #endregion
}
