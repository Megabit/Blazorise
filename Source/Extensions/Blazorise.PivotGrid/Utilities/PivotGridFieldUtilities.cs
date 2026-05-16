#region Using directives
using System;
using System.Linq.Expressions;
#endregion

namespace Blazorise.PivotGrid.Utilities;

internal static class PivotGridFieldUtilities
{
    internal static Type GetFieldValueType<TItem>( string fieldName )
    {
        if ( string.IsNullOrWhiteSpace( fieldName ) )
            return typeof( object );

        try
        {
            ParameterExpression item = Expression.Parameter( typeof( TItem ), "item" );
            Expression expression = PivotGridExpressionCompiler.GetSafePropertyOrFieldExpression( item, fieldName );

            return Nullable.GetUnderlyingType( expression.Type ) ?? expression.Type;
        }
        catch
        {
            return typeof( object );
        }
    }
}