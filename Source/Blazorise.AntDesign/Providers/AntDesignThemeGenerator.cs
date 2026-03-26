#region Using directives
using System;
using System.Globalization;
using System.Text;
using Blazorise.Extensions;
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

    protected override void GenerateBodyVariables( Theme theme )
    {
        base.GenerateBodyVariables( theme );

        var borderRadius = GetBorderRadius( theme, theme.InputOptions?.BorderRadius, Var( ThemeVariables.BorderRadius ) );
        var borderRadiusLarge = GetBorderRadius( theme, theme.ButtonOptions?.LargeBorderRadius, Var( ThemeVariables.BorderRadiusLarge ) );
        var borderRadiusSmall = GetBorderRadius( theme, theme.ButtonOptions?.SmallBorderRadius, Var( ThemeVariables.BorderRadiusSmall ) );

        SetAntToken( "--ant-border-radius-xs", borderRadiusSmall );
        SetAntToken( "--ant-border-radius-sm", borderRadiusSmall );
        SetAntToken( "--ant-border-radius", borderRadius );
        SetAntToken( "--ant-border-radius-lg", borderRadiusLarge );
        SetAntToken( "--ant-border-radius-outer", borderRadiusSmall );

        SetAntToken( "--ant-font-family", Var( ThemeVariables.BodyFontFamily ) );
        SetAntToken( "--ant-font-size", Var( ThemeVariables.BodyFontSize ) );
        SetAntToken( "--ant-color-text-base", Var( ThemeVariables.BodyTextColor ) );
        SetAntToken( "--ant-color-text", Var( ThemeVariables.BodyTextColor ) );
        SetAntToken( "--ant-color-text-heading", Var( ThemeVariables.TextEmphasisColor( "body" ) ) );
        SetAntToken( "--ant-color-text-secondary", Var( ThemeVariables.TextColor( "secondary" ) ) );
        SetAntToken( "--ant-color-text-description", Var( ThemeVariables.TextColor( "muted" ) ) );
        SetAntToken( "--ant-color-text-light-solid", Var( ThemeVariables.TextColor( "white" ) ) );
        SetAntToken( "--ant-color-bg-base", Var( ThemeVariables.BodyBackgroundColor ) );
        SetAntToken( "--ant-color-bg-container", Var( ThemeVariables.BodyBackgroundColor ) );
        SetAntToken( "--ant-color-bg-elevated", Var( ThemeVariables.BodyBackgroundColor ) );

        if ( !string.IsNullOrEmpty( theme.ProgressOptions?.BorderRadius ) )
        {
            SetAntToken(
                "--ant-progress-line-border-radius",
                GetBorderRadius( theme, theme.ProgressOptions.BorderRadius, borderRadius ) );
        }
    }

    protected override void GenerateColorVariables( Theme theme, string variant, string value )
    {
        base.GenerateColorVariables( theme, variant, value );

        if ( string.IsNullOrEmpty( value ) )
            return;

        switch ( variant )
        {
            case "primary":
                GenerateAntPrimaryColorVariables( theme, value );

                if ( string.IsNullOrEmpty( theme.ColorOptions?.Link ) )
                    GenerateAntLinkColorVariables( value );

                break;
            case "success":
                GenerateAntSemanticColorVariables( theme, "success", value );
                break;
            case "danger":
                GenerateAntSemanticColorVariables( theme, "error", value );
                break;
            case "warning":
                GenerateAntSemanticColorVariables( theme, "warning", value );
                break;
            case "info":
                GenerateAntSemanticColorVariables( theme, "info", value );
                break;
            case "link":
                GenerateAntLinkColorVariables( value );
                break;
        }
    }

    protected override void GenerateBarVariables( Theme theme, ThemeBarOptions barOptions )
    {
        base.GenerateBarVariables( theme, barOptions );

        if ( barOptions is null )
            return;

        if ( !string.IsNullOrEmpty( barOptions.HorizontalHeight ) && barOptions.HorizontalHeight != "auto" )
            SetAntToken( "--ant-menu-horizontal-line-height", barOptions.HorizontalHeight );

        if ( !string.IsNullOrEmpty( barOptions.VerticalSmallWidth ) )
            SetAntToken( "--ant-menu-collapsed-width", barOptions.VerticalSmallWidth );

        GenerateBarColorTokens( barOptions.DarkColors, isDark: true );
        GenerateBarColorTokens( barOptions.LightColors, isDark: false );
    }

    private void GenerateAntPrimaryColorVariables( Theme theme, string color )
    {
        var baseColor = ParseColor( color );

        if ( baseColor.IsEmpty )
            return;

        var primary1 = ToHex( TintColor( baseColor, 88 ) );
        var primary2 = ToHex( TintColor( baseColor, 75 ) );
        var primary3 = ToHex( TintColor( baseColor, 60 ) );
        var primary4 = ToHex( TintColor( baseColor, 45 ) );
        var primary5 = ToHex( Lighten( baseColor, 20f ) );
        var primary6 = ToHex( baseColor );
        var primary7 = ToHex( Darken( baseColor, 15f ) );
        var outline = ToHexRGBA( Transparency( baseColor, 51 ) );

        SetAntToken( "--ant-primary-1", primary1 );
        SetAntToken( "--ant-primary-2", primary2 );
        SetAntToken( "--ant-primary-3", primary3 );
        SetAntToken( "--ant-primary-4", primary4 );
        SetAntToken( "--ant-primary-5", primary5 );
        SetAntToken( "--ant-primary-6", primary6 );
        SetAntToken( "--ant-primary-7", primary7 );
        SetAntToken( "--ant-primary-color", primary6 );
        SetAntToken( "--ant-primary-color-hover", primary5 );
        SetAntToken( "--ant-primary-color-active", primary7 );
        SetAntToken( "--ant-primary-color-outline", outline );
        SetAntToken( "--ant-color-primary-bg", primary1 );
        SetAntToken( "--ant-color-primary-bg-hover", primary2 );
        SetAntToken( "--ant-color-primary-border", primary3 );
        SetAntToken( "--ant-color-primary-border-hover", primary4 );
        SetAntToken( "--ant-color-primary-hover", primary5 );
        SetAntToken( "--ant-color-primary", primary6 );
        SetAntToken( "--ant-color-primary-active", primary7 );
        SetAntToken( "--ant-color-primary-text-hover", primary5 );
        SetAntToken( "--ant-color-primary-text", primary6 );
        SetAntToken( "--ant-color-primary-text-active", primary7 );
        SetAntToken( "--ant-control-outline", outline );
        SetAntToken( "--ant-menu-dark-item-selected-bg", primary6 );
        SetAntToken( "--ant-menu-item-selected-color", primary6 );
        SetAntToken( "--ant-menu-item-selected-bg", primary1 );
        SetAntToken( "--ant-menu-item-active-bg", primary1 );
        SetAntToken( "--ant-menu-horizontal-item-selected-color", primary6 );
    }

    private void GenerateAntSemanticColorVariables( Theme theme, string antVariant, string color )
    {
        var baseColor = ParseColor( color );

        if ( baseColor.IsEmpty )
            return;

        var background = ToHex( TintColor( baseColor, theme?.BackgroundOptions?.SubtleTintWeight ?? 80 ) );
        var border = ToHex( TintColor( baseColor, theme?.BorderOptions?.SubtleTintWeight ?? 60 ) );
        var text = ToHex( ShadeColor( baseColor, theme?.TextColorOptions?.EmphasisShadeWeight ?? 60 ) );
        var hover = ToHex( Lighten( baseColor, 20f ) );
        var active = ToHex( Darken( baseColor, 15f ) );
        var outline = ToHexRGBA( Transparency( baseColor, 51 ) );
        var deprecatedPrefix = $"--ant-{antVariant}-color";

        SetAntToken( deprecatedPrefix, color );
        SetAntToken( $"{deprecatedPrefix}-hover", hover );
        SetAntToken( $"{deprecatedPrefix}-active", active );
        SetAntToken( $"{deprecatedPrefix}-outline", outline );
        SetAntToken( $"--ant-color-{antVariant}", color );
        SetAntToken( $"--ant-color-{antVariant}-hover", hover );
        SetAntToken( $"--ant-color-{antVariant}-active", active );
        SetAntToken( $"--ant-color-{antVariant}-bg", background );
        SetAntToken( $"--ant-color-{antVariant}-bg-hover", background );
        SetAntToken( $"--ant-color-{antVariant}-border", border );
        SetAntToken( $"--ant-color-{antVariant}-border-hover", border );
        SetAntToken( $"--ant-color-{antVariant}-text", text );
        SetAntToken( $"--ant-color-{antVariant}-text-hover", hover );
        SetAntToken( $"--ant-color-{antVariant}-text-active", active );
        SetAntToken( $"--ant-color-{antVariant}-outline", outline );
    }

    private void GenerateAntLinkColorVariables( string color )
    {
        var baseColor = ParseColor( color );

        if ( baseColor.IsEmpty )
            return;

        SetAntToken( "--ant-color-link", color );
        SetAntToken( "--ant-color-link-hover", ToHex( Lighten( baseColor, 20f ) ) );
        SetAntToken( "--ant-color-link-active", ToHex( Darken( baseColor, 15f ) ) );
    }

    private void GenerateBarColorTokens( ThemeBarColorOptions barColors, bool isDark )
    {
        if ( barColors is null )
            return;

        if ( isDark )
        {
            SetAntToken( "--ant-layout-sider-bg", barColors.BackgroundColor );
            SetAntToken( "--ant-menu-dark-item-bg", barColors.BackgroundColor );
            SetAntToken( "--ant-menu-dark-popup-bg", barColors.DropdownColorOptions?.BackgroundColor ?? barColors.BackgroundColor );
            SetAntToken( "--ant-menu-dark-sub-menu-item-bg", barColors.DropdownColorOptions?.BackgroundColor ?? barColors.BackgroundColor );
            SetAntToken( "--ant-layout-trigger-bg", barColors.BrandColorOptions?.BackgroundColor ?? barColors.BackgroundColor );
            SetAntToken( "--ant-menu-dark-item-color", barColors.Color );
            SetAntToken( "--ant-layout-trigger-color", barColors.Color );
            SetAntToken( "--ant-menu-dark-item-selected-bg", barColors.ItemColorOptions?.ActiveBackgroundColor );
            SetAntToken( "--ant-menu-dark-item-selected-color", barColors.ItemColorOptions?.ActiveColor );
            SetAntToken( "--ant-menu-dark-item-hover-bg", barColors.ItemColorOptions?.HoverBackgroundColor );
            SetAntToken( "--ant-menu-dark-item-hover-color", barColors.ItemColorOptions?.HoverColor );
        }
        else
        {
            SetAntToken( "--ant-layout-light-sider-bg", barColors.BackgroundColor );
            SetAntToken( "--ant-layout-light-trigger-bg", barColors.BrandColorOptions?.BackgroundColor ?? barColors.BackgroundColor );
            SetAntToken( "--ant-layout-light-trigger-color", barColors.Color );
            SetAntToken( "--ant-menu-item-bg", barColors.BackgroundColor );
            SetAntToken( "--ant-menu-popup-bg", barColors.DropdownColorOptions?.BackgroundColor ?? barColors.BackgroundColor );
            SetAntToken( "--ant-menu-sub-menu-item-bg", barColors.DropdownColorOptions?.BackgroundColor ?? barColors.BackgroundColor );
            SetAntToken( "--ant-menu-item-color", barColors.Color );
            SetAntToken( "--ant-menu-item-selected-bg", barColors.ItemColorOptions?.ActiveBackgroundColor );
            SetAntToken( "--ant-menu-item-selected-color", barColors.ItemColorOptions?.ActiveColor );
            SetAntToken( "--ant-menu-item-hover-bg", barColors.ItemColorOptions?.HoverBackgroundColor );
            SetAntToken( "--ant-menu-item-hover-color", barColors.ItemColorOptions?.HoverColor );
        }
    }

    private void SetAntToken( string name, string value )
    {
        if ( !string.IsNullOrEmpty( value ) )
            Variables[name] = value;
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

        sb.Append( $".ant-hero-{variant}" ).Append( "{" )
            .Append( $"background-color: {Var( ThemeVariables.BackgroundColor( variant ) )} !important;" )
            .Append( $"color: {ToHex( Contrast( theme, Var( ThemeVariables.BackgroundColor( variant ) ) ) )} !important;" )
            .AppendLine( "}" );
    }

    protected override void GenerateBorderVariantStyles( StringBuilder sb, Theme theme, string variant )
    {
        var hexBorderColor = Var( ThemeVariables.BorderColor( variant ) );
        var hexBorderColorSubtle = Var( ThemeVariables.BorderSubtleColor( variant ) );

        sb.Append( $".ant-border-{variant}" ).Append( "{" )
            .Append( $"border-color: {hexBorderColor} !important;" )
            .AppendLine( "}" );

        sb.Append( $".ant-border-{variant}-subtle" ).Append( "{" )
            .Append( $"border-color: {hexBorderColorSubtle} !important;" )
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
            .Append( $".ant-btn-{variant}:active," )
            .Append( $".ant-btn-{variant}.active," )
            .Append( $".ant-btn-{variant}-active" )
            .Append( "{" )
            .Append( $"color: {yiqActiveBackground} !important;" )
            .Append( $"background-color: {activeBackground} !important;" )
            .Append( $"border-color: {activeBorder} !important;" )
            .AppendLine( "}" );

        sb
            .Append( $".ant-btn-{variant}:active > a:only-child," )
            .Append( $".ant-btn-{variant}.active > a:only-child," )
            .Append( $".ant-btn-{variant}-active > a:only-child" )
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
            .Append( $".ant-btn-{variant}:disabled" )
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
            .Append( $".ant-btn-{variant}:disabled" )
            .Append( "{" )
            .Append( "color: currentColor !important;" )
            .AppendLine( "}" );

        if ( options?.DisabledOpacity != null )
            sb.Append( $".ant-btn-{variant}[disabled]" ).Append( "{" )
                .Append( $"color: rgba(0, 0, 0, {options.DisabledOpacity.ToCultureInvariantString()}) !important;" )
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
            .Append( $".ant-btn-outline-{variant}:disabled" )
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
            .Append( $".ant-btn-outline-{variant}:disabled" )
            .Append( "{" )
            .Append( "color: currentColor !important;" )
            .AppendLine( "}" );
    }

    protected override void GenerateButtonStyles( StringBuilder sb, Theme theme, ThemeButtonOptions options )
    {
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
                .Append( $"color: rgba(0, 0, 0, {options.DisabledOpacity.ToCultureInvariantString()}) !important;" )
                .AppendLine( "}" );
    }

    protected override void GenerateDropdownStyles( StringBuilder sb, Theme theme, ThemeDropdownOptions options )
    {
        _ = sb;
        _ = theme;
        _ = options;
    }

    protected override void GenerateInputStyles( StringBuilder sb, Theme theme, ThemeInputOptions options )
    {
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
        if ( string.IsNullOrEmpty( options?.CheckColor ) )
            return;

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
        _ = theme;

        if ( string.IsNullOrEmpty( options?.SliderColor ) )
            return;

        var hoverSliderColor = ToHex( Darken( options.SliderColor, 20f ) );
        var sliderOutlineColor = ToHexRGBA( Transparency( options.SliderColor, 51 ) );

        sb.Append( ".ant-slider-track" ).Append( "{" )
            .Append( $"background-color: {options.SliderColor};" )
            .AppendLine( "}" );

        sb.Append( ".ant-slider:hover .ant-slider-track" ).Append( "{" )
            .Append( $"background-color: {hoverSliderColor};" )
            .AppendLine( "}" );

        sb.Append( ".ant-slider-handle" ).Append( "{" )
            .Append( $"border-color: {options.SliderColor};" )
            .AppendLine( "}" );

        sb
            .Append( ".ant-slider-handle:focus," )
            .Append( ".ant-slider:hover .ant-slider-handle:not(.ant-tooltip-open)" )
            .Append( "{" )
            .Append( $"border-color: {hoverSliderColor};" )
            .AppendLine( "}" );

        sb.Append( ".ant-range-slider > .ant-range-slider-range" ).Append( "{" )
            .Append( $"background-color: {options.SliderColor};" )
            .AppendLine( "}" );

        sb.Append( ".ant-range-slider:hover > .ant-range-slider-range" ).Append( "{" )
            .Append( $"background-color: {hoverSliderColor};" )
            .AppendLine( "}" );

        sb.Append( ".ant-range-slider > .ant-range-slider-input::-webkit-slider-thumb" ).Append( "{" )
            .Append( $"border-color: {options.SliderColor};" )
            .Append( $"box-shadow: 0 0 0 2px {sliderOutlineColor};" )
            .AppendLine( "}" );

        sb.Append( ".ant-range-slider > .ant-range-slider-input::-moz-range-thumb" ).Append( "{" )
            .Append( $"border-color: {options.SliderColor};" )
            .Append( $"box-shadow: 0 0 0 2px {sliderOutlineColor};" )
            .AppendLine( "}" );

        sb
            .Append( ".ant-range-slider > .ant-range-slider-input:focus::-webkit-slider-thumb," )
            .Append( ".ant-range-slider:hover > .ant-range-slider-input::-webkit-slider-thumb" )
            .Append( "{" )
            .Append( $"border-color: {hoverSliderColor};" )
            .AppendLine( "}" );

        sb
            .Append( ".ant-range-slider > .ant-range-slider-input:focus::-moz-range-thumb," )
            .Append( ".ant-range-slider:hover > .ant-range-slider-input::-moz-range-thumb" )
            .Append( "{" )
            .Append( $"border-color: {hoverSliderColor};" )
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

        // Subtle variant

        var hexBackgroundColorSubtle = Var( ThemeVariables.BackgroundSubtleColor( variant ) );
        var hexBorderColorSubtle = Var( ThemeVariables.BorderSubtleColor( variant ) );
        var hexTextColorEmphasis = Var( ThemeVariables.TextEmphasisColor( variant ) );

        sb.Append( $".ant-tag.ant-tag-{variant}-subtle," )
            .Append( $".ant-tag .anticon.anticon-close.ant-tag-{variant}-subtle" )
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
        if ( stepsOptions is null )
            return;

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
        if ( stepsOptions is null )
            return;

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
        if ( ratingOptions is null )
            return;

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
        if ( !string.IsNullOrEmpty( options?.ImageTopRadius ) )
            sb.Append( ".ant-card-cover" ).Append( "{" )
                .Append( $"border-top-left-radius: {options.ImageTopRadius};" )
                .Append( $"border-top-right-radius: {options.ImageTopRadius};" )
                .AppendLine( "}" );
    }

    protected override void GenerateModalStyles( StringBuilder sb, Theme theme, ThemeModalOptions options )
    {
        _ = sb;
        _ = theme;
        _ = options;
    }

    protected override void GenerateTabsStyles( StringBuilder sb, Theme theme, ThemeTabsOptions options )
    {
        _ = sb;
        _ = theme;
        _ = options;
    }

    protected override void GenerateProgressStyles( StringBuilder sb, Theme theme, ThemeProgressOptions options )
    {
        base.GenerateProgressStyles( sb, theme, options );
    }

    protected override void GenerateAlertStyles( StringBuilder sb, Theme theme, ThemeAlertOptions options )
    {
        _ = sb;
        _ = theme;
        _ = options;
    }

    protected override void GenerateBreadcrumbStyles( StringBuilder sb, Theme theme, ThemeBreadcrumbOptions options )
    {
        if ( !string.IsNullOrEmpty( Var( ThemeVariables.BreadcrumbColor ) ) )
        {
            sb.Append( ".ant-breadcrumb-link>a" ).Append( "{" )
                .Append( $"color: {Var( ThemeVariables.BreadcrumbColor )};" )
                .AppendLine( "}" );
        }
    }

    protected override void GenerateBadgeStyles( StringBuilder sb, Theme theme, ThemeBadgeOptions options )
    {
        _ = sb;
        _ = theme;
        _ = options;
    }

    protected override void GeneratePaginationStyles( StringBuilder sb, Theme theme, ThemePaginationOptions options )
    {
        _ = sb;
        _ = theme;
        _ = options;
    }

    protected override void GenerateBarStyles( StringBuilder sb, Theme theme, ThemeBarOptions options )
    {
        if ( options is null )
            return;

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
    }

    protected override void GenerateParagraphVariantStyles( StringBuilder sb, Theme theme, string variant, string inTextColor )
    {
        var textColor = variant == "body" && !string.IsNullOrEmpty( theme.BodyOptions?.TextColor )
            ? ParseColor( theme.BodyOptions.TextColor )
            : ParseColor( inTextColor );

        var hexTextColor = ToHex( textColor );
        var hexTextColorEmphasis = ToHex( ShadeColor( textColor, theme.TextColorOptions.EmphasisShadeWeight ?? 22 ) );

        sb.Append( $".ant-typography-{variant}" )
            .Append( "{" )
            .Append( $"color: {hexTextColor};" )
            .AppendLine( "}" );

        sb.Append( $".ant-typography-{variant}-emphasis" )
            .Append( "{" )
            .Append( $"color: {hexTextColorEmphasis} !important;" )
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