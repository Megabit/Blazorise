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

        static string BuildFunction( Expression<T> expression, string jsBody )
        {
            var sb = new StringBuilder();

            sb.Append( "function(" );
            sb.Append( BuildFunctionParameters( expression ) );
            sb.Append( ") {" );

            sb.Append( "return " );
            sb.Append( jsBody );
            sb.Append( "; }" );

            return sb.ToString();
        }

        static string BuildFunctionParameters( Expression<T> value )
        {
            if ( value.Parameters == null || value.Parameters.Count == 0 )
                return null;

            var sb = new StringBuilder();

            for ( int i = 0; i < value.Parameters.Count; i++ )
            {
                if ( i > 0 )
                    sb.Append( ", " );
                sb.Append( value.Parameters[i].Name );
            }

            return sb.ToString();
        }
    }

    //class JsExpressionVisitor : ExpressionVisitor
    //{
    //    private readonly StringBuilder _builder;

    //    public JsExpressionVisitor()
    //    {
    //        _builder = new StringBuilder();
    //    }

    //    public string JavaScriptCode
    //    {
    //        get { return _builder.ToString(); }
    //    }

    //    public override Expression Visit( Expression node )
    //    {
    //        _builder.Clear();
    //        return base.Visit( node );
    //    }

    //    protected override Expression VisitParameter( ParameterExpression node )
    //    {
    //        _builder.Append( node.Name );
    //        base.VisitParameter( node );
    //        return node;
    //    }

    //    protected override Expression VisitBinary( BinaryExpression node )
    //    {
    //        base.Visit( node.Left );
    //        _builder.Append( GetOperator( node.NodeType ) );
    //        base.Visit( node.Right );
    //        return node;
    //    }

    //    protected override Expression VisitLambda<T>( Expression<T> node )
    //    {
    //        _builder.Append( "function(" );
    //        for ( int i = 0; i < node.Parameters.Count; i++ )
    //        {
    //            if ( i > 0 )
    //                _builder.Append( ", " );
    //            _builder.Append( node.Parameters[i].Name );
    //        }
    //        _builder.Append( ") {" );
    //        if ( node.Body.Type != typeof( void ) )
    //        {
    //            _builder.Append( "return " );
    //        }
    //        base.Visit( node.Body );
    //        _builder.Append( "; }" );
    //        return node;
    //    }

    //    private static string GetOperator( ExpressionType nodeType )
    //    {
    //        switch ( nodeType )
    //        {
    //            case ExpressionType.Add:
    //                return " + ";
    //            case ExpressionType.Multiply:
    //                return " * ";
    //            case ExpressionType.Subtract:
    //                return " - ";
    //            case ExpressionType.Divide:
    //                return " / ";
    //            case ExpressionType.Assign:
    //                return " = ";
    //            case ExpressionType.Equal:
    //                return " == ";
    //            case ExpressionType.NotEqual:
    //                return " != ";

    //                // TODO: Add other operators...
    //        }
    //        throw new NotImplementedException( "Operator not implemented" );
    //    }
    //}
}
