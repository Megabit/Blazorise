#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
#endregion

namespace Blazorise.Reporting.Internal;

internal static class ReportFormulaEvaluator
{
    #region Methods

    internal static object Evaluate( string formula, ReportFormulaContext context )
    {
        if ( string.IsNullOrWhiteSpace( formula ) )
            return null;

        var parser = new Parser( formula, context );

        return parser.Parse();
    }

    #endregion

    #region Classes

    private sealed class Parser
    {
        #region Members

        private readonly string formula;

        private readonly ReportFormulaContext context;

        private int position;

        #endregion

        #region Constructors

        internal Parser( string formula, ReportFormulaContext context )
        {
            this.formula = formula;
            this.context = context ?? new();
        }

        #endregion

        #region Methods

        internal object Parse()
        {
            var value = ParseConditional();
            SkipWhiteSpace();

            return value;
        }

        private object ParseConditional()
        {
            var condition = ParseOr();
            SkipWhiteSpace();

            if ( !Match( "?" ) )
                return condition;

            var whenTrue = ParseConditional();
            Expect( ":" );
            var whenFalse = ParseConditional();

            return ToBoolean( condition ) ? whenTrue : whenFalse;
        }

        private object ParseOr()
        {
            var value = ParseAnd();

            while ( true )
            {
                SkipWhiteSpace();

                if ( !Match( "||" ) )
                    return value;

                value = ToBoolean( value ) || ToBoolean( ParseAnd() );
            }
        }

        private object ParseAnd()
        {
            var value = ParseEquality();

            while ( true )
            {
                SkipWhiteSpace();

                if ( !Match( "&&" ) )
                    return value;

                value = ToBoolean( value ) && ToBoolean( ParseEquality() );
            }
        }

        private object ParseEquality()
        {
            var value = ParseComparison();

            while ( true )
            {
                SkipWhiteSpace();

                if ( Match( "==" ) )
                    value = Compare( value, ParseComparison() ) == 0;
                else if ( Match( "!=" ) )
                    value = Compare( value, ParseComparison() ) != 0;
                else
                    return value;
            }
        }

        private object ParseComparison()
        {
            var value = ParseAdditive();

            while ( true )
            {
                SkipWhiteSpace();

                if ( Match( ">=" ) )
                    value = Compare( value, ParseAdditive() ) >= 0;
                else if ( Match( "<=" ) )
                    value = Compare( value, ParseAdditive() ) <= 0;
                else if ( Match( ">" ) )
                    value = Compare( value, ParseAdditive() ) > 0;
                else if ( Match( "<" ) )
                    value = Compare( value, ParseAdditive() ) < 0;
                else
                    return value;
            }
        }

        private object ParseAdditive()
        {
            var value = ParseMultiplicative();

            while ( true )
            {
                SkipWhiteSpace();

                if ( Match( "+" ) )
                {
                    var otherValue = ParseMultiplicative();
                    value = value is string || otherValue is string
                        ? $"{FormatValue( value )}{FormatValue( otherValue )}"
                        : ToDecimal( value ) + ToDecimal( otherValue );
                }
                else if ( Match( "-" ) )
                {
                    value = ToDecimal( value ) - ToDecimal( ParseMultiplicative() );
                }
                else
                {
                    return value;
                }
            }
        }

        private object ParseMultiplicative()
        {
            var value = ParseUnary();

            while ( true )
            {
                SkipWhiteSpace();

                if ( Match( "*" ) )
                    value = ToDecimal( value ) * ToDecimal( ParseUnary() );
                else if ( Match( "/" ) )
                    value = Divide( ToDecimal( value ), ToDecimal( ParseUnary() ) );
                else if ( Match( "%" ) )
                    value = ToDecimal( value ) % ToDecimal( ParseUnary() );
                else
                    return value;
            }
        }

        private object ParseUnary()
        {
            SkipWhiteSpace();

            if ( Match( "!" ) )
                return !ToBoolean( ParseUnary() );

            if ( Match( "-" ) )
                return -ToDecimal( ParseUnary() );

            return ParsePrimary();
        }

        private object ParsePrimary()
        {
            SkipWhiteSpace();

            if ( Match( "(" ) )
            {
                var value = ParseConditional();
                Expect( ")" );
                return value;
            }

            if ( Peek() == '"' || Peek() == '\'' )
                return ParseString();

            if ( Peek() == '{' )
                return ParseFieldToken();

            if ( char.IsDigit( Peek() ) )
                return ParseNumber();

            if ( IsIdentifierStart( Peek() ) )
                return ParseIdentifierOrFunction();

            return null;
        }

        private object ParseFieldToken()
        {
            Expect( "{" );
            var start = position;

            while ( position < formula.Length && formula[position] != '}' )
                position++;

            var fieldPath = formula[start..position].Trim();
            Expect( "}" );

            return ResolveFieldValue( fieldPath );
        }

        private object ParseIdentifierOrFunction()
        {
            var identifier = ParseIdentifier();
            SkipWhiteSpace();

            if ( !Match( "(" ) )
            {
                if ( string.Equals( identifier, "null", StringComparison.OrdinalIgnoreCase ) )
                    return null;

                if ( string.Equals( identifier, "true", StringComparison.OrdinalIgnoreCase ) )
                    return true;

                if ( string.Equals( identifier, "false", StringComparison.OrdinalIgnoreCase ) )
                    return false;

                return ResolveFieldValue( identifier );
            }

            var arguments = new List<FormulaArgument>();

            SkipWhiteSpace();

            if ( !Match( ")" ) )
            {
                do
                {
                    arguments.Add( ParseArgument() );
                    SkipWhiteSpace();
                }
                while ( Match( "," ) );

                Expect( ")" );
            }

            return EvaluateFunction( identifier, arguments );
        }

        private FormulaArgument ParseArgument()
        {
            SkipWhiteSpace();

            if ( Peek() == '{' )
            {
                var tokenStart = position;
                var value = ParseFieldToken();
                var token = formula[tokenStart..position].Trim();

                return new( value, token.Length > 1 ? token[1..^1].Trim() : null );
            }

            return new( ParseConditional(), null );
        }

        private object EvaluateFunction( string name, IReadOnlyList<FormulaArgument> arguments )
        {
            var normalizedName = name?.Trim();

            if ( TryEvaluateAggregateFunction( normalizedName, arguments, out var aggregateValue ) )
                return aggregateValue;

            return normalizedName?.ToLowerInvariant() switch
            {
                "isnull" => arguments.Count == 0 || arguments[0].Value is null,
                "isnullorempty" => arguments.Count == 0 || string.IsNullOrEmpty( Convert.ToString( arguments[0].Value, CultureInfo.CurrentCulture ) ),
                "coalesce" => arguments.Select( argument => argument.Value ).FirstOrDefault( value => value is not null ),
                "contains" => Contains( arguments ),
                "startswith" => StartsWith( arguments ),
                "endswith" => EndsWith( arguments ),
                "upper" => arguments.Count == 0 ? null : Convert.ToString( arguments[0].Value, CultureInfo.CurrentCulture )?.ToUpper( CultureInfo.CurrentCulture ),
                "lower" => arguments.Count == 0 ? null : Convert.ToString( arguments[0].Value, CultureInfo.CurrentCulture )?.ToLower( CultureInfo.CurrentCulture ),
                "length" => arguments.Count == 0 ? 0 : Convert.ToString( arguments[0].Value, CultureInfo.CurrentCulture )?.Length ?? 0,
                "round" => Round( arguments ),
                "abs" => Math.Abs( ToDecimal( arguments.FirstOrDefault()?.Value ) ),
                "today" => DateTime.Today,
                "now" => DateTime.Now,
                _ => null,
            };
        }

        private bool TryEvaluateAggregateFunction( string name, IReadOnlyList<FormulaArgument> arguments, out object value )
        {
            value = null;

            if ( arguments.Count == 0 || string.IsNullOrWhiteSpace( arguments[0].FieldPath ) )
                return false;

            ReportAggregateFunction? function = name?.ToLowerInvariant() switch
            {
                "count" => ReportAggregateFunction.Count,
                "sum" => ReportAggregateFunction.Sum,
                "average" => ReportAggregateFunction.Average,
                "avg" => ReportAggregateFunction.Average,
                "min" => ReportAggregateFunction.Minimum,
                "minimum" => ReportAggregateFunction.Minimum,
                "max" => ReportAggregateFunction.Maximum,
                "maximum" => ReportAggregateFunction.Maximum,
                _ => null,
            };

            if ( function is null )
                return false;

            SplitAggregateFieldPath( function.Value, arguments[0].FieldPath, out var dataSource, out var field );
            value = ReportAggregateResolver.ResolveAggregateValue( context.Definition, context.Data, context.Item, function.Value, dataSource, field );

            return true;
        }

        private void SplitAggregateFieldPath( ReportAggregateFunction function, string fieldPath, out string dataSource, out string field )
        {
            dataSource = null;
            field = fieldPath;

            if ( string.IsNullOrWhiteSpace( fieldPath ) )
                return;

            var separatorIndex = fieldPath.LastIndexOf( ".", StringComparison.Ordinal );

            if ( separatorIndex <= 0 || separatorIndex >= fieldPath.Length - 1 )
            {
                if ( function == ReportAggregateFunction.Count )
                {
                    dataSource = fieldPath;
                    field = null;
                }

                return;
            }

            dataSource = fieldPath[..separatorIndex];
            field = fieldPath[( separatorIndex + 1 )..];
        }

        private object ResolveFieldValue( string fieldPath )
        {
            return ReportExpressionResolver.ResolveValue( context.Definition, context.Data, context.Item, fieldPath, context.Section?.DataSource );
        }

        private string ParseIdentifier()
        {
            var start = position;

            while ( position < formula.Length && IsIdentifierPart( formula[position] ) )
                position++;

            return formula[start..position];
        }

        private object ParseNumber()
        {
            var start = position;

            while ( position < formula.Length && ( char.IsDigit( formula[position] ) || formula[position] == '.' ) )
                position++;

            return decimal.TryParse( formula[start..position], NumberStyles.Number, CultureInfo.InvariantCulture, out var value )
                ? value
                : 0m;
        }

        private string ParseString()
        {
            var quote = formula[position++];
            var result = string.Empty;

            while ( position < formula.Length )
            {
                var character = formula[position++];

                if ( character == quote )
                    break;

                if ( character == '\\' && position < formula.Length )
                    character = formula[position++];

                result += character;
            }

            return result;
        }

        private void Expect( string token )
        {
            SkipWhiteSpace();

            if ( !Match( token ) )
                throw new InvalidOperationException( $"Expected '{token}'." );
        }

        private bool Match( string token )
        {
            SkipWhiteSpace();

            if ( position + token.Length > formula.Length )
                return false;

            if ( !string.Equals( formula.Substring( position, token.Length ), token, StringComparison.Ordinal ) )
                return false;

            position += token.Length;

            return true;
        }

        private char Peek()
        {
            return position < formula.Length ? formula[position] : '\0';
        }

        private void SkipWhiteSpace()
        {
            while ( position < formula.Length && char.IsWhiteSpace( formula[position] ) )
                position++;
        }

        private static bool Contains( IReadOnlyList<FormulaArgument> arguments )
        {
            if ( arguments.Count < 2 )
                return false;

            var value = Convert.ToString( arguments[0].Value, CultureInfo.CurrentCulture );
            var otherValue = Convert.ToString( arguments[1].Value, CultureInfo.CurrentCulture );

            return ( value?.IndexOf( otherValue, StringComparison.CurrentCultureIgnoreCase ) >= 0 ) == true;
        }

        private static int Compare( object value, object otherValue )
        {
            if ( value is null && otherValue is null )
                return 0;

            if ( value is null )
                return -1;

            if ( otherValue is null )
                return 1;

            if ( TryGetDecimal( value, out var number ) && TryGetDecimal( otherValue, out var otherNumber ) )
                return number.CompareTo( otherNumber );

            if ( value is IComparable comparable )
                return comparable.CompareTo( otherValue );

            return string.Compare( Convert.ToString( value, CultureInfo.CurrentCulture ), Convert.ToString( otherValue, CultureInfo.CurrentCulture ), StringComparison.CurrentCulture );
        }

        private static decimal Divide( decimal value, decimal otherValue )
        {
            return otherValue == 0 ? 0 : value / otherValue;
        }

        private static bool EndsWith( IReadOnlyList<FormulaArgument> arguments )
        {
            return arguments.Count >= 2
                && Convert.ToString( arguments[0].Value, CultureInfo.CurrentCulture )?.EndsWith( Convert.ToString( arguments[1].Value, CultureInfo.CurrentCulture ), StringComparison.CurrentCultureIgnoreCase ) == true;
        }

        private static string FormatValue( object value )
        {
            return Convert.ToString( value, CultureInfo.CurrentCulture );
        }

        private static bool IsIdentifierPart( char character )
        {
            return IsIdentifierStart( character ) || char.IsDigit( character ) || character == '.';
        }

        private static bool IsIdentifierStart( char character )
        {
            return char.IsLetter( character ) || character == '_';
        }

        private static object Round( IReadOnlyList<FormulaArgument> arguments )
        {
            if ( arguments.Count == 0 )
                return 0m;

            var decimals = arguments.Count > 1 ? Convert.ToInt32( ToDecimal( arguments[1].Value ) ) : 0;

            return Math.Round( ToDecimal( arguments[0].Value ), decimals );
        }

        private static bool StartsWith( IReadOnlyList<FormulaArgument> arguments )
        {
            return arguments.Count >= 2
                && Convert.ToString( arguments[0].Value, CultureInfo.CurrentCulture )?.StartsWith( Convert.ToString( arguments[1].Value, CultureInfo.CurrentCulture ), StringComparison.CurrentCultureIgnoreCase ) == true;
        }

        private static bool ToBoolean( object value )
        {
            if ( value is bool boolValue )
                return boolValue;

            if ( value is null )
                return false;

            if ( TryGetDecimal( value, out var number ) )
                return number != 0;

            return bool.TryParse( Convert.ToString( value, CultureInfo.CurrentCulture ), out var parsedValue ) && parsedValue;
        }

        private static decimal ToDecimal( object value )
        {
            return TryGetDecimal( value, out var number ) ? number : 0;
        }

        private static bool TryGetDecimal( object value, out decimal number )
        {
            try
            {
                number = value is null ? 0 : Convert.ToDecimal( value, CultureInfo.CurrentCulture );
                return value is not null;
            }
            catch
            {
                number = 0;
                return false;
            }
        }

        #endregion
    }

    private sealed record FormulaArgument( object Value, string FieldPath );

    #endregion
}