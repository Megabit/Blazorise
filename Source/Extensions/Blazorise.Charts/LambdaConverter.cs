#region Using directives
using System;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Lambda2Js;
#endregion

namespace Blazorise.Charts
{
    public class LambdaConverter<T> : JsonConverter<Expression<T>>
    {
        public override Expression<T> Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
        {
            throw new NotImplementedException();
        }

        public override void Write( Utf8JsonWriter writer, Expression<T> value, JsonSerializerOptions options )
        {
            var jsBody = value.CompileToJavascript( new JavascriptCompilationOptions( JsCompilationFlags.BodyOnly ) );

            var result = BuildFunction( value, jsBody );

            writer.WriteStringValue( result );
        }

        private static string BuildFunction( Expression<T> expression, string jsBody )
        {
            var sb = new StringBuilder();

            sb.Append( "function(" );
            BuildFunctionParameters( sb, expression );
            sb.Append( ") {" );

            sb.Append( "return " );
            sb.Append( jsBody );
            sb.Append( "; }" );

            return sb.ToString();
        }

        private static void BuildFunctionParameters( StringBuilder sb, Expression<T> value )
        {
            if ( value.Parameters == null || value.Parameters.Count == 0 )
                return;

            for ( int i = 0; i < value.Parameters.Count; i++ )
            {
                if ( i > 0 )
                    sb.Append( ", " );

                sb.Append( value.Parameters[i].Name );
            }
        }
    }
}
