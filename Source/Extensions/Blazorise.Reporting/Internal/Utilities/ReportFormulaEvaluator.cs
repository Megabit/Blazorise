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

    internal static ReportFormulaValidationResult Validate( string formula, ReportFormulaContext context )
    {
        if ( string.IsNullOrWhiteSpace( formula ) )
            return new( true, "Formula is empty." );

        try
        {
            var parser = new Parser( formula, context, validationMode: true );

            object value = parser.Parse();

            return new( true, $"Formula is valid. Result: {Parser.FormatValidationValue( value )}." );
        }
        catch ( ReportFormulaValidationException exception )
        {
            return new( false, exception.Message, exception.Position, exception.Length );
        }
        catch ( Exception exception )
        {
            return new( false, exception.Message );
        }
    }

    #endregion

    #region Classes

    private sealed class Parser
    {
        #region Members

        private readonly string formula;

        private readonly ReportFormulaContext context;

        private readonly bool validationMode;

        private int position;

        #endregion

        #region Constructors

        internal Parser( string formula, ReportFormulaContext context, bool validationMode = false )
        {
            this.formula = formula;
            this.context = context ?? new();
            this.validationMode = validationMode;
        }

        #endregion

        #region Methods

        internal object Parse()
        {
            var value = ParseConditional();
            SkipWhiteSpace();

            if ( position < formula.Length )
                throw CreateValidationException( $"Unexpected token '{formula[position]}'.", position );

            return value;
        }

        private object ParseConditional()
        {
            if ( MatchKeyword( "if" ) )
                return ParseIfThenElse();

            var condition = ParseOr();
            SkipWhiteSpace();

            if ( !Match( "?" ) )
                return condition;

            var whenTrue = ParseConditional();
            Expect( ":" );
            var whenFalse = ParseConditional();

            return ToBoolean( condition ) ? whenTrue : whenFalse;
        }

        private object ParseIfThenElse()
        {
            object condition = ParseOr();
            ExpectKeyword( "then" );
            object whenTrue = ParseConditional();
            ExpectKeyword( "else" );
            object whenFalse = ParseConditional();

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

                object otherValue = ParseAnd();
                value = ToBoolean( value ) || ToBoolean( otherValue );
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

                object otherValue = ParseEquality();
                value = ToBoolean( value ) && ToBoolean( otherValue );
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

            if ( validationMode )
                throw CreateValidationException( "Expected expression.", position );

            return null;
        }

        private object ParseFieldToken()
        {
            int tokenStart = position;

            Expect( "{" );
            var start = position;

            while ( position < formula.Length && formula[position] != '}' )
                position++;

            var fieldPath = formula[start..position].Trim();
            Expect( "}" );

            return ResolveFieldValue( fieldPath, tokenStart, Math.Max( 1, position - tokenStart ) );
        }

        private object ParseIdentifierOrFunction()
        {
            int identifierStart = position;
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

                return ResolveFieldValue( identifier, identifierStart, identifier.Length );
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

            return EvaluateFunction( identifier, arguments, identifierStart );
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

        private object EvaluateFunction( string name, IReadOnlyList<FormulaArgument> arguments, int identifierStart )
        {
            var normalizedName = name?.Trim();

            if ( validationMode )
                ValidateFunctionArguments( normalizedName, arguments.Count, identifierStart );

            if ( TryEvaluateAggregateFunction( normalizedName, arguments, identifierStart, out var aggregateValue ) )
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
                _ => validationMode ? throw CreateValidationException( $"Unknown function '{name}'.", identifierStart, name?.Length ?? 1 ) : null,
            };
        }

        private void ValidateFunctionArguments( string name, int argumentCount, int identifierStart )
        {
            ( int Minimum, int Maximum ) argumentRange = name?.ToLowerInvariant() switch
            {
                "isnull" or "isnullorempty" or "upper" or "lower" or "length" or "abs" => ( 1, 1 ),
                "contains" or "startswith" or "endswith" => ( 2, 2 ),
                "coalesce" => ( 1, int.MaxValue ),
                "round" => ( 1, 2 ),
                "today" or "now" => ( 0, 0 ),
                "count" or "sum" or "average" or "avg" or "minimum" or "min" or "maximum" or "max" => ( 1, 1 ),
                _ => ( -1, -1 ),
            };

            if ( argumentRange.Minimum < 0
                || ( argumentCount >= argumentRange.Minimum && argumentCount <= argumentRange.Maximum ) )
            {
                return;
            }

            string expectedArguments = argumentRange.Maximum == int.MaxValue
                ? $"at least {argumentRange.Minimum} argument{( argumentRange.Minimum == 1 ? null : "s" )}"
                : argumentRange.Minimum == argumentRange.Maximum
                    ? $"{argumentRange.Minimum} argument{( argumentRange.Minimum == 1 ? null : "s" )}"
                    : $"between {argumentRange.Minimum} and {argumentRange.Maximum} arguments";

            throw CreateValidationException(
                $"Function '{name}' expects {expectedArguments}, but received {argumentCount}.",
                identifierStart,
                name?.Length ?? 1 );
        }

        private bool TryEvaluateAggregateFunction( string name, IReadOnlyList<FormulaArgument> arguments, int identifierStart, out object value )
        {
            value = null;

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

            if ( arguments.Count == 0 || string.IsNullOrWhiteSpace( arguments[0].FieldPath ) )
            {
                if ( validationMode )
                {
                    throw CreateValidationException(
                        $"Aggregate function '{name}' requires a report field reference.",
                        identifierStart,
                        name?.Length ?? 1 );
                }

                return true;
            }

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

        private object ResolveFieldValue( string fieldPath, int tokenStart, int tokenLength )
        {
            if ( validationMode )
            {
                if ( TryResolveValidationFieldValue( fieldPath, out object validationValue ) )
                    return validationValue;

                throw CreateValidationException( $"Unknown field '{{{fieldPath}}}'.", tokenStart, tokenLength );
            }

            return ReportExpressionResolver.ResolveValue( context.Definition, context.Data, context.Item, fieldPath, context.Section?.DataSource, context.RunningTotals );
        }

        private bool TryResolveValidationFieldValue( string fieldPath, out object value )
        {
            value = null;

            if ( string.IsNullOrWhiteSpace( fieldPath ) )
                return false;

            if ( ReportSpecialFieldResolver.IsSpecialField( fieldPath ) )
            {
                value = ReportExpressionResolver.ResolveValue( context.Definition, context.Data, context.Item, fieldPath, context.Section?.DataSource );
                return true;
            }

            if ( ReportFormulaFieldResolver.IsFormulaField( context.Definition, fieldPath ) )
            {
                value = string.Empty;
                return true;
            }

            if ( ReportRunningTotalResolver.IsRunningTotalField( context.Definition, fieldPath ) )
            {
                value = 0m;
                return true;
            }

            if ( TryResolveValidationDataSourceFieldValue( fieldPath, out value ) )
                return true;

            if ( ReportDataSourceExplorer.TryResolveFieldType( context.Definition, context.Data, context.Section?.DataSource, fieldPath, out Type sectionFieldType ) )
            {
                value = CreateValidationValue( sectionFieldType );
                return true;
            }

            if ( ReportDataSourceExplorer.TryResolveFieldType( context.Definition, context.Data, null, fieldPath, out Type reportFieldType ) )
            {
                value = CreateValidationValue( reportFieldType );
                return true;
            }

            return TryResolveCurrentItemFieldValue( fieldPath, out value );
        }

        private bool TryResolveValidationDataSourceFieldValue( string fieldPath, out object value )
        {
            value = null;

            if ( !ReportDataSourceExplorer.TryResolveField( context.ValidationDataSources, fieldPath, out ReportDesignerFieldNode field ) )
                return false;

            value = CreateValidationValue( field.DataType );
            return true;
        }

        private bool TryResolveCurrentItemFieldValue( string fieldPath, out object value )
        {
            value = null;

            if ( context.Item is null )
                return false;

            foreach ( var candidate in GetCurrentItemFieldCandidates( fieldPath ) )
            {
                if ( ReportDataResolver.TryResolvePathValue( context.Item, candidate, out value ) )
                    return true;
            }

            return false;
        }

        private static object CreateValidationValue( Type dataType )
        {
            dataType = Nullable.GetUnderlyingType( dataType ) ?? dataType;

            if ( dataType is null )
                return null;

            if ( dataType == typeof( string ) )
                return string.Empty;

            if ( dataType == typeof( DateTime ) )
                return DateTime.Today;

            if ( dataType == typeof( DateTimeOffset ) )
                return DateTimeOffset.Now;

            if ( dataType == typeof( TimeSpan ) )
                return TimeSpan.Zero;

            if ( dataType == typeof( Guid ) )
                return Guid.Empty;

            return dataType.IsValueType
                ? Activator.CreateInstance( dataType )
                : null;
        }

        private IEnumerable<string> GetCurrentItemFieldCandidates( string fieldPath )
        {
            yield return fieldPath;

            string dataSource = context.Section?.DataSource;

            if ( string.IsNullOrWhiteSpace( dataSource ) )
                yield break;

            string dataSourcePrefix = $"{dataSource.Trim()}.";

            if ( fieldPath.StartsWith( dataSourcePrefix, StringComparison.OrdinalIgnoreCase ) )
                yield return fieldPath[dataSourcePrefix.Length..];

            string dataSourceLeaf = dataSource.Split( '.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries ).LastOrDefault();

            if ( string.IsNullOrWhiteSpace( dataSourceLeaf ) )
                yield break;

            string dataSourceLeafPrefix = $"{dataSourceLeaf}.";

            if ( !string.Equals( dataSourceLeafPrefix, dataSourcePrefix, StringComparison.OrdinalIgnoreCase )
                && fieldPath.StartsWith( dataSourceLeafPrefix, StringComparison.OrdinalIgnoreCase ) )
            {
                yield return fieldPath[dataSourceLeafPrefix.Length..];
            }
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

            string numberToken = formula[start..position];

            if ( validationMode
                && ( numberToken.EndsWith( ".", StringComparison.Ordinal ) || numberToken.Count( character => character == '.' ) > 1 ) )
            {
                throw CreateValidationException( $"Invalid number '{numberToken}'.", start, position - start );
            }

            if ( decimal.TryParse( numberToken, NumberStyles.Number, CultureInfo.InvariantCulture, out var value ) )
                return value;

            if ( validationMode )
                throw CreateValidationException( "Invalid number.", start, position - start );

            return 0m;
        }

        private string ParseString()
        {
            int stringStart = position;
            var quote = formula[position++];
            var result = string.Empty;
            var closed = false;

            while ( position < formula.Length )
            {
                var character = formula[position++];

                if ( character == quote )
                {
                    closed = true;
                    break;
                }

                if ( character == '\\' && position < formula.Length )
                    character = formula[position++];

                result += character;
            }

            if ( validationMode && !closed )
                throw CreateValidationException( "Expected closing string quote.", stringStart, position - stringStart );

            return result;
        }

        private void Expect( string token )
        {
            SkipWhiteSpace();

            if ( !Match( token ) )
                throw CreateValidationException( $"Expected '{token}'.", position );
        }

        private void ExpectKeyword( string keyword )
        {
            if ( !MatchKeyword( keyword ) )
                throw CreateValidationException( $"Expected '{keyword}'.", position );
        }

        private ReportFormulaValidationException CreateValidationException( string message, int errorPosition, int errorLength = 1 )
        {
            return new( message, errorPosition, errorLength );
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

        private bool MatchKeyword( string keyword )
        {
            SkipWhiteSpace();

            if ( position + keyword.Length > formula.Length )
                return false;

            if ( !string.Equals( formula.Substring( position, keyword.Length ), keyword, StringComparison.OrdinalIgnoreCase ) )
                return false;

            int nextPosition = position + keyword.Length;

            if ( nextPosition < formula.Length && IsIdentifierPart( formula[nextPosition] ) )
                return false;

            position = nextPosition;

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

        private int Compare( object value, object otherValue )
        {
            if ( value is null && otherValue is null )
                return 0;

            if ( value is null )
                return -1;

            if ( otherValue is null )
                return 1;

            if ( TryGetDecimal( value, out var number ) && TryGetDecimal( otherValue, out var otherNumber ) )
                return number.CompareTo( otherNumber );

            if ( value is IComparable comparable && IsComparableWith( value, otherValue ) )
                return comparable.CompareTo( otherValue );

            if ( validationMode && !CanCompareAsText( value, otherValue ) )
                throw new InvalidOperationException( $"Cannot compare {GetValidationTypeName( value )} with {GetValidationTypeName( otherValue )}." );

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

        internal static string FormatValidationValue( object value )
        {
            if ( value is null )
                return "null";

            if ( value is IEnumerable enumerable and not string and not IDictionary )
            {
                var values = new List<string>();

                foreach ( object item in enumerable )
                {
                    values.Add( FormatValue( item ) );

                    if ( values.Count == 3 )
                        break;
                }

                return values.Count == 0
                    ? "empty collection"
                    : $"{string.Join( ", ", values )}{( values.Count == 3 ? ", ..." : null )}";
            }

            return FormatValue( value );
        }

        private static string GetValidationTypeName( object value )
        {
            if ( value is null )
                return "null";

            if ( value is IEnumerable enumerable and not string and not IDictionary )
            {
                foreach ( object item in enumerable )
                {
                    return $"{item?.GetType().Name ?? "object"} collection";
                }

                return "collection";
            }

            return value.GetType().Name;
        }

        private static bool CanCompareAsText( object value, object otherValue )
        {
            return value is string || otherValue is string;
        }

        private static bool IsComparableWith( object value, object otherValue )
        {
            if ( value is null || otherValue is null )
                return true;

            Type valueType = value.GetType();
            Type otherValueType = otherValue.GetType();

            return valueType.IsAssignableFrom( otherValueType ) || otherValueType.IsAssignableFrom( valueType );
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

    private sealed class ReportFormulaValidationException : Exception
    {
        internal ReportFormulaValidationException( string message, int position, int length )
            : base( message )
        {
            Position = Math.Max( 0, position );
            Length = Math.Max( 1, length );
        }

        internal int Position { get; }

        internal int Length { get; }
    }

    private sealed record FormulaArgument( object Value, string FieldPath );

    #endregion
}