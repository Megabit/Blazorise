#region Using directives
using System;
using System.Globalization;
using System.Text;
#endregion

namespace Blazorise.AntDesign.Providers;

public class AntDesignThemeGenerator : ThemeGenerator
{
    #region Constructors

    public AntDesignThemeGenerator( IThemeCache themeCache )
        : base( themeCache )
    {
    }

    #endregion

    #region Methods

    protected override void GenerateBackgroundVariantStyles( StringBuilder sb, Theme theme, string variant )
    {
        sb.Append( $".bg-{variant}" ).Append( "{" )
            .Append( $"background-color: {Var( ThemeVariables.BackgroundColor( variant ) )} !important;" )
            .AppendLine( "}" );

        sb.Append( $".ant-hero-{variant}" ).Append( "{" )
            .Append( $"background-color: {Var( ThemeVariables.BackgroundColor( variant ) )} !important;" )
            .Append( $"color: {ToHex( Contrast( theme, Var( ThemeVariables.BackgroundColor( variant ) ) ) )} !important;" )
            .AppendLine( "}" );
    }

    protected override void GenerateBorderVariantStyles( StringBuilder sb, Theme theme, string variant )
    {
        sb.Append( $".ant-border-{variant}" ).Append( "{" )
            .Append( $"border-color: {Var( ThemeVariables.BackgroundColor( variant ) )} !important;" )
            .AppendLine( "}" );
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

        sb.Append( $".ant-btn-{variant}" ).Append( "{" )
            .Append( $"color: {yiqBackground} !important;" )
            .Append( GetGradientBg( theme, background, options?.GradientBlendPercentage, true ) )
            .Append( $"border-color: {border} !important;" )
            .AppendLine( "}" );

        sb.Append( $".ant-btn-{variant} > a:only-child" ).Append( "{" )
            .Append( "color: currentColor !important;" )
            .AppendLine( "}" );

        sb.Append( $".ant-btn-{variant}:hover," )
            .Append( $".ant-btn-{variant}:focus" )
            .Append( "{" )
            .Append( $"color: {yiqHoverBackground} !important;" )
            .Append( GetGradientBg( theme, hoverBackground, options?.GradientBlendPercentage, true ) )
            .Append( $"border-color: {hoverBorder} !important;" )
            .AppendLine( "}" );

        sb.Append( $".ant-btn-{variant}:hover > a:only-child," )
            .Append( $".ant-btn-{variant}:focus > a:only-child" )
            .Append( "{" )
            .Append( "color: currentColor !important;" )
            .AppendLine( "}" );

        sb
            .Append( $".btn-{variant}:active," )
            .Append( $".btn-{variant}.active," )
            .Append( $".btn-{variant}-active" )
            .Append( "{" )
            .Append( $"color: {yiqActiveBackground} !important;" )
            .Append( $"background-color: {activeBackground} !important;" )
            .Append( $"border-color: {activeBorder} !important;" )
            .AppendLine( "}" );

        sb
            .Append( $".btn-{variant}:active > a:only-child," )
            .Append( $".btn-{variant}.active > a:only-child," )
            .Append( $".btn-{variant}-active > a:only-child" )
            .Append( "{" )
            .Append( "color: currentColor !important;" )
            .AppendLine( "}" );

        sb
            .Append( $".ant-btn-{variant}-disabled," )
            .Append( $".ant-btn-{variant}.disabled," )
            .Append( $".ant-btn-{variant}[disabled]," )
            .Append( $".ant-btn-{variant}-disabled:hover," )
            .Append( $".ant-btn-{variant}.disabled:hover," )
            .Append( $".ant-btn-{variant}[disabled]:hover," )
            .Append( $".ant-btn-{variant}-disabled:focus," )
            .Append( $".ant-btn-{variant}.disabled:focus," )
            .Append( $".ant-btn-{variant}[disabled]:focus," )
            .Append( $".ant-btn-{variant}-disabled:active," )
            .Append( $".ant-btn-{variant}.disabled:active," )
            .Append( $".ant-btn-{variant}[disabled]:active," )
            .Append( $".ant-btn-{variant}-disabled.active," )
            .Append( $".ant-btn-{variant}.disabled.active," )
            .Append( $".ant-btn-{variant}[disabled].active" )
            .Append( $".btn-{variant}:disabled" )
            .Append( "{" )
            .Append( "color: rgba(0, 0, 0, 0.25) !important;" )
            .Append( "background-color: #f5f5f5 !important;" )
            .Append( "border-color: #d9d9d9 !important;" )
            .Append( "text-shadow: none !important;" )
            .AppendLine( "}" );

        sb
            .Append( $".ant-btn-{variant}-disabled > a:only-child," )
            .Append( $".ant-btn-{variant}.disabled > a:only-child," )
            .Append( $".ant-btn-{variant}[disabled] > a:only-child," )
            .Append( $".ant-btn-{variant}-disabled:hover > a:only-child," )
            .Append( $".ant-btn-{variant}.disabled:hover > a:only-child," )
            .Append( $".ant-btn-{variant}[disabled]:hover > a:only-child," )
            .Append( $".ant-btn-{variant}-disabled:focus > a:only-child," )
            .Append( $".ant-btn-{variant}.disabled:focus > a:only-child," )
            .Append( $".ant-btn-{variant}[disabled]:focus > a:only-child," )
            .Append( $".ant-btn-{variant}-disabled:active > a:only-child," )
            .Append( $".ant-btn-{variant}.disabled:active > a:only-child," )
            .Append( $".ant-btn-{variant}[disabled]:active > a:only-child," )
            .Append( $".ant-btn-{variant}-disabled.active > a:only-child," )
            .Append( $".ant-btn-{variant}.disabled.active > a:only-child," )
            .Append( $".ant-btn-{variant}[disabled].active" )
            .Append( $".btn-{variant}:disabled" )
            .Append( "{" )
            .Append( "color: currentColor !important;" )
            .AppendLine( "}" );

        if ( options?.DisabledOpacity != null )
            sb.Append( $".ant-btn-{variant}[disabled]" ).Append( "{" )
                .Append( $"color: rgba(0, 0, 0, {options.DisabledOpacity}) !important;" )
                .AppendLine( "}" );
    }

    protected override void GenerateButtonOutlineVariantStyles( StringBuilder sb, Theme theme, string variant, ThemeButtonOptions options )
    {
        var color = Var( ThemeVariables.OutlineButtonColor( variant ) );
        var hoverColor = Var( ThemeVariables.OutlineButtonHoverColor( variant ) );
        var activeColor = Var( ThemeVariables.OutlineButtonActiveColor( variant ) );

        sb.Append( $".ant-btn-outline-{variant}" ).Append( "{" )
            .Append( $"color: {color} !important;" )
            .Append( "background: transparent !important;" )
            .Append( $"border-color: {color} !important;" )
            .AppendLine( "}" );

        sb.Append( $".ant-btn-outline-{variant} > a:only-child" ).Append( "{" )
            .Append( "color: currentColor !important;" )
            .AppendLine( "}" );

        sb.Append( $".ant-btn-outline-{variant}:hover," )
            .Append( $".ant-btn-outline-{variant}:focus" )
            .Append( "{" )
            .Append( $"color: {hoverColor} !important;" )
            .Append( $"border-color: {hoverColor} !important;" )
            .AppendLine( "}" );

        sb.Append( $".ant-btn-outline-{variant}:hover > a:only-child," )
            .Append( $".ant-btn-outline-{variant}:focus > a:only-child" )
            .Append( "{" )
            .Append( "color: currentColor !important;" )
            .AppendLine( "}" );

        sb
            .Append( $".ant-btn-outline-{variant}:active," )
            .Append( $".ant-btn-outline-{variant}.active" )
            .Append( "{" )
            .Append( $"color: {activeColor} !important;" )
            .Append( $"border-color: {activeColor} !important;" )
            .AppendLine( "}" );

        sb
            .Append( $".ant-btn-outline-{variant}:active > a:only-child," )
            .Append( $".ant-btn-outline-{variant}.active > a:only-child" )
            .Append( "{" )
            .Append( "color: currentColor !important;" )
            .AppendLine( "}" );

        sb
            .Append( $".ant-btn-outline-{variant}-disabled," )
            .Append( $".ant-btn-outline-{variant}.disabled," )
            .Append( $".ant-btn-outline-{variant}[disabled]," )
            .Append( $".ant-btn-outline-{variant}-disabled:hover," )
            .Append( $".ant-btn-outline-{variant}.disabled:hover," )
            .Append( $".ant-btn-outline-{variant}[disabled]:hover," )
            .Append( $".ant-btn-outline-{variant}-disabled:focus," )
            .Append( $".ant-btn-outline-{variant}.disabled:focus," )
            .Append( $".ant-btn-outline-{variant}[disabled]:focus," )
            .Append( $".ant-btn-outline-{variant}-disabled:active," )
            .Append( $".ant-btn-outline-{variant}.disabled:active," )
            .Append( $".ant-btn-outline-{variant}[disabled]:active," )
            .Append( $".ant-btn-outline-{variant}-disabled.active," )
            .Append( $".ant-btn-outline-{variant}.disabled.active," )
            .Append( $".ant-btn-outline-{variant}[disabled].active" )
            .Append( $".btn-{variant}:disabled" )
            .Append( "{" )
            .Append( "color: rgba(0, 0, 0, 0.25) !important;" )
            .Append( "border-color: #d9d9d9 !important;" )
            .Append( "text-shadow: none !important;" )
            .AppendLine( "}" );

        sb
            .Append( $".ant-btn-outline-{variant}-disabled > a:only-child," )
            .Append( $".ant-btn-outline-{variant}.disabled > a:only-child," )
            .Append( $".ant-btn-outline-{variant}[disabled] > a:only-child," )
            .Append( $".ant-btn-outline-{variant}-disabled:hover > a:only-child," )
            .Append( $".ant-btn-outline-{variant}.disabled:hover > a:only-child," )
            .Append( $".ant-btn-outline-{variant}[disabled]:hover > a:only-child," )
            .Append( $".ant-btn-outline-{variant}-disabled:focus > a:only-child," )
            .Append( $".ant-btn-outline-{variant}.disabled:focus > a:only-child," )
            .Append( $".ant-btn-outline-{variant}[disabled]:focus > a:only-child," )
            .Append( $".ant-btn-outline-{variant}-disabled:active > a:only-child," )
            .Append( $".ant-btn-outline-{variant}.disabled:active > a:only-child," )
            .Append( $".ant-btn-outline-{variant}[disabled]:active > a:only-child," )
            .Append( $".ant-btn-outline-{variant}-disabled.active > a:only-child," )
            .Append( $".ant-btn-outline-{variant}.disabled.active > a:only-child," )
            .Append( $".ant-btn-outline-{variant}[disabled].active" )
            .Append( $".btn-{variant}:disabled" )
            .Append( "{" )
            .Append( "color: currentColor !important;" )
            .AppendLine( "}" );
    }

    protected override void GenerateButtonStyles( StringBuilder sb, Theme theme, ThemeButtonOptions options )
    {
        sb.Append( ".ant-btn" ).Append( "{" )
            .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
            .AppendLine( "}" );

        sb.Append( ".ant-btn-sm" ).Append( "{" )
            .Append( $"border-radius: {GetBorderRadius( theme, options?.SmallBorderRadius, Var( ThemeVariables.BorderRadiusSmall ) )};" )
            .AppendLine( "}" );

        sb.Append( ".ant-btn-lg" ).Append( "{" )
            .Append( $"border-radius: {GetBorderRadius( theme, options?.LargeBorderRadius, Var( ThemeVariables.BorderRadiusLarge ) )};" )
            .AppendLine( "}" );

        if ( !string.IsNullOrEmpty( options?.Padding ) )
            sb.Append( ".ant-btn" ).Append( "{" )
                .Append( $"padding: {options.Padding};" )
                .AppendLine( "}" );

        if ( !string.IsNullOrEmpty( options?.Margin ) )
            sb.Append( ".ant-btn" ).Append( "{" )
                .Append( $"margin: {options.Margin};" )
                .AppendLine( "}" );

        if ( options?.DisabledOpacity != null )
            sb.Append( ".ant-btn[disabled]" ).Append( "{" )
                .Append( $"color: rgba(0, 0, 0, {options.DisabledOpacity}) !important;" )
                .AppendLine( "}" );
    }

    protected override void GenerateDropdownStyles( StringBuilder sb, Theme theme, ThemeDropdownOptions options )
    {
        sb.Append( ".ant-dropdown-menu" ).Append( "{" )
            .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
            .AppendLine( "}" );

        if ( !string.IsNullOrEmpty( theme.ColorOptions?.Primary ) )
        {
            var backgroundColor = ParseColor( theme.ColorOptions.Primary );

            if ( !backgroundColor.IsEmpty )
            {
                var background = ToHex( backgroundColor );

                sb.Append( ".ant-dropdown-menu-item.active," )
                    .Append( ".ant-dropdown-menu-item:active" ).Append( "{" )
                    .Append( GetGradientBg( theme, background, options?.GradientBlendPercentage ) )
                    .AppendLine( "}" );
            }
        }
    }

    protected override void GenerateInputStyles( StringBuilder sb, Theme theme, ThemeInputOptions options )
    {
        sb.Append( ".ant-form-item input" ).Append( "{" )
            .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
            .AppendLine( "}" );

        sb.Append( ".ant-input-group-addon:first-child" ).Append( "{" )
            .Append( $"border-top-left-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
            .Append( $"border-bottom-left-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
            .AppendLine( "}" );

        sb.Append( ".ant-input-group-addon:last-child" ).Append( "{" )
            .Append( $"border-top-right-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
            .Append( $"border-bottom-right-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
            .AppendLine( "}" );

        sb.Append( ".ant-select-selector, .ant-select-selector input" ).Append( "{" )
            .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )} !important;" )
            .AppendLine( "}" );

        sb.Append( ".ant-checkbox-inner" ).Append( "{" )
            .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
            .AppendLine( "}" );

        sb.Append( ".ant-upload button" ).Append( "{" )
            .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
            .AppendLine( "}" );

        sb.Append( ".b-is-autocomplete.b-is-autocomplete-multipleselection" ).Append( "{" )
            .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
            .AppendLine( "}" );

        if ( !string.IsNullOrEmpty( options?.Color ) )
        {
            sb.Append( ".ant-input" ).Append( "{" )
                .Append( $"color: {options.Color};" )
                .AppendLine( "}" );

            //sb.Append( $".input-group-text" ).Append( "{" )
            //    .Append( $"color: {options.Color};" )
            //    .AppendLine( "}" );

            sb.Append( ".ant-select-selection-search-input" ).Append( "{" )
                .Append( $"color: {options.Color};" )
                .AppendLine( "}" );
        }

        if ( !string.IsNullOrEmpty( options?.CheckColor ) )
        {
            GenerateInputCheckEditStyles( sb, theme, options );
        }

        if ( !string.IsNullOrEmpty( options?.SliderColor ) )
        {
            GenerateInputSliderStyles( sb, theme, options );
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
            .Append( ".ant-checkbox-checked .ant-checkbox-inner" ).Append( "{" )
            .Append( $"background-color: {options.CheckColor};" )
            .Append( $"border-color: {options.CheckColor};" )
            .AppendLine( "}" );

        sb
            .Append( ".ant-radio-wrapper:hover .ant-radio," )
            .Append( ".ant-radio:hover .ant-radio-inner," )
            .Append( ".ant-radio-input:focus + .ant-radio-inner," )
            .Append( ".ant-radio-checked .ant-radio-inner" )
            .Append( "{" )
            .Append( $"border-color: {options.CheckColor};" )
            .AppendLine( "}" );

        sb
            .Append( ".ant-radio-inner::after" ).Append( "{" )
            .Append( $"background-color: {options.CheckColor};" )
            .AppendLine( "}" );

        sb
            .Append( ".ant-switch-checked" ).Append( "{" )
            .Append( $"background-color: {options.CheckColor};" )
            .AppendLine( "}" );
    }

    protected virtual void GenerateInputSliderStyles( StringBuilder sb, Theme theme, ThemeInputOptions options )
    {
        sb.Append( ".ant-slider-track" ).Append( "{" )
            .Append( $"background-color: {options.SliderColor};" )
            .AppendLine( "}" );

        sb.Append( ".ant-slider:hover .ant-slider-track" ).Append( "{" )
            .Append( $"background-color: {ToHex( Darken( options.SliderColor, 20f ) )};" )
            .AppendLine( "}" );

        sb.Append( ".ant-slider-handle" ).Append( "{" )
            .Append( $"border-color: {options.SliderColor};" )
            .AppendLine( "}" );

        sb
            .Append( ".ant-slider-handle:focus," )
            .Append( ".ant-slider:hover .ant-slider-handle:not(.ant-tooltip-open)" )
            .Append( "{" )
            .Append( $"border-color: {ToHex( Darken( options.SliderColor, 20f ) )};" )
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

        sb.Append( $".ant-tag-{variant}" ).Append( "{" )
            .Append( $"color: {yiqBackground};" )
            .Append( $"background-color: {background};" )
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
            .Append( $".ant-switch.ant-switch-{variant}.ant-switch-checked" ).Append( "{" )
            .Append( $"background-color: {background};" )
            .AppendLine( "}" );

        sb
            .Append( $".ant-switch.ant-switch-{variant}:focus" ).Append( "{" )
            .Append( $"box-shadow: {boxShadow};" )
            .AppendLine( "}" );

        sb
            .Append( $".ant-switch:disabled.ant-switch-{variant}.ant-switch-checked" ).Append( "{" )
            .Append( $"background-color: {disabledBackground};" )
            .AppendLine( "}" );
    }

    protected override void GenerateStepsStyles( StringBuilder sb, Theme theme, ThemeStepsOptions stepsOptions )
    {
        sb
            .Append( ".ant-steps-item.ant-steps-item-active.ant-steps-item-process.ant-steps-item-finish .ant-steps-item-icon" ).Append( "{" )
            .Append( $"background-color: {Var( ThemeVariables.StepsItemIconActive, Var( ThemeVariables.Color( "primary" ) ) )};" )
            .AppendLine( "}" );

        sb
            .Append( ".ant-steps-item.ant-steps-item-active.ant-steps-item-process.ant-steps-item-finish .ant-steps-item-icon .ant-steps-icon" ).Append( "{" )
            .Append( $"color: {Var( ThemeVariables.StepsItemIconActiveYiq, Var( ThemeVariables.White ) )};" )
            .AppendLine( "}" );

        sb
            .Append( ".ant-steps-item.ant-steps-item-active.ant-steps-item-process .ant-steps-item-icon" ).Append( "{" )
            .Append( $"background: {Var( ThemeVariables.StepsItemIconActive, Var( ThemeVariables.Color( "primary" ) ) )};" )
            .Append( $"border-color: {Var( ThemeVariables.StepsItemIconActive, Var( ThemeVariables.Color( "primary" ) ) )};" )
            .AppendLine( "}" );
    }

    protected override void GenerateStepsVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, ThemeStepsOptions stepsOptions )
    {
        sb
            .Append( $".ant-steps-item-{variant}.ant-steps-item-finish .ant-steps-item-content .ant-steps-item-title" ).Append( "{" )
            .Append( $"color: {Var( ThemeVariables.VariantStepsItemText( variant ) )};" )
            .AppendLine( "}" );

        sb
            .Append( $".ant-steps-item-{variant}.ant-steps-item-wait .ant-steps-item-icon" ).Append( "{" )
            .Append( $"border-color: {Var( ThemeVariables.VariantStepsItemIcon( variant ) )};" )
            .Append( $"color: {Var( ThemeVariables.VariantStepsItemIconYiq( variant ) )};" )
            .AppendLine( "}" );

        sb
            .Append( $".ant-steps-item-{variant}.ant-steps-item-wait .ant-steps-item-content .ant-steps-item-title" ).Append( "{" )
            .Append( $"color: {Var( ThemeVariables.VariantStepsItemText( variant ) )};" )
            .AppendLine( "}" );

        sb
            .Append( $".ant-steps-item-{variant}.ant-steps-item-active.ant-steps-item-finish .ant-steps-item-icon" ).Append( "{" )
            .Append( $"background-color: {Var( ThemeVariables.StepsItemIconActive, Var( ThemeVariables.Color( "primary" ) ) )};" )
            .Append( $"border-color: {Var( ThemeVariables.StepsItemIconActive, Var( ThemeVariables.Color( "primary" ) ) )};" )
            .Append( $"color: {Var( ThemeVariables.StepsItemIconActiveYiq, Var( ThemeVariables.White ) )};" )
            .AppendLine( "}" );

        sb
            .Append( $".ant-steps-item-{variant}.ant-steps-item-active.ant-steps-item-process .ant-steps-icon" ).Append( "{" )
            .Append( $"border-color: {Var( ThemeVariables.StepsItemIconActive, Var( ThemeVariables.Color( "primary" ) ) )};" )
            .Append( $"color: {Var( ThemeVariables.StepsItemIconActiveYiq, Var( ThemeVariables.White ) )};" )
            .AppendLine( "}" );

        sb
            .Append( $".ant-steps-item-{variant}.ant-steps-item-active.ant-steps-item-process .ant-steps-item-content .ant-steps-item-title" ).Append( "{" )
            .Append( $"color: {Var( ThemeVariables.VariantStepsItemIcon( variant ) )};" )
            .AppendLine( "}" );

        sb
            .Append( $".ant-steps-item-{variant} .ant-steps-item-icon" ).Append( "{" )
            .Append( $"border-color: {Var( ThemeVariables.VariantStepsItemIcon( variant ) )};" )
            .AppendLine( "}" );

        sb
            .Append( $".ant-steps-item-{variant} .ant-steps-item-icon .ant-steps-icon" ).Append( "{" )
            .Append( $"color: {Var( ThemeVariables.VariantStepsItemIcon( variant ) )};" )
            .AppendLine( "}" );

        sb
            .Append( $".ant-steps-item-{variant} .ant-steps-item-content .ant-steps-item-title" ).Append( "{" )
            .Append( $"color: {Var( ThemeVariables.VariantStepsItemText( variant ) )};" )
            .AppendLine( "}" );
    }

    protected override void GenerateRatingStyles( StringBuilder sb, Theme theme, ThemeRatingOptions ratingOptions )
    {
        if ( ratingOptions?.HoverOpacity != null )
        {
            sb
                .Append( ".ant-rate .ant-rate-star.ant-rate-star-focused" ).Append( "{" )
                .Append( $"opacity: {string.Format( CultureInfo.InvariantCulture, "{0:F1}", ratingOptions.HoverOpacity )};" )
                .AppendLine( "}" );
        }
    }

    protected override void GenerateRatingVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, ThemeRatingOptions ratingOptions )
    {
        sb
            .Append( $".ant-rate .ant-rate-star.ant-rate-star-{variant}" ).Append( "{" )
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

        sb.Append( $".ant-alert-{variant}" ).Append( "{" )
            .Append( $"color: {text};" )
            .Append( GetGradientBg( theme, background, options?.GradientBlendPercentage ) )
            .Append( $"border-color: {border};" )
            .AppendLine( "}" );

        sb.Append( $".ant-alert-{variant}.alert-link" ).Append( "{" )
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

        sb.Append( $".ant-table-{variant}," )
            .Append( $".ant-table-{variant}>th," )
            .Append( $".ant-table-{variant}>td" )
            .Append( "{" )
            .Append( $"background-color: {background};" )
            .AppendLine( "}" );

        sb.Append( $".ant-table-{variant} th," )
            .Append( $".ant-table-{variant} td," )
            .Append( $".ant-table-{variant} thead td," )
            .Append( $".ant-table-{variant} tbody + tbody" )
            .Append( "{" )
            .Append( $"border-color: {border};" )
            .AppendLine( "}" );

        sb.Append( $".ant-table-hover .ant-table-{variant}:hover" )
            .Append( "{" )
            .Append( $"background-color: {hoverBackground};" )
            .AppendLine( "}" );

        sb.Append( $".ant-table-hover .ant-table-{variant}:hover>td" )
            .Append( $".ant-table-hover .ant-table-{variant}:hover>th" )
            .Append( "{" )
            .Append( $"background-color: {hoverBackground};" )
            .AppendLine( "}" );
    }

    protected override void GenerateCardStyles( StringBuilder sb, Theme theme, ThemeCardOptions options )
    {
        sb.Append( ".ant-card" ).Append( "{" )
            .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
            .AppendLine( "}" );

        sb.Append( ".ant-card-cover:first-child" ).Append( "{" )
            .Append( $"border-top-left-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
            .Append( $"border-top-right-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
            .AppendLine( "}" );

        sb.Append( ".ant-card-cover:last-child" ).Append( "{" )
            .Append( $"border-bottom-left-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
            .Append( $"border-bottom-right-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
            .AppendLine( "}" );

        if ( !string.IsNullOrEmpty( options?.ImageTopRadius ) )
            sb.Append( ".ant-card-cover" ).Append( "{" )
                .Append( $"border-top-left-radius: {options.ImageTopRadius};" )
                .Append( $"border-top-right-radius: {options.ImageTopRadius};" )
                .AppendLine( "}" );
    }

    protected override void GenerateModalStyles( StringBuilder sb, Theme theme, ThemeModalOptions options )
    {
        sb.Append( ".modal-content" ).Append( "{" )
            .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
            .AppendLine( "}" );
    }

    protected override void GenerateTabsStyles( StringBuilder sb, Theme theme, ThemeTabsOptions options )
    {
        sb.Append( ".ant-tabs .ant-tabs-tab" ).Append( "{" )
            .Append( $"border-top-left-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
            .Append( $"border-top-right-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
            .AppendLine( "}" );

        sb.Append( ".ant-tabs-pills .ant-tabs-nav .ant-tabs-tab" ).Append( "{" )
            .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
            .AppendLine( "}" );

        if ( !string.IsNullOrEmpty( theme.ColorOptions?.Primary ) )
        {
            sb
                .Append( ".ant-tabs-pills .ant-tabs-nav .ant-tabs-tab-active" )
                .Append( "{" )
                .Append( $"color: {Var( ThemeVariables.White )};" )
                .Append( $"background-color: {Var( ThemeVariables.Color( "primary" ) )};" )
                .AppendLine( "}" );

            sb
                .Append( ".ant-tabs-nav .ant-tabs-tab:active," )
                .Append( ".ant-tabs-nav .ant-tabs-tab-active" )
                .Append( "{" )
                .Append( $"color: {Var( ThemeVariables.Color( "primary" ) )};" )
                .AppendLine( "}" );

            var hoverColor = ToHex( Lighten( Var( ThemeVariables.Color( "primary" ) ), 20f ) );

            sb
                .Append( ".ant-tabs-nav .ant-tabs-tab:hover" )
                .Append( "{" )
                .Append( $"color: {hoverColor};" )
                .AppendLine( "}" );
        }
    }

    protected override void GenerateProgressStyles( StringBuilder sb, Theme theme, ThemeProgressOptions options )
    {
        sb
            .Append( ".ant-progress-inner," )
            .Append( ".ant-progress-bg" )
            .Append( "{" )
            .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
            .AppendLine( "}" );

        if ( !string.IsNullOrEmpty( theme.ColorOptions?.Primary ) )
        {
            sb.Append( ".ant-progress-bg" ).Append( "{" )
                .Append( $"background-color: {Var( ThemeVariables.Color( "primary" ) )};" )
                .AppendLine( "}" );
        }

        base.GenerateProgressStyles( sb, theme, options );
    }

    protected override void GenerateAlertStyles( StringBuilder sb, Theme theme, ThemeAlertOptions options )
    {
        sb.Append( ".ant-alert" ).Append( "{" )
            .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
            .AppendLine( "}" );
    }

    protected override void GenerateBreadcrumbStyles( StringBuilder sb, Theme theme, ThemeBreadcrumbOptions options )
    {
        sb.Append( ".ant-breadcrumb" ).Append( "{" )
            .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
            .AppendLine( "}" );


        if ( !string.IsNullOrEmpty( Var( ThemeVariables.BreadcrumbColor ) ) )
        {
            sb.Append( ".ant-breadcrumb-link>a" ).Append( "{" )
                .Append( $"color: {Var( ThemeVariables.BreadcrumbColor )};" )
                .AppendLine( "}" );
        }
    }

    protected override void GenerateBadgeStyles( StringBuilder sb, Theme theme, ThemeBadgeOptions options )
    {
        sb.Append( ".ant-tag:not(.ant-tag-pill)" ).Append( "{" )
            .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
            .AppendLine( "}" );
    }

    protected override void GeneratePaginationStyles( StringBuilder sb, Theme theme, ThemePaginationOptions options )
    {
        sb.Append( ".ant-pagination-item:first-child .ant-pagination-link" ).Append( "{" )
            .Append( $"border-top-left-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
            .Append( $"border-bottom-left-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
            .AppendLine( "}" );

        sb.Append( ".ant-pagination-item:last-child .ant-pagination-link" ).Append( "{" )
            .Append( $"border-top-right-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
            .Append( $"border-bottom-right-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
            .AppendLine( "}" );

        sb.Append( ".ant-pagination-lg .ant-pagination-item:first-child .ant-pagination-link" ).Append( "{" )
            .Append( $"border-top-left-radius: {GetBorderRadius( theme, options?.LargeBorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
            .Append( $"border-bottom-left-radius: {GetBorderRadius( theme, options?.LargeBorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
            .AppendLine( "}" );

        sb.Append( ".ant-pagination-lg .ant-pagination-item:last-child .ant-pagination-link" ).Append( "{" )
            .Append( $"border-top-right-radius: {GetBorderRadius( theme, options?.LargeBorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
            .Append( $"border-bottom-right-radius: {GetBorderRadius( theme, options?.LargeBorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
            .AppendLine( "}" );

        if ( !string.IsNullOrEmpty( theme.ColorOptions?.Primary ) )
        {
            var color = theme.ColorOptions.Primary;

            sb
                .Append( ".ant-pagination-item:focus," )
                .Append( ".ant-pagination-item:hover" )
                .Append( "{" )
                .Append( $"border-color: {color};" )
                .AppendLine( "}" );

            sb
                .Append( ".ant-pagination-item:focus a," )
                .Append( ".ant-pagination-item:hover a" )
                .Append( "{" )
                .Append( $"color: {color};" )
                .AppendLine( "}" );

            sb
                .Append( ".ant-pagination-item-active" )
                .Append( "{" )
                .Append( $"border-color: {color};" )
                .AppendLine( "}" );

            sb.Append( ".ant-pagination-item-active a" )
                .Append( "{" )
                .Append( $"color: {color};" )
                .AppendLine( "}" );

            var hoverColor = ToHex( Lighten( color, 40f ) );

            sb
                .Append( ".ant-pagination-item-active:focus," )
                .Append( ".ant-pagination-item-active:hover" )
                .Append( "{" )
                .Append( $"border-color: {hoverColor};" )
                .AppendLine( "}" );

            sb
                .Append( ".ant-pagination-item-active:focus a," )
                .Append( ".ant-pagination-item-active:hover a" )
                .Append( "{" )
                .Append( $"color: {hoverColor};" )
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

            sb
                .Append( $".ant-menu.bg-{variant} .ant-menu-item .ant-menu-link," )
                .Append( $".ant-menu.bg-{variant} .ant-menu-item .ant-menu-submenu-title span" )
                .Append( "{" )
                .Append( $"color: {yiqColor};" )
                .AppendLine( "}" );
        }

        sb
            .Append( ".ant-menu.ant-menu-root.ant-menu-dark.ant-menu-inline" )
            .Append( "{" )
            .Append( $"background-color: var(--b-bar-dark-background, #001529);" )
            .Append( $"color: var(--b-bar-dark-color, rgba(255, 255, 255, 0.5));" )
            .AppendLine( "}" );
    }

    protected override void GenerateParagraphVariantStyles( StringBuilder sb, Theme theme, string variant, string inTextColor )
    {
        var textColor = variant == "body" && !string.IsNullOrEmpty( theme.BodyOptions?.TextColor )
            ? ParseColor( theme.BodyOptions.TextColor )
            : ParseColor( inTextColor );

        var hexTextColor = ToHex( textColor );

        sb.Append( $".ant-typography-{variant}" )
            .Append( "{" )
            .Append( $"color: {hexTextColor};" )
            .AppendLine( "}" );
    }

    protected override void GenerateInputVariantStyles( StringBuilder sb, Theme theme, string variant, string inColor )
    {
        var color = ToHex( ParseColor( inColor ) );

        sb
            .Append( $".ant-input.ant-form-text-{variant}," )
            .Append( $".ant-form-text.ant-form-text-{variant}" )
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
                .Append( ".ant-list-item.active" )
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

        sb.Append( $".ant-list .ant-list-items > .ant-list-item.ant-list-item-{variant}" ).Append( "{" )
            .Append( $"color: {text};" )
            .Append( GetGradientBg( theme, background, options?.GradientBlendPercentage ) )
            .AppendLine( "}" );

        sb
            .Append( $".ant-list .ant-list-items > .ant-list-item.ant-list-item-{variant}.ant-list-item-actionable:hover," )
            .Append( $".ant-list .ant-list-items > .ant-list-item.ant-list-item-{variant}.ant-list-item-actionable:focus" )
            .Append( "{" )
            .Append( $"color: {text};" )
            .Append( ToHex( Darken( GetGradientBg( theme, background, options?.GradientBlendPercentage ), 5 ) ) )
            .AppendLine( "}" );

        sb
            .Append( $".ant-list .ant-list-items > .ant-list-item.ant-list-item-{variant}.ant-list-item-actionable.active" )
            .Append( "{" )
            .Append( "color: #fff;" )
            .Append( $"background-color: {text};" )
            .Append( $"border-color: {text};" )
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

            if ( hasMinMedia )
            {
                sb.Append( "}" );
            }
        }
    }

    #endregion
}