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
        }
    }
}
