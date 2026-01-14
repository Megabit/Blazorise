#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Blazorise.Extensions;
#endregion

namespace Blazorise.Bulma.Providers;

public class BulmaThemeGenerator : ThemeGenerator
{
    #region Members

    private static readonly HashSet<string> BulmaThemeColors = new( StringComparer.OrdinalIgnoreCase )
    {
        "primary",
        "link",
        "info",
        "success",
        "warning",
        "danger",
        "light",
        "dark",
    };

    #endregion

    #region Constructors

    public BulmaThemeGenerator( IThemeCache themeCache )
        : base( themeCache )
    {
    }

    #endregion

    #region Methods

    public override string GenerateVariables( Theme theme )
    {
        if ( ThemeCache.TryGetVariablesFromCache( theme, out var cachedVariables ) )
            return cachedVariables;

        var baseVariables = base.GenerateVariables( theme );
        var bulmaVariables = GenerateBulmaVariables( theme );
        var generatedVariables = string.Concat( baseVariables, bulmaVariables );

        ThemeCache.CacheVariables( theme, generatedVariables );

        return generatedVariables;
    }

    private static string GenerateBulmaVariables( Theme theme )
    {
        var sb = new StringBuilder();

        foreach ( var (name, color) in theme.ValidColors )
        {
            if ( !BulmaThemeColors.Contains( name ) )
                continue;

            var parsed = ParseColor( color );

            if ( parsed.IsEmpty )
                continue;

            var baseColor = parsed.A < 255 ? Rgba2Rgb( System.Drawing.Color.White, parsed ) : parsed;
            var baseHex = ToHex( baseColor );
            var hsl = HexStringToHslColor( baseHex );
            var hue = Math.Round( hsl.Hue );
            var saturation = Math.Round( hsl.Saturation );
            var luminosity = Math.Round( hsl.Luminosity );
            var invertLuminosity = GetBulmaInvertLightness( baseColor );

            sb.AppendLine( $"--bulma-{name}: hsla(var(--bulma-{name}-h), var(--bulma-{name}-s), var(--bulma-{name}-l), 1);" );
            sb.AppendLine( $"--bulma-{name}-base: hsla(var(--bulma-{name}-h), var(--bulma-{name}-s), var(--bulma-{name}-l), 1);" );
            sb.AppendLine( $"--bulma-{name}-rgb: {baseColor.R.ToString( CultureInfo.InvariantCulture )}, {baseColor.G.ToString( CultureInfo.InvariantCulture )}, {baseColor.B.ToString( CultureInfo.InvariantCulture )};" );
            sb.AppendLine( $"--bulma-{name}-h: {hue.ToString( CultureInfo.InvariantCulture )}deg;" );
            sb.AppendLine( $"--bulma-{name}-s: {saturation.ToString( CultureInfo.InvariantCulture )}%;" );
            sb.AppendLine( $"--bulma-{name}-l: {luminosity.ToString( CultureInfo.InvariantCulture )}%;" );
            sb.AppendLine( $"--bulma-{name}-invert: hsla(var(--bulma-{name}-h), var(--bulma-{name}-s), var(--bulma-{name}-invert-l), 1);" );
            sb.AppendLine( $"--bulma-{name}-invert-l: {invertLuminosity.ToString( CultureInfo.InvariantCulture )}%;" );
        }

        return sb.ToString();
    }

    private static int GetBulmaInvertLightness( System.Drawing.Color baseColor )
    {
        var baseHex = ToHex( baseColor );
        var baseHsl = HexStringToHslColor( baseHex );
        var hue = (int)Math.Round( baseHsl.Hue );
        var saturation = (int)Math.Round( baseHsl.Saturation );
        var lightness = (int)Math.Round( baseHsl.Luminosity );

        var lBase = lightness % 10;
        var l0 = 0;
        var l5 = 5;

        if ( lBase < 3 )
        {
            l0 = lBase;
            l5 = lBase + 5;
        }
        else if ( lBase < 8 )
        {
            l0 = lBase - 5;
            l5 = lBase;
        }
        else
        {
            l0 = lBase - 10;
            l5 = lBase - 5;
        }

        var shades = new List<(string Digits, int Lightness)>();

        for ( int i = 0; i <= 9; i++ )
        {
            var colorL0 = Math.Max( l0 + ( i * 10 ), 0 );
            var colorL5 = l5 + ( i * 10 );
            var digits0 = $"{i}0";
            var digits5 = $"{i}5";

            shades.Add( (digits0, colorL0) );
            shades.Add( (digits5, colorL5) );

        }

        var lightness100 = Math.Min( l0 + 100, 100 );
        shades.Add( ("100", lightness100) );

        var baseLum = BulmaColorLuminance( baseColor );
        var baseIsLight = baseLum > 0.55;
        var selectedDigits = baseIsLight ? "10" : "100";
        var found = false;

        foreach ( var shade in shades )
        {
            var shadeHsl = new HslColor( hue, saturation, shade.Lightness );
            var shadeColor = shadeHsl.ToColor();
            var shadeLum = BulmaColorLuminance( shadeColor );
            var isLightForeground = shade.Lightness > lightness;

            var ratio = isLightForeground
                ? ( shadeLum + 0.05 ) / ( baseLum + 0.05 )
                : ( baseLum + 0.05 ) / ( shadeLum + 0.05 );

            if ( ratio > 7 )
            {
                if ( isLightForeground )
                {
                    if ( !found )
                    {
                        selectedDigits = shade.Digits;
                        found = true;
                    }
                }
                else
                {
                    selectedDigits = shade.Digits;
                }
            }
        }

        foreach ( var shade in shades )
        {
            if ( shade.Digits == selectedDigits )
                return shade.Lightness;
        }

        return baseIsLight ? 10 : 100;
    }

    private static double BulmaColorLuminance( System.Drawing.Color color )
    {
        double r = color.R / 255d;
        double g = color.G / 255d;
        double b = color.B / 255d;

        r = r < 0.03928 ? r / 12.92 : Math.Pow( ( r + 0.055 ) / 1.055, 2 );
        g = g < 0.03928 ? g / 12.92 : Math.Pow( ( g + 0.055 ) / 1.055, 2 );
        b = b < 0.03928 ? b / 12.92 : Math.Pow( ( b + 0.055 ) / 1.055, 2 );

        return ( r * 0.2126 ) + ( g * 0.7152 ) + ( b * 0.0722 );
    }

    protected override void GenerateBackgroundVariantStyles( StringBuilder sb, Theme theme, string variant )
    {
        sb.Append( $".has-background-{variant}" ).Append( "{" )
            .Append( $"background-color: {Var( ThemeVariables.BackgroundColor( variant ) )} !important;" )
            .AppendLine( "}" );

        sb.Append( $".hero.is-{variant}" ).Append( "{" )
            .Append( $"background-color: {Var( ThemeVariables.BackgroundColor( variant ) )} !important;" )
            .Append( $"color: {ToHex( Contrast( theme, Var( ThemeVariables.BackgroundColor( variant ) ) ) )} !important;" )
            .AppendLine( "}" );
    }

    protected override void GenerateBorderVariantStyles( StringBuilder sb, Theme theme, string variant )
    {
        sb.Append( $".has-border-{variant}" ).Append( "{" )
            .Append( $"border-color: {Var( ThemeVariables.BackgroundColor( variant ) )} !important;" )
            .AppendLine( "}" );
    }

    protected override void GenerateButtonVariantStyles( StringBuilder sb, Theme theme, string variant, ThemeButtonOptions options )
    {
        if ( BulmaThemeColors.Contains( variant ) )
            return;

        var background = Var( ThemeVariables.ButtonBackground( variant ) );
        var border = Var( ThemeVariables.ButtonBorder( variant ) );
        var hoverBackground = Var( ThemeVariables.ButtonHoverBackground( variant ) );
        var hoverBorder = Var( ThemeVariables.ButtonHoverBorder( variant ) );
        var activeBackground = Var( ThemeVariables.ButtonActiveBackground( variant ) );
        var activeBorder = Var( ThemeVariables.ButtonActiveBorder( variant ) );
        var yiqBackground = Var( ThemeVariables.ButtonYiqBackground( variant ) );
        var yiqHoverBackground = Var( ThemeVariables.ButtonYiqHoverBackground( variant ) );
        var yiqActiveBackground = Var( ThemeVariables.ButtonYiqActiveBackground( variant ) );
        var boxShadow = Var( ThemeVariables.ButtonBoxShadow( variant ) );

        sb.Append( $".button.is-{variant}" ).Append( "{" )
            //.Append( $"color: {yiqBackground};" )
            .Append( GetGradientBg( theme, background, options?.GradientBlendPercentage ) )
            .Append( "border-color: transparent;" )
            .AppendLine( "}" );

        sb.Append( $".button.is-{variant}:hover," ).Append( $".button.is-{variant}.is-hovered" ).Append( "{" )
            .Append( $"color: {yiqHoverBackground};" )
            .Append( GetGradientBg( theme, hoverBackground, options?.GradientBlendPercentage ) )
            .Append( $"border-color: {hoverBorder};" )
            .AppendLine( "}" );

        sb.Append( $".button.is-{variant}:focus," ).Append( $".button.is-{variant}.is-focused" ).Append( "{" )
            .Append( $"color: {yiqHoverBackground};" )
            .Append( GetGradientBg( theme, hoverBackground, options?.GradientBlendPercentage ) )
            .Append( "border-color: transparent;" )
            .AppendLine( "}" );

        sb.Append( $".button.is-{variant}:focus:not(:active)," ).Append( $".button.is-{variant}.is-focused:not(:active)" ).Append( "{" )
            .Append( $"box-shadow: 0 0 0 {options?.BoxShadowSize ?? ".125rem"} {boxShadow};" )
            .AppendLine( "}" );

        sb.Append( $".button.is-{variant}[disabled]," ).Append( $"fieldset[disabled] .button.is-{variant}" ).Append( "{" )
            .Append( $"color: {yiqBackground};" )
            .Append( $"background-color: {background};" )
            .Append( $"border-color: {border};" )
            .Append( "box-shadow: none;" )
            .AppendLine( "}" );

        sb
            .Append( $".button.is-{variant}:active," ).Append( $".button.is-{variant}.is-active" ).Append( "{" )
            .Append( $"color: {yiqActiveBackground};" )
            .Append( $"background-color: {activeBackground};" )
            .Append( $"border-color: {activeBorder};" )
            .AppendLine( "}" );

        sb
            .Append( $".button.is-{variant}.is-loading::after" ).Append( "{" )
            .Append( $"border-color: transparent transparent {activeBorder} {activeBorder};" )
            .AppendLine( "}" );

        //sb
        //    .Append( $".btn-{variant}:not(:disabled):not(.disabled):active:focus," )
        //    .Append( $".btn-{variant}:not(:disabled):not(.disabled).active:focus," )
        //    .Append( $".show>.btn-{variant}.dropdown-toggle:focus" )
        //    .Append( "{" )
        //    .Append( $"box-shadow: 0 0 0 {options?.BoxShadowSize ?? ".2rem"} {boxShadow}" )
        //    .AppendLine( "}" );
    }

    protected override void GenerateButtonOutlineVariantStyles( StringBuilder sb, Theme theme, string variant, ThemeButtonOptions buttonOptions )
    {
        if ( BulmaThemeColors.Contains( variant ) )
            return;

        var color = Var( ThemeVariables.OutlineButtonColor( variant ) );
        var yiqColor = Var( ThemeVariables.OutlineButtonYiqColor( variant ) );
        //var hoverColor = Var( ThemeVariables.OutlineButtonHoverColor( variant ) );
        //var activeColor = Var( ThemeVariables.OutlineButtonActiveColor( variant ) );

        sb.Append( $".button.is-{variant}.is-outlined" ).Append( "{" )
            .Append( $"color: {color};" )
            .Append( $"border-color: {color};" )
            .Append( "background-color: transparent;" )
            .Append( "background: transparent;" )
            .AppendLine( "}" );

        sb.Append( $".button.is-{variant}.is-outlined:hover," )
            .Append( $".button.is-{variant}.is-outlined.is-hovered," )
            .Append( $".button.is-{variant}.is-outlined:focus," )
            .Append( $".button.is-{variant}.is-outlined.is-focused" ).Append( "{" )
            .Append( $"color: {yiqColor};" )
            .Append( $"background-color: {color};" )
            .Append( $"border-color: {color};" )
            .AppendLine( "}" );

        sb.Append( $".button.is-{variant}.is-outlined.is-loading::after" ).Append( "{" )
            .Append( "border-color: transparent transparent white white;" )
            .AppendLine( "}" );

        sb.Append( $".button.is-{variant}.is-outlined[disabled]," ).Append( $"fieldset[disabled] .button.is-{variant}.is-outlined" ).Append( "{" )
            .Append( $"color: {color};" )
            .Append( "background-color: transparent;" )
            .Append( $"border-color: {color};" )
            .Append( "box-shadow: none;" )
            .AppendLine( "}" );
    }

    protected override void GenerateButtonStyles( StringBuilder sb, Theme theme, ThemeButtonOptions options )
    {
        if ( !string.IsNullOrEmpty( options?.BorderRadius ) )
        {
            sb.Append( ".button" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );
        }

        if ( !string.IsNullOrEmpty( options?.SmallBorderRadius ) )
        {
            sb.Append( ".button.is-small" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.SmallBorderRadius, Var( ThemeVariables.BorderRadiusSmall ) )};" )
                .AppendLine( "}" );
        }

        if ( !string.IsNullOrEmpty( options?.LargeBorderRadius ) )
        {
            sb.Append( ".button.is-large" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.LargeBorderRadius, Var( ThemeVariables.BorderRadiusLarge ) )};" )
                .AppendLine( "}" );
        }

        if ( !string.IsNullOrEmpty( options?.Padding ) )
            sb.Append( ".button" ).Append( "{" )
                .Append( $"padding: {options.Padding};" )
                .AppendLine( "}" );

        if ( !string.IsNullOrEmpty( options?.Margin ) )
            sb.Append( ".button" ).Append( "{" )
                .Append( $"margin: {options.Margin};" )
                .AppendLine( "}" );

        if ( options?.DisabledOpacity != null )
            sb.Append( ".button[disabled], fieldset[disabled] .button" ).Append( "{" )
                .Append( $"opacity: {options.DisabledOpacity.ToCultureInvariantString()};" )
                .AppendLine( "}" );
    }

    protected override void GenerateDropdownStyles( StringBuilder sb, Theme theme, ThemeDropdownOptions options )
    {
        if ( !string.IsNullOrEmpty( options?.BorderRadius ) )
        {
            sb.Append( ".dropdown-content" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );
        }
    }

    protected override void GenerateInputStyles( StringBuilder sb, Theme theme, ThemeInputOptions options )
    {
        if ( !string.IsNullOrEmpty( options?.BorderRadius ) )
        {
            sb.Append( ".input" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );

            sb.Append( ".select select" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );

            sb.Append( ".textarea" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );

            sb.Append( ".b-is-autocomplete.b-is-autocomplete-multipleselection" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );
        }

        if ( !string.IsNullOrEmpty( options?.CheckColor ) )
        {
            GenerateInputCheckEditStyles( sb, theme, options );
        }

        var validationSuccessColor = Var( ThemeVariables.Color( "success" ) );
        var validationDangerColor = Var( ThemeVariables.Color( "danger" ) );

        if ( !string.IsNullOrEmpty( validationSuccessColor ) )
        {
            var validationSuccessShadow = ToHexRGBA( Transparency( validationSuccessColor, 64 ) );

            sb.Append( ".input.is-success," )
                .Append( ".textarea.is-success," )
                .Append( ".select.is-success select" )
                .Append( "{" )
                .Append( $"border-color: {validationSuccessColor};" )
                .AppendLine( "}" );

            sb.Append( ".input.is-success:focus," )
                .Append( ".textarea.is-success:focus," )
                .Append( ".select.is-success select:focus" )
                .Append( "{" )
                .Append( $"border-color: {validationSuccessColor};" )
                .Append( $"box-shadow: 0 0 0 0.125em {validationSuccessShadow};" )
                .AppendLine( "}" );

            sb.Append( ".help.is-success" ).Append( "{" )
                .Append( $"color: {validationSuccessColor};" )
                .AppendLine( "}" );

            sb.Append( ".b-is-autocomplete.is-success" ).Append( "{" )
                .Append( $"border-color: {validationSuccessColor};" )
                .AppendLine( "}" );

            sb.Append( ".b-is-autocomplete.is-success.focus" ).Append( "{" )
                .Append( $"border-color: {validationSuccessColor};" )
                .Append( $"box-shadow: 0 0 0 0.125em {validationSuccessShadow};" )
                .AppendLine( "}" );
        }

        if ( !string.IsNullOrEmpty( validationDangerColor ) )
        {
            var validationDangerShadow = ToHexRGBA( Transparency( validationDangerColor, 64 ) );

            sb.Append( ".input.is-danger," )
                .Append( ".textarea.is-danger," )
                .Append( ".select.is-danger select" )
                .Append( "{" )
                .Append( $"border-color: {validationDangerColor};" )
                .AppendLine( "}" );

            sb.Append( ".input.is-danger:focus," )
                .Append( ".textarea.is-danger:focus," )
                .Append( ".select.is-danger select:focus" )
                .Append( "{" )
                .Append( $"border-color: {validationDangerColor};" )
                .Append( $"box-shadow: 0 0 0 0.125em {validationDangerShadow};" )
                .AppendLine( "}" );

            sb.Append( ".help.is-danger" ).Append( "{" )
                .Append( $"color: {validationDangerColor};" )
                .AppendLine( "}" );

            sb.Append( ".b-is-autocomplete.is-danger" ).Append( "{" )
                .Append( $"border-color: {validationDangerColor};" )
                .AppendLine( "}" );

            sb.Append( ".b-is-autocomplete.is-danger.focus" ).Append( "{" )
                .Append( $"border-color: {validationDangerColor};" )
                .Append( $"box-shadow: 0 0 0 0.125em {validationDangerShadow};" )
                .AppendLine( "}" );
        }

        if ( !string.IsNullOrEmpty( theme.ColorOptions?.Primary ) )
        {
            sb
                .Append( ".flatpickr-months .flatpickr-month:hover svg," )
                .Append( ".flatpickr-months .flatpickr-next-month:hover svg," )
                .Append( ".flatpickr-months .flatpickr-prev-month:hover svg" )
                .Append( "{" )
                .Append( $"fill: {Var( ThemeVariables.Color( "primary" ) )} !important;" )
                .AppendLine( "}" );

            sb
                .Append( ".flatpickr-day.selected, .flatpickr-day.startRange, .flatpickr-day.endRange, .flatpickr-day.selected.inRange, .flatpickr-day.startRange.inRange, .flatpickr-day.endRange.inRange, .flatpickr-day.selected:focus, .flatpickr-day.startRange:focus, .flatpickr-day.endRange:focus, .flatpickr-day.selected:hover, .flatpickr-day.startRange:hover, .flatpickr-day.endRange:hover, .flatpickr-day.selected.prevMonthDay, .flatpickr-day.startRange.prevMonthDay, .flatpickr-day.endRange.prevMonthDay, .flatpickr-day.selected.nextMonthDay, .flatpickr-day.startRange.nextMonthDay, .flatpickr-day.endRange.nextMonthDay" ).Append( "{" )
                .Append( $"background: {Var( ThemeVariables.Color( "primary" ) )};" )
                .Append( $"border-color: {Var( ThemeVariables.Color( "primary" ) )};" )
                .AppendLine( "}" );

            sb
                .Append( ".flatpickr-day:hover" ).Append( "{" )
                .Append( $"background: {ToHex( Lighten( Var( ThemeVariables.Color( "primary" ) ), 90f ) )};" )
                .AppendLine( "}" );

            sb
                .Append( ".flatpickr-day.selected.startRange + .endRange:not(:nth-child(7n+1)), .flatpickr-day.startRange.startRange + .endRange:not(:nth-child(7n+1)), .flatpickr-day.endRange.startRange + .endRange:not(:nth-child(7n+1))" ).Append( "{" )
                .Append( $"box-shadow: -10px 0 0 {Var( ThemeVariables.Color( "primary" ) )};" )
                .AppendLine( "}" );

            sb
                .Append( ".flatpickr-day.today" ).Append( "{" )
                .Append( $"border-color: {Var( ThemeVariables.Color( "primary" ) )};" )
                .AppendLine( "}" );

            sb
                .Append( ".flatpickr-day.today:hover" ).Append( "{" )
                .Append( $"background: {Var( ThemeVariables.Color( "primary" ) )};" )
                .Append( $"border-color: {Var( ThemeVariables.Color( "primary" ) )};" )
                .AppendLine( "}" );

            sb
                .Append( ".flatpickr-monthSelect-month:hover,.flatpickr-monthSelect-month:focus" ).Append( "{" )
                .Append( $"background: {ToHex( Lighten( Var( ThemeVariables.Color( "primary" ) ), 90f ) )};" )
                .AppendLine( "}" );

            sb
                .Append( ".flatpickr-monthSelect-month.selected" ).Append( "{" )
                .Append( $"background: {Var( ThemeVariables.Color( "primary" ) )};" )
                .AppendLine( "}" );
        }
    }

    protected virtual void GenerateInputCheckEditStyles( StringBuilder sb, Theme theme, ThemeInputOptions options )
    {
        sb
            .Append( ".is-checkradio[type=\"checkbox\"] + label::after, .is-checkradio[type=\"checkbox\"] + label:after" ).Append( "{" )
            .Append( $"border-color: {options.CheckColor};" )
            .AppendLine( "}" );

        sb
            .Append( ".is-checkradio[type=\"radio\"] + label::after, .is-checkradio[type=\"radio\"] + label:after" ).Append( "{" )
            .Append( $"background: {options.CheckColor};" )
            .AppendLine( "}" );

        sb
            .Append( ".is-checkradio[type=\"radio\"]:hover:not([disabled]) + label::before, .is-checkradio[type=\"radio\"]:hover:not([disabled]) + label:before, .is-checkradio[type=\"checkbox\"]:hover:not([disabled]) + label::before, .is-checkradio[type=\"checkbox\"]:hover:not([disabled]) + label:before" ).Append( "{" )
            .Append( $"border-color: {options.CheckColor} !important;" )
            .AppendLine( "}" );
    }

    protected override void GenerateBadgeVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor )
    {
        var backgroundColor = ParseColor( inBackgroundColor );

        if ( backgroundColor.IsEmpty )
            return;

        var yiqBackgroundColor = Contrast( theme, backgroundColor );

        var background = ToHex( backgroundColor );
        var yiqBackground = ToHex( yiqBackgroundColor );

        sb.Append( $".tag:not(body).is-{variant}" ).Append( "{" )
            .Append( $"color: {yiqBackground};" )
            .Append( $"background-color: {background};" )
            .AppendLine( "}" );
    }

    protected override void GenerateSwitchVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, ThemeSwitchOptions options )
    {
        var backgroundColor = ParseColor( inBackgroundColor );

        if ( backgroundColor.IsEmpty )
            return;

        //var boxShadowColor = Lighten( backgroundColor, options?.BoxShadowLightenColor ?? 25 );
        var disabledBackgroundColor = Lighten( backgroundColor, options?.DisabledLightenColor ?? 50 );

        var background = ToHex( backgroundColor );
        //var boxShadow = ToHex( boxShadowColor );
        var disabledBackground = ToHex( disabledBackgroundColor );

        sb
            .Append( $".switch[type=\"checkbox\"].is-{variant}:checked + label::before," )
            .Append( $".switch[type=\"checkbox\"].is-{variant}:checked + label:before" ).Append( "{" )
            .Append( $"background-color: {background};" )
            .AppendLine( "}" );

        sb
            .Append( $".switch[type=\"checkbox\"]:disabled.is-{variant}:checked + label::before" ).Append( "{" )
            .Append( $"background-color: {disabledBackground};" )
            .AppendLine( "}" );
    }

    protected override void GenerateStepsStyles( StringBuilder sb, Theme theme, ThemeStepsOptions stepsOptions )
    {
        if ( stepsOptions is null )
            return;

        sb
            .Append( ".steps .step-item.is-completed::before" ).Append( "{" )
            .Append( "background-position: left bottom;" )
            .AppendLine( "}" );

        sb
            .Append( ".steps .step-item.is-completed .step-marker" ).Append( "{" )
            .Append( $"color: {Var( ThemeVariables.White )};" )
            .Append( $"background-color: {Var( ThemeVariables.StepsItemIconCompleted, Var( ThemeVariables.Color( "success" ) ) )};" )
            .AppendLine( "}" );

        sb
            .Append( ".steps .step-item.is-active.is-completed .step-marker," )
            .Append( ".steps .step-item.is-active .step-marker" ).Append( "{" )
            .Append( $"color: {Var( ThemeVariables.White )};" )
            .Append( $"background-color: {Var( ThemeVariables.StepsItemIconActive, Var( ThemeVariables.Color( "primary" ) ) )};" )
            .AppendLine( "}" );

        sb
            .Append( ".steps .step-item.is-active::before" ).Append( "{" )
            .Append( $"background: {Var( ThemeVariables.StepsItemIconActive, Var( ThemeVariables.Color( "primary" ) ) )};" )
            .AppendLine( "}" );
    }

    protected override void GenerateStepsVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, ThemeStepsOptions stepsOptions )
    {
        if ( stepsOptions is null )
            return;

        sb
            .Append( $".steps .step-item.is-{variant}::before" ).Append( "{" )
            .Append( $"background: linear-gradient(to left, #dbdbdb 50%, {Var( ThemeVariables.VariantStepsItemIcon( variant ) )} 50%);" )
            .Append( "background-size: 200% 100%;" )
            .Append( "background-position: right bottom;" )
            .AppendLine( "}" );

        sb
            .Append( $".steps .step-item.is-{variant} .step-marker" ).Append( "{" )
            .Append( $"color: {Var( ThemeVariables.VariantStepsItemIconYiq( variant ) )};" )
            .Append( $"background-color: {Var( ThemeVariables.VariantStepsItemIcon( variant ) )};" )
            .AppendLine( "}" );

        sb
            .Append( $".steps .step-item.is-{variant}.is-completed::before" ).Append( "{" )
            .Append( "background-position: left bottom;" )
            .AppendLine( "}" );

        sb
            .Append( $".steps .step-item.is-{variant}.is-completed .step-marker" ).Append( "{" )
            .Append( $"color: {Var( ThemeVariables.StepsItemIconCompletedYiq )};" )
            .Append( $"background-color: {Var( ThemeVariables.StepsItemIconCompleted )};" )
            .AppendLine( "}" );

        sb
            .Append( $".steps .step-item.is-{variant}.is-active::before" ).Append( "{" )
            .Append( "background-position: left bottom;" )
            .AppendLine( "}" );

        sb
            .Append( $".steps .step-item.is-{variant}.is-active.is-completed .step-marker," )
            .Append( $".steps .step-item.is-{variant}.is-active .step-marker" ).Append( "{" )
            .Append( $"color: {Var( ThemeVariables.StepsItemIconActiveYiq, Var( ThemeVariables.White ) )};" )
            .Append( $"background-color: {Var( ThemeVariables.StepsItemIconActive, Var( ThemeVariables.Color( "primary" ) ) )};" )
            .Append( $"border-color: {Var( ThemeVariables.StepsItemIconActive, Var( ThemeVariables.Color( "primary" ) ) )};" )
            .AppendLine( "}" );
    }

    protected override void GenerateRatingStyles( StringBuilder sb, Theme theme, ThemeRatingOptions ratingOptions )
    {
        if ( ratingOptions?.HoverOpacity != null )
        {
            sb
                .Append( ".rating .rating-item.is-hover" ).Append( "{" )
                .Append( $"opacity: {string.Format( CultureInfo.InvariantCulture, "{0:F1}", ratingOptions.HoverOpacity )};" )
                .AppendLine( "}" );
        }
    }

    protected override void GenerateRatingVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, ThemeRatingOptions ratingOptions )
    {
        sb
            .Append( $".rating .rating-item.is-{variant}" ).Append( "{" )
            .Append( $"color: {Var( ThemeVariables.VariantRatingColor( variant ) )};" )
            .AppendLine( "}" );
    }

    protected override void GenerateAlertVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, string inBorderColor, string inColor, ThemeAlertOptions options )
    {
        var backgroundColor = ParseColor( inBackgroundColor );
        var borderColor = ParseColor( inBorderColor );
        var textColor = ParseColor( inColor );

        var alertLinkColor = Darken( textColor, 10f );

        var background = ToHex( backgroundColor );
        var border = ToHex( borderColor );
        var text = ToHex( textColor );
        var alertLink = ToHex( alertLinkColor );

        sb.Append( $".notification.is-{variant}" ).Append( "{" )
            .Append( $"color: {text};" )
            .Append( GetGradientBg( theme, background, options?.GradientBlendPercentage ) )
            .Append( $"border-color: {border};" )
            .AppendLine( "}" );
    }

    protected override void GenerateTableVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, string inBorderColor )
    {
        var backgroundColor = ParseColor( inBackgroundColor );
        var hoverBackgroundColor = Darken( backgroundColor, 5 );
        var borderColor = ParseColor( inBorderColor );

        var background = ToHex( backgroundColor );
        var hoverBackground = ToHex( hoverBackgroundColor );
        var border = ToHex( borderColor );

        sb.Append( $".table td.is-{variant}," )
            .Append( $".table th.is-{variant}" )
            .Append( "{" )
            .Append( $"background-color: {background};" )
            .Append( $"border-color: {border};" )
            .AppendLine( "}" );

        //sb.Append( $".table-{variant} th," )
        //    .Append( $".table-{variant} td," )
        //    .Append( $".table-{variant} thead td," )
        //    .Append( $".table-{variant} tbody + tbody," )
        //    .Append( "{" )
        //    .Append( $"border-color: {border};" )
        //    .AppendLine( "}" );

        //sb.Append( $".table-hover table-{variant}:hover" )
        //    .Append( "{" )
        //    .Append( $"background-color: {hoverBackground};" )
        //    .AppendLine( "}" );

        //sb.Append( $".table-hover table-{variant}:hover>td" )
        //    .Append( $".table-hover table-{variant}:hover>th" )
        //    .Append( "{" )
        //    .Append( $"background-color: {hoverBackground};" )
        //    .AppendLine( "}" );
    }

    protected override void GenerateCardStyles( StringBuilder sb, Theme theme, ThemeCardOptions options )
    {
        //if ( !string.IsNullOrEmpty( options.BorderRadius ) )
        //    sb.Append( $".card" ).Append( "{" )
        //        .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius )};" )
        //        .AppendLine( "}" );

        //if ( !string.IsNullOrEmpty( options.ImageTopRadius ) )
        //    sb.Append( $".card-image-top" ).Append( "{" )
        //        .Append( $"border-top-left-radius: {options.ImageTopRadius};" )
        //        .Append( $"border-top-right-radius: {options.ImageTopRadius};" )
        //        .AppendLine( "}" );
    }

    protected override void GenerateModalStyles( StringBuilder sb, Theme theme, ThemeModalOptions options )
    {
        if ( !string.IsNullOrEmpty( options?.BorderRadius ) )
        {
            sb.Append( ".modal-card-head" ).Append( "{" )
                .Append( $"border-top-left-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .Append( $"border-top-right-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );

            sb.Append( ".modal-card-foot" ).Append( "{" )
                .Append( $"border-bottom-left-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .Append( $"border-bottom-right-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );
        }
    }

    protected override void GenerateTabsStyles( StringBuilder sb, Theme theme, ThemeTabsOptions options )
    {
        if ( !string.IsNullOrEmpty( theme.ColorOptions?.Primary ) )
        {
            sb
                .Append( ".tabs.is-toggle li.is-active a" )
                .Append( "{" )
                .Append( $"background-color: {Var( ThemeVariables.Color( "primary" ) )};" )
                .AppendLine( "}" );
        }
    }

    protected override void GenerateProgressStyles( StringBuilder sb, Theme theme, ThemeProgressOptions options )
    {
        if ( !string.IsNullOrEmpty( options?.BorderRadius ) )
        {
            sb.Append( ".progress" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );
        }

        base.GenerateProgressStyles( sb, theme, options );
    }

    protected override void GenerateAlertStyles( StringBuilder sb, Theme theme, ThemeAlertOptions options )
    {
        if ( !string.IsNullOrEmpty( options?.BorderRadius ) )
        {
            sb.Append( ".notification" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );
        }
    }

    protected override void GenerateBreadcrumbStyles( StringBuilder sb, Theme theme, ThemeBreadcrumbOptions options )
    {
        if ( !string.IsNullOrEmpty( Var( ThemeVariables.BreadcrumbColor ) ) )
        {
            sb.Append( ".breadcrumb a" ).Append( "{" )
                .Append( $"color: {Var( ThemeVariables.BreadcrumbColor )};" )
                .AppendLine( "}" );
        }
    }

    protected override void GenerateBadgeStyles( StringBuilder sb, Theme theme, ThemeBadgeOptions options )
    {
        if ( !string.IsNullOrEmpty( options?.BorderRadius ) )
        {
            sb.Append( ".tag:not(body)" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );
        }
    }

    protected override void GeneratePaginationStyles( StringBuilder sb, Theme theme, ThemePaginationOptions options )
    {
        if ( !string.IsNullOrEmpty( options?.BorderRadius ) )
        {
            sb.Append( ".pagination-link,.pagination-previous,.pagination-next" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );
        }

        if ( !string.IsNullOrEmpty( options?.LargeBorderRadius ) )
        {
            sb.Append( ".pagination.is-large .pagination-link,.pagination-previous,.pagination-next" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.LargeBorderRadius, Var( ThemeVariables.BorderRadiusLarge ) )};" )
                .AppendLine( "}" );
        }

        if ( !string.IsNullOrEmpty( theme.ColorOptions?.Primary ) )
        {
            sb.Append( ".pagination-link.is-current,.pagination-previous.is-current,.pagination-next.is-current" ).Append( "{" )
                .Append( $"background-color: {Var( ThemeVariables.Color( "primary" ) )};" )
                .Append( $"border-color: {Var( ThemeVariables.Color( "primary" ) )};" )
                .AppendLine( "}" );
        }
    }

    protected override void GenerateBarStyles( StringBuilder sb, Theme theme, ThemeBarOptions options )
    {
        if ( options is null )
            return;

        foreach ( var (variant, _) in theme?.ValidBackgroundColors )
        {
            var yiqColor = Var( ThemeVariables.BackgroundYiqColor( variant ) );

            if ( string.IsNullOrEmpty( yiqColor ) )
                continue;

            sb.Append( $".navbar.has-background-{variant} .navbar-brand .navbar-item a.navbar-item" ).Append( "{" )
                .Append( $"color: {yiqColor};" )
                .AppendLine( "}" );
        }
    }

    protected override void GenerateParagraphVariantStyles( StringBuilder sb, Theme theme, string variant, string inTextColor )
    {
        var textColor = variant == "body" && !string.IsNullOrEmpty( theme.BodyOptions?.TextColor )
            ? ParseColor( theme.BodyOptions.TextColor )
            : ParseColor( inTextColor );

        var hexTextColor = ToHex( textColor );

        sb.Append( $".has-text-{variant}" )
            .Append( "{" )
            .Append( $"color: {hexTextColor};" )
            .AppendLine( "}" );
    }

    protected override void GenerateInputVariantStyles( StringBuilder sb, Theme theme, string variant, string inColor )
    {
        var color = ToHex( ParseColor( inColor ) );

        sb.Append( $".input.is-{variant}" )
            .Append( "{" )
            .Append( $"color: {color};" )
            .AppendLine( "}" );
    }

    protected override void GenerateListGroupItemStyles( StringBuilder sb, Theme theme, ThemeListGroupItemOptions options )
    {
        if ( !string.IsNullOrEmpty( theme?.ColorOptions?.Primary ) )
        {
            var white = Var( ThemeVariables.White );
            var primary = Var( ThemeVariables.Color( "primary" ) );

            sb
                .Append( ".list-group-item.is-active" )
                .Append( "{" )
                .Append( $"color: {white};" )
                .Append( GetGradientBg( theme, primary, options?.GradientBlendPercentage ) )
                .Append( $"border-color: {primary};" )
                .AppendLine( "}" );
        }
    }

    protected override void GenerateListGroupItemVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, string inColor, ThemeListGroupItemOptions options )
    {
        var backgroundColor = ParseColor( inBackgroundColor );
        var textColor = ParseColor( inColor );

        var background = ToHex( backgroundColor );
        var text = ToHex( textColor );

        sb.Append( $".list-group > .list-group-item.is-{variant}" ).Append( "{" )
            .Append( $"color: {text};" )
            .Append( GetGradientBg( theme, background, options?.GradientBlendPercentage ) )
            .AppendLine( "}" );
    }

    protected override void GenerateSpacingStyles( StringBuilder sb, Theme theme, ThemeSpacingOptions options )
    {
        if ( theme.BreakpointOptions == null || options == null )
            return;

        foreach ( var breakpoint in theme.BreakpointOptions )
        {
            var breakpointName = breakpoint.Key;
            var breakpointMin = breakpoint.Value();

            var hasMinMedia = !string.IsNullOrEmpty( breakpointMin ) && breakpointMin != "0";

            if ( hasMinMedia )
            {
                sb.Append( $"@media (min-width: {breakpointMin})" ).Append( "{" );
            }

            var infix = string.IsNullOrEmpty( breakpointMin ) || breakpointMin == "0"
                ? ""
                : $"-{breakpointName}";

            foreach ( (string prop, string abbrev) in new[] { ("margin", "m"), ("padding", "p") } )
            {
                foreach ( (string size, Func<string> lenghtFunc) in options )
                {
                    var length = lenghtFunc.Invoke();

                    sb
                        .Append( $".{abbrev}{infix}-{size}" )
                        .Append( "{" ).Append( $"{prop}: {length} !important;" ).Append( "}" );

                    sb
                        .Append( $".{abbrev}t{infix}-{size}," )
                        .Append( $".{abbrev}y{infix}-{size}" )
                        .Append( "{" ).Append( $"{prop}-top: {length} !important;" ).Append( "}" );

                    sb
                        .Append( $".{abbrev}r{infix}-{size}," )
                        .Append( $".{abbrev}x{infix}-{size}" )
                        .Append( "{" ).Append( $"{prop}-right: {length} !important;" ).Append( "}" );

                    sb
                        .Append( $".{abbrev}b{infix}-{size}," )
                        .Append( $".{abbrev}y{infix}-{size}" )
                        .Append( "{" ).Append( $"{prop}-bottom: {length} !important;" ).Append( "}" );

                    sb
                        .Append( $".{abbrev}l{infix}-{size}," )
                        .Append( $".{abbrev}x{infix}-{size}" )
                        .Append( "{" ).Append( $"{prop}-left: {length} !important;" ).Append( "}" );
                }
            }

            foreach ( (string size, Func<string> lenghtFunc) in options )
            {
                if ( string.IsNullOrEmpty( size ) || size == "0" )
                    continue;

                var length = lenghtFunc.Invoke();

                sb
                    .Append( $".m{infix}-n{size}" )
                    .Append( "{" ).Append( $"margin: -{length} !important;" ).Append( "}" );

                sb
                    .Append( $".mt{infix}-n{size}," )
                    .Append( $".my{infix}-n{size}" )
                    .Append( "{" ).Append( $"margin-top: -{length} !important;" ).Append( "}" );

                sb
                    .Append( $".mr{infix}-n{size}," )
                    .Append( $".mx{infix}-n{size}" )
                    .Append( "{" ).Append( $"margin-right: -{length} !important;" ).Append( "}" );

                sb
                    .Append( $".mb{infix}-n{size}," )
                    .Append( $".my{infix}-n{size}" )
                    .Append( "{" ).Append( $"margin-bottom: -{length} !important;" ).Append( "}" );

                sb
                    .Append( $".ml{infix}-n{size}," )
                    .Append( $".mx{infix}-n{size}" )
                    .Append( "{" ).Append( $"margin-left: -{length} !important;" ).Append( "}" );
            }

            if ( hasMinMedia )
            {
                sb.Append( "}" );
            }
        }
    }

    #endregion
}