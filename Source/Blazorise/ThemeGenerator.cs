#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

    public abstract class ThemeGenerator : IThemeGenerator
    {
        #region Members

        protected readonly Dictionary<string, string> variables = new Dictionary<string, string>();

        #endregion

        #region Constructors

        public ThemeGenerator()
        {

        }

        #endregion

        #region Methods

        #region Variables

        public virtual void GenerateVariables( StringBuilder sb, Theme theme )
        {
            if ( !string.IsNullOrEmpty( theme.White ) )
                variables[ThemeVariables.White] = theme.White;

            if ( !string.IsNullOrEmpty( theme.Black ) )
                variables[ThemeVariables.Black] = theme.Black;

            variables[ThemeVariables.BorderRadius] = ".25rem";
            variables[ThemeVariables.BorderRadiusLarge] = ".3rem";
            variables[ThemeVariables.BorderRadiusSmall] = ".2rem";

            foreach ( var (name, size) in theme.ValidBreakpoints )
                GenerateBreakpointVariables( theme, name, size );

            foreach ( var (name, color) in theme.ValidColors )
                GenerateColorVariables( theme, name, color );

            foreach ( var (name, color) in theme.ValidBackgroundColors )
                GenerateBackgroundVariables( theme, name, color );

            foreach ( var (name, color) in theme.ValidTextColors )
                GenerateTextColorVariables( theme, name, color );

            if ( theme.SidebarOptions != null )
                GenerateSidebarVariables( theme.SidebarOptions );

            if ( theme.SnackbarOptions != null )
                GenerateSnackbarVariables( theme.SnackbarOptions );

            if ( theme.DividerOptions != null )
                GenerateDividerVariables( theme.DividerOptions );

            GenerateTooltipVariables( theme, theme.TooltipOptions );

            GenerateBreadcrumbVariables( theme, theme.BreadcrumbOptions );

            // apply variables
            foreach ( var kv in variables )
                sb.AppendLine( $"{kv.Key}: {kv.Value};" );
        }

        protected virtual void GenerateBreakpointVariables( Theme theme, string name, string size )
        {
            variables[ThemeVariables.Breakpoint( name )] = size;
        }

        protected virtual void GenerateColorVariables( Theme theme, string variant, string value )
        {
            variables[ThemeVariables.Color( variant )] = value;

            GenerateButtonColorVariables( variant, value, value, theme.ButtonOptions );
            GenerateOutlineButtonColorVariables( variant, value, theme.ButtonOptions );
            GenerateSnackbarColorVariables( theme, variant, value, theme.SnackbarOptions );
        }

        protected virtual void GenerateButtonColorVariables( string variant, string inBackgroundColor, string inBorderColor, ThemeButtonOptions options )
        {
            var backgroundColor = ParseColor( inBackgroundColor );
            var borderColor = ParseColor( inBorderColor );

            if ( backgroundColor.IsEmpty )
                return;

            var hoverBackgroundColor = Darken( backgroundColor, options?.HoverDarkenColor ?? 15f );
            var hoverBorderColor = Lighten( borderColor, options?.HoverLightenColor ?? 20f );
            var activeBackgroundColor = Darken( backgroundColor, options?.ActiveDarkenColor ?? 20f );
            var activeBorderColor = Lighten( borderColor, options?.ActiveLightenColor ?? 25f );
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

            var boxShadow = ToHexRGBA( Transparency( Blend( yiqBackgroundColor, backgroundColor, 15f ), options?.BoxShadowTransparency ?? 127 ) );

            variables[ThemeVariables.ButtonBackground( variant )] = background;
            variables[ThemeVariables.ButtonBorder( variant )] = border;
            variables[ThemeVariables.ButtonHoverBackground( variant )] = hoverBackground;
            variables[ThemeVariables.ButtonHoverBorder( variant )] = hoverBorder;
            variables[ThemeVariables.ButtonActiveBackground( variant )] = activeBackground;
            variables[ThemeVariables.ButtonActiveBorder( variant )] = activeBorder;
            variables[ThemeVariables.ButtonYiqBackground( variant )] = yiqBackground;
            variables[ThemeVariables.ButtonYiqHoverBackground( variant )] = yiqHoverBackground;
            variables[ThemeVariables.ButtonYiqActiveBackground( variant )] = yiqActiveBackground;
            variables[ThemeVariables.ButtonBoxShadow( variant )] = boxShadow;
        }

        protected virtual void GenerateOutlineButtonColorVariables( string variant, string inBorderColor, ThemeButtonOptions options )
        {
            var borderColor = ParseColor( inBorderColor );

            if ( borderColor.IsEmpty )
                return;

            var color = ToHex( borderColor );
            var yiqColor = ToHex( Contrast( borderColor ) );
            var boxShadow = ToHexRGBA( Transparency( borderColor, 127 ) );
            var hoverColor = ToHex( Lighten( borderColor, options?.HoverLightenColor ?? 20f ) );
            var activeColor = ToHex( Darken( borderColor, options?.ActiveDarkenColor ?? 20f ) );

            variables[ThemeVariables.OutlineButtonColor( variant )] = color;
            variables[ThemeVariables.OutlineButtonYiqColor( variant )] = yiqColor;
            variables[ThemeVariables.OutlineButtonBoxShadowColor( variant )] = boxShadow;
            variables[ThemeVariables.OutlineButtonHoverColor( variant )] = hoverColor;
            variables[ThemeVariables.OutlineButtonActiveColor( variant )] = activeColor;
        }

        protected virtual void GenerateSnackbarColorVariables( Theme theme, string variant, string inColor, ThemeSnackbarOptions options )
        {
            // this color variant is not supported
            if ( variant == "link" )
                return;

            var backgroundColor = ThemeColorLevel( theme, inColor, options?.VariantBackgroundColorLevel ?? -3 );
            var textColor = Contrast( backgroundColor );
            var buttonColor = Darken( textColor, 40f );
            var buttonHoverColor = Lighten( textColor, 40f );
            //var textColor = Contrast( ThemeColorLevel( theme, inColor, options?.VariantTextColorLevel ?? 6 ) );
            //var buttonColor = Contrast( ThemeColorLevel( theme, inColor, options?.VariantButtonColorLevel ?? 8 ) );
            //var buttonHoverColor = ThemeColorLevel( theme, buttonColor, options?.VariantButtonHoverColorLevel ?? 4 );

            variables[$"{ThemeVariables.SnackbarBackground}-{ variant }"] = ToHex( backgroundColor );
            variables[$"{ThemeVariables.SnackbarTextColor}-{ variant }"] = ToHex( textColor );
            variables[$"{ThemeVariables.SnackbarButtonColor}-{ variant }"] = ToHex( buttonColor );
            variables[$"{ThemeVariables.SnackbarButtonHoverColor}-{ variant }"] = ToHex( buttonHoverColor );
        }

        protected virtual void GenerateBackgroundVariables( Theme theme, string variant, string inColor )
        {
            var backgroundColor = ParseColor( inColor );

            if ( backgroundColor.IsEmpty )
                return;

            var backgroundYiqColor = Contrast( backgroundColor );

            variables[ThemeVariables.BackgroundColor( variant )] = ToHex( backgroundColor );
            variables[ThemeVariables.BackgroundYiqColor( variant )] = ToHex( backgroundYiqColor );
        }

        protected virtual void GenerateTextColorVariables( Theme theme, string variant, string inColor )
        {
            var color = ParseColor( inColor );

            if ( color.IsEmpty )
                return;

            variables[ThemeVariables.TextColor( variant )] = ToHex( color );
        }

        protected virtual void GenerateSidebarVariables( ThemeSidebarOptions sidebarOptions )
        {
            if ( sidebarOptions.Width != null )
                variables[ThemeVariables.SidebarWidth] = sidebarOptions.Width;

            if ( sidebarOptions.BackgroundColor != null )
                variables[ThemeVariables.SidebarBackground] = ToHex( ParseColor( sidebarOptions.BackgroundColor ) );

            if ( sidebarOptions.Color != null )
                variables[ThemeVariables.SidebarColor] = ToHex( ParseColor( sidebarOptions.Color ) );
        }

        protected virtual void GenerateSnackbarVariables( ThemeSnackbarOptions snackbarOptions )
        {
            if ( snackbarOptions?.BackgroundColor != null )
                variables[ThemeVariables.SnackbarBackground] = ToHex( ParseColor( snackbarOptions.BackgroundColor ) );

            if ( snackbarOptions?.TextColor != null )
                variables[ThemeVariables.SnackbarTextColor] = ToHex( ParseColor( snackbarOptions.TextColor ) );

            if ( snackbarOptions?.ButtonColor != null )
                variables[ThemeVariables.SnackbarButtonColor] = ToHex( ParseColor( snackbarOptions.ButtonColor ) );

            if ( snackbarOptions?.ButtonHoverColor != null )
                variables[ThemeVariables.SnackbarButtonHoverColor] = ToHex( ParseColor( snackbarOptions.ButtonHoverColor ) );
        }

        protected virtual void GenerateDividerVariables( ThemeDividerOptions dividerOptions )
        {
            if ( dividerOptions.Color != null )
                variables[ThemeVariables.DividerColor] = ToHex( ParseColor( dividerOptions.Color ) );

            if ( dividerOptions.Color != null )
                variables[ThemeVariables.DividerThickness] = dividerOptions.Thickness;

            if ( dividerOptions.Color != null )
                variables[ThemeVariables.DividerTextSize] = dividerOptions.TextSize;
        }

        protected virtual void GenerateTooltipVariables( Theme theme, ThemeTooltipOptions tooltipOptions )
        {
            if ( tooltipOptions?.BackgroundColor != null )
            {
                var backgroundColor = ParseColor( tooltipOptions.BackgroundColor );

                variables[ThemeVariables.TooltipBackgroundColorR] = backgroundColor.R.ToString( CultureInfo.InvariantCulture );
                variables[ThemeVariables.TooltipBackgroundColorG] = backgroundColor.G.ToString( CultureInfo.InvariantCulture );
                variables[ThemeVariables.TooltipBackgroundColorB] = backgroundColor.B.ToString( CultureInfo.InvariantCulture );
            }

            if ( tooltipOptions?.BackgroundOpacity != null )
            {
                variables[ThemeVariables.TooltipBackgroundOpacity] = tooltipOptions.BackgroundOpacity.Value.ToString( CultureInfo.InvariantCulture );
            }

            if ( tooltipOptions?.Color != null )
            {
                variables[ThemeVariables.TooltipColor] = tooltipOptions.Color;
            }

            if ( tooltipOptions?.FontSize != null )
            {
                variables[ThemeVariables.TooltipFontSize] = tooltipOptions.FontSize;
            }

            variables[ThemeVariables.TooltipBorderRadius] = GetBorderRadius( theme, tooltipOptions?.BorderRadius, Var( ThemeVariables.BorderRadius ) );

            if ( tooltipOptions?.FadeTime != null )
            {
                variables[ThemeVariables.TooltipFadeTime] = tooltipOptions.FadeTime;
            }

            if ( tooltipOptions?.MaxWidth != null )
            {
                variables[ThemeVariables.TooltipMaxWidth] = tooltipOptions.MaxWidth;
            }

            if ( tooltipOptions?.Padding != null )
            {
                variables[ThemeVariables.TooltipPadding] = tooltipOptions.Padding;
            }

            if ( tooltipOptions?.ZIndex != null )
            {
                variables[ThemeVariables.TooltipZIndex] = tooltipOptions.ZIndex;
            }
        }

        protected virtual void GenerateBreadcrumbVariables( Theme theme, ThemeBreadcrumbOptions breadcrumbOptions )
        {
            if ( FirstNotEmpty( out var color, breadcrumbOptions?.Color, theme.ColorOptions?.Primary ) )
            {
                variables[ThemeVariables.BreadcrumbColor] = color;
            }
        }

        protected string Var( string name, string defaultValue = null )
        {
            if ( variables.TryGetValue( name, out var value ) )
                return value;

            return defaultValue;
        }

        #endregion

        #region Styles

        public virtual void GenerateStyles( StringBuilder sb, Theme theme )
        {
            foreach ( var (name, size) in theme.ValidBreakpoints )
            {
                GenerateBreakpointStyles( sb, theme, name, size );
            }

            foreach ( var (name, color) in theme.ValidColors )
            {
                GenerateColorStyles( sb, theme, name, color );
            }

            foreach ( var (name, color) in theme.ValidBackgroundColors )
            {
                GenerateBackgroundStyles( sb, theme, name, color );
            }

            foreach ( var (name, color) in theme.ValidTextColors )
            {
                GenerateTypographyVariantStyles( sb, theme, name, color );
            }

            GenerateButtonStyles( sb, theme, theme.ButtonOptions );

            GenerateDropdownStyles( sb, theme, theme.DropdownOptions );

            GenerateInputStyles( sb, theme, theme.InputOptions );

            GenerateCardStyles( sb, theme, theme.CardOptions );

            GenerateModalStyles( sb, theme, theme.ModalOptions );

            GenerateTabsStyles( sb, theme, theme.TabsOptions );

            GenerateProgressStyles( sb, theme, theme.ProgressOptions );

            GenerateAlertStyles( sb, theme, theme.AlertOptions );

            GenerateBreadcrumbStyles( sb, theme, theme.BreadcrumbOptions );

            GenerateBadgeStyles( sb, theme, theme.BadgeOptions );

            GeneratePaginationStyles( sb, theme, theme.PaginationOptions );

            GenerateBarStyles( sb, theme, theme.BarOptions );
        }

        protected virtual void GenerateBreakpointStyles( StringBuilder sb, Theme theme, string breakpoint, string value )
        {
        }

        /// <summary>
        /// Generates styles that are based on the variant colors.
        /// </summary>
        /// <param name="sb">Target string builder.</param>
        /// <param name="variant">Variant name.</param>
        /// <param name="color">Color value.</param>
        protected virtual void GenerateColorStyles( StringBuilder sb, Theme theme, string variant, string color )
        {
            //GenerateBackgroundVariantStyles( sb, theme, variant );
            GenerateButtonVariantStyles( sb, theme, variant, theme.ButtonOptions );
            GenerateButtonOutlineVariantStyles( sb, theme, variant, theme.ButtonOptions );
            GenerateBadgeVariantStyles( sb, theme, variant, color );

            GenerateAlertVariantStyles( sb, theme, variant,
                ThemeColorLevelHex( theme, color, theme.AlertOptions?.BackgroundLevel ?? -10 ),
                ThemeColorLevelHex( theme, color, theme.AlertOptions?.BorderLevel ?? -7 ),
                ThemeColorLevelHex( theme, color, theme.AlertOptions?.ColorLevel ?? 6 ),
                theme.AlertOptions );

            GenerateTableVariantStyles( sb, theme, variant,
                ThemeColorLevelHex( theme, color, theme.TableOptions?.BackgroundLevel ?? -9 ),
                ThemeColorLevelHex( theme, color, theme.TableOptions?.BorderLevel ?? -6 ) );
        }

        protected virtual void GenerateBackgroundStyles( StringBuilder sb, Theme theme, string variant, string color )
        {
            GenerateBackgroundVariantStyles( sb, theme, variant );
        }

        protected virtual void GenerateTypographyVariantStyles( StringBuilder sb, Theme theme, string variant, string color )
        {
            GenerateParagraphVariantStyles( sb, theme, variant, color );
            GenerateInputVariantStyles( sb, theme, variant, color );
        }

        protected abstract void GenerateBackgroundVariantStyles( StringBuilder sb, Theme theme, string variant );

        protected abstract void GenerateButtonVariantStyles( StringBuilder sb, Theme theme, string variant, ThemeButtonOptions options );

        protected abstract void GenerateButtonOutlineVariantStyles( StringBuilder sb, Theme theme, string variant, ThemeButtonOptions buttonOptions );

        protected abstract void GenerateButtonStyles( StringBuilder sb, Theme theme, ThemeButtonOptions options );

        protected abstract void GenerateDropdownStyles( StringBuilder sb, Theme theme, ThemeDropdownOptions options );

        protected abstract void GenerateInputStyles( StringBuilder sb, Theme theme, ThemeInputOptions options );

        protected abstract void GenerateBadgeVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor );

        protected abstract void GenerateAlertVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, string inBorderColor, string inColor, ThemeAlertOptions options );

        protected abstract void GenerateTableVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, string inBorderColor );

        protected abstract void GenerateCardStyles( StringBuilder sb, Theme theme, ThemeCardOptions options );

        protected abstract void GenerateModalStyles( StringBuilder sb, Theme theme, ThemeModalOptions options );

        protected abstract void GenerateTabsStyles( StringBuilder sb, Theme theme, ThemeTabsOptions options );

        protected abstract void GenerateProgressStyles( StringBuilder sb, Theme theme, ThemeProgressOptions options );

        protected abstract void GenerateAlertStyles( StringBuilder sb, Theme theme, ThemeAlertOptions options );

        protected abstract void GenerateBreadcrumbStyles( StringBuilder sb, Theme theme, ThemeBreadcrumbOptions options );

        protected abstract void GenerateBadgeStyles( StringBuilder sb, Theme theme, ThemeBadgeOptions options );

        protected abstract void GeneratePaginationStyles( StringBuilder sb, Theme theme, ThemePaginationOptions options );

        protected abstract void GenerateBarStyles( StringBuilder sb, Theme theme, ThemeBarOptions options );

        protected abstract void GenerateParagraphVariantStyles( StringBuilder sb, Theme theme, string variant, string color );

        protected abstract void GenerateInputVariantStyles( StringBuilder sb, Theme theme, string variant, string color );

        #endregion

        #region Helpers

        private static string FirstNonEmptyString( params string[] values )
        {
            return values.FirstOrDefault( x => !string.IsNullOrEmpty( x ) );
        }

        protected string GetBorderRadius( Theme theme, string borderRadius, string fallbackRadius )
        {
            if ( theme.IsRounded )
                return FirstNonEmptyString( borderRadius, fallbackRadius, "0rem" );

            return "0rem";
        }

        protected virtual string GetGradientBg( Theme theme, string color, float? percentage, bool important = false )
        {
            return theme.IsGradient
                ? $"background: {color} linear-gradient(180deg, {ToHex( Blend( System.Drawing.Color.White, ParseColor( color ), percentage ?? 15f ) )}, {color}) repeat-x{( important ? " !important" : "" )};"
                : $"background-color: {color}{( important ? " !important" : "" )};";
        }

        protected System.Drawing.Color ThemeColorLevel( Theme theme, string inColor, int level )
        {
            var color = ParseColor( inColor );

            var colorBase = level > 0
                ? ParseColor( Var( ThemeVariables.Black, "#343a40" ) )
                : ParseColor( Var( ThemeVariables.White, "#ffffff" ) );

            level = Math.Abs( level );

            return Blend( colorBase, color, level * theme.ThemeColorInterval );
        }

        protected System.Drawing.Color ThemeColorLevel( Theme theme, System.Drawing.Color color, int level )
        {
            var colorBase = level > 0
                ? ParseColor( Var( ThemeVariables.Black, "#343a40" ) )
                : ParseColor( Var( ThemeVariables.White, "#ffffff" ) );

            level = Math.Abs( level );

            return Blend( colorBase, color, level * theme.ThemeColorInterval );
        }

        protected string ThemeColorLevelHex( Theme theme, string inColor, int level )
        {
            return ToHex( ThemeColorLevel( theme, inColor, level ) );
        }

        protected static System.Drawing.Color ParseColor( string value )
        {
            return value.StartsWith( "#" )
                ? HexStringToColor( value )
                : System.Drawing.Color.FromName( value );
        }

        protected static System.Drawing.Color HexStringToColor( string hexColor )
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
        protected static string ExtractHexDigits( string input )
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

        protected static string ToHex( System.Drawing.Color color )
        {
            return $"#{color.R.ToString( "X2" )}{color.G.ToString( "X2" )}{color.B.ToString( "X2" )}";
        }

        protected static string ToHexRGBA( System.Drawing.Color color )
        {
            return $"#{color.R.ToString( "X2" )}{color.G.ToString( "X2" )}{color.B.ToString( "X2" )}{color.A.ToString( "X2" )}";
        }

        protected static System.Drawing.Color Transparency( string hexColor, int A )
        {
            var color = ParseColor( hexColor );

            return System.Drawing.Color.FromArgb( A, color.R, color.G, color.B );
        }

        protected static System.Drawing.Color Transparency( System.Drawing.Color color, int A )
        {
            return System.Drawing.Color.FromArgb( A, color.R, color.G, color.B );
        }

        protected static System.Drawing.Color Darken( string hexColor, float percentage )
        {
            var color = ParseColor( hexColor );

            return Darken( color, percentage );
        }

        protected static System.Drawing.Color Darken( System.Drawing.Color color, float percentage )
        {
            return ChangeColorBrightness( color, -1 * percentage / 100f );
        }

        protected static System.Drawing.Color Lighten( string hexColor, float percentage )
        {
            var color = ParseColor( hexColor );

            return Lighten( color, percentage );
        }

        protected static System.Drawing.Color Lighten( System.Drawing.Color color, float percentage )
        {
            return ChangeColorBrightness( color, percentage / 100f );
        }

        protected System.Drawing.Color Invert( System.Drawing.Color color )
        {
            return System.Drawing.Color.FromArgb( 255 - color.R, 255 - color.G, 255 - color.B );
        }

        protected static System.Drawing.Color ChangeColorBrightness( System.Drawing.Color color, float correctionFactor )
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

        protected static System.Drawing.Color Contrast( string hexColor )
        {
            var color = ParseColor( hexColor );

            return Contrast( color );
        }

        protected static System.Drawing.Color Contrast( System.Drawing.Color color )
        {
            int d = 0;

            // Counting the perceptive luminance - human eye favors green color... 
            double luminance = ( 299 * color.R + 587 * color.G + 114 * color.B ) / 1000d;

            // The yiq lightness value that determines when the lightness of color changes from "dark" to "light". Acceptable values are between 0 and 255.
            if ( luminance > 150 )
                d = 0; // bright colors - black font
            else
                d = 255; // dark colors - white font

            return System.Drawing.Color.FromArgb( d, d, d );
        }

        protected static System.Drawing.Color Blend( System.Drawing.Color color, System.Drawing.Color color2, float percentage )
        {
            var alpha = percentage / 100f;
            byte r = (byte)( ( color.R * alpha ) + color2.R * ( 1f - alpha ) );
            byte g = (byte)( ( color.G * alpha ) + color2.G * ( 1f - alpha ) );
            byte b = (byte)( ( color.B * alpha ) + color2.B * ( 1f - alpha ) );
            return System.Drawing.Color.FromArgb( r, g, b );
        }

        protected bool FirstNotEmpty( out string first, params string[] values )
        {
            first = values?.FirstOrDefault( x => !string.IsNullOrEmpty( x ) );

            return first != null;
        }

        protected static string MediaBreakpointUp( string size, string content )
        {
            if ( !string.IsNullOrEmpty( size ) )
            {
                return $"@media (min-width: {size}) {{{content}}}";
            }
            else
            {
                return $"{content}";
            }
        }

        #endregion

        #endregion

        #region Properties

        [Inject] protected IClassProvider ClassProvider { get; set; }

        #endregion
    }
}
