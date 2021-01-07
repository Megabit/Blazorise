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
                GenerateSidebarVariables( theme, theme.SidebarOptions );

            if ( theme.BarOptions != null )
                GenerateBarVariables( theme.BarOptions );

            if ( theme.SnackbarOptions != null )
                GenerateSnackbarVariables( theme, theme.SnackbarOptions );

            if ( theme.DividerOptions != null )
                GenerateDividerVariables( theme, theme.DividerOptions );

            GenerateTooltipVariables( theme, theme.TooltipOptions );

            GenerateBreadcrumbVariables( theme, theme.BreadcrumbOptions );

            GenerateStepsVariables( theme, theme.StepsOptions );

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

            GenerateButtonColorVariables( theme, variant, value, value, theme.ButtonOptions );
            GenerateOutlineButtonColorVariables( theme, variant, value, theme.ButtonOptions );
            GenerateSnackbarColorVariables( theme, variant, value, theme.SnackbarOptions );
            GenerateStepsColorVariables( theme, variant, value, theme.StepsOptions );
            GenerateProgressColorVariables( theme, variant, value, theme.ProgressOptions );
        }

        protected virtual void GenerateButtonColorVariables( Theme theme, string variant, string inBackgroundColor, string inBorderColor, ThemeButtonOptions options )
        {
            var backgroundColor = ParseColor( inBackgroundColor );
            var borderColor = ParseColor( inBorderColor );

            if ( backgroundColor.IsEmpty )
                return;

            var hoverBackgroundColor = Darken( backgroundColor, options?.HoverDarkenColor ?? 15f );
            var hoverBorderColor = Lighten( borderColor, options?.HoverLightenColor ?? 20f );
            var activeBackgroundColor = Darken( backgroundColor, options?.ActiveDarkenColor ?? 20f );
            var activeBorderColor = Lighten( borderColor, options?.ActiveLightenColor ?? 25f );
            var yiqBackgroundColor = Contrast( theme, backgroundColor );
            var yiqHoverBackgroundColor = Contrast( theme, hoverBackgroundColor );
            var yiqActiveBackgroundColor = Contrast( theme, activeBackgroundColor );

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

        protected virtual void GenerateOutlineButtonColorVariables( Theme theme, string variant, string inBorderColor, ThemeButtonOptions options )
        {
            var borderColor = ParseColor( inBorderColor );

            if ( borderColor.IsEmpty )
                return;

            var color = ToHex( borderColor );
            var yiqColor = ToHex( Contrast( theme, borderColor ) );
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
            var textColor = Contrast( theme, backgroundColor );
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

        protected virtual void GenerateStepsColorVariables( Theme theme, string variant, string inColor, ThemeStepsOptions options )
        {
            var argbColor = ParseColor( inColor );

            if ( argbColor.IsEmpty )
                return;

            var color = ToHex( argbColor );

            variables[ThemeVariables.VariantStepsItemIcon( variant )] = color;
            variables[ThemeVariables.VariantStepsItemIconYiq( variant )] = ToHex( Contrast( theme, color ) );
            variables[ThemeVariables.VariantStepsItemText( variant )] = color;
        }

        protected virtual void GenerateProgressColorVariables( Theme theme, string variant, string inColor, ThemeProgressOptions options )
        {
            var inArgbColor = ParseColor( inColor );

            if ( inArgbColor.IsEmpty )
                return;

            var color = ToHex( inArgbColor );

            variables[ThemeVariables.VariantPageProgressIndicator( variant )] = color;
        }

        protected virtual void GenerateBackgroundVariables( Theme theme, string variant, string inColor )
        {
            var backgroundColor = ParseColor( inColor );

            if ( backgroundColor.IsEmpty )
                return;

            var backgroundYiqColor = Contrast( theme, backgroundColor );

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

        protected virtual void GenerateSidebarVariables( Theme theme, ThemeSidebarOptions sidebarOptions )
        {
            if ( sidebarOptions.Width != null )
                variables[ThemeVariables.SidebarWidth] = sidebarOptions.Width;

            if ( sidebarOptions.BackgroundColor != null )
                variables[ThemeVariables.SidebarBackground] = ToHex( ParseColor( sidebarOptions.BackgroundColor ) );

            if ( sidebarOptions.Color != null )
                variables[ThemeVariables.SidebarColor] = ToHex( ParseColor( sidebarOptions.Color ) );
        }

        protected virtual void GenerateBarVariables( ThemeBarOptions barOptions )
        {
            if ( !string.IsNullOrEmpty( barOptions.VerticalWidth ) )
                variables[ThemeVariables.VerticalBarWidth] = barOptions.VerticalWidth;

            if ( !string.IsNullOrEmpty( barOptions.VerticalSmallWidth ) )
                variables[ThemeVariables.VerticalBarSmallWidth] = barOptions.VerticalSmallWidth;

            if ( !string.IsNullOrEmpty( barOptions.VerticalBrandHeight ) )
                variables[ThemeVariables.VerticalBarBrandHeight] = barOptions.VerticalBrandHeight;

            if ( !string.IsNullOrEmpty( barOptions.VerticalPopoutMenuWidth ) )
                variables[ThemeVariables.VerticalPopoutMenuWidth] = barOptions.VerticalPopoutMenuWidth;

            if ( !string.IsNullOrEmpty( barOptions.HorizontalHeight ) )
                variables[ThemeVariables.HorizontalBarHeight] = barOptions.HorizontalHeight;

            if ( barOptions?.DarkColors != null )
            {
                variables[ThemeVariables.BarDarkBackground] = ToHex( ParseColor( barOptions.DarkColors.BackgroundColor ) );
                variables[ThemeVariables.BarDarkColor] = ToHex( ParseColor( barOptions.DarkColors.Color ) );

                if ( barOptions.DarkColors.ItemColorOptions != null )
                {
                    variables[ThemeVariables.BarItemDarkActiveBackground] = ToHex( ParseColor( barOptions.DarkColors.ItemColorOptions.ActiveBackgroundColor ) );
                    variables[ThemeVariables.BarItemDarkActiveColor] = ToHex( ParseColor( barOptions.DarkColors.ItemColorOptions.ActiveColor ) );

                    variables[ThemeVariables.BarItemDarkHoverBackground] = ToHex( ParseColor( barOptions.DarkColors.ItemColorOptions.HoverBackgroundColor ) );
                    variables[ThemeVariables.BarItemDarkHoverColor] = ToHex( ParseColor( barOptions.DarkColors.ItemColorOptions.HoverColor ) );
                }

                if ( barOptions.DarkColors.DropdownColorOptions != null )
                {
                    variables[ThemeVariables.BarDropdownDarkBackground] = ToHex( ParseColor( barOptions.DarkColors.DropdownColorOptions.BackgroundColor ) );
                }

                if ( barOptions.DarkColors.BrandColorOptions != null )
                {
                    variables[ThemeVariables.BarBrandDarkBackground] = ToHex( ParseColor( barOptions.DarkColors.BrandColorOptions.BackgroundColor ) );
                }
            }

            if ( barOptions?.LightColors != null )
            {
                variables[ThemeVariables.BarLightBackground] = ToHex( ParseColor( barOptions.LightColors.BackgroundColor ) );
                variables[ThemeVariables.BarLightColor] = ToHex( ParseColor( barOptions.LightColors.Color ) );

                if ( barOptions.LightColors.ItemColorOptions != null )
                {
                    variables[ThemeVariables.BarItemLightActiveBackground] = ToHex( ParseColor( barOptions.LightColors.ItemColorOptions.ActiveBackgroundColor ) );
                    variables[ThemeVariables.BarItemLightActiveColor] = ToHex( ParseColor( barOptions.LightColors.ItemColorOptions.ActiveColor ) );

                    variables[ThemeVariables.BarItemLightHoverBackground] = ToHex( ParseColor( barOptions.LightColors.ItemColorOptions.HoverBackgroundColor ) );
                    variables[ThemeVariables.BarItemLightHoverColor] = ToHex( ParseColor( barOptions.LightColors.ItemColorOptions.HoverColor ) );
                }

                if ( barOptions.LightColors.DropdownColorOptions != null )
                {
                    variables[ThemeVariables.BarDropdownLightBackground] = ToHex( ParseColor( barOptions.LightColors.DropdownColorOptions.BackgroundColor ) );
                }

                if ( barOptions.LightColors.BrandColorOptions != null )
                {
                    variables[ThemeVariables.BarBrandLightBackground] = ToHex( ParseColor( barOptions.LightColors.BrandColorOptions.BackgroundColor ) );
                }
            }
        }

        protected virtual void GenerateSnackbarVariables( Theme theme, ThemeSnackbarOptions snackbarOptions )
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

        protected virtual void GenerateDividerVariables( Theme theme, ThemeDividerOptions dividerOptions )
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
                variables[ThemeVariables.TooltipBackgroundOpacity] = ( backgroundColor.A / 255f ).ToString( "n2", CultureInfo.InvariantCulture );
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

        protected virtual void GenerateStepsVariables( Theme theme, ThemeStepsOptions stepsOptions )
        {
            if ( stepsOptions != null )
            {
                if ( !string.IsNullOrEmpty( stepsOptions.StepsItemIconColor ) )
                {
                    variables[ThemeVariables.StepsItemIcon] = ToHex( ParseColor( stepsOptions.StepsItemIconColor ) );
                }

                if ( !string.IsNullOrEmpty( stepsOptions.StepsItemIconCompleted ) )
                {
                    variables[ThemeVariables.StepsItemIconCompleted] = ToHex( ParseColor( stepsOptions.StepsItemIconCompleted ) );
                }

                if ( !string.IsNullOrEmpty( stepsOptions.StepsItemIconCompletedYiq ) )
                {
                    variables[ThemeVariables.StepsItemIconCompletedYiq] = ToHex( ParseColor( stepsOptions.StepsItemIconCompletedYiq ) );
                }

                if ( !string.IsNullOrEmpty( stepsOptions.StepsItemIconActive ) )
                {
                    variables[ThemeVariables.StepsItemIconActive] = ToHex( ParseColor( stepsOptions.StepsItemIconActive ) );
                }

                if ( !string.IsNullOrEmpty( stepsOptions.StepsItemIconActiveYiq ) )
                {
                    variables[ThemeVariables.StepsItemIconActiveYiq] = ToHex( ParseColor( stepsOptions.StepsItemIconActiveYiq ) );
                }

                if ( !string.IsNullOrEmpty( stepsOptions.StepsItemTextColor ) )
                {
                    variables[ThemeVariables.StepsItemText] = ToHex( ParseColor( stepsOptions.StepsItemTextColor ) );
                }

                if ( !string.IsNullOrEmpty( stepsOptions.StepsItemTextCompleted ) )
                {
                    variables[ThemeVariables.StepsItemTextCompleted] = ToHex( ParseColor( stepsOptions.StepsItemTextCompleted ) );
                }

                if ( !string.IsNullOrEmpty( stepsOptions.StepsItemTextActive ) )
                {
                    variables[ThemeVariables.StepsItemTextActive] = ToHex( ParseColor( stepsOptions.StepsItemTextActive ) );
                }
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

            GenerateStepsStyles( sb, theme, theme.StepsOptions );
        }

        protected virtual void GenerateBreakpointStyles( StringBuilder sb, Theme theme, string breakpoint, string value )
        {
            if ( string.IsNullOrEmpty( value ) )
                return;

            // mobile is configured diferently from other breakpoints
            if ( breakpoint != "mobile" )
            {
                sb.Append( $"@media (min-width: {value})" ).Append( "{" )
                    .Append( $"body:before" ).Append( "{" )
                        .Append( $"content: \"{breakpoint}\";" ).Append( "}" )
                    .AppendLine( "}" );
            }
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
            GenerateSwitchVariantStyles( sb, theme, variant, color, theme.SwitchOptions );
            GenerateStepsVariantStyles( sb, theme, variant, color, theme.StepsOptions );
            GenerateProgressVariantStyles( sb, theme, variant, color, theme.ProgressOptions );

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

        protected abstract void GenerateSwitchVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, ThemeSwitchOptions switchOptions );

        protected abstract void GenerateStepsVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, ThemeStepsOptions stepsOptions );

        protected virtual void GenerateProgressVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, ThemeProgressOptions progressOptions )
        {
            sb
                .Append( $".b-page-progress .b-page-progress-indicator.b-page-progress-indicator-{variant}" ).Append( "{" )
                .Append( $"background-color: {Var( ThemeVariables.VariantPageProgressIndicator( variant ) )};" )
                .AppendLine( "}" );
        }

        protected abstract void GenerateAlertVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, string inBorderColor, string inColor, ThemeAlertOptions options );

        protected abstract void GenerateTableVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, string inBorderColor );

        protected abstract void GenerateCardStyles( StringBuilder sb, Theme theme, ThemeCardOptions options );

        protected abstract void GenerateModalStyles( StringBuilder sb, Theme theme, ThemeModalOptions options );

        protected abstract void GenerateTabsStyles( StringBuilder sb, Theme theme, ThemeTabsOptions options );

        protected virtual void GenerateProgressStyles( StringBuilder sb, Theme theme, ThemeProgressOptions options )
        {
            if ( !string.IsNullOrEmpty( options?.PageProgressDefaultColor ) )
            {
                sb
                    .Append( $".b-page-progress .b-page-progress-indicator" ).Append( "{" )
                    .Append( $"background-color: {options.PageProgressDefaultColor};" )
                    .AppendLine( "}" );
            }
        }

        protected abstract void GenerateAlertStyles( StringBuilder sb, Theme theme, ThemeAlertOptions options );

        protected abstract void GenerateBreadcrumbStyles( StringBuilder sb, Theme theme, ThemeBreadcrumbOptions options );

        protected abstract void GenerateBadgeStyles( StringBuilder sb, Theme theme, ThemeBadgeOptions options );

        protected abstract void GeneratePaginationStyles( StringBuilder sb, Theme theme, ThemePaginationOptions options );

        protected abstract void GenerateBarStyles( StringBuilder sb, Theme theme, ThemeBarOptions options );

        protected abstract void GenerateStepsStyles( StringBuilder sb, Theme theme, ThemeStepsOptions stepsOptions );

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
            if ( value.StartsWith( '#' ) )
                return HexStringToColor( value );
            else if ( value.StartsWith( "rgb" ) )
                return CssRgbaFunctionToColor( value );

            return System.Drawing.Color.FromName( value );
        }

        protected static System.Drawing.Color Rgba2Rgb( System.Drawing.Color background, System.Drawing.Color color, float? customAlpha = null )
        {
            var alpha = customAlpha ?? color.A / byte.MaxValue;

            return System.Drawing.Color.FromArgb(
                (int)( ( 1 - alpha ) * background.R + alpha * color.R ),
                (int)( ( 1 - alpha ) * background.G + alpha * color.G ),
                (int)( ( 1 - alpha ) * background.B + alpha * color.B )
            );
        }

        protected static System.Drawing.Color HexStringToColor( string hexColor )
        {
            var hc = ExtractHexDigits( hexColor );

            if ( hc.Length < 6 )
                return System.Drawing.Color.Empty;

            try
            {
                var r = int.Parse( hc.Substring( 0, 2 ), NumberStyles.HexNumber );
                var g = int.Parse( hc.Substring( 2, 2 ), NumberStyles.HexNumber );
                var b = int.Parse( hc.Substring( 4, 2 ), NumberStyles.HexNumber );

                if ( hc.Length == 8 )
                {
                    var a = int.Parse( hc.Substring( 6, 2 ), NumberStyles.HexNumber );

                    return System.Drawing.Color.FromArgb( a, r, g, b );
                }

                return System.Drawing.Color.FromArgb( r, g, b );
            }
            catch
            {
                return System.Drawing.Color.Empty;
            }
        }

        protected static System.Drawing.Color CssRgbaFunctionToColor( string cssColor )
        {
            int left = cssColor.IndexOf( '(' );
            int right = cssColor.IndexOf( ')' );

            if ( 0 > left || 0 > right )
                throw new FormatException( $"Invalid rgb or rgba function format: {cssColor}" );

            var noBrackets = cssColor.Substring( left + 1, right - left - 1 );

            var parts = noBrackets.Split( ',' );

            if ( parts.Length < 3 )
                throw new FormatException( $"Invalid rgb format: {cssColor}" );

            var r = int.Parse( parts[0], CultureInfo.InvariantCulture );
            var g = int.Parse( parts[1], CultureInfo.InvariantCulture );
            var b = int.Parse( parts[2], CultureInfo.InvariantCulture );

            if ( 3 == parts.Length )
            {
                return System.Drawing.Color.FromArgb( r, g, b );
            }
            else if ( 4 == parts.Length )
            {
                var a = float.Parse( parts[3], CultureInfo.InvariantCulture );

                return System.Drawing.Color.FromArgb( (int)( a * 255 ), r, g, b );
            }

            return System.Drawing.Color.Empty;
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
            if ( color.A < 255 )
                return $"#{color.R:X2}{color.G:X2}{color.B:X2}{color.A:X2}";

            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        protected static string ToHexRGBA( System.Drawing.Color color )
        {
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}{color.A:X2}";
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

        protected static System.Drawing.Color Contrast( Theme theme, string hexColor )
        {
            var color = ParseColor( hexColor );

            return Contrast( theme, color );
        }

        protected static System.Drawing.Color Contrast( Theme theme, System.Drawing.Color color, byte? luminanceThreshold = null )
        {
            // Counting the perceptive luminance - human eye favors green color... 
            double luminance = ( 299 * color.R + 587 * color.G + 114 * color.B ) / 1000d;

            System.Drawing.Color contrast;

            // The yiq lightness value that determines when the lightness of color changes from "dark" to "light". Acceptable values are between 0 and 255.
            if ( luminance > ( luminanceThreshold ?? theme.LuminanceThreshold ) )
                contrast = ParseColor( theme.Black ); // bright colors - black font
            else
                contrast = ParseColor( theme.White ); // dark colors - white font

            return contrast;
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
