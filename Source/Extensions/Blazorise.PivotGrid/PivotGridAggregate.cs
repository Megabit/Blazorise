#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.PivotGrid;

/// <summary>
/// Defines an aggregate value in <see cref="PivotGrid{TItem}"/>.
/// </summary>
/// <typeparam name="TItem">Item type.</typeparam>
public class PivotGridAggregate<TItem> : BasePivotGridField<TItem>
{
    #region Methods

    /// <summary>
    /// Aggregates the supplied source items.
    /// </summary>
    public object Aggregate( IReadOnlyList<TItem> items )
    {
        if ( Aggregator is not null )
            return Aggregator.Invoke( items );

        return AggregateFunction switch
        {
            PivotGridAggregateFunction.Count => items.Count,
            PivotGridAggregateFunction.CountNonNull => items.Count( item => GetValue( item ) is not null ),
            PivotGridAggregateFunction.Average => Average( items ),
            PivotGridAggregateFunction.Min => Min( items ),
            PivotGridAggregateFunction.Max => Max( items ),
            _ => Sum( items ),
        };
    }

    /// <summary>
    /// Formats an aggregate value.
    /// </summary>
    public override string FormatValue( object value )
    {
        if ( value is null )
            return EmptyText;

        return base.FormatValue( value );
    }

    private object Sum( IReadOnlyList<TItem> items )
    {
        decimal sum = 0m;
        var hasValue = false;

        foreach ( var item in items )
        {
            if ( TryGetDecimalValue( item, out var value ) )
            {
                sum += value;
                hasValue = true;
            }
        }

        return hasValue ? (object)sum : null;
    }

    private object Average( IReadOnlyList<TItem> items )
    {
        decimal sum = 0m;
        var count = 0;

        foreach ( var item in items )
        {
            if ( TryGetDecimalValue( item, out var value ) )
            {
                sum += value;
                count++;
            }
        }

        return count > 0 ? (object)( sum / count ) : null;
    }

    private object Min( IReadOnlyList<TItem> items )
    {
        object current = null;

        foreach ( var value in items.Select( GetValue ).Where( x => x is not null ) )
        {
            if ( current is null || CompareValues( value, current ) < 0 )
                current = value;
        }

        return current;
    }

    private object Max( IReadOnlyList<TItem> items )
    {
        object current = null;

        foreach ( var value in items.Select( GetValue ).Where( x => x is not null ) )
        {
            if ( current is null || CompareValues( value, current ) > 0 )
                current = value;
        }

        return current;
    }

    private bool TryGetDecimalValue( TItem item, out decimal value )
    {
        var rawValue = GetValue( item );

        if ( rawValue is null )
        {
            value = 0m;
            return false;
        }

        try
        {
            value = Convert.ToDecimal( rawValue, CultureInfo.InvariantCulture );
            return true;
        }
        catch
        {
            value = 0m;
            return false;
        }
    }

    private static int CompareValues( object left, object right )
    {
        if ( left is IComparable comparable )
            return comparable.CompareTo( right );

        return string.Compare( left?.ToString(), right?.ToString(), StringComparison.CurrentCulture );
    }

    /// <inheritdoc />
    internal override int GetFieldStateHash()
        => HashCode.Combine(
            base.GetFieldStateHash(),
            AggregateFunction,
            Aggregator );

    #endregion

    #region Properties

    /// <inheritdoc />
    public override PivotGridFieldArea FieldArea => PivotGridFieldArea.Aggregate;

    /// <summary>
    /// Defines which built-in aggregate function is used.
    /// </summary>
    [Parameter] public PivotGridAggregateFunction AggregateFunction { get; set; } = PivotGridAggregateFunction.Sum;

    /// <summary>
    /// Defines a custom aggregate function. When set, it takes precedence over <see cref="AggregateFunction"/>.
    /// </summary>
    [Parameter] public Func<IReadOnlyList<TItem>, object> Aggregator { get; set; }

    /// <summary>
    /// Defines custom aggregate cell template.
    /// </summary>
    [Parameter] public RenderFragment<PivotGridCellContext<TItem>> CellTemplate { get; set; }

    #endregion
}