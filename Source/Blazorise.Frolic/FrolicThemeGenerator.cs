#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace Blazorise.Frolic
{
    public class FrolicThemeGenerator : ThemeGenerator
    {
        #region Methods

        protected override void GenerateBackgroundVariantStyles( StringBuilder sb, Theme theme, string variant )
        {
            sb.Append( $".bg-{variant}" ).Append( "{" )
                .Append( $"background-color: {Var( $"--b-theme-background-{variant}" )};" )
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

            sb.Append( $".e-btn.{variant}" ).Append( "{" )
                .Append( $"color: {yiqBackground};" )
                .Append( GradientBg( theme, background ) )
                .Append( $"border-color: {border};" )
                .AppendLine( "}" );

            sb.Append( $".e-btn.{variant}:hover," )
                .Append( $".e-btn.{variant}:focus" )
                .Append( "{" )
                .Append( GradientBg( theme, hoverBackground ) )
                .Append( $"box-shadow: 0 0 0 {options?.BoxShadowSize ?? ".2rem"} {boxShadow};" )
                .AppendLine( "}" );

            //sb.Append( $".e-btn.{variant}:focus" ).Append( $".e-btn.{variant}.focus" ).Append( "{" )
            //    .Append( $"color: {yiqHoverBackground};" )
            //    .Append( GradientBg( theme, hoverBackground ) )
            //    .Append( $"border-color: {hoverBorder};" )
            //    .Append( $"box-shadow: 0 0 0 {options?.BoxShadowSize ?? ".2rem"} {boxShadow};" )
            //    .AppendLine( "}" );

            //sb.Append( $".e-btn.{variant}.disabled" ).Append( $".e-btn.{variant}:disabled" ).Append( "{" )
            //    .Append( $"color: {yiqBackground};" )
            //    .Append( $"background-color: {background};" )
            //    .Append( $"border-color: {border};" )
            //    .Append( $"box-shadow: 0 0 0 {options?.BoxShadowSize ?? ".2rem"} {boxShadow};" )
            //    .AppendLine( "}" );

            //sb
            //    .Append( $".e-btn.{variant}:not(:disabled):not(.disabled):active," )
            //    .Append( $".e-btn.{variant}:not(:disabled):not(.disabled).active," )
            //    .Append( $".show>.e-btn.{variant}.dropdown-toggle" )
            //    .Append( "{" )
            //    .Append( $"color: {yiqActiveBackground};" )
            //    .Append( $"background-color: {activeBackground};" )
            //    .Append( $"border-color: {activeBorder};" )
            //    .AppendLine( "}" );

            //sb
            //    .Append( $".e-btn.{variant}:not(:disabled):not(.disabled):active:focus," )
            //    .Append( $".e-btn.{variant}:not(:disabled):not(.disabled).active:focus," )
            //    .Append( $".show>.e-btn.{variant}.dropdown-toggle:focus" )
            //    .Append( "{" )
            //    .Append( $"box-shadow: 0 0 0 {options?.BoxShadowSize ?? ".2rem"} {boxShadow}" )
            //    .AppendLine( "}" );
        }

        protected override void GenerateButtonOutlineVariantStyles( StringBuilder sb, Theme theme, string variant, ThemeButtonOptions buttonOptions )
        {
            var color = Var( $"--b-outline-button-{variant}-color" );
            var yiqColor = Var( $"--b-outline-button-{variant}-yiq-color" );
            var boxShadow = Var( $"--b-outline-button-{variant}-box-shadow" );

            sb.Append( $".e-btn.outlined.{variant}" ).Append( "{" )
                .Append( $"color: {color};" )
                .Append( $"background-color: white;" )
                .Append( $"border-color: {color};" )
                .AppendLine( "}" );

            sb.Append( $".e-btn.outlined.{variant}:hover," )
                .Append( $".e-btn.outlined.{variant}:focus" )
                .Append( "{" )
                .Append( $"background-color: {color};" )
                .AppendLine( "}" );

            //sb.Append( $".e-btn.outlined.{variant}.disabled" ).Append( $".e-btn.outlined.{variant}:disabled" ).Append( "{" )
            //    .Append( $"color: {color};" )
            //    .Append( $"background-color: transparent;" )
            //    .AppendLine( "}" );

            //sb
            //    .Append( $".e-btn.outlined.{variant}:not(:disabled):not(.disabled):active," )
            //    .Append( $".e-btn.outlined.{variant}:not(:disabled):not(.disabled).active," )
            //    .Append( $".show>.e-btn.outlined.{variant}.dropdown-toggle" )
            //    .Append( "{" )
            //    .Append( $"color: {yiqColor};" )
            //    .Append( $"background-color: {color};" )
            //    .Append( $"border-color: {color};" )
            //    .AppendLine( "}" );

            //sb
            //    .Append( $".e-btn.outlined.{variant}:not(:disabled):not(.disabled):active:focus," )
            //    .Append( $".e-btn.outlined.{variant}:not(:disabled):not(.disabled).active:focus," )
            //    .Append( $".show>.e-btn.outlined.{variant}.dropdown-toggle:focus" )
            //    .Append( "{" )
            //    .Append( $"box-shadow: 0 0 0 2rem {boxShadow};" )
            //    .AppendLine( "}" );
        }

        protected override void GenerateButtonStyles( StringBuilder sb, Theme theme, ThemeButtonOptions options )
        {
            if ( !string.IsNullOrEmpty( options.BorderRadius ) )
                sb.Append( $".e-btn" ).Append( "{" )
                    .Append( $"border-radius: {options.BorderRadius};" )
                    .AppendLine( "}" );

            if ( !string.IsNullOrEmpty( options.Padding ) )
                sb.Append( $".e-btn" ).Append( "{" )
                    .Append( $"padding: {options.Padding};" )
                    .AppendLine( "}" );

            if ( !string.IsNullOrEmpty( options.Margin ) )
                sb.Append( $".e-btn" ).Append( "{" )
                    .Append( $"margin: {options.Margin};" )
                    .AppendLine( "}" );
        }

        protected override void GenerateDropdownStyles( StringBuilder sb, Theme theme, ThemeDropdownOptions options )
        {
            if ( !string.IsNullOrEmpty( options.BorderRadius ) )
                sb.Append( $".drop-items" ).Append( "{" )
                    .Append( $"border-radius: {options.BorderRadius};" )
                    .AppendLine( "}" );

            if ( !string.IsNullOrEmpty( theme.ColorOptions?.Primary ) )
            {
                var backgroundColor = ParseColor( theme.ColorOptions.Primary );

                if ( !backgroundColor.IsEmpty )
                {
                    var background = ToHex( backgroundColor );

                    sb.Append( $".drop-item:hover" )
                        .Append( "{" )
                        .Append( GradientBg( theme, background ) )
                        .AppendLine( "}" );
                }
            }
        }

        protected override void GenerateInputStyles( StringBuilder sb, Theme theme, ThemeInputOptions options )
        {
            if ( !string.IsNullOrEmpty( options.BorderRadius ) )
            {
                sb.Append( $".e-control" ).Append( "{" )
                    .Append( $"border-radius: {options.BorderRadius};" )
                    .AppendLine( "}" );

                sb.Append( $".e-control-helper" ).Append( "{" )
                    .Append( $"border-radius: {options.BorderRadius};" )
                    .AppendLine( "}" );

                sb.Append( $".e-select" ).Append( "{" )
                    .Append( $"border-radius: {options.BorderRadius};" )
                    .AppendLine( "}" );
            }

            if ( !string.IsNullOrEmpty( options.Color ) )
            {
                sb.Append( $".e-control" ).Append( "{" )
                    .Append( $"color: {options.Color};" )
                    .AppendLine( "}" );

                sb.Append( $".e-control-helper" ).Append( "{" )
                    .Append( $"color: {options.Color};" )
                    .AppendLine( "}" );

                sb.Append( $".e-select" ).Append( "{" )
                    .Append( $"color: {options.Color};" )
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

            sb.Append( $".e-tag.{variant}" ).Append( "{" )
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

            sb.Append( $".e-alert.{variant}" ).Append( "{" )
                .Append( $"color: {text};" )
                .Append( GradientBg( theme, background ) )
                .Append( $"border-color: {border};" )
                .AppendLine( "}" );

            sb.Append( $".e-alert.{variant}.alert-link" ).Append( "{" )
                .Append( $"color: {alertLink};" )
                .AppendLine( "}" );
        }

        protected override void GenerateCardStyles( StringBuilder sb, Theme theme, ThemeCardOptions options )
        {
            if ( !string.IsNullOrEmpty( options.BorderRadius ) )
                sb.Append( $".e-card" ).Append( "{" )
                    .Append( $"border-radius: {options.BorderRadius};" )
                    .AppendLine( "}" );
        }

        protected override void GenerateModalStyles( StringBuilder sb, Theme theme, ThemeModalOptions options )
        {
            if ( !string.IsNullOrEmpty( options.BorderRadius ) )
                sb.Append( $".e-modal-content" ).Append( "{" )
                    .Append( $"border-radius: {options.BorderRadius};" )
                    .AppendLine( "}" );
        }

        protected override void GenerateTabsStyles( StringBuilder sb, Theme theme, ThemeTabsOptions options )
        {
            if ( !string.IsNullOrEmpty( options.BorderRadius ) )
            {
                sb.Append( $".e-tabs" ).Append( "{" )
                    .Append( $"border-radius: {options.BorderRadius};" )
                    .AppendLine( "}" );
            }
        }

        protected override void GenerateProgressStyles( StringBuilder sb, Theme theme, ThemeProgressOptions options )
        {
            if ( !string.IsNullOrEmpty( options.BorderRadius ) )
                sb.Append( $".e-progress" ).Append( "{" )
                    .Append( $"border-radius: {options.BorderRadius};" )
                    .AppendLine( "}" );
        }

        protected override void GenerateAlertStyles( StringBuilder sb, Theme theme, ThemeAlertOptions options )
        {
            if ( !string.IsNullOrEmpty( options.BorderRadius ) )
                sb.Append( $".e-alert" ).Append( "{" )
                    .Append( $"border-radius: {options.BorderRadius};" )
                    .AppendLine( "}" );
        }

        protected override void GenerateBreadcrumbStyles( StringBuilder sb, Theme theme, ThemeBreadcrumbOptions options )
        {
            if ( !string.IsNullOrEmpty( options.BorderRadius ) )
            {
                sb.Append( $".e-breadcrumb" ).Append( "{" )
                     .Append( $"border-radius: {options.BorderRadius};" )
                     .AppendLine( "}" );
            }

            //if ( !string.IsNullOrEmpty( theme.ColorOptions?.Primary ) )
            //{
            //    sb.Append( $".breadcrumb-item>a" ).Append( "{" )
            //        .Append( $"color: {theme.ColorOptions.Primary};" )
            //        .AppendLine( "}" );
            //}
        }

        protected override void GenerateBadgeStyles( StringBuilder sb, Theme theme, ThemeBadgeOptions options )
        {
            if ( !string.IsNullOrEmpty( options.BorderRadius ) )
                sb.Append( $".e-tag" ).Append( "{" )
                    .Append( $"border-radius: {options.BorderRadius};" )
                    .AppendLine( "}" );
        }

        protected override void GeneratePaginationStyles( StringBuilder sb, Theme theme, ThemePaginationOptions options )
        {
            if ( !string.IsNullOrEmpty( options.BorderRadius ) )
            {
                sb.Append( $".e-page-item" ).Append( "{" )
                    .Append( $"border-radius: {options.BorderRadius};" )
                    .AppendLine( "}" );
            }

            if ( !string.IsNullOrEmpty( theme.ColorOptions?.Primary ) )
            {
                sb.Append( $".e-page-item.on-page" ).Append( "{" )
                    .Append( $"background-color: {theme.ColorOptions.Primary};" )
                    .Append( $"border-color: {theme.ColorOptions.Primary};" )
                    .AppendLine( "}" );
            }
        }

        #endregion
    }
}
