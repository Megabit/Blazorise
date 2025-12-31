#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blazorise.Utilities;
#endregion

namespace Blazorise.Tailwind.Providers;

public class TailwindThemeGenerator : ThemeGenerator
{
    #region Constructors

    public TailwindThemeGenerator( IThemeCache themeCache )
        : base( themeCache )
    {
    }

    #endregion

    #region Methods

    protected override void GenerateBreakpointStyles( StringBuilder sb, Theme theme, string breakpointName, string breakpointSize )
    {
        if ( !string.IsNullOrEmpty( breakpointSize ) )
        {
            if ( !string.IsNullOrEmpty( theme?.ContainerMaxWidthOptions?[breakpointName]?.Invoke() ) )
            {
                var containerSize = theme.ContainerMaxWidthOptions[breakpointName].Invoke();

                sb.Append( MediaBreakpointUp( breakpointName, $".container{{max-width: {containerSize};}}" ) );
            }
        }

        base.GenerateBreakpointStyles( sb, theme, breakpointName, breakpointSize );
    }

    IReadOnlyDictionary<int, double> CreateHueScale( double tweak )
    {
        return new Dictionary<int, double>()
        {
            { 0, tweak != 0 ? tweak * 4 + tweak : 0 },
            { 50, tweak != 0 ? tweak * 3.5 + tweak : 0 },
            { 100, tweak != 0 ? tweak * 3 + tweak : 0 },
            { 200, tweak != 0 ? tweak * 2 + tweak : 0 },
            { 300, tweak != 0 ? tweak * 1 + tweak : 0 },
            { 400, tweak != 0 ? tweak + 0 : 0 },
            { 500, 0},
            { 600, tweak != 0 ? tweak : 0 },
            { 700, tweak != 0 ? tweak * 1 + tweak : 0 },
            { 800, tweak != 0 ? tweak * 2 + tweak : 0 },
            { 900, tweak != 0 ? tweak * 3 + tweak : 0 },
            { 1000, tweak != 0 ? tweak * 4 + tweak : 0 }
        };
    }

    IReadOnlyDictionary<int, double> CreateSaturationScale( double tweak )
    {
        return new Dictionary<int, double>()
        {
            { 0, Math.Round( tweak * 1.15 )},
            { 50, Math.Round( tweak * 1.125 )},
            { 100, Math.Round( tweak )},
            { 200, Math.Round( tweak * 0.75 )},
            { 300, Math.Round( tweak * 0.5 )},
            { 400, Math.Round( tweak * 0.25 )},
            { 500, 0},
            { 600, Math.Round( tweak * 0.25 )},
            { 700, Math.Round( tweak * 0.5 )},
            { 800, Math.Round( tweak * 0.75 )},
            { 900, Math.Round( tweak )},
            { 1000, Math.Round( tweak ) * 1.25}
        };
    }

    IReadOnlyDictionary<int, double> CreateDistributionValues( double? min, double? max, double lightness )
    {
        // A `0` swatch (lightest color) would have this lightness
        var maxLightness = max ?? 100;
        var maxStep = MathUtils.Round( ( maxLightness - lightness ) / 5, 2 );

        // A `1000` swatch (darkest color) would have this lightness
        var minLightness = min ?? 0;
        var minStep = MathUtils.Round( ( lightness - minLightness ) / 5, 2 );

        var values = new Dictionary<int, double>()
        {
            { 0, Math.Round( lightness + maxStep * 5 )}, // Closest to 100, lightest colour
            { 50, Math.Round( lightness + maxStep * 4.5 )},
            { 100, Math.Round( lightness + maxStep * 4 )},
            { 200, Math.Round( lightness + maxStep * 3 )},
            { 300, Math.Round( lightness + maxStep * 2 )},
            { 400, Math.Round( lightness + maxStep * 1 )},
            { 500, Math.Round( lightness )}, // Lightness of original colour
            { 600, Math.Round( lightness - minStep * 1 )},
            { 700, Math.Round( lightness - minStep * 2 )},
            { 800, Math.Round( lightness - minStep * 3 )},
            { 900, Math.Round( lightness - minStep * 4 )},
            { 1000, Math.Round( lightness - minStep * 5 )}, // Closest to 0, darkest colour
        };

        // Each 'tweak' value must be between 0 and 100
        var safeValues = values.ToDictionary( kv => kv.Key, kv => Math.Min( Math.Max( kv.Value, 0 ), 100 ) );

        return safeValues;
    }

    double LightnessFromHSL( HslColor hslColor )
    {
        var vals = new List<double>();

        for ( var index = 99; index >= 0; index-- )
        {
            vals[index] = Math.Abs( hslColor.Luminosity - LuminanceFromColor( new HslColor( hslColor.Hue, hslColor.Saturation, index ).ToColor() ) );
        }

        // Run through all these and find the closest to 0
        var lowestDiff = 100d;
        var newL = 100d;

        for ( var i = vals.Count - 1; i >= 0; i-- )
        {
            if ( vals[i] < lowestDiff )
            {
                newL = i;
                lowestDiff = vals[i];
            }
        }

        return newL;
    }

    IReadOnlyCollection<ThemeSwatch> CreateThemeSwatches( ThemeSwatchOptions swatchOptions )
    {
        var value = swatchOptions.HexColor;

        // Tweaks may be passed in, otherwise use defaults
        var useLightness = swatchOptions.UseLightness ?? true;
        var hue = swatchOptions.Hue ?? 0;
        var saturation = swatchOptions.Saturation ?? 0;
        var lightnessMin = swatchOptions.LightnessMin ?? 0;
        var lightnessMax = swatchOptions.LightnessMax ?? 100;

        // Create hue and saturation scales based on tweaks
        var hueScale = CreateHueScale( hue );
        var saturationScale = CreateSaturationScale( saturation );

        // Get the base hex's H/S/L values
        var hslColor = HexStringToHslColor( value );

        // Create lightness scales based on tweak + lightness/luminance of current value
        var lightnessValue = useLightness ? hslColor.Luminosity : LuminanceFromColor( value );
        var distributionScale = CreateDistributionValues( lightnessMin, lightnessMax, lightnessValue );

        return hueScale.Select( ( kv, i ) =>
        {
            // Hue value must be between 0-360
            // todo: fix this inside the function
            var newH = hslColor.Hue + kv.Value;
            newH = newH < 0 ? 360 + newH - 1 : newH;
            newH = newH > 720 ? newH - 360 : newH;
            newH = newH > 360 ? newH - 360 : newH;

            // Saturation must be between 0-100
            var newS = hslColor.Saturation + saturationScale.ElementAt( i ).Value;
            newS = newS > 100 ? 100 : newS;

            var newL = useLightness
               ? distributionScale.ElementAt( i ).Value
               : LightnessFromHSL( new HslColor( newH, newS, distributionScale.ElementAt( i ).Value ) );

            var newHex = ToHex( new HslColor( newH, newS, newL ) );
            var swatchKey = kv.Key;

            return new ThemeSwatch
            {
                Key = swatchKey,
                // Sometimes the initial value is changed slightly during conversion,
                // overriding that with the original value
                HexColor = swatchKey == 500 ? swatchOptions.HexColor.ToUpperInvariant() : newHex.ToUpperInvariant(),
                // Used in graphs
                Hue = newH,
                HueScale = hueScale.ElementAt( i ).Value,
                Saturation = newS,
                SaturationScale = saturationScale.ElementAt( i ).Value,
                Lightness = newL,
            };
        } ).ToList();
    }

    protected override void GenerateColorStyles( StringBuilder sb, Theme theme, string variant, string color )
    {
        base.GenerateColorStyles( sb, theme, variant, color );

        var swatches = CreateThemeSwatches( new ThemeSwatchOptions
        {
            HexColor = color
        } );

        foreach ( var swatch in swatches )
        {
            sb.Append( $".bg-{variant}-{swatch.Key}" ).Append( "{" ).Append( $"background-color: {swatch.HexColor};" ).AppendLine( "}" );
        }
    }

    protected override void GenerateTypographyVariantStyles( StringBuilder sb, Theme theme, string variant, string color )
    {
        base.GenerateTypographyVariantStyles( sb, theme, variant, color );

        var swatches = CreateThemeSwatches( new ThemeSwatchOptions
        {
            HexColor = color
        } );

        foreach ( var swatch in swatches )
        {
            sb.Append( $".text-{variant}-{swatch.Key}" ).Append( "{" ).Append( $"color: {swatch.HexColor};" ).AppendLine( "}" );
        }
    }

    protected override void GenerateBackgroundVariantStyles( StringBuilder sb, Theme theme, string variant )
    {
        //sb.Append( $".bg-{variant}" ).Append( "{" )
        //    .Append( $"background-color: {Var( ThemeVariables.BackgroundColor( variant ) )} !important;" )
        //    .AppendLine( "}" );

        //sb.Append( $".jumbotron-{variant}" ).Append( "{" )
        //    .Append( $"background-color: {Var( ThemeVariables.BackgroundColor( variant ) )} !important;" )
        //    .Append( $"color: {ToHex( Contrast( theme, Var( ThemeVariables.BackgroundColor( variant ) ) ) )} !important;" )
        //    .AppendLine( "}" );
    }

    protected override void GenerateBorderVariantStyles( StringBuilder sb, Theme theme, string variant )
    {
        //sb.Append( $".border-{variant}" ).Append( "{" )
        //    .Append( $"border-color: {Var( ThemeVariables.BackgroundColor( variant ) )} !important;" )
        //    .AppendLine( "}" );

        //for ( int i = 1; i <= 5; ++i )
        //{
        //    sb.Append( $".border-{i}.border-{variant}" ).Append( "{" )
        //        .Append( $"border-color: {Var( ThemeVariables.BackgroundColor( variant ) )} !important;" )
        //        .AppendLine( "}" );
        //}

        var validationSuccessColor = Var( ThemeVariables.Color( "success" ) );
        var validationDangerColor = Var( ThemeVariables.Color( "danger" ) );

        if ( !string.IsNullOrEmpty( validationSuccessColor ) )
        {
            sb.Append( ".b-is-autocomplete.border-green-500" ).Append( "{" )
                .Append( $"border-color: {validationSuccessColor};" )
                .AppendLine( "}" );

            sb.Append( ".b-is-autocomplete.focus.border-green-500" ).Append( "{" )
                .Append( "--tw-ring-inset: var(--tw-empty, /*!*/ /*!*/ );" )
                .Append( "--tw-ring-offset-width: 0px;" )
                .Append( "--tw-ring-offset-color: #fff;" )
                .Append( $"--tw-ring-color: {validationSuccessColor};" )
                .Append( "--tw-ring-offset-shadow: var(--tw-ring-inset) 0 0 0 var(--tw-ring-offset-width) var(--tw-ring-offset-color);" )
                .Append( "--tw-ring-shadow: var(--tw-ring-inset) 0 0 0 calc(1px + var(--tw-ring-offset-width)) var(--tw-ring-color);" )
                .Append( $"border-color: {validationSuccessColor};" )
                .Append( "box-shadow: var(--tw-ring-offset-shadow),var(--tw-ring-shadow),var(--tw-shadow);" )
                .Append( "outline: 2px solid transparent;" )
                .Append( "outline-offset: 2px;" )
                .AppendLine( "}" );
        }

        if ( !string.IsNullOrEmpty( validationDangerColor ) )
        {
            sb.Append( ".b-is-autocomplete.border-red-500" ).Append( "{" )
                .Append( $"border-color: {validationDangerColor};" )
                .AppendLine( "}" );

            sb.Append( ".b-is-autocomplete.focus.border-red-500" ).Append( "{" )
                .Append( "--tw-ring-inset: var(--tw-empty, /*!*/ /*!*/ );" )
                .Append( "--tw-ring-offset-width: 0px;" )
                .Append( "--tw-ring-offset-color: #fff;" )
                .Append( $"--tw-ring-color: {validationDangerColor};" )
                .Append( "--tw-ring-offset-shadow: var(--tw-ring-inset) 0 0 0 var(--tw-ring-offset-width) var(--tw-ring-offset-color);" )
                .Append( "--tw-ring-shadow: var(--tw-ring-inset) 0 0 0 calc(1px + var(--tw-ring-offset-width)) var(--tw-ring-color);" )
                .Append( $"border-color: {validationDangerColor};" )
                .Append( "box-shadow: var(--tw-ring-offset-shadow),var(--tw-ring-shadow),var(--tw-shadow);" )
                .Append( "outline: 2px solid transparent;" )
                .Append( "outline-offset: 2px;" )
                .AppendLine( "}" );
        }
    }

    protected override void GenerateButtonVariantStyles( StringBuilder sb, Theme theme, string variant, ThemeButtonOptions options )
    {
        //var background = Var( ThemeVariables.ButtonBackground( variant ) );
        //var border = Var( ThemeVariables.ButtonBorder( variant ) );
        //var hoverBackground = Var( ThemeVariables.ButtonHoverBackground( variant ) );
        //var hoverBorder = Var( ThemeVariables.ButtonHoverBorder( variant ) );
        //var activeBackground = Var( ThemeVariables.ButtonActiveBackground( variant ) );
        //var activeBorder = Var( ThemeVariables.ButtonActiveBorder( variant ) );
        //var yiqBackground = Var( ThemeVariables.ButtonYiqBackground( variant ) );
        //var yiqHoverBackground = Var( ThemeVariables.ButtonYiqHoverBackground( variant ) );
        //var yiqActiveBackground = Var( ThemeVariables.ButtonYiqActiveBackground( variant ) );
        //var boxShadow = Var( ThemeVariables.ButtonBoxShadow( variant ) );

        //if ( variant == "link" )
        //{
        //    sb
        //        .Append( $".b-button-{variant}" ).Append( "{" )
        //        .Append( $"color: {background};" )
        //        .AppendLine( "}" );

        //    sb.Append( $".b-button-{variant}:hover" )
        //        .Append( "{" )
        //        .Append( $"color: {hoverBackground};" )
        //        .AppendLine( "}" );

        //    sb.Append( $".b-button-{variant}.disabled," )
        //        .Append( $".b-button-{variant}:disabled" )
        //        .Append( "{" )
        //        .Append( $"color: {ToHex( Darken( background, 15f ) )};" )
        //        .AppendLine( "}" );
        //}
        //else
        //{
        //    sb.Append( $".b-button-{variant}," )
        //        .Append( $"a.button-{variant}" )
        //        .Append( "{" )
        //        .Append( $"color: {yiqBackground};" )
        //        .Append( GetGradientBg( theme, background, options?.GradientBlendPercentage ) )
        //        .AppendLine( "}" );

        //    sb.Append( $".b-button-{variant}:hover," )
        //        .Append( $"a.b-button-{variant}:hover" )
        //        .Append( "{" )
        //        .Append( $"color: {yiqHoverBackground};" )
        //        .Append( GetGradientBg( theme, hoverBackground, options?.GradientBlendPercentage ) )
        //        .AppendLine( "}" );

        //    sb.Append( $".b-button-{variant}:focus," )
        //        .Append( $".b-button-{variant}.focus," )
        //        .Append( $"a.b-button-{variant}:focus," )
        //        .Append( $"a.b-button-{variant}.focus" )
        //        .Append( "{" )
        //        .Append( $"color: {yiqHoverBackground};" )
        //        .Append( GetGradientBg( theme, hoverBackground, options?.GradientBlendPercentage ) )
        //        .Append( $"box-shadow: 0 0 0 {options?.BoxShadowSize ?? ".25rem"} {boxShadow};" )
        //        .AppendLine( "}" );

        //    sb.Append( $".b-button-{variant}.disabled," )
        //        .Append( $".b-button-{variant}:disabled," )
        //        .Append( $"a.b-button-{variant}.disabled," )
        //        .Append( $"a.b-button-{variant}:disabled" )
        //        .Append( "{" )
        //        .Append( $"color: {yiqBackground};" )
        //        .Append( $"background-color: {background};" )
        //        .AppendLine( "}" );

        //    sb
        //        .Append( $".b-button-{variant}:not(:disabled):not(.disabled):active," )
        //        .Append( $".b-button-{variant}:not(:disabled):not(.disabled).active," )
        //        .Append( $".show>.b-button-{variant}.b-dropdown-toggle," )
        //        .Append( $"a.b-button-{variant}:not(:disabled):not(.disabled):active," )
        //        .Append( $"a.b-button-{variant}:not(:disabled):not(.disabled).active," )
        //        .Append( $"a.show>.b-button-{variant}.b-dropdown-toggle" )
        //        .Append( "{" )
        //        .Append( $"color: {yiqActiveBackground};" )
        //        .Append( $"background-color: {activeBackground};" )
        //        .AppendLine( "}" );

        //    sb
        //        .Append( $".b-button-{variant}:not(:disabled):not(.disabled):active:focus," )
        //        .Append( $".b-button-{variant}:not(:disabled):not(.disabled).active:focus," )
        //        .Append( $".show>.b-button-{variant}.b-dropdown-toggle:focus," )
        //        .Append( $"a.b-button-{variant}:not(:disabled):not(.disabled):active:focus," )
        //        .Append( $"a.b-button-{variant}:not(:disabled):not(.disabled).active:focus," )
        //        .Append( $"a.show>.b-button-{variant}.b-dropdown-toggle:focus" )
        //        .Append( "{" )
        //        .Append( $"box-shadow: 0 0 0 {options?.BoxShadowSize ?? ".25rem"} {boxShadow}" )
        //        .AppendLine( "}" );
        //}
    }

    protected override void GenerateButtonOutlineVariantStyles( StringBuilder sb, Theme theme, string variant, ThemeButtonOptions options )
    {
        //var color = Var( ThemeVariables.OutlineButtonColor( variant ) );
        //var yiqColor = Var( ThemeVariables.OutlineButtonYiqColor( variant ) );
        //var boxShadow = Var( ThemeVariables.OutlineButtonBoxShadowColor( variant ) );

        //sb
        //    .Append( $".b-button-outline-{variant}," )
        //    .Append( $"a.b-button-outline-{variant}" )
        //    .Append( "{" )
        //    .Append( $"color: {color};" )
        //    .Append( $"border-color: {color};" )
        //    .AppendLine( "}" );

        //sb
        //    .Append( $".b-button-outline-{variant}:hover," )
        //    .Append( $"a.b-button-outline-{variant}:hover" )
        //    .Append( "{" )
        //    .Append( $"color: {yiqColor};" )
        //    .Append( $"background-color: {color};" )
        //    .Append( $"border-color: {color};" )
        //    .AppendLine( "}" );

        //sb
        //    .Append( $".b-button-outline-{variant}:focus," )
        //    .Append( $".b-button-outline-{variant}.focus," )
        //    .Append( $"a.b-button-outline-{variant}:focus," )
        //    .Append( $"a.b-button-outline-{variant}.focus" )
        //    .Append( "{" )
        //    .Append( $"box-shadow: 0 0 0 {options?.BoxShadowSize ?? ".25rem"} {boxShadow};" )
        //    .AppendLine( "}" );

        //sb
        //    .Append( $".b-button-outline-{variant}.disabled," )
        //    .Append( $".b-button-outline-{variant}:disabled," )
        //    .Append( $"a.b-button-outline-{variant}.disabled," )
        //    .Append( $"a.b-button-outline-{variant}:disabled" )
        //    .Append( "{" )
        //    .Append( $"color: {color};" )
        //    .Append( "background-color: transparent;" )
        //    .AppendLine( "}" );

        //sb
        //    .Append( $".b-button-outline-{variant}:not(:disabled):not(.disabled):active," )
        //    .Append( $".b-button-outline-{variant}:not(:disabled):not(.disabled).active," )
        //    .Append( $".show>.b-button-outline-{variant}.b-dropdown-toggle," )
        //    .Append( $"a.b-button-outline-{variant}:not(:disabled):not(.disabled):active," )
        //    .Append( $"a.b-button-outline-{variant}:not(:disabled):not(.disabled).active," )
        //    .Append( $"a.show>.b-button-outline-{variant}.b-dropdown-toggle" )
        //    .Append( "{" )
        //    .Append( $"color: {yiqColor};" )
        //    .Append( $"background-color: {color};" )
        //    .Append( $"border-color: {color};" )
        //    .AppendLine( "}" );

        //sb
        //    .Append( $".b-button-outline-{variant}:not(:disabled):not(.disabled):active:focus," )
        //    .Append( $".b-button-outline-{variant}:not(:disabled):not(.disabled).active:focus," )
        //    .Append( $".show>.b-button-outline-{variant}.b-dropdown-toggle:focus," )
        //    .Append( $"a.b-button-outline-{variant}:not(:disabled):not(.disabled):active:focus," )
        //    .Append( $"a.b-button-outline-{variant}:not(:disabled):not(.disabled).active:focus," )
        //    .Append( $"a.show>.b-button-outline-{variant}.b-dropdown-toggle:focus" )
        //    .Append( "{" )
        //    .Append( $"box-shadow: 0 0 0 {options?.BoxShadowSize ?? ".25rem"} {boxShadow};" )
        //    .AppendLine( "}" );
    }

    protected override void GenerateButtonStyles( StringBuilder sb, Theme theme, ThemeButtonOptions options )
    {
        //sb.Append( ".btn" ).Append( "{" )
        //    .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
        //    .AppendLine( "}" );

        //sb.Append( ".btn-sm" ).Append( "{" )
        //    .Append( $"border-radius: {GetBorderRadius( theme, options?.SmallBorderRadius, Var( ThemeVariables.BorderRadiusSmall ) )};" )
        //    .AppendLine( "}" );

        //sb.Append( ".btn-lg" ).Append( "{" )
        //    .Append( $"border-radius: {GetBorderRadius( theme, options?.LargeBorderRadius, Var( ThemeVariables.BorderRadiusLarge ) )};" )
        //    .AppendLine( "}" );

        //if ( !string.IsNullOrEmpty( options?.Padding ) )
        //    sb.Append( ".btn" ).Append( "{" )
        //        .Append( $"padding: {options.Padding};" )
        //        .AppendLine( "}" );

        //if ( !string.IsNullOrEmpty( options?.Margin ) )
        //    sb.Append( ".btn" ).Append( "{" )
        //        .Append( $"margin: {options.Margin};" )
        //        .AppendLine( "}" );

        //if ( options?.DisabledOpacity != null )
        //    sb.Append( ".btn.disabled, .btn:disabled" ).Append( "{" )
        //        .Append( $"opacity: {options.DisabledOpacity};" )
        //        .AppendLine( "}" );
    }

    protected override void GenerateDropdownStyles( StringBuilder sb, Theme theme, ThemeDropdownOptions options )
    {
        //sb.Append( ".dropdown-menu" ).Append( "{" )
        //    .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
        //    .AppendLine( "}" );

        //if ( !string.IsNullOrEmpty( theme.ColorOptions?.Primary ) )
        //{
        //    var backgroundColor = ParseColor( theme.ColorOptions.Primary );

        //    if ( !backgroundColor.IsEmpty )
        //    {
        //        var background = ToHex( backgroundColor );
        //        var color = ToHex( Contrast( theme, background ) );

        //        sb.Append( ".dropdown-item.active," )
        //            .Append( ".dropdown-item:active" ).Append( "{" )
        //            .Append( GetGradientBg( theme, background, options?.GradientBlendPercentage ) )
        //            .Append( $"color: {color} !important;" )
        //            .AppendLine( "}" );
        //    }
        //}
    }

    protected override void GenerateInputStyles( StringBuilder sb, Theme theme, ThemeInputOptions options )
    {
        //sb.Append( ".form-control" ).Append( "{" )
        //    .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
        //    .AppendLine( "}" );

        //sb.Append( ".input-group-text" ).Append( "{" )
        //    .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
        //    .AppendLine( "}" );

        //sb.Append( ".custom-select" ).Append( "{" )
        //    .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
        //    .AppendLine( "}" );

        //sb.Append( ".custom-checkbox .custom-control-label::before" ).Append( "{" )
        //    .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
        //    .AppendLine( "}" );

        //sb.Append( ".custom-file-label" ).Append( "{" )
        //    .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
        //    .AppendLine( "}" );

        //if ( !string.IsNullOrEmpty( options?.Color ) )
        //{
        //    sb.Append( ".form-control" ).Append( "{" )
        //        .Append( $"color: {options.Color};" )
        //        .AppendLine( "}" );

        //    sb.Append( ".input-group-text" ).Append( "{" )
        //        .Append( $"color: {options.Color};" )
        //        .AppendLine( "}" );

        //    sb.Append( ".custom-select" ).Append( "{" )
        //        .Append( $"color: {options.Color};" )
        //        .AppendLine( "}" );
        //}

        //if ( !string.IsNullOrEmpty( options?.CheckColor ) )
        //{
        //    GenerateInputCheckEditStyles( sb, theme, options );
        //}

        //if ( !string.IsNullOrEmpty( theme.ColorOptions?.Primary ) )
        //{
        //    var focusColor = ToHex( Lighten( Var( ThemeVariables.Color( "primary" ) ), 75f ) );

        //    sb
        //        .Append( ".form-control:focus," )
        //        .Append( ".custom-select:focus," )
        //        .Append( ".b-is-autocomplete.b-is-autocomplete-multipleselection.focus" )
        //        .Append( "{" )
        //        .Append( $"border-color: {focusColor};" )
        //        .Append( $"box-shadow: 0 0 0 {theme.ButtonOptions?.BoxShadowSize ?? ".2rem"} {focusColor};" )
        //        .AppendLine( "}" );
        //}

        //if ( !string.IsNullOrEmpty( theme.ColorOptions?.Primary ) )
        //{
        //    sb
        //        .Append( ".flatpickr-months .flatpickr-month:hover svg," )
        //        .Append( ".flatpickr-months .flatpickr-next-month:hover svg," )
        //        .Append( ".flatpickr-months .flatpickr-prev-month:hover svg" )
        //        .Append( "{" )
        //        .Append( $"fill: {Var( ThemeVariables.Color( "primary" ) )} !important;" )
        //        .AppendLine( "}" );

        //    sb
        //        .Append( ".flatpickr-day.selected, .flatpickr-day.startRange, .flatpickr-day.endRange, .flatpickr-day.selected.inRange, .flatpickr-day.startRange.inRange, .flatpickr-day.endRange.inRange, .flatpickr-day.selected:focus, .flatpickr-day.startRange:focus, .flatpickr-day.endRange:focus, .flatpickr-day.selected:hover, .flatpickr-day.startRange:hover, .flatpickr-day.endRange:hover, .flatpickr-day.selected.prevMonthDay, .flatpickr-day.startRange.prevMonthDay, .flatpickr-day.endRange.prevMonthDay, .flatpickr-day.selected.nextMonthDay, .flatpickr-day.startRange.nextMonthDay, .flatpickr-day.endRange.nextMonthDay" ).Append( "{" )
        //        .Append( $"background: {Var( ThemeVariables.Color( "primary" ) )};" )
        //        .Append( $"border-color: {Var( ThemeVariables.Color( "primary" ) )};" )
        //        .AppendLine( "}" );

        //    sb
        //        .Append( ".flatpickr-day:hover" ).Append( "{" )
        //        .Append( $"background: {ToHex( Lighten( Var( ThemeVariables.Color( "primary" ) ), 90f ) )};" )
        //        .AppendLine( "}" );

        //    sb
        //        .Append( ".flatpickr-day.selected.startRange + .endRange:not(:nth-child(7n+1)), .flatpickr-day.startRange.startRange + .endRange:not(:nth-child(7n+1)), .flatpickr-day.endRange.startRange + .endRange:not(:nth-child(7n+1))" ).Append( "{" )
        //        .Append( $"box-shadow: -10px 0 0 {Var( ThemeVariables.Color( "primary" ) )};" )
        //        .AppendLine( "}" );

        //    sb
        //        .Append( ".flatpickr-day.today" ).Append( "{" )
        //        .Append( $"border-color: {Var( ThemeVariables.Color( "primary" ) )};" )
        //        .AppendLine( "}" );

        //    sb
        //        .Append( ".flatpickr-day.today:hover" ).Append( "{" )
        //        .Append( $"background: {Var( ThemeVariables.Color( "primary" ) )};" )
        //        .Append( $"border-color: {Var( ThemeVariables.Color( "primary" ) )};" )
        //        .AppendLine( "}" );

        //    sb
        //        .Append( ".flatpickr-monthSelect-month:hover,.flatpickr-monthSelect-month:focus" ).Append( "{" )
        //        .Append( $"background: {ToHex( Lighten( Var( ThemeVariables.Color( "primary" ) ), 90f ) )};" )
        //        .AppendLine( "}" );

        //    sb
        //        .Append( ".flatpickr-monthSelect-month.selected" ).Append( "{" )
        //        .Append( $"background: {Var( ThemeVariables.Color( "primary" ) )};" )
        //        .AppendLine( "}" );
        //}

        var validationSuccessColor = Var( ThemeVariables.Color( "success" ) );
        var validationDangerColor = Var( ThemeVariables.Color( "danger" ) );

        if ( !string.IsNullOrEmpty( validationSuccessColor ) )
        {
            var validationSuccessBackground = ToHexRGBA( Transparency( validationSuccessColor, 16 ) );

            sb.Append( "input.bg-green-50," )
                .Append( "textarea.bg-green-50," )
                .Append( "select.bg-green-50" )
                .Append( "{" )
                .Append( $"background-color: {validationSuccessBackground};" )
                .AppendLine( "}" );

            sb.Append( "input.border-green-500," )
                .Append( "textarea.border-green-500," )
                .Append( "select.border-green-500" )
                .Append( "{" )
                .Append( $"border-color: {validationSuccessColor};" )
                .AppendLine( "}" );

            sb.Append( "input.text-green-900," )
                .Append( "textarea.text-green-900," )
                .Append( "select.text-green-900" )
                .Append( "{" )
                .Append( $"color: {validationSuccessColor};" )
                .AppendLine( "}" );

            sb.Append( "input.placeholder-green-700::placeholder," )
                .Append( "textarea.placeholder-green-700::placeholder" )
                .Append( "{" )
                .Append( $"color: {validationSuccessColor};" )
                .AppendLine( "}" );

            sb.Append( "input.focus\\:border-green-500:focus," )
                .Append( "textarea.focus\\:border-green-500:focus," )
                .Append( "select.focus\\:border-green-500:focus" )
                .Append( "{" )
                .Append( $"border-color: {validationSuccessColor};" )
                .AppendLine( "}" );

            sb.Append( "input.focus\\:ring-green-500:focus," )
                .Append( "textarea.focus\\:ring-green-500:focus," )
                .Append( "select.focus\\:ring-green-500:focus" )
                .Append( "{" )
                .Append( $"--tw-ring-color: {validationSuccessColor};" )
                .AppendLine( "}" );

            sb.Append( ".dark input.dark\\:text-green-400," )
                .Append( ".dark textarea.dark\\:text-green-400," )
                .Append( ".dark select.dark\\:text-green-400" )
                .Append( "{" )
                .Append( $"color: {validationSuccessColor};" )
                .AppendLine( "}" );

            sb.Append( ".dark input.dark\\:placeholder-green-500::placeholder," )
                .Append( ".dark textarea.dark\\:placeholder-green-500::placeholder" )
                .Append( "{" )
                .Append( $"color: {validationSuccessColor};" )
                .AppendLine( "}" );

            sb.Append( ".dark input.dark\\:border-green-500," )
                .Append( ".dark textarea.dark\\:border-green-500," )
                .Append( ".dark select.dark\\:border-green-500" )
                .Append( "{" )
                .Append( $"border-color: {validationSuccessColor};" )
                .AppendLine( "}" );

            sb.Append( ".text-success-600" ).Append( "{" )
                .Append( $"color: {validationSuccessColor};" )
                .AppendLine( "}" );

            sb.Append( ".dark .dark\\:text-success-500" ).Append( "{" )
                .Append( $"color: {validationSuccessColor};" )
                .AppendLine( "}" );
        }

        if ( !string.IsNullOrEmpty( validationDangerColor ) )
        {
            var validationDangerBackground = ToHexRGBA( Transparency( validationDangerColor, 16 ) );

            sb.Append( "input.bg-red-50," )
                .Append( "textarea.bg-red-50," )
                .Append( "select.bg-red-50" )
                .Append( "{" )
                .Append( $"background-color: {validationDangerBackground};" )
                .AppendLine( "}" );

            sb.Append( "input.border-red-500," )
                .Append( "textarea.border-red-500," )
                .Append( "select.border-red-500" )
                .Append( "{" )
                .Append( $"border-color: {validationDangerColor};" )
                .AppendLine( "}" );

            sb.Append( "input.text-red-900," )
                .Append( "textarea.text-red-900," )
                .Append( "select.text-red-900" )
                .Append( "{" )
                .Append( $"color: {validationDangerColor};" )
                .AppendLine( "}" );

            sb.Append( "input.placeholder-red-700::placeholder," )
                .Append( "textarea.placeholder-red-700::placeholder" )
                .Append( "{" )
                .Append( $"color: {validationDangerColor};" )
                .AppendLine( "}" );

            sb.Append( "input.focus\\:border-red-500:focus," )
                .Append( "textarea.focus\\:border-red-500:focus," )
                .Append( "select.focus\\:border-red-500:focus" )
                .Append( "{" )
                .Append( $"border-color: {validationDangerColor};" )
                .AppendLine( "}" );

            sb.Append( "input.focus\\:ring-red-500:focus," )
                .Append( "textarea.focus\\:ring-red-500:focus," )
                .Append( "select.focus\\:ring-red-500:focus" )
                .Append( "{" )
                .Append( $"--tw-ring-color: {validationDangerColor};" )
                .AppendLine( "}" );

            sb.Append( ".dark input.dark\\:text-red-500," )
                .Append( ".dark textarea.dark\\:text-red-500," )
                .Append( ".dark select.dark\\:text-red-500" )
                .Append( "{" )
                .Append( $"color: {validationDangerColor};" )
                .AppendLine( "}" );

            sb.Append( ".dark input.dark\\:placeholder-red-500::placeholder," )
                .Append( ".dark textarea.dark\\:placeholder-red-500::placeholder" )
                .Append( "{" )
                .Append( $"color: {validationDangerColor};" )
                .AppendLine( "}" );

            sb.Append( ".dark input.dark\\:border-red-500," )
                .Append( ".dark textarea.dark\\:border-red-500," )
                .Append( ".dark select.dark\\:border-red-500" )
                .Append( "{" )
                .Append( $"border-color: {validationDangerColor};" )
                .AppendLine( "}" );

            sb.Append( ".text-danger-600" ).Append( "{" )
                .Append( $"color: {validationDangerColor};" )
                .AppendLine( "}" );

            sb.Append( ".dark .dark\\:text-danger-500" ).Append( "{" )
                .Append( $"color: {validationDangerColor};" )
                .AppendLine( "}" );
        }
    }

    protected virtual void GenerateInputCheckEditStyles( StringBuilder sb, Theme theme, ThemeInputOptions options )
    {
        //sb
        //    .Append( ".custom-checkbox .custom-control-input:checked ~ .custom-control-label::before" ).Append( "{" )
        //    .Append( $"background-color: {options.CheckColor};" )
        //    .AppendLine( "}" );

        //sb
        //    .Append( ".custom-checkbox .custom-control-input:disabled:checked ~ .custom-control-label::before" ).Append( "{" )
        //    .Append( $"background-color: {ToHexRGBA( Transparency( options.CheckColor, 128 ) )};" )
        //    .AppendLine( "}" );

        //sb
        //    .Append( ".custom-radio .custom-control-input:disabled:checked ~ .custom-control-label::before" ).Append( "{" )
        //    .Append( $"background-color: {ToHexRGBA( Transparency( options.CheckColor, 128 ) )};" )
        //    .AppendLine( "}" );

        //sb
        //    .Append( ".custom-control-input:checked ~ .custom-control-label::before" ).Append( "{" )
        //    .Append( $"color: {options.Color};" )
        //    .Append( $"border-color: {options.CheckColor};" )
        //    .Append( $"background-color: {options.CheckColor};" )
        //    .AppendLine( "}" );

        //sb
        //    .Append( ".custom-switch .custom-control-input:checked ~ .custom-control-label::before" ).Append( "{" )
        //    .Append( $"background-color: {options.CheckColor};" )
        //    .AppendLine( "}" );
    }

    protected override void GenerateBadgeVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor )
    {
        //var backgroundColor = ParseColor( inBackgroundColor );

        //if ( backgroundColor.IsEmpty )
        //    return;

        //var yiqBackgroundColor = Contrast( theme, backgroundColor );

        //var background = ToHex( backgroundColor );
        //var yiqBackground = ToHex( yiqBackgroundColor );

        //sb.Append( $".badge-{variant}" ).Append( "{" )
        //    .Append( $"color: {yiqBackground};" )
        //    .Append( $"background-color: {background};" )
        //    .AppendLine( "}" );
    }

    protected override void GenerateSwitchVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, ThemeSwitchOptions options )
    {
        //var backgroundColor = ParseColor( inBackgroundColor );

        //if ( backgroundColor.IsEmpty )
        //    return;

        //var boxShadowColor = Lighten( backgroundColor, options?.BoxShadowLightenColor ?? 25 );
        //var disabledBackgroundColor = Lighten( backgroundColor, options?.DisabledLightenColor ?? 50 );

        //var background = ToHex( backgroundColor );
        //var boxShadow = ToHex( boxShadowColor );
        //var disabledBackground = ToHex( disabledBackgroundColor );

        //sb
        //    .Append( $".custom-switch .custom-control-input.custom-control-input-{variant}:checked ~ .custom-control-label::before" ).Append( "{" )
        //    .Append( $"background-color: {background};" )
        //    .Append( $"border-color: {background};" )
        //    .AppendLine( "}" );

        //sb
        //    .Append( $".custom-switch .custom-control-input.custom-control-input-{variant}:focus ~ .custom-control-label::before" ).Append( "{" )
        //    .Append( $"box-shadow: {boxShadow};" )
        //    .Append( $"border-color: {background};" )
        //    .AppendLine( "}" );

        //sb
        //    .Append( $".custom-switch .custom-control-input:disabled.custom-control-input-{variant}:checked ~ .custom-control-label::before" ).Append( "{" )
        //    .Append( $"background-color: {disabledBackground};" )
        //    .AppendLine( "}" );
    }

    protected override void GenerateStepsStyles( StringBuilder sb, Theme theme, ThemeStepsOptions stepsOptions )
    {
        //sb
        //    .Append( ".step-completed .step-circle" ).Append( "{" )
        //    .Append( $"color: {Var( ThemeVariables.White )};" )
        //    .Append( $"background-color: {Var( ThemeVariables.StepsItemIconCompleted, Var( ThemeVariables.Color( "success" ) ) )};" )
        //    .Append( $"border-color: {Var( ThemeVariables.StepsItemIconCompleted, Var( ThemeVariables.Color( "success" ) ) )};" )
        //    .AppendLine( "}" );

        //sb
        //    .Append( ".step-completed .step-circle::before" ).Append( "{" )
        //    .Append( $"color: {Var( ThemeVariables.StepsItemIconCompleted, Var( ThemeVariables.Color( "success" ) ) )};" )
        //    .AppendLine( "}" );

        //sb
        //    .Append( ".step-completed .step-text" ).Append( "{" )
        //    .Append( $"color: {Var( ThemeVariables.StepsItemTextCompleted, Var( ThemeVariables.Color( "success" ) ) )};" )
        //    .AppendLine( "}" );

        //sb
        //    .Append( ".step-active .step-circle" ).Append( "{" )
        //    .Append( $"color: {Var( ThemeVariables.White )};" )
        //    .Append( $"background-color: {Var( ThemeVariables.StepsItemIconActive, Var( ThemeVariables.Color( "primary" ) ) )};" )
        //    .Append( $"border-color: {Var( ThemeVariables.StepsItemIconActive, Var( ThemeVariables.Color( "primary" ) ) )};" )
        //    .AppendLine( "}" );

        //sb
        //    .Append( ".step-active .step-circle::before" ).Append( "{" )
        //    .Append( $"color: {Var( ThemeVariables.StepsItemIconActive, Var( ThemeVariables.Color( "primary" ) ) )};" )
        //    .AppendLine( "}" );

        //sb
        //    .Append( ".step-active .step-text" ).Append( "{" )
        //    .Append( $"color: {Var( ThemeVariables.StepsItemTextActive, Var( ThemeVariables.Color( "primary" ) ) )};" )
        //    .AppendLine( "}" );
    }

    protected override void GenerateStepsVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, ThemeStepsOptions stepsOptions )
    {
        //sb
        //    .Append( $".step-{variant} .step-circle" ).Append( "{" )
        //    .Append( $"color: {Var( ThemeVariables.VariantStepsItemIcon( variant ) )};" )
        //    .Append( $"border-color: {Var( ThemeVariables.VariantStepsItemIcon( variant ) )};" )
        //    .AppendLine( "}" );

        //sb
        //    .Append( $".step-{variant}.step-completed .step-circle" ).Append( "{" )
        //    .Append( $"color: {Var( ThemeVariables.VariantStepsItemIconYiq( variant ) )};" )
        //    .Append( $"background-color: {Var( ThemeVariables.VariantStepsItemIcon( variant ) )};" )
        //    .Append( $"border-color: {Var( ThemeVariables.VariantStepsItemIcon( variant ) )};" )
        //    .AppendLine( "}" );

        //sb
        //    .Append( $".step-{variant}.step-completed .step-circle::before" ).Append( "{" )
        //    .Append( $"color: {Var( ThemeVariables.VariantStepsItemIcon( variant ) )};" )
        //    .AppendLine( "}" );

        //sb
        //    .Append( $".step-{variant}.step-completed .step-text" ).Append( "{" )
        //    .Append( $"color: {Var( ThemeVariables.VariantStepsItemText( variant ) )};" )
        //    .AppendLine( "}" );

        //sb
        //    .Append( $".step-{variant}.step-active .step-circle" ).Append( "{" )
        //    .Append( $"color: {Var( ThemeVariables.StepsItemIconActiveYiq, Var( ThemeVariables.White ) )};" )
        //    .Append( $"background-color: {Var( ThemeVariables.StepsItemIconActive, Var( ThemeVariables.Color( "primary" ) ) )};" )
        //    .Append( $"border-color: {Var( ThemeVariables.StepsItemIconActive, Var( ThemeVariables.Color( "primary" ) ) )};" )
        //    .AppendLine( "}" );

        //sb
        //    .Append( $".step-{variant}.step-active::before" ).Append( "{" )
        //    .Append( $"color: {Var( ThemeVariables.StepsItemIconActive, Var( ThemeVariables.Color( "primary" ) ) )};" )
        //    .AppendLine( "}" );

        //sb
        //    .Append( $".step-{variant}.step-active .step-text" ).Append( "{" )
        //    .Append( $"color: {Var( ThemeVariables.StepsItemIconActive, Var( ThemeVariables.Color( "primary" ) ) )};" )
        //    .AppendLine( "}" );
    }

    protected override void GenerateRatingStyles( StringBuilder sb, Theme theme, ThemeRatingOptions ratingOptions )
    {
        //if ( ratingOptions?.HoverOpacity != null )
        //{
        //    sb
        //        .Append( ".rating .rating-item.rating-item-hover" ).Append( "{" )
        //        .Append( $"opacity: {string.Format( CultureInfo.InvariantCulture, "{0:F1}", ratingOptions.HoverOpacity )};" )
        //        .AppendLine( "}" );
        //}
    }

    protected override void GenerateRatingVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, ThemeRatingOptions ratingOptions )
    {
        //sb
        //    .Append( $".rating .rating-item.rating-item-{variant}" ).Append( "{" )
        //    .Append( $"color: {Var( ThemeVariables.VariantRatingColor( variant ) )};" )
        //    .AppendLine( "}" );
    }

    protected override void GenerateAlertVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, string inBorderColor, string inColor, ThemeAlertOptions options )
    {
        //var backgroundColor = ParseColor( inBackgroundColor );
        //var borderColor = ParseColor( inBorderColor );
        //var textColor = ParseColor( inColor );

        //var alertLinkColor = Darken( textColor, 10f );

        //var background = ToHex( backgroundColor );
        //var border = ToHex( borderColor );
        //var text = ToHex( textColor );
        //var alertLink = ToHex( alertLinkColor );

        //sb.Append( $".alert-{variant}" ).Append( "{" )
        //    .Append( $"color: {text};" )
        //    .Append( GetGradientBg( theme, background, options?.GradientBlendPercentage ) )
        //    .Append( $"border-color: {border};" )
        //    .AppendLine( "}" );

        //sb.Append( $".alert-{variant}.alert-link" ).Append( "{" )
        //    .Append( $"color: {alertLink};" )
        //    .AppendLine( "}" );
    }

    protected override void GenerateTableVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, string inBorderColor )
    {
        //var backgroundColor = ParseColor( inBackgroundColor );
        //var hoverBackgroundColor = Darken( backgroundColor, 5 );
        //var borderColor = ParseColor( inBorderColor );

        //var background = ToHex( backgroundColor );
        //var hoverBackground = ToHex( hoverBackgroundColor );
        //var border = ToHex( borderColor );

        //sb.Append( $".table-{variant}," )
        //    .Append( $".table-{variant}>th," )
        //    .Append( $".table-{variant}>td" )
        //    .Append( "{" )
        //    .Append( $"background-color: {background};" )
        //    .AppendLine( "}" );

        //sb.Append( $".table-{variant} th," )
        //    .Append( $".table-{variant} td," )
        //    .Append( $".table-{variant} thead td," )
        //    .Append( $".table-{variant} tbody + tbody" )
        //    .Append( "{" )
        //    .Append( $"border-color: {border};" )
        //    .AppendLine( "}" );

        //sb.Append( $".table-hover .table-{variant}:hover" )
        //    .Append( "{" )
        //    .Append( $"background-color: {hoverBackground};" )
        //    .AppendLine( "}" );

        //sb.Append( $".table-hover .table-{variant}:hover>td," )
        //    .Append( $".table-hover .table-{variant}:hover>th" )
        //    .Append( "{" )
        //    .Append( $"background-color: {hoverBackground};" )
        //    .AppendLine( "}" );
    }

    protected override void GenerateCardStyles( StringBuilder sb, Theme theme, ThemeCardOptions options )
    {
        //sb.Append( ".card" ).Append( "{" )
        //    .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
        //    .AppendLine( "}" );

        //if ( !string.IsNullOrEmpty( options?.ImageTopRadius ) )
        //    sb.Append( ".card-img-top" ).Append( "{" )
        //        .Append( $"border-top-left-radius: {options.ImageTopRadius};" )
        //        .Append( $"border-top-right-radius: {options.ImageTopRadius};" )
        //        .AppendLine( "}" );
    }

    protected override void GenerateModalStyles( StringBuilder sb, Theme theme, ThemeModalOptions options )
    {
        //sb.Append( ".modal-content" ).Append( "{" )
        //    .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
        //    .AppendLine( "}" );
    }

    protected override void GenerateTabsStyles( StringBuilder sb, Theme theme, ThemeTabsOptions options )
    {
        //sb.Append( ".nav-tabs .nav-link" ).Append( "{" )
        //    .Append( $"border-top-left-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
        //    .Append( $"border-top-right-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
        //    .AppendLine( "}" );

        //sb.Append( ".nav-pills .nav-link" ).Append( "{" )
        //    .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
        //    .AppendLine( "}" );

        //if ( !string.IsNullOrEmpty( theme.ColorOptions?.Primary ) )
        //{
        //    sb
        //        .Append( ".nav-pills .nav-link.active," )
        //        .Append( ".nav-pills .show>.nav-link" )
        //        .Append( "{" )
        //        .Append( $"background-color: {Var( ThemeVariables.Color( "primary" ) )};" )
        //        .AppendLine( "}" );

        //    sb
        //        .Append( ".nav.nav-tabs .nav-item a.nav-link:not(.active)" )
        //        .Append( "{" )
        //        .Append( $"color: {Var( ThemeVariables.Color( "primary" ) )};" )
        //        .AppendLine( "}" );
        //}
    }

    protected override void GenerateProgressStyles( StringBuilder sb, Theme theme, ThemeProgressOptions options )
    {
        //sb.Append( ".progress" ).Append( "{" )
        //    .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
        //    .AppendLine( "}" );

        //if ( !string.IsNullOrEmpty( theme.ColorOptions?.Primary ) )
        //{
        //    sb.Append( ".progress-bar" ).Append( "{" )
        //        .Append( $"background-color: {Var( ThemeVariables.Color( "primary" ) )};" )
        //        .AppendLine( "}" );
        //}

        //base.GenerateProgressStyles( sb, theme, options );
    }

    protected override void GenerateAlertStyles( StringBuilder sb, Theme theme, ThemeAlertOptions options )
    {
        //sb.Append( ".alert" ).Append( "{" )
        //    .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
        //    .AppendLine( "}" );
    }

    protected override void GenerateBreadcrumbStyles( StringBuilder sb, Theme theme, ThemeBreadcrumbOptions options )
    {
        //sb.Append( ".breadcrumb" ).Append( "{" )
        //    .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
        //    .AppendLine( "}" );


        //if ( !string.IsNullOrEmpty( Var( ThemeVariables.BreadcrumbColor ) ) )
        //{
        //    sb.Append( ".breadcrumb-item>a" ).Append( "{" )
        //        .Append( $"color: {Var( ThemeVariables.BreadcrumbColor )};" )
        //        .AppendLine( "}" );
        //}
    }

    protected override void GenerateBadgeStyles( StringBuilder sb, Theme theme, ThemeBadgeOptions options )
    {
        //sb.Append( ".badge:not(.badge-pill)" ).Append( "{" )
        //    .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
        //    .AppendLine( "}" );
    }

    protected override void GeneratePaginationStyles( StringBuilder sb, Theme theme, ThemePaginationOptions options )
    {
        //sb.Append( ".page-item:first-child .page-link" ).Append( "{" )
        //    .Append( $"border-top-left-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
        //    .Append( $"border-bottom-left-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
        //    .AppendLine( "}" );

        //sb.Append( ".page-item:last-child .page-link" ).Append( "{" )
        //    .Append( $"border-top-right-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
        //    .Append( $"border-bottom-right-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
        //    .AppendLine( "}" );

        //sb.Append( ".pagination-lg .page-item:first-child .page-link" ).Append( "{" )
        //    .Append( $"border-top-left-radius: {GetBorderRadius( theme, options?.LargeBorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
        //    .Append( $"border-bottom-left-radius: {GetBorderRadius( theme, options?.LargeBorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
        //    .AppendLine( "}" );

        //sb.Append( ".pagination-lg .page-item:last-child .page-link" ).Append( "{" )
        //    .Append( $"border-top-right-radius: {GetBorderRadius( theme, options?.LargeBorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
        //    .Append( $"border-bottom-right-radius: {GetBorderRadius( theme, options?.LargeBorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
        //    .AppendLine( "}" );

        //if ( !string.IsNullOrEmpty( theme.ColorOptions?.Primary ) )
        //{
        //    sb.Append( ".page-link" ).Append( "{" )
        //        .Append( $"color: {theme.ColorOptions.Primary};" )
        //        .AppendLine( "}" );

        //    sb.Append( ".page-link:hover" ).Append( "{" )
        //        .Append( $"color: {ToHex( Darken( theme.ColorOptions.Primary, 15f ) )};" )
        //        .AppendLine( "}" );

        //    sb.Append( ".page-item.active .page-link" ).Append( "{" )
        //        .Append( $"color: {ToHex( Contrast( theme, theme.ColorOptions.Primary ) )};" )
        //        .Append( $"background-color: {theme.ColorOptions.Primary};" )
        //        .Append( $"border-color: {theme.ColorOptions.Primary};" )
        //        .AppendLine( "}" );
        //}
    }

    protected override void GenerateBarStyles( StringBuilder sb, Theme theme, ThemeBarOptions options )
    {
        //foreach ( var (variant, _) in theme.ValidBackgroundColors )
        //{
        //    var yiqColor = Var( ThemeVariables.BackgroundYiqColor( variant ) );

        //    if ( string.IsNullOrEmpty( yiqColor ) )
        //        continue;

        //    sb.Append( $".navbar.bg-{variant} .navbar-brand .nav-item .nav-link" ).Append( "{" )
        //        .Append( $"color: {yiqColor};" )
        //        .AppendLine( "}" );
        //}
    }

    protected override void GenerateParagraphVariantStyles( StringBuilder sb, Theme theme, string variant, string inTextColor )
    {
        //var textColor = variant == "body" && !string.IsNullOrEmpty( theme.BodyOptions?.TextColor )
        //    ? ParseColor( theme.BodyOptions.TextColor )
        //    : ParseColor( inTextColor );

        //var hexTextColor = ToHex( textColor );

        //sb.Append( $".text-{variant}" )
        //    .Append( "{" )
        //    .Append( $"color: {hexTextColor} !important;" )
        //    .AppendLine( "}" );
    }

    protected override void GenerateInputVariantStyles( StringBuilder sb, Theme theme, string variant, string inColor )
    {
        //var color = ToHex( ParseColor( inColor ) );

        //sb
        //    .Append( $".form-control.text-{variant}," )
        //    .Append( $".form-control-plaintext.text-{variant}" )
        //    .Append( "{" )
        //    .Append( $"color: {color};" )
        //    .AppendLine( "}" );
    }

    protected override void GenerateListGroupItemStyles( StringBuilder sb, Theme theme, ThemeListGroupItemOptions options )
    {
        //if ( !string.IsNullOrEmpty( theme.ColorOptions?.Primary ) )
        //{
        //    var white = Var( ThemeVariables.White );
        //    var primary = Var( ThemeVariables.Color( "primary" ) );

        //    sb
        //        .Append( ".list-group-item.active" )
        //        .Append( "{" )
        //        .Append( $"color: {white};" )
        //        .Append( GetGradientBg( theme, primary, options?.GradientBlendPercentage ) )
        //        .Append( $"border-color: {primary};" )
        //        .AppendLine( "}" );
        //}
    }

    protected override void GenerateListGroupItemVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, string inColor, ThemeListGroupItemOptions options )
    {
        //var backgroundColor = ParseColor( inBackgroundColor );
        //var hoverBackgroundColor = Darken( backgroundColor, 5 );

        //var background = ToHex( backgroundColor );
        //var hoverBackground = ToHex( hoverBackgroundColor );
        //var color = ToHex( ParseColor( inColor ) );

        //var white = Var( ThemeVariables.White );

        //sb
        //    .Append( $".list-group-item-{variant}" )
        //    .Append( "{" )
        //    .Append( $"color: {color};" )
        //    .Append( GetGradientBg( theme, background, options?.GradientBlendPercentage ) )
        //    .AppendLine( "}" );

        //sb
        //    .Append( $".list-group-item-{variant}.list-group-item-action:focus," )
        //    .Append( $".list-group-item-{variant}.list-group-item-action:hover" )
        //    .Append( "{" )
        //    .Append( $"color: {color};" )
        //    .Append( GetGradientBg( theme, hoverBackground, options?.GradientBlendPercentage ) )
        //    .AppendLine( "}" );

        //sb
        //    .Append( $".list-group-item-{variant}.list-group-item-action.active" )
        //    .Append( "{" )
        //    .Append( $"color: {white};" )
        //    .Append( GetGradientBg( theme, color, options?.GradientBlendPercentage ) )
        //    .Append( $"border-color: {color};" )
        //    .AppendLine( "}" );
    }

    private static string GetValidBreakpointName( string name ) => name switch
    {
        "mobile" => "xs",
        "tablet" => "sm",
        "desktop" => "md",
        "widescreen" => "lg",
        "fullhd" => "xl",
        _ => "",
    };

    protected override void GenerateSpacingStyles( StringBuilder sb, Theme theme, ThemeSpacingOptions options )
    {
        //if ( theme.BreakpointOptions == null || options == null )
        //    return;

        //foreach ( var breakpoint in theme.BreakpointOptions )
        //{
        //    var breakpointName = GetValidBreakpointName( breakpoint.Key );
        //    var breakpointMin = breakpoint.Value();

        //    var hasMinMedia = !string.IsNullOrEmpty( breakpointMin ) && breakpointMin != "0";

        //    if ( hasMinMedia )
        //    {
        //        sb.Append( $"@media (min-width: {breakpointMin})" ).Append( "{" );
        //    }

        //    var infix = string.IsNullOrEmpty( breakpointMin ) || breakpointMin == "0"
        //        ? ""
        //        : $"-{breakpointName}";

        //    foreach ( (string prop, string abbrev) in new[] { ("margin", "m"), ("padding", "p") } )
        //    {
        //        foreach ( (string size, Func<string> lenghtFunc) in options )
        //        {
        //            var length = lenghtFunc.Invoke();

        //            sb
        //                .Append( $".{abbrev}{infix}-{size}" )
        //                .Append( "{" ).Append( $"{prop}: {length} !important;" ).Append( "}" );

        //            sb
        //                .Append( $".{abbrev}t{infix}-{size}," )
        //                .Append( $".{abbrev}y{infix}-{size}" )
        //                .Append( "{" ).Append( $"{prop}-top: {length} !important;" ).Append( "}" );

        //            sb
        //                .Append( $".{abbrev}r{infix}-{size}," )
        //                .Append( $".{abbrev}x{infix}-{size}" )
        //                .Append( "{" ).Append( $"{prop}-right: {length} !important;" ).Append( "}" );

        //            sb
        //                .Append( $".{abbrev}b{infix}-{size}," )
        //                .Append( $".{abbrev}y{infix}-{size}" )
        //                .Append( "{" ).Append( $"{prop}-bottom: {length} !important;" ).Append( "}" );

        //            sb
        //                .Append( $".{abbrev}l{infix}-{size}," )
        //                .Append( $".{abbrev}x{infix}-{size}" )
        //                .Append( "{" ).Append( $"{prop}-left: {length} !important;" ).Append( "}" );
        //        }
        //    }

        //    foreach ( (string size, Func<string> lenghtFunc) in options )
        //    {
        //        if ( string.IsNullOrEmpty( size ) || size == "0" )
        //            continue;

        //        var length = lenghtFunc.Invoke();

        //        sb
        //            .Append( $".m{infix}-n{size}" )
        //            .Append( "{" ).Append( $"margin: -{length} !important;" ).Append( "}" );

        //        sb
        //            .Append( $".mt{infix}-n{size}," )
        //            .Append( $".my{infix}-n{size}" )
        //            .Append( "{" ).Append( $"margin-top: -{length} !important;" ).Append( "}" );

        //        sb
        //            .Append( $".mr{infix}-n{size}," )
        //            .Append( $".mx{infix}-n{size}" )
        //            .Append( "{" ).Append( $"margin-right: -{length} !important;" ).Append( "}" );

        //        sb
        //            .Append( $".mb{infix}-n{size}," )
        //            .Append( $".my{infix}-n{size}" )
        //            .Append( "{" ).Append( $"margin-bottom: -{length} !important;" ).Append( "}" );

        //        sb
        //            .Append( $".ml{infix}-n{size}," )
        //            .Append( $".mx{infix}-n{size}" )
        //            .Append( "{" ).Append( $"margin-left: -{length} !important;" ).Append( "}" );
        //    }

        //    if ( hasMinMedia )
        //    {
        //        sb.Append( "}" );
        //    }
        //}
    }

    #endregion
}