#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
using Blazorise.Bootstrap;
#endregion

namespace Blazorise.Material
{
    public class MaterialThemeGenerator : BootstrapThemeGenerator
    {
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
