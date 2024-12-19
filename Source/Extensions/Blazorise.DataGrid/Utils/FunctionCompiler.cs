#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Components.Forms;
#endregion

namespace Blazorise.DataGrid.Utils;

public static class FunctionCompiler
{
    public static Func<TItem> CreateNewItem<TItem>()
    {
        return Expression.Lambda<Func<TItem>>( Expression.New( typeof( TItem ) ) ).Compile();
    }

    /// <summary>
    /// Creates the lambda expression that is suitable for usage with Blazor <see cref="FieldIdentifier"/>.
    /// </summary>
    /// <typeparam name="TItem">Type of model that contains the data-annotations.</typeparam>
    /// <typeparam name="TValue">Return type of validation field.</typeparam>
    /// <param name="item">An actual instance of the validation model.</param>
    /// <param name="fieldName">Field name to validate.</param>
    /// <returns>Expression compatible with <see cref="FieldIdentifier"/> parser.</returns>
    public static Expression<Func<TValue>> CreateValidationExpressionGetter<TItem, TValue>( TItem item, string fieldName )
    {
        return ExpressionCompiler.CreateValidationGetterExpression<TItem, TValue>( item, fieldName );
    }

    public static Func<TItem, object> CreateValueGetter<TItem>( string fieldName )
    {
        return ExpressionCompiler.CreateValueGetterExpression<TItem>( fieldName ).Compile();
    }

    public static Func<TItem, Type> CreateValueTypeGetter<TItem>( string fieldName )
    {
        var item = Expression.Parameter( typeof( TItem ), "item" );
        var property = ExpressionCompiler.GetPropertyOrFieldExpression( item, fieldName );
        return Expression.Lambda<Func<TItem, Type>>( Expression.Constant( property.Type ), item ).Compile();
    }

    public static Func<object> CreateDefaultValueByType<TItem>( string fieldName )
    {
        var item = Expression.Parameter( typeof( TItem ) );
        var property = ExpressionCompiler.GetPropertyOrFieldExpression( item, fieldName );
        return Expression.Lambda<Func<object>>( Expression.Convert( Expression.Default( property.Type ), typeof( object ) ) ).Compile();
    }

    public static Action<TItem, object> CreateValueSetter<TItem>( string fieldName )
    {
        var item = Expression.Parameter( typeof( TItem ), "item" );
        var value = Expression.Parameter( typeof( object ), "value" );

        // There's ne safe field setter because that should be a developer responsibility
        // to don't allow for null nested fields.
        var field = ExpressionCompiler.GetPropertyOrFieldExpression( item, fieldName );
        return Expression.Lambda<Action<TItem, object>>( Expression.Assign( field, Expression.Convert( value, field.Type ) ), item, value ).Compile();
    }
}