#region Using directives
using System;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Lambda2Js;
#endregion

namespace Blazorise.Charts;

public class ScriptableValueBasedOptionsConverter<TOptions, TValue, TContext> : JsonConverter<ScriptableValueBasedOptions<TOptions, TValue, TContext>>
    where TContext : ScriptableOptionsContext
{
    public override ScriptableValueBasedOptions<TOptions, TValue, TContext> Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
    {
        // I only need serialization currently
        throw new NotImplementedException();
    }

    public override void Write( Utf8JsonWriter writer, ScriptableValueBasedOptions<TOptions, TValue, TContext> value, JsonSerializerOptions options )
    {
        if ( value.IsScriptable && value.ScriptableValue != null )
        {
            var jsBody = value.ScriptableValue.CompileToJavascript( new JavascriptCompilationOptions( JsCompilationFlags.BodyOnly ) );

            var result = BuildFunction( value.ScriptableValue, jsBody );

            writer.WriteStringValue( result );
        }
        else if ( value.Value != null )
            JsonSerializer.Serialize( writer, value.Value, value.Value.GetType(), options );
    }

    private static string BuildFunction<T>( Expression<T> expression, string jsBody )
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

    private static void BuildFunctionParameters<T>( StringBuilder sb, Expression<T> value )
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