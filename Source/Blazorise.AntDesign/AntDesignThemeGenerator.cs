#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace Blazorise.AntDesign
{
    public class AntDesignThemeGenerator : ThemeGenerator
    {
        #region Methods

        protected override void GenerateBackgroundVariantStyles( StringBuilder sb, Theme theme, string variant )
        {
            sb.Append( $".bg-{variant}" ).Append( "{" )
                .Append( $"background-color: {Var( ThemeVariables.BackgroundColor( variant ) )} !important;" )
                .AppendLine( "}" );

            sb.Append( $".ant-hero-{variant}" ).Append( "{" )
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

            sb.Append( $".ant-btn-{variant}" ).Append( "{" )
                .Append( $"color: {yiqBackground} !important;" )
                .Append( GetGradientBg( theme, background, options?.GradientBlendPercentage, true ) )
                .Append( $"border-color: {border} !important;" )
                .AppendLine( "}" );

            sb.Append( $".ant-btn-{variant} > a:only-child" ).Append( "{" )
                .Append( $"color: currentColor !important;" )
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
                .Append( $"color: currentColor !important;" )
                .AppendLine( "}" );

            sb
                .Append( $".btn-{variant}:active," )
                .Append( $".btn-{variant}.active," )
                .Append( $".btn-{variant}-active" )
                .Append( "{" )
                .Append( $"color: {yiqActiveBackground} !important;" )
                .Append( $"background-color: {activeBackground} !important;" )
                .Append( $"border-color: {activeBorder} !important;" )
                .AppendLine( "}" );

            sb
                .Append( $".btn-{variant}:active > a:only-child," )
                .Append( $".btn-{variant}.active > a:only-child," )
                .Append( $".btn-{variant}-active > a:only-child" )
                .Append( "{" )
                .Append( $"color: currentColor !important;" )
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
                .Append( $".btn-{variant}:disabled" )
                .Append( "{" )
                .Append( $"color: rgba(0, 0, 0, 0.25) !important;" )
                .Append( $"background-color: #f5f5f5 !important;" )
                .Append( $"border-color: #d9d9d9 !important;" )
                .Append( $"text-shadow: none !important;" )
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
                .Append( $".btn-{variant}:disabled" )
                .Append( "{" )
                .Append( $"color: currentColor !important;" )
                .AppendLine( "}" );
        }

        protected override void GenerateButtonOutlineVariantStyles( StringBuilder sb, Theme theme, string variant, ThemeButtonOptions options )
        {
            var color = Var( ThemeVariables.OutlineButtonColor( variant ) );
            var hoverColor = Var( ThemeVariables.OutlineButtonHoverColor( variant ) );
            var activeColor = Var( ThemeVariables.OutlineButtonActiveColor( variant ) );

            sb.Append( $".ant-btn-outline-{variant}" ).Append( "{" )
                .Append( $"color: {color} !important;" )
                .Append( $"background: transparent !important;" )
                .Append( $"border-color: {color} !important;" )
                .AppendLine( "}" );

            sb.Append( $".ant-btn-outline-{variant} > a:only-child" ).Append( "{" )
                .Append( $"color: currentColor !important;" )
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
                .Append( $"color: currentColor !important;" )
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
                .Append( $"color: currentColor !important;" )
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
                .Append( $".btn-{variant}:disabled" )
                .Append( "{" )
                .Append( $"color: rgba(0, 0, 0, 0.25) !important;" )
                .Append( $"border-color: #d9d9d9 !important;" )
                .Append( $"text-shadow: none !important;" )
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
                .Append( $".btn-{variant}:disabled" )
                .Append( "{" )
                .Append( $"color: currentColor !important;" )
                .AppendLine( "}" );
        }

        protected override void GenerateButtonStyles( StringBuilder sb, Theme theme, ThemeButtonOptions options )
        {
            sb.Append( $".ant-btn" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );

            sb.Append( $".ant-btn-sm" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.SmallBorderRadius, Var( ThemeVariables.BorderRadiusSmall ) )};" )
                .AppendLine( "}" );

            sb.Append( $".ant-btn-lg" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.LargeBorderRadius, Var( ThemeVariables.BorderRadiusLarge ) )};" )
                .AppendLine( "}" );

            if ( !string.IsNullOrEmpty( options?.Padding ) )
                sb.Append( $".ant-btn" ).Append( "{" )
                    .Append( $"padding: {options.Padding};" )
                    .AppendLine( "}" );

            if ( !string.IsNullOrEmpty( options?.Margin ) )
                sb.Append( $".ant-btn" ).Append( "{" )
                    .Append( $"margin: {options.Margin};" )
                    .AppendLine( "}" );
        }

        protected override void GenerateDropdownStyles( StringBuilder sb, Theme theme, ThemeDropdownOptions options )
        {
            sb.Append( $".ant-dropdown-menu" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );

            if ( !string.IsNullOrEmpty( theme.ColorOptions?.Primary ) )
            {
                var backgroundColor = ParseColor( theme.ColorOptions.Primary );

                if ( !backgroundColor.IsEmpty )
                {
                    var background = ToHex( backgroundColor );

                    sb.Append( $".ant-dropdown-menu-item.active," )
                        .Append( $".ant-dropdown-menu-item:active" ).Append( "{" )
                        .Append( GetGradientBg( theme, background, options?.GradientBlendPercentage ) )
                        .AppendLine( "}" );
                }
            }
        }

        protected override void GenerateInputStyles( StringBuilder sb, Theme theme, ThemeInputOptions options )
        {
            sb.Append( $".ant-form-item input" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );

            sb.Append( $".ant-input-group-addon:first-child" ).Append( "{" )
                .Append( $"border-top-left-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .Append( $"border-bottom-left-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );

            sb.Append( $".ant-input-group-addon:last-child" ).Append( "{" )
                .Append( $"border-top-right-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .Append( $"border-bottom-right-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );

            sb.Append( $".ant-select-selector, .ant-select-selector input" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )} !important;" )
                .AppendLine( "}" );

            sb.Append( $".ant-checkbox-inner" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );

            sb.Append( $".ant-upload button" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );

            if ( !string.IsNullOrEmpty( options?.Color ) )
            {
                sb.Append( $".ant-input" ).Append( "{" )
                    .Append( $"color: {options.Color};" )
                    .AppendLine( "}" );

                //sb.Append( $".input-group-text" ).Append( "{" )
                //    .Append( $"color: {options.Color};" )
                //    .AppendLine( "}" );

                sb.Append( $".ant-select-selection-search-input" ).Append( "{" )
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
        }

        protected virtual void GenerateInputCheckEditStyles( StringBuilder sb, Theme theme, ThemeInputOptions options )
        {
            sb
                .Append( $".ant-checkbox-checked .ant-checkbox-inner" ).Append( "{" )
                .Append( $"background-color: {options.CheckColor};" )
                .Append( $"border-color: {options.CheckColor};" )
                .AppendLine( "}" );

            sb
                .Append( $".ant-radio-wrapper:hover .ant-radio," )
                .Append( $".ant-radio:hover .ant-radio-inner," )
                .Append( $".ant-radio-input:focus + .ant-radio-inner," )
                .Append( $".ant-radio-checked .ant-radio-inner" )
                .Append( "{" )
                .Append( $"border-color: {options.CheckColor};" )
                .AppendLine( "}" );

            sb
                .Append( $".ant-radio-inner::after" ).Append( "{" )
                .Append( $"background-color: {options.CheckColor};" )
                .AppendLine( "}" );

            sb
                .Append( $".ant-switch-checked" ).Append( "{" )
                .Append( $"background-color: {options.CheckColor};" )
                .AppendLine( "}" );
        }

        protected virtual void GenerateInputSliderStyles( StringBuilder sb, Theme theme, ThemeInputOptions options )
        {
            sb.Append( $".ant-slider-track" ).Append( "{" )
                .Append( $"background-color: {options.SliderColor};" )
                .AppendLine( "}" );

            sb.Append( $".ant-slider:hover .ant-slider-track" ).Append( "{" )
                .Append( $"background-color: {ToHex( Darken( options.SliderColor, 20f ) )};" )
                .AppendLine( "}" );

            sb.Append( $".ant-slider-handle" ).Append( "{" )
                .Append( $"border-color: {options.SliderColor};" )
                .AppendLine( "}" );

            sb
                .Append( $".ant-slider-handle:focus," )
                .Append( $".ant-slider:hover .ant-slider-handle:not(.ant-tooltip-open)" )
                .Append( "{" )
                .Append( $"border-color: {ToHex( Darken( options.SliderColor, 20f ) )};" )
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

            sb.Append( $".ant-tag-{variant}" ).Append( "{" )
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
            sb.Append( $".ant-card" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );

            sb.Append( $".ant-card-cover:first-child" ).Append( "{" )
                .Append( $"border-top-left-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .Append( $"border-top-right-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );

            sb.Append( $".ant-card-cover:last-child" ).Append( "{" )
                .Append( $"border-bottom-left-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .Append( $"border-bottom-right-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );

            if ( !string.IsNullOrEmpty( options?.ImageTopRadius ) )
                sb.Append( $".ant-card-cover" ).Append( "{" )
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
            sb.Append( $".ant-tabs .ant-tabs-tab" ).Append( "{" )
                .Append( $"border-top-left-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .Append( $"border-top-right-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );

            sb.Append( $".ant-tabs-pills .ant-tabs-nav .ant-tabs-tab" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );

            if ( !string.IsNullOrEmpty( theme.ColorOptions?.Primary ) )
            {
                sb
                    .Append( $".ant-tabs-pills .ant-tabs-nav .ant-tabs-tab-active" )
                    .Append( "{" )
                    .Append( $"color: {Var( ThemeVariables.White )};" )
                    .Append( $"background-color: {Var( ThemeVariables.Color( "primary" ) )};" )
                    .AppendLine( "}" );

                sb
                    .Append( $".ant-tabs-nav .ant-tabs-tab:active," )
                    .Append( $".ant-tabs-nav .ant-tabs-tab-active" )
                    .Append( "{" )
                    .Append( $"color: {Var( ThemeVariables.Color( "primary" ) )};" )
                    .AppendLine( "}" );

                var hoverColor = ToHex( Lighten( Var( ThemeVariables.Color( "primary" ) ), 20f ) );

                sb
                    .Append( $".ant-tabs-nav .ant-tabs-tab:hover" )
                    .Append( "{" )
                    .Append( $"color: {hoverColor};" )
                    .AppendLine( "}" );
            }
        }

        protected override void GenerateProgressStyles( StringBuilder sb, Theme theme, ThemeProgressOptions options )
        {
            sb
                .Append( $".ant-progress-inner," )
                .Append( $".ant-progress-bg" )
                .Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );

            if ( !string.IsNullOrEmpty( theme.ColorOptions?.Primary ) )
            {
                sb.Append( $".ant-progress-bg" ).Append( "{" )
                    .Append( $"background-color: {Var( ThemeVariables.Color( "primary" ) )};" )
                    .AppendLine( "}" );
            }
        }

        protected override void GenerateAlertStyles( StringBuilder sb, Theme theme, ThemeAlertOptions options )
        {
            sb.Append( $".ant-alert" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );
        }

        protected override void GenerateBreadcrumbStyles( StringBuilder sb, Theme theme, ThemeBreadcrumbOptions options )
        {
            sb.Append( $".ant-breadcrumb" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );


            if ( !string.IsNullOrEmpty( Var( ThemeVariables.BreadcrumbColor ) ) )
            {
                sb.Append( $".ant-breadcrumb-link>a" ).Append( "{" )
                    .Append( $"color: {Var( ThemeVariables.BreadcrumbColor )};" )
                    .AppendLine( "}" );
            }
        }

        protected override void GenerateBadgeStyles( StringBuilder sb, Theme theme, ThemeBadgeOptions options )
        {
            sb.Append( $".ant-tag:not(.ant-tag-pill)" ).Append( "{" )
                .Append( $"border-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );
        }

        protected override void GeneratePaginationStyles( StringBuilder sb, Theme theme, ThemePaginationOptions options )
        {
            sb.Append( $".ant-pagination-item:first-child .ant-pagination-link" ).Append( "{" )
                .Append( $"border-top-left-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .Append( $"border-bottom-left-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );

            sb.Append( $".ant-pagination-item:last-child .ant-pagination-link" ).Append( "{" )
                .Append( $"border-top-right-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .Append( $"border-bottom-right-radius: {GetBorderRadius( theme, options?.BorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );

            sb.Append( $".ant-pagination-lg .ant-pagination-item:first-child .ant-pagination-link" ).Append( "{" )
                .Append( $"border-top-left-radius: {GetBorderRadius( theme, options?.LargeBorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .Append( $"border-bottom-left-radius: {GetBorderRadius( theme, options?.LargeBorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );

            sb.Append( $".ant-pagination-lg .ant-pagination-item:last-child .ant-pagination-link" ).Append( "{" )
                .Append( $"border-top-right-radius: {GetBorderRadius( theme, options?.LargeBorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .Append( $"border-bottom-right-radius: {GetBorderRadius( theme, options?.LargeBorderRadius, Var( ThemeVariables.BorderRadius ) )};" )
                .AppendLine( "}" );

            if ( !string.IsNullOrEmpty( theme.ColorOptions?.Primary ) )
            {
                var color = theme.ColorOptions.Primary;

                sb
                    .Append( $".ant-pagination-item:focus," )
                    .Append( $".ant-pagination-item:hover" )
                    .Append( "{" )
                    .Append( $"border-color: {color};" )
                    .AppendLine( "}" );

                sb
                    .Append( $".ant-pagination-item:focus a," )
                    .Append( $".ant-pagination-item:hover a" )
                    .Append( "{" )
                    .Append( $"color: {color};" )
                    .AppendLine( "}" );

                sb
                    .Append( $".ant-pagination-item-active" )
                    .Append( "{" )
                    .Append( $"border-color: {color};" )
                    .AppendLine( "}" );

                sb.Append( $".ant-pagination-item-active a" )
                    .Append( "{" )
                    .Append( $"color: {color};" )
                    .AppendLine( "}" );

                var hoverColor = ToHex( Lighten( color, 40f ) );

                sb
                    .Append( $".ant-pagination-item-active:focus," )
                    .Append( $".ant-pagination-item-active:hover" )
                    .Append( "{" )
                    .Append( $"border-color: {hoverColor};" )
                    .AppendLine( "}" );

                sb
                    .Append( $".ant-pagination-item-active:focus a," )
                    .Append( $".ant-pagination-item-active:hover a" )
                    .Append( "{" )
                    .Append( $"color: {hoverColor};" )
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

                sb
                    .Append( $".ant-menu.bg-{variant} .ant-menu-item .ant-menu-link," )
                    .Append( $".ant-menu.bg-{variant} .ant-menu-item .ant-menu-submenu-title span" )
                    .Append( "{" )
                    .Append( $"color: {yiqColor};" )
                    .AppendLine( "}" );
            }
        }

        protected override void GenerateParagraphVariantStyles( StringBuilder sb, Theme theme, string variant, string inColor )
        {
            var color = ToHex( ParseColor( inColor ) );

            sb.Append( $".ant-typography-{variant}" )
                .Append( "{" )
                .Append( $"color: {color};" )
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

        #endregion
    }
}
