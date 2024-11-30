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
    /// Applies all the DataGrid filters to the queryable data.
    /// </summary>
    /// <typeparam name="TItem">The TItem</typeparam>
    /// <param name="data">The Data to be queried</param>
    /// <param name="dataGridColumns">The DataGrid Columns Info</param>
    /// <param name="page">Optionally provide the page number</param>
    /// <param name="pageSize">Optionally provide the page size</param>
    /// <returns></returns>
    public static IQueryable<TItem> ApplyDataGridFilters<TItem>( this IQueryable<TItem> data, IEnumerable<DataGridColumnInfo> dataGridColumns, int page = 0, int pageSize = 0 )
    {
        return data.ApplyDataGridSort( dataGridColumns ).ApplyDataGridSearch( dataGridColumns ).ApplyDataGridPaging( page, pageSize );
    }

    /// <summary>
    /// Applies the search filter to the queryable data.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="data">The Data to be queried</param>
    /// <param name="dataGridColumns">The DataGrid Columns Info</param>
    /// <returns></returns>
    public static IQueryable<TItem> ApplyDataGridSearch<TItem>( this IQueryable<TItem> data, IEnumerable<DataGridColumnInfo> dataGridColumns )
    {
        if ( dataGridColumns.IsNullOrEmpty() )
            return data;

        foreach ( var column in dataGridColumns.Where( x => !string.IsNullOrWhiteSpace( x.SearchValue?.ToString() ) ) )
        {
            var filterMethod = column.FilterMethod ?? DataGridColumnFilterMethod.Contains;

            switch ( filterMethod )
            {
                case DataGridColumnFilterMethod.Contains:
                    data = data.Where( GetWhereContainsExpression<TItem>( column.Field, column.SearchValue.ToString() ) );
                    break;
                case DataGridColumnFilterMethod.StartsWith:
                    data = data.Where( GetWhereStartsWithExpression<TItem>( column.Field, column.SearchValue.ToString() ) );
                    break;
                case DataGridColumnFilterMethod.EndsWith:
                    data = data.Where( GetWhereEndsWithExpression<TItem>( column.Field, column.SearchValue.ToString() ) );
                    break;
                case DataGridColumnFilterMethod.Equals:
                    data = data.Where( GetWhereEqualsExpression<TItem>( column.Field, column.SearchValue.ToString() ) );
                    break;
                case DataGridColumnFilterMethod.NotEquals:
                    data = data.Where( GetWhereNotEqualsExpression<TItem>( column.Field, column.SearchValue.ToString() ) );
                    break;
                case DataGridColumnFilterMethod.LessThan:
                    if ( column.SearchValue is null )
                        break;

                    if ( column.ColumnType == DataGridColumnType.Numeric )
                    {
                        if ( column.ValueType == typeof( decimal ) || column.ValueType == typeof( decimal? ) )
                            data = data.Where( GetWhereLessThanExpression<TItem, decimal>( column.Field, decimal.Parse( column.SearchValue.ToString() ) ) );

                        if ( column.ValueType == typeof( double ) || column.ValueType == typeof( double? ) )
                            data = data.Where( GetWhereLessThanExpression<TItem, double>( column.Field, double.Parse( column.SearchValue.ToString() ) ) );

                        if ( column.ValueType == typeof( float ) || column.ValueType == typeof( float? ) )
                            data = data.Where( GetWhereLessThanExpression<TItem, float>( column.Field, float.Parse( column.SearchValue.ToString() ) ) );

                        if ( column.ValueType == typeof( int ) || column.ValueType == typeof( int? ) )
                            data = data.Where( GetWhereLessThanExpression<TItem, int>( column.Field, int.Parse( column.SearchValue.ToString() ) ) );

                        if ( column.ValueType == typeof( short ) || column.ValueType == typeof( short? ) )
                            data = data.Where( GetWhereLessThanExpression<TItem, short>( column.Field, short.Parse( column.SearchValue.ToString() ) ) );
                    }
                    else if ( column.ColumnType == DataGridColumnType.Date )
                    {
                        if ( column.ValueType == typeof( DateTime ) || column.ValueType == typeof( DateTime? ) )
                            data = data.Where( GetWhereLessThanExpression<TItem, DateTime>( column.Field, DateTime.Parse( column.SearchValue.ToString() ) ) );

                        if ( column.ValueType == typeof( DateTimeOffset ) || column.ValueType == typeof( DateTimeOffset? ) )
                            data = data.Where( GetWhereLessThanExpression<TItem, DateTimeOffset>( column.Field, DateTimeOffset.Parse( column.SearchValue.ToString() ) ) );

                        if ( column.ValueType == typeof( DateOnly ) || column.ValueType == typeof( DateOnly? ) )
                            data = data.Where( GetWhereLessThanExpression<TItem, DateOnly>( column.Field, DateOnly.Parse( column.SearchValue.ToString() ) ) );

                        if ( column.ValueType == typeof( TimeOnly ) || column.ValueType == typeof( TimeOnly? ) )
                            data = data.Where( GetWhereLessThanExpression<TItem, TimeOnly>( column.Field, TimeOnly.Parse( column.SearchValue.ToString() ) ) );

                        if ( column.ValueType == typeof( TimeSpan ) || column.ValueType == typeof( TimeSpan? ) )
                            data = data.Where( GetWhereLessThanExpression<TItem, TimeSpan>( column.Field, TimeSpan.Parse( column.SearchValue.ToString() ) ) );
                    }
                    break;
                case DataGridColumnFilterMethod.LessThanOrEqual:
                    if ( column.SearchValue is null )
                        break;

                    if ( column.ColumnType == DataGridColumnType.Numeric )
                    {
                        if ( column.ValueType == typeof( decimal ) || column.ValueType == typeof( decimal? ) )
                            data = data.Where( GetWhereLessThanOrEqualExpression<TItem, decimal>( column.Field, decimal.Parse( column.SearchValue.ToString() ) ) );

                        if ( column.ValueType == typeof( double ) || column.ValueType == typeof( double? ) )
                            data = data.Where( GetWhereLessThanOrEqualExpression<TItem, double>( column.Field, double.Parse( column.SearchValue.ToString() ) ) );

                        if ( column.ValueType == typeof( float ) || column.ValueType == typeof( float? ) )
                            data = data.Where( GetWhereLessThanOrEqualExpression<TItem, float>( column.Field, float.Parse( column.SearchValue.ToString() ) ) );

                        if ( column.ValueType == typeof( int ) || column.ValueType == typeof( int? ) )
                            data = data.Where( GetWhereLessThanOrEqualExpression<TItem, int>( column.Field, int.Parse( column.SearchValue.ToString() ) ) );

                        if ( column.ValueType == typeof( short ) || column.ValueType == typeof( short? ) )
                            data = data.Where( GetWhereLessThanOrEqualExpression<TItem, short>( column.Field, short.Parse( column.SearchValue.ToString() ) ) );
                    }
                    else if ( column.ColumnType == DataGridColumnType.Date )
                    {
                        if ( column.ValueType == typeof( DateTime ) || column.ValueType == typeof( DateTime? ) )
                            data = data.Where( GetWhereLessThanOrEqualExpression<TItem, DateTime>( column.Field, DateTime.Parse( column.SearchValue.ToString() ) ) );

                        if ( column.ValueType == typeof( DateTimeOffset ) || column.ValueType == typeof( DateTimeOffset? ) )
                            data = data.Where( GetWhereLessThanOrEqualExpression<TItem, DateTimeOffset>( column.Field, DateTimeOffset.Parse( column.SearchValue.ToString() ) ) );

                        if ( column.ValueType == typeof( DateOnly ) || column.ValueType == typeof( DateOnly? ) )
                            data = data.Where( GetWhereLessThanOrEqualExpression<TItem, DateOnly>( column.Field, DateOnly.Parse( column.SearchValue.ToString() ) ) );

                        if ( column.ValueType == typeof( TimeOnly ) || column.ValueType == typeof( TimeOnly? ) )
                            data = data.Where( GetWhereLessThanOrEqualExpression<TItem, TimeOnly>( column.Field, TimeOnly.Parse( column.SearchValue.ToString() ) ) );

                        if ( column.ValueType == typeof( TimeSpan ) || column.ValueType == typeof( TimeSpan? ) )
                            data = data.Where( GetWhereLessThanOrEqualExpression<TItem, TimeSpan>( column.Field, TimeSpan.Parse( column.SearchValue.ToString() ) ) );
                    }
                    break;
                case DataGridColumnFilterMethod.GreaterThan:
                    if ( column.SearchValue is null )
                        break;

                    if ( column.ColumnType == DataGridColumnType.Numeric )
                    {
                        if ( column.ValueType == typeof( decimal ) || column.ValueType == typeof( decimal? ) )
                            data = data.Where( GetWhereGreaterThanExpression<TItem, decimal>( column.Field, decimal.Parse( column.SearchValue.ToString() ) ) );

                        if ( column.ValueType == typeof( double ) || column.ValueType == typeof( double? ) )
                            data = data.Where( GetWhereGreaterThanExpression<TItem, double>( column.Field, double.Parse( column.SearchValue.ToString() ) ) );

                        if ( column.ValueType == typeof( float ) || column.ValueType == typeof( float? ) )
                            data = data.Where( GetWhereGreaterThanExpression<TItem, float>( column.Field, float.Parse( column.SearchValue.ToString() ) ) );

                        if ( column.ValueType == typeof( int ) || column.ValueType == typeof( int? ) )
                            data = data.Where( GetWhereGreaterThanExpression<TItem, int>( column.Field, int.Parse( column.SearchValue.ToString() ) ) );

                        if ( column.ValueType == typeof( short ) || column.ValueType == typeof( short? ) )
                            data = data.Where( GetWhereGreaterThanExpression<TItem, short>( column.Field, short.Parse( column.SearchValue.ToString() ) ) );
                    }
                    else if ( column.ColumnType == DataGridColumnType.Date )
                    {
                        if ( column.ValueType == typeof( DateTime ) || column.ValueType == typeof( DateTime? ) )
                            data = data.Where( GetWhereGreaterThanExpression<TItem, DateTime>( column.Field, DateTime.Parse( column.SearchValue.ToString() ) ) );

                        if ( column.ValueType == typeof( DateTimeOffset ) || column.ValueType == typeof( DateTimeOffset? ) )
                            data = data.Where( GetWhereGreaterThanExpression<TItem, DateTimeOffset>( column.Field, DateTimeOffset.Parse( column.SearchValue.ToString() ) ) );

                        if ( column.ValueType == typeof( DateOnly ) || column.ValueType == typeof( DateOnly? ) )
                            data = data.Where( GetWhereGreaterThanExpression<TItem, DateOnly>( column.Field, DateOnly.Parse( column.SearchValue.ToString() ) ) );

                        if ( column.ValueType == typeof( TimeOnly ) || column.ValueType == typeof( TimeOnly? ) )
                            data = data.Where( GetWhereGreaterThanExpression<TItem, TimeOnly>( column.Field, TimeOnly.Parse( column.SearchValue.ToString() ) ) );

                        if ( column.ValueType == typeof( TimeSpan ) || column.ValueType == typeof( TimeSpan? ) )
                            data = data.Where( GetWhereGreaterThanExpression<TItem, TimeSpan>( column.Field, TimeSpan.Parse( column.SearchValue.ToString() ) ) );
                    }
                    break;
                case DataGridColumnFilterMethod.GreaterThanOrEqual:
                    if ( column.SearchValue is null )
                        break;

                    if ( column.ColumnType == DataGridColumnType.Numeric )
                    {
                        if ( column.ValueType == typeof( decimal ) || column.ValueType == typeof( decimal? ) )
                            data = data.Where( GetWhereGreaterThanOrEqualExpression<TItem, decimal>( column.Field, decimal.Parse( column.SearchValue.ToString() ) ) );

                        if ( column.ValueType == typeof( double ) || column.ValueType == typeof( double? ) )
                            data = data.Where( GetWhereGreaterThanOrEqualExpression<TItem, double>( column.Field, double.Parse( column.SearchValue.ToString() ) ) );

                        if ( column.ValueType == typeof( float ) || column.ValueType == typeof( float? ) )
                            data = data.Where( GetWhereGreaterThanOrEqualExpression<TItem, float>( column.Field, float.Parse( column.SearchValue.ToString() ) ) );

                        if ( column.ValueType == typeof( int ) || column.ValueType == typeof( int? ) )
                            data = data.Where( GetWhereGreaterThanOrEqualExpression<TItem, int>( column.Field, int.Parse( column.SearchValue.ToString() ) ) );

                        if ( column.ValueType == typeof( short ) || column.ValueType == typeof( short? ) )
                            data = data.Where( GetWhereGreaterThanOrEqualExpression<TItem, short>( column.Field, short.Parse( column.SearchValue.ToString() ) ) );
                    }
                    else if ( column.ColumnType == DataGridColumnType.Date )
                    {
                        if ( column.ValueType == typeof( DateTime ) || column.ValueType == typeof( DateTime? ) )
                            data = data.Where( GetWhereGreaterThanOrEqualExpression<TItem, DateTime>( column.Field, DateTime.Parse( column.SearchValue.ToString() ) ) );

                        if ( column.ValueType == typeof( DateTimeOffset ) || column.ValueType == typeof( DateTimeOffset? ) )
                            data = data.Where( GetWhereGreaterThanOrEqualExpression<TItem, DateTimeOffset>( column.Field, DateTimeOffset.Parse( column.SearchValue.ToString() ) ) );

                        if ( column.ValueType == typeof( DateOnly ) || column.ValueType == typeof( DateOnly? ) )
                            data = data.Where( GetWhereGreaterThanOrEqualExpression<TItem, DateOnly>( column.Field, DateOnly.Parse( column.SearchValue.ToString() ) ) );

                        if ( column.ValueType == typeof( TimeOnly ) || column.ValueType == typeof( TimeOnly? ) )
                            data = data.Where( GetWhereGreaterThanOrEqualExpression<TItem, TimeOnly>( column.Field, TimeOnly.Parse( column.SearchValue.ToString() ) ) );

                        if ( column.ValueType == typeof( TimeSpan ) || column.ValueType == typeof( TimeSpan? ) )
                            data = data.Where( GetWhereGreaterThanOrEqualExpression<TItem, TimeSpan>( column.Field, TimeSpan.Parse( column.SearchValue.ToString() ) ) );
                    }
                    break;
                case DataGridColumnFilterMethod.Between:

                    if ( column.SearchValue is not object[] rangeSearchValues || rangeSearchValues.Length < 2 )
                        break;

                    var stringSearchValue1 = rangeSearchValues[0]?.ToString();
                    var stringSearchValue2 = rangeSearchValues[1]?.ToString();

                    if ( stringSearchValue1 is null || stringSearchValue2 is null )
                        break;

                    if ( column.ColumnType == DataGridColumnType.Numeric )
                    {
                        if ( column.ValueType == typeof( decimal ) || column.ValueType == typeof( decimal? ) )
                            data = data.Where( GetWhereBetweenExpression<TItem, decimal>( column.Field, decimal.Parse( stringSearchValue1 ), decimal.Parse( stringSearchValue2 ) ) );

                        if ( column.ValueType == typeof( double ) || column.ValueType == typeof( double? ) )
                            data = data.Where( GetWhereBetweenExpression<TItem, double>( column.Field, double.Parse( stringSearchValue1 ), double.Parse( stringSearchValue2 ) ) );

                        if ( column.ValueType == typeof( float ) || column.ValueType == typeof( float? ) )
                            data = data.Where( GetWhereBetweenExpression<TItem, float>( column.Field, float.Parse( stringSearchValue1 ), float.Parse( stringSearchValue2 ) ) );

                        if ( column.ValueType == typeof( int ) || column.ValueType == typeof( int? ) )
                            data = data.Where( GetWhereBetweenExpression<TItem, int>( column.Field, int.Parse( stringSearchValue1 ), int.Parse( stringSearchValue2 ) ) );

                        if ( column.ValueType == typeof( short ) || column.ValueType == typeof( short? ) )
                            data = data.Where( GetWhereBetweenExpression<TItem, short>( column.Field, short.Parse( stringSearchValue1 ), short.Parse( stringSearchValue2 ) ) );
                    }
                    else if ( column.ColumnType == DataGridColumnType.Date )
                    {
                        if ( column.ValueType == typeof( DateTime ) || column.ValueType == typeof( DateTime? ) )
                            data = data.Where( GetWhereBetweenExpression<TItem, DateTime>( column.Field, DateTime.Parse( stringSearchValue1 ), DateTime.Parse( stringSearchValue2 ) ) );

                        if ( column.ValueType == typeof( DateTimeOffset ) || column.ValueType == typeof( DateTimeOffset? ) )
                            data = data.Where( GetWhereBetweenExpression<TItem, DateTimeOffset>( column.Field, DateTimeOffset.Parse( stringSearchValue1 ), DateTimeOffset.Parse( stringSearchValue2 ) ) );

                        if ( column.ValueType == typeof( DateOnly ) || column.ValueType == typeof( DateOnly? ) )
                            data = data.Where( GetWhereBetweenExpression<TItem, DateOnly>( column.Field, DateOnly.Parse( stringSearchValue1 ), DateOnly.Parse( stringSearchValue2 ) ) );

                        if ( column.ValueType == typeof( TimeOnly ) || column.ValueType == typeof( TimeOnly? ) )
                            data = data.Where( GetWhereBetweenExpression<TItem, TimeOnly>( column.Field, TimeOnly.Parse( stringSearchValue1 ), TimeOnly.Parse( stringSearchValue2 ) ) );

                        if ( column.ValueType == typeof( TimeSpan ) || column.ValueType == typeof( TimeSpan? ) )
                            data = data.Where( GetWhereBetweenExpression<TItem, TimeSpan>( column.Field, TimeSpan.Parse( stringSearchValue1 ), TimeSpan.Parse( stringSearchValue2 ) ) );
                    }

                    break;
            }
        }

        return data;
    }

    /// <summary>
    /// Applies the sort filter to the queryable data.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="data">The Data to be queried</param>
    /// <param name="dataGridColumns">The DataGrid Columns Info</param>
    /// <returns></returns>
    public static IQueryable<TItem> ApplyDataGridSort<TItem>( this IQueryable<TItem> data, IEnumerable<DataGridColumnInfo> dataGridColumns )
    {
        if ( dataGridColumns.IsNullOrEmpty() )
            return data;

        var sortByColumns = dataGridColumns.Where( x => x.SortDirection != SortDirection.Default );
        var firstSort = true;
        if ( sortByColumns.Any() )
        {

            foreach ( var sortByColumn in sortByColumns.OrderBy( x => x.SortIndex ) )
            {
                var valueGetterExpression = string.IsNullOrWhiteSpace( sortByColumn.SortField ) ? CreateValueGetterExpression<TItem>( sortByColumn.Field ) : CreateValueGetterExpression<TItem>( sortByColumn.SortField );

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
        return data;
    }

    /// <summary>
    /// Applies the paging filter to the queryable data.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="data">The Data to be queried</param>
    /// <param name="page">The current page</param>
    /// <param name="pageSize">The page size</param>
    /// <returns></returns>
    public static IQueryable<TItem> ApplyDataGridPaging<TItem>( this IQueryable<TItem> data, int page, int pageSize )
    {
        if ( page > 0 && pageSize > 0 )
        {
            return data.Skip( ( page - 1 ) * pageSize ).Take( pageSize );
        }

        return data;
    }

    /// <summary>
    /// Applies all the DataGrid filters to the queryable data.
    /// </summary>
    /// <typeparam name="TItem">The TItem</typeparam>
    /// <param name="data">The Data to be queried</param>
    /// <param name="dataGridReadDataEventArgs">The DataGrid Read Data Event Arguments</param>
    /// <returns></returns>
    public static IQueryable<TItem> ApplyDataGridFilters<TItem>( this IQueryable<TItem> data, DataGridReadDataEventArgs<TItem> dataGridReadDataEventArgs )
    {
        return data.ApplyDataGridSort( dataGridReadDataEventArgs ).ApplyDataGridSearch( dataGridReadDataEventArgs ).ApplyDataGridPaging( dataGridReadDataEventArgs );
    }

    /// <summary>
    /// Applies the search filter to the queryable data.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="data">The Data to be queried</param>
    /// <param name="dataGridReadDataEventArgs">The DataGrid Read Data Event Arguments</param>
    /// <returns></returns>
    public static IQueryable<TItem> ApplyDataGridSearch<TItem>( this IQueryable<TItem> data, DataGridReadDataEventArgs<TItem> dataGridReadDataEventArgs  )
    {
        return data.ApplyDataGridSearch( dataGridReadDataEventArgs.Columns );
    }

    /// <summary>
    /// Applies the sort filter to the queryable data.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="data">The Data to be queried</param>
    /// <param name="dataGridReadDataEventArgs">The DataGrid Read Data Event Arguments</param>
    /// <returns></returns>
    public static IQueryable<TItem> ApplyDataGridSort<TItem>( this IQueryable<TItem> data, DataGridReadDataEventArgs<TItem> dataGridReadDataEventArgs )
    {
        return data.ApplyDataGridSort( dataGridReadDataEventArgs.Columns );
    }

    /// <summary>
    /// Applies the paging filter to the queryable data.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="data">The Data to be queried</param>
    /// <param name="dataGridReadDataEventArgs">The DataGrid Read Data Event Arguments</param>
    /// <returns></returns>
    public static IQueryable<TItem> ApplyDataGridPaging<TItem>( this IQueryable<TItem> data, DataGridReadDataEventArgs<TItem> dataGridReadDataEventArgs )
    {
        if ( dataGridReadDataEventArgs.ReadDataMode is DataGridReadDataMode.Virtualize )
            data = data.ApplyDataGridPaging( dataGridReadDataEventArgs.VirtualizeOffset + 1, dataGridReadDataEventArgs.VirtualizeCount );
        else if ( dataGridReadDataEventArgs.ReadDataMode is DataGridReadDataMode.Paging )
            data = data.ApplyDataGridPaging( dataGridReadDataEventArgs.Page, dataGridReadDataEventArgs.PageSize );

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

    private static Expression ConvertToStringExpression( Expression property )
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
        Expression body = Expression.Call(
            propertyExpression,
            typeof( string ).GetMethod( nameof( string.Contains ), new[] { typeof( string ) } )!,
            Expression.Constant( searchValue )
        );
        return body;
    }

    private static Expression StartsWithExpression( Expression propertyExpression, string searchValue )
    {
        Expression body = Expression.Call(
            propertyExpression,
            typeof( string ).GetMethod( nameof( string.StartsWith ), new[] { typeof( string ) } )!,
            Expression.Constant( searchValue )
        );
        return body;
    }

    private static Expression EndsWithExpression( Expression propertyExpression, string searchValue )
    {
        Expression body = Expression.Call(
            propertyExpression,
            typeof( string ).GetMethod( nameof( string.EndsWith ), new[] { typeof( string ) } )!,
            Expression.Constant( searchValue )
        );
        return body;
    }

    private static Expression EqualsWithExpression( Expression propertyExpression, string searchValue )
    {
        Expression body = Expression.Call(
            propertyExpression,
            typeof( string ).GetMethod( nameof( string.Equals ), new[] { typeof( string ) } )!,
            Expression.Constant( searchValue )
        );
        return body;
    }

    private static Expression NotEqualsWithExpression( Expression propertyExpression, string searchValue )
    {
        return Expression.IsFalse( EqualsWithExpression( propertyExpression, searchValue ) );
    }

    /// <summary>
    /// Builds a where expression. Where the source property contains the searchValue.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="sourceProperty"></param>
    /// <param name="searchValue"></param>
    /// <returns></returns>
    public static Expression<Func<TItem, bool>> GetWhereContainsExpression<TItem>(
                             string sourceProperty,
                             string searchValue )
    {
        var sourceParameterExpression = GetParameterExpression<TItem>();
        var sourcePropertyExpression = GetPropertyOrFieldExpression( sourceParameterExpression, sourceProperty );

        Expression convertSourcePropertyExpression = ConvertToStringExpression( sourcePropertyExpression );
        Expression body = ContainsExpression( convertSourcePropertyExpression, searchValue );
        return Expression.Lambda<Func<TItem, bool>>( body, sourceParameterExpression );
    }

    /// <summary>
    /// Builds a where expression. Where the source property starts with the searchValue.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="sourceProperty"></param>
    /// <param name="searchValue"></param>
    /// <returns></returns>
    public static Expression<Func<TItem, bool>> GetWhereStartsWithExpression<TItem>(
                         string sourceProperty,
                         string searchValue )
    {
        var sourceParameterExpression = GetParameterExpression<TItem>();
        var sourcePropertyExpression = GetPropertyOrFieldExpression( sourceParameterExpression, sourceProperty );

        Expression convertSourcePropertyExpression = ConvertToStringExpression( sourcePropertyExpression );
        Expression body = StartsWithExpression( convertSourcePropertyExpression, searchValue );
        return Expression.Lambda<Func<TItem, bool>>( body, sourceParameterExpression );
    }

    /// <summary>
    /// Builds a where expression. Where the source property ends with the searchValue.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="sourceProperty"></param>
    /// <param name="searchValue"></param>
    /// <returns></returns>
    public static Expression<Func<TItem, bool>> GetWhereEndsWithExpression<TItem>(
                         string sourceProperty,
                         string searchValue )
    {
        var sourceParameterExpression = GetParameterExpression<TItem>();
        var sourcePropertyExpression = GetPropertyOrFieldExpression( sourceParameterExpression, sourceProperty );

        Expression convertSourcePropertyExpression = ConvertToStringExpression( sourcePropertyExpression );
        Expression body = EndsWithExpression( convertSourcePropertyExpression, searchValue );
        return Expression.Lambda<Func<TItem, bool>>( body, sourceParameterExpression );
    }

    /// <summary>
    /// Builds a where expression. Where the source property equals the searchValue.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="sourceProperty"></param>
    /// <param name="searchValue"></param>
    /// <returns></returns>
    public static Expression<Func<TItem, bool>> GetWhereEqualsExpression<TItem>(
                     string sourceProperty,
                     string searchValue )
    {
        var sourceParameterExpression = GetParameterExpression<TItem>();
        var sourcePropertyExpression = GetPropertyOrFieldExpression( sourceParameterExpression, sourceProperty );

        Expression convertSourcePropertyExpression = ConvertToStringExpression( sourcePropertyExpression );
        Expression body = EqualsWithExpression( convertSourcePropertyExpression, searchValue );
        return Expression.Lambda<Func<TItem, bool>>( body, sourceParameterExpression );
    }

    /// <summary>
    /// Builds a where expression. Where the source property is not equal to the searchValue.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="sourceProperty"></param>
    /// <param name="searchValue"></param>
    /// <returns></returns>
    public static Expression<Func<TItem, bool>> GetWhereNotEqualsExpression<TItem>(
                 string sourceProperty,
                 string searchValue )
    {
        var sourceParameterExpression = GetParameterExpression<TItem>();
        var sourcePropertyExpression = GetPropertyOrFieldExpression( sourceParameterExpression, sourceProperty );

        Expression convertSourcePropertyExpression = ConvertToStringExpression( sourcePropertyExpression );
        Expression body = NotEqualsWithExpression( convertSourcePropertyExpression, searchValue );
        return Expression.Lambda<Func<TItem, bool>>( body, sourceParameterExpression );
    }

    /// <summary>
    /// Builds a where expression. Where the source property is less than the searchValue.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="sourceProperty"></param>
    /// <param name="searchValue"></param>
    /// <returns></returns>
    public static Expression<Func<TItem, bool>> GetWhereLessThanExpression<TItem, TValue>(
             string sourceProperty,
             TValue searchValue )
    {
        var sourceParameterExpression = GetParameterExpression<TItem>();
        var sourcePropertyExpression = GetPropertyOrFieldExpression( sourceParameterExpression, sourceProperty );

        //Note : Another option, would be to not convert and assume the searchValue is of the same type as the source property?
        var convertSourcePropertyExpression = Expression.Convert( sourcePropertyExpression, typeof( TValue ) );
        return Expression.Lambda<Func<TItem, bool>>( Expression.LessThan( convertSourcePropertyExpression, Expression.Constant( searchValue ) ), sourceParameterExpression );
    }

    /// <summary>
    /// Builds a where expression. Where the source property is less than or equal to the searchValue.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="sourceProperty"></param>
    /// <param name="searchValue"></param>
    /// <returns></returns>
    public static Expression<Func<TItem, bool>> GetWhereLessThanOrEqualExpression<TItem, TValue>(
         string sourceProperty,
         TValue searchValue )
    {
        var sourceParameterExpression = GetParameterExpression<TItem>();
        var sourcePropertyExpression = GetPropertyOrFieldExpression( sourceParameterExpression, sourceProperty );

        //Note : Another option, would be to not convert and assume the searchValue is of the same type as the source property?
        var convertSourcePropertyExpression = Expression.Convert( sourcePropertyExpression, typeof( TValue ) );
        return Expression.Lambda<Func<TItem, bool>>( Expression.LessThanOrEqual( convertSourcePropertyExpression, Expression.Constant( searchValue ) ), sourceParameterExpression );
    }

    /// <summary>
    /// Builds a where expression. Where the source property greater than the searchValue.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="sourceProperty"></param>
    /// <param name="searchValue"></param>
    /// <returns></returns>
    public static Expression<Func<TItem, bool>> GetWhereGreaterThanExpression<TItem, TValue>(
         string sourceProperty,
         TValue searchValue )
    {
        var sourceParameterExpression = GetParameterExpression<TItem>();
        var sourcePropertyExpression = GetPropertyOrFieldExpression( sourceParameterExpression, sourceProperty );

        //Note : Another option, would be to not convert and assume the searchValue is of the same type as the source property?
        var convertSourcePropertyExpression = Expression.Convert( sourcePropertyExpression, typeof( TValue ) );
        return Expression.Lambda<Func<TItem, bool>>( Expression.GreaterThan( convertSourcePropertyExpression, Expression.Constant( searchValue ) ), sourceParameterExpression );
    }

    /// <summary>
    /// Builds a where expression. Where the source property greater than or equal to the searchValue.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="sourceProperty"></param>
    /// <param name="searchValue"></param>
    /// <returns></returns>
    public static Expression<Func<TItem, bool>> GetWhereGreaterThanOrEqualExpression<TItem, TValue>(
         string sourceProperty,
         TValue searchValue )
    {
        var sourceParameterExpression = GetParameterExpression<TItem>();
        var sourcePropertyExpression = GetPropertyOrFieldExpression( sourceParameterExpression, sourceProperty );

        //Note : Another option, would be to not convert and assume the searchValue is of the same type as the source property?
        var convertSourcePropertyExpression = Expression.Convert( sourcePropertyExpression, typeof( TValue ) );
        return Expression.Lambda<Func<TItem, bool>>( Expression.GreaterThanOrEqual( convertSourcePropertyExpression, Expression.Constant( searchValue ) ), sourceParameterExpression );
    }

    /// <summary>
    /// Builds a where expression. Where the source property is between two provided searchValues.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="sourceProperty"></param>
    /// <param name="searchValue1"></param>
    /// <param name="searchValue2"></param>
    /// <returns></returns>
    public static Expression<Func<TItem, bool>> GetWhereBetweenExpression<TItem, TValue>(
     string sourceProperty,
     TValue searchValue1,
     TValue searchValue2 )
    {
        var sourceParameterExpression = GetParameterExpression<TItem>();
        var sourcePropertyExpression = GetPropertyOrFieldExpression( sourceParameterExpression, sourceProperty );

        //Note : Another option, would be to not convert and assume the searchValue is of the same type as the source property?
        var convertSourcePropertyExpression = Expression.Convert( sourcePropertyExpression, typeof( TValue ) );

        var value1GreaterThanOrEqualExpression = Expression.GreaterThanOrEqual( convertSourcePropertyExpression, Expression.Constant( searchValue1 ) );
        var value2LessThanOrEqualExpression = Expression.LessThanOrEqual( convertSourcePropertyExpression, Expression.Constant( searchValue2 ) );

        return Expression.Lambda<Func<TItem, bool>>( Expression.MakeBinary( ExpressionType.AndAlso, value1GreaterThanOrEqualExpression, value2LessThanOrEqualExpression ), sourceParameterExpression );
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
