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
                .Append( $"background-color: {Var( ThemeVariables.BackgroundColor( variant ) )} !important;" )
                .AppendLine( "}" );

            sb.Append( $".hero-{variant}" ).Append( "{" )
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

            sb.Append( $".button.is-{variant}" ).Append( "{" )
                //.Append( $"color: {yiqBackground};" )
                .Append( GetGradientBg( theme, background, options?.GradientBlendPercentage ) )
                .Append( $"border-color: transparent;" )
                .AppendLine( "}" );

            sb.Append( $".button.is-{variant}:hover," ).Append( $".button.is-{variant}.is-hovered" ).Append( "{" )
                .Append( $"color: {yiqHoverBackground};" )
                .Append( GetGradientBg( theme, hoverBackground, options?.GradientBlendPercentage ) )
                .Append( $"border-color: {hoverBorder};" )
                .AppendLine( "}" );

            sb.Append( $".button.is-{variant}:focus," ).Append( $".button.is-{variant}.is-focused" ).Append( "{" )
                .Append( $"color: {yiqHoverBackground};" )
                .Append( GetGradientBg( theme, hoverBackground, options?.GradientBlendPercentage ) )
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
            var color = Var( ThemeVariables.OutlineButtonColor( variant ) );
            var yiqColor = Var( ThemeVariables.OutlineButtonYiqColor( variant ) );
            //var hoverColor = Var( ThemeVariables.OutlineButtonHoverColor( variant ) );
            //var activeColor = Var( ThemeVariables.OutlineButtonActiveColor( variant ) );

            sb.Append( $".button.is-{variant}.is-outlined" ).Append( "{" )
                .Append( $"color: {color};" )
                .Append( $"border-color: {color};" )
                .Append( $"background-color: transparent;" )
                .Append( $"background: transparent;" )
                .AppendLine( "}" );

            sb.Append( $".button.is-{variant}.is-outlined:hover," )
                .Append( $".button.is-{variant}.is-outlined.is-hovered," )
                .Append( $".button.is-{variant}.is-outlined:focus," )
                .Append( $".button.is-{variant}.is-outlined.is-focused" ).Append( "{" )
                .Append( $"color: {yiqColor};" )
                .Append( $"background-color: {color};" )
                .Append( $"border-color: {color};" )
                .AppendLine( "}" );

            sb.Append( $".button.is-{variant}.is-outlined.is-loading::after" ).Append( "{" )
                .Append( $"border-color: transparent transparent white white;" )
                .AppendLine( "}" );

            sb.Append( $".button.is-{variant}.is-outlined[disabled]," ).Append( $"fieldset[disabled] .button.is-{variant}.is-outlined" ).Append( "{" )
                .Append( $"color: {color};" )
                .Append( $"background-color: transparent;" )
                .Append( $"border-color: {color};" )
                .Append( $"box-shadow: none;" )
                .AppendLine( "}" );
        }

        protected override void GenerateButtonStyles( StringBuilder sb, Theme theme, ThemeButtonOptions options )
        {
            sb.Append( $".button" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );

            sb.Append( $".button.is-small" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.SmallBorderRadius, Var( ThemeVariables.BorderRadiusSmall ) )};" )
                .AppendLine( "}" );

            sb.Append( $".button.is-large" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.LargeBorderRadius, Var( ThemeVariables.BorderRadiusLarge ) )};" )
                .AppendLine( "}" );

            if ( !string.IsNullOrEmpty( options?.Padding ) )
                sb.Append( $".button" ).Append( "{" )
                    .Append( $"padding: {options.Padding};" )
                    .AppendLine( "}" );

            if ( !string.IsNullOrEmpty( options?.Margin ) )
                sb.Append( $".button" ).Append( "{" )
                    .Append( $"margin: {options.Margin};" )
                    .AppendLine( "}" );
        }

        protected override void GenerateDropdownStyles( StringBuilder sb, Theme theme, ThemeDropdownOptions options )
        {
            sb.Append( $".dropdown-content" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );
        }

        protected override void GenerateInputStyles( StringBuilder sb, Theme theme, ThemeInputOptions options )
        {
            sb.Append( $".input" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );

            sb.Append( $".select select" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );

            sb.Append( $".textarea" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
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

            sb.Append( $".tag:not(body).is-{variant}" ).Append( "{" )
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

            sb.Append( $".notification-{variant}" ).Append( "{" )
                .Append( $"color: {text};" )
                .Append( GetGradientBg( theme, background, options?.GradientBlendPercentage ) )
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
            //        .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius )};" )
            //        .AppendLine( "}" );

            //if ( !string.IsNullOrEmpty( options.ImageTopRadius ) )
            //    sb.Append( $".card-image-top" ).Append( "{" )
            //        .Append( $"border-top-left-radius: {options.ImageTopRadius};" )
            //        .Append( $"border-top-right-radius: {options.ImageTopRadius};" )
            //        .AppendLine( "}" );
        }

        protected override void GenerateModalStyles( StringBuilder sb, Theme theme, ThemeModalOptions options )
        {
            if ( !string.IsNullOrEmpty( options?.BorderRadius ) )
            {
                sb.Append( $".modal-card-head" ).Append( "{" )
                    .Append( $"border-top-left-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                    .Append( $"border-top-right-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                    .AppendLine( "}" );

                sb.Append( $".modal-card-foot" ).Append( "{" )
                   .Append( $"border-bottom-left-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                   .Append( $"border-bottom-right-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                   .AppendLine( "}" );
            }
        }

        protected override void GenerateTabsStyles( StringBuilder sb, Theme theme, ThemeTabsOptions options )
        {
            if ( !string.IsNullOrEmpty( theme.ColorOptions?.Primary ) )
            {
                sb
                    .Append( $".tabs.is-toggle li.is-active a" )
                    .Append( "{" )
                    .Append( $"background-color: {Var( ThemeVariables.Color( "primary" ) )};" )
                    .AppendLine( "}" );
            }
        }

        protected override void GenerateProgressStyles( StringBuilder sb, Theme theme, ThemeProgressOptions options )
        {
            sb.Append( $".progress" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );
        }

        protected override void GenerateAlertStyles( StringBuilder sb, Theme theme, ThemeAlertOptions options )
        {
            sb.Append( $".notification" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );
        }

        protected override void GenerateBreadcrumbStyles( StringBuilder sb, Theme theme, ThemeBreadcrumbOptions options )
        {
            if ( !string.IsNullOrEmpty( Var( ThemeVariables.BreadcrumbColor ) ) )
            {
                sb.Append( $".breadcrumb a" ).Append( "{" )
                .Append( $"color: {Var( ThemeVariables.BreadcrumbColor )};" )
                .AppendLine( "}" );
            }
        }

        protected override void GenerateBadgeStyles( StringBuilder sb, Theme theme, ThemeBadgeOptions options )
        {
            sb.Append( $".tag:not(body)" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );
        }

        protected override void GeneratePaginationStyles( StringBuilder sb, Theme theme, ThemePaginationOptions options )
        {
            sb.Append( $".pagination-link,.pagination-previous,.pagination-next" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );

            sb.Append( $".pagination.is-large .pagination-link,.pagination-previous,.pagination-next" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.LargeBorderRadius, Var( ThemeVariables.BorderRadiusLarge ) )};" )
                .AppendLine( "}" );

            if ( !string.IsNullOrEmpty( theme.ColorOptions?.Primary ) )
            {
                sb.Append( $".pagination-link.is-current,.pagination-previous.is-current,.pagination-next.is-current" ).Append( "{" )
                    .Append( $"background-color: {Var( ThemeVariables.Color( "primary" ) )};" )
                    .Append( $"border-color: {Var( ThemeVariables.Color( "primary" ) )};" )
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

                sb.Append( $".navbar.has-background-{variant} .navbar-brand .navbar-item a.navbar-item" ).Append( "{" )
                    .Append( $"color: {yiqColor};" )
                    .AppendLine( "}" );
            }
        }

        protected override void GenerateParagraphVariantStyles( StringBuilder sb, Theme theme, string variant, string inTextColor )
        {
            var textColor = ParseColor( inTextColor );

            var textColorHex = ToHex( textColor );

            sb.Append( $".has-text-{variant}" )
                .Append( "{" )
                .Append( $"color: {textColorHex};" )
                .AppendLine( "}" );
        }

        protected override void GenerateInputVariantStyles( StringBuilder sb, Theme theme, string variant, string inColor )
        {
            var color = ToHex( ParseColor( inColor ) );

            sb.Append( $".input.is-{variant}" )
                .Append( "{" )
                .Append( $"color: {color};" )
                .AppendLine( "}" );
        }

        #endregion
    }
}
