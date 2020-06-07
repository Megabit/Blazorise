#region Using directives
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
#endregion

namespace Blazorise.Bootstrap
{
    public class BootstrapThemeGenerator : ThemeGenerator
    {
        #region Methods

        protected override void GenerateBreakpointStyles( StringBuilder sb, Theme theme, string breakpointName, string breakpointSize )
        {
            if ( string.IsNullOrEmpty( breakpointSize ) )
                return;

            if ( !string.IsNullOrEmpty( theme?.ContainerMaxWidthOptions?[breakpointName]?.Invoke() ) )
            {
                var containerSize = theme.ContainerMaxWidthOptions[breakpointName].Invoke();

                sb.Append( MediaBreakpointUp( breakpointName, $".container{{max-width: {containerSize};}}" ) );
            }
        }

        protected override void GenerateBackgroundVariantStyles( StringBuilder sb, Theme theme, string variant )
        {
            sb.Append( $".bg-{variant}" ).Append( "{" )
                .Append( $"background-color: {Var( ThemeVariables.BackgroundColor( variant ) )} !important;" )
                .AppendLine( "}" );

            sb.Append( $".jumbotron-{variant}" ).Append( "{" )
                .Append( $"background-color: {Var( ThemeVariables.BackgroundColor( variant ) )} !important;" )
                .Append( $"color: {ToHex( Contrast( Var( ThemeVariables.BackgroundColor( variant ) ) ) )} !important;" )
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

            sb.Append( $".btn-{variant}" ).Append( "{" )
                .Append( $"color: {yiqBackground};" )
                .Append( GetGradientBg( theme, background, options?.GradientBlendPercentage ) )
                .Append( $"border-color: {border};" )
                .AppendLine( "}" );

            sb.Append( $".btn-{variant}:hover" ).Append( "{" )
                .Append( $"color: {yiqHoverBackground};" )
                .Append( GetGradientBg( theme, hoverBackground, options?.GradientBlendPercentage ) )
                .Append( $"border-color: {hoverBorder};" )
                .AppendLine( "}" );

            sb.Append( $".btn-{variant}:focus," )
                .Append( $".btn-{variant}.focus" )
                .Append( "{" )
                .Append( $"color: {yiqHoverBackground};" )
                .Append( GetGradientBg( theme, hoverBackground, options?.GradientBlendPercentage ) )
                .Append( $"border-color: {hoverBorder};" )
                .Append( $"box-shadow: 0 0 0 {options?.BoxShadowSize ?? ".2rem"} {boxShadow};" )
                .AppendLine( "}" );

            sb.Append( $".btn-{variant}.disabled," )
                .Append( $".btn-{variant}:disabled" )
                .Append( "{" )
                .Append( $"color: {yiqBackground};" )
                .Append( $"background-color: {background};" )
                .Append( $"border-color: {border};" )
                .Append( $"box-shadow: 0 0 0 {options?.BoxShadowSize ?? ".2rem"} {boxShadow};" )
                .AppendLine( "}" );

            sb
                .Append( $".btn-{variant}:not(:disabled):not(.disabled):active," )
                .Append( $".btn-{variant}:not(:disabled):not(.disabled).active," )
                .Append( $".show>.btn-{variant}.dropdown-toggle" )
                .Append( "{" )
                .Append( $"color: {yiqActiveBackground};" )
                .Append( $"background-color: {activeBackground};" )
                .Append( $"border-color: {activeBorder};" )
                .AppendLine( "}" );

            sb
                .Append( $".btn-{variant}:not(:disabled):not(.disabled):active:focus," )
                .Append( $".btn-{variant}:not(:disabled):not(.disabled).active:focus," )
                .Append( $".show>.btn-{variant}.dropdown-toggle:focus" )
                .Append( "{" )
                .Append( $"box-shadow: 0 0 0 {options?.BoxShadowSize ?? ".2rem"} {boxShadow}" )
                .AppendLine( "}" );
        }

        protected override void GenerateButtonOutlineVariantStyles( StringBuilder sb, Theme theme, string variant, ThemeButtonOptions options )
        {
            var color = Var( ThemeVariables.OutlineButtonColor( variant ) );
            var yiqColor = Var( ThemeVariables.OutlineButtonYiqColor( variant ) );
            var boxShadow = Var( ThemeVariables.OutlineButtonBoxShadowColor( variant ) );

            sb.Append( $".btn-outline-{variant}" ).Append( "{" )
                .Append( $"color: {color};" )
                .Append( $"border-color: {color};" )
                .AppendLine( "}" );

            sb.Append( $".btn-outline-{variant}:hover" ).Append( "{" )
                .Append( $"color: {yiqColor};" )
                .Append( $"background-color: {color};" )
                .Append( $"border-color: {color};" )
                .AppendLine( "}" );

            sb.Append( $".btn-outline-{variant}:focus," )
                .Append( $".btn-outline-{variant}.focus" )
                .Append( "{" )
                .Append( $"box-shadow: 0 0 0 {options?.BoxShadowSize ?? ".2rem"} {boxShadow};" )
                .AppendLine( "}" );

            sb.Append( $".btn-outline-{variant}.disabled," )
                .Append( $".btn-outline-{variant}:disabled" )
                .Append( "{" )
                .Append( $"color: {color};" )
                .Append( $"background-color: transparent;" )
                .AppendLine( "}" );

            sb
                .Append( $".btn-outline-{variant}:not(:disabled):not(.disabled):active," )
                .Append( $".btn-outline-{variant}:not(:disabled):not(.disabled).active," )
                .Append( $".show>.btn-outline-{variant}.dropdown-toggle" )
                .Append( "{" )
                .Append( $"color: {yiqColor};" )
                .Append( $"background-color: {color};" )
                .Append( $"border-color: {color};" )
                .AppendLine( "}" );

            sb
                .Append( $".btn-outline-{variant}:not(:disabled):not(.disabled):active:focus," )
                .Append( $".btn-outline-{variant}:not(:disabled):not(.disabled).active:focus," )
                .Append( $".show>.btn-outline-{variant}.dropdown-toggle:focus" )
                .Append( "{" )
                .Append( $"box-shadow: 0 0 0 {options?.BoxShadowSize ?? ".2rem"} {boxShadow};" )
                .AppendLine( "}" );
        }

        protected override void GenerateButtonStyles( StringBuilder sb, Theme theme, ThemeButtonOptions options )
        {
            sb.Append( $".btn" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );

            sb.Append( $".btn-sm" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.SmallBorderRadius, Var( ThemeVariables.BorderRadiusSmall ) )};" )
                .AppendLine( "}" );

            sb.Append( $".btn-lg" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.LargeBorderRadius, Var( ThemeVariables.BorderRadiusLarge ) )};" )
                .AppendLine( "}" );

            if ( !string.IsNullOrEmpty( options?.Padding ) )
                sb.Append( $".btn" ).Append( "{" )
                    .Append( $"padding: {options.Padding};" )
                    .AppendLine( "}" );

            if ( !string.IsNullOrEmpty( options?.Margin ) )
                sb.Append( $".btn" ).Append( "{" )
                    .Append( $"margin: {options.Margin};" )
                    .AppendLine( "}" );
        }

        protected override void GenerateDropdownStyles( StringBuilder sb, Theme theme, ThemeDropdownOptions options )
        {
            sb.Append( $".dropdown-menu" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );

            if ( !string.IsNullOrEmpty( theme.ColorOptions?.Primary ) )
            {
                var backgroundColor = ParseColor( theme.ColorOptions.Primary );

                if ( !backgroundColor.IsEmpty )
                {
                    var background = ToHex( backgroundColor );

                    sb.Append( $".dropdown-item.active," )
                        .Append( $".dropdown-item:active" ).Append( "{" )
                        .Append( GetGradientBg( theme, background, options?.GradientBlendPercentage ) )
                        .AppendLine( "}" );
                }
            }
        }

        protected override void GenerateInputStyles( StringBuilder sb, Theme theme, ThemeInputOptions options )
        {
            sb.Append( $".form-control" ).Append( "{" )
                    .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                    .AppendLine( "}" );

            sb.Append( $".input-group-text" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );

            sb.Append( $".custom-select" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );

            sb.Append( $".custom-checkbox .custom-control-label::before" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );

            sb.Append( $".custom-file-label" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );

            if ( !string.IsNullOrEmpty( options?.Color ) )
            {
                sb.Append( $".form-control" ).Append( "{" )
                    .Append( $"color: {options.Color};" )
                    .AppendLine( "}" );

                sb.Append( $".input-group-text" ).Append( "{" )
                    .Append( $"color: {options.Color};" )
                    .AppendLine( "}" );

                sb.Append( $".custom-select" ).Append( "{" )
                    .Append( $"color: {options.Color};" )
                    .AppendLine( "}" );
            }

            if ( !string.IsNullOrEmpty( options?.CheckColor ) )
            {
                GenerateInputCheckEditStyles( sb, theme, options );
            }
        }

        protected virtual void GenerateInputCheckEditStyles( StringBuilder sb, Theme theme, ThemeInputOptions options )
        {
            sb
                .Append( $".custom-checkbox .custom-control-input:checked ~ .custom-control-label::before" ).Append( "{" )
                .Append( $"background-color: {options.CheckColor};" )
                .AppendLine( "}" );

            sb
                .Append( $".custom-control-input:checked ~ .custom-control-label::before" ).Append( "{" )
                .Append( $"color: {options.Color};" )
                .Append( $"border-color: {options.CheckColor};" )
                .Append( $"background-color: {options.CheckColor};" )
                .AppendLine( "}" );

            sb
                .Append( $".custom-switch .custom-control-input:checked ~ .custom-control-label::before" ).Append( "{" )
                .Append( $"background-color: {options.CheckColor};" )
                .AppendLine( "}" );
        }

        protected override void GenerateBadgeVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor )
        {
            var backgroundColor = ParseColor( inBackgroundColor );

            if ( backgroundColor.IsEmpty )
                return;

            var yiqBackgroundColor = Contrast( backgroundColor );

            var background = ToHex( backgroundColor );
            var yiqBackground = ToHex( yiqBackgroundColor );

            sb.Append( $".badge-{variant}" ).Append( "{" )
                .Append( $"color: {yiqBackground};" )
                .Append( $"background-color: {background};" )
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

            sb.Append( $".table-hover table-{variant}:hover" )
                .Append( "{" )
                .Append( $"background-color: {hoverBackground};" )
                .AppendLine( "}" );

            sb.Append( $".table-hover table-{variant}:hover>td" )
                .Append( $".table-hover table-{variant}:hover>th" )
                .Append( "{" )
                .Append( $"background-color: {hoverBackground};" )
                .AppendLine( "}" );
        }

        protected override void GenerateCardStyles( StringBuilder sb, Theme theme, ThemeCardOptions options )
        {
            sb.Append( $".card" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );

            if ( !string.IsNullOrEmpty( options?.ImageTopRadius ) )
                sb.Append( $".card-image-top" ).Append( "{" )
                    .Append( $"border-top-left-radius: {options.ImageTopRadius};" )
                    .Append( $"border-top-right-radius: {options.ImageTopRadius};" )
                    .AppendLine( "}" );
        }

        protected override void GenerateModalStyles( StringBuilder sb, Theme theme, ThemeModalOptions options )
        {
            sb.Append( $".modal-content" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );
        }

        protected override void GenerateTabsStyles( StringBuilder sb, Theme theme, ThemeTabsOptions options )
        {
            sb.Append( $".nav-tabs .nav-link" ).Append( "{" )
                .Append( $"border-top-left-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .Append( $"border-top-right-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );

            sb.Append( $".nav-pills .nav-link" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );

            if ( !string.IsNullOrEmpty( theme.ColorOptions?.Primary ) )
            {
                sb
                    .Append( $".nav-pills .nav-link.active," )
                    .Append( $".nav-pills .show>.nav-link" )
                    .Append( "{" )
                    .Append( $"background-color: {Var( ThemeVariables.Color( "primary" ) )};" )
                    .AppendLine( "}" );

                sb
                    .Append( $".nav.nav-tabs .nav-item a.nav-link:not(.active)" )
                    .Append( "{" )
                    .Append( $"color: {Var( ThemeVariables.Color( "primary" ) )};" )
                    .AppendLine( "}" );
            }
        }

        protected override void GenerateProgressStyles( StringBuilder sb, Theme theme, ThemeProgressOptions options )
        {
            sb.Append( $".progress" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );

            if ( !string.IsNullOrEmpty( theme.ColorOptions?.Primary ) )
            {
                sb.Append( $".progress-bar" ).Append( "{" )
                    .Append( $"background-color: {Var( ThemeVariables.Color( "primary" ) )};" )
                    .AppendLine( "}" );
            }
        }

        protected override void GenerateAlertStyles( StringBuilder sb, Theme theme, ThemeAlertOptions options )
        {
            sb.Append( $".alert" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );
        }

        protected override void GenerateBreadcrumbStyles( StringBuilder sb, Theme theme, ThemeBreadcrumbOptions options )
        {
            sb.Append( $".breadcrumb" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );


            if ( !string.IsNullOrEmpty( Var( ThemeVariables.BreadcrumbColor ) ) )
            {
                sb.Append( $".breadcrumb-item>a" ).Append( "{" )
                    .Append( $"color: {Var( ThemeVariables.BreadcrumbColor )};" )
                    .AppendLine( "}" );
            }
        }

        protected override void GenerateBadgeStyles( StringBuilder sb, Theme theme, ThemeBadgeOptions options )
        {
            sb.Append( $".badge:not(.badge-pill)" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );
        }

        protected override void GeneratePaginationStyles( StringBuilder sb, Theme theme, ThemePaginationOptions options )
        {
            sb.Append( $".page-item:first-child .page-link" ).Append( "{" )
                .Append( $"border-top-left-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .Append( $"border-bottom-left-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );

            sb.Append( $".page-item:last-child .page-link" ).Append( "{" )
                .Append( $"border-top-right-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .Append( $"border-bottom-right-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );

            sb.Append( $".pagination-lg .page-item:first-child .page-link" ).Append( "{" )
                .Append( $"border-top-left-radius: {GetBorderRadius( theme, options?.LargeBorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .Append( $"border-bottom-left-radius: {GetBorderRadius( theme, options?.LargeBorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );

            sb.Append( $".pagination-lg .page-item:last-child .page-link" ).Append( "{" )
                .Append( $"border-top-right-radius: {GetBorderRadius( theme, options?.LargeBorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .Append( $"border-bottom-right-radius: {GetBorderRadius( theme, options?.LargeBorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );

            if ( !string.IsNullOrEmpty( theme.ColorOptions?.Primary ) )
            {
                sb.Append( $".page-item.active .page-link" ).Append( "{" )
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
            var textColor = ParseColor( inTextColor );

            var textColorHex = ToHex( textColor );

            sb.Append( $".text-{variant}" )
                .Append( "{" )
                .Append( $"color: {textColorHex};" )
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

        #endregion
    }
}
