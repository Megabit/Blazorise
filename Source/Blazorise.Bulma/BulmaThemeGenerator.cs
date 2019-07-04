#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace Blazorise.Bulma
{
    public class BulmaThemeGenerator : ThemeGenerator
    {
        #region Methods

        protected override void GenerateBackgroundVariantStyles( StringBuilder sb, Theme theme, string variant )
        {
            sb.Append( $".has-background-{variant}" ).Append( "{" )
                .Append( $"background-color: {Var( $"--b-theme-background-{variant}" )} !important;" )
                .AppendLine( "}" );
        }

        protected override void GenerateButtonVariantStyles( StringBuilder sb, Theme theme, string variant, ThemeButtonOptions options )
        {
            var background = Var( $"--b-button-{variant}-background" );
            var border = Var( $"--b-button-{variant}-border" );
            var hoverBackground = Var( $"--b-button-{variant}-hover-background" );
            var hoverBorder = Var( $"--b-button-{variant}-hover-border" );
            var activeBackground = Var( $"--b-button-{variant}-active-background" );
            var activeBorder = Var( $"--b-button-{variant}-active-border" );
            var yiqBackground = Var( $"--b-button-{variant}-yiq-background" );
            var yiqHoverBackground = Var( $"--b-button-{variant}-yiq-hover-background" );
            var yiqActiveBackground = Var( $"--b-button-{variant}-yiq-active-background" );
            var boxShadow = Var( $"--b-button-{variant}-box-shadow" );

            sb.Append( $".button.is-{variant}" ).Append( "{" )
                //.Append( $"color: {yiqBackground};" )
                .Append( GradientBg( theme, background ) )
                .Append( $"border-color: transparent;" )
                .AppendLine( "}" );

            sb.Append( $".button.is-{variant}:hover," ).Append( $".button.is-{variant}.is-hovered" ).Append( "{" )
                .Append( $"color: {yiqHoverBackground};" )
                .Append( GradientBg( theme, hoverBackground ) )
                .Append( $"border-color: {hoverBorder};" )
                .AppendLine( "}" );

            sb.Append( $".button.is-{variant}:focus," ).Append( $".button.is-{variant}.is-focused" ).Append( "{" )
                .Append( $"color: {yiqHoverBackground};" )
                .Append( GradientBg( theme, hoverBackground ) )
                .Append( $"border-color: transparent;" )
                .AppendLine( "}" );

            sb.Append( $".button.is-{variant}:focus:not(:active)," ).Append( $".button.is-{variant}.is-focused:not(:active)" ).Append( "{" )
                .Append( $"box-shadow: 0 0 0 {options?.BoxShadowSize ?? ".125rem"} {boxShadow};" )
                .AppendLine( "}" );

            sb.Append( $".button.is-{variant}[disabled]," ).Append( $"fieldset[disabled] .button.is-{variant}" ).Append( "{" )
                .Append( $"color: {yiqBackground};" )
                .Append( $"background-color: {background};" )
                .Append( $"border-color: {border};" )
                .Append( $"box-shadow: none;" )
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
            var color = Var( $"--b-outline-button-{variant}-color" );
            var yiqColor = Var( $"--b-outline-button-{variant}-yiq-color" );
            var boxShadow = Var( $"--b-outline-button-{variant}-box-shadow" );

            sb.Append( $".button.is-{variant}.is-outlined-{variant}" ).Append( "{" )
                .Append( $"color: {color};" )
                .Append( $"border-color: {color};" )
                .AppendLine( "}" );

            sb.Append( $".button.is-{variant}.is-outlined-{variant}:hover," )
                .Append( $".button.is-{variant}.is-outlined-{variant}.is-hovered," )
                .Append( $".button.is-{variant}.is-outlined-{variant}:focus," )
                .Append( $".button.is-{variant}.is-outlined-{variant}.is-focused" ).Append( "{" )
                .Append( $"color: {yiqColor};" )
                .Append( $"background-color: white;" )
                .Append( $"border-color: {color};" )
                .AppendLine( "}" );

            sb.Append( $".button.is-{variant}.is-outlined-{variant}.is-loading::after" ).Append( "{" )
                .Append( $"border-color: transparent transparent white white;" )
                .AppendLine( "}" );

            sb.Append( $".button.is-{variant}.is-outlined-{variant}[disabled]," ).Append( $"fieldset[disabled] .button.is-{variant}.is-outlined-{variant}" ).Append( "{" )
                .Append( $"color: {color};" )
                .Append( $"background-color: transparent;" )
                .Append( $"border-color: {color};" )
                .Append( $"box-shadow: none;" )
                .AppendLine( "}" );
        }

        protected override void GenerateButtonStyles( StringBuilder sb, Theme theme, ThemeButtonOptions options )
        {
            if ( !string.IsNullOrEmpty( options.BorderRadius ) )
                sb.Append( $".button" ).Append( "{" )
                    .Append( $"border-radius: {options.BorderRadius};" )
                    .AppendLine( "}" );

            if ( !string.IsNullOrEmpty( options.Padding ) )
                sb.Append( $".button" ).Append( "{" )
                    .Append( $"padding: {options.Padding};" )
                    .AppendLine( "}" );

            if ( !string.IsNullOrEmpty( options.Margin ) )
                sb.Append( $".button" ).Append( "{" )
                    .Append( $"margin: {options.Margin};" )
                    .AppendLine( "}" );
        }

        protected override void GenerateDropdownStyles( StringBuilder sb, Theme theme, ThemeDropdownOptions options )
        {
            if ( !string.IsNullOrEmpty( options.BorderRadius ) )
                sb.Append( $".dropdown-content" ).Append( "{" )
                    .Append( $"border-radius: {options.BorderRadius};" )
                    .AppendLine( "}" );
        }

        protected override void GenerateInputStyles( StringBuilder sb, Theme theme, ThemeInputOptions options )
        {
            if ( !string.IsNullOrEmpty( options.BorderRadius ) )
            {
                sb.Append( $".input" ).Append( "{" )
                    .Append( $"border-radius: {options.BorderRadius};" )
                    .AppendLine( "}" );

                sb.Append( $".select select" ).Append( "{" )
                    .Append( $"border-radius: {options.BorderRadius};" )
                    .AppendLine( "}" );

                sb.Append( $".textarea" ).Append( "{" )
                    .Append( $"border-radius: {options.BorderRadius};" )
                    .AppendLine( "}" );
            }
        }

        protected override void GenerateBadgeVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor )
        {
            var backgroundColor = ParseColor( inBackgroundColor );

            if ( backgroundColor.IsEmpty )
                return;

            var yiqBackgroundColor = Contrast( backgroundColor );

            var background = ToHex( backgroundColor );
            var yiqBackground = ToHex( yiqBackgroundColor );

            sb.Append( $".tag.is-{variant}" ).Append( "{" )
                .Append( $"color: {yiqBackground};" )
                .Append( $"background-color: {background};" )
                .AppendLine( "}" );
        }

        protected override void GenerateAlertVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, string inBorderColor, string inColor )
        {
            var backgroundColor = ParseColor( inBackgroundColor );
            var borderColor = ParseColor( inBorderColor );
            var textColor = ParseColor( inColor );

            var alertLinkColor = Darken( textColor, 10f );

            var background = ToHex( backgroundColor );
            var border = ToHex( borderColor );
            var text = ToHex( textColor );
            var alertLink = ToHex( alertLinkColor );

            sb.Append( $".notification-{variant}" ).Append( "{" )
                .Append( $"color: {text};" )
                .Append( GradientBg( theme, background ) )
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
            //        .Append( $"border-radius: {options.BorderRadius};" )
            //        .AppendLine( "}" );

            //if ( !string.IsNullOrEmpty( options.ImageTopRadius ) )
            //    sb.Append( $".card-image-top" ).Append( "{" )
            //        .Append( $"border-top-left-radius: {options.ImageTopRadius};" )
            //        .Append( $"border-top-right-radius: {options.ImageTopRadius};" )
            //        .AppendLine( "}" );
        }

        protected override void GenerateModalStyles( StringBuilder sb, Theme theme, ThemeModalOptions options )
        {
            if ( !string.IsNullOrEmpty( options.BorderRadius ) )
            {
                sb.Append( $".modal-card-head" ).Append( "{" )
                    .Append( $"border-top-left-radius: {options.BorderRadius};" )
                    .Append( $"border-top-right-radius: {options.BorderRadius};" )
                    .AppendLine( "}" );

                sb.Append( $".modal-card-foot" ).Append( "{" )
                   .Append( $"border-bottom-left-radius: {options.BorderRadius};" )
                   .Append( $"border-bottom-right-radius: {options.BorderRadius};" )
                   .AppendLine( "}" );
            }
        }

        protected override void GenerateTabsStyles( StringBuilder sb, Theme theme, ThemeTabsOptions options )
        {
            //if ( !string.IsNullOrEmpty( options.BorderRadius ) )
            //{
            //    sb.Append( $".nav-tabs .nav-link" ).Append( "{" )
            //        .Append( $"border-top-left-radius: {options.BorderRadius};" )
            //        .Append( $"border-top-right-radius: {options.BorderRadius};" )
            //        .AppendLine( "}" );

            //    sb.Append( $".nav-pills .nav-link" ).Append( "{" )
            //        .Append( $"border-radius: {options.BorderRadius};" )
            //        .AppendLine( "}" );
            //}
        }

        protected override void GenerateProgressStyles( StringBuilder sb, Theme theme, ThemeProgressOptions options )
        {
            if ( !string.IsNullOrEmpty( options.BorderRadius ) )
                sb.Append( $".progress" ).Append( "{" )
                    .Append( $"border-radius: {options.BorderRadius};" )
                    .AppendLine( "}" );
        }

        protected override void GenerateAlertStyles( StringBuilder sb, Theme theme, ThemeAlertOptions options )
        {
            if ( !string.IsNullOrEmpty( options.BorderRadius ) )
                sb.Append( $".notification" ).Append( "{" )
                    .Append( $"border-radius: {options.BorderRadius};" )
                    .AppendLine( "}" );
        }

        protected override void GenerateBreadcrumbStyles( StringBuilder sb, Theme theme, ThemeBreadcrumbOptions options )
        {
        }

        protected override void GenerateBadgeStyles( StringBuilder sb, Theme theme, ThemeBadgeOptions options )
        {
            if ( !string.IsNullOrEmpty( options.BorderRadius ) )
                sb.Append( $".tag:not(body)" ).Append( "{" )
                    .Append( $"border-radius: {options.BorderRadius};" )
                    .AppendLine( "}" );
        }

        protected override void GeneratePaginationStyles( StringBuilder sb, Theme theme, ThemePaginationOptions options )
        {
            if ( !string.IsNullOrEmpty( options.BorderRadius ) )
            {
                sb.Append( $".pagination-link,.pagination-previous,.pagination-next" ).Append( "{" )
                    .Append( $"border-radius: {options.BorderRadius};" )
                    .AppendLine( "}" );
            }

            if ( !string.IsNullOrEmpty( options.LargeBorderRadius ) )
            {
                sb.Append( $".pagination.is-large .pagination-link,.pagination-previous,.pagination-next" ).Append( "{" )
                    .Append( $"border-radius: {options.BorderRadius};" )
                    .AppendLine( "}" );
            }

            if ( !string.IsNullOrEmpty( theme.ColorOptions?.Primary ) )
            {
                sb.Append( $".pagination-link.is-current,.pagination-previous.is-current,.pagination-next.is-current" ).Append( "{" )
                    .Append( $"background-color: {theme.ColorOptions.Primary};" )
                    .Append( $"border-color: {theme.ColorOptions.Primary};" )
                    .AppendLine( "}" );
            }
        }

        #endregion
    }
}
