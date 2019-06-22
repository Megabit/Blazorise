#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public interface IThemeGenerator
    {
        void GenerateVariables( StringBuilder sb, Theme theme );

        void GenerateStyles( StringBuilder sb, Theme theme );
    }

    public class ThemeGenerator : IThemeGenerator
    {
        #region Members

        #endregion

        #region Constructors

        public ThemeGenerator()
        {

        }

        #endregion

        #region Methods

        public virtual void GenerateVariables( StringBuilder sb, Theme theme )
        {
            if ( !string.IsNullOrEmpty( theme.White ) )
                sb.AppendLine( $"--b-theme-white: {theme.White};" );

            if ( !string.IsNullOrEmpty( theme.Black ) )
                sb.AppendLine( $"--b-theme-black: {theme.Black};" );

            foreach ( var (name, color) in theme.Variants )
                GenerateVariantVariables( sb, theme, name, color );
        }

        protected virtual void GenerateVariantVariables( StringBuilder sb, Theme theme, string variant, string value )
        {
            sb.AppendLine( $"--b-theme-variant-{variant}: {value};" );

            GenerateButtonVariables( sb, variant, value, value, theme.ButtonOptions );
        }

        protected virtual void GenerateButtonVariables( StringBuilder sb, string variant, string inBackground, string inBorder, ThemeButtonOptions options )
        {
            var backgroundColor = ParseColor( inBackground );
            var borderColor = ParseColor( inBorder );

            if ( backgroundColor.IsEmpty )
                return;

            var hoverBackgroundColor = Darken( backgroundColor, 7.5f );
            var hoverBorderColor = Lighten( borderColor, 10f );
            var activeBackgroundColor = Darken( backgroundColor, 10f );
            var activeBorderColor = Lighten( borderColor, 12.5f );
            var yiqBackgroundColor = Contrast( backgroundColor );
            var yiqHoverBackgroundColor = Contrast( hoverBackgroundColor );
            var yiqActiveBackgroundColor = Contrast( activeBackgroundColor );

            var background = ToHex( backgroundColor );
            var border = ToHex( borderColor );
            var hoverBackground = ToHex( hoverBackgroundColor );
            var hoverBorder = ToHex( hoverBorderColor );
            var activeBackground = ToHex( activeBackgroundColor );
            var activeBorder = ToHex( activeBorderColor );
            var yiqBackground = ToHex( yiqBackgroundColor );
            var yiqHoverBackground = ToHex( yiqHoverBackgroundColor );
            var yiqActiveBackground = ToHex( yiqActiveBackgroundColor );

            var transparentColor = Transparency( Blend( yiqBackgroundColor, backgroundColor, 15f ), options?.BoxShadowTransparency ?? 127 );
            var transparent = ToHexRGBA( transparentColor );

            sb.AppendLine( $"--b-button-{variant}-background: {background};" );
            sb.AppendLine( $"--b-button-{variant}-border: {border};" );
            sb.AppendLine( $"--b-button-{variant}-hover-background: {hoverBackground};" );
            sb.AppendLine( $"--b-button-{variant}-hover-border: {hoverBorder};" );
            sb.AppendLine( $"--b-button-{variant}-active-background: {activeBackground};" );
            sb.AppendLine( $"--b-button-{variant}-active-border: {activeBorder};" );
            sb.AppendLine( $"--b-button-{variant}-yiq-background: {yiqBackground};" );
            sb.AppendLine( $"--b-button-{variant}-yiq-hover-background: {yiqHoverBackground};" );
            sb.AppendLine( $"--b-button-{variant}-yiq-active-background: {yiqActiveBackground};" );
            sb.AppendLine( $"--b-button-{variant}-transparent: {transparent};" );
        }

        public virtual void GenerateStyles( StringBuilder sb, Theme theme )
        {
            foreach ( var (name, color) in theme.Variants )
            {
                GenerateVariantStyles( sb, theme, name, color );
            }

            if ( theme.ButtonOptions != null )
                GenerateButtonStyles( sb, theme, theme.ButtonOptions );

            if ( theme.DropdownOptions != null )
                GenerateDropdownStyles( sb, theme, theme.DropdownOptions );

            if ( theme.InputOptions != null )
                GenerateInputStyles( sb, theme, theme.InputOptions );

            if ( theme.CardOptions != null )
                GenerateCardStyles( sb, theme, theme.CardOptions );

            if ( theme.ModalOptions != null )
                GenerateModalStyles( sb, theme, theme.ModalOptions );

            if ( theme.TabsOptions != null )
                GenerateTabsStyles( sb, theme, theme.TabsOptions );

            if ( theme.ProgressOptions != null )
                GenerateProgressStyles( sb, theme, theme.ProgressOptions );

            if ( theme.AlertOptions != null )
                GenerateAlertStyles( sb, theme, theme.AlertOptions );

            if ( theme.BreadcrumbOptions != null )
                GenerateBreadcrumbStyles( sb, theme, theme.BreadcrumbOptions );

            if ( theme.BadgeOptions != null )
                GenerateBadgeStyles( sb, theme, theme.BadgeOptions );

            if ( theme.PaginationOptions != null )
                GeneratePaginationStyles( sb, theme, theme.PaginationOptions );
        }

        /// <summary>
        /// Generates styles that are based on the variant colors.
        /// </summary>
        /// <param name="sb">Target string builder.</param>
        /// <param name="variant">Variant name.</param>
        /// <param name="color">Color value.</param>
        protected virtual void GenerateVariantStyles( StringBuilder sb, Theme theme, string variant, string color )
        {
            GenerateButtonVariantStyles( sb, theme, variant, color, color, theme.ButtonOptions );
            GenerateButtonOutlineVariantStyles( sb, theme, variant, color, theme.ButtonOptions );
            GenerateBadgeVariantStyles( sb, theme, variant, color );

            GenerateAlertVariantStyles( sb, theme, variant,
                ThemeColorLevel( theme, color, theme.AlertOptions?.BackgroundLevel ?? -10 ),
                ThemeColorLevel( theme, color, theme.AlertOptions?.BorderLevel ?? -9 ),
                ThemeColorLevel( theme, color, theme.AlertOptions?.ColorLevel ?? 6 ) );
        }

        protected virtual void GenerateButtonVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, string inBorderColor, ThemeButtonOptions options )
        {
            var backgroundColor = ParseColor( inBackgroundColor );
            var borderColor = ParseColor( inBorderColor );

            if ( backgroundColor.IsEmpty )
                return;

            var hoverBackgroundColor = Darken( backgroundColor, 7.5f );
            var hoverBorderColor = Lighten( borderColor, 10f );
            var activeBackgroundColor = Darken( backgroundColor, 10f );
            var activeBorderColor = Lighten( borderColor, 12.5f );
            var yiqBackgroundColor = Contrast( backgroundColor );
            var yiqHoverBackgroundColor = Contrast( hoverBackgroundColor );
            var yiqActiveBackgroundColor = Contrast( activeBackgroundColor );

            var background = ToHex( backgroundColor );
            var border = ToHex( borderColor );
            var hoverBackground = ToHex( hoverBackgroundColor );
            var hoverBorder = ToHex( hoverBorderColor );
            var activeBackground = ToHex( activeBackgroundColor );
            var activeBorder = ToHex( activeBorderColor );
            var yiqBackground = ToHex( yiqBackgroundColor );
            var yiqHoverBackground = ToHex( yiqHoverBackgroundColor );
            var yiqActiveBackground = ToHex( yiqActiveBackgroundColor );

            sb.Append( $".btn-{variant}" ).Append( "{" )
                .Append( $"color: {yiqBackground};" )
                .Append( GradientBg( theme, background ) )
                .Append( $"border-color: {border};" )
                .AppendLine( "}" );

            sb.Append( $".btn-{variant}:hover" ).Append( "{" )
                .Append( $"color: {yiqHoverBackground};" )
                .Append( GradientBg( theme, hoverBackground ) )
                .Append( $"border-color: {hoverBorder};" )
                .AppendLine( "}" );

            var transparentColor = Transparency( Blend( yiqBackgroundColor, backgroundColor, 15f ), options?.BoxShadowTransparency ?? 127 );
            var transparent = ToHexRGBA( transparentColor );

            sb.Append( $".btn-{variant}:focus" ).Append( $".btn-{variant}.focus" ).Append( "{" )
                .Append( $"color: {yiqHoverBackground};" )
                .Append( GradientBg( theme, hoverBackground ) )
                .Append( $"border-color: {hoverBorder};" )
                .Append( $"box-shadow: 0 0 0 {options?.BoxShadowSize ?? ".2rem"} {transparent};" )
                .AppendLine( "}" );

            sb.Append( $".btn-{variant}.disabled" ).Append( $".btn-{variant}:disabled" ).Append( "{" )
                .Append( $"color: {yiqBackground};" )
                .Append( $"background-color: {background};" )
                .Append( $"border-color: {border};" )
                .Append( $"box-shadow: 0 0 0 {options?.BoxShadowSize ?? ".2rem"} {transparent};" )
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
                .Append( $"box-shadow: 0 0 0 {options?.BoxShadowSize ?? ".2rem"} {transparent}" )
                .AppendLine( "}" );
        }

        protected virtual void GenerateButtonOutlineVariantStyles( StringBuilder sb, Theme theme, string variant, string inBorderColor, ThemeButtonOptions buttonOptions )
        {
            var borderColor = ParseColor( inBorderColor );

            if ( borderColor.IsEmpty )
                return;

            var transparentColor = Transparency( borderColor, 127 );

            var color = ToHex( borderColor );
            var yiqColor = ToHex( Contrast( borderColor ) );
            var transparent = ToHexRGBA( transparentColor );

            sb.Append( $".btn-outline-{variant}" ).Append( "{" )
                .Append( $"color: {color};" )
                .Append( $"border-color: {color};" )
                .AppendLine( "}" );

            sb.Append( $".btn-outline-{variant}:hover" ).Append( "{" )
                .Append( $"color: {yiqColor};" )
                .Append( $"background-color: {color};" )
                .Append( $"border-color: {color};" )
                .AppendLine( "}" );

            sb.Append( $".btn-outline-{variant}:focus" ).Append( $".btn-outline-{variant}.focus" ).Append( "{" )
                .Append( $"box-shadow: 0 0 0 2rem {transparent};" )
                .AppendLine( "}" );

            sb.Append( $".btn-outline-{variant}.disabled" ).Append( $".btn-outline-{variant}:disabled" ).Append( "{" )
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
                .Append( $"box-shadow: 0 0 0 2rem {transparent};" )
                .AppendLine( "}" );
        }

        protected virtual void GenerateButtonStyles( StringBuilder sb, Theme theme, ThemeButtonOptions options )
        {
            if ( !string.IsNullOrEmpty( options.BorderRadius ) )
                sb.Append( $".btn" ).Append( "{" )
                    .Append( $"border-radius: {options.BorderRadius};" )
                    .AppendLine( "}" );

            if ( !string.IsNullOrEmpty( options.Padding ) )
                sb.Append( $".btn" ).Append( "{" )
                    .Append( $"padding: {options.Padding};" )
                    .AppendLine( "}" );

            if ( !string.IsNullOrEmpty( options.Margin ) )
                sb.Append( $".btn" ).Append( "{" )
                    .Append( $"margin: {options.Margin};" )
                    .AppendLine( "}" );
        }

        protected virtual void GenerateDropdownStyles( StringBuilder sb, Theme theme, ThemeDropdownOptions options )
        {
            if ( !string.IsNullOrEmpty( options.BorderRadius ) )
                sb.Append( $".dropdown-menu" ).Append( "{" )
                    .Append( $"border-radius: {options.BorderRadius};" )
                    .AppendLine( "}" );

            if ( !string.IsNullOrEmpty( theme.VariantOptions?.Primary ) )
            {
                var backgroundColor = ParseColor( theme.VariantOptions.Primary );

                if ( !backgroundColor.IsEmpty )
                {
                    var background = ToHex( backgroundColor );

                    sb.Append( $".dropdown-item.active," )
                        .Append( $".dropdown-item:active" ).Append( "{" )
                        .Append( GradientBg( theme, background ) )
                        .AppendLine( "}" );
                }
            }
        }

        protected virtual void GenerateInputStyles( StringBuilder sb, Theme theme, ThemeInputOptions options )
        {
            if ( !string.IsNullOrEmpty( options.BorderRadius ) )
            {
                sb.Append( $".form-control" ).Append( "{" )
                    .Append( $"border-radius: {options.BorderRadius};" )
                    .AppendLine( "}" );

                sb.Append( $".input-group-text" ).Append( "{" )
                    .Append( $"border-radius: {options.BorderRadius};" )
                    .AppendLine( "}" );

                sb.Append( $".custom-select" ).Append( "{" )
                    .Append( $"border-radius: {options.BorderRadius};" )
                    .AppendLine( "}" );

                sb.Append( $".custom-checkbox .custom-control-label::before" ).Append( "{" )
                    .Append( $"border-radius: {options.BorderRadius};" )
                    .AppendLine( "}" );

                sb.Append( $".custom-file-label" ).Append( "{" )
                    .Append( $"border-radius: {options.BorderRadius};" )
                    .AppendLine( "}" );
            }

            if ( !string.IsNullOrEmpty( options.Color ) )
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
        }

        protected virtual void GenerateBadgeVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor )
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

        protected virtual void GenerateAlertVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, string inBorderColor, string inColor )
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
                .Append( GradientBg( theme, background ) )
                .Append( $"border-color: {border};" )
                .AppendLine( "}" );

            sb.Append( $".alert-{variant}.alert-link" ).Append( "{" )
                .Append( $"color: {alertLink};" )
                .AppendLine( "}" );
        }

        protected virtual void GenerateCardStyles( StringBuilder sb, Theme theme, ThemeCardOptions options )
        {
            if ( !string.IsNullOrEmpty( options.BorderRadius ) )
                sb.Append( $".card" ).Append( "{" )
                    .Append( $"border-radius: {options.BorderRadius};" )
                    .AppendLine( "}" );

            if ( !string.IsNullOrEmpty( options.ImageTopRadius ) )
                sb.Append( $".card-image-top" ).Append( "{" )
                    .Append( $"border-top-left-radius: {options.ImageTopRadius};" )
                    .Append( $"border-top-right-radius: {options.ImageTopRadius};" )
                    .AppendLine( "}" );
        }

        protected virtual void GenerateModalStyles( StringBuilder sb, Theme theme, ThemeModalOptions options )
        {
            if ( !string.IsNullOrEmpty( options.BorderRadius ) )
                sb.Append( $".modal-content" ).Append( "{" )
                    .Append( $"border-radius: {options.BorderRadius};" )
                    .AppendLine( "}" );
        }

        protected virtual void GenerateTabsStyles( StringBuilder sb, Theme theme, ThemeTabsOptions options )
        {
            if ( !string.IsNullOrEmpty( options.BorderRadius ) )
            {
                sb.Append( $".nav-tabs .nav-link" ).Append( "{" )
                    .Append( $"border-top-left-radius: {options.BorderRadius};" )
                    .Append( $"border-top-right-radius: {options.BorderRadius};" )
                    .AppendLine( "}" );

                sb.Append( $".nav-pills .nav-link" ).Append( "{" )
                    .Append( $"border-radius: {options.BorderRadius};" )
                    .AppendLine( "}" );
            }
        }

        protected virtual void GenerateProgressStyles( StringBuilder sb, Theme theme, ThemeProgressOptions options )
        {
            if ( !string.IsNullOrEmpty( options.BorderRadius ) )
                sb.Append( $".progress" ).Append( "{" )
                    .Append( $"border-radius: {options.BorderRadius};" )
                    .AppendLine( "}" );
        }

        protected virtual void GenerateAlertStyles( StringBuilder sb, Theme theme, ThemeAlertOptions options )
        {
            if ( !string.IsNullOrEmpty( options.BorderRadius ) )
                sb.Append( $".alert" ).Append( "{" )
                    .Append( $"border-radius: {options.BorderRadius};" )
                    .AppendLine( "}" );
        }

        protected virtual void GenerateBreadcrumbStyles( StringBuilder sb, Theme theme, ThemeBreadcrumbOptions options )
        {
            if ( !string.IsNullOrEmpty( options.BorderRadius ) )
            {
                sb.Append( $".breadcrumb" ).Append( "{" )
                     .Append( $"border-radius: {options.BorderRadius};" )
                     .AppendLine( "}" );
            }

            if ( !string.IsNullOrEmpty( theme.VariantOptions?.Primary ) )
            {
                sb.Append( $".breadcrumb-item>a" ).Append( "{" )
                    .Append( $"color: {theme.VariantOptions.Primary};" )
                    .AppendLine( "}" );
            }
        }

        protected virtual void GenerateBadgeStyles( StringBuilder sb, Theme theme, ThemeBadgeOptions options )
        {
            if ( !string.IsNullOrEmpty( options.BorderRadius ) )
                sb.Append( $".badge" ).Append( "{" )
                    .Append( $"border-radius: {options.BorderRadius};" )
                    .AppendLine( "}" );
        }

        protected virtual void GeneratePaginationStyles( StringBuilder sb, Theme theme, ThemePaginationOptions options )
        {
            if ( !string.IsNullOrEmpty( options.BorderRadius ) )
            {
                sb.Append( $".page-item:first-child .page-link" ).Append( "{" )
                    .Append( $"border-top-left-radius: {options.BorderRadius};" )
                    .Append( $"border-bottom-left-radius: {options.BorderRadius};" )
                    .AppendLine( "}" );

                sb.Append( $".page-item:last-child .page-link" ).Append( "{" )
                    .Append( $"border-top-right-radius: {options.BorderRadius};" )
                    .Append( $"border-bottom-right-radius: {options.BorderRadius};" )
                    .AppendLine( "}" );
            }

            if ( !string.IsNullOrEmpty( options.LargeBorderRadius ) )
            {
                sb.Append( $".pagination-lg .page-item:first-child .page-link" ).Append( "{" )
                    .Append( $"border-top-left-radius: {options.BorderRadius};" )
                    .Append( $"border-bottom-left-radius: {options.BorderRadius};" )
                    .AppendLine( "}" );

                sb.Append( $".pagination-lg .page-item:last-child .page-link" ).Append( "{" )
                    .Append( $"border-top-right-radius: {options.LargeBorderRadius};" )
                    .Append( $"border-bottom-right-radius: {options.LargeBorderRadius};" )
                    .AppendLine( "}" );
            }

            if ( !string.IsNullOrEmpty( theme.VariantOptions?.Primary ) )
            {
                sb.Append( $".page-item.active .page-link" ).Append( "{" )
                    .Append( $"background-color: {theme.VariantOptions.Primary};" )
                    .Append( $"border-color: {theme.VariantOptions.Primary};" )
                    .AppendLine( "}" );
            }
        }

        #region Helpers

        protected virtual string GradientBg( Theme theme, string color )
        {
            return theme.IsGradient
                ? $"background: {color} linear-gradient(180deg, {ToHex( Blend( System.Drawing.Color.White, ParseColor( color ), 15 ) )}, {color}) repeat-x;"
                : $"background-color: {color};";
        }

        protected virtual string ThemeColorLevel( Theme theme, string inColor, int level )
        {
            var color = ParseColor( inColor );
            var colorBase = level > 0 ? ParseColor( theme.Black ?? "#343a40" ) : ParseColor( theme.White ?? "#ffffff" );
            level = Math.Abs( level );
            return ToHex( Blend( colorBase, color, level * 8f ) );
        }

        static System.Drawing.Color ParseColor( string value )
        {
            return value.StartsWith( "#" )
                ? HexStringToColor( value )
                : System.Drawing.Color.FromName( value );
        }

        static System.Drawing.Color HexStringToColor( string hexColor )
        {
            string hc = ExtractHexDigits( hexColor );

            if ( hc.Length != 6 )
            {
                // you can choose whether to throw an exception
                //throw new ArgumentException("hexColor is not exactly 6 digits.");
                return System.Drawing.Color.Empty;
            }

            string r = hc.Substring( 0, 2 );
            string g = hc.Substring( 2, 2 );
            string b = hc.Substring( 4, 2 );
            System.Drawing.Color color;
            try
            {
                int ri = Int32.Parse( r, System.Globalization.NumberStyles.HexNumber );
                int gi = Int32.Parse( g, System.Globalization.NumberStyles.HexNumber );
                int bi = Int32.Parse( b, System.Globalization.NumberStyles.HexNumber );
                color = System.Drawing.Color.FromArgb( ri, gi, bi );
            }
            catch
            {
                // you can choose whether to throw an exception
                //throw new ArgumentException("Conversion failed.");
                return System.Drawing.Color.Empty;
            }
            return color;
        }

        /// <summary>
        /// Extract only the hex digits from a string.
        /// </summary>
        static string ExtractHexDigits( string input )
        {
            // remove any characters that are not digits (like #)
            Regex isHexDigit = new Regex( "[abcdefABCDEF\\d]+", RegexOptions.Compiled );
            string newnum = "";
            foreach ( char c in input )
            {
                if ( isHexDigit.IsMatch( c.ToString() ) )
                    newnum += c.ToString();
            }
            return newnum;
        }

        static string ToHex( System.Drawing.Color color )
        {
            return $"#{color.R.ToString( "X2" )}{color.G.ToString( "X2" )}{color.B.ToString( "X2" )}";
        }

        static string ToHexRGBA( System.Drawing.Color color )
        {
            return $"#{color.R.ToString( "X2" )}{color.G.ToString( "X2" )}{color.B.ToString( "X2" )}{color.A.ToString( "X2" )}";
        }

        static System.Drawing.Color Transparency( System.Drawing.Color color, int A )
        {
            return System.Drawing.Color.FromArgb( A, color.R, color.G, color.B );
        }

        static System.Drawing.Color Darken( System.Drawing.Color color, float correctionFactor )
        {
            return ChangeColorBrightness( color, -( correctionFactor / 100f ) );
        }

        static System.Drawing.Color Lighten( System.Drawing.Color color, float correctionFactor )
        {
            return ChangeColorBrightness( color, correctionFactor / 100f );
        }

        static System.Drawing.Color ChangeColorBrightness( System.Drawing.Color color, float correctionFactor )
        {
            float red = color.R;
            float green = color.G;
            float blue = color.B;

            if ( correctionFactor < 0 )
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = ( 255 - red ) * correctionFactor + red;
                green = ( 255 - green ) * correctionFactor + green;
                blue = ( 255 - blue ) * correctionFactor + blue;
            }

            return System.Drawing.Color.FromArgb( color.A, (int)red, (int)green, (int)blue );
        }

        static System.Drawing.Color Contrast( System.Drawing.Color color )
        {
            int d = 0;

            // Counting the perceptive luminance - human eye favors green color... 
            double luminance = ( 0.299 * color.R + 0.587 * color.G + 0.114 * color.B ) / 255;

            if ( luminance > 0.5 )
                d = 0; // bright colors - black font
            else
                d = 255; // dark colors - white font

            return System.Drawing.Color.FromArgb( d, d, d );
        }

        static System.Drawing.Color Blend( System.Drawing.Color color, System.Drawing.Color backColor, float percentage )
        {
            var amount = percentage / 100;
            byte r = (byte)( ( color.R * amount ) + backColor.R * ( 1 - amount ) );
            byte g = (byte)( ( color.G * amount ) + backColor.G * ( 1 - amount ) );
            byte b = (byte)( ( color.B * amount ) + backColor.B * ( 1 - amount ) );
            return System.Drawing.Color.FromArgb( r, g, b );
        }

        #endregion

        #endregion

        #region Properties

        [Inject] protected IClassProvider ClassProvider { get; set; }

        #endregion
    }
}
