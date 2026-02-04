#region Using directives
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.DataGrid.Utils;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid.Internal;

public partial class _DataGridCellEditValidation<TItem> : _DataGridCellEdit<TItem>
{
    #region Members

    private Expression<Func<string>> stringExpression;

    private Expression<Func<decimal>> decimalExpression;
    private Expression<Func<decimal?>> nullableDecimalExpression;

    private Expression<Func<double>> doubleExpression;
    private Expression<Func<double?>> nullableDoubleExpression;

    private Expression<Func<float>> floatExpression;
    private Expression<Func<float?>> nullableFloatExpression;

    private Expression<Func<int>> intExpression;
    private Expression<Func<int?>> nullableIntExpression;

    private Expression<Func<long>> longExpression;
    private Expression<Func<long?>> nullableLongExpression;

    private Expression<Func<bool>> boolExpression;
    private Expression<Func<bool?>> nullableBoolExpression;

    private Expression<Func<DateTime>> dateTimeExpression;
    private Expression<Func<DateTime?>> nullableDateTimeExpression;

    private Expression<Func<DateOnly>> dateOnlyExpression;
    private Expression<Func<DateOnly?>> nullableDateOnlyExpression;

    private Expression<Func<DateTimeOffset>> dateTimeOffsetExpression;
    private Expression<Func<DateTimeOffset?>> nullableDateTimeOffsetExpression;

    private Expression<Func<TimeSpan>> timeSpanExpression;
    private Expression<Func<TimeSpan?>> nullableTimeSpanExpression;

    private Expression<Func<TimeOnly>> timeOnlyExpression;
    private Expression<Func<TimeOnly?>> nullableTimeOnlyExpression;

    #endregion

    #region Methods

    protected override Task OnInitializedAsync()
    {
        if ( ValueType == typeof( string ) )
            stringExpression = FunctionCompiler.CreateValidationExpressionGetter<TItem, string>( ValidationItem, Field );
        else if ( ValueType == typeof( decimal ) )
            decimalExpression = FunctionCompiler.CreateValidationExpressionGetter<TItem, decimal>( ValidationItem, Field );
        else if ( ValueType == typeof( decimal? ) )
            nullableDecimalExpression = FunctionCompiler.CreateValidationExpressionGetter<TItem, decimal?>( ValidationItem, Field );
        else if ( ValueType == typeof( double ) )
            doubleExpression = FunctionCompiler.CreateValidationExpressionGetter<TItem, double>( ValidationItem, Field );
        else if ( ValueType == typeof( double? ) )
            nullableDoubleExpression = FunctionCompiler.CreateValidationExpressionGetter<TItem, double?>( ValidationItem, Field );
        else if ( ValueType == typeof( float ) )
            floatExpression = FunctionCompiler.CreateValidationExpressionGetter<TItem, float>( ValidationItem, Field );
        else if ( ValueType == typeof( float? ) )
            nullableFloatExpression = FunctionCompiler.CreateValidationExpressionGetter<TItem, float?>( ValidationItem, Field );
        else if ( ValueType == typeof( int ) )
            intExpression = FunctionCompiler.CreateValidationExpressionGetter<TItem, int>( ValidationItem, Field );
        else if ( ValueType == typeof( int? ) )
            nullableIntExpression = FunctionCompiler.CreateValidationExpressionGetter<TItem, int?>( ValidationItem, Field );
        else if ( ValueType == typeof( long ) )
            longExpression = FunctionCompiler.CreateValidationExpressionGetter<TItem, long>( ValidationItem, Field );
        else if ( ValueType == typeof( long? ) )
            nullableLongExpression = FunctionCompiler.CreateValidationExpressionGetter<TItem, long?>( ValidationItem, Field );
        else if ( ValueType == typeof( bool ) )
            boolExpression = FunctionCompiler.CreateValidationExpressionGetter<TItem, bool>( ValidationItem, Field );
        else if ( ValueType == typeof( bool? ) )
            nullableBoolExpression = FunctionCompiler.CreateValidationExpressionGetter<TItem, bool?>( ValidationItem, Field );
        else if ( ValueType == typeof( DateTime ) )
            dateTimeExpression = FunctionCompiler.CreateValidationExpressionGetter<TItem, DateTime>( ValidationItem, Field );
        else if ( ValueType == typeof( DateTime? ) )
            nullableDateTimeExpression = FunctionCompiler.CreateValidationExpressionGetter<TItem, DateTime?>( ValidationItem, Field );
        else if ( ValueType == typeof( DateOnly ) )
            dateOnlyExpression = FunctionCompiler.CreateValidationExpressionGetter<TItem, DateOnly>( ValidationItem, Field );
        else if ( ValueType == typeof( DateOnly? ) )
            nullableDateOnlyExpression = FunctionCompiler.CreateValidationExpressionGetter<TItem, DateOnly?>( ValidationItem, Field );
        else if ( ValueType == typeof( DateTimeOffset ) )
            dateTimeOffsetExpression = FunctionCompiler.CreateValidationExpressionGetter<TItem, DateTimeOffset>( ValidationItem, Field );
        else if ( ValueType == typeof( DateTimeOffset? ) )
            nullableDateTimeOffsetExpression = FunctionCompiler.CreateValidationExpressionGetter<TItem, DateTimeOffset?>( ValidationItem, Field );
        else if ( ValueType == typeof( TimeSpan ) )
            timeSpanExpression = FunctionCompiler.CreateValidationExpressionGetter<TItem, TimeSpan>( ValidationItem, Field );
        else if ( ValueType == typeof( TimeSpan? ) )
            nullableTimeSpanExpression = FunctionCompiler.CreateValidationExpressionGetter<TItem, TimeSpan?>( ValidationItem, Field );
        else if ( ValueType == typeof( TimeOnly ) )
            timeOnlyExpression = FunctionCompiler.CreateValidationExpressionGetter<TItem, TimeOnly>( ValidationItem, Field );
        else if ( ValueType == typeof( TimeOnly? ) )
            nullableTimeOnlyExpression = FunctionCompiler.CreateValidationExpressionGetter<TItem, TimeOnly?>( ValidationItem, Field );

        return base.OnInitializedAsync();
    }

    #endregion

    #region Properties

    [Parameter] public bool ShowValidationFeedback { get; set; }

    [Parameter] public string ValidationPattern { get; set; }

    #endregion
}