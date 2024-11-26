using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components.Forms;

namespace Blazorise.DataGrid.Utils;
public static class ExpressionCompiler
{
    /// <summary>
    /// Applies the DataGrid filter to the queryable data.
    /// </summary>
    /// <typeparam name="TItem">The TItem</typeparam>
    /// <param name="data">The Data to be queried</param>
    /// <param name="dataGridColumns">The DataGrid Columns Info</param>
    /// <param name="page">Optionally provide the page number</param>
    /// <param name="pageSize">Optionally provide the page size</param>
    /// <returns></returns>
    public static IQueryable<TItem> ApplyDataGridFilter<TItem>( this IQueryable<TItem> data, IEnumerable<DataGridColumnInfo> dataGridColumns, int page = 0, int pageSize = 0 )
    {
        if ( dataGridColumns.IsNullOrEmpty() )
            return data;


        var sortByColumns = dataGridColumns.Where( x => x.SortDirection != SortDirection.Default );
        var firstSort = true;
        if ( sortByColumns.Any() )
        {

            foreach ( var sortByColumn in sortByColumns.OrderBy( x => x.SortIndex ) )
            {
                var valueGetterExpression = CreateValueGetterExpression<TItem>( sortByColumn.SortField );

                if ( firstSort )
                {
                    if ( sortByColumn.SortDirection == SortDirection.Ascending )
                        data = data.OrderBy( valueGetterExpression );
                    else
                        data = data.OrderByDescending( valueGetterExpression );

                    firstSort = false;
                }
                else
                {
                    if ( sortByColumn.SortDirection == SortDirection.Ascending )
                        data = ( data as IOrderedQueryable<TItem> ).ThenBy( valueGetterExpression );
                    else
                        data = ( data as IOrderedQueryable<TItem> ).ThenByDescending( valueGetterExpression );
                }
            }
        }
        foreach ( var column in dataGridColumns.Where( x => !string.IsNullOrWhiteSpace( x.SearchValue?.ToString() ) ) )
        {
            var filterMethod = column.FilterMethod ?? DataGridColumnFilterMethod.Contains;

            switch ( filterMethod )
            {
                case DataGridColumnFilterMethod.Contains:
                    data = data.Where( GetWhereContainsExpression<TItem>( column.Field, column.SearchValue.ToString() ) );
                    break;
            }

        }

        if ( page > 0 && pageSize > 0 )
        {
            data = data.Skip( ( page - 1 ) * pageSize ).Take( pageSize );
        }

        return data;
    }

    /// <summary>
    /// Creates the lambda expression that is suitable for usage with Blazor <see cref="FieldIdentifier"/>.
    /// </summary>
    /// <typeparam name="TItem">Type of model that contains the data-annotations.</typeparam>
    /// <typeparam name="TValue">Return type of validation field.</typeparam>
    /// <param name="item">An actual instance of the validation model.</param>
    /// <param name="fieldName">Field name to validate.</param>
    /// <returns>Expression compatible with <see cref="FieldIdentifier"/> parser.</returns>
    public static Expression<Func<TValue>> CreateValidationGetterExpression<TItem, TValue>( TItem item, string fieldName )
    {
        var parameter = Expression.Parameter( typeof( TItem ), "item" );
        var property = ExpressionCompiler.GetPropertyOrFieldExpression( parameter, fieldName );
        var path = fieldName.Split( '.' );

        //TODO : Couldn't this be done with an expression?
        Func<TItem, object> instanceGetter;

        if ( path.Length <= 1 )
            instanceGetter = ( item ) => item;
        else
            instanceGetter = FunctionCompiler.CreateValueGetter<TItem>( string.Join( '.', path.Take( path.Length - 1 ) ) );

        var convertExpression = Expression.MakeMemberAccess( Expression.Constant( instanceGetter( item ) ), property.Member );

        return Expression.Lambda<Func<TValue>>( convertExpression );
    }

    /// <summary>
    /// Builds an access expression for nested properties while checking for null values.
    /// </summary>
    /// <param name="item">Item that has the requested field name.</param>
    /// <param name="propertyOrFieldName">Item field name.</param>
    /// <returns>Returns the requested field if it exists.</returns>
    public static Expression GetSafePropertyOrFieldExpression( Expression item, string propertyOrFieldName )
    {
        if ( string.IsNullOrEmpty( propertyOrFieldName ) )
            throw new ArgumentException( $"{nameof( propertyOrFieldName )} is not specified." );

        var parts = propertyOrFieldName.Split( new char[] { '.' }, 2 );

        Expression field = null;

        MemberInfo memberInfo = GetSafeMember( item.Type, parts[0] );

        if ( memberInfo is PropertyInfo propertyInfo )
            field = Expression.Property( item, propertyInfo );
        else if ( memberInfo is FieldInfo fieldInfo )
            field = Expression.Field( item, fieldInfo );

        if ( field == null )
            throw new ArgumentException( $"Cannot detect the member of {item.Type}", propertyOrFieldName );

        field = Expression.Condition( Expression.Equal( item, Expression.Default( item.Type ) ),
            IsNullable( field.Type ) ? Expression.Constant( null, field.Type ) : Expression.Default( field.Type ),
            field );

        if ( parts.Length > 1 )
            field = GetSafePropertyOrFieldExpression( field, parts[1] );

        return field;
    }

    public static MemberExpression GetPropertyOrFieldExpression( Expression item, string propertyOrFieldName )
    {
        if ( string.IsNullOrEmpty( propertyOrFieldName ) )
            throw new ArgumentException( $"{nameof( propertyOrFieldName )} is not specified." );

        var parts = propertyOrFieldName.Split( new char[] { '.' }, 2 );

        MemberExpression field = null;

        MemberInfo memberInfo = GetSafeMember( item.Type, parts[0] );

        if ( memberInfo is PropertyInfo propertyInfo )
            field = Expression.Property( item, propertyInfo );
        else if ( memberInfo is FieldInfo fieldInfo )
            field = Expression.Field( item, fieldInfo );

        if ( field == null )
            throw new ArgumentException( $"Cannot detect the member of {item.Type}", propertyOrFieldName );

        if ( parts.Length > 1 )
            field = GetPropertyOrFieldExpression( field, parts[1] );

        return field;
    }

    public static Expression<Func<TItem, object>> CreateValueGetterExpression<TItem>( string fieldName )
    {
        var item = Expression.Parameter( typeof( TItem ), "item" );
        var property = GetSafePropertyOrFieldExpression( item, fieldName );
        return Expression.Lambda<Func<TItem, object>>( Expression.Convert( property, typeof( object ) ), item );
    }

    private static Expression ConvertExpression( Expression property )
    {
        Expression convert;
        var propInfo = (PropertyInfo)( property as MemberExpression ).Member;

        if ( IsNullable( propInfo.PropertyType ) && ( propInfo.PropertyType != typeof( string ) ) )
        {
            var hasValueExpression = Expression.Property( property, "HasValue" );
            var nullableValueExpression = Expression.Property( property, "Value" );

            var trueExpression = Expression.Call(
                         typeof( Convert ).GetMethod( nameof( Convert.ToString ), new[] { propInfo.PropertyType.GenericTypeArguments[0] } ),
                         nullableValueExpression );

            var falseExpression = Expression.Constant( "", typeof( string ) );
            convert = Expression.Condition( hasValueExpression,
                trueExpression,
                falseExpression );
        }
        else if ( propInfo.PropertyType.IsEnum )
        {
            var convertToInt = Expression.Convert( property, typeof( int ) );
            convert = Expression.Call(
                typeof( Convert ).GetMethod( nameof( Convert.ToString ), new[] { typeof( int ) } ),
                convertToInt );
        }
        else
        {
            convert = Expression.Call(
                typeof( Convert ).GetMethod( nameof( Convert.ToString ), new[] { propInfo.PropertyType } ),
                property );
        }

        return convert;
    }

    private static Expression ContainsExpression( Expression propertyExpression, string searchValue )
    {
        var searchValueExpression = Expression.Constant( searchValue );
        return ContainsExpression( propertyExpression, searchValueExpression );
    }

    private static Expression ContainsExpression( Expression propertyExpression, Expression searchValueExpression )
    {
        Expression body = Expression.Call(
            propertyExpression,
            typeof( string ).GetMethod( nameof( string.Contains ), new[] { typeof( string ) } )!,
            searchValueExpression
        );
        return body;
    }

    public static Expression<Func<TItem, bool>> GetWhereContainsExpression<TItem>(
                             string sourceProperty,
                             string searchValue )
    {
        var sourceParameterExpression = GetParameterExpression<TItem>();
        var propertyExpression = GetPropertyOrFieldExpression( sourceParameterExpression, sourceProperty );

        Expression convert = ConvertExpression( propertyExpression );
        Expression body = ContainsExpression( convert, searchValue );
        return Expression.Lambda<Func<TItem, bool>>( body, sourceParameterExpression );
    }


    /// <summary>
    /// Checks if requested type can bu nullable.
    /// </summary>
    /// <param name="type">Object type.</param>
    /// <returns></returns>
    private static bool IsNullable( Type type )
    {
        if ( type.IsClass )
            return true;

        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof( Nullable<> );
    }

    // inspired by: https://stackoverflow.com/questions/2496256/expression-tree-with-property-inheritance-causes-an-argument-exception
    private static MemberInfo GetSafeMember( Type type, string fieldName )
    {
        MemberInfo memberInfo = (MemberInfo)type.GetProperty( fieldName )
                                ?? type.GetField( fieldName );

        if ( memberInfo == null )
        {
            var baseTypesAndInterfaces = new List<Type>();

            if ( type.BaseType != null )
            {
                baseTypesAndInterfaces.Add( type.BaseType );
            }

            baseTypesAndInterfaces.AddRange( type.GetInterfaces() );

            foreach ( var baseType in baseTypesAndInterfaces )
            {
                memberInfo = GetSafeMember( baseType, fieldName );

                if ( memberInfo != null )
                    break;
            }
        }

        return memberInfo;
    }

    private static ParameterExpression GetParameterExpression<TItem>()
        => Expression.Parameter( typeof( TItem ), "item" );

}
