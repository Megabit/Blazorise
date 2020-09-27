﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
using Blazorise.Bootstrap;
#endregion

namespace Blazorise.Material
{
    public class MaterialThemeGenerator : BootstrapThemeGenerator
    {
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

            // Material provider have some special rules for buttons placed inside of modal footer. So to keep it 
            // consistent we need to apply the same styles as in the base generator.
            sb.Append( $".modal-footer .btn-{variant}" ).Append( "{" )
                .Append( $"color: {yiqBackground};" )
                .Append( GetGradientBg( theme, background, options?.GradientBlendPercentage ) )
                .Append( $"border-color: {border};" )
                .AppendLine( "}" );

            sb.Append( $".modal-footer .btn-{variant}:hover" ).Append( "{" )
                .Append( $"color: {yiqHoverBackground};" )
                .Append( GetGradientBg( theme, hoverBackground, options?.GradientBlendPercentage ) )
                .Append( $"border-color: {hoverBorder};" )
                .AppendLine( "}" );

            sb.Append( $".modal-footer .btn-{variant}:focus," )
                .Append( $".modal-footer .btn-{variant}.focus" )
                .Append( "{" )
                .Append( $"color: {yiqHoverBackground};" )
                .Append( GetGradientBg( theme, hoverBackground, options?.GradientBlendPercentage ) )
                .Append( $"border-color: {hoverBorder};" )
                .Append( $"box-shadow: 0 0 0 {options?.BoxShadowSize ?? ".2rem"} {boxShadow};" )
                .AppendLine( "}" );

            sb.Append( $".modal-footer .btn-{variant}.disabled," )
                .Append( $".modal-footer .btn-{variant}:disabled" )
                .Append( "{" )
                .Append( $"color: {yiqBackground};" )
                .Append( $"background-color: {background};" )
                .Append( $"border-color: {border};" )
                .Append( $"box-shadow: 0 0 0 {options?.BoxShadowSize ?? ".2rem"} {boxShadow};" )
                .AppendLine( "}" );

            sb
                .Append( $".modal-footer .btn-{variant}:not(:disabled):not(.disabled):active," )
                .Append( $".modal-footer .btn-{variant}:not(:disabled):not(.disabled).active," )
                .Append( $".modal-footer .show>.btn-{variant}.dropdown-toggle" )
                .Append( "{" )
                .Append( $"color: {yiqActiveBackground};" )
                .Append( $"background-color: {activeBackground};" )
                .Append( $"border-color: {activeBorder};" )
                .AppendLine( "}" );

            sb
                .Append( $".modal-footer .btn-{variant}:not(:disabled):not(.disabled):active:focus," )
                .Append( $".modal-footer .btn-{variant}:not(:disabled):not(.disabled).active:focus," )
                .Append( $".modal-footer .show>.btn-{variant}.dropdown-toggle:focus" )
                .Append( "{" )
                .Append( $"box-shadow: 0 0 0 {options?.BoxShadowSize ?? ".2rem"} {boxShadow}" )
                .AppendLine( "}" );

            base.GenerateButtonVariantStyles( sb, theme, variant, options );
        }

        protected override void GenerateButtonOutlineVariantStyles( StringBuilder sb, Theme theme, string variant, ThemeButtonOptions options )
        {
            var color = Var( ThemeVariables.OutlineButtonColor( variant ) );

            sb.Append( $".btn-outline-{variant}," )
                .Append( $".btn-outline-{variant}.active," )
                .Append( $".btn-outline-{variant}:focus," )
                .Append( $".btn-outline-{variant}:hover" )
                .Append( "{" )
                .Append( $"color: {color};" )
                .AppendLine( "}" );

            sb.Append( $".btn-outline-{variant}.disabled," )
                .Append( $".btn-outline-{variant}:disabled" )
                .Append( "{" )
                .Append( $"color: {color};" )
                .AppendLine( "}" );
        }

        protected override void GenerateInputCheckEditStyles( StringBuilder sb, Theme theme, ThemeInputOptions options )
        {
            sb
                .Append( $".custom-checkbox .custom-control-input:checked ~ .custom-control-label::before" ).Append( "{" )
                .Append( $"background-color: {options.CheckColor};" )
                .AppendLine( "}" );

            sb
                .Append( $".custom-control-input:checked ~ .custom-control-label::after" ).Append( "{" )
                .Append( $"color: {options.CheckColor};" )
                .AppendLine( "}" );

            sb
                .Append( $".custom-control-input:checked ~ .custom-control-label::before" ).Append( "{" )
                .Append( $"background-color: {options.CheckColor};" )
                .AppendLine( "}" );

            sb
                .Append( $".custom-switch .custom-control-input:checked ~ .custom-control-label::after" ).Append( "{" )
                .Append( $"background-color: {options.CheckColor};" )
                .AppendLine( "}" );

            var trackColor = ToHex( Lighten( options.CheckColor, 50f ) );

            if ( !string.IsNullOrEmpty( trackColor ) )
            {
                sb
                    .Append( $".custom-switch .custom-control-input:checked~.custom-control-track" ).Append( "{" )
                    .Append( $"background-color: {trackColor};" )
                    .AppendLine( "}" );
            }
        }
    }
}
