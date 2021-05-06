﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Default implementation of <see cref="IThemeGenerator"/>.
    /// </summary>
    public abstract class ThemeGenerator : IThemeGenerator
    {
        #region Constructors

        /// <summary>
        /// A default <see cref="ThemeGenerator"/> constructor.
        /// </summary>
        /// <param name="themeCache">Cache used to save build results.</param>
        public ThemeGenerator( IThemeCache themeCache )
        {
            ThemeCache = themeCache;
        }

        #endregion

        #region Methods

        #region Variables

        /// <inheritdoc/>
        public virtual string GenerateVariables( Theme theme )
        {
            if ( ThemeCache.TryGetVariablesFromCache( theme, out var cachedVariables ) )
            {
                return cachedVariables;
            }

            var sb = new StringBuilder();

            if ( !string.IsNullOrEmpty( theme.White ) )
                Variables[ThemeVariables.White] = theme.White;

            if ( !string.IsNullOrEmpty( theme.Black ) )
                Variables[ThemeVariables.Black] = theme.Black;

            Variables[ThemeVariables.BorderRadius] = ".25rem";
            Variables[ThemeVariables.BorderRadiusLarge] = ".3rem";
            Variables[ThemeVariables.BorderRadiusSmall] = ".2rem";

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

            GenerateSpinKitVariables( theme, theme.SpinKitOptions );

            // apply variables
            foreach ( var kv in Variables )
                sb.AppendLine( $"{kv.Key}: {kv.Value};" );

            var generatedVariables = sb.ToString();

            ThemeCache.CacheVariables( theme, generatedVariables );

            return generatedVariables;
        }

        /// <summary>
        /// Generates the breakpoint CSS variables.
        /// </summary>
        /// <param name="theme">Currently used theme options.</param>
        /// <param name="name">Name of the breakpoint.</param>
        /// <param name="size">Size of the breakpoint.</param>
        protected virtual void GenerateBreakpointVariables( Theme theme, string name, string size )
        {
            Variables[ThemeVariables.Breakpoint( name )] = size;
        }

        /// <summary>
        /// Generates the color CSS variables.
        /// </summary>
        /// <param name="theme">Currently used theme options.</param>
        /// <param name="variant">Color variant name.</param>
        /// <param name="value">Color value.</param>
        protected virtual void GenerateColorVariables( Theme theme, string variant, string value )
        {
            Variables[ThemeVariables.Color( variant )] = value;

            GenerateButtonColorVariables( theme, variant, value, value, theme.ButtonOptions );
            GenerateOutlineButtonColorVariables( theme, variant, value, theme.ButtonOptions );
            GenerateSnackbarColorVariables( theme, variant, value, theme.SnackbarOptions );
            GenerateStepsColorVariables( theme, variant, value, theme.StepsOptions );
            GenerateProgressColorVariables( theme, variant, value, theme.ProgressOptions );
            GenerateRatingColorVariables( theme, variant, value, theme.RatingOptions );
        }

        /// <summary>
        /// Generates the button color CSS variables.
        /// </summary>
        /// <param name="theme">Currently used theme options.</param>
        /// <param name="variant">Color variant name.</param>
        /// <param name="inBackgroundColor">Button background color.</param>
        /// <param name="inBorderColor">Button border color.</param>
        /// <param name="options">Button options.</param>
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

            Variables[ThemeVariables.ButtonBackground( variant )] = background;
            Variables[ThemeVariables.ButtonBorder( variant )] = border;
            Variables[ThemeVariables.ButtonHoverBackground( variant )] = hoverBackground;
            Variables[ThemeVariables.ButtonHoverBorder( variant )] = hoverBorder;
            Variables[ThemeVariables.ButtonActiveBackground( variant )] = activeBackground;
            Variables[ThemeVariables.ButtonActiveBorder( variant )] = activeBorder;
            Variables[ThemeVariables.ButtonYiqBackground( variant )] = yiqBackground;
            Variables[ThemeVariables.ButtonYiqHoverBackground( variant )] = yiqHoverBackground;
            Variables[ThemeVariables.ButtonYiqActiveBackground( variant )] = yiqActiveBackground;
            Variables[ThemeVariables.ButtonBoxShadow( variant )] = boxShadow;
        }

        /// <summary>
        /// Generates the outline button color CSS variables.
        /// </summary>
        /// <param name="theme">Currently used theme options.</param>
        /// <param name="variant">Color variant name.</param>
        /// <param name="inBorderColor">Button border color.</param>
        /// <param name="options">Button options.</param>
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

            Variables[ThemeVariables.OutlineButtonColor( variant )] = color;
            Variables[ThemeVariables.OutlineButtonYiqColor( variant )] = yiqColor;
            Variables[ThemeVariables.OutlineButtonBoxShadowColor( variant )] = boxShadow;
            Variables[ThemeVariables.OutlineButtonHoverColor( variant )] = hoverColor;
            Variables[ThemeVariables.OutlineButtonActiveColor( variant )] = activeColor;
        }

        /// <summary>
        /// Generates the snackbar color CSS variables.
        /// </summary>
        /// <param name="theme">Currently used theme options.</param>
        /// <param name="variant">Color variant name.</param>
        /// <param name="inColor">Snackbar color.</param>
        /// <param name="options">Snackbar options.</param>
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

            Variables[$"{ThemeVariables.SnackbarBackground}-{ variant }"] = ToHex( backgroundColor );
            Variables[$"{ThemeVariables.SnackbarTextColor}-{ variant }"] = ToHex( textColor );
            Variables[$"{ThemeVariables.SnackbarButtonColor}-{ variant }"] = ToHex( buttonColor );
            Variables[$"{ThemeVariables.SnackbarButtonHoverColor}-{ variant }"] = ToHex( buttonHoverColor );
        }

        /// <summary>
        /// Generates the steps color CSS variables.
        /// </summary>
        /// <param name="theme">Currently used theme options.</param>
        /// <param name="variant">Color variant name.</param>
        /// <param name="inColor">Steps color.</param>
        /// <param name="options">Steps options.</param>
        protected virtual void GenerateStepsColorVariables( Theme theme, string variant, string inColor, ThemeStepsOptions options )
        {
            var argbColor = ParseColor( inColor );

            if ( argbColor.IsEmpty )
                return;

            var color = ToHex( argbColor );

            Variables[ThemeVariables.VariantStepsItemIcon( variant )] = color;
            Variables[ThemeVariables.VariantStepsItemIconYiq( variant )] = ToHex( Contrast( theme, color ) );
            Variables[ThemeVariables.VariantStepsItemText( variant )] = color;
        }

        /// <summary>
        /// Generates the progress-bar color CSS variables.
        /// </summary>
        /// <param name="theme">Currently used theme options.</param>
        /// <param name="variant">Color variant name.</param>
        /// <param name="inColor">Progress-bar color.</param>
        /// <param name="options">Progress-bar options.</param>
        protected virtual void GenerateProgressColorVariables( Theme theme, string variant, string inColor, ThemeProgressOptions options )
        {
            var inArgbColor = ParseColor( inColor );

            if ( inArgbColor.IsEmpty )
                return;

            var color = ToHex( inArgbColor );

            Variables[ThemeVariables.VariantPageProgressIndicator( variant )] = color;
        }

        /// <summary>
        /// Generates the rating color CSS variables.
        /// </summary>
        /// <param name="theme">Currently used theme options.</param>
        /// <param name="variant">Color variant name.</param>
        /// <param name="inColor">Rating color.</param>
        /// <param name="options">Rating options.</param>
        protected virtual void GenerateRatingColorVariables( Theme theme, string variant, string inColor, ThemeRatingOptions options )
        {
            var inArgbColor = ParseColor( inColor );

            if ( inArgbColor.IsEmpty )
                return;

            var color = ToHex( inArgbColor );

            Variables[ThemeVariables.VariantRatingColor( variant )] = color;
        }

        /// <summary>
        /// Generates the background CSS variables.
        /// </summary>
        /// <param name="theme">Currently used theme options.</param>
        /// <param name="variant">Background variant name.</param>
        /// <param name="inColor">Background color.</param>
        protected virtual void GenerateBackgroundVariables( Theme theme, string variant, string inColor )
        {
            var backgroundColor = ParseColor( inColor );

            if ( backgroundColor.IsEmpty )
                return;

            var backgroundYiqColor = Contrast( theme, backgroundColor );

            Variables[ThemeVariables.BackgroundColor( variant )] = ToHex( backgroundColor );
            Variables[ThemeVariables.BackgroundYiqColor( variant )] = ToHex( backgroundYiqColor );
        }

        /// <summary>
        /// Generates the text CSS variables.
        /// </summary>
        /// <param name="theme">Currently used theme options.</param>
        /// <param name="variant">Text variant name.</param>
        /// <param name="inColor">Text color.</param>
        protected virtual void GenerateTextColorVariables( Theme theme, string variant, string inColor )
        {
            var color = ParseColor( inColor );

            if ( color.IsEmpty )
                return;

            Variables[ThemeVariables.TextColor( variant )] = ToHex( color );
        }

        /// <summary>
        /// Generates the text CSS variables.
        /// </summary>
        /// <param name="theme">Currently used theme options.</param>
        /// <param name="sidebarOptions">Sidebar options.</param>
        protected virtual void GenerateSidebarVariables( Theme theme, ThemeSidebarOptions sidebarOptions )
        {
            if ( sidebarOptions.Width != null )
                Variables[ThemeVariables.SidebarWidth] = sidebarOptions.Width;

            if ( sidebarOptions.BackgroundColor != null )
                Variables[ThemeVariables.SidebarBackground] = ToHex( ParseColor( sidebarOptions.BackgroundColor ) );

            if ( sidebarOptions.Color != null )
                Variables[ThemeVariables.SidebarColor] = ToHex( ParseColor( sidebarOptions.Color ) );
        }

        /// <summary>
        /// Generates the bar component CSS variables.
        /// </summary>
        /// <param name="barOptions">Bar options.</param>
        protected virtual void GenerateBarVariables( ThemeBarOptions barOptions )
        {
            if ( !string.IsNullOrEmpty( barOptions.VerticalWidth ) )
                Variables[ThemeVariables.VerticalBarWidth] = barOptions.VerticalWidth;

            if ( !string.IsNullOrEmpty( barOptions.VerticalSmallWidth ) )
                Variables[ThemeVariables.VerticalBarSmallWidth] = barOptions.VerticalSmallWidth;

            if ( !string.IsNullOrEmpty( barOptions.VerticalBrandHeight ) )
                Variables[ThemeVariables.VerticalBarBrandHeight] = barOptions.VerticalBrandHeight;

            if ( !string.IsNullOrEmpty( barOptions.VerticalPopoutMenuWidth ) )
                Variables[ThemeVariables.VerticalPopoutMenuWidth] = barOptions.VerticalPopoutMenuWidth;

            if ( !string.IsNullOrEmpty( barOptions.HorizontalHeight ) )
                Variables[ThemeVariables.HorizontalBarHeight] = barOptions.HorizontalHeight;

            if ( barOptions?.DarkColors != null )
            {
                if ( !string.IsNullOrEmpty( barOptions.DarkColors.BackgroundColor ) )
                    Variables[ThemeVariables.BarDarkBackground] = ToHex( ParseColor( barOptions.DarkColors.BackgroundColor ) );

                if ( !string.IsNullOrEmpty( barOptions.DarkColors.Color ) )
                    Variables[ThemeVariables.BarDarkColor] = ToHex( ParseColor( barOptions.DarkColors.Color ) );

                if ( barOptions.DarkColors.ItemColorOptions != null )
                {
                    if ( !string.IsNullOrEmpty( barOptions.DarkColors.ItemColorOptions.ActiveBackgroundColor ) )
                        Variables[ThemeVariables.BarItemDarkActiveBackground] = ToHex( ParseColor( barOptions.DarkColors.ItemColorOptions.ActiveBackgroundColor ) );

                    if ( !string.IsNullOrEmpty( barOptions.DarkColors.ItemColorOptions.ActiveColor ) )
                        Variables[ThemeVariables.BarItemDarkActiveColor] = ToHex( ParseColor( barOptions.DarkColors.ItemColorOptions.ActiveColor ) );

                    if ( !string.IsNullOrEmpty( barOptions.DarkColors.ItemColorOptions.HoverBackgroundColor ) )
                        Variables[ThemeVariables.BarItemDarkHoverBackground] = ToHex( ParseColor( barOptions.DarkColors.ItemColorOptions.HoverBackgroundColor ) );

                    if ( !string.IsNullOrEmpty( barOptions.DarkColors.ItemColorOptions.HoverColor ) )
                        Variables[ThemeVariables.BarItemDarkHoverColor] = ToHex( ParseColor( barOptions.DarkColors.ItemColorOptions.HoverColor ) );
                }

                if ( !string.IsNullOrEmpty( barOptions.DarkColors.DropdownColorOptions?.BackgroundColor ) )
                    Variables[ThemeVariables.BarDropdownDarkBackground] = ToHex( ParseColor( barOptions.DarkColors.DropdownColorOptions.BackgroundColor ) );

                if ( !string.IsNullOrEmpty( barOptions.DarkColors.BrandColorOptions?.BackgroundColor ) )
                    Variables[ThemeVariables.BarBrandDarkBackground] = ToHex( ParseColor( barOptions.DarkColors.BrandColorOptions.BackgroundColor ) );
            }

            if ( barOptions?.LightColors != null )
            {
                if ( !string.IsNullOrEmpty( barOptions.LightColors.BackgroundColor ) )
                    Variables[ThemeVariables.BarLightBackground] = ToHex( ParseColor( barOptions.LightColors.BackgroundColor ) );

                if ( !string.IsNullOrEmpty( barOptions.LightColors.Color ) )
                    Variables[ThemeVariables.BarLightColor] = ToHex( ParseColor( barOptions.LightColors.Color ) );

                if ( barOptions.LightColors.ItemColorOptions != null )
                {
                    if ( !string.IsNullOrEmpty( barOptions.LightColors.ItemColorOptions.ActiveBackgroundColor ) )
                        Variables[ThemeVariables.BarItemLightActiveBackground] = ToHex( ParseColor( barOptions.LightColors.ItemColorOptions.ActiveBackgroundColor ) );

                    if ( !string.IsNullOrEmpty( barOptions.LightColors.ItemColorOptions.ActiveColor ) )
                        Variables[ThemeVariables.BarItemLightActiveColor] = ToHex( ParseColor( barOptions.LightColors.ItemColorOptions.ActiveColor ) );

                    if ( !string.IsNullOrEmpty( barOptions.LightColors.ItemColorOptions.HoverBackgroundColor ) )
                        Variables[ThemeVariables.BarItemLightHoverBackground] = ToHex( ParseColor( barOptions.LightColors.ItemColorOptions.HoverBackgroundColor ) );

                    if ( !string.IsNullOrEmpty( barOptions.LightColors.ItemColorOptions.HoverColor ) )
                        Variables[ThemeVariables.BarItemLightHoverColor] = ToHex( ParseColor( barOptions.LightColors.ItemColorOptions.HoverColor ) );
                }

                if ( !string.IsNullOrEmpty( barOptions.LightColors.DropdownColorOptions?.BackgroundColor ) )
                    Variables[ThemeVariables.BarDropdownLightBackground] = ToHex( ParseColor( barOptions.LightColors.DropdownColorOptions.BackgroundColor ) );

                if ( !string.IsNullOrEmpty( barOptions.LightColors.BrandColorOptions?.BackgroundColor ) )
                    Variables[ThemeVariables.BarBrandLightBackground] = ToHex( ParseColor( barOptions.LightColors.BrandColorOptions.BackgroundColor ) );
            }
        }

        /// <summary>
        /// Generates the snackbar CSS variables.
        /// </summary>
        /// <param name="theme">Currently used theme options.</param>
        /// <param name="snackbarOptions">Snackbar options</param>
        protected virtual void GenerateSnackbarVariables( Theme theme, ThemeSnackbarOptions snackbarOptions )
        {
            if ( snackbarOptions?.BackgroundColor != null )
                Variables[ThemeVariables.SnackbarBackground] = ToHex( ParseColor( snackbarOptions.BackgroundColor ) );

            if ( snackbarOptions?.TextColor != null )
                Variables[ThemeVariables.SnackbarTextColor] = ToHex( ParseColor( snackbarOptions.TextColor ) );

            if ( snackbarOptions?.ButtonColor != null )
                Variables[ThemeVariables.SnackbarButtonColor] = ToHex( ParseColor( snackbarOptions.ButtonColor ) );

            if ( snackbarOptions?.ButtonHoverColor != null )
                Variables[ThemeVariables.SnackbarButtonHoverColor] = ToHex( ParseColor( snackbarOptions.ButtonHoverColor ) );
        }

        /// <summary>
        /// Generates the divider CSS variables.
        /// </summary>
        /// <param name="theme">Currently used theme options.</param>
        /// <param name="dividerOptions">Divider options</param>
        protected virtual void GenerateDividerVariables( Theme theme, ThemeDividerOptions dividerOptions )
        {
            if ( dividerOptions.Color != null )
                Variables[ThemeVariables.DividerColor] = ToHex( ParseColor( dividerOptions.Color ) );

            if ( dividerOptions.Color != null )
                Variables[ThemeVariables.DividerThickness] = dividerOptions.Thickness;

            if ( dividerOptions.Color != null )
                Variables[ThemeVariables.DividerTextSize] = dividerOptions.TextSize;
        }

        /// <summary>
        /// Generates the tooltip CSS variables.
        /// </summary>
        /// <param name="theme">Currently used theme options.</param>
        /// <param name="tooltipOptions">Tooltip options</param>
        protected virtual void GenerateTooltipVariables( Theme theme, ThemeTooltipOptions tooltipOptions )
        {
            if ( tooltipOptions?.BackgroundColor != null )
            {
                var backgroundColor = ParseColor( tooltipOptions.BackgroundColor );

                Variables[ThemeVariables.TooltipBackgroundColorR] = backgroundColor.R.ToString( CultureInfo.InvariantCulture );
                Variables[ThemeVariables.TooltipBackgroundColorG] = backgroundColor.G.ToString( CultureInfo.InvariantCulture );
                Variables[ThemeVariables.TooltipBackgroundColorB] = backgroundColor.B.ToString( CultureInfo.InvariantCulture );
                Variables[ThemeVariables.TooltipBackgroundOpacity] = ( backgroundColor.A / 255f ).ToString( "n2", CultureInfo.InvariantCulture );
            }

            if ( tooltipOptions?.Color != null )
            {
                Variables[ThemeVariables.TooltipColor] = tooltipOptions.Color;
            }

            if ( tooltipOptions?.FontSize != null )
            {
                Variables[ThemeVariables.TooltipFontSize] = tooltipOptions.FontSize;
            }

            Variables[ThemeVariables.TooltipBorderRadius] = GetBorderRadius( theme, tooltipOptions?.BorderRadius, Var( ThemeVariables.BorderRadius ) );

            if ( tooltipOptions?.FadeTime != null )
            {
                Variables[ThemeVariables.TooltipFadeTime] = tooltipOptions.FadeTime;
            }

            if ( tooltipOptions?.MaxWidth != null )
            {
                Variables[ThemeVariables.TooltipMaxWidth] = tooltipOptions.MaxWidth;
            }

            if ( tooltipOptions?.Padding != null )
            {
                Variables[ThemeVariables.TooltipPadding] = tooltipOptions.Padding;
            }

            if ( tooltipOptions?.ZIndex != null )
            {
                Variables[ThemeVariables.TooltipZIndex] = tooltipOptions.ZIndex;
            }
        }

        /// <summary>
        /// Generates the breadcrumb CSS variables.
        /// </summary>
        /// <param name="theme">Currently used theme options.</param>
        /// <param name="breadcrumbOptions">Breadcrumb options.</param>
        protected virtual void GenerateBreadcrumbVariables( Theme theme, ThemeBreadcrumbOptions breadcrumbOptions )
        {
            if ( FirstNotEmpty( out var color, breadcrumbOptions?.Color, theme.ColorOptions?.Primary ) )
            {
                Variables[ThemeVariables.BreadcrumbColor] = color;
            }
        }

        /// <summary>
        /// Generates the steps CSS variables.
        /// </summary>
        /// <param name="theme">Currently used theme options.</param>
        /// <param name="stepsOptions">Steps options.</param>
        protected virtual void GenerateStepsVariables( Theme theme, ThemeStepsOptions stepsOptions )
        {
            if ( stepsOptions != null )
            {
                if ( !string.IsNullOrEmpty( stepsOptions.StepsItemIconColor ) )
                {
                    Variables[ThemeVariables.StepsItemIcon] = ToHex( ParseColor( stepsOptions.StepsItemIconColor ) );
                }

                if ( !string.IsNullOrEmpty( stepsOptions.StepsItemIconCompleted ) )
                {
                    Variables[ThemeVariables.StepsItemIconCompleted] = ToHex( ParseColor( stepsOptions.StepsItemIconCompleted ) );
                }

                if ( !string.IsNullOrEmpty( stepsOptions.StepsItemIconCompletedYiq ) )
                {
                    Variables[ThemeVariables.StepsItemIconCompletedYiq] = ToHex( ParseColor( stepsOptions.StepsItemIconCompletedYiq ) );
                }

                if ( !string.IsNullOrEmpty( stepsOptions.StepsItemIconActive ) )
                {
                    Variables[ThemeVariables.StepsItemIconActive] = ToHex( ParseColor( stepsOptions.StepsItemIconActive ) );
                }

                if ( !string.IsNullOrEmpty( stepsOptions.StepsItemIconActiveYiq ) )
                {
                    Variables[ThemeVariables.StepsItemIconActiveYiq] = ToHex( ParseColor( stepsOptions.StepsItemIconActiveYiq ) );
                }

                if ( !string.IsNullOrEmpty( stepsOptions.StepsItemTextColor ) )
                {
                    Variables[ThemeVariables.StepsItemText] = ToHex( ParseColor( stepsOptions.StepsItemTextColor ) );
                }

                if ( !string.IsNullOrEmpty( stepsOptions.StepsItemTextCompleted ) )
                {
                    Variables[ThemeVariables.StepsItemTextCompleted] = ToHex( ParseColor( stepsOptions.StepsItemTextCompleted ) );
                }

                if ( !string.IsNullOrEmpty( stepsOptions.StepsItemTextActive ) )
                {
                    Variables[ThemeVariables.StepsItemTextActive] = ToHex( ParseColor( stepsOptions.StepsItemTextActive ) );
                }
            }
        }

        /// <summary>
        /// Generates the spinkit CSS variables.
        /// </summary>
        /// <param name="theme">Currently used theme options.</param>
        /// <param name="spinKitOptions">SpinKit options.</param>
        protected virtual void GenerateSpinKitVariables( Theme theme, ThemeSpinKitOptions spinKitOptions )
        {
            if ( !string.IsNullOrEmpty( spinKitOptions?.Color ) )
            {
                Variables[ThemeVariables.SpinKitColor] = ToHex( ParseColor( spinKitOptions.Color ) );
            }

            if ( !string.IsNullOrEmpty( spinKitOptions?.Size ) )
            {
                Variables[ThemeVariables.SpinKitSize] = spinKitOptions.Size;
            }
        }

        /// <summary>
        /// Gets the variable value.
        /// </summary>
        /// <param name="name">Variable name.</param>
        /// <param name="defaultValue">Fallback value if variable is not found.</param>
        /// <returns>Variable value.</returns>
        protected string Var( string name, string defaultValue = null )
        {
            if ( Variables.TryGetValue( name, out var value ) )
                return value;

            return defaultValue;
        }

        #endregion

        #region Styles

        /// <inheritdoc/>
        public virtual string GenerateStyles( Theme theme )
        {
            if ( ThemeCache.TryGetStylesFromCache( theme, out var cachedStyle ) )
            {
                return cachedStyle;
            }

            var sb = new StringBuilder();

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

            GenerateRatingStyles( sb, theme, theme.RatingOptions );

            var generatedStyles = sb.ToString();

            ThemeCache.CacheStyles( theme, generatedStyles );

            return generatedStyles;
        }

        /// <summary>
        /// Generates the breakpoint styles.
        /// </summary>
        /// <param name="sb">Result of the generator.</param>
        /// <param name="theme">Currently used theme options.</param>
        /// <param name="breakpoint">Breakpoint options.</param>
        /// <param name="value">Breakpoint size.</param>
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
        /// Generates color styles that are based on the variant names.
        /// </summary>
        /// <param name="sb">Target string builder.</param>
        /// <param name="theme">Theme settings.</param>
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
            GenerateRatingVariantStyles( sb, theme, variant, color, theme.RatingOptions );

            GenerateAlertVariantStyles( sb, theme, variant,
                ThemeColorLevelHex( theme, color, theme.AlertOptions?.BackgroundLevel ?? -10 ),
                ThemeColorLevelHex( theme, color, theme.AlertOptions?.BorderLevel ?? -7 ),
                ThemeColorLevelHex( theme, color, theme.AlertOptions?.ColorLevel ?? 6 ),
                theme.AlertOptions );

            GenerateTableVariantStyles( sb, theme, variant,
                ThemeColorLevelHex( theme, color, theme.TableOptions?.BackgroundLevel ?? -9 ),
                ThemeColorLevelHex( theme, color, theme.TableOptions?.BorderLevel ?? -6 ) );
        }

        /// <summary>
        /// Generates the background styles that are based on the variant names.
        /// </summary>
        /// <param name="sb">Target string builder.</param>
        /// <param name="theme">Theme settings.</param>
        /// <param name="variant">Variant name.</param>
        /// <param name="color">Color value.</param>
        protected virtual void GenerateBackgroundStyles( StringBuilder sb, Theme theme, string variant, string color )
        {
            GenerateBackgroundVariantStyles( sb, theme, variant );
            GenerateBorderVariantStyles( sb, theme, variant );
        }

        /// <summary>
        /// Generates the text styles that are based on the variant names.
        /// </summary>
        /// <param name="sb">Target string builder.</param>
        /// <param name="theme">Theme settings.</param>
        /// <param name="variant">Variant name.</param>
        /// <param name="color">Color value.</param>
        protected virtual void GenerateTypographyVariantStyles( StringBuilder sb, Theme theme, string variant, string color )
        {
            GenerateParagraphVariantStyles( sb, theme, variant, color );
            GenerateInputVariantStyles( sb, theme, variant, color );
        }

        /// <summary>
        /// Generates the background styles that are based on the variant names.
        /// </summary>
        /// <param name="sb">Target string builder.</param>
        /// <param name="theme">Theme settings.</param>
        /// <param name="variant">Variant name.</param>
        protected abstract void GenerateBackgroundVariantStyles( StringBuilder sb, Theme theme, string variant );

        /// <summary>
        /// Generates the border styles that are based on the variant names.
        /// </summary>
        /// <param name="sb">Target string builder.</param>
        /// <param name="theme">Theme settings.</param>
        /// <param name="variant">Variant name.</param>
        protected abstract void GenerateBorderVariantStyles( StringBuilder sb, Theme theme, string variant );

        /// <summary>
        /// Generates the button styles that are based on the variant names.
        /// </summary>
        /// <param name="sb">Target string builder.</param>
        /// <param name="theme">Theme settings.</param>
        /// <param name="variant">Variant name.</param>
        /// <param name="options">Button options.</param>
        protected abstract void GenerateButtonVariantStyles( StringBuilder sb, Theme theme, string variant, ThemeButtonOptions options );

        /// <summary>
        /// Generates the outline button styles that are based on the variant names.
        /// </summary>
        /// <param name="sb">Target string builder.</param>
        /// <param name="theme">Theme settings.</param>
        /// <param name="variant">Variant name.</param>
        /// <param name="options">Button options.</param>
        protected abstract void GenerateButtonOutlineVariantStyles( StringBuilder sb, Theme theme, string variant, ThemeButtonOptions options );

        /// <summary>
        /// Generates the button styles.
        /// </summary>
        /// <param name="sb">Target string builder.</param>
        /// <param name="theme">Theme settings.</param>
        /// <param name="options">Button options.</param>
        protected abstract void GenerateButtonStyles( StringBuilder sb, Theme theme, ThemeButtonOptions options );

        /// <summary>
        /// Generates the dropdown styles.
        /// </summary>
        /// <param name="sb">Target string builder.</param>
        /// <param name="theme">Theme settings.</param>
        /// <param name="options">Dropdown options.</param>
        protected abstract void GenerateDropdownStyles( StringBuilder sb, Theme theme, ThemeDropdownOptions options );

        /// <summary>
        /// Generates the input element styles.
        /// </summary>
        /// <param name="sb">Target string builder.</param>
        /// <param name="theme">Theme settings.</param>
        /// <param name="options">Input options.</param>
        protected abstract void GenerateInputStyles( StringBuilder sb, Theme theme, ThemeInputOptions options );

        /// <summary>
        /// Generates the badge styles.
        /// </summary>
        /// <param name="sb">Target string builder.</param>
        /// <param name="theme">Theme settings.</param>
        /// <param name="variant">Badge variant name.</param>
        /// <param name="inBackgroundColor">Badge color value.</param>
        protected abstract void GenerateBadgeVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor );

        /// <summary>
        /// Generates the switch styles.
        /// </summary>
        /// <param name="sb">Target string builder.</param>
        /// <param name="theme">Theme settings.</param>
        /// <param name="variant">Switch variant name.</param>
        /// <param name="inBackgroundColor">Switch color value.</param>
        /// <param name="switchOptions">Switch options.</param>
        protected abstract void GenerateSwitchVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, ThemeSwitchOptions switchOptions );

        /// <summary>
        /// Generates the steps styles.
        /// </summary>
        /// <param name="sb">Target string builder.</param>
        /// <param name="theme">Theme settings.</param>
        /// <param name="variant">Steps variant name.</param>
        /// <param name="inBackgroundColor">Steps color value.</param>
        /// <param name="stepsOptions">Steps options.</param>
        protected abstract void GenerateStepsVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, ThemeStepsOptions stepsOptions );

        /// <summary>
        /// Generates the progress styles.
        /// </summary>
        /// <param name="sb">Target string builder.</param>
        /// <param name="theme">Theme settings.</param>
        /// <param name="variant">Progress variant name.</param>
        /// <param name="inBackgroundColor">Progress color value.</param>
        /// <param name="progressOptions">Progress options.</param>
        protected virtual void GenerateProgressVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, ThemeProgressOptions progressOptions )
        {
            sb
                .Append( $".b-page-progress .b-page-progress-indicator.b-page-progress-indicator-{variant}" ).Append( "{" )
                .Append( $"background-color: {Var( ThemeVariables.VariantPageProgressIndicator( variant ) )};" )
                .AppendLine( "}" );
        }

        /// <summary>
        /// Generates the rating styles.
        /// </summary>
        /// <param name="sb">Target string builder.</param>
        /// <param name="theme">Theme settings.</param>
        /// <param name="variant">Rating variant name.</param>
        /// <param name="inBackgroundColor">Rating color value.</param>
        /// <param name="ratingOptions">Rating options.</param>
        protected abstract void GenerateRatingVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, ThemeRatingOptions ratingOptions );

        /// <summary>
        /// Generates the alert styles.
        /// </summary>
        /// <param name="sb">Target string builder.</param>
        /// <param name="theme">Theme settings.</param>
        /// <param name="variant">Alert variant name.</param>
        /// <param name="inBackgroundColor">Alert background value.</param>
        /// <param name="inBorderColor">Alert border value.</param>
        /// <param name="inColor">Alert text color value.</param>
        /// <param name="options">Alert options.</param>
        protected abstract void GenerateAlertVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, string inBorderColor, string inColor, ThemeAlertOptions options );

        /// <summary>
        /// Generates the table styles.
        /// </summary>
        /// <param name="sb">Target string builder.</param>
        /// <param name="theme">Theme settings.</param>
        /// <param name="variant">Table variant name.</param>
        /// <param name="inBackgroundColor">Table background value.</param>
        /// <param name="inBorderColor">Table border value.</param>
        protected abstract void GenerateTableVariantStyles( StringBuilder sb, Theme theme, string variant, string inBackgroundColor, string inBorderColor );

        /// <summary>
        /// Generates the card styles.
        /// </summary>
        /// <param name="sb">Target string builder.</param>
        /// <param name="theme">Theme settings.</param>
        /// <param name="options">Card options.</param>
        protected abstract void GenerateCardStyles( StringBuilder sb, Theme theme, ThemeCardOptions options );

        /// <summary>
        /// Generates the modal styles.
        /// </summary>
        /// <param name="sb">Target string builder.</param>
        /// <param name="theme">Theme settings.</param>
        /// <param name="options">Modal options.</param>
        protected abstract void GenerateModalStyles( StringBuilder sb, Theme theme, ThemeModalOptions options );

        /// <summary>
        /// Generates the tabs styles.
        /// </summary>
        /// <param name="sb">Target string builder.</param>
        /// <param name="theme">Theme settings.</param>
        /// <param name="options">Tabs options.</param>
        protected abstract void GenerateTabsStyles( StringBuilder sb, Theme theme, ThemeTabsOptions options );

        /// <summary>
        /// Generates the progress styles.
        /// </summary>
        /// <param name="sb">Target string builder.</param>
        /// <param name="theme">Theme settings.</param>
        /// <param name="options">Progress options.</param>
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

        /// <summary>
        /// Generates the alert styles.
        /// </summary>
        /// <param name="sb">Target string builder.</param>
        /// <param name="theme">Theme settings.</param>
        /// <param name="options">Alert options.</param>
        protected abstract void GenerateAlertStyles( StringBuilder sb, Theme theme, ThemeAlertOptions options );

        /// <summary>
        /// Generates the breadcrumb styles.
        /// </summary>
        /// <param name="sb">Target string builder.</param>
        /// <param name="theme">Theme settings.</param>
        /// <param name="options">Breadcrumb options.</param>
        protected abstract void GenerateBreadcrumbStyles( StringBuilder sb, Theme theme, ThemeBreadcrumbOptions options );

        /// <summary>
        /// Generates the badge styles.
        /// </summary>
        /// <param name="sb">Target string builder.</param>
        /// <param name="theme">Theme settings.</param>
        /// <param name="options">Badge options.</param>
        protected abstract void GenerateBadgeStyles( StringBuilder sb, Theme theme, ThemeBadgeOptions options );

        /// <summary>
        /// Generates the pagination styles.
        /// </summary>
        /// <param name="sb">Target string builder.</param>
        /// <param name="theme">Theme settings.</param>
        /// <param name="options">Pagination options.</param>
        protected abstract void GeneratePaginationStyles( StringBuilder sb, Theme theme, ThemePaginationOptions options );

        /// <summary>
        /// Generates the bar styles.
        /// </summary>
        /// <param name="sb">Target string builder.</param>
        /// <param name="theme">Theme settings.</param>
        /// <param name="options">Bar options.</param>
        protected abstract void GenerateBarStyles( StringBuilder sb, Theme theme, ThemeBarOptions options );

        /// <summary>
        /// Generates the steps styles.
        /// </summary>
        /// <param name="sb">Target string builder.</param>
        /// <param name="theme">Theme settings.</param>
        /// <param name="options">Steps options.</param>
        protected abstract void GenerateStepsStyles( StringBuilder sb, Theme theme, ThemeStepsOptions options );

        /// <summary>
        /// Generates the rating styles.
        /// </summary>
        /// <param name="sb">Target string builder.</param>
        /// <param name="theme">Theme settings.</param>
        /// <param name="options">Rating options.</param>
        protected abstract void GenerateRatingStyles( StringBuilder sb, Theme theme, ThemeRatingOptions options );

        /// <summary>
        /// Generates the paragraph variant styles.
        /// </summary>
        /// <param name="sb">Target string builder.</param>
        /// <param name="theme">Theme settings.</param>
        /// <param name="variant">Color variant name.</param>
        /// <param name="color">Color value.</param>
        protected abstract void GenerateParagraphVariantStyles( StringBuilder sb, Theme theme, string variant, string color );

        /// <summary>
        /// Generates the input variant styles.
        /// </summary>
        /// <param name="sb">Target string builder.</param>
        /// <param name="theme">Theme settings.</param>
        /// <param name="variant">Color variant name.</param>
        /// <param name="color">Color value.</param>
        protected abstract void GenerateInputVariantStyles( StringBuilder sb, Theme theme, string variant, string color );

        #endregion

        #region Helpers

        /// <summary>
        /// Determines the border radius from the supplied parameters.
        /// </summary>
        /// <param name="theme">Theme settings.</param>
        /// <param name="borderRadius">Border radius.</param>
        /// <param name="fallbackRadius">Fallback radius if <paramref name="borderRadius">border radius</paramref> is undefined.</param>
        /// <returns>The right border radius or 0rem if none is defined.</returns>
        protected static string GetBorderRadius( Theme theme, string borderRadius, string fallbackRadius )
        {
            if ( theme.IsRounded )
                return FirstNonEmptyString( borderRadius, fallbackRadius, "0rem" );

            return "0rem";
        }

        /// <summary>
        /// Builds the gradient or background CSS style.
        /// </summary>
        /// <param name="theme">Theme settings.</param>
        /// <param name="color">Background color.</param>
        /// <param name="percentage">Percentage of blend if gradiend is used.</param>
        /// <param name="important">If true, !important flag will be set.</param>
        /// <returns>Gradient or background CSS style.</returns>
        protected virtual string GetGradientBg( Theme theme, string color, float? percentage, bool important = false )
        {
            return theme.IsGradient
                ? $"background: {color} linear-gradient(180deg, {ToHex( Blend( System.Drawing.Color.White, ParseColor( color ), percentage ?? 15f ) )}, {color}) repeat-x{( important ? " !important" : "" )};"
                : $"background-color: {color}{( important ? " !important" : "" )};";
        }

        /// <summary>
        /// Lightens or darkens the color based on the supplied level.
        /// </summary>
        /// <param name="theme">Theme settings.</param>
        /// <param name="inColor">Base color.</param>
        /// <param name="level">Level to adjust the color.</param>
        /// <remarks>Negative level values will lighten the color, while higher levels will darken.</remarks>
        /// <returns>The adjusted color.</returns>
        protected System.Drawing.Color ThemeColorLevel( Theme theme, string inColor, int level )
        {
            var color = ParseColor( inColor );

            var colorBase = level > 0
                ? ParseColor( Var( ThemeVariables.Black, "#343a40" ) )
                : ParseColor( Var( ThemeVariables.White, "#ffffff" ) );

            level = Math.Abs( level );

            return Blend( colorBase, color, level * theme.ThemeColorInterval );
        }

        /// <summary>
        /// Lightens or darkens the color based on the supplied level.
        /// </summary>
        /// <param name="theme">Theme settings.</param>
        /// <param name="color">Base color.</param>
        /// <param name="level">Level to adjust the color.</param>
        /// <remarks>Negative level values will lighten the color, while higher levels will darken.</remarks>
        /// <returns>The adjusted color.</returns>
        protected System.Drawing.Color ThemeColorLevel( Theme theme, System.Drawing.Color color, int level )
        {
            var colorBase = level > 0
                ? ParseColor( Var( ThemeVariables.Black, "#343a40" ) )
                : ParseColor( Var( ThemeVariables.White, "#ffffff" ) );

            level = Math.Abs( level );

            return Blend( colorBase, color, level * theme.ThemeColorInterval );
        }

        /// <summary>
        /// Lightens or darkens the color based on the supplied level.
        /// </summary>
        /// <param name="theme">Theme settings.</param>
        /// <param name="inColor">Base color.</param>
        /// <param name="level">Level to adjust the color.</param>
        /// <remarks>Negative level values will lighten the color, while higher levels will darken.</remarks>
        /// <returns>The adjusted color in hex format.</returns>
        protected string ThemeColorLevelHex( Theme theme, string inColor, int level )
        {
            return ToHex( ThemeColorLevel( theme, inColor, level ) );
        }

        /// <summary>
        /// Parses the supplied string value and converts it to a <see cref="System.Drawing.Color"/>.
        /// </summary>
        /// <param name="value">String that represents a color.</param>
        /// <returns>Color value.</returns>
        protected static System.Drawing.Color ParseColor( string value )
        {
            if ( value.StartsWith( '#' ) )
                return HexStringToColor( value );
            else if ( value.StartsWith( "rgb" ) )
                return CssRgbaFunctionToColor( value );

            return System.Drawing.Color.FromName( value );
        }

        /// <summary>
        /// Converts the RGBA to RGB color format.
        /// </summary>
        /// <param name="background">Tha background color of the system.</param>
        /// <param name="color">The color to convert.</param>
        /// <param name="customAlpha">Alpha component of a new color value.</param>
        /// <returns>A blend of all the supplied color value.</returns>
        protected static System.Drawing.Color Rgba2Rgb( System.Drawing.Color background, System.Drawing.Color color, float? customAlpha = null )
        {
            var alpha = customAlpha ?? color.A / byte.MaxValue;

            return System.Drawing.Color.FromArgb(
                (int)( ( 1 - alpha ) * background.R + alpha * color.R ),
                (int)( ( 1 - alpha ) * background.G + alpha * color.G ),
                (int)( ( 1 - alpha ) * background.B + alpha * color.B )
            );
        }

        /// <summary>
        /// Converts the hexadecimal string into a <see cref="System.Drawing.Color">Color</see> value.
        /// </summary>
        /// <param name="hexColor">A color represented as hexadecimal string.</param>
        /// <returns>Parsed color value or <see cref="System.Drawing.Color.Empty">Empty</see> if failed.</returns>
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

        /// <summary>
        /// Converts the function call into into a <see cref="System.Drawing.Color">Color</see> value.
        /// </summary>
        /// <param name="cssColor">A color represented as (rgb or rgba) function call.</param>
        /// <returns>Parsed color value or <see cref="System.Drawing.Color.Empty">Empty</see> if failed.</returns>
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
        /// <param name="input">A string to extract.</param>
        /// <returns>A new hex string.</returns>
        protected static string ExtractHexDigits( string input )
        {
            // remove any characters that are not digits (like #)
            Regex isHexDigit = new( "[abcdefABCDEF\\d]+", RegexOptions.Compiled );
            string newnum = "";
            foreach ( char c in input )
            {
                if ( isHexDigit.IsMatch( c.ToString() ) )
                    newnum += c.ToString();
            }
            return newnum;
        }

        /// <summary>
        /// Converts the color to a 6 digit hexadecimal, or 8 digit hexadecimal string if alpha is defined.
        /// </summary>
        /// <param name="color">Color to convert.</param>
        /// <returns>A 6 or 8 hexadecimal digit representation of color value.</returns>
        protected static string ToHex( System.Drawing.Color color )
        {
            if ( color.A < 255 )
                return $"#{color.R:X2}{color.G:X2}{color.B:X2}{color.A:X2}";

            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        /// <summary>
        /// Converts the color 8 digit hexadecimal string.
        /// </summary>
        /// <param name="color">Color to convert.</param>
        /// <returns>A 8 hexadecimal representation of color value.</returns>
        protected static string ToHexRGBA( System.Drawing.Color color )
        {
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}{color.A:X2}";
        }

        /// <summary>
        /// Applied the transparency to the supplied color.
        /// </summary>
        /// <param name="hexColor">Hexadecimal representation of color value.</param>
        /// <param name="alpha">The alpha component. Valid values are 0 through 255.</param>
        /// <returns>New transparent color.</returns>
        protected static System.Drawing.Color Transparency( string hexColor, int alpha )
        {
            var color = ParseColor( hexColor );

            return System.Drawing.Color.FromArgb( alpha, color.R, color.G, color.B );
        }

        /// <summary>
        /// Applied the transparency to the supplied color.
        /// </summary>
        /// <param name="color">Color value.</param>
        /// <param name="alpha">The alpha component. Valid values are 0 through 255.</param>
        /// <returns>New transparent color.</returns>
        protected static System.Drawing.Color Transparency( System.Drawing.Color color, int alpha )
        {
            return System.Drawing.Color.FromArgb( alpha, color.R, color.G, color.B );
        }

        /// <summary>
        /// Darkens the color based on the defined percentage.
        /// </summary>
        /// <param name="hexColor">Hexadecimal representation of the color to darken.</param>
        /// <param name="percentage">Percentage of how much to darken the color.</param>
        /// <returns>Darkened color.</returns>
        protected static System.Drawing.Color Darken( string hexColor, float percentage )
        {
            var color = ParseColor( hexColor );

            return Darken( color, percentage );
        }

        /// <summary>
        /// Darkens the color based on the defined percentage.
        /// </summary>
        /// <param name="color">Color to darken.</param>
        /// <param name="percentage">Percentage of how much to darken the color.</param>
        /// <returns>Darkened color.</returns>
        protected static System.Drawing.Color Darken( System.Drawing.Color color, float percentage )
        {
            return ChangeColorBrightness( color, -1 * percentage / 100f );
        }

        /// <summary>
        /// Lightens the color based on the defined percentage.
        /// </summary>
        /// <param name="hexColor">Hexadecimal representation of the color to darken.</param>
        /// <param name="percentage">Percentage of how much to lighten the color.</param>
        /// <returns>Lightened color.</returns>
        protected static System.Drawing.Color Lighten( string hexColor, float percentage )
        {
            var color = ParseColor( hexColor );

            return Lighten( color, percentage );
        }

        /// <summary>
        /// Lightens the color based on the defined percentage.
        /// </summary>
        /// <param name="color">Color to lighten.</param>
        /// <param name="percentage">Percentage of how much to lighten the color.</param>
        /// <returns>Lightened color.</returns>
        protected static System.Drawing.Color Lighten( System.Drawing.Color color, float percentage )
        {
            return ChangeColorBrightness( color, percentage / 100f );
        }

        /// <summary>
        /// Inverts the supplied color.
        /// </summary>
        /// <param name="color">Color to invert.</param>
        /// <returns>Inverted color.</returns>
        protected static System.Drawing.Color Invert( System.Drawing.Color color )
        {
            return System.Drawing.Color.FromArgb( 255 - color.R, 255 - color.G, 255 - color.B );
        }

        /// <summary>
        /// Applies the correction factor on a color to make it brighter.
        /// </summary>
        /// <param name="color">Color to brighten.</param>
        /// <param name="correctionFactor">How much to correct the colot.</param>
        /// <returns>Brightened color.</returns>
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

        /// <summary>
        /// Applies the theme contrast to supplied color value.
        /// </summary>
        /// <param name="theme">Theme settings.</param>
        /// <param name="hexColor">Hexadecimal representation of the color.</param>
        /// <returns>New color with the applied contrast.</returns>
        protected static System.Drawing.Color Contrast( Theme theme, string hexColor )
        {
            var color = ParseColor( hexColor );

            return Contrast( theme, color );
        }

        /// <summary>
        /// Applies the theme contrast to supplied color value.
        /// </summary>
        /// <param name="theme">Theme settings.</param>
        /// <param name="color">Color to change.</param>
        /// <param name="luminanceThreshold">The treshold that controls the contrast level.</param>
        /// <returns>New color with the applied contrast.</returns>
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

        /// <summary>
        /// Blends the two color based on the supplied percentage.
        /// </summary>
        /// <param name="color">First color.</param>
        /// <param name="color2">Second color.</param>
        /// <param name="percentage">The level of blend.</param>
        /// <returns>Combination of two colors.</returns>
        protected static System.Drawing.Color Blend( System.Drawing.Color color, System.Drawing.Color color2, float percentage )
        {
            var alpha = percentage / 100f;
            byte r = (byte)( ( color.R * alpha ) + color2.R * ( 1f - alpha ) );
            byte g = (byte)( ( color.G * alpha ) + color2.G * ( 1f - alpha ) );
            byte b = (byte)( ( color.B * alpha ) + color2.B * ( 1f - alpha ) );
            return System.Drawing.Color.FromArgb( r, g, b );
        }

        /// <summary>
        /// Gets the first string that is not null or empty.
        /// </summary>
        /// <param name="first">First found string that is not empty.</param>
        /// <param name="values">Array of string to search.</param>
        /// <returns>True if the result is not null.</returns>
        protected static bool FirstNotEmpty( out string first, params string[] values )
        {
            first = values?.FirstOrDefault( x => !string.IsNullOrEmpty( x ) );

            return first != null;
        }

        /// <summary>
        /// Gets the first string that is not null or empty.
        /// </summary>
        /// <param name="values">Array of string to search.</param>
        /// <returns>First found string that is not empty.</returns>
        protected static string FirstNonEmptyString( params string[] values )
        {
            return values.FirstOrDefault( x => !string.IsNullOrEmpty( x ) );
        }

        /// <summary>
        /// Builds the media breakpoint.
        /// </summary>
        /// <param name="size">Size of the media breakpoint.</param>
        /// <param name="content">Content of media breakpoint.</param>
        /// <returns>CSS style with media breakpoint.</returns>
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

        /// <summary>
        /// Gets the currently used theme cache.
        /// </summary>
        protected IThemeCache ThemeCache { get; }

        /// <summary>
        /// Map of all variables currently used.
        /// </summary>
        protected readonly Dictionary<string, string> Variables = new();

        #endregion
    }
}
