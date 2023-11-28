using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Blazorise.Utilities;

/// <summary>
/// Helper methods to convert a <see cref="LambdaExpression"/> to valid JavaScript.
/// </summary>
public static class ExpressionConverter
{
    /// <summary>
    /// Converts a <see cref="LambdaExpression"/> to valid Templated String Literal.
    /// </summary>
    /// <param name="expression">The expression</param>
    /// <returns>The Templated String Literal</returns>
    public static string ToTemplatedStringLiteral( LambdaExpression expression )
    {
        if ( expression is null )
        {
            throw new ArgumentNullException( nameof( expression ) );
        }

        if ( expression.ReturnType != typeof( FormattableString ) )
        {
            throw new ArgumentException( "The expression must return a FormattableString.", nameof( expression ) );
        }

        var argumentProvider = (IArgumentProvider)expression.Body;
        var format = $"`{( argumentProvider.GetArgument( 0 ) as ConstantExpression ).Value}`";
        var arrayExpression = (NewArrayExpression)argumentProvider.GetArgument( 1 );

        var arguments = new List<object>();
        foreach ( var subExpression in arrayExpression.Expressions )
        {
            HandleExpression( arguments, subExpression );
        }

        return string.Format( format, arguments.ToArray() );
    }

    static void HandleExpression( List<object> arguments, Expression expression, bool interpolateVariable = true )
    {
        switch ( expression )
        {
            case BinaryExpression be:
                HandleBinaryExpression( arguments, be, interpolateVariable );
                break;

            case ConditionalExpression ce:
                var ceArgs = new List<object>();
                HandleExpression( ceArgs, ce.Test, false );
                HandleExpression( ceArgs, ce.IfTrue, false );
                HandleExpression( ceArgs, ce.IfFalse, false );

                arguments.Add( Build( $"({ceArgs[0]}) ? ({ceArgs[1]}) : ({ceArgs[2]})", interpolateVariable ) );
                break;

            case ConstantExpression ce:
                arguments.Add( Build( ce.Value, interpolateVariable ) );
                break;

            case UnaryExpression ue:
                HandleExpression( arguments, ue.Operand, interpolateVariable );
                break;

            case ParameterExpression pe:
                arguments.Add( Build( pe.Name, interpolateVariable ) );
                break;

            default:
                throw new NotSupportedException( $"Expression of type {expression.GetType()} is not supported." );
        }
    }

    private static void HandleBinaryExpression( List<object> arguments, BinaryExpression be, bool interpolateVariable )
    {
        var @operator = GetOperator( be.NodeType, out var useStringFormat );
        if ( @operator != null )
        {
            var beArgs = new List<object>();
            HandleExpression( beArgs, be.Left, false );
            HandleExpression( beArgs, be.Right, false );

            if ( useStringFormat )
            {
                arguments.Add( Build( string.Format( @operator, beArgs[0], beArgs[1] ), interpolateVariable ) );
            }
            else
            {
                arguments.Add( Build( $"{beArgs[0]} {@operator} {beArgs[1]}", interpolateVariable ) );
            }
        }
        else
        {
            throw new NotSupportedException( $"Operation '{be.NodeType}' is not supported." );
        }
    }

    static string Build( object value, bool interpolateVariable )
    {
        string result = value switch
        {
            string stringValue => $"'{stringValue}'",
            _ => value.ToString(),
        };
        return Build( result, interpolateVariable );
    }

    static string Build( string value, bool interpolateVariable )
    {
        return interpolateVariable ? $"${{({value})}}" : value;
    }

    private static string GetOperator( ExpressionType type, out bool useStringFormat )
    {
        useStringFormat = false;
        switch ( type )
        {
            case ExpressionType.Add:
                return "+";
            case ExpressionType.And:
                return "&";
            case ExpressionType.AndAlso:
                return "&&";
            case ExpressionType.ArrayIndex:
                useStringFormat = true;
                return "{0}[{1}]";
            case ExpressionType.Divide:
                return "/";
            case ExpressionType.Equal:
                return "===";
            case ExpressionType.GreaterThan:
                return ">";
            case ExpressionType.GreaterThanOrEqual:
                return ">=";
            case ExpressionType.LessThan:
                return "<";
            case ExpressionType.LessThanOrEqual:
                return "<=";
            case ExpressionType.Multiply:
                return "*";
            case ExpressionType.Not:
                return "!";
            case ExpressionType.NotEqual:
                return "!==";
            case ExpressionType.Power:
                return "^";
            case ExpressionType.Or:
                return "|";
            case ExpressionType.OrElse:
                return "||";
            case ExpressionType.Subtract:
                return "-";
            default:
                return null;
        }
    }
}