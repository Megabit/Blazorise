#region Using directives
using System;
using System.Globalization;
using System.Text;
using Blazorise.Extensions;
#endregion

namespace Blazorise.Bootstrap.Providers;

public class BootstrapThemeGenerator : ThemeGenerator
{
    #region Constructors

    public BootstrapThemeGenerator( IThemeCache themeCache )
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

    protected override void GenerateBackgroundVariantStyles( StringBuilder sb, Theme theme, string variant )
    {
        var hexBackgroundColor = Var( ThemeVariables.BackgroundColor( variant ) );
        var hexBackgroundColorSubtle = Var( ThemeVariables.BackgroundSubtleColor( variant ) );

        sb.Append( $".bg-{variant}" ).Append( "{" )
            .Append( $"background-color: {hexBackgroundColor} !important;" )
            .AppendLine( "}" );

        sb.Append( $".bg-{variant}-subtle" ).Append( "{" )
            .Append( $"background-color: {hexBackgroundColorSubtle} !important;" )
            .AppendLine( "}" );

        sb.Append( $".jumbotron-{variant}" ).Append( "{" )
            .Append( $"background-color: {hexBackgroundColor} !important;" )
            .Append( $"color: {ToHex( Contrast( theme, hexBackgroundColor ) )} !important;" )
            .AppendLine( "}" );
    }

    protected override void GenerateBorderVariantStyles( StringBuilder sb, Theme theme, string variant )
    {
        var hexBorderColor = Var( ThemeVariables.BorderColor( variant ) );
        var hexBorderColorSubtle = Var( ThemeVariables.BorderSubtleColor( variant ) );

        sb.Append( $".border-{variant}" ).Append( "{" )
            .Append( $"border-color: {hexBorderColor} !important;" )
            .AppendLine( "}" );

        sb.Append( $".border-{variant}-subtle" ).Append( "{" )
            .Append( $"border-color: {hexBorderColorSubtle} !important;" )
            .AppendLine( "}" );

        for ( int i = 1; i <= 5; ++i )
        {
            sb.Append( $".border-{i}.border-{variant}" ).Append( "{" )
                .Append( $"border-color: {hexBorderColor} !important;" )
                .AppendLine( "}" );
        }
    }

    protected override void GenerateButtonVariantStyles( StringBuilder sb, Theme theme, string variant, ThemeButtonOptions options )
    {
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

        if ( variant == "link" )
        {
            sb
                .Append( $".btn-{variant}" ).Append( "{" )
                .Append( $"color: {background};" )
                .AppendLine( "}" );

            sb.Append( $".btn-{variant}:hover" )
                .Append( "{" )
                .Append( $"color: {hoverBackground};" )
                .AppendLine( "}" );

            sb.Append( $".btn-{variant}.disabled," )
                .Append( $".btn-{variant}:disabled" )
                .Append( "{" )
                .Append( $"color: {ToHex( Darken( background, 15f ) )};" )
                .AppendLine( "}" );
        }
        else
        {
            sb.Append( $".btn-{variant}," )
                .Append( $"a.btn-{variant}" )
                .Append( "{" )
                .Append( $"color: {yiqBackground};" )
                .Append( GetGradientBg( theme, background, options?.GradientBlendPercentage ) )
                .Append( $"border-color: {border};" )
                .AppendLine( "}" );

            sb.Append( $".btn-{variant}:hover," )
                .Append( $"a.btn-{variant}:hover" )
                .Append( "{" )
                .Append( $"color: {yiqHoverBackground};" )
                .Append( GetGradientBg( theme, hoverBackground, options?.GradientBlendPercentage ) )
                .Append( $"border-color: {hoverBorder};" )
                .AppendLine( "}" );

            sb.Append( $".btn-{variant}:focus," )
                .Append( $".btn-{variant}.focus," )
                .Append( $"a.btn-{variant}:focus," )
                .Append( $"a.btn-{variant}.focus" )
                .Append( "{" )
                .Append( $"color: {yiqHoverBackground};" )
                .Append( GetGradientBg( theme, hoverBackground, options?.GradientBlendPercentage ) )
                .Append( $"border-color: {hoverBorder};" )
                .Append( $"box-shadow: 0 0 0 {options?.BoxShadowSize ?? ".2rem"} {boxShadow};" )
                .AppendLine( "}" );

            sb.Append( $".btn-{variant}.disabled," )
                .Append( $".btn-{variant}:disabled," )
                .Append( $"a.btn-{variant}.disabled," )
                .Append( $"a.btn-{variant}:disabled" )
                .Append( "{" )
                .Append( $"color: {yiqBackground};" )
                .Append( $"background-color: {background};" )
                .Append( $"border-color: {border};" )
                .AppendLine( "}" );

            sb
                .Append( $".btn-{variant}:not(:disabled):not(.disabled):active," )
                .Append( $".btn-{variant}:not(:disabled):not(.disabled).active," )
                .Append( $".show>.btn-{variant}.dropdown-toggle," )
                .Append( $"a.btn-{variant}:not(:disabled):not(.disabled):active," )
                .Append( $"a.btn-{variant}:not(:disabled):not(.disabled).active," )
                .Append( $"a.show>.btn-{variant}.dropdown-toggle" )
                .Append( "{" )
                .Append( $"color: {yiqActiveBackground};" )
                .Append( $"background-color: {activeBackground};" )
                .Append( $"border-color: {activeBorder};" )
                .AppendLine( "}" );

            sb
                .Append( $".btn-{variant}:not(:disabled):not(.disabled):active:focus," )
                .Append( $".btn-{variant}:not(:disabled):not(.disabled).active:focus," )
                .Append( $".show>.btn-{variant}.dropdown-toggle:focus," )
                .Append( $"a.btn-{variant}:not(:disabled):not(.disabled):active:focus," )
                .Append( $"a.btn-{variant}:not(:disabled):not(.disabled).active:focus," )
                .Append( $"a.show>.btn-{variant}.dropdown-toggle:focus" )
                .Append( "{" )
                .Append( $"box-shadow: 0 0 0 {options?.BoxShadowSize ?? ".2rem"} {boxShadow}" )
                .AppendLine( "}" );
        }
    }

    protected override void GenerateButtonOutlineVariantStyles( StringBuilder sb, Theme theme, string variant, ThemeButtonOptions options )
    {
        var color = Var( ThemeVariables.OutlineButtonColor( variant ) );
        var yiqColor = Var( ThemeVariables.OutlineButtonYiqColor( variant ) );
        var boxShadow = Var( ThemeVariables.OutlineButtonBoxShadowColor( variant ) );

        sb
            .Append( $".btn-outline-{variant}," )
            .Append( $"a.btn-outline-{variant}" )
            .Append( "{" )
            .Append( $"color: {color};" )
            .Append( $"border-color: {color};" )
            .AppendLine( "}" );

        sb
            .Append( $".btn-outline-{variant}:hover," )
            .Append( $"a.btn-outline-{variant}:hover" )
            .Append( "{" )
            .Append( $"color: {yiqColor};" )
            .Append( $"background-color: {color};" )
            .Append( $"border-color: {color};" )
            .AppendLine( "}" );

        sb
            .Append( $".btn-outline-{variant}:focus," )
            .Append( $".btn-outline-{variant}.focus," )
            .Append( $"a.btn-outline-{variant}:focus," )
            .Append( $"a.btn-outline-{variant}.focus" )
            .Append( "{" )
            .Append( $"box-shadow: 0 0 0 {options?.BoxShadowSize ?? ".2rem"} {boxShadow};" )
            .AppendLine( "}" );

        sb
            .Append( $".btn-outline-{variant}.disabled," )
            .Append( $".btn-outline-{variant}:disabled," )
            .Append( $"a.btn-outline-{variant}.disabled," )
            .Append( $"a.btn-outline-{variant}:disabled" )
            .Append( "{" )
            .Append( $"color: {color};" )
            .Append( "background-color: transparent;" )
            .AppendLine( "}" );

        sb
            .Append( $".btn-outline-{variant}:not(:disabled):not(.disabled):active," )
            .Append( $".btn-outline-{variant}:not(:disabled):not(.disabled).active," )
            .Append( $".show>.btn-outline-{variant}.dropdown-toggle," )
            .Append( $"a.btn-outline-{variant}:not(:disabled):not(.disabled):active," )
            .Append( $"a.btn-outline-{variant}:not(:disabled):not(.disabled).active," )
            .Append( $"a.show>.btn-outline-{variant}.dropdown-toggle" )
            .Append( "{" )
            .Append( $"color: {yiqColor};" )
            .Append( $"background-color: {color};" )
            .Append( $"border-color: {color};" )
            .AppendLine( "}" );

        sb
            .Append( $".btn-outline-{variant}:not(:disabled):not(.disabled):active:focus," )
            .Append( $".btn-outline-{variant}:not(:disabled):not(.disabled).active:focus," )
            .Append( $".show>.btn-outline-{variant}.dropdown-toggle:focus," )
            .Append( $"a.btn-outline-{variant}:not(:disabled):not(.disabled):active:focus," )
            .Append( $"a.btn-outline-{variant}:not(:disabled):not(.disabled).active:focus," )
            .Append( $"a.show>.btn-outline-{variant}.dropdown-toggle:focus" )
            .Append( "{" )
            .Append( $"box-shadow: 0 0 0 {options?.BoxShadowSize ?? ".2rem"} {boxShadow};" )
            .AppendLine( "}" );
    }

    protected override void GenerateButtonStyles( StringBuilder sb, Theme theme, ThemeButtonOptions options )
    {
        if ( !string.IsNullOrEmpty( options?.BorderRadius ) )
        {
            sb.Append( ".btn" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );
        }

        if ( !string.IsNullOrEmpty( options?.SmallBorderRadius ) )
        {
            sb.Append( ".btn-sm" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.SmallBorderRadius, Var( ThemeVariables.BorderRadiusSmall ) )};" )
                .AppendLine( "}" );
        }

        if ( !string.IsNullOrEmpty( options?.LargeBorderRadius ) )
        {
            sb.Append( ".btn-lg" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.LargeBorderRadius, Var( ThemeVariables.BorderRadiusLarge ) )};" )
                .AppendLine( "}" );
        }

        if ( !string.IsNullOrEmpty( options?.Padding ) )
            sb.Append( ".btn" ).Append( "{" )
                .Append( $"padding: {options.Padding};" )
                .AppendLine( "}" );

        if ( !string.IsNullOrEmpty( options?.Margin ) )
            sb.Append( ".btn" ).Append( "{" )
                .Append( $"margin: {options.Margin};" )
                .AppendLine( "}" );

        if ( options?.DisabledOpacity != null )
            sb.Append( ".btn.disabled, .btn:disabled" ).Append( "{" )
                .Append( $"opacity: {options.DisabledOpacity.ToCultureInvariantString()};" )
                .AppendLine( "}" );
    }

    protected override void GenerateDropdownStyles( StringBuilder sb, Theme theme, ThemeDropdownOptions options )
    {
        if ( !string.IsNullOrEmpty( options?.BorderRadius ) )
        {
            sb.Append( ".dropdown-menu" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );
        }

        if ( !string.IsNullOrEmpty( theme.ColorOptions?.Primary ) )
        {
            var backgroundColor = ParseColor( theme.ColorOptions.Primary );

            if ( !backgroundColor.IsEmpty )
            {
                var background = ToHex( backgroundColor );
                var color = ToHex( Contrast( theme, background ) );

                sb.Append( ".dropdown-item.active," )
                    .Append( ".dropdown-item:active" ).Append( "{" )
                    .Append( GetGradientBg( theme, background, options?.GradientBlendPercentage ) )
                    .Append( $"color: {color} !important;" )
                    .AppendLine( "}" );
            }
        }
    }

    protected override void GenerateInputStyles( StringBuilder sb, Theme theme, ThemeInputOptions options )
    {
        if ( !string.IsNullOrEmpty( options?.BorderRadius ) )
        {
            var borderRadius = GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) );

            sb.Append( ".form-control" ).Append( "{" )
                .Append( $"border-radius: {borderRadius};" )
                .AppendLine( "}" );

            sb.Append( ".input-group-text" ).Append( "{" )
                .Append( $"border-radius: {borderRadius};" )
                .AppendLine( "}" );

            sb.Append( ".custom-select" ).Append( "{" )
                .Append( $"border-radius: {borderRadius};" )
                .AppendLine( "}" );

            sb.Append( ".custom-checkbox .custom-control-label::before" ).Append( "{" )
                .Append( $"border-radius: {borderRadius};" )
                .AppendLine( "}" );

            sb.Append( ".custom-file-label" ).Append( "{" )
                .Append( $"border-radius: {borderRadius};" )
                .AppendLine( "}" );
        }

        if ( !string.IsNullOrEmpty( options?.Color ) )
        {
            sb.Append( ".form-control" ).Append( "{" )
                .Append( $"color: {options.Color};" )
                .AppendLine( "}" );

            sb.Append( ".input-group-text" ).Append( "{" )
                .Append( $"color: {options.Color};" )
                .AppendLine( "}" );

            sb.Append( ".custom-select" ).Append( "{" )
                .Append( $"color: {options.Color};" )
                .AppendLine( "}" );
        }

        if ( !string.IsNullOrEmpty( options?.CheckColor ) )
        {
            GenerateInputCheckEditStyles( sb, theme, options );
        }

        if ( !string.IsNullOrEmpty( theme.ColorOptions?.Primary ) )
        {
            var focusColor = ToHex( Lighten( Var( ThemeVariables.Color( "primary" ) ), 75f ) );

            sb
                .Append( ".form-control:focus," )
                .Append( ".custom-select:focus," )
                .Append( ".b-is-autocomplete.b-is-autocomplete-multipleselection.focus" )
                .Append( "{" )
                .Append( $"border-color: {focusColor};" )
                .Append( $"box-shadow: 0 0 0 {theme.ButtonOptions?.BoxShadowSize ?? ".2rem"} {focusColor};" )
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

            //sb
            //    .Append( $".flatpickr-time .flatpickr-am-pm" ).Append( "{" )
            //    .Append( $"color: { Var( ThemeVariables.Color( "primary" ) )};" )
            //    .AppendLine( "}" );

            //sb
            //    .Append( $".flatpickr-time .flatpickr-am-pm:focus, .flatpickr-time input:focus" ).Append( "{" )
            //    .Append( $"background: { ToHex( Transparency( Var( ThemeVariables.Color( "primary" ) ), 16 ) )};" )
            //    .Append( $"color: { Var( ThemeVariables.Color( "primary" ) )};" )
            //    .AppendLine( "}" );
        }
    }

    protected virtual void GenerateInputCheckEditStyles( StringBuilder sb, Theme theme, ThemeInputOptions options )
    {
        sb
            .Append( ".custom-checkbox .custom-control-input:checked ~ .custom-control-label::before" ).Append( "{" )
            .Append( $"background-color: {options.CheckColor};" )
            .AppendLine( "}" );

        sb
            .Append( ".custom-checkbox .custom-control-input:disabled:checked ~ .custom-control-label::before" ).Append( "{" )
            .Append( $"background-color: {ToHexRGBA( Transparency( options.CheckColor, 128 ) )};" )
            .AppendLine( "}" );

        sb
            .Append( ".custom-radio .custom-control-input:disabled:checked ~ .custom-control-label::before" ).Append( "{" )
            .Append( $"background-color: {ToHexRGBA( Transparency( options.CheckColor, 128 ) )};" )
            .AppendLine( "}" );

        sb
            .Append( ".custom-control-input:checked ~ .custom-control-label::before" ).Append( "{" )
            .Append( $"color: {options.Color};" )
            .Append( $"border-color: {options.CheckColor};" )
            .Append( $"background-color: {options.CheckColor};" )
            .AppendLine( "}" );

        sb
            .Append( ".custom-switch .custom-control-input:checked ~ .custom-control-label::before" ).Append( "{" )
            .Append( $"background-color: {options.CheckColor};" )
            .AppendLine( "}" );
    }

    protected override void GenerateBadgeVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor )
    {
        var backgroundColor = ParseColor( inBackgroundColor );

        if ( backgroundColor.IsEmpty )
            return;

        var yiqBackgroundColor = Contrast( theme, backgroundColor );

        var hexBackgroundColor = ToHex( backgroundColor );
        var hexYiqBackgroundColor = ToHex( yiqBackgroundColor );

        sb.Append( $".badge-{variant}" ).Append( "{" )
            .Append( $"color: {hexYiqBackgroundColor};" )
            .Append( $"background-color: {hexBackgroundColor};" )
            .AppendLine( "}" );

        // Subtle variant

        var hexBackgroundColorSubtle = Var( ThemeVariables.BackgroundSubtleColor( variant ) );
        var hexBorderColorSubtle = Var( ThemeVariables.BorderSubtleColor( variant ) );
        var hexTextColorEmphasis = Var( ThemeVariables.TextEmphasisColor( variant ) );

        sb.Append( $".badge.badge-{variant}-subtle," )
            .Append( $".badge-close.badge-{variant}-subtle" )
            .Append( "{" )
            .Append( $"color: {hexTextColorEmphasis};" )
            .Append( $"background-color: {hexBackgroundColorSubtle};" )
            .Append( $"border-color: {hexBorderColorSubtle};" )
            .AppendLine( "}" );
    }

    protected override void GenerateSwitchVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, ThemeSwitchOptions options )
    {
        var backgroundColor = ParseColor( inBackgroundColor );

        if ( backgroundColor.IsEmpty )
            return;

        var boxShadowColor = Lighten( backgroundColor, options?.BoxShadowLightenColor ?? 25 );
        var disabledBackgroundColor = Lighten( backgroundColor, options?.DisabledLightenColor ?? 50 );

        var background = ToHex( backgroundColor );
        var boxShadow = ToHex( boxShadowColor );
        var disabledBackground = ToHex( disabledBackgroundColor );

        sb
            .Append( $".custom-switch .custom-control-input.custom-control-input-{variant}:checked ~ .custom-control-label::before" ).Append( "{" )
            .Append( $"background-color: {background};" )
            .Append( $"border-color: {background};" )
            .AppendLine( "}" );

        sb
            .Append( $".custom-switch .custom-control-input.custom-control-input-{variant}:focus ~ .custom-control-label::before" ).Append( "{" )
            .Append( $"box-shadow: {boxShadow};" )
            .Append( $"border-color: {background};" )
            .AppendLine( "}" );

        sb
            .Append( $".custom-switch .custom-control-input:disabled.custom-control-input-{variant}:checked ~ .custom-control-label::before" ).Append( "{" )
            .Append( $"background-color: {disabledBackground};" )
            .AppendLine( "}" );
    }

    protected override void GenerateStepsStyles( StringBuilder sb, Theme theme, ThemeStepsOptions stepsOptions )
    {
        if ( stepsOptions is null )
            return;

        sb
            .Append( ".step-completed .step-circle" ).Append( "{" )
            .Append( $"color: {Var( ThemeVariables.White )};" )
            .Append( $"background-color: {Var( ThemeVariables.StepsItemIconCompleted, Var( ThemeVariables.Color( "success" ) ) )};" )
            .Append( $"border-color: {Var( ThemeVariables.StepsItemIconCompleted, Var( ThemeVariables.Color( "success" ) ) )};" )
            .AppendLine( "}" );

        sb
            .Append( ".step-completed .step-circle::before" ).Append( "{" )
            .Append( $"color: {Var( ThemeVariables.StepsItemIconCompleted, Var( ThemeVariables.Color( "success" ) ) )};" )
            .AppendLine( "}" );

        sb
            .Append( ".step-completed .step-text" ).Append( "{" )
            .Append( $"color: {Var( ThemeVariables.StepsItemTextCompleted, Var( ThemeVariables.Color( "success" ) ) )};" )
            .AppendLine( "}" );

        sb
            .Append( ".step-active .step-circle" ).Append( "{" )
            .Append( $"color: {Var( ThemeVariables.White )};" )
            .Append( $"background-color: {Var( ThemeVariables.StepsItemIconActive, Var( ThemeVariables.Color( "primary" ) ) )};" )
            .Append( $"border-color: {Var( ThemeVariables.StepsItemIconActive, Var( ThemeVariables.Color( "primary" ) ) )};" )
            .AppendLine( "}" );

        sb
            .Append( ".step-active .step-circle::before" ).Append( "{" )
            .Append( $"color: {Var( ThemeVariables.StepsItemIconActive, Var( ThemeVariables.Color( "primary" ) ) )};" )
            .AppendLine( "}" );

        sb
            .Append( ".step-active .step-text" ).Append( "{" )
            .Append( $"color: {Var( ThemeVariables.StepsItemTextActive, Var( ThemeVariables.Color( "primary" ) ) )};" )
            .AppendLine( "}" );
    }

    protected override void GenerateStepsVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, ThemeStepsOptions stepsOptions )
    {
        if ( stepsOptions is null )
            return;

        sb
            .Append( $".step-{variant} .step-circle" ).Append( "{" )
            .Append( $"color: {Var( ThemeVariables.VariantStepsItemIcon( variant ) )};" )
            .Append( $"border-color: {Var( ThemeVariables.VariantStepsItemIcon( variant ) )};" )
            .AppendLine( "}" );

        sb
            .Append( $".step-{variant}.step-completed .step-circle" ).Append( "{" )
            .Append( $"color: {Var( ThemeVariables.VariantStepsItemIconYiq( variant ) )};" )
            .Append( $"background-color: {Var( ThemeVariables.VariantStepsItemIcon( variant ) )};" )
            .Append( $"border-color: {Var( ThemeVariables.VariantStepsItemIcon( variant ) )};" )
            .AppendLine( "}" );

        sb
            .Append( $".step-{variant}.step-completed .step-circle::before" ).Append( "{" )
            .Append( $"color: {Var( ThemeVariables.VariantStepsItemIcon( variant ) )};" )
            .AppendLine( "}" );

        sb
            .Append( $".step-{variant}.step-completed .step-text" ).Append( "{" )
            .Append( $"color: {Var( ThemeVariables.VariantStepsItemText( variant ) )};" )
            .AppendLine( "}" );

        sb
            .Append( $".step-{variant}.step-active .step-circle" ).Append( "{" )
            .Append( $"color: {Var( ThemeVariables.StepsItemIconActiveYiq, Var( ThemeVariables.White ) )};" )
            .Append( $"background-color: {Var( ThemeVariables.StepsItemIconActive, Var( ThemeVariables.Color( "primary" ) ) )};" )
            .Append( $"border-color: {Var( ThemeVariables.StepsItemIconActive, Var( ThemeVariables.Color( "primary" ) ) )};" )
            .AppendLine( "}" );

        sb
            .Append( $".step-{variant}.step-active::before" ).Append( "{" )
            .Append( $"color: {Var( ThemeVariables.StepsItemIconActive, Var( ThemeVariables.Color( "primary" ) ) )};" )
            .AppendLine( "}" );

        sb
            .Append( $".step-{variant}.step-active .step-text" ).Append( "{" )
            .Append( $"color: {Var( ThemeVariables.StepsItemIconActive, Var( ThemeVariables.Color( "primary" ) ) )};" )
            .AppendLine( "}" );
    }

    protected override void GenerateRatingStyles( StringBuilder sb, Theme theme, ThemeRatingOptions ratingOptions )
    {
        if ( ratingOptions?.HoverOpacity != null )
        {
            sb
                .Append( ".rating .rating-item.rating-item-hover" ).Append( "{" )
                .Append( $"opacity: {string.Format( CultureInfo.InvariantCulture, "{0:F1}", ratingOptions.HoverOpacity )};" )
                .AppendLine( "}" );
        }
    }

    protected override void GenerateRatingVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, ThemeRatingOptions ratingOptions )
    {
        sb
            .Append( $".rating .rating-item.rating-item-{variant}" ).Append( "{" )
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

        sb.Append( $".alert-{variant}" ).Append( "{" )
            .Append( $"color: {text};" )
            .Append( GetGradientBg( theme, background, options?.GradientBlendPercentage ) )
            .Append( $"border-color: {border};" )
            .AppendLine( "}" );

        sb.Append( $".alert-{variant}.alert-link" ).Append( "{" )
            .Append( $"color: {alertLink};" )
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

        sb.Append( $".table-{variant}," )
            .Append( $".table-{variant}>th," )
            .Append( $".table-{variant}>td" )
            .Append( "{" )
            .Append( $"background-color: {background};" )
            .AppendLine( "}" );

        sb.Append( $".table-{variant} th," )
            .Append( $".table-{variant} td," )
            .Append( $".table-{variant} thead td," )
            .Append( $".table-{variant} tbody + tbody" )
            .Append( "{" )
            .Append( $"border-color: {border};" )
            .AppendLine( "}" );

        sb.Append( $".table-hover .table-{variant}:hover" )
            .Append( "{" )
            .Append( $"background-color: {hoverBackground};" )
            .AppendLine( "}" );

        sb.Append( $".table-hover .table-{variant}:hover>td," )
            .Append( $".table-hover .table-{variant}:hover>th" )
            .Append( "{" )
            .Append( $"background-color: {hoverBackground};" )
            .AppendLine( "}" );
    }

    protected override void GenerateCardStyles( StringBuilder sb, Theme theme, ThemeCardOptions options )
    {
        if ( !string.IsNullOrEmpty( options?.BorderRadius ) )
        {
            sb.Append( ".card" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );
        }

        if ( !string.IsNullOrEmpty( options?.ImageTopRadius ) )
            sb.Append( ".card-img-top" ).Append( "{" )
                .Append( $"border-top-left-radius: {options.ImageTopRadius};" )
                .Append( $"border-top-right-radius: {options.ImageTopRadius};" )
                .AppendLine( "}" );
    }

    protected override void GenerateModalStyles( StringBuilder sb, Theme theme, ThemeModalOptions options )
    {
        if ( !string.IsNullOrEmpty( options?.BorderRadius ) )
        {
            sb.Append( ".modal-content" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );
        }
    }

    protected override void GenerateTabsStyles( StringBuilder sb, Theme theme, ThemeTabsOptions options )
    {
        if ( !string.IsNullOrEmpty( options?.BorderRadius ) )
        {
            var borderRadius = GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) );

            sb.Append( ".nav-tabs .nav-link" ).Append( "{" )
                .Append( $"border-top-left-radius: {borderRadius};" )
                .Append( $"border-top-right-radius: {borderRadius};" )
                .AppendLine( "}" );

            sb.Append( ".nav-pills .nav-link" ).Append( "{" )
                .Append( $"border-radius: {borderRadius};" )
                .AppendLine( "}" );
        }

        if ( !string.IsNullOrEmpty( theme.ColorOptions?.Primary ) )
        {
            sb
                .Append( ".nav-pills .nav-link.active," )
                .Append( ".nav-pills .show>.nav-link" )
                .Append( "{" )
                .Append( $"background-color: {Var( ThemeVariables.Color( "primary" ) )};" )
                .AppendLine( "}" );

            sb
                .Append( ".nav.nav-tabs .nav-item a.nav-link:not(.active)" )
                .Append( "{" )
                .Append( $"color: {Var( ThemeVariables.Color( "primary" ) )};" )
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

        if ( !string.IsNullOrEmpty( theme.ColorOptions?.Primary ) )
        {
            sb.Append( ".progress-bar" ).Append( "{" )
                .Append( $"background-color: {Var( ThemeVariables.Color( "primary" ) )};" )
                .AppendLine( "}" );
        }

        base.GenerateProgressStyles( sb, theme, options );
    }

    protected override void GenerateAlertStyles( StringBuilder sb, Theme theme, ThemeAlertOptions options )
    {
        if ( !string.IsNullOrEmpty( options?.BorderRadius ) )
        {
            sb.Append( ".alert" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );
        }
    }

    protected override void GenerateBreadcrumbStyles( StringBuilder sb, Theme theme, ThemeBreadcrumbOptions options )
    {
        if ( !string.IsNullOrEmpty( options?.BorderRadius ) )
        {
            sb.Append( ".breadcrumb" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );
        }

        if ( !string.IsNullOrEmpty( Var( ThemeVariables.BreadcrumbColor ) ) )
        {
            sb.Append( ".breadcrumb-item>a" ).Append( "{" )
                .Append( $"color: {Var( ThemeVariables.BreadcrumbColor )};" )
                .AppendLine( "}" );
        }
    }

    protected override void GenerateBadgeStyles( StringBuilder sb, Theme theme, ThemeBadgeOptions options )
    {
        if ( !string.IsNullOrEmpty( options?.BorderRadius ) )
        {
            sb.Append( ".badge:not(.badge-pill)" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );
        }
    }

    protected override void GeneratePaginationStyles( StringBuilder sb, Theme theme, ThemePaginationOptions options )
    {
        if ( !string.IsNullOrEmpty( options?.BorderRadius ) )
        {
            var borderRadius = GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) );

            sb.Append( ".page-item:first-child .page-link" ).Append( "{" )
                .Append( $"border-top-left-radius: {borderRadius};" )
                .Append( $"border-bottom-left-radius: {borderRadius};" )
                .AppendLine( "}" );

            sb.Append( ".page-item:last-child .page-link" ).Append( "{" )
                .Append( $"border-top-right-radius: {borderRadius};" )
                .Append( $"border-bottom-right-radius: {borderRadius};" )
                .AppendLine( "}" );
        }

        if ( !string.IsNullOrEmpty( options?.LargeBorderRadius ) )
        {
            var largeBorderRadius = GetBorderRadius( theme, options?.LargeBorderRadius, Var( ThemeVariables.BorderRadius ) );

            sb.Append( ".pagination-lg .page-item:first-child .page-link" ).Append( "{" )
                .Append( $"border-top-left-radius: {largeBorderRadius};" )
                .Append( $"border-bottom-left-radius: {largeBorderRadius};" )
                .AppendLine( "}" );

            sb.Append( ".pagination-lg .page-item:last-child .page-link" ).Append( "{" )
                .Append( $"border-top-right-radius: {largeBorderRadius};" )
                .Append( $"border-bottom-right-radius: {largeBorderRadius};" )
                .AppendLine( "}" );
        }

        if ( !string.IsNullOrEmpty( theme.ColorOptions?.Primary ) )
        {
            sb.Append( ".page-link" ).Append( "{" )
                .Append( $"color: {theme.ColorOptions.Primary};" )
                .AppendLine( "}" );

            sb.Append( ".page-link:hover" ).Append( "{" )
                .Append( $"color: {ToHex( Darken( theme.ColorOptions.Primary, 15f ) )};" )
                .AppendLine( "}" );

            sb.Append( ".page-item.active .page-link" ).Append( "{" )
                .Append( $"color: {ToHex( Contrast( theme, theme.ColorOptions.Primary ) )};" )
                .Append( $"background-color: {theme.ColorOptions.Primary};" )
                .Append( $"border-color: {theme.ColorOptions.Primary};" )
                .AppendLine( "}" );
        }
    }

    protected override void GenerateBarStyles( StringBuilder sb, Theme theme, ThemeBarOptions options )
    {
        foreach ( var (variant, _) in theme.ValidBackgroundColors )
        {
            var yiqColor = Var( ThemeVariables.BackgroundYiqColor( variant ) );

            if ( string.IsNullOrEmpty( yiqColor ) )
                continue;

            sb.Append( $".navbar.bg-{variant} .navbar-brand .nav-item .nav-link" ).Append( "{" )
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
        var hexTextColorEmphasis = ToHex( ShadeColor( textColor, theme.TextColorOptions.EmphasisShadeWeight ?? 60 ) );

        sb.Append( $".text-{variant}" )
            .Append( "{" )
            .Append( $"color: {hexTextColor} !important;" )
            .AppendLine( "}" );

        sb.Append( $".text-{variant}-emphasis" )
            .Append( "{" )
            .Append( $"color: {hexTextColorEmphasis} !important;" )
            .AppendLine( "}" );
    }

    protected override void GenerateInputVariantStyles( StringBuilder sb, Theme theme, string variant, string inColor )
    {
        var color = ToHex( ParseColor( inColor ) );

        sb
            .Append( $".form-control.text-{variant}," )
            .Append( $".form-control-plaintext.text-{variant}" )
            .Append( "{" )
            .Append( $"color: {color};" )
            .AppendLine( "}" );
    }

    protected override void GenerateListGroupItemStyles( StringBuilder sb, Theme theme, ThemeListGroupItemOptions options )
    {
        if ( !string.IsNullOrEmpty( theme.ColorOptions?.Primary ) )
        {
            var white = Var( ThemeVariables.White );
            var primary = Var( ThemeVariables.Color( "primary" ) );

            sb
                .Append( ".list-group-item.active" )
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
        var hoverBackgroundColor = Darken( backgroundColor, 5 );

        var background = ToHex( backgroundColor );
        var hoverBackground = ToHex( hoverBackgroundColor );
        var color = ToHex( ParseColor( inColor ) );

        var white = Var( ThemeVariables.White );

        sb
            .Append( $".list-group-item-{variant}" )
            .Append( "{" )
            .Append( $"color: {color};" )
            .Append( GetGradientBg( theme, background, options?.GradientBlendPercentage ) )
            .AppendLine( "}" );

        sb
            .Append( $".list-group-item-{variant}.list-group-item-action:focus," )
            .Append( $".list-group-item-{variant}.list-group-item-action:hover" )
            .Append( "{" )
            .Append( $"color: {color};" )
            .Append( GetGradientBg( theme, hoverBackground, options?.GradientBlendPercentage ) )
            .AppendLine( "}" );

        sb
            .Append( $".list-group-item-{variant}.list-group-item-action.active" )
            .Append( "{" )
            .Append( $"color: {white};" )
            .Append( GetGradientBg( theme, color, options?.GradientBlendPercentage ) )
            .Append( $"border-color: {color};" )
            .AppendLine( "}" );
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
        if ( theme.BreakpointOptions == null || options == null )
            return;

        foreach ( var breakpoint in theme.BreakpointOptions )
        {
            var breakpointName = GetValidBreakpointName( breakpoint.Key );
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

            //sb
            //    .Append( $".m{infix}-auto" )
            //    .Append( "{" ).Append( $"margin: auto !important;" ).Append( "}" );

            //sb
            //    .Append( $".mt{infix}-auto," )
            //    .Append( $".my{infix}-auto" )
            //    .Append( "{" ).Append( $"margin-top: auto !important;" ).Append( "}" );

            //sb
            //    .Append( $".mr{infix}-auto," )
            //    .Append( $".mx{infix}-auto" )
            //    .Append( "{" ).Append( $"margin-right: auto !important;" ).Append( "}" );

            //sb
            //    .Append( $".mb{infix}-auto," )
            //    .Append( $".my{infix}-auto" )
            //    .Append( "{" ).Append( $"margin-bottom: auto !important;" ).Append( "}" );

            //sb
            //    .Append( $".ml{infix}-auto," )
            //    .Append( $".mx{infix}-auto" )
            //    .Append( "{" ).Append( $"margin-left: auto !important;" ).Append( "}" );

            if ( hasMinMedia )
            {
                sb.Append( "}" );
            }
        }
    }

    #endregion
}