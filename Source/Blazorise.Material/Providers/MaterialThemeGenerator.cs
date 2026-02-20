#region Using directives
using System;
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

    protected override void GenerateBackgroundVariantStyles( StringBuilder sb, Theme theme, string variant )
    {
    }

    protected override void GenerateBorderVariantStyles( StringBuilder sb, Theme theme, string variant )
    {
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

        sb
            .Append( $".modal .btn-{variant}," ).Append( $".modal a.btn-{variant}," )
            .Append( $".modal-footer .btn-{variant}," ).Append( $".modal-footer a.btn-{variant}" )
            .Append( "{" )
            .Append( $"color: {yiqBackground};" )
            .Append( GetGradientBg( theme, background, options?.GradientBlendPercentage ) )
            .Append( $"border-color: {border};" )
            .AppendLine( "}" );

        sb
            .Append( $".modal .btn-{variant}:hover," ).Append( $".modal a.btn-{variant}:hover," )
            .Append( $".modal-footer .btn-{variant}:hover," ).Append( $".modal-footer a.btn-{variant}:hover" )
            .Append( "{" )
            .Append( $"color: {yiqHoverBackground};" )
            .Append( GetGradientBg( theme, hoverBackground, options?.GradientBlendPercentage ) )
            .Append( $"border-color: {hoverBorder};" )
            .AppendLine( "}" );

        sb
            .Append( $".modal .btn-{variant}:focus," ).Append( $".modal .btn-{variant}.focus," ).Append( $".modal a.btn-{variant}:focus," ).Append( $".modal a.btn-{variant}.focus," )
            .Append( $".modal-footer .btn-{variant}:focus," ).Append( $".modal-footer .btn-{variant}.focus," ).Append( $".modal-footer a.btn-{variant}:focus," ).Append( $".modal-footer a.btn-{variant}.focus" )
            .Append( "{" )
            .Append( $"color: {yiqHoverBackground};" )
            .Append( GetGradientBg( theme, hoverBackground, options?.GradientBlendPercentage ) )
            .Append( $"border-color: {hoverBorder};" )
            .Append( $"box-shadow: 0 0 0 {options?.BoxShadowSize ?? ".2rem"} {boxShadow};" )
            .AppendLine( "}" );

        sb
            .Append( $".modal .btn-{variant}.disabled," ).Append( $".modal .btn-{variant}:disabled," ).Append( $".modal a.btn-{variant}.disabled," ).Append( $".modal a.btn-{variant}:disabled," )
            .Append( $".modal-footer .btn-{variant}.disabled," ).Append( $".modal-footer .btn-{variant}:disabled," ).Append( $".modal-footer a.btn-{variant}.disabled," ).Append( $".modal-footer a.btn-{variant}:disabled" )
            .Append( "{" )
            .Append( $"color: {yiqBackground};" )
            .Append( $"background-color: {background};" )
            .Append( $"border-color: {border};" )
            .Append( $"box-shadow: 0 0 0 {options?.BoxShadowSize ?? ".2rem"} {boxShadow};" )
            .AppendLine( "}" );

        sb
            .Append( $".modal .btn-{variant}:not(:disabled):not(.disabled):active," ).Append( $".modal .btn-{variant}:not(:disabled):not(.disabled).active," ).Append( $".modal .show>.btn-{variant}.dropdown-toggle," ).Append( $".modal a.btn-{variant}:not(:disabled):not(.disabled):active," ).Append( $".modal a.btn-{variant}:not(:disabled):not(.disabled).active," ).Append( $".modal a.show>.btn-{variant}.dropdown-toggle," )
            .Append( $".modal-footer .btn-{variant}:not(:disabled):not(.disabled):active," ).Append( $".modal-footer .btn-{variant}:not(:disabled):not(.disabled).active," ).Append( $".modal-footer .show>.btn-{variant}.dropdown-toggle," ).Append( $".modal-footer a.btn-{variant}:not(:disabled):not(.disabled):active," ).Append( $".modal-footer a.btn-{variant}:not(:disabled):not(.disabled).active," ).Append( $".modal-footer a.show>.btn-{variant}.dropdown-toggle" )
            .Append( "{" )
            .Append( $"color: {yiqActiveBackground};" )
            .Append( $"background-color: {activeBackground};" )
            .Append( $"border-color: {activeBorder};" )
            .AppendLine( "}" );

        sb
            .Append( $".modal .btn-{variant}:not(:disabled):not(.disabled):active:focus," ).Append( $".modal .btn-{variant}:not(:disabled):not(.disabled).active:focus," ).Append( $".modal .show>.btn-{variant}.dropdown-toggle:focus," ).Append( $".modal a.btn-{variant}:not(:disabled):not(.disabled):active:focus," ).Append( $".modal a.btn-{variant}:not(:disabled):not(.disabled).active:focus," ).Append( $".modal a.show>.btn-{variant}.dropdown-toggle:focus," )
            .Append( $".modal-footer .btn-{variant}:not(:disabled):not(.disabled):active:focus," ).Append( $".modal-footer .btn-{variant}:not(:disabled):not(.disabled).active:focus," ).Append( $".modal-footer .show>.btn-{variant}.dropdown-toggle:focus," ).Append( $".modal-footer a.btn-{variant}:not(:disabled):not(.disabled):active:focus," ).Append( $".modal-footer a.btn-{variant}:not(:disabled):not(.disabled).active:focus," ).Append( $".modal-footer a.show>.btn-{variant}.dropdown-toggle:focus" )
            .Append( "{" )
            .Append( $"box-shadow: 0 0 0 {options?.BoxShadowSize ?? ".2rem"} {boxShadow}" )
            .AppendLine( "}" );
    }

    protected override void GenerateButtonOutlineVariantStyles( StringBuilder sb, Theme theme, string variant, ThemeButtonOptions options )
    {
        if ( options is null )
            return;

        var color = Var( ThemeVariables.OutlineButtonColor( variant ) );

        sb
            .Append( $".btn-outline-{variant}," )
            .Append( $".btn-outline-{variant}.active," )
            .Append( $".btn-outline-{variant}:focus," )
            .Append( $".btn-outline-{variant}:hover," )
            .Append( $"a.btn-outline-{variant}," )
            .Append( $"a.btn-outline-{variant}.active," )
            .Append( $"a.btn-outline-{variant}:focus," )
            .Append( $"a.btn-outline-{variant}:hover" )
            .Append( "{" )
            .Append( $"color: {color};" )
            .AppendLine( "}" );

        sb
            .Append( $".btn-outline-{variant}.disabled," )
            .Append( $".btn-outline-{variant}:disabled," )
            .Append( $"a.btn-outline-{variant}.disabled," )
            .Append( $"a.btn-outline-{variant}:disabled" )
            .Append( "{" )
            .Append( $"color: {color};" )
            .AppendLine( "}" );
    }

    protected override void GenerateButtonStyles( StringBuilder sb, Theme theme, ThemeButtonOptions options )
    {
    }

    protected override void GenerateDropdownStyles( StringBuilder sb, Theme theme, ThemeDropdownOptions options )
    {
    }

    protected override void GenerateInputStyles( StringBuilder sb, Theme theme, ThemeInputOptions options )
    {
        if ( options is null )
            return;

        if ( !string.IsNullOrEmpty( options.CheckColor ) )
            GenerateInputCheckEditStyles( sb, theme, options );

        if ( !string.IsNullOrEmpty( theme.ColorOptions?.Primary ) )
        {
            var focusColor = Var( ThemeVariables.Color( "primary" ) );

            sb
                .Append( ".form-control:focus," )
                .Append( ".custom-select:focus" )
                .Append( "{" )
                .Append( $"border-color: {focusColor};" )
                .Append( $"box-shadow: inset 0 -2px 0 -1px {focusColor};" )
                .AppendLine( "}" );

            sb.Append( ".b-is-autocomplete.b-is-autocomplete-multipleselection.focus" )
                .Append( "{" )
                .Append( "border:none;" )
                .Append( "box-shadow: none;" )
                .AppendLine( "}" );

            sb
                .Append( "select.custom-select:focus[multiple], select.custom-select:focus[size]:not([size=\"1\"]), select.form-control:focus[multiple], select.form-control:focus[size]:not([size=\"1\"]), textarea.form-control:focus:not([rows=\"1\"])" )
                .Append( "{" )
                .Append( $"border-color: {focusColor};" )
                .Append( $"box-shadow: inset 2px 2px 0 -1px {focusColor}, inset -2px -2px 0 -1px {focusColor};" )
                .AppendLine( "}" );

            sb
                .Append( ".form-group:focus-within label:not(.custom-control-label):not(.form-check-label):not(.btn):not(.card-link), [class*=form-ripple]:focus-within label:not(.custom-control-label):not(.form-check-label):not(.btn):not(.card-link)" )
                .Append( "{" )
                .Append( $"color: {focusColor};" )
                .AppendLine( "}" );
        }

        var validationSuccessColor = Var( ThemeVariables.Color( "success" ) );
        var validationDangerColor = Var( ThemeVariables.Color( "danger" ) );

        if ( !string.IsNullOrEmpty( validationSuccessColor ) )
        {
            sb.Append( ".b-is-autocomplete.is-valid" ).Append( "{" )
                .Append( $"border-color: {validationSuccessColor};" )
                .AppendLine( "}" );
        }

        if ( !string.IsNullOrEmpty( validationDangerColor ) )
        {
            sb.Append( ".b-is-autocomplete.is-invalid" ).Append( "{" )
                .Append( $"border-color: {validationDangerColor};" )
                .AppendLine( "}" );
        }
    }

    protected void GenerateInputCheckEditStyles( StringBuilder sb, Theme theme, ThemeInputOptions options )
    {
        if ( options is null )
            return;

        if ( string.IsNullOrEmpty( options.CheckColor ) )
            return;

        sb
            .Append( ".custom-checkbox .custom-control-input:checked ~ .custom-control-label::before" ).Append( "{" )
            .Append( $"background-color: {options.CheckColor};" )
            .AppendLine( "}" );

        sb
            .Append( ".custom-checkbox .custom-control-input:checked~.custom-control-label:after" ).Append( "{" )
            .Append( $"content: url(\"data:image/svg+xml;charset=utf-8,{GenerateSvgDataUrl( options.CheckColor, 1 )}\");" )
            .AppendLine( "}" );

        sb
            .Append( ".custom-switch .custom-control-input:checked ~ .custom-control-label::before" ).Append( "{" )
            .Append( $"background-color: {options.CheckColor};" )
            .AppendLine( "}" );

        sb
            .Append( ".custom-switch .custom-control-input:checked ~ .custom-control-label::after" ).Append( "{" )
            .Append( $"background-color: {options.CheckColor};" )
            .AppendLine( "}" );

        var trackColor = ToHex( Lighten( options.CheckColor, 50f ) );

        if ( !string.IsNullOrEmpty( trackColor ) )
        {
            sb
                .Append( ".custom-switch .custom-control-input:checked~.custom-control-track" ).Append( "{" )
                .Append( $"background-color: {trackColor};" )
                .AppendLine( "}" );
        }
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
        if ( options is null )
            return;

        var backgroundColor = ParseColor( inBackgroundColor );

        if ( backgroundColor.IsEmpty )
            return;

        var boxShadowColor = Lighten( backgroundColor, options.BoxShadowLightenColor ?? 25 );
        var disabledBackgroundColor = Lighten( backgroundColor, options.DisabledLightenColor ?? 50 );

        var background = ToHex( backgroundColor );
        var boxShadow = ToHex( boxShadowColor );
        var disabledBackground = ToHex( disabledBackgroundColor );

        sb
            .Append( $".custom-switch .custom-control-input:checked.custom-control-input-{variant} ~ .custom-control-label::after" ).Append( "{" )
            .Append( $"background-color: {background};" )
            .AppendLine( "}" );

        sb
            .Append( $".custom-switch .custom-control-input:checked.custom-control-input-{variant} ~ .custom-control-track" ).Append( "{" )
            .Append( $"background-color: {boxShadow};" )
            .AppendLine( "}" );

        sb
            .Append( $".custom-switch .custom-control-input:disabled.custom-control-input-{variant} ~ .custom-control-label::after" ).Append( "{" )
            .Append( $"background-color: {boxShadow};" )
            .AppendLine( "}" );

        sb
            .Append( $".custom-switch .custom-control-input:disabled.custom-control-input-{variant} ~ .custom-control-track" ).Append( "{" )
            .Append( $"background-color: {disabledBackground};" )
            .AppendLine( "}" );
    }

    protected override void GenerateStepsVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, ThemeStepsOptions stepsOptions )
    {
        if ( stepsOptions is null )
            return;

        sb
            .Append( $".stepper-{variant}.done .stepper-icon" ).Append( "{" )
            .Append( $"background-color: {Var( ThemeVariables.VariantStepsItemIcon( variant ) )};" )
            .AppendLine( "}" );

        sb
            .Append( $".stepper-{variant}.done .stepper-text" ).Append( "{" )
            .Append( $"color: {Var( ThemeVariables.VariantStepsItemText( variant ) )};" )
            .AppendLine( "}" );

        sb
            .Append( $".stepper-{variant}.active .stepper-icon" ).Append( "{" )
            .Append( $"color: {Var( ThemeVariables.StepsItemIconActiveYiq )};" )
            .Append( $"background-color: {Var( ThemeVariables.StepsItemIconActive, Var( ThemeVariables.Color( "primary" ) ) )};" )
            .AppendLine( "}" );

        sb
            .Append( $".stepper-{variant}.active .stepper-text" ).Append( "{" )
            .Append( $"color: {Var( ThemeVariables.VariantStepsItemText( variant ) )};" )
            .AppendLine( "}" );

        sb
            .Append( $".stepper-{variant} .stepper-icon" ).Append( "{" )
            .Append( $"background-color: {Var( ThemeVariables.VariantStepsItemIcon( variant ) )};" )
            .AppendLine( "}" );

        sb
            .Append( $".stepper-{variant} .stepper-icon" ).Append( "{" )
            .Append( $"color: {Var( ThemeVariables.VariantStepsItemIconYiq( variant ) )};" )
            .AppendLine( "}" );
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

        if ( !string.IsNullOrEmpty( theme.ColorOptions?.Primary ) )
        {
            sb.Append( ".mui-progress" ).Append( "{" )
                .Append( $"--mui-progress-color: {Var( ThemeVariables.Color( "primary" ) )};" )
                .Append( $"--mui-progress-text-color: {Var( ThemeVariables.ButtonYiqBackground( "primary" ), Var( ThemeVariables.White ) )};" )
                .AppendLine( "}" );
        }

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
        if ( stepsOptions is null )
            return;

        sb
            .Append( ".stepper.active .stepper-icon" ).Append( "{" )
            .Append( $"color: {Var( ThemeVariables.StepsItemIconActiveYiq )};" )
            .Append( $"background-color: {Var( ThemeVariables.StepsItemIconActive, Var( ThemeVariables.Color( "primary" ) ) )};" )
            .AppendLine( "}" );
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

    #endregion
}